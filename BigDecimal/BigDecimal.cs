#nullable enable

namespace ExtendedNumerics;

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Exceptions;
using Helpers;
using Properties;
using Reflection;

/// <summary>
/// <para>Arbitrary precision decimal. All operations are exact, except for division.</para>
/// <para>Division never determines more digits than the given precision.</para>
/// <para>Based on code by Jan Christoph Bernack (http://stackoverflow.com/a/4524254 or jc.bernack at gmail.com)</para>
/// <para>Modified and extended by Adam White (https://csharpcodewhisperer.blogspot.com/)</para>
/// <para>Further modified by Rick Harker, Rick.Rick.Harker@gmail.com</para>
/// </summary>
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
[Immutable]
public readonly record struct BigDecimal : IComparable, IComparable<BigDecimal>, IComparable<Int32>, IComparable<Int32?>, IComparable<Decimal>, IComparable<Double>, IComparable<Single>
{
	private const String NullString = "(null)";

	static BigDecimal()
	{
		try
		{
			Pi = Parse(
				"3.14159265358979323846264338327950288419716939937510582097494459230781640628620899862803482534211706798214808651328230664709384460955058223172535940812848111745028410270193852110555964462294895493038196");
		}
		catch (Exception exception)
		{
			Debug.WriteLine(exception);
		}

		try
		{
			π = GetPiDigits();
		}
		catch (Exception exception)
		{
			Debug.WriteLine(exception);
		}

		try
		{
			E = Parse(
				"2.71828182845904523536028747135266249775724709369995957496696762772407663035354759457138217852516642749193200305992181741359662904357290033429526059563073813232862794349076323382988075319525101901157383");
		}
		catch (Exception exception)
		{
			Debug.WriteLine(exception);
		}
	}

	/// <summary>
	/// Private Constructor. This one bypasses <see cref="AlwaysTruncate"/> and <see cref="AlwaysNormalize"/> check and behavior.
	/// </summary>
	/// <param name="tuple"></param>
	private BigDecimal(Tuple<BigInteger, Int32> tuple)
	{
		this.Mantissa = tuple.Item1;
		this.Exponent = tuple.Item2;
	}

	public BigDecimal(BigInteger numerator, BigInteger denominator)
	{
		BigDecimal quotient = BigDecimal.Divide(new BigDecimal(numerator), new BigDecimal(denominator));
		this.Mantissa = quotient.Mantissa;
		this.Exponent = quotient.Exponent;
	}

	public BigDecimal(BigInteger mantissa, Int32 exponent = 0)
	{
		this.Mantissa = mantissa;
		this.Exponent = exponent;
		BigDecimal result;
		if (AlwaysTruncate)
		{
			result = Round(this, Precision);
			this.Mantissa = result.Mantissa;
			this.Exponent = result.Exponent;
		}
		if (AlwaysNormalize)
		{
			result = Normalize(this);
			this.Mantissa = result.Mantissa;
			this.Exponent = result.Exponent;
		}
	}

	public BigDecimal(Int32 value) : this(new BigInteger(value), 0) { }

	public BigDecimal(Double value)
	{
		if (Double.IsInfinity(value))
		{
			throw new OverflowException("BigDecimal cannot represent infinity.");
		}

		if (Double.IsNaN(value))
		{
			throw new NotFiniteNumberException($"{nameof(value)} is not a number (double.NaN).");
		}

		var mantissa = new BigInteger(value);
		var exponent = 0;
		Double scaleFactor = 1;
		while (Math.Abs(value * scaleFactor - Double.Parse(mantissa.ToString())) > 0)
		{
			exponent--;
			scaleFactor *= 10;
			mantissa = new BigInteger(value * scaleFactor);
		}

		this.Mantissa = mantissa;
		this.Exponent = exponent;
		BigDecimal result;
		if (AlwaysTruncate)
		{
			result = Round(this, Precision);
			this.Mantissa = result.Mantissa;
			this.Exponent = result.Exponent;
		}

		if (AlwaysNormalize)
		{
			result = Normalize(this);
			this.Mantissa = result.Mantissa;
			this.Exponent = result.Exponent;
		}
	}

	public BigDecimal(Single value)
	{
		if (Single.IsInfinity(value))
		{
			throw new OverflowException("BigDecimal cannot represent infinity.");
		}

		if (Single.IsNaN(value))
		{
			throw new NotFiniteNumberException($"{nameof(value)} is not a number (Single.NaN).");
		}

		var mantissa = new BigInteger(value);
		var exponent = 0;
		Single scaleFactor = 1;
		while (Math.Abs(value * scaleFactor - Single.Parse(mantissa.ToString())) > 0)
		{
			exponent--;
			scaleFactor *= 10;
			mantissa = new BigInteger(value * scaleFactor);
		}

		this.Mantissa = mantissa;
		this.Exponent = exponent;
		BigDecimal result;
		if (AlwaysTruncate)
		{
			result = Round(this, Precision);
			this.Mantissa = result.Mantissa;
			this.Exponent = result.Exponent;
		}

		if (AlwaysNormalize)
		{
			result = Normalize(this);
			this.Mantissa = result.Mantissa;
			this.Exponent = result.Exponent;
		}
	}

	/// <summary>Gets a value that represents the number 0 (zero).</summary>
	public static BigDecimal Ten => 10;

	/// <summary>Gets a value that represents the number 1 ().</summary>
	public static BigDecimal One => 1;

	/// <summary>Gets a value that represents the number 0 (zero).</summary>
	public static BigDecimal Zero => 0;

	/// <summary>Gets a value that represents the number 0.5.</summary>
	public static BigDecimal OneHalf => new BigDecimal(0.5d);

	/// <summary>Gets a value that represents the number -1 .</summary>
	public static BigDecimal MinusOne => new BigDecimal(-1);

	/// <summary>Gets a value that represents the number e, also called Euler's number.</summary>
	public static BigDecimal E { get; }

	/// <summary>Gets a value that represents the number Pi.</summary>
	public static BigDecimal Pi { get; }

	/// <summary>Gets a value that represents the number Pi.</summary>
	public static BigDecimal π { get; }

	private static BigInteger TenInt { get; } = new(10);

	private static NumberFormatInfo BigDecimalNumberFormatInfo { get; } = CultureInfo.InvariantCulture.NumberFormat;

	/// <summary>Sets the maximum precision of division operations. If AlwaysTruncate is set to true all operations are affected.</summary>
	public static Int32 Precision { get; set; } = 5000;

	/// <summary>
	/// Specifies whether the significant digits should be truncated to the given precision after each operation.	
	/// Setting this to true will tend to accumulate errors at the precision boundary after several arithmetic operations.
	/// Therefore, you should prefer using <see cref="Round(BigDecimal, int)"/> explicitly when you need it instead.
	/// This should generally be left disabled by default.
	/// This setting may be useful if you are running into memory or performance issues, as could conceivably be brought on by many operations on irrational numbers.
	/// </summary>
	public static Boolean AlwaysTruncate { get; set; } = false;

	/// <summary>TODO Describe this.</summary>
	public static Boolean AlwaysNormalize { get; set; } = true;

	/// <summary>The mantissa of the internal floating point number representation of this BigDecimal.</summary>
	public readonly BigInteger Mantissa;

	/// <summary>The exponent of the internal floating point number representation of this BigDecimal.</summary>
	public readonly Int32 Exponent;

	/// <summary>Gets a number that indicates the sign (negative, positive, or zero) of the current <see cref="BigDecimal" /> object. </summary>
	/// <returns>-1 if the value of this object is negative, 0 if the value of this object is zero or 1 if the value of this object is positive.</returns
	public Int32 Sign => this.GetSign();

	/// <summary>Gets the number of signifigant digits in <see cref="BigDecimal"/>.
	///Essentially tells you the number of digits in the mantisa.</summary>
	public Int32 SignifigantDigits => GetSignifigantDigits(this.Mantissa);

	public Int32 DecimalPlaces => PlacesRightOfDecimal(this);

	/// <summary>
	/// Gets the whole-number integer (positive or negative) value of this BigDecimal, so everything to the left of the decimal place.
	/// Equivalent to the Truncate function for a float.
	/// </summary>
	public BigInteger WholeValue => this.GetWholePart();

	public Int32 Length => GetSignifigantDigits(this.Mantissa) + this.Exponent;

	/// <summary>This method returns true if the BigDecimal is equal to zero, false otherwise.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Boolean IsZero() => this.Mantissa.IsZero;

	/// <summary>This method returns true if the BigDecimal is greater than zero, false otherwise.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Boolean IsPositve() => !this.IsZero() && !this.IsNegative();

	/// <summary>This method returns true if the BigDecimal is less than zero, false otherwise.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Boolean IsNegative() => this.Mantissa.Sign < 0;

	private static Lazy<BigDecimal> MaxBigDecimalForDecimal => new(() => Decimal.MaxValue);

	private static Lazy<BigDecimal> MaxBigDecimalForSingle => new(() => Single.MaxValue);

	private static Lazy<BigDecimal> MaxBigDecimalForDouble => new(() => Double.MaxValue);

	private static Lazy<BigDecimal> MaxBigDemicalForInt32 => new(() => Int32.MaxValue);

	private static Lazy<BigDecimal> MaxBigDemicalForUInt32 => new(() => UInt32.MaxValue);

	public Int32 CompareTo(BigDecimal other) =>
		this < other ? SortingOrder.Before :
		this > other ? SortingOrder.After : SortingOrder.Same;

	public Int32 CompareTo(Decimal other) =>
		this < other ? SortingOrder.Before :
		this > other ? SortingOrder.After : SortingOrder.Same;

	public Int32 CompareTo(Double other) =>
		this < other ? SortingOrder.Before :
		this > other ? SortingOrder.After : SortingOrder.Same;

	public Int32 CompareTo(Single other) =>
		this < other ? SortingOrder.Before :
		this > other ? SortingOrder.After : SortingOrder.Same;

	public Int32 CompareTo(Int32? other) =>
		other is null || this > other ? SortingOrder.After :
		this < other ? SortingOrder.Before : SortingOrder.Same;

	public Int32 CompareTo(Int32 other) =>
		this < other ? SortingOrder.Before :
		this > other ? SortingOrder.After : SortingOrder.Same;

	/// <summary>
	/// Compares the current instance with another object of the same type and returns
	/// an integer that indicates whether the current instance precedes, follows, or
	/// occurs in the same position in the sort order as the other object.
	/// </summary>
	/// <param name="obj"> An object to compare with this instance.</param>	
	/// <returns>
	/// A return value of less than zero means this instance precedes obj in the sort order.
	/// A return value of zero means  this instance occurs in the same position in the sort order as obj.
	/// A return value of greater than zero means this instance follows obj in the sort order.
	/// </returns>
	int IComparable.CompareTo(Object obj)
	{
		if (obj == null) { return SortingOrder.After; }
		if (!(obj is BigDecimal)) { throw new ArgumentException($"Argument must be of type {nameof(BigDecimal)}", nameof(obj)); }
		return CompareTo((BigDecimal)obj);
	}

	public Boolean Equals(BigDecimal? other) => Equals(this, other);

	/// <summary>
	/// Load the value of π up to 1 million digits.
	/// <para>Defaults to 512 digits.</para>
	/// </summary>
	/// <param name="digits"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	/// <exception cref="InvalidOperationException"></exception>
	public static BigDecimal GetPiDigits(Int32 digits = 512)
	{
		digits += "3.".Length;
		if (digits is <= 0 or > 1_000_000)
		{
			throw new ArgumentOutOfRangeException(nameof(digits));
		}

		var s = Resources.PiString.Substring(0, 2 + digits);

		if (String.IsNullOrWhiteSpace(s))
		{
			throw new InvalidOperationException($"Unable to load the value of π.[{digits}] from resources.");
		}

		return Parse(s);
	}

	/// <summary>Static equality test.</summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	/// <returns></returns>
	public static Boolean Equals(BigDecimal? left, BigDecimal? right)
	{
		if (left is null || right is null)
		{
			return false;
		}

		BigDecimal l = left.Value;
		BigDecimal r = right.Value;
		if (AlwaysTruncate)
		{
			l = Round(l, Precision);
			r = Round(r, Precision);
		}
		if (AlwaysNormalize)
		{
			l = Normalize(l);
			r = Normalize(r);
		}

		return l.Mantissa.Equals(r.Mantissa)
			&& l.Exponent.Equals(r.Exponent)
			&& l.Sign.Equals(r.Sign);
	}

	/// <summary>Converts the string representation of a decimal to the BigDecimal equivalent.</summary>
	public static BigDecimal Parse(Double input) => Parse(input.ToString(CultureInfo.CurrentCulture));

	/// <summary>Converts the string representation of a decimal to the BigDecimal equivalent.</summary>
	public static BigDecimal Parse(Decimal input) => Parse(input.ToString(CultureInfo.CurrentCulture));

	/// <summary>Converts the string representation of a decimal to the BigDecimal equivalent.</summary>
	/// <param name="input"></param>
	/// <returns></returns>
	public static BigDecimal Parse(String text)
	{
		String input = text.Trim();
		if (String.IsNullOrEmpty(input))
		{
			return BigInteger.Zero;
		}

		var exponent = 0;
		var isNegative = false;

		if (input.StartsWith(BigDecimalNumberFormatInfo.NegativeSign, StringComparison.OrdinalIgnoreCase))
		{
			isNegative = true;
			input = input.Replace(BigDecimalNumberFormatInfo.NegativeSign, String.Empty);
		}

		var posE = input.LastIndexOf('E') + 1;

		if (posE > 0)
		{
			var sE = input.Substring(posE);

			if (Int32.TryParse(sE, out exponent))
			{
				//Trim off the exponent
				input = input.Substring(0, posE - 1);
			}
		}

		if (input.Contains(BigDecimalNumberFormatInfo.NumberDecimalSeparator))
		{
			var decimalPlace = input.IndexOf(BigDecimalNumberFormatInfo.NumberDecimalSeparator, StringComparison.Ordinal);

			exponent += decimalPlace + 1 - input.Length;
			input = input.Replace(BigDecimalNumberFormatInfo.NumberDecimalSeparator, String.Empty);
		}

		var mantissa = BigInteger.Parse(input);
		if (isNegative)
		{
			mantissa = BigInteger.Negate(mantissa);
		}

		BigDecimal result = new BigDecimal(new Tuple<BigInteger, Int32>(mantissa, exponent));
		if (AlwaysTruncate)
		{
			result = Round(result, Precision);
		}
		if (AlwaysNormalize)
		{
			result = Normalize(result);
		}
		return result;
	}

	public static Int32 NumberOfDigits(BigInteger value) => (Int32)Math.Ceiling(BigInteger.Log10(value * value.Sign));

	/// <summary>Removes any trailing zeros on the mantissa, adjusts the exponent, and returns a new <see cref="BigDecimal" />.</summary>
	/// <param name="value"></param>
	[Pure]
	public static BigDecimal Normalize(BigDecimal value)
	{
		if (value.IsZero())
		{
			return value;
		}

		var s = value.Mantissa.ToString();
		var pos = s.LastIndexOf('0', s.Length - 1);

		if (pos < s.Length - 1)
		{
			return value;
		}

		var c = s[pos];

		while (pos > 0 && c == '0')
		{
			c = s[--pos]; //scan backwards to find the last not-0.
		}

		var mant = s.Substring(0, pos + 1);
		var zeros = s.Substring(pos + 1);

		if (BigInteger.TryParse(mant, out var mantissa))
		{
			return new BigDecimal(new Tuple<BigInteger, Int32>(mantissa, value.Exponent + zeros.Length));
		}

		return value;
	}

	/// <summary>Returns the zero-based index of the decimal point, if the BigDecimal were rendered as a string.</summary>
	public Int32 GetDecimalIndex() => GetDecimalIndex(this.Mantissa, this.Exponent);

	/// <summary>
	/// Returns the whole number integer part of the BigDecimal, dropping anything right of the decimal point. Essentially behaves like Math.Truncate(). For
	/// example, GetWholePart() would return 3 for Math.PI.
	/// </summary>
	[NeedsTesting]
	public BigInteger GetWholePart()
	{
		var resultString = String.Empty;
		var decimalString = this.ToString(BigDecimalNumberFormatInfo);

		var valueSplit = decimalString.Split(new Char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
		if (valueSplit.Length > 0)
		{
			resultString = valueSplit[0];
		}

		var posE = resultString.IndexOf("E", StringComparison.Ordinal);
		if (posE > 0)
		{
			resultString = resultString.Split(new Char[] { 'E' }, StringSplitOptions.RemoveEmptyEntries)[0];
		}

		return BigInteger.Parse(resultString);
	}

	/// <summary>Gets the fractional part of the BigDecimal, setting everything left of the decimal point to zero.</summary>
	[NeedsTesting] //TODO Needs tests on E notation, negative numbers, and extra small or large numbers.
	public BigDecimal GetFractionalPart()
	{
		var resultString = String.Empty;
		var decimalString = this.ToString();

		var valueSplit = decimalString.Split('.');
		if (valueSplit.Length == 1)
		{
			return Zero; //BUG Is this right?
		}

		if (valueSplit.Length == 2)
		{
			resultString = valueSplit[1];
		}

		var newmantissa = BigInteger.Parse(resultString.TrimStart('0'));
		var result = new BigDecimal(newmantissa, 0 - resultString.Length);
		return result;
	}

	private Int32 GetSign()
	{
		if (this.Mantissa.IsZero)
		{
			return 0;
		}

		if (this.Mantissa.Sign == 1)
		{
			return 1; //BUG Is this correct?
		}

		if (this.Exponent >= 0)
		{
			return -1; //BUG Is this correct?
		}

		var mant = this.Mantissa.ToString();
		var length = mant.Length + this.Exponent;
		return length switch
		{
			0 when Int32.TryParse(mant[0].ToString(), out var tenthsPlace) => tenthsPlace >= 5 ? 1 : 0,
			> 0 => 1,
			var _ => 0
		};
	}

	private static Int32 GetDecimalIndex(BigInteger mantissa, Int32 exponent)
	{
		var mantissaLength = mantissa.GetLength();
		if (mantissa.Sign < 0)
		{
			mantissaLength++;
		}

		return mantissaLength + exponent;
	}

	private static Int32 GetSignifigantDigits(BigInteger value) => value.GetSignifigantDigits();

	/// <summary>Returns the mantissa of value, aligned to the exponent of reference. Assumes the exponent of value is larger than of reference.</summary>
	private static BigInteger AlignExponent(BigDecimal value, BigDecimal reference) => value.Mantissa * BigInteger.Pow(TenInt, value.Exponent - reference.Exponent);

	public static implicit operator BigDecimal(BigInteger value) => new(value, 0);

	public static implicit operator BigDecimal(Byte value) => new(new BigInteger(value), 0);

	public static implicit operator BigDecimal(SByte value) => new(new BigInteger(value), 0);

	public static implicit operator BigDecimal(UInt32 value) => new(new BigInteger(value), 0);

	public static implicit operator BigDecimal(Int32 value) => new(new BigInteger(value), 0);

	public static implicit operator BigDecimal(UInt16 value) => new(new BigInteger(value), 0);

	public static implicit operator BigDecimal(Int16 value) => new(new BigInteger(value), 0);

	public static implicit operator BigDecimal(UInt64 value) => new(new BigInteger(value), 0);

	public static implicit operator BigDecimal(Int64 value) => new(new BigInteger(value), 0);

	public static implicit operator BigDecimal(Single value) => new(value);

	public static implicit operator BigDecimal(Decimal value)
	{
		var mantissa = new BigInteger(value);
		var exponent = 0;
		Decimal scaleFactor = 1;
		while (Decimal.Parse(mantissa.ToString()) != value * scaleFactor)
		{
			exponent--;
			scaleFactor *= 10;
			mantissa = new BigInteger(value * scaleFactor);
		}

		return new BigDecimal(mantissa, exponent);
	}

	public static implicit operator BigDecimal(Double value)
	{
		var s = value.ToString("G17", BigDecimalNumberFormatInfo); //G17 according to the Microsoft docs.
		return Parse(s);
	}

	public static explicit operator BigInteger(BigDecimal value)
	{
		var floored = Floor(value);
		return floored.Mantissa * BigInteger.Pow(10, floored.Exponent);
	}

	/*
	public static explicit operator BigInteger( BigDecimal value ) {
		if ( value.Exponent < 0 ) {
			var mant = value.Mantissa.ToString();

			var length = value.GetDecimalIndex();
			if ( length > 0 ) {
				return BigInteger.Parse( mant[ ..length ] );
			}

			if ( length == 0 ) {
				var tenthsPlace = Int32.Parse( mant[ 0 ].ToString() );
				return tenthsPlace >= 5 ? new BigInteger( 1 ) : BigInteger.Zero;
			}

			return BigInteger.Zero;
		}

		return BigInteger.Multiply( value.Mantissa, BigInteger.Pow( TenInt, value.Exponent ) );
	}
	*/

	/// <summary>Converts <paramref name="value" /> to an <see cref="Double" /> if possible, otherwise throws <see cref="OutOfRangeException" /> .</summary>
	/// <param name="value"></param>
	/// <exception cref="OutOfRangeException"></exception>
	public static explicit operator Double(BigDecimal value)
	{
		var t = (BigDecimal)value.Mantissa * Math.Pow(10, value.Exponent);
		if (t > MaxBigDecimalForDouble.Value)
		{
			throw new OutOfRangeException($"{nameof(BigDecimal)} is too large to convert to a {nameof(Double)}.");
		}

		return Convert.ToDouble(t.ToString());
	}

	/// <summary>Converts <paramref name="value" /> to an <see cref="Single" /> if possible, otherwise throws <see cref="OutOfRangeException" /> .</summary>
	/// <param name="value"></param>
	/// <exception cref="OutOfRangeException"></exception>
	public static explicit operator Single(BigDecimal value)
	{
		var t = (BigDecimal)value.Mantissa * Math.Pow(10, value.Exponent);
		if (t > MaxBigDecimalForSingle.Value)
		{
			throw new OutOfRangeException($"{nameof(BigDecimal)} is too large to convert to a {nameof(Single)}.");
		}

		return Convert.ToSingle(t.ToString());
	}

	/// <summary>Converts <paramref name="value" /> to an <see cref="Decimal" /> if possible, otherwise throws <see cref="OutOfRangeException" /> .</summary>
	/// <param name="value"></param>
	/// <exception cref="OutOfRangeException"></exception>
	public static explicit operator Decimal(BigDecimal value)
	{
		var t = (BigDecimal)value.Mantissa * Math.Pow(10, value.Exponent);
		if (t > MaxBigDecimalForDecimal.Value)
		{
			throw new OutOfRangeException($"{nameof(BigDecimal)} is too large to convert to a {nameof(Decimal)}.");
		}

		return Convert.ToDecimal(t.ToString());
	}

	/// <summary>Converts <paramref name="value" /> to an <see cref="Int32" /> if possible, otherwise throws <see cref="OutOfRangeException" /> .</summary>
	/// <param name="value"></param>
	/// <exception cref="OutOfRangeException"></exception>
	public static explicit operator Int32(BigDecimal value)
	{
		var t = value.Mantissa * Pow(10, value.Exponent);
		if (t > MaxBigDemicalForInt32.Value)
		{
			throw new OutOfRangeException($"{nameof(BigDecimal)} is too large to convert to a {nameof(Int32)}.");
		}

		return Convert.ToInt32(t.ToString());
	}

	/// <summary>Converts <paramref name="value" /> to an <see cref="UInt32" /> if possible, otherwise throws <see cref="OutOfRangeException" /> .</summary>
	/// <param name="value"></param>
	/// <exception cref="OutOfRangeException"></exception>
	public static explicit operator UInt32(BigDecimal value)
	{
		var t = value.Mantissa * Pow(10, value.Exponent);
		if (t > MaxBigDemicalForUInt32.Value)
		{
			throw new OutOfRangeException($"{nameof(BigDecimal)} is too large to convert to a {nameof(UInt32)}.");
		}

		return Convert.ToUInt32(t.ToString());
	}

	[NeedsTesting]
	public static BigDecimal operator %(BigDecimal left, BigDecimal right) => left - Floor(right * (left / right));

	public static BigDecimal operator +(BigDecimal value) => value;

	public static BigDecimal operator -(BigDecimal value) => Negate(value);

	public static BigDecimal operator ++(BigDecimal value) => Add(value, 1);

	public static BigDecimal operator --(BigDecimal value) => Subtract(value, 1);

	public static BigDecimal operator +(BigDecimal left, BigDecimal right) => Add(left, right);

	public static BigDecimal operator -(BigDecimal left, BigDecimal right) => Subtract(left, right);

	public static BigDecimal operator *(BigDecimal left, BigDecimal right) => Multiply(left, right);

	public static BigDecimal operator /(BigDecimal dividend, BigDecimal divisor) => Divide(dividend, divisor);

	public static Boolean operator <(BigDecimal left, BigDecimal right) =>
		left.Exponent > right.Exponent ? AlignExponent(left, right) < right.Mantissa : left.Mantissa < AlignExponent(right, left);

	public static Boolean operator >(BigDecimal left, BigDecimal right) =>
		left.Exponent > right.Exponent ? AlignExponent(left, right) > right.Mantissa : left.Mantissa > AlignExponent(right, left);

	public static Boolean operator <=(BigDecimal left, BigDecimal right) =>
		left.Exponent > right.Exponent ? AlignExponent(left, right) <= right.Mantissa : left.Mantissa <= AlignExponent(right, left);

	public static Boolean operator >=(BigDecimal left, BigDecimal right) =>
		left.Exponent > right.Exponent ? AlignExponent(left, right) >= right.Mantissa : left.Mantissa >= AlignExponent(right, left);

	/// <summary>Returns the result of multiplying a BigDecimal by negative one.</summary>
	public static BigDecimal Negate(BigDecimal value) => new BigDecimal(BigInteger.Negate(value.Mantissa), value.Exponent);

	/// <summary>Adds two BigDecimal values.</summary>
	public static BigDecimal Add(BigDecimal left, BigDecimal right)
	{
		if (left.Exponent > right.Exponent)
		{
			return new BigDecimal(AlignExponent(left, right) + right.Mantissa, right.Exponent);
		}

		return new BigDecimal(AlignExponent(right, left) + left.Mantissa, left.Exponent);
	}

	/// <summary>Subtracts two BigDecimal values.</summary>
	public static BigDecimal Subtract(BigDecimal left, BigDecimal right) => Add(left, Negate(right));

	/// <summary>Multiplies two BigDecimal values.</summary>
	public static BigDecimal Multiply(BigDecimal left, BigDecimal right) => new(left.Mantissa * right.Mantissa, left.Exponent + right.Exponent);

	/// <summary>Divides two BigDecimal values, returning the remainder and discarding the quotient.</summary>
	public static BigDecimal Mod(BigDecimal value, BigDecimal mod)
	{

		// x – q * y
		var quotient = Divide(value, mod);
		var floor = Floor(quotient);
		return Subtract(value, Multiply(floor, mod));
	}

	/// <summary>Divides two BigDecimal values.</summary>
	public static BigDecimal Divide(BigDecimal dividend, BigDecimal divisor)
	{
		if (divisor == Zero)
		{
			throw new DivideByZeroException(nameof(divisor));
		}

		var exponentChange = dividend.Exponent - divisor.Exponent;

		var counter = 0;

		BigInteger remainder;

		var mantissa = BigInteger.DivRem(dividend.Mantissa, divisor.Mantissa, out remainder);

		bool firstLoop = true;
		BigInteger lastRemainder = 0;
		while (remainder != 0)
		{
			if (firstLoop)
			{
				firstLoop = false;
			}
			else if (remainder == lastRemainder)
			{
				if (GetSignifigantDigits(mantissa) >= divisor.SignifigantDigits)
				{
					break;
				}
			}
			else if (GetSignifigantDigits(mantissa) >= Precision)
			{
				break;
			}


			while (BigInteger.Abs(remainder) < BigInteger.Abs(divisor.Mantissa))
			{
				remainder *= 10;
				mantissa *= 10;
				counter++;
			}

			lastRemainder = remainder;
			mantissa += BigInteger.DivRem(remainder, divisor.Mantissa, out remainder);
		}

		return new BigDecimal(mantissa, exponentChange - counter);
	}

	/// <summary>Returns e raised to the specified power</summary>
	public static BigDecimal Exp(Double exponent)
	{
		BigInteger wholeNumber = (BigInteger)Math.Truncate(exponent);
		Double fractionalPart = exponent % 1;
		int root = (int)Math.Truncate(1d / fractionalPart);

		BigDecimal result = Exp(wholeNumber) * NthRoot(E, root, Precision);
		return result;
	}

	/// <summary>Returns e raised to the specified power</summary>
	public static BigDecimal Exp(BigInteger exponent)
	{
		return Pow(E, exponent);
	}

	/// <summary>Returns a specified number raised to the specified power.</summary>
	public static BigDecimal Pow(BigDecimal @base, BigInteger exponent)
	{
		if (AlwaysTruncate)
		{
			return Pow_Precision(@base, exponent);
		}
		else
		{
			return Pow_Fast(@base, exponent);
		}
	}

	/// <summary>
	/// Returns a specified number raised to the specified power.
	/// </summary>
	/// <remarks>
	/// This version uses exponentiation by squaring.
	/// This method should take fewer steps than <see cref="Pow_Precision"/>, and so is used by default
	/// unless <see cref="AlwaysTruncate"/> is <see langword="true"/>, 
	/// in which case <see cref="Pow_Precision"/> is used as it loses precision slower.
	/// </remarks>
	private static BigDecimal Pow_Fast(BigDecimal @base, BigInteger exponent)
	{
		if (exponent.IsZero)
		{
			return One;
		}

		BigDecimal baseValue = @base;

		if (exponent.Sign < 0)
		{
			if (baseValue == Zero)
			{
				throw new NotSupportedException("Cannot raise zero to a negative power");
			}

			// n^(-e) -> (1/n)^e
			baseValue = One / baseValue;
			exponent = BigInteger.Negate(exponent);
		}

		var result = BigDecimal.One;
		while (exponent > BigInteger.Zero)
		{
			if (exponent % 2 == 1)
			{
				result *= baseValue;
				exponent--;
				if (exponent == 0)
				{
					break;
				}
			}

			baseValue *= baseValue;
			exponent /= 2;
		}

		return result;
	}

	/// <summary>
	/// Returns a specified number raised to the specified power.
	/// </summary>
	/// <remarks>
	/// This version loses precision slower, and so is used when <see cref="AlwaysTruncate"/> is set to <see langword="true"/>. 
	/// Otherwise <see cref="Pow_Fast"/> is used because it is more performant.
	/// </remarks>
	private static BigDecimal Pow_Precision(BigDecimal baseValue, BigInteger exponent)
	{
		if (exponent.IsZero)
		{
			return One;
		}

		if (exponent.Sign < 0)
		{
			if (baseValue == Zero)
			{
				throw new NotSupportedException("Cannot raise zero to a negative power");
			}

			// n^(-e) -> (1/n)^e
			baseValue = One / baseValue;
			exponent = BigInteger.Negate(exponent);
		}

		var result = baseValue;
		while (exponent > BigInteger.One)
		{
			result *= baseValue;
			exponent--;
		}

		return result;
	}

	/// <summary>Returns a specified number raised to the specified power.</summary>
	public static BigDecimal Pow(Double basis, Double exponent)
	{
		var tmp = One;
		while (Math.Abs(exponent) > 100)
		{
			var diff = exponent > 0 ? 100 : -100;
			tmp *= Math.Pow(basis, diff);
			exponent -= diff;
		}

		return tmp * Math.Pow(basis, exponent);
	}

	/// <summary> Returns the Nth root of the supplied input decimal to the given number of places. </summary>
	/// <returns></returns>
	public static BigDecimal NthRoot(BigDecimal input, Int32 root, Int32 decimalPlaces)
	{
		BigInteger mantissa = input.Mantissa;
		Int32 exponent = input.Exponent;

		Int32 placesNeeded = (decimalPlaces * root) - PlacesRightOfDecimal(input);
		if (placesNeeded > 0)
		{
			mantissa *= BigInteger.Pow(10, placesNeeded);
			exponent -= placesNeeded;
		}

		while (exponent % root != 0)
		{
			mantissa *= 10;
			exponent += 1;
		}

		BigInteger rem;
		BigInteger newMantissa = BigIntegerHelper.NthRoot(mantissa, root, out rem);
		Int32 newExponent = exponent /= root;

		return new BigDecimal(newMantissa, newExponent);
	}

	/// <summary> Returns the number of digits or place values to the left of the decimal point. </summary>
	private static Int32 PlacesLeftOfDecimal(BigDecimal input)
	{
		int mantLen = BigInteger.Abs(input.Mantissa).GetLength();
		int leftSideOfDecimal = mantLen + input.Exponent;
		return leftSideOfDecimal;
	}

	/// <summary> Returns the number of digits or place values to the right of the decimal point. </summary>
	private static Int32 PlacesRightOfDecimal(BigDecimal input)
	{
		int mantLen = BigInteger.Abs(input.Mantissa).GetLength();

		int rightSideOfDecimal = mantLen - PlacesLeftOfDecimal(input);
		return rightSideOfDecimal;
	}

	/// <summary>Returns the absolute value of the BigDecimal</summary>
	public static BigDecimal Abs(BigDecimal value)
	{
		BigDecimal result;
		if (value.IsNegative())
		{
			result = BigDecimal.Negate(value);
		}
		else
		{
			result = new BigDecimal(value.Mantissa, value.Exponent);
		}
		return result;
	}

	/// <summary>Truncates the BigDecimal to the given precision by removing the least significant digits.</summary>
	public static BigInteger Truncate(BigDecimal value) => Round(value);

	/// <summary>Rounds a BigDecimal value to the nearest integral value.</summary>
	public static BigInteger Round(BigDecimal value) => Round(value, MidpointRounding.AwayFromZero);

	/// <summary>Rounds a BigDecimal value to the nearest integral value. A parameter specifies how to round the value if it is midway between two numbers.</summary>
	public static BigInteger Round(BigDecimal value, MidpointRounding mode)
	{
		var wholePart = value.WholeValue;
		var decimalPart = value.GetFractionalPart();

		BigInteger addOne = value.IsNegative() ? -1 : 1;

		if (decimalPart > OneHalf)
		{
			wholePart += addOne;
		}
		else if (decimalPart == OneHalf)
		{
			if (mode == MidpointRounding.AwayFromZero)
			{
				wholePart += addOne;
			}
			else // MidpointRounding.ToEven
			{
				if (!wholePart.IsEven)
				{
					wholePart += addOne;
				}
			}
		}

		return wholePart;
	}

	/// <summary>Rounds a BigDecimal to the given number of digits.</summary>
	public static BigDecimal Round(BigDecimal value, Int32 precision)
	{
		var mantissa = value.Mantissa;
		var exponent = value.Exponent;

		var sign = Math.Sign(exponent);
		var digits = PlacesRightOfDecimal(value);
		if (digits > precision)
		{
			int difference = (digits - precision);
			mantissa = BigInteger.Divide(mantissa, BigInteger.Pow(TenInt, difference));

			if (sign != 0)
			{
				exponent -= sign * difference;
			}
		}

		return new BigDecimal(new Tuple<BigInteger, Int32>(mantissa, exponent));
	}

	/// <summary>Rounds a BigDecimal to an integer value. The BigDecimal argument is rounded towards positive infinity.</summary>
	[NeedsTesting]
	public static BigDecimal Ceiling(BigDecimal value)
	{
		BigDecimal result = value.WholeValue;

		if (result != value.Mantissa && value >= 0)
		{
			result += 1;
		}

		return result;
	}

	public static BigDecimal Floor(BigDecimal value)
	{
		BigDecimal result = value.WholeValue;

		if (result != value.Mantissa && value <= 0)
		{
			result -= 1;
		}

		return result;
	}

	public override Int32 GetHashCode() => new Tuple<BigInteger, Int32>(this.Mantissa, this.Exponent).GetHashCode();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override String ToString() => this.ToString(BigDecimalNumberFormatInfo);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public String ToString(IFormatProvider provider) => ToString(this.Mantissa, this.Exponent, provider);

	private static String ToString(BigInteger mantissa, Int32 exponent, IFormatProvider provider)
	{
		if (provider is null)
		{
			throw new ArgumentNullException(nameof(provider));
		}

		var formatProvider = NumberFormatInfo.GetInstance(provider);

		var negativeValue = mantissa.Sign == -1;
		var negativeExponent = Math.Sign(exponent) == -1;

		var result = BigInteger.Abs(mantissa).ToString();
		var absExp = Math.Abs(exponent);

		if (negativeExponent)
		{
			if (absExp > result.Length)
			{
				var zerosToAdd = Math.Abs(absExp - result.Length);
				var zeroString = String.Concat(Enumerable.Repeat(formatProvider.NativeDigits[0], zerosToAdd));
				result = zeroString + result;
				result = result.Insert(0, formatProvider.NumberDecimalSeparator);
				result = result.Insert(0, formatProvider.NativeDigits[0]);
			}
			else
			{
				var indexOfRadixPoint = Math.Abs(absExp - result.Length);
				result = result.Insert(indexOfRadixPoint, formatProvider.NumberDecimalSeparator);
				if (indexOfRadixPoint == 0)
				{
					result = result.Insert(0, formatProvider.NativeDigits[0]);
				}
			}

			result = result.TrimEnd('0');
			if (result.Last().ToString() == formatProvider.NumberDecimalSeparator)
			{
				result = result.Substring(0, result.Length - 1);
			}
		}
		else
		{
			var zeroString = String.Concat(Enumerable.Repeat(formatProvider.NativeDigits[0], absExp));
			result += zeroString;
		}

		if (negativeExponent)
		{

			//TODO // Prefix "0."?
		}

		if (negativeValue)
		{

			// Prefix "-"
			result = result.Insert(0, formatProvider.NegativeSign);
		}

		return result;
	}

	public static String UnNormalize(BigInteger mantissa, Int32 exponent, IFormatProvider provider)
	{
		if (provider is null)
		{
			throw new ArgumentNullException(nameof(provider));
		}

		var formatProvider = NumberFormatInfo.GetInstance(provider);

		var negativeExponent = Math.Sign(exponent) == -1;

		var result = BigInteger.Abs(mantissa).ToString();
		var absExp = Math.Abs(exponent);

		if (negativeExponent)
		{
			if (absExp > result.Length)
			{
				var zerosToAdd = Math.Abs(absExp - result.Length);

				var zeroString = String.Empty.PadRight(zerosToAdd, formatProvider.NativeDigits[0][0]);
				result = zeroString + result;
				result = result.Insert(0, formatProvider.NumberDecimalSeparator);
				result = result.Insert(0, formatProvider.NativeDigits[0]);
			}
			else
			{
				var indexOfRadixPoint = Math.Abs(absExp - result.Length);
				result = result.Insert(indexOfRadixPoint, formatProvider.NumberDecimalSeparator);
				if (indexOfRadixPoint == 0)
				{
					result = result.Insert(0, formatProvider.NativeDigits[0]);
				}
			}

			result = result.TrimEnd('0');
			if (result.Last().ToString() == formatProvider.NumberDecimalSeparator)
			{
				result = result.Substring(0, result.Length - 1);
			}
		}
		else
		{
			var zeroString = String.Concat(Enumerable.Repeat(formatProvider.NativeDigits[0], absExp));
			result += zeroString;
		}

		if (mantissa.Sign == -1)
		{
			return $"{formatProvider.NegativeSign}{result}";
		}

		return result;
	}

	/// <summary>Allow the BigDecimal to be formatted with the E notation.</summary>
	/// <param name="bigDecimal"></param>
	/// <returns></returns>
	[NeedsTesting]
	private static String ToScientificENotation(BigDecimal bigDecimal)
	{

		// 1.238E456 1.238E-456
		// -1.238E456
		// -1.238E-456 1E456 1E-456
		// -1E456
		// -1E-456
		// -3E-2

		var manString = bigDecimal.Mantissa.ToString(); //Note: will be prefixed with "-" if negative.

		var period = manString.Length - bigDecimal.Exponent + 1;
		if (bigDecimal.Mantissa.Sign == -1)
		{
			++period;
		}

		//TODO none of this is tested or guaranteed to work yet. Like negatives, or small numbers need the correct logic.

		var result2 = $"{manString.Insert(period, ".")}E{bigDecimal.Exponent:D}";
		return result2;
	}

}