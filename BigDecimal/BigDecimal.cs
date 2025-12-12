using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using ExtendedNumerics.Helpers;
using ExtendedNumerics.Properties;

namespace ExtendedNumerics
{
	/// <summary>
	/// An arbitrary precision floating point number type.
	/// </summary>
	/// <remarks> 
	/// <para>Arbitrary precision decimal. All operations are exact, except for division.</para>
	/// <para>Division never determines more digits than the given precision.</para>
	/// <para>Based on code by Jan Christoph Bernack (https://gist.github.com/JcBernack/0b4eef59ca97ee931a2f45542b9ff06d)</para>
	/// <para>Modified, extended and maintained by Adam White (https://github.com/AdamWhiteHat or adamwhitehat 𝚊𝚝 outlook 𝖽𝗈𝗍 com)</para>
	/// <para>Contributions by Rick Harker (Rick.Rick.Harker 𝖺𝗍 gmail 𝚍𝚘𝚝 com) and Protiguous (https://github.com/Protiguous)</para> 
	/// </remarks>
	public readonly partial record struct BigDecimal : IComparable, IComparable<BigDecimal>, IComparable<Int32>, IComparable<Int32?>, IComparable<Decimal>, IComparable<Double>, IComparable<Single>
	{
		#region Constructors

		/// <summary>Private Constructor. This one bypasses <see cref="AlwaysTruncate"/> and <see cref="AlwaysNormalize"/> check and behavior.</summary>
		/// <param name="tuple"></param>
		private BigDecimal(Tuple<BigInteger, Int32> tuple)
		{
			this.Mantissa = tuple.Item1;
			this.Exponent = tuple.Item2;
		}

		/// <summary>Initializes a new instance of <see cref="BigDecimal" /> from a fraction, specified as a numerator and a denominator.</summary>
		public BigDecimal(BigInteger numerator, BigInteger denominator)
		{
			BigDecimal quotient = Divide(new BigDecimal(numerator), new BigDecimal(denominator));
			this.Mantissa = quotient.Mantissa;
			this.Exponent = quotient.Exponent;
		}

		/// <summary>Initializes a new instance of <see cref="BigDecimal" /> from a mantissa and an exponent.</summary>
		public BigDecimal(BigInteger mantissa, Int32 exponent = 0)
		{
			this.Mantissa = mantissa;
			this.Exponent = exponent;
			BigDecimal result;
			if (AlwaysTruncate)
			{
				result = Truncate(this, Precision);
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

		/// <summary>Initializes a new instance of <see cref="BigDecimal" /> from an <see cref="Int32"/>.</summary>
		public BigDecimal(Int32 value) : this(new BigInteger(value)) { }

		/// <summary>Initializes a new instance of <see cref="BigDecimal" /> from a <see cref="BigInteger"/>.</summary>
		public BigDecimal(BigInteger value) : this(value, 0) { }

		/// <summary>Initializes a new instance of <see cref="BigDecimal" /> from a <see cref="Single"/>.</summary>
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

		/// <summary>Initializes a new instance of <see cref="BigDecimal" /> from a <see cref="Double"/>.</summary>
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

		/// <summary>Initializes a new instance of <see cref="BigDecimal" /> from a <see cref="Decimal"/>.</summary>
		public BigDecimal(Decimal value)
		{
			Int32 exponent = ((Decimal.GetBits(value)[3] & DecimalScaleMask) >> 16);
			decimal value_scaled = value * (decimal)Math.Pow(10, exponent);
			this.Mantissa = new BigInteger(value_scaled);
			this.Exponent = -exponent;
		}

		#endregion

		#region Static Members

		/// <summary>Gets a value that represents the number 10 (ten).</summary>
		public static BigDecimal Ten => new BigDecimal(new Tuple<BigInteger, Int32>(1, 1));

		/// <summary>Gets a value that represents the number 1 (one).</summary>
		public static BigDecimal One => new BigDecimal(new Tuple<BigInteger, Int32>(1, 0));

		/// <summary>Gets a value that represents the number 0 (zero).</summary>
		public static BigDecimal Zero => new BigDecimal(new Tuple<BigInteger, Int32>(0, 0));

		/// <summary>Gets a value that represents the number 0.5 (one half).</summary>
		public static BigDecimal OneHalf => new BigDecimal(new Tuple<BigInteger, Int32>(5, -1));

		/// <summary>Gets a value that represents the number -1 (negative one).</summary>
		public static BigDecimal MinusOne => new BigDecimal(new Tuple<BigInteger, Int32>(-1, 0));

		/// <summary>Gets a value that represents the number e, also called Euler's number.</summary>
		public static BigDecimal E { get { return ApproximateE(Precision); } }

		/// <summary>Gets a value that represents the number Pi.</summary>
		public static BigDecimal Pi { get { return ApproximatePi(Precision); } }

		/// <summary>Gets a value that represents the number Pi.</summary>
		public static BigDecimal π { get { return ApproximatePi(Precision); } }

		/// <summary>
		/// Sets the desired precision of all <see cref="BigDecimal" /> instances, in terms of the number of digits to the right of the decimal.
		/// <para>The default value is <c>100</c>.</para> 
		/// </summary>
		/// <remarks>If <see cref="AlwaysTruncate"/> is set to <see langword="true"/> all operations are affected.</remarks>
		public static Int32 Precision { get; set; } = 100;

		/// <summary>
		/// Specifies whether the significant digits should be truncated to the given precision after each operation.
		/// <para>The default value is <see langword="false"/>.</para>
		/// </summary>
		/// <remarks>
		/// Setting this to true will tend to accumulate errors at the precision boundary after several arithmetic operations.
		/// Therefore, you should prefer using <see cref="Round(BigDecimal, int)"/> explicitly when you need it instead, 
		/// such st at the end of a series of operations, especially if you are expecting the result to be truncated at the precision length.
		/// This should generally be left disabled by default.
		/// This setting may be useful if you are running into memory or performance issues, as could conceivably be brought on by many operations on irrational numbers.
		/// </remarks>
		public static Boolean AlwaysTruncate { get; set; } = false;

		/// <summary>
		/// Specifies whether a call to Normalize is made after every operation and during constructor invocation.
		/// <para>The default value is <see langword="true"/>.</para>
		/// </summary>
		public static Boolean AlwaysNormalize { get; set; } = true;

		/// <summary>
		/// Default mid-point rounding strategy when calling <see cref="Round(BigDecimal, int)"/>.
		/// <para>The default value is <see cref="RoundingStrategy.ToEven"/>.</para>
		/// </summary>
		/// <remarks>
		/// This was introduced when the default rounding strategy changed (a breaking change) so those who relied upon it
		/// can recover the old behavior by setting this to <see cref="RoundingStrategy.AwayFromZero"/>.
		/// </remarks>
		public static RoundingStrategy DefaultRoundingStrategy { get; set; } = RoundingStrategy.AwayFromZero;

		#endregion

		#region Public Properties and Property-like Members

		/// <summary>The mantissa of the internal floating point number representation of this <see cref="BigDecimal" />.</summary>
		public readonly BigInteger Mantissa;

		/// <summary>The exponent of the internal floating point number representation of this <see cref="BigDecimal" />.</summary>
		public readonly Int32 Exponent;

		/// <summary>Gets a number that indicates the sign (negative, positive, or zero) of the current <see cref="BigDecimal" /> object. </summary>
		/// <returns>-1 if the value of this object is negative, 0 if the value of this object is zero or 1 if the value of this object is positive.</returns>
		public Int32 Sign => this.Mantissa.Sign;

		/// <summary>Gets the number of significant digits in <see cref="BigDecimal" />.
		///Essentially tells you the number of digits in the mantissa.</summary>
		public Int32 SignificantDigits => GetSignificantDigits(this.Mantissa);

		/// <summary>The length of the <see cref="BigDecimal" /> value (Equivalent to <see cref="SignificantDigits"/>).</summary>
		public Int32 Length => GetSignificantDigits(this.Mantissa) + this.Exponent;

		/// <summary>Returns the number of digits to the right of the decimal point. Same thing as the output of <see cref="PlacesRightOfDecimal"/></summary>
		public Int32 DecimalPlaces => PlacesRightOfDecimal(this);

		/// <summary>
		/// Gets the whole-number integer (positive or negative) value of this <see cref="BigDecimal" />, so everything to the left of the decimal place.
		/// Equivalent to the Truncate function for a <see langword="float"/>.
		/// </summary>
		public BigInteger WholeValue => this.GetWholePart();

		/// <summary>This method returns <see langword="true"/> if the <see cref="BigDecimal" /> is equal to zero, <see langword="false"/> otherwise.</summary>
		public Boolean IsZero() => this.Mantissa.IsZero;

		/// <summary>This method returns <see langword="true"/> if the <see cref="BigDecimal" /> is greater than zero, <see langword="false"/> otherwise.</summary>
		public Boolean IsPositive() => !this.IsZero() && !this.IsNegative();

		/// <summary>This method returns <see langword="true"/> if the <see cref="BigDecimal" /> is less than zero, <see langword="false"/> otherwise.</summary>
		public Boolean IsNegative() => this.Mantissa.Sign < 0;

		#endregion

		#region Private Members

		private static BigInteger TenInt { get; } = new BigInteger(10);
		private static BigDecimal MaxBigDecimalForDecimal => (BigDecimal)Decimal.MaxValue;
		private static BigDecimal MaxBigDecimalForSingle => (BigDecimal)Single.MaxValue;
		private static BigDecimal MaxBigDecimalForDouble => (BigDecimal)Double.MaxValue;
		private static BigDecimal MaxBigDemicalForInt32 => (BigDecimal)Int32.MaxValue;
		private static BigDecimal MaxBigDemicalForUInt32 => (BigDecimal)UInt32.MaxValue;
		private static Int32 GetSignificantDigits(BigInteger value) => value.GetSignificantDigits();
		private const Int32 DecimalScaleMask = 0x00FF0000;
		private static NumberFormatInfo BigDecimalNumberFormatInfo = new System.Globalization.NumberFormatInfo()
		{
			NativeDigits = CultureInfo.CurrentCulture.NumberFormat.NativeDigits,
			PositiveSign = CultureInfo.CurrentCulture.NumberFormat.PositiveSign,
			NegativeSign = CultureInfo.CurrentCulture.NumberFormat.NegativeSign,
			NumberNegativePattern = CultureInfo.CurrentCulture.NumberFormat.NumberNegativePattern,
			NumberGroupSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator,
			NumberGroupSizes = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSizes,
			NumberDecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator,
			NumberDecimalDigits = 0
		};

		#endregion

		#region CompareTo

		/// <summary>
		/// Compares the current instance to a second <see cref="BigDecimal"/> and returns
		/// an integer that indicates whether the current instance precedes, follows, or
		/// occurs in the same position in the sort order as the other value.
		/// </summary>
		/// <param name="other">The other value to compare with this instance.</param>
		/// <returns>
		/// A return value of less than zero means this instance precedes the other value in the sort order.
		/// A return value of zero means  this instance occurs in the same position in the sort order as the other value.
		/// A return value of greater than zero means this instance follows the other value in the sort order.
		/// </returns>
		public Int32 CompareTo(BigDecimal other) =>
				this < other ? SortingOrder.Before :
				this > other ? SortingOrder.After : SortingOrder.Same;

		/// <summary>
		/// Compares the current instance to a <see cref="Decimal"/> floating point value and returns
		/// an integer that indicates whether the current instance precedes, follows, or
		/// occurs in the same position in the sort order as the <see cref="Decimal"/> value.
		/// </summary>
		/// <param name="other">The <see cref="Decimal"/> value to compare with this instance.</param>
		/// <returns>
		/// A return value of less than zero means this instance precedes the <see cref="Decimal"/> value in the sort order.
		/// A return value of zero means  this instance occurs in the same position in the sort order as the <see cref="Decimal"/> value.
		/// A return value of greater than zero means this instance follows the <see cref="Decimal"/> value in the sort order.
		/// </returns>
		public Int32 CompareTo(Decimal other) =>
			this < other ? SortingOrder.Before :
			this > other ? SortingOrder.After : SortingOrder.Same;

		/// <summary>
		/// Compares the current instance to a double-precision floating-point value and returns
		/// an integer that indicates whether the current instance precedes, follows, or
		/// occurs in the same position in the sort order as the double-precision floating-point value.
		/// </summary>
		/// <param name="other">The double-precision floating-point value to compare with this instance.</param>
		/// <returns>
		/// A return value of less than zero means this instance precedes the double-precision floating-point value in the sort order.
		/// A return value of zero means  this instance occurs in the same position in the sort order as the double-precision floating-point value.
		/// A return value of greater than zero means this instance follows the double-precision floating-point value in the sort order.
		/// </returns>
		public Int32 CompareTo(Double other) =>
			this < other ? SortingOrder.Before :
			this > other ? SortingOrder.After : SortingOrder.Same;

		/// <summary>
		/// Compares the current instance to a single-precision floating-point value and returns
		/// an integer that indicates whether the current instance precedes, follows, or
		/// occurs in the same position in the sort order as the single-precision floating-point value.
		/// </summary>
		/// <param name="other">The single-precision floating-point value to compare with this instance.</param>
		/// <returns>
		/// A return value of less than zero means this instance precedes the single-precision floating-point value in the sort order.
		/// A return value of zero means  this instance occurs in the same position in the sort order as the single-precision floating-point value.
		/// A return value of greater than zero means this instance follows the single-precision floating-point value in the sort order.
		/// </returns>
		public Int32 CompareTo(Single other) =>
			this < other ? SortingOrder.Before :
			this > other ? SortingOrder.After : SortingOrder.Same;

		/// <summary>
		/// Compares the current instance to a nullable <see cref="Int32"/> value and returns
		/// an integer that indicates whether the current instance precedes, follows, or
		/// occurs in the same position in the sort order as the nullable <see cref="Int32"/> value.
		/// </summary>
		/// <param name="other">The nullable <see cref="Int32"/> value to compare with this instance.</param>
		/// <returns>
		/// A return value of less than zero means this instance precedes the nullable <see cref="Int32"/> value in the sort order.
		/// A return value of zero means  this instance occurs in the same position in the sort order as the nullable <see cref="Int32"/> value.
		/// A return value of greater than zero means this instance follows the nullable <see cref="Int32"/> value in the sort order.
		/// </returns>
		public Int32 CompareTo(Int32? other) =>
			other is null || (this > other) ? SortingOrder.After :
			this < other ? SortingOrder.Before : SortingOrder.Same;

		/// <summary>
		/// Compares the current instance to a 32-bit signed integer and returns
		/// an integer that indicates whether the current instance precedes, follows, or
		/// occurs in the same position in the sort order as the 32-bit signed integer.
		/// </summary>
		/// <param name="other">The 32-bit signed integer to compare with this instance.</param>
		/// <returns>
		/// A return value of less than zero means this instance precedes the 32-bit signed integer in the sort order.
		/// A return value of zero means  this instance occurs in the same position in the sort order as the 32-bit signed integer.
		/// A return value of greater than zero means this instance follows the 32-bit signed integer in the sort order.
		/// </returns>
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

		#endregion

		#region Equals

		public Boolean Equals(BigDecimal? other) => Equals(this, other);

		/// <summary>
		/// Determines if two instances of <see cref="BigDecimal" /> are equal. 
		/// The precise behavior of this method depends on the values of the static members <see cref="AlwaysTruncate"/> and <see cref="AlwaysNormalize"/>.
		/// If <see cref="AlwaysTruncate"/> or <see cref="AlwaysNormalize"/> is true, then both values are rounded at <see cref="Precision"/> 
		/// or normalized with <see cref="Normalize(BigDecimal)"/>, respectively.
		/// Then their mantissas and exponents are compared.
		/// If <see cref="AlwaysTruncate"/> is false, then their mantissas and exponents are compared exactly as they are, to as many digits as is tracked by their instances
		/// (which may well exceed <see cref="Precision"/> and will be unlikely to have the same number of digits,
		/// unless the same number of multiplications and divisions were performed on them).
		/// If <see cref="AlwaysTruncate"/> is false, it is recommended that you use the overload of <see cref="Equals(BigDecimal?, BigDecimal?, int)"/> that takes a precision parameter,
		/// or you round both values off to some the level of precision that you care about first before calling this method.
		/// </summary>
		/// <returns>True if the two numbers are equal.</returns>
		public static Boolean Equals(BigDecimal? left, BigDecimal? right)
		{
			if (left == null || right == null)
			{
				return (left == null && right == null);
			}

			BigDecimal l = left.Value;
			BigDecimal r = right.Value;
			if (AlwaysNormalize)
			{
				l = Normalize(l);
				r = Normalize(r);
			}
			if (AlwaysTruncate)
			{
				l = Truncate(l, Precision);
				r = Truncate(r, Precision);
			}

			return l.Sign.Equals(r.Sign) && l.Exponent.Equals(r.Exponent) && l.Mantissa.Equals(r.Mantissa);
		}

		/// <summary>
		/// Determines if two instances of <see cref="BigDecimal" /> are equal, up to <paramref name="precision"/> digits.
		/// The precise behavior of this method depends on the value of <see cref="AlwaysTruncate"/>.
		/// If <see cref="AlwaysTruncate"/> is true, then both arguments will first be rounded to <paramref name="precision"/> or <see cref="Precision"/> decimal places, 
		/// which ever is smaller. Then they will be checked for equivalency. 
		/// If <see cref="AlwaysTruncate"/> is false, then this method will behave as expected.
		/// </summary>
		/// <param name="precision">
		/// The number of digits to the right of the decimal place to perform the comparison to.  Any digits beyond this, will be ignored. 
		/// <see cref="Precision"/> supersedes this parameter, if <see cref="AlwaysTruncate"/> is true. 
		/// If <see cref="AlwaysTruncate"/> is true and <see cref="Precision"/> is smaller than <paramref name="precision"/>, then this parameter is effectively ignored.
		/// </param>
		/// <returns>True if the two numbers are equal, up to <paramref name="precision"/>.</returns>
		public static Boolean Equals(BigDecimal? left, BigDecimal? right, int precision)
		{
			if (left == null || right == null)
			{
				return (left == null && right == null);
			}

			BigDecimal l = left.Value;
			BigDecimal r = right.Value;
			if (AlwaysNormalize)
			{
				l = Normalize(l);
				r = Normalize(r);
			}

			int prec = precision;
			if (AlwaysTruncate)
			{
				prec = Math.Min(prec, Precision);
			}

			l = Round(l, prec);
			r = Round(r, prec);

			return l.Sign.Equals(r.Sign) && l.Exponent.Equals(r.Exponent) && l.Mantissa.Equals(r.Mantissa);
		}

		#endregion

		#region Parse

		/// <summary>Converts the string representation of a <see langword="float"/> to the <see cref="BigDecimal" /> equivalent.</summary>
		public static BigDecimal Parse(Single input) => Parse(input.ToString("R", BigDecimalNumberFormatInfo));

		/// <summary>Converts the string representation of a <see langword="double"/> to the <see cref="BigDecimal" /> equivalent.</summary>
		public static BigDecimal Parse(Double input) => Parse(input.ToString("R", BigDecimalNumberFormatInfo));

		/// <summary>Converts the string representation of a <see langword="decimal"/> to the <see cref="BigDecimal" /> equivalent.</summary>
		public static BigDecimal Parse(Decimal input) => new BigDecimal(input);

		/// <summary>Converts the string representation of a <see langword="decimal"/> to the <see cref="BigDecimal" /> equivalent.</summary>
		/// <param name="input">A string that contains a number to convert.</param>
		/// <returns>
		/// A BigDecimal equivalent of the supplied string representation of a decimal number,
		/// or zero if the input string was null, empty or whitespace.
		/// </returns>
		public static BigDecimal Parse(String input)
		{
			return Parse(input, BigDecimalNumberFormatInfo);
		}

		/// <summary>
		/// Converts the <see langword="string"/> representation of a decimal in a specified culture-specific format to its BigDecimal equivalent.
		/// </summary>
		/// <param name="input">A string that contains a number to convert.</param>
		/// <param name="provider">An object that provides culture-specific formatting information about value.</param>
		/// <returns>
		/// A BigDecimal equivalent of the supplied string representation of a decimal number in the specified culture-specific format,
		/// or zero if the input string was null, empty or whitespace.
		/// </returns>
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
				result = Truncate(result, Precision);
			}
			if (AlwaysNormalize)
			{
				result = Normalize(result);
			}
			return result;
		}

		/// <summary>
		/// Tries to convert the <see langword="string"/> representation of a number to its BigDecimal equivalent, and returns a value that indicates whether the conversion succeeded.
		/// </summary>
		/// <param name="input">The string representation of a number.</param>
		/// <param name="result">When this method returns, this out parameter contains the BigDecimal equivalent
		/// to the number that is contained in value, or default(BigDecimal) if the conversion fails.
		/// The conversion fails if the value parameter is null or is not of the correct format.</param>
		/// <returns>
		/// True if parsing was successful and the out parameter contains the parsed value.
		/// False if parsing failed and the out parameter contains default(BigDecimal).
		/// </returns>
		public static bool TryParse(String input, out BigDecimal result)
		{
			return TryParse(input, BigDecimalNumberFormatInfo, out result);
		}

		/// <summary>
		/// Tries to convert the string representation of a number in a specified style and culture-specific format
		/// to its <see cref="BigDecimal" /> equivalent and passing the result to the out parameter if successful.
		/// Returns a boolean value that indicates whether the conversion was successful.
		/// </summary>
		/// <param name="input">The string representation of a number.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information about value.</param>
		/// <param name="result">When this method returns, this out parameter contains the BigDecimal equivalent
		/// to the number that is contained in value, or default(BigDecimal) if the conversion fails.
		/// The conversion fails if the value parameter is null or is not of the correct format.</param>
		/// <returns>
		/// True if parsing was successful and the out parameter contains the parsed value.
		/// False if parsing failed and the out parameter contains default(BigDecimal).
		/// </returns>
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

		#endregion

		#region Get Decimal Parts

		/// <summary> Returns the number of digits to the right of the decimal point.</summary>
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

			var s = value.Mantissa.ToString(BigDecimalNumberFormatInfo);
			var pos = s.LastIndexOf(BigDecimalNumberFormatInfo.NativeDigits[0][0], s.Length - 1);

			if (pos < (s.Length - 1))
			{
				return value;
			}

			var c = s[pos];

			while ((pos > 0) && (c == BigDecimalNumberFormatInfo.NativeDigits[0][0]))
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

		/// <summary>Returns the zero-based index of the decimal point, if the <see cref="BigDecimal" /> were rendered as a string.</summary>
		public Int32 GetDecimalIndex()
		{
			var mantissaLength = this.Mantissa.GetLength();
			if (this.Mantissa.Sign < 0)
			{
				mantissaLength++;
			}

			return mantissaLength + this.Exponent;
		}

		/// <summary>Returns the whole number integer part of the BigDecimal, dropping anything right of the decimal point.</summary>
		/// <remarks>Essentially behaves like Math.Truncate().</remarks>
		/// <example>BigDecimal.PI.GetWholePart() == 3</example>
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

		/// <summary>Gets the fractional part of the <see cref="BigDecimal" />, setting everything left of the decimal point to zero.</summary>
		public BigDecimal GetFractionalPart()
		{
			var resultString = String.Empty;
			var decimalString = this.ToString(BigDecimalNumberFormatInfo);

			var valueSplit = decimalString.Split(BigDecimalNumberFormatInfo.NumberDecimalSeparator.ToCharArray());
			if (valueSplit.Length == 1)
			{
				return Zero; //BUG Is this right?
			}

			if (valueSplit.Length == 2)
			{
				resultString = valueSplit[1];
			}

			var newmantissa = BigInteger.Parse(resultString.TrimStart(BigDecimalNumberFormatInfo.NativeDigits[0][0]));
			var result = new BigDecimal(newmantissa, 0 - resultString.Length);
			return result;
		}

		#endregion

		#region Conversions & Casts

		/// <summary>Performs an implicit conversion of a <see cref="BigInteger"/> value to a <see cref="BigDecimal"/> value.</summary>
		public static implicit operator BigDecimal(BigInteger value) => new BigDecimal(value, 0);

		/// <summary>Performs an implicit conversion of a <see cref="Byte"/> value to a <see cref="BigDecimal"/> value.</summary>
		public static implicit operator BigDecimal(Byte value) => new BigDecimal(new BigInteger(value), 0);

		/// <summary>Performs an implicit conversion of a <see cref="SByte"/> value to a <see cref="BigDecimal"/> value.</summary>
		public static implicit operator BigDecimal(SByte value) => new BigDecimal(new BigInteger(value), 0);

		/// <summary>Performs an implicit conversion of a <see cref="UInt32"/> value to a <see cref="BigDecimal"/> value.</summary>
		public static implicit operator BigDecimal(UInt32 value) => new BigDecimal(new BigInteger(value), 0);

		/// <summary>Performs an implicit conversion of a <see cref="Int32"/> value to a <see cref="BigDecimal"/> value.</summary>
		public static implicit operator BigDecimal(Int32 value) => new BigDecimal(new BigInteger(value), 0);

		/// <summary>Performs an implicit conversion of a <see cref="UInt16"/> value to a <see cref="BigDecimal"/> value.</summary>
		public static implicit operator BigDecimal(UInt16 value) => new BigDecimal(new BigInteger(value), 0);

		/// <summary>Performs an implicit conversion of a <see cref="Int16"/> value to a <see cref="BigDecimal"/> value.</summary>
		public static implicit operator BigDecimal(Int16 value) => new BigDecimal(new BigInteger(value), 0);

		/// <summary>Performs an implicit conversion of a <see cref="UInt64"/> value to a <see cref="BigDecimal"/> value.</summary>
		public static implicit operator BigDecimal(UInt64 value) => new BigDecimal(new BigInteger(value), 0);

		/// <summary>Performs an implicit conversion of a <see cref="Int64"/> value to a <see cref="BigDecimal"/> value.</summary>
		public static implicit operator BigDecimal(Int64 value) => new BigDecimal(new BigInteger(value), 0);

		/// <summary>Performs an implicit conversion of a <see cref="Single"/> value to a <see cref="BigDecimal"/> value.</summary>
		public static implicit operator BigDecimal(Single value) => Parse(value);

		/// <summary>Performs an implicit conversion of a <see cref="Decimal"/> value to a <see cref="BigDecimal"/> value.</summary>
		public static implicit operator BigDecimal(Decimal value) => new BigDecimal(value);

		/// <summary>Performs an implicit conversion of a <see cref="Double"/> value to a <see cref="BigDecimal"/> value.</summary>
		public static implicit operator BigDecimal(Double value) => Parse(value);

		/// <summary>Performs an explicit conversion of a <see cref="BigDecimal"/> value to a <see cref="BigInteger"/> value.</summary>
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
			return Convert.ToDouble(value.ToString(BigDecimalNumberFormatInfo));
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
			return Convert.ToSingle(value.ToString(BigDecimalNumberFormatInfo));
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
			return Convert.ToDecimal(value.ToString(BigDecimalNumberFormatInfo));
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
			return Convert.ToInt32(value.ToString(BigDecimalNumberFormatInfo));
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
			return Convert.ToUInt32(value.ToString(BigDecimalNumberFormatInfo));
		}

		#endregion

		#region Operator Overloads

		public static BigDecimal operator %(BigDecimal left, BigDecimal right) => Mod(left, right);

		/// <summary>Returns the value of the <see cref="BigDecimal"/> operand. (The sign of the operand is unchanged.)</summary>
		public static BigDecimal operator +(BigDecimal value) => value;

		/// <summary>Negates a specified <see cref="BigDecimal"/> value. </summary>
		public static BigDecimal operator -(BigDecimal value) => Negate(value);

		/// <summary>Increments a <see cref="BigDecimal"/> value by 1.</summary>
		public static BigDecimal operator ++(BigDecimal value) => Add(value, 1);

		/// <summary>Decrements a <see cref="BigDecimal"/> value by 1.</summary>
		public static BigDecimal operator --(BigDecimal value) => Subtract(value, 1);

		/// <summary>Adds two specified <see cref="BigDecimal"/> values.</summary >
		public static BigDecimal operator +(BigDecimal left, BigDecimal right) => Add(left, right);

		/// <summary>Subtracts two specified <see cref="BigDecimal"/> values.</summary >
		public static BigDecimal operator -(BigDecimal left, BigDecimal right) => Subtract(left, right);

		/// <summary>Multiplies two specified <see cref="BigDecimal"/> values.</summary >
		public static BigDecimal operator *(BigDecimal left, BigDecimal right) => Multiply(left, right);

		/// <summary>Divides two specified <see cref="BigDecimal"/> values.</summary >
		public static BigDecimal operator /(BigDecimal dividend, BigDecimal divisor) => Divide(dividend, divisor);

		#region Comparison Operators

		/// <summary>Returns a value that indicates whether a <see cref="BigDecimal"/> value is less than another <see cref="BigDecimal"/> value.</summary>
		public static Boolean operator <(BigDecimal left, BigDecimal right) => left.Exponent > right.Exponent ? AlignExponent(left, right) < right.Mantissa : left.Mantissa < AlignExponent(right, left);

		/// <summary>Returns a value that indicates whether a <see cref="BigDecimal"/> value is greater than another <see cref="BigDecimal"/> value.</summary>
		public static Boolean operator >(BigDecimal left, BigDecimal right) => left.Exponent > right.Exponent ? AlignExponent(left, right) > right.Mantissa : left.Mantissa > AlignExponent(right, left);

		/// <summary>Returns a value that indicates whether a <see cref="BigDecimal"/> value is less than or equal to another <see cref="BigDecimal"/> value.</summary>
		public static Boolean operator <=(BigDecimal left, BigDecimal right) => left.Exponent > right.Exponent ? AlignExponent(left, right) <= right.Mantissa : left.Mantissa <= AlignExponent(right, left);

		/// <summary>Returns a value that indicates whether a <see cref="BigDecimal"/> value is greater than or equal to another <see cref="BigDecimal"/> value.</summary>
		public static Boolean operator >=(BigDecimal left, BigDecimal right) => left.Exponent > right.Exponent ? AlignExponent(left, right) >= right.Mantissa : left.Mantissa >= AlignExponent(right, left);

		#endregion

		#endregion

		#region Abs, Min, Max, Negate

		/// <summary>Returns the absolute value of the <see cref="BigDecimal" /></summary>
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

		/// <summary>Returns the smaller of two <see cref="BigDecimal" /> values.</summary>
		public static BigDecimal Min(BigDecimal left, BigDecimal right) => left <= right ? left : right;

		/// <summary>Returns the larger of two <see cref="BigDecimal" /> values.</summary>	
		public static BigDecimal Max(BigDecimal left, BigDecimal right) => left >= right ? left : right;

		/// <summary>Returns the result of multiplying a <see cref="BigDecimal" /> by negative one.</summary>
		public static BigDecimal Negate(BigDecimal value) => new BigDecimal(BigInteger.Negate(value.Mantissa), value.Exponent);

		#endregion

		#region Arithmetic

		/// <summary>Adds two <see cref="BigDecimal" /> values.</summary>
		public static BigDecimal Add(BigDecimal left, BigDecimal right)
		{
			if (left.Exponent > right.Exponent)
			{
				return new BigDecimal(AlignExponent(left, right) + right.Mantissa, right.Exponent);
			}
			return new BigDecimal(AlignExponent(right, left) + left.Mantissa, left.Exponent);
		}

		/// <summary>Subtracts two <see cref="BigDecimal" /> values.</summary>
		public static BigDecimal Subtract(BigDecimal left, BigDecimal right) => Add(left, Negate(right));

		/// <summary>Multiplies two <see cref="BigDecimal" /> values.</summary>
		public static BigDecimal Multiply(BigDecimal left, BigDecimal right) =>
			new BigDecimal(left.Mantissa * right.Mantissa, left.Exponent + right.Exponent);

		/// <summary>Divides two <see cref="BigDecimal" /> values, returning the remainder and discarding the quotient.</summary>
		public static BigDecimal Mod(BigDecimal value, BigDecimal mod)
		{
			//TODO Verify if the % overload is correct, and/or this one is. Then merge the two to use the same function.
			// x – q * y
			var quotient = Divide(value, mod);
			var floor = Floor(quotient);
			return Subtract(value, Multiply(floor, mod));
		}

		/// <summary>Divides two <see cref="BigDecimal" /> values.</summary>
		public static BigDecimal Divide(BigDecimal dividend, BigDecimal divisor)
		{
			return Divide(dividend, divisor, Precision);
		}

		/// <summary>Divides two <see cref="BigDecimal" /> values.</summary>
		public static BigDecimal Divide(BigDecimal dividend, BigDecimal divisor, int precision)
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
					if (GetSignificantDigits(mantissa) >= divisor.SignificantDigits)
					{
						break;
					}
				}
				else if (GetSignificantDigits(mantissa) >= precision)
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

		/// <summary>
		/// Returns a specified number raised to the specified power.
		/// Note: This method may be a bit slower than the other Pow overloads. Prefer the other overloads if possible. 
		/// To improve execution speed, set <see cref="BigDecimal.AlwaysTruncate"/> to <see langword="true"/> 
		/// and set <see cref="BigDecimal.Precision"/> to only the precision that you need.
		/// </summary>
		public static BigDecimal Pow(BigDecimal @base, BigDecimal exponent, int precision)
		{
			//  b^p = e^(p*ln(b))
			BigDecimal ln_b = BigDecimal.Ln(@base, precision);
			return BigDecimal.Exp(exponent * ln_b, precision);
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
					throw new ArgumentException(LanguageResources.Arg_NegativePowerOfZero, nameof(@base));
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
					throw new ArgumentException(LanguageResources.Arg_NegativePowerOfZero, nameof(@base));
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
			if (exponent == 0)
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

		/// <summary>Returns the square root of the supplied <see cref="BigDecimal"/> to the given number of places..</summary>
		public static BigDecimal SquareRoot(BigDecimal input, Int32 decimalPlaces)
		{
			return NthRoot(input, 2, decimalPlaces);
		}

		/// <summary> Returns the Nth root of the supplied input decimal to the given number of places.</summary>
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

		#endregion

		#region Private Methods

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

		#endregion

		#region Rounding, Floor, Ceiling, Truncate

		/// <summary>Truncates the <see cref="BigDecimal" /> at the decimal point. Equivalent to using Floor.</summary>
		public static BigDecimal Truncate(BigDecimal value)
		{
			return (value.Sign == -1) ? Ceiling(value) : Floor(value);
		}

		/// <summary>Truncates the <see cref="BigDecimal" /> at the specified precision (number of digits to the right).</summary>
		/// <remarks>
		/// Accepts negative precision values, which zeros out digits to the left of the decimal point, with the absolute value of precision equaling the number of places to zero.
		/// A precision value of zero truncates the BigDecimal at the decimal point, returning the integer part only.
		/// This method is functionally identical to Excel's ROUNDDOWN function.
		/// </remarks>
		public static BigDecimal Truncate(BigDecimal value, int precision)
		{
			if (precision < 0)
			{
				return Round(value, precision);
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

		/// <summary>Rounds a <see cref="BigDecimal" /> value to the nearest integral value.</summary>
		public static BigInteger Round(BigDecimal value) => Round(value, DefaultRoundingStrategy);

		/// <summary>Rounds a <see cref="BigDecimal" /> value to the nearest integral value. A parameter specifies how to round the value if it is midway between two numbers.</summary>
		public static BigInteger Round(BigDecimal value, RoundingStrategy mode)
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
				if (mode == RoundingStrategy.AwayFromZero)
				{
					wholePart += addOne;
				}
				else if (mode == RoundingStrategy.ToEven) // RoundingStrategy.ToEven
				{
					if (!wholePart.IsEven)
					{
						wholePart += addOne;
					}
				}
				else
				{
					throw new NotImplementedException($"You must have added a new {nameof(RoundingStrategy)} enum value but not implemented it. Please add the implementation here.");
				}
			}
			return wholePart;
		}

		/// <summary>Rounds a <see cref="BigDecimal" /> value off at the specified level of precision. A parameter specifies how to round the value if it is midway between two values.</summary>
		public static BigDecimal Round(BigDecimal value, int precision, RoundingStrategy roundingStrategy)
		{
			if (precision < 0)
			{
				var roundingMethod = (Func<BigDecimal, int, BigDecimal>)BigDecimal.Round;
				throw new ArithmeticException($"This rounding function does not accept negative precision values. Perhaps you were thinking of {roundingMethod.Method.GetMethodSignature()} ?");
			}

			if (value == BigDecimal.Zero)
			{
				return BigDecimal.Zero;
			}

			int sign = value.Sign;
			BigDecimal absValue = BigDecimal.Abs(value);
			BigDecimal precisionTarget = new BigDecimal(mantissa: BigInteger.One, exponent: -precision);
			BigDecimal truncated = Truncate(absValue, precision + 1);
			BigInteger lastDigit = truncated.Mantissa % 10;

			truncated = Truncate(truncated, precision);

			if (lastDigit == 5)
			{
				if (roundingStrategy == RoundingStrategy.AwayFromZero)
				{
					return (truncated + precisionTarget) * sign;
				}
				else if (roundingStrategy == RoundingStrategy.ToEven)
				{
					if (truncated.Mantissa % 2 == 1)
					{
						return (truncated + precisionTarget) * sign;
					}
				}
			}
			else if (lastDigit > 5)
			{
				return (truncated + precisionTarget) * sign;
			}


			return truncated * sign;
		}

		/// <summary>
		/// Rounds a <see cref="BigDecimal" /> to the given number of digits to the right of the decimal point.
		/// </summary>
		/// <remarks>
		/// Pass a negative precision value to zero out <c>abs(precision)</c> many digits to the left of the decimal point, in a manner that mimics Excel's ROUNDDOWN function.<br />
		/// A zero or greater precision value performs rounding in the standard way.<br />
		/// Call <see cref="Truncate(BigDecimal, int)"/> for a method that is functionally identical to Excel's ROUNDDOWN function.
		/// </remarks>
		public static BigDecimal Round(BigDecimal value, Int32 precision)
		{
			if (precision < 0)
			{
				string integer = value.WholeValue.ToString(BigDecimalNumberFormatInfo);
				int len = integer.Length;
				if (Math.Abs(precision) >= len)
				{
					return BigDecimal.Zero;
				}
				int diff = len + precision;

				string result = integer.Substring(0, diff);
				result += new string(Enumerable.Repeat(BigDecimalNumberFormatInfo.NativeDigits[0][0], Math.Abs(precision)).ToArray());
				return BigDecimal.Parse(result);
			}
			return Round(value, precision, DefaultRoundingStrategy);
		}

		/// <summary>Rounds a <see cref="BigDecimal" /> up to the next largest integer value, even if the fractional part is less than one half. Equivalent to obtaining the floor and then adding one.</summary>
		public static BigDecimal Ceiling(BigDecimal value)
		{
			BigDecimal result = value.WholeValue;
			if ((result != value.Mantissa) && (value >= 0))
			{
				result += 1;
			}
			return result;
		}

		/// <summary>Rounds a <see cref="BigDecimal" /> down to the next smallest integer value, even if the fractional part is greater than one half. Equivalent to discarding everything right of the decimal point.</summary>
		public static BigDecimal Floor(BigDecimal value)
		{
			BigDecimal result = value.WholeValue;
			if (result != value)
			{
				if (value < BigDecimal.Zero)
				{
					result -= BigDecimal.One;
				}
			}
			return result;
		}

		#endregion

		#region Overrides

		/// <summary>Returns the hash code for the current <see cref="BigDecimal"/> object.</summary>
		public override Int32 GetHashCode() => new Tuple<BigInteger, Int32>(this.Mantissa, this.Exponent).GetHashCode();

		/// <summary>Converts the numeric value of the current <see cref="BigDecimal"/> object to its equivalent string representation.</summary>
		public override String ToString() => this.ToString(BigDecimalNumberFormatInfo);

		/// <summary>Converts the numeric value of the current <see cref="BigDecimal"/> object to its equivalent string representation by using the specified culture-specific formatting information.</summary>
		public String ToString(IFormatProvider provider) => ToString(this.Mantissa, this.Exponent, provider);

		private static String ToString(BigInteger mantissa, Int32 exponent, IFormatProvider? provider = null)
		{
			provider ??= CultureInfo.CurrentCulture;

			var formatProvider = NumberFormatInfo.GetInstance(provider);

			var negativeValue = mantissa.Sign == -1;
			var negativeExponent = Math.Sign(exponent) == -1;

			var result = BigInteger.Abs(mantissa).ToString(provider);
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

				result = result.TrimEnd(formatProvider.NativeDigits[0][0]);
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

			int decimalPlaces = 0;
			if (negativeExponent)
			{
				decimalPlaces = Math.Abs(exponent);
			}

			int paddZeros = formatProvider.NumberDecimalDigits - decimalPlaces;
			if (paddZeros < 0)
			{
				paddZeros = 0;
			}
			if (paddZeros > 0)
			{
				if (!result.Contains(formatProvider.NumberDecimalSeparator))
				{
					result += formatProvider.NumberDecimalSeparator;
				}
				result += String.Concat(Enumerable.Repeat(formatProvider.NativeDigits[0], paddZeros));
			}

			if (negativeValue)
			{
				// Prefix "-"
				result = result.Insert(0, formatProvider.NegativeSign);
			}

			return result;
		}

		/// <summary>Converts the numeric value of the specified <see cref="BigDecimal"/> object to its equivalent string representation in scientific E notation.</summary>
		public static String ToScientificENotation(BigDecimal bigDecimal)
		{
			// 1.238E456 1.238E-456
			// -1.238E456
			// -1.238E-456 1E456 1E-456
			// -1E456
			// -1E-456
			// -3E-2

			string mantissa = bigDecimal.Mantissa.ToString(); //Note: will be prefixed with "-" if negative.

			int exponent = bigDecimal.Exponent + (bigDecimal.SignificantDigits - 1);
			int point = 1;
			if (bigDecimal.Mantissa.Sign == -1)
			{
				point += 1;
			}

			//TODO none of this is tested or guaranteed to work yet. Like negatives, or small numbers need the correct logic.
			string sign = BigDecimalNumberFormatInfo.PositiveSign;
			if (Math.Sign(exponent) == -1)
			{
				sign = BigDecimalNumberFormatInfo.NegativeSign;
			}
			var result = $"{mantissa.Insert(point, BigDecimalNumberFormatInfo.NumberDecimalSeparator)}E{sign}{Math.Abs(exponent)}";
			return result;
		}

		#endregion
	}
}
