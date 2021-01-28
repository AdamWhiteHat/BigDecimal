using System;
using System.Linq;
using System.Numerics;
using System.Globalization;

namespace ExtendedNumerics
{
	/// <summary>
	/// Arbitrary precision decimal.
	/// All operations are exact, except for division. Division never determines more digits than the given precision.
	/// Based on code by Jan Christoph Bernack (http://stackoverflow.com/a/4524254 or jc.bernack at googlemail.com)
	//  Modified and extended by Adam White https://csharpcodewhisperer.blogspot.com
	/// </summary>
	public struct BigDecimal : IComparable, IComparable<BigDecimal>
	{

		#region Public static values

		public readonly static BigDecimal Ten;
		public readonly static BigDecimal One;
		public readonly static BigDecimal Zero;
		public readonly static BigDecimal OneHalf;
		public readonly static BigDecimal MinusOne;
		public readonly static BigDecimal E;
		public readonly static BigDecimal Pi;

		public static int Precision = 5000; // Sets the maximum precision of division operations. If AlwaysTruncate is set to true all operations are affected.
		public static bool AlwaysTruncate = false; // Specifies whether the significant digits should be truncated to the given precision after each operation.

		#endregion

		#region Private static values

		private static readonly BigInteger TenInt;
		private static readonly string NumericCharacters;
		private static NumberFormatInfo BigDecimalNumberFormatInfo;

		#endregion

		#region Public properties

		public BigInteger Mantissa { get; private set; }
		public int Exponent { get; private set; }
		public int Sign { get { return GetSign(); } }
		public int SignifigantDigits { get { return GetSignifigantDigits(Mantissa); } }
		public int DecimalPlaces { get { return (SignifigantDigits + Exponent); } }
		public BigInteger WholeValue { get { return GetWholePart(); } }
		public int Length { get { return BigDecimal.GetSignifigantDigits(this.Mantissa) + this.Exponent; } }

		#endregion

		#region Constructors

		public BigDecimal(int value)
			: this(new BigInteger(value), 0)
		{
		}

		public BigDecimal(BigInteger value)
			: this(value, 0)
		{
		}

		public BigDecimal(BigInteger mantissa, int exponent)
		{
			if (mantissa != null)
			{
				this.Mantissa = mantissa.Clone();
				this.Exponent = exponent;

				if (AlwaysTruncate)
				{
					Truncate();
				}
				else
				{
					Normalize();
				}
			}
			else
			{
				this.Mantissa = new BigInteger();
				this.Exponent = 0;
			}
		}

		public BigDecimal(bool alwaysTruncate, int precision)
		{
			AlwaysTruncate = alwaysTruncate;
			Precision = precision;
			Mantissa = new BigInteger(0);
			Exponent = 0;
		}

		static BigDecimal()
		{
			Precision = 5000;
			AlwaysTruncate = false;

			TenInt = new BigInteger(10);
			NumericCharacters = "-0.1234567890";
			BigDecimalNumberFormatInfo = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();


			One = new BigDecimal(1);
			Zero = new BigDecimal(0);
			Ten = new BigDecimal(10, 0);
			OneHalf = 0.5d;
			MinusOne = new BigDecimal(BigInteger.MinusOne, 0);

			E = new BigDecimal(BigInteger.Parse("271828182845904523536028747135266249775724709369995957496696762772407663035354759457138217852516642749193200305992181741359662904357290033429526059563073813232862794349076323382988075319525101901157383"), 1);
			Pi = new BigDecimal(BigInteger.Parse("314159265358979323846264338327950288419716939937510582097494459230781640628620899862803482534211706798214808651328230664709384460955058223172535940812848111745028410270193852110555964462294895493038196"), 1);
		}

		public BigDecimal Clone()
		{
			return new BigDecimal(Mantissa.Clone(), Exponent);
		}

		#endregion

		#region Parsing methods

		/// <summary>
		/// Converts the string representation of a decimal to the BigDecimal equivalent.
		/// </summary>
		public static BigDecimal Parse(double input)
		{
			return Parse(input.ToString());
		}

		/// <summary>
		/// Converts the string representation of a decimal to the BigDecimal equivalent.
		/// </summary>
		public static BigDecimal Parse(decimal input)
		{
			return Parse(input.ToString());
		}

		/// <summary>
		/// Converts the string representation of a decimal to the BigDecimal equivalent.
		/// </summary>
		public static BigDecimal Parse(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				return new BigInteger(0);
			}

			int exponent = 0;
			int decimalPlace = 0;
			bool isNegative = false;
			string localInput = new string(input.Trim().Where(c => NumericCharacters.Contains(c)).ToArray());

			if (localInput.StartsWith(BigDecimalNumberFormatInfo.NegativeSign))
			{
				isNegative = true;
				localInput = localInput.Replace(BigDecimalNumberFormatInfo.NegativeSign, string.Empty);
			}

			if (localInput.Contains(BigDecimalNumberFormatInfo.NumberDecimalSeparator))
			{
				decimalPlace = localInput.IndexOf(BigDecimalNumberFormatInfo.NumberDecimalSeparator);

				exponent = ((decimalPlace + 1) - (localInput.Length));
				localInput = localInput.Replace(BigDecimalNumberFormatInfo.NumberDecimalSeparator, string.Empty);
			}

			BigInteger mantessa = BigInteger.Parse(localInput);
			if (isNegative)
			{
				mantessa = BigInteger.Negate(mantessa);
			}

			return new BigDecimal(mantessa, exponent);
		}

		#endregion

		#region Truncate/Normalize

		/// <summary>
		/// Truncates the BigDecimal to the given precision by removing the least significant digits.
		/// </summary>
		public void Truncate(int precision)
		{
			Normalize();

			int sign = Math.Sign(Exponent);
			int difference = (precision - GetSignifigantDigits(Mantissa)) * -1;
			if (difference >= 1)
			{
				Mantissa = BigInteger.Divide(Mantissa, BigInteger.Pow(TenInt, difference));
				if (sign != 0)
				{
					Exponent += (sign * difference);
				}
			}

			Normalize();
		}

		/// <summary>
		/// Truncates the BigDecimal to the current precision by removing the least significant digits.
		/// </summary>
		public void Truncate()
		{
			Truncate(Precision);
		}

		/// <summary>
		/// Removes any trailing zeros on the mantissa
		/// </summary>
		public void Normalize()
		{
			if (IsZero)
			{
				return;
			}
		}

		/// <summary>
		/// Returns the zero-based index of the decimal point, if the BigDecimal were rendered as a string.
		/// </summary>		
		public int GetDecimalIndex()
		{
			return GetDecimalIndex(this.Mantissa, this.Exponent);
		}

		/// <summary>
		/// Returns the whole number integer part of the BigDecimal, dropping anything right of the decimal point. Essentially behaves like Math.Truncate(). For example, GetWholePart() would return 3 for Math.PI.
		/// </summary>
		public BigInteger GetWholePart()
		{
			if (this == null)
			{
				throw new TypeInitializationException(nameof(BigDecimal), new NullReferenceException());
			}

			if (Mantissa == null) return BigInteger.Zero;

			string resultString = string.Empty;
			string decimalString = BigDecimal.ToString(Mantissa, Exponent, BigDecimalNumberFormatInfo);
			string[] valueSplit = decimalString.Split(new string[] { BigDecimalNumberFormatInfo.NumberDecimalSeparator }, StringSplitOptions.RemoveEmptyEntries);
			if (valueSplit.Length > 0)
			{
				resultString = valueSplit[0];
			}
			return BigInteger.Parse(resultString);
		}

		/// <summary>
		/// Gets the fractional part of the BigDecimal, setting everything left of the decimal point to zero.
		/// </summary>
		public BigDecimal GetFractionalPart()
		{
			if (this == null)
			{
				throw new TypeInitializationException(nameof(BigDecimal), new NullReferenceException());
			}

			if (Mantissa == null) return BigInteger.Zero;

			string resultString = string.Empty;
			string decimalString = this.ToString();
			string[] valueSplit = decimalString.Split('.');
			if (valueSplit.Length == 1)
			{
				return new BigDecimal();
			}
			else if (valueSplit.Length == 2)
			{
				resultString = valueSplit[1];
			}

			BigInteger newMantessa = BigInteger.Parse(resultString.TrimStart(new char[] { '0' }));
			BigDecimal result = new BigDecimal(newMantessa, (0 - resultString.Length));
			return result;
		}

		#endregion

		#region Private methods

		private static bool CheckIsValidObject(BigDecimal instance)
		{
			if (instance == null)
			{
				throw new TypeInitializationException(nameof(BigDecimal), new NullReferenceException());
			}

			if (instance.Mantissa == null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		private int GetSign()
		{
			if (!CheckIsValidObject(this))
			{
				return 0;
			}

			Normalize();

			if (Mantissa.IsZero)
			{
				return 0;
			}
			else if (Mantissa.Sign == -1)
			{
				if (Exponent < 0)
				{
					string mant = Mantissa.ToString();
					int length = mant.Length + Exponent;
					if (length == 0)
					{
						int tenthsPlace = 0;
						int.TryParse(mant[0].ToString(), out tenthsPlace);
						return (tenthsPlace < 5) ? 0 : 1;
					}
					else
					{
						return (length > 0) ? 1 : 0;
					}
				}
				else
				{
					return -1;
				}
			}
			else
			{
				return 1;
			}
		}

		private static int GetDecimalIndex(BigInteger mantissa, int exponent)
		{
			int mantissaLength = mantissa.GetLength();
			if (mantissa.Sign < 0)
			{
				mantissaLength += 1;
			}
			return mantissaLength + exponent;
		}

		private static int GetSignifigantDigits(BigInteger value)
		{
			if (value.IsZero)
			{
				return 0;
			}

			string valueString = value.ToString();
			if (string.IsNullOrWhiteSpace(valueString))
			{
				return 0;
			}

			valueString = new string(valueString.Trim().Where(c => NumericCharacters.Contains(c)).ToArray());
			valueString = valueString.Replace(BigDecimalNumberFormatInfo.NegativeSign, string.Empty);
			valueString = valueString.Replace(BigDecimalNumberFormatInfo.PositiveSign, string.Empty);
			valueString = valueString.TrimEnd(new char[] { '0' });
			valueString = valueString.Replace(BigDecimalNumberFormatInfo.NumberDecimalSeparator, string.Empty);

			return valueString.Length;
		}

		/// <summary>
		/// Returns the mantissa of value, aligned to the exponent of reference. Assumes the exponent of value is larger than of reference.
		/// </summary>
		private static BigInteger AlignExponent(BigDecimal value, BigDecimal reference)
		{
			return value.Mantissa * BigInteger.Pow(TenInt, (value.Exponent - reference.Exponent));
		}

		#endregion

		#region Conversion operators

		#region Implicit

		public static implicit operator BigDecimal(BigInteger value)
		{
			return new BigDecimal(value, 0);
		}

		public static implicit operator BigDecimal(int value)
		{
			return new BigDecimal(new BigInteger(value), 0);
		}

		public static implicit operator BigDecimal(double value)
		{
			if (double.IsInfinity(value))
			{
				throw new OverflowException("BigDecimal cannot represent infinity");
			}

			if (double.IsNaN(value))
			{
				throw new NotFiniteNumberException("Value is not a number");
			}

			BigInteger mantissa = new BigInteger(value);
			int exponent = 0;
			double scaleFactor = 1;
			double abs = 0;
			while ((abs = Math.Abs(value * scaleFactor - double.Parse(mantissa.ToString()))) > 0)
			{
				exponent -= 1;
				scaleFactor *= 10;
				mantissa = new BigInteger(value * scaleFactor);
			}
			return new BigDecimal(mantissa, exponent);
		}

		public static implicit operator BigDecimal(decimal value)
		{
			BigInteger mantissa = new BigInteger(value);
			int exponent = 0;
			decimal scaleFactor = 1;
			while ((decimal.Parse(mantissa.ToString()) != value * scaleFactor))
			{
				exponent -= 1;
				scaleFactor *= 10;
				mantissa = new BigInteger(value * scaleFactor);
			}
			return new BigDecimal(mantissa, exponent);
		}

		#endregion

		#region Explicit

		public static explicit operator BigInteger(BigDecimal v)
		{
			v.Normalize();
			if (v.Exponent < 0)
			{
				string mant = v.Mantissa.ToString();

				int length = v.GetDecimalIndex();
				if (length > 0)
				{
					return BigInteger.Parse(mant.Substring(0, length));
				}
				else if (length == 0)
				{
					int tenthsPlace = int.Parse(mant[0].ToString());
					return (tenthsPlace >= 5) ?
						new BigInteger(1) :
						new BigInteger(0);
				}
				else if (length < 0)
				{
					return new BigInteger(0);
				}
			}
			return BigInteger.Multiply(v.Mantissa, BigInteger.Pow(TenInt, v.Exponent));
		}

		public static explicit operator double(BigDecimal value)
		{
			double mantissa = 0;

			if (!double.TryParse(value.Mantissa.ToString(), out mantissa))
			{
				mantissa = Convert.ToDouble(value.Mantissa.ToString());
			}

			return mantissa * Math.Pow(10, value.Exponent);
		}

		public static explicit operator float(BigDecimal value)
		{
			float mantissa = 0;

			if (!float.TryParse(value.Mantissa.ToString(), out mantissa))
			{
				mantissa = Convert.ToSingle(value.Mantissa.ToString());
			}

			return mantissa * (float)Math.Pow(10, value.Exponent);
		}

		public static explicit operator decimal(BigDecimal value)
		{
			decimal mantissa = 0;

			if (!decimal.TryParse(value.Mantissa.ToString(), out mantissa))
			{
				mantissa = Convert.ToDecimal(value.Mantissa.ToString());
			}

			return mantissa * (decimal)Math.Pow(10, value.Exponent);
		}

		public static explicit operator int(BigDecimal value)
		{
			int mantissa = 0;

			if (!int.TryParse(value.Mantissa.ToString(), out mantissa))
			{
				mantissa = Convert.ToInt32(value.Mantissa.ToString());
			}

			return (mantissa * (int)BigInteger.Pow(TenInt, value.Exponent));
		}

		public static explicit operator uint(BigDecimal value)
		{
			uint mantissa = 0;

			if (!uint.TryParse(value.Mantissa.ToString(), out mantissa))
			{
				mantissa = Convert.ToUInt32(value.Mantissa.ToString());
			}

			return (mantissa * (uint)BigInteger.Pow(TenInt, value.Exponent));
		}

		#endregion

		#endregion

		#region Arithmetic operators

		public static BigDecimal operator +(BigDecimal value)
		{
			return value;
		}

		public static BigDecimal operator -(BigDecimal value)
		{
			return BigDecimal.Negate(value);
		}

		public static BigDecimal operator ++(BigDecimal value)
		{
			return Add(value, 1);
		}

		public static BigDecimal operator --(BigDecimal value)
		{
			return Subtract(value, 1);
		}

		public static BigDecimal operator +(BigDecimal left, BigDecimal right)
		{
			return Add(left, right);
		}

		public static BigDecimal operator -(BigDecimal left, BigDecimal right)
		{
			return Subtract(left, right);
		}

		public static BigDecimal operator *(BigDecimal left, BigDecimal right)
		{
			return Multiply(left, right);
		}

		public static BigDecimal operator /(BigDecimal dividend, BigDecimal divisor)
		{
			return Divide(dividend, divisor);
		}

		#endregion

		#region Comparison and equality operators

		public static bool operator ==(BigDecimal left, BigDecimal right)
		{
			return left.Exponent == right.Exponent && left.Mantissa == right.Mantissa;
		}

		public static bool operator !=(BigDecimal left, BigDecimal right)
		{
			return left.Exponent != right.Exponent || left.Mantissa != right.Mantissa;
		}

		public static bool operator <(BigDecimal left, BigDecimal right)
		{
			return left.Exponent > right.Exponent ? AlignExponent(left, right) < right.Mantissa : left.Mantissa < AlignExponent(right, left);
		}

		public static bool operator >(BigDecimal left, BigDecimal right)
		{
			return left.Exponent > right.Exponent ? AlignExponent(left, right) > right.Mantissa : left.Mantissa > AlignExponent(right, left);
		}

		public static bool operator <=(BigDecimal left, BigDecimal right)
		{
			return left.Exponent > right.Exponent ? AlignExponent(left, right) <= right.Mantissa : left.Mantissa <= AlignExponent(right, left);
		}

		public static bool operator >=(BigDecimal left, BigDecimal right)
		{
			return left.Exponent > right.Exponent ? AlignExponent(left, right) >= right.Mantissa : left.Mantissa >= AlignExponent(right, left);
		}

		#endregion

		#region General BigDecimal arithmetic methods

		/// <summary>
		/// Returns the result of multiplying a BigDecimal by negative one.
		/// </summary>
		public static BigDecimal Negate(BigDecimal value)
		{
			value.Mantissa = BigInteger.Negate(value.Mantissa);
			return value.Clone();
		}

		/// <summary>
		/// Adds two BigDecimal values.
		/// </summary>
		public static BigDecimal Add(BigDecimal left, BigDecimal right)
		{
			return left.Exponent > right.Exponent
				? new BigDecimal(AlignExponent(left, right) + right.Mantissa, right.Exponent)
				: new BigDecimal(AlignExponent(right, left) + left.Mantissa, left.Exponent);
		}

		/// <summary>
		/// Subtracts two BigDecimal values.
		/// </summary>
		public static BigDecimal Subtract(BigDecimal left, BigDecimal right)
		{
			return Add(left, BigDecimal.Negate(right));
		}

		/// <summary>
		/// Multiplies two BigDecimal values.
		/// </summary>
		public static BigDecimal Multiply(BigDecimal left, BigDecimal right)
		{
			return new BigDecimal(left.Mantissa * right.Mantissa, left.Exponent + right.Exponent);
		}

		/// <summary>
		/// Divides two BigDecimal values, returning the remainder and discarding the quotient.
		/// </summary>
		public static BigDecimal Mod(BigDecimal value, BigDecimal mod)
		{
			// x – q * y            
			BigDecimal q = BigDecimal.Floor(BigDecimal.Divide(value, mod));
			return BigDecimal.Multiply(BigDecimal.Subtract(value, q), mod);
		}

		/// <summary>
		/// Divides two BigDecimal values.
		/// </summary>
		/// 
		public static BigDecimal Divide(BigDecimal dividend, BigDecimal divisor)
		{
			if (divisor == BigDecimal.Zero) throw new DivideByZeroException();

			dividend.Normalize();
			divisor.Normalize();

			//	if (dividend > divisor) { return Divide_Positive(dividend, divisor); }

			if (BigDecimal.Abs(dividend) == 1)
			{
				double doubleDivisor = double.Parse(divisor.ToString());
				doubleDivisor = (1d / doubleDivisor);

				return BigDecimal.Parse(doubleDivisor.ToString());
			}

			string remString = "";
			string mantissaString = "";
			string dividendMantissaString = dividend.Mantissa.ToString();
			string divisorMantissaString = divisor.Mantissa.ToString();

			int dividendMantissaLength = dividend.DecimalPlaces;
			int divisorMantissaLength = divisor.DecimalPlaces;
			var exponentChange = (dividendMantissaLength - divisorMantissaLength);

			BigDecimal result = 0;
			BigInteger remainder = 0;
			result.Mantissa = BigInteger.DivRem(dividend.Mantissa, divisor.Mantissa, out remainder);
			while (remainder != 0 && result.SignifigantDigits < divisor.SignifigantDigits)
			{
				while (BigInteger.Abs(remainder) < BigInteger.Abs(divisor.Mantissa))
				{
					remainder *= 10;
					result.Mantissa *= 10;

					remString = remainder.ToString();
					mantissaString = result.Mantissa.ToString();
				}
				result.Mantissa = result.Mantissa + BigInteger.DivRem(remainder, divisor.Mantissa, out remainder);

				remString = remainder.ToString();
				mantissaString = result.Mantissa.ToString();
			}

			result.Exponent = exponentChange;
			return result;
		}

		/// <summary>
		/// Returns e raised to the specified power
		/// </summary>
		public static BigDecimal Exp(double exponent)
		{
			BigDecimal tmp = (BigDecimal)1;
			while (Math.Abs(exponent) > 100)
			{
				int diff = exponent > 0 ? 100 : -100;
				tmp *= Math.Exp(diff);
				exponent -= diff;
			}
			return tmp * Math.Exp(exponent);
		}

		/// <summary>
		/// Returns e raised to the specified power
		/// </summary>
		public static BigDecimal Exp(BigInteger exponent)
		{
			BigDecimal tmp = (BigDecimal)1;
			while (BigInteger.Abs(exponent) > 100)
			{
				int diff = exponent > 0 ? 100 : -100;
				tmp *= Math.Exp(diff);
				exponent -= diff;
			}
			double exp = (double)exponent;
			return (tmp * Math.Exp(exp));
		}

		/// <summary>
		/// Returns a specified number raised to the specified power.
		/// </summary>
		public static BigDecimal Pow(BigDecimal baseValue, BigInteger exponent)
		{
			if (exponent.IsZero)
			{
				return BigDecimal.One;
			}
			else if (exponent.Sign < 0)
			{
				if (baseValue == BigDecimal.Zero)
				{
					throw new NotSupportedException("Cannot raise zero to a negative power");
				}

				// n^(-e) -> (1/n)^e
				baseValue = BigDecimal.One / baseValue;
				exponent = BigInteger.Negate(exponent);
			}

			BigDecimal result = baseValue;
			while (exponent > BigInteger.One)
			{
				result = result * baseValue;
				exponent--;
			}

			return result;
		}

		/// <summary>
		/// Returns a specified number raised to the specified power.
		/// </summary>
		public static BigDecimal Pow(double basis, double exponent)
		{
			BigDecimal tmp = (BigDecimal)1;
			while (Math.Abs(exponent) > 100)
			{
				int diff = exponent > 0 ? 100 : -100;
				tmp *= Math.Pow(basis, diff);
				exponent -= diff;
			}
			return (tmp * Math.Pow(basis, exponent));
		}

		#endregion

		#region Additional mathematical functions

		/// <summary>
		/// Returns the absolute value of the BigDecimal
		/// </summary>
		public static BigDecimal Abs(BigDecimal value)
		{
			if (value.IsNegative)
			{
				return value * -1;
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// Rounds a BigDecimal value to the nearest integral value.
		/// </summary>
		public static BigInteger Round(BigDecimal value)
		{
			return Round(value, MidpointRounding.AwayFromZero);
		}

		/// <summary>
		/// Rounds a BigDecimal value to the nearest integral value. A parameter specifies how to round the value if it is midway between two numbers.
		/// </summary>
		public static BigInteger Round(BigDecimal value, MidpointRounding mode)
		{
			value.Normalize();

			BigInteger wholePart = value.WholeValue;
			BigDecimal decimalPart = value.GetFractionalPart();

			BigInteger addOne = value.IsNegative ? -1 : 1;

			if (decimalPart > BigDecimal.OneHalf)
			{
				wholePart += addOne;
			}
			else if (decimalPart == BigDecimal.OneHalf)
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
		/// Rounds a BigDecimal to an integer value. The BigDecimal argument is rounded towards positive infinity.
		/// </summary>
		public static BigDecimal Ceiling(BigDecimal value)
		{
			value.Normalize();
			return new BigDecimal(value.Mantissa);
		}

		/// <summary>
		/// 
		/// </summary>
		public static BigDecimal Floor(BigDecimal value)
		{
			value.Normalize();
			return new BigDecimal(value.Mantissa + 1);
		}

		/// <summary>
		/// This method returns true if the BigDecimal is equal to zero, false otherwise.
		/// </summary>
		public bool IsZero
		{
			get
			{
				return (Mantissa.IsZero);
			}
		}

		/// <summary>
		/// This method returns true if the BigDecimal is greater than zero, false otherwise.
		/// </summary>
		public bool IsPositve
		{
			get
			{
				return (!IsZero && !IsNegative);
			}
		}

		/// <summary>
		/// This method returns true if the BigDecimal is less than zero, false otherwise.
		/// </summary>
		public bool IsNegative
		{
			get
			{
				return (Mantissa.Sign < 0);
			}
		}

		#endregion

		#region Overrides

		/// <summary>
		///  Compares two BigDecimal values, returning an integer that indicates their relationship.
		/// </summary>
		public int CompareTo(object obj)
		{
			if (ReferenceEquals(obj, null) || !(obj is BigDecimal))
			{
				throw new ArgumentException();
			}
			return CompareTo((BigDecimal)obj);
		}

		/// <summary>
		///  Compares two BigDecimal values, returning an integer that indicates their relationship.
		/// </summary>
		public int CompareTo(BigDecimal other)
		{
			return this < other ? -1 : (this > other ? 1 : 0);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			return obj is BigDecimal && Equals((BigDecimal)obj);
		}

		public bool Equals(BigDecimal other)
		{
			this.Normalize();
			other.Normalize();

			bool matchMantissa = this.Mantissa.Equals(other.Mantissa);
			bool matchExponent = this.Exponent.Equals(other.Exponent);
			bool matchSign = this.Sign.Equals(other.Sign);

			if (matchMantissa && matchExponent && matchSign)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Mantissa.GetHashCode() * 397) ^ Exponent.GetHashCode();
			}
		}

		public override string ToString()
		{
			return this.ToString(BigDecimalNumberFormatInfo);
		}

		private static readonly string NullString = "(null)";

		public String ToString(IFormatProvider provider)
		{
			return BigDecimal.ToString(Mantissa, Exponent, provider);
		}

		private static String ToString(BigInteger mantissa, int exponent, IFormatProvider provider)
		{
			if (provider == null) throw new ArgumentNullException();
			if (mantissa == null || BigDecimalNumberFormatInfo == null) { return NullString; }

			NumberFormatInfo formatProvider = NumberFormatInfo.GetInstance(provider);

			bool negativeValue = (mantissa.Sign == -1);
			bool negativeExponent = (Math.Sign(exponent) == -1);

			string result = BigInteger.Abs(mantissa).ToString();
			int absExp = Math.Abs(exponent);

			if (negativeExponent)
			{
				if (absExp > result.Length)
				{
					int zerosToAdd = Math.Abs(absExp - result.Length);
					string zeroString = string.Join(string.Empty, Enumerable.Repeat(formatProvider.NativeDigits[0], zerosToAdd));
					result = zeroString + result;
					result = result.Insert(0, formatProvider.NumberDecimalSeparator);
					result = result.Insert(0, formatProvider.NativeDigits[0]);
				}
				else
				{
					int indexOfRadixPoint = Math.Abs(absExp - result.Length);
					result = result.Insert(indexOfRadixPoint, formatProvider.NumberDecimalSeparator);
					if (indexOfRadixPoint == 0)
					{
						result = result.Insert(0, formatProvider.NativeDigits[0]);
					}
				}

				result = result.TrimEnd(new char[] { '0' });
				if (result.Last().ToString() == formatProvider.NumberDecimalSeparator)
				{
					result = result.Substring(0, result.Length - 1);
				}
			}
			else
			{
				string zeroString = string.Join(string.Empty, Enumerable.Repeat(formatProvider.NativeDigits[0], absExp));
				result += zeroString;
			}

			if (negativeExponent) // Prefix "0."
			{

			}
			if (negativeValue) // Prefix "-"
			{
				result = result.Insert(0, formatProvider.NegativeSign);
			}

			return result;


		}

		#endregion

	}
}
