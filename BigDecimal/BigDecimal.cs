using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using ExtendedNumerics.Helpers;
using ExtendedNumerics.Properties;

namespace ExtendedNumerics
{
	/// <summary>
	/// <para>Arbitrary precision decimal. All operations are exact, except for division.</para>
	/// <para>Division never determines more digits than the given precision.</para>
	/// <para>Based on code by Jan Christoph Bernack (http://stackoverflow.com/a/4524254 or jc.bernack at gmail.com)</para>
	/// <para>Modified and extended by Adam White (https://csharpcodewhisperer.blogspot.com/)</para>
	/// <para>Further modified by Rick Harker, Rick.Rick.Harker@gmail.com</para>
	/// </summary>
	public readonly record struct BigDecimal : IComparable, IComparable<BigDecimal>, IComparable<Int32>, IComparable<Int32?>, IComparable<Decimal>, IComparable<Double>, IComparable<Single>
	{
		private const String NullString = "(null)";

		static BigDecimal()
		{
			Pi = Parse(
				"3.14159265358979323846264338327950288419716939937510582097494459230781640628620899862803482534211706798214808651328230664709384460955058223172535940812848111745028410270193852110555964462294895493038196", CultureInfo.InvariantCulture);

			π = Pi;

			E = Parse(
				"2.71828182845904523536028747135266249775724709369995957496696762772407663035354759457138217852516642749193200305992181741359662904357290033429526059563073813232862794349076323382988075319525101901157383", CultureInfo.InvariantCulture);
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
			BigDecimal quotient = Divide(new BigDecimal(numerator), new BigDecimal(denominator));
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
			if (this.Mantissa == 0)
			{
				this.Exponent = 0;
			}
		}

		public BigDecimal(Int32 value) : this(new BigInteger(value)) { }

		public BigDecimal(BigInteger value) : this(value, 0) { }

		public BigDecimal(Single value)
		{
			if (Single.IsInfinity(value))
			{
				throw new OverflowException(LanguageResources.Overflow_BigDecimal_Infinity);
			}

			if (Single.IsNaN(value))
			{
				throw new NotFiniteNumberException(LanguageResources.NotFinite_NaN);
			}

			BigDecimal results = Parse(value);
			this.Mantissa = results.Mantissa;
			this.Exponent = results.Exponent;
		}

		public BigDecimal(Double value)
		{
			if (Double.IsInfinity(value))
			{
				throw new OverflowException(LanguageResources.Overflow_BigDecimal_Infinity);
			}

			if (Double.IsNaN(value))
			{
				throw new NotFiniteNumberException(LanguageResources.NotFinite_NaN);
			}

			BigDecimal results = Parse(value);
			this.Mantissa = results.Mantissa;
			this.Exponent = results.Exponent;
		}

		public BigDecimal(Decimal value)
		{
			BigDecimal results = Parse(value);
			this.Mantissa = results.Mantissa;
			this.Exponent = results.Exponent;
		}

		/// <summary>Gets a value that represents the number 0 (zero).</summary>
		public static BigDecimal Ten => 10;

		/// <summary>Gets a value that represents the number 1 ().</summary>
		public static BigDecimal One => 1;

		/// <summary>Gets a value that represents the number 0 (zero).</summary>
		public static BigDecimal Zero => 0;

		/// <summary>Gets a value that represents the number 0.5.</summary>
		public static BigDecimal OneHalf => 0.5d;

		/// <summary>Gets a value that represents the number -1 .</summary>
		public static BigDecimal MinusOne => -1;

		/// <summary>Gets a value that represents the number e, also called Euler's number.</summary>
		public static BigDecimal E { get; }

		/// <summary>Gets a value that represents the number Pi.</summary>
		public static BigDecimal Pi { get; }

		/// <summary>Gets a value that represents the number Pi.</summary>
		public static BigDecimal π { get; }

		private static BigInteger TenInt { get; } = new BigInteger(10);

		private static NumberFormatInfo BigDecimalNumberFormatInfo { get { return CultureInfo.CurrentCulture.NumberFormat; } }

		/// <summary>
		/// Sets the desired precision of all BigDecimal instances, in terms of the number of .
		/// 
		/// 
		/// If AlwaysTruncate is set to true all operations are affected.</summary>
		public static Int32 Precision { get; set; } = 5000;

		/// <summary>
		/// Specifies whether the significant digits should be truncated to the given precision after each operation.	
		/// Setting this to true will tend to accumulate errors at the precision boundary after several arithmetic operations.
		/// Therefore, you should prefer using <see cref="Round(BigDecimal, int)"/> explicitly when you need it instead, 
		/// such st at the end of a series of operations, especially if you are expecting the result to be truncated at the precision length.
		/// This should generally be left disabled by default.
		/// This setting may be useful if you are running into memory or performance issues, as could conceivably be brought on by many operations on irrational numbers.
		/// </summary>
		public static Boolean AlwaysTruncate { get; set; } = false;

		/// <summary>Specifies whether a call to Normalize is made after every operation and during constructor invocation. The default value is true.</summary>
		public static Boolean AlwaysNormalize { get; set; } = true;

		/// <summary>The mantissa of the internal floating point number representation of this BigDecimal.</summary>
		public readonly BigInteger Mantissa;

		/// <summary>The exponent of the internal floating point number representation of this BigDecimal.</summary>
		public readonly Int32 Exponent;

		/// <summary>Gets a number that indicates the sign (negative, positive, or zero) of the current <see cref="BigDecimal" /> object. </summary>
		/// <returns>-1 if the value of this object is negative, 0 if the value of this object is zero or 1 if the value of this object is positive.</returns>
		public Int32 Sign => this.Mantissa.Sign;

		/// <summary>Gets the number of significant digits in <see cref="BigDecimal"/>.
		///Essentially tells you the number of digits in the mantissa.</summary>
		public Int32 SignifigantDigits => GetSignifigantDigits(this.Mantissa);

		/// <summary>The length of the BigDecimal value (Equivalent to SignifigantDigits).</summary>
		public Int32 Length => GetSignifigantDigits(this.Mantissa) + this.Exponent;

		private static Int32 GetSignifigantDigits(BigInteger value) => value.GetSignifigantDigits();

		public Int32 DecimalPlaces => PlacesRightOfDecimal(this);

		/// <summary>
		/// Gets the whole-number integer (positive or negative) value of this BigDecimal, so everything to the left of the decimal place.
		/// Equivalent to the Truncate function for a float.
		/// </summary>
		public BigInteger WholeValue => this.GetWholePart();


		/// <summary>This method returns true if the BigDecimal is equal to zero, false otherwise.</summary>
		public Boolean IsZero() => this.Mantissa.IsZero;

		/// <summary>This method returns true if the BigDecimal is greater than zero, false otherwise.</summary>
		public Boolean IsPositve() => !this.IsZero() && !this.IsNegative();

		/// <summary>This method returns true if the BigDecimal is less than zero, false otherwise.</summary>
		public Boolean IsNegative() => this.Mantissa.Sign < 0;

		private static BigDecimal MaxBigDecimalForDecimal => (BigDecimal)Decimal.MaxValue;

		private static BigDecimal MaxBigDecimalForSingle => (BigDecimal)Single.MaxValue;

		private static BigDecimal MaxBigDecimalForDouble => (BigDecimal)Double.MaxValue;

		private static BigDecimal MaxBigDemicalForInt32 => (BigDecimal)Int32.MaxValue;

		private static BigDecimal MaxBigDemicalForUInt32 => (BigDecimal)UInt32.MaxValue;

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
			other is null || (this > other) ? SortingOrder.After :
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
		int IComparable.CompareTo(Object? obj)
		{
			if (obj == null) { return SortingOrder.After; }
			if (obj is not BigDecimal) { throw new ArgumentException(String.Format(LanguageResources.Arg_MustBeOfType, nameof(BigDecimal)), nameof(obj)); }
			return this.CompareTo((BigDecimal)obj);
		}

		public Boolean Equals(BigDecimal? other) => Equals(this, other);

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

		/// <summary>Converts the string representation of a float to the BigDecimal equivalent.</summary>
		public static BigDecimal Parse(Single input) => Parse(input.ToString("R"));

		/// <summary>Converts the string representation of a double to the BigDecimal equivalent.</summary>
		public static BigDecimal Parse(Double input) => Parse(input.ToString("R"));

		/// <summary>Converts the string representation of a decimal to the BigDecimal equivalent.</summary>
		public static BigDecimal Parse(Decimal input) => Parse(input.ToString("R"));

		/// <summary>Converts the string representation of a decimal to the BigDecimal equivalent.</summary>
		/// <param name="input">A string that contains a number to convert.</param>
		/// <returns></returns>
		public static BigDecimal Parse(String input)
		{
			return Parse(input, BigDecimalNumberFormatInfo);
		}

		/// <summary>
		/// Converts the string representation of a decimal in a specified culture-specific format to its BigDecimal equivalent.
		/// </summary>
		/// <param name="input">A string that contains a number to convert.</param>
		/// <param name="provider">An object that provides culture-specific formatting information about value.</param>
		/// <returns></returns>
		public static BigDecimal Parse(String input, IFormatProvider provider)
		{
			if (provider is null)
			{
				provider = BigDecimalNumberFormatInfo;
			}

			NumberFormatInfo numberFormatProvider = NumberFormatInfo.GetInstance(provider);

			input = input.Trim();
			if (String.IsNullOrEmpty(input))
			{
				return BigInteger.Zero;
			}

			var exponent = 0;
			var isNegative = false;

			if (input.StartsWith(numberFormatProvider.NegativeSign, StringComparison.OrdinalIgnoreCase))
			{
				isNegative = true;
				input = input.TrimStart([numberFormatProvider.NegativeSign.Single()]);
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

			if (input.Contains(numberFormatProvider.NumberDecimalSeparator))
			{
				var decimalPlace = input.IndexOf(numberFormatProvider.NumberDecimalSeparator, StringComparison.Ordinal);

				exponent += (decimalPlace + 1) - input.Length;
				input = input.Replace(numberFormatProvider.NumberDecimalSeparator, String.Empty);
			}

			var mantissa = BigInteger.Parse(input, numberFormatProvider);
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

		/// <summary>
		/// Tries to convert the string representation of a number to its BigDecimal equivalent, and returns a value that indicates whether the conversion succeeded.
		/// </summary>
		/// <param name="input">The string representation of a number.</param>
		/// <param name="result">When this method returns, this out parameter contains the BigDecimal equivalent
		/// to the number that is contained in value, or default(BigDecimal) if the conversion fails.
		/// The conversion fails if the value parameter is null or is not of the correct format.</param>
		/// <returns></returns>
		public static bool TryParse(String input, out BigDecimal result)
		{
			return TryParse(input, BigDecimalNumberFormatInfo, out result);
		}

		/// <summary>
		/// Tries to convert the string representation of a number in a specified style and culture-specific format
		/// to its BigDecimal equivalent, and returns a value that indicates whether the conversion succeeded.
		/// </summary>
		/// <param name="input">The string representation of a number.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information about value.</param>
		/// <param name="result">When this method returns, this out parameter contains the BigDecimal equivalent
		/// to the number that is contained in value, or default(BigDecimal) if the conversion fails.
		/// The conversion fails if the value parameter is null or is not of the correct format.</param>
		/// <returns></returns>
		public static bool TryParse(String input, IFormatProvider provider, out BigDecimal result)
		{
			try
			{
				BigDecimal output = Parse(input, provider);
				result = output;
				return true;
			}
			catch
			{
			}
			result = default(BigDecimal);
			return false;
		}

		public static Int32 NumberOfDigits(BigInteger value) => (Int32)Math.Ceiling(BigInteger.Log10(value * value.Sign));

		/// <summary>Removes any trailing zeros on the mantissa, adjusts the exponent, and returns a new <see cref="BigDecimal" />.</summary>
		/// <param name="value"></param>

		public static BigDecimal Normalize(BigDecimal value)
		{
			if (value.Mantissa.IsZero)
			{
				if (value.Exponent != 0)
				{
					return new BigDecimal(new Tuple<BigInteger, Int32>(BigInteger.Zero, 0));
				}

				return value;
			}

			var s = value.Mantissa.ToString();
			var pos = s.LastIndexOf('0', s.Length - 1);

			if (pos < (s.Length - 1))
			{
				return value;
			}

			var c = s[pos];

			while ((pos > 0) && (c == '0'))
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
		public Int32 GetDecimalIndex()
		{
			var mantissaLength = this.Mantissa.GetLength();
			if (this.Mantissa.Sign < 0)
			{
				mantissaLength++;
			}

			return mantissaLength + this.Exponent;
		}

		/// <summary>
		/// Returns the whole number integer part of the BigDecimal, dropping anything right of the decimal point. Essentially behaves like Math.Truncate(). For
		/// example, GetWholePart() would return 3 for Math.PI.
		/// </summary>
		public BigInteger GetWholePart()
		{
			var resultString = String.Empty;
			var decimalString = this.ToString(BigDecimalNumberFormatInfo);

			var valueSplit = decimalString.Split(BigDecimalNumberFormatInfo.NumberDecimalSeparator.ToCharArray()); //, StringSplitOptions.RemoveEmptyEntries
			if (valueSplit.Length > 0)
			{
				resultString = valueSplit[0];
			}

			var posE = resultString.IndexOf("E", StringComparison.Ordinal);
			if (posE > 0)
			{
				resultString = resultString.Split('E')[0]; //, StringSplitOptions.RemoveEmptyEntries 
			}

			return BigInteger.Parse(resultString);
		}

		/// <summary>Gets the fractional part of the BigDecimal, setting everything left of the decimal point to zero.</summary>
		public BigDecimal GetFractionalPart()
		{
			var resultString = String.Empty;
			var decimalString = this.ToString();

			var valueSplit = decimalString.Split(BigDecimalNumberFormatInfo.NumberDecimalSeparator.ToCharArray());
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

		public static implicit operator BigDecimal(BigInteger value) => new BigDecimal(value, 0);

		public static implicit operator BigDecimal(Byte value) => new BigDecimal(new BigInteger(value), 0);

		public static implicit operator BigDecimal(SByte value) => new BigDecimal(new BigInteger(value), 0);

		public static implicit operator BigDecimal(UInt32 value) => new BigDecimal(new BigInteger(value), 0);

		public static implicit operator BigDecimal(Int32 value) => new BigDecimal(new BigInteger(value), 0);

		public static implicit operator BigDecimal(UInt16 value) => new BigDecimal(new BigInteger(value), 0);

		public static implicit operator BigDecimal(Int16 value) => new BigDecimal(new BigInteger(value), 0);

		public static implicit operator BigDecimal(UInt64 value) => new BigDecimal(new BigInteger(value), 0);

		public static implicit operator BigDecimal(Int64 value) => new BigDecimal(new BigInteger(value), 0);

		public static implicit operator BigDecimal(Single value) => Parse(value);

		public static implicit operator BigDecimal(Decimal value) => Parse(value);

		public static implicit operator BigDecimal(Double value) => Parse(value);

		public static explicit operator BigInteger(BigDecimal value)
		{
			var floored = Floor(value);
			return floored.Mantissa * BigInteger.Pow(10, floored.Exponent);
		}

		/// <summary>Converts <paramref name="value" /> to an <see cref="Double" /> if possible, otherwise throws <see cref="OverflowException" /> .</summary>
		/// <param name="value"></param>
		/// <exception cref="OverflowException"></exception>
		public static explicit operator Double(BigDecimal value)
		{
			if (value > MaxBigDecimalForDouble)
			{
				throw new OverflowException(LanguageResources.Overflow_Double);
			}
			return Convert.ToDouble(value.ToString());
		}

		/// <summary>Converts <paramref name="value" /> to an <see cref="Single" /> if possible, otherwise throws <see cref="OverflowException" /> .</summary>
		/// <param name="value"></param>
		/// <exception cref="OverflowException"></exception>
		public static explicit operator Single(BigDecimal value)
		{
			if (value > MaxBigDecimalForSingle)
			{
				throw new OverflowException(LanguageResources.Overflow_Single);
			}
			return Convert.ToSingle(value.ToString());
		}

		/// <summary>Converts <paramref name="value" /> to an <see cref="Decimal" /> if possible, otherwise throws <see cref="OverflowException" /> .</summary>
		/// <param name="value"></param>
		/// <exception cref="OverflowException"></exception>
		public static explicit operator Decimal(BigDecimal value)
		{
			if (value > MaxBigDecimalForDecimal)
			{
				throw new OverflowException(LanguageResources.Overflow_Decimal);
			}
			return Convert.ToDecimal(value.ToString());
		}

		/// <summary>Converts <paramref name="value" /> to an <see cref="Int32" /> if possible, otherwise throws <see cref="OverflowException" /> .</summary>
		/// <param name="value"></param>
		/// <exception cref="OverflowException"></exception>
		public static explicit operator Int32(BigDecimal value)
		{
			if (value > MaxBigDemicalForInt32)
			{
				throw new OverflowException(LanguageResources.Overflow_Int32);
			}
			return Convert.ToInt32(value.ToString());
		}

		/// <summary>Converts <paramref name="value" /> to an <see cref="UInt32" /> if possible, otherwise throws <see cref="OverflowException" /> .</summary>
		/// <param name="value"></param>
		/// <exception cref="OverflowException"></exception>
		public static explicit operator UInt32(BigDecimal value)
		{
			if (value > MaxBigDemicalForUInt32)
			{
				throw new OverflowException(LanguageResources.Overflow_UInt32);
			}
			return Convert.ToUInt32(value.ToString());
		}

		public static BigDecimal operator %(BigDecimal left, BigDecimal right) => Mod(left, right);

		public static BigDecimal operator +(BigDecimal value) => value;

		public static BigDecimal operator -(BigDecimal value) => Negate(value);

		public static BigDecimal operator ++(BigDecimal value) => Add(value, 1);

		public static BigDecimal operator --(BigDecimal value) => Subtract(value, 1);

		public static BigDecimal operator +(BigDecimal left, BigDecimal right) => Add(left, right);

		public static BigDecimal operator -(BigDecimal left, BigDecimal right) => Subtract(left, right);

		public static BigDecimal operator *(BigDecimal left, BigDecimal right) => Multiply(left, right);

		public static BigDecimal operator /(BigDecimal dividend, BigDecimal divisor) => Divide(dividend, divisor);

		public static Boolean operator <(BigDecimal left, BigDecimal right) => left.Exponent > right.Exponent ? AlignExponent(left, right) < right.Mantissa : left.Mantissa < AlignExponent(right, left);

		public static Boolean operator >(BigDecimal left, BigDecimal right) => left.Exponent > right.Exponent ? AlignExponent(left, right) > right.Mantissa : left.Mantissa > AlignExponent(right, left);

		public static Boolean operator <=(BigDecimal left, BigDecimal right) => left.Exponent > right.Exponent ? AlignExponent(left, right) <= right.Mantissa : left.Mantissa <= AlignExponent(right, left);

		public static Boolean operator >=(BigDecimal left, BigDecimal right) => left.Exponent > right.Exponent ? AlignExponent(left, right) >= right.Mantissa : left.Mantissa >= AlignExponent(right, left);

		/// <summary>Returns the smaller of two BigDecimal values.</summary>
		public static BigDecimal Min(BigDecimal left, BigDecimal right) => left <= right ? left : right;

		/// <summary>Returns the larger of two BigDecimal values.</summary>	
		public static BigDecimal Max(BigDecimal left, BigDecimal right) => left >= right ? left : right;

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
		public static BigDecimal Multiply(BigDecimal left, BigDecimal right) =>
			new BigDecimal(left.Mantissa * right.Mantissa, left.Exponent + right.Exponent);

		/// <summary>Divides two BigDecimal values, returning the remainder and discarding the quotient.</summary>
		public static BigDecimal Mod(BigDecimal value, BigDecimal mod)
		{
			//TODO Verify if the % overload is correct, and/or this one is. Then merge the two to use the same function.
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
			var mantissa = BigInteger.DivRem(dividend.Mantissa, divisor.Mantissa, out var remainder);

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

		/// <summary>Returns a specified number raised to the specified power.</summary>
		public static BigDecimal Pow(BigDecimal @base, BigInteger exponent)
		{
			if (AlwaysTruncate)
			{
				return Pow_Precision(@base, exponent);
			}
			return Pow_Fast(@base, exponent);
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
			if (exponent == One)
			{
				return @base;
			}

			BigDecimal baseValue = @base;
			BigInteger exp = exponent;

			if (exp.Sign < 0)
			{
				if (baseValue == Zero)
				{
					throw new ArgumentException(LanguageResources.Arg_MustNotEqualZero, nameof(@base));
				}

				// n^(-e) -> (1/n)^e
				baseValue = One / baseValue;
				exp = BigInteger.Negate(exp);
			}

			var result = One;
			while (exp > BigInteger.Zero)
			{
				if ((exp % 2) == 1)
				{
					result *= baseValue;
					exp--;
					if (exp == 0)
					{
						break;
					}
				}

				baseValue *= baseValue;
				exp /= 2;
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
		private static BigDecimal Pow_Precision(BigDecimal @base, BigInteger exponent)
		{
			if (exponent.IsZero)
			{
				return One;
			}
			if (exponent == One)
			{
				return @base;
			}

			BigDecimal baseValue = @base;
			BigInteger exp = exponent;

			if (exp.Sign < 0)
			{
				if (baseValue == Zero)
				{
					throw new ArgumentException(LanguageResources.Arg_MustNotEqualZero, nameof(@base));
				}

				// n^(-e) -> (1/n)^e
				baseValue = One / baseValue;
				exp = BigInteger.Negate(exp);
			}

			var result = @base;
			while (exp > BigInteger.One)
			{
				result *= @base;
				exp--;
			}

			return result;
		}

		/// <summary>Returns a specified number raised to the specified power.</summary>
		public static BigDecimal Pow(Double @base, Double exponent)
		{
			if (exponent==0)
			{
				return One;
			}
			if (exponent == 1.0d)
			{
				return @base;
			}

			var tmp = One;
			while (Math.Abs(exponent) > ExpChunk)
			{
				var diff = exponent > 0 ? ExpChunk : -ExpChunk;
				tmp *= Math.Pow(@base, diff);
				exponent -= diff;
			}

			return tmp * Math.Pow(@base, exponent);
		}
		private static double ExpChunk = 2.0d;

		public static BigDecimal SquareRoot(BigDecimal input, Int32 decimalPlaces)
		{
			return NthRoot(input, 2, decimalPlaces);
		}

		/// <summary> Returns the Nth root of the supplied input decimal to the given number of places. </summary>
		/// <returns></returns>
		public static BigDecimal NthRoot(BigDecimal input, Int32 root, Int32 decimalPlaces)
		{
			BigInteger mantissa = input.Mantissa;
			Int32 exponent = input.Exponent;

			int sign = Math.Sign(exponent);
			if (sign == 0)
			{
				sign = 1;
			}

			Int32 placesNeeded = (decimalPlaces * root) - PlacesRightOfDecimal(input);
			if (placesNeeded > 0)
			{
				mantissa *= BigInteger.Pow(10, placesNeeded);
				exponent -= placesNeeded;
			}

			while ((exponent % root) != 0)
			{
				mantissa *= 10;
				exponent += sign;
			}

			BigInteger rem;
			BigInteger newMantissa = mantissa.NthRoot(root, out rem);
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

		/// <summary>Returns the mantissa of value, aligned to the exponent of reference. Assumes the exponent of value is larger than of reference.</summary>
		private static BigInteger AlignExponent(BigDecimal value, BigDecimal reference) => value.Mantissa * BigInteger.Pow(TenInt, value.Exponent - reference.Exponent);

		/// <summary>Returns the absolute value of the BigDecimal</summary>
		public static BigDecimal Abs(BigDecimal value)
		{
			BigDecimal result;
			if (value.IsNegative())
			{
				result = Negate(value);
			}
			else
			{
				result = new BigDecimal(value.Mantissa, value.Exponent);
			}
			return result;
		}

		/// <summary>Truncates the BigDecimal at the decimal point. Equivalent to using Floor.</summary>
		public static BigDecimal Truncate(BigDecimal value) => Floor(value);

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

		/// <summary>
		/// Rounds a BigDecimal to the given number of digits to the right of the decimal point.
		/// Pass a negative precision value to round (zero) digits to the left of the decimal point in a manner that mimics Excel's ROUNDDOWN function.
		/// </summary>
		public static BigDecimal Round(BigDecimal value, Int32 precision)
		{
			if (precision < 0)
			{
				string integer = value.WholeValue.ToString();
				int len = integer.Length;
				if (Math.Abs(precision) >= len)
				{
					return BigDecimal.Zero;
				}
				int diff = len + precision;

				string result = integer.Substring(0, diff);
				result += new string(Enumerable.Repeat('0', Math.Abs(precision)).ToArray());
				return BigDecimal.Parse(result);
			}
			else if (precision == 0)
			{
				return new BigDecimal(value.WholeValue);
			}

			var mantissa = value.Mantissa;
			var exponent = value.Exponent;
			var sign = Math.Sign(exponent);
			var digits = PlacesRightOfDecimal(value);
			if (digits > precision)
			{
				int difference = digits - precision;
				mantissa = BigInteger.Divide(mantissa, BigInteger.Pow(TenInt, difference));

				if (sign != 0)
				{
					exponent -= sign * difference;
				}
			}
			return new BigDecimal(new Tuple<BigInteger, Int32>(mantissa, exponent));
		}

		/// <summary>Rounds a BigDecimal up to the next largest integer value, even if the fractional part is less than one half. Equivalent to obtaining the floor and then adding one.</summary>
		public static BigDecimal Ceiling(BigDecimal value)
		{
			BigDecimal result = value.WholeValue;
			if ((result != value.Mantissa) && (value >= 0))
			{
				result += 1;
			}
			return result;
		}

		/// <summary>Rounds a BigDecimal down to the next smallest integer value, even if the fractional part is greater than one half. Equivalent to discarding everything right of the decimal point.</summary>
		public static BigDecimal Floor(BigDecimal value)
		{
			BigDecimal result = value.WholeValue;
			if ((result != value.Mantissa) && (value <= 0))
			{
				result -= 1;
			}
			return result;
		}

		#region Trigonometric Functions

		#region Standard Trigonometric Functions

		/// <summary>
		/// Arbitrary precision sine function. 
		/// The input should be the angle in radians.
		/// The input must be restricted to the range of -π/2 &lt;= θ &lt;= π/2.
		/// If your input is negative, just flip the sign.
		/// </summary>
		/// <returns></returns>
		public static BigDecimal Sin(BigDecimal radians)
		{
			return Sin(radians, Precision);
		}

		/// <summary>
		/// Arbitrary precision sine function. 
		/// The input should be the angle in radians.
		/// The input must be restricted to the range of -π/2 &lt;= θ &lt;= π/2.
		/// If your input is negative, just flip the sign.
		/// </summary>
		/// <param name="radians">The argument radians.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		/// <returns></returns>
		public static BigDecimal Sin(BigDecimal radians, int precision)
		{
			int resultSign = radians.Sign;
			BigDecimal input = Abs(radians);

			if (input == 0)
			{
				return 0;
			}

			if (input == TrigonometricHelper.HalfPi)
			{
				return 1 * resultSign;
			}
			if (input > TrigonometricHelper.HalfPi)
			{
				input = TrigonometricHelper.WrapInput(radians);
			}

			BigDecimal sumStart = 0;
			BigInteger counterStart = 1;
			BigInteger jump = 2;
			BigInteger multiplier = -1;
			bool factorialDenominator = true;

			BigDecimal result = TrigonometricHelper.TaylorSeriesSum(input, sumStart, counterStart, jump, multiplier, factorialDenominator, precision);

			return result * resultSign;
		}

		/// <summary>
		/// Arbitrary precision cosine function.
		/// </summary>
		public static BigDecimal Cos(BigDecimal radians)
		{
			return Cos(radians, Precision);
		}

		/// <summary>
		/// Arbitrary precision cosine function.
		/// </summary>
		/// <param name="radians">The argument radians.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		public static BigDecimal Cos(BigDecimal radians, int precision)
		{
			return Sin(radians + TrigonometricHelper.HalfPi, precision);
		}

		/// <summary>
		/// Arbitrary precision tangent function. 
		/// The input must not be π/2 or 3π/2, as the tangent is undefined at that value.
		/// </summary>
		public static BigDecimal Tan(BigDecimal radians)
		{
			return Tan(radians, Precision);
		}

		/// <summary>
		/// Arbitrary precision tangent function. 
		/// The input must not be π/2 or 3π/2, as the tangent is undefined at that value.
		/// </summary>
		/// <param name="radians">The argument radians.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		public static BigDecimal Tan(BigDecimal radians, int precision)
		{
			BigDecimal mod = Normalize(Mod(radians, TrigonometricHelper.HalfPi));
			if (mod.IsZero())
			{
				throw new ArithmeticException(LanguageResources.Arithmetic_Trig_Undefined_Tan_PiOver2);
			}

			BigDecimal modThreePiOverTwo = Normalize(Mod(radians, new BigDecimal(3) * TrigonometricHelper.HalfPi));
			if (modThreePiOverTwo.IsZero())
			{
				throw new ArithmeticException(LanguageResources.Arithmetic_Trig_Undefined_Tan_3PiOver2);
			}

			// Wrap around at π
			BigDecimal modPi = Normalize(Mod(radians, Pi));

			BigDecimal sin = Sin(modPi, precision);
			BigDecimal cos = Cos(modPi, precision);
			return sin / cos; // tan = sin / cos
		}

		/// <summary>
		/// Arbitrary precision cotangent function. 
		/// The input must not be zero, as the cotangent is undefined at that value.
		/// </summary>
		public static BigDecimal Cot(BigDecimal radians)
		{
			return Cot(radians, Precision);
		}

		/// <summary>
		/// Arbitrary precision cotangent function. 
		/// The input must not be zero, as the cotangent is undefined at that value.
		/// </summary>
		/// <param name="radians">The argument radians.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		public static BigDecimal Cot(BigDecimal radians, int precision)
		{
			if (radians.IsZero())
			{
				throw new ArithmeticException(LanguageResources.Arithmetic_Trig_Undefined_Cot_Zero);
			}

			BigDecimal modPi = Normalize(Mod(radians, Pi));
			if (modPi.IsZero())
			{
				throw new ArithmeticException(LanguageResources.Arithmetic_Trig_Undefined_Cot_Pi);
			}

			// Use modPi to wrap around at π
			BigDecimal cos = Cos(modPi, precision);
			BigDecimal sin = Sin(modPi, precision);

			return cos / sin; // cot = cos / sin
		}

		/// <summary>
		/// Arbitrary precision secant function. 
		/// The input must not be (2*n + 1)*π/2 (an odd multiple of π/2), as the secant is undefined at that value.
		/// </summary>
		public static BigDecimal Sec(BigDecimal radians)
		{
			return Sec(radians, Precision);
		}

		/// <summary>
		/// Arbitrary precision secant function. 
		/// The input must not be (2*n + 1)*π/2 (an odd multiple of π/2), as the secant is undefined at that value.
		/// </summary>
		/// <param name="radians">The argument radians.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		public static BigDecimal Sec(BigDecimal radians, int precision)
		{
			BigDecimal modOddPiOverTwo = TrigonometricHelper.ModOddHalfPi(radians);
			if (modOddPiOverTwo == One)
			{
				throw new ArithmeticException(LanguageResources.Arithmetic_Trig_Undefined_Sec_OddPiOver2);
			}

			// Wrap around at 2π
			BigDecimal modTwoPi = Normalize(Mod(radians, new BigDecimal(2) * Pi));

			BigDecimal cos = Cos(modTwoPi, precision);
			return One / cos; // sec = 1 / cos
		}

		/// <summary>
		/// Arbitrary precision cosecant function. 
		/// The input must not be zero or π, as the cosecant is undefined at that value.
		/// </summary>
		public static BigDecimal Csc(BigDecimal radians)
		{
			return Csc(radians, Precision);
		}

		/// <summary>
		/// Arbitrary precision cosecant function. 
		/// The input must not be zero or π, as the cosecant is undefined at that value.
		/// </summary>
		/// <param name="radians">The argument radians.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		public static BigDecimal Csc(BigDecimal radians, int precision)
		{
			if (radians.IsZero())
			{
				throw new ArithmeticException(LanguageResources.Arithmetic_Trig_Undefined_Csc_Zero);
			}

			BigDecimal modPi = Normalize(Mod(radians, Pi));
			if (modPi.IsZero())
			{
				throw new ArithmeticException(LanguageResources.Arithmetic_Trig_Undefined_Csc_Pi);
			}

			// Wrap around at 2π
			BigDecimal twoPi = Normalize(Mod(radians, 2 * Pi));

			BigDecimal sin = Sin(twoPi, precision);
			return One / sin; // csc = 1 / sin
		}

		#endregion

		#region Hyperbolic Trigonometric Functions

		/// <summary>Arbitrary precision hyperbolic sine function.</summary>
		public static BigDecimal Sinh(BigDecimal radians)
		{
			return Sinh(radians, Precision);
		}

		/// <summary>
		/// Arbitrary precision hyperbolic sine function.
		/// </summary>
		/// <param name="radians">The argument radians.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		public static BigDecimal Sinh(BigDecimal radians, int precision)
		{
			BigDecimal sumStart = 0;
			BigInteger counterStart = 1;
			BigInteger jump = 2;
			BigInteger multiplier = 1;
			bool factorialDenominator = true;

			return TrigonometricHelper.TaylorSeriesSum(radians, sumStart, counterStart, jump, multiplier, factorialDenominator, precision);
		}

		/// <summary>Arbitrary precision Hyperbolic cosine function.</summary>
		public static BigDecimal Cosh(BigDecimal radians)
		{
			return Cosh(radians, Precision);
		}

		/// <summary>
		/// Arbitrary precision Hyperbolic cosine function.
		/// </summary>
		/// <param name="radians">The argument radians.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		public static BigDecimal Cosh(BigDecimal radians, int precision)
		{
			BigDecimal sumStart = 1;
			BigInteger counterStart = 2;
			BigInteger jump = 2;
			BigInteger multiplier = 1;
			bool factorialDenominator = true;

			return TrigonometricHelper.TaylorSeriesSum(radians, sumStart, counterStart, jump, multiplier, factorialDenominator, precision);
		}

		/// <summary>Arbitrary precision hyperbolic tangent function.</summary>
		public static BigDecimal Tanh(BigDecimal radians)
		{
			return Tanh(radians, Precision);
		}

		/// <summary>Arbitrary precision hyperbolic tangent function.</summary>
		/// <param name="radians">The argument radians.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		public static BigDecimal Tanh(BigDecimal radians, int precision)
		{
			BigDecimal sinh = Sinh(radians, precision);
			BigDecimal cosh = Cosh(radians, precision);

			return sinh / cosh; // tan = sinh / cosh
		}

		/// <summary>Arbitrary precision hyperbolic cotangent function.</summary>
		public static BigDecimal Coth(BigDecimal radians)
		{
			return Coth(radians, Precision);
		}

		/// <summary>Arbitrary precision hyperbolic cotangent function.</summary>
		/// <param name="radians">The argument radians.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		public static BigDecimal Coth(BigDecimal radians, int precision)
		{
			BigDecimal cosh = Cosh(radians, precision);
			BigDecimal sinh = Sinh(radians, precision);

			return cosh / sinh; // coth = cosh / sinh
		}

		/// <summary>Arbitrary precision hyperbolic secant function.</summary>
		public static BigDecimal Sech(BigDecimal radians)
		{
			return Sech(radians, Precision);
		}

		/// <summary>Arbitrary precision hyperbolic secant function.</summary>
		/// <param name="radians">The argument radians.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		public static BigDecimal Sech(BigDecimal radians, int precision)
		{
			BigDecimal cosh = Cosh(radians, precision);

			return One / cosh;   // sech = 1 / cosh
		}

		/// <summary>
		/// Arbitrary precision hyperbolic cosecant function.
		/// The input must not be zero.
		/// </summary>
		public static BigDecimal Csch(BigDecimal radians)
		{
			return Csch(radians, Precision);
		}

		/// <summary>
		/// Arbitrary precision hyperbolic cosecant function.
		/// The input must not be zero.
		/// </summary>
		/// <param name="radians">The argument radians.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		public static BigDecimal Csch(BigDecimal radians, int precision)
		{
			BigDecimal input = Normalize(radians);
			if (input.IsZero())
			{
				throw new ArithmeticException(LanguageResources.Arithmetic_Trig_Undefined_Csch_Zero);
			}

			BigDecimal sinh = Sinh(input, precision);

			return One / sinh;   // csch = 1 / sinh
		}

		#endregion

		#region Inverse Trigonometric Functions

		/// <summary>Arbitrary precision inverse sine function.</summary>
		public static BigDecimal Arcsin(BigDecimal radians)
		{
			return Arcsin(radians, Precision);
		}

		/// <summary>Arbitrary precision inverse sine function.</summary>
		/// <param name="radians">The argument radians.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		public static BigDecimal Arcsin(BigDecimal radians, int precision)
		{
			int sign = radians.Sign;
			BigDecimal input = Mod(Abs(radians), One);
			BigDecimal denominator = SquareRoot(One - (input * input), precision);
			BigDecimal quotient = input / denominator;
			return sign * Arctan(quotient, precision);
		}

		/// <summary>Arbitrary precision inverse cosine function.</summary>
		public static BigDecimal Arccos(BigDecimal radians)
		{
			return Arccos(radians, Precision);
		}

		/// <summary>Arbitrary precision inverse cosine function.</summary>
		/// <param name="radians">The argument radians.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		public static BigDecimal Arccos(BigDecimal radians, int precision)
		{
			int sign = radians.Sign;
			BigDecimal input = Mod(Abs(radians), One);
			BigDecimal denominator = SquareRoot(One - (input * input), precision);
			BigDecimal quotient = denominator / input;
			if (sign == -1)
			{
				return Pi - Arctan(quotient, precision);
			}
			return Arctan(quotient, precision);
		}

		/// <summary>Arbitrary precision inverse tangent function.</summary>
		public static BigDecimal Arctan(BigDecimal radians)
		{
			return Arctan(radians, Precision);
		}

		/// <summary>Arbitrary precision inverse tangent function.</summary>
		/// <param name="radians">The argument radians.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		public static BigDecimal Arctan(BigDecimal radians, int precision)
		{
			int sign = radians.Sign;
			BigDecimal input = new BigDecimal(radians.Mantissa, radians.Exponent);

			bool needsAdjustment = false;
			if (sign == 0)
			{
				return 0;
			}

			if (sign == -1)
			{
				needsAdjustment = input < -1;
			}
			else
			{
				needsAdjustment = input > 1;
			}

			if (needsAdjustment)
			{
				input = One / input;
			}

			input = Abs(input);

			BigDecimal sumStart = 0;
			BigInteger counterStart = 1;
			BigInteger jump = 2;
			BigInteger multiplier = -1;
			bool factorialDenominator = false;

			BigDecimal result = TrigonometricHelper.TaylorSeriesSum(input, sumStart, counterStart, jump, multiplier, factorialDenominator, precision);

			result = result * sign;

			if (needsAdjustment)
			{
				result = (sign * TrigonometricHelper.HalfPi) - result;
			}

			return result;
		}

		/// <summary>Arbitrary precision inverse cotangent function.</summary>
		public static BigDecimal Arccot(BigDecimal radians)
		{
			return Arccot(radians, Precision);
		}

		/// <summary>Arbitrary precision inverse cotangent function.</summary>
		/// <param name="radians">The argument radians.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		public static BigDecimal Arccot(BigDecimal radians, int precision)
		{
			return TrigonometricHelper.HalfPi - Arctan(radians, precision);
		}

		/// <summary>Arbitrary precision inverse cosecant function.</summary>
		public static BigDecimal Arccsc(BigDecimal radians)
		{
			return Arccsc(radians, Precision);
		}

		/// <summary>Arbitrary precision inverse cosecant function.</summary>
		/// <param name="radians">The argument radians.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		public static BigDecimal Arccsc(BigDecimal radians, int precision)
		{
			return Arcsin(One / radians, precision);
		}

		#endregion

		#region Natural Log & Exponentiation Function

		/// <summary>Calculates e^x to arbitrary precision.</summary>
		public static BigDecimal Exp(BigDecimal x)
		{
			return Exp(x, Precision);
		}

		/// <summary>Calculates e^x to arbitrary precision.</summary>
		/// <param name="x">The exponent to raise e to the power of.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		public static BigDecimal Exp(BigDecimal x, int precision)
		{
			BigDecimal sumStart = 1;
			BigInteger counterStart = 1;
			BigInteger jump = 1;
			BigInteger multiplier = 1;
			bool factorialDenominator = true;

			return TrigonometricHelper.TaylorSeriesSum(x, sumStart, counterStart, jump, multiplier, factorialDenominator, precision);
		}

		/// <summary>
		/// Returns the natural logarithm of the input.
		/// </summary>
		/// <param name="argument">The argument to take the natural logarithm of.</param>
		public static BigDecimal Ln(BigDecimal argument)
		{
			return Ln(argument, Precision);
		}

		/// <summary>
		/// Returns the natural logarithm of the input to a specified precision.
		/// </summary>
		/// <param name="argument">The argument to take the natural logarithm of.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>	
		public static BigDecimal Ln(BigDecimal argument, int precision)
		{
			BigDecimal x = new BigDecimal(argument.Mantissa, argument.Exponent);

			BigDecimal result = 0;
			while (x > ChunkSize)
			{
				var cbrt = Round(NthRoot(x, 3, precision));
				x /= cbrt;
				x /= cbrt;
				result += Ln(cbrt, precision);
				result += Ln(cbrt, precision);
			}

			result += LogNatural(x, precision);

			return result;
		}
		private static BigDecimal ChunkSize = 1_000_000_000;

		/// <summary>
		/// Internal implementation of the natural log function to arbitrary precision.
		/// </summary>	
		/// <param name="argument">The argument to take the natural logarithm of.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		internal static BigDecimal LogNatural(BigDecimal argument, int precision)
		{
			BigDecimal targetPrecision = TrigonometricHelper.GetPrecisionTarget(precision);
			BigDecimal two = new BigDecimal(2);
			BigDecimal x = new BigDecimal(argument.Mantissa, argument.Exponent);

			bool firstRound = true;
			BigDecimal nextGuess = 0;
			BigDecimal currentGuess = 14;
			BigDecimal difference = 1;

			int counter = 0;
			do
			{
				BigDecimal expGuess = Exp(currentGuess);

				BigDecimal num = x - expGuess;
				BigDecimal denom = x + expGuess;

				BigDecimal quotient = two * (num / denom);

				nextGuess = currentGuess + quotient;

				if (firstRound)
				{
					firstRound = false;
				}
				else
				{
					difference = nextGuess - currentGuess;
				}

				currentGuess = nextGuess;
				counter++;
			}
			while (Abs(difference) > targetPrecision);

			return currentGuess;
		}

		#endregion

		#region Arbitrary Base Logarithm

		/// <summary>
		/// Returns the logarithm of an argument in an arbitrary base.
		/// </summary>
		/// <param name="base">The base of the logarithm.</param>
		/// <param name="argument">The argument to take the logarithm of.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		public static BigDecimal LogN(int @base, BigDecimal argument, int precision)
		{
			// Use change of base formula: logn(b, a) = ln(a) / ln(b)
			return Ln(argument, precision) / Ln(@base, precision);
		}

		/// <summary>
		/// Returns the base-2 logarithm of an argument.
		/// </summary>
		/// <param name="argument">The argument to take the base-2 logarithm of.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		public static BigDecimal Log2(BigDecimal argument, int precision)
		{
			return LogN(2, argument, precision);
		}

		/// <summary>
		/// Returns the base-10 logarithm of an argument.
		/// </summary>
		/// <param name="argument">The argument to take the base-10 logarithm of.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		public static BigDecimal Log10(BigDecimal argument, int precision)
		{
			return LogN(10, argument, precision);
		}

		#endregion

		#endregion

		public override Int32 GetHashCode() => new Tuple<BigInteger, Int32>(this.Mantissa, this.Exponent).GetHashCode();

		public override String ToString() => this.ToString(BigDecimalNumberFormatInfo);

		public String ToString(IFormatProvider provider) => ToString(this.Mantissa, this.Exponent, provider);

		private static String ToString(BigInteger mantissa, Int32 exponent, IFormatProvider? provider = null)
		{
			provider ??= CultureInfo.CurrentCulture;

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

			if (negativeValue)
			{

				// Prefix "-"
				result = result.Insert(0, formatProvider.NegativeSign);
			}

			return result;
		}

		/// <summary>Allow the BigDecimal to be formatted with the E notation.</summary>
		/// <param name="bigDecimal"></param>
		/// <returns></returns>
		public static String ToScientificENotation(BigDecimal bigDecimal)
		{
			// 1.238E456 1.238E-456
			// -1.238E456
			// -1.238E-456 1E456 1E-456
			// -1E456
			// -1E-456
			// -3E-2

			string mantissa = bigDecimal.Mantissa.ToString(); //Note: will be prefixed with "-" if negative.

			int exponent = bigDecimal.Exponent + (bigDecimal.SignifigantDigits - 1);
			int point = 1;
			if (bigDecimal.Mantissa.Sign == -1)
			{
				point += 1;
			}

			//TODO none of this is tested or guaranteed to work yet. Like negatives, or small numbers need the correct logic.
			string sign = "+";
			if (Math.Sign(exponent) == -1)
			{
				sign = "-";
			}
			var result = $"{mantissa.Insert(point, BigDecimalNumberFormatInfo.NumberDecimalSeparator)}E{sign}{Math.Abs(exponent)}";
			return result;
		}
	}
}