using System;
using System.Linq;
using System.Numerics;
using System.Globalization;

namespace AJRLibray.Mathematics
{
	/// <summary>
	/// Arbitrary precision decimal.
	/// All operations are exact, except for division. Division never determines more digits than the given precision.
	/// Based on code by Jan Christoph Bernack (http://stackoverflow.com/a/4524254 or jc.bernack at googlemail.com)
	//  Modified and extended by Adam Rakaska http://www.csharpprogramming.tips
	/// </summary>
	public struct BigDecimal
		: IComparable
		, IComparable<BigDecimal>
	{
		public static BigDecimal Ten;
		public static BigDecimal One;
		public static BigDecimal Zero;
		public static BigDecimal OneHalf;
		public static BigDecimal MinusOne;
		//public static BigDecimal E = new BigDecimal(BigInteger.Parse("271828182845904523536028747135266249775724709369995957496696762772407663035354759457138217852516642749193200305992181741359662904357290033429526059563073813232862794349076323382988075319525101901157383"), 1);
		//public static BigDecimal Pi = new BigDecimal(BigInteger.Parse("314159265358979323846264338327950288419716939937510582097494459230781640628620899862803482534211706798214808651328230664709384460955058223172535940812848111745028410270193852110555964462294895493038196"), 1);

		static BigDecimal()
		{
			Ten = new BigDecimal(10, 0);
			TenInt = new BigInteger(10);
			One = new BigDecimal(1);
			Zero = new BigDecimal(0);
			OneHalf = 0.5d;
			MinusOne = new BigDecimal(BigInteger.MinusOne, 0);
		}

		public BigInteger Mantissa { get; set; } // 
		public int Exponent { get; set; } // 
		public int Sign { get { return GetSign(this); } } // 
		public int SignifigantDigits { get { return GetSignifigantDigits(Mantissa); } } // 
		public int DecimalPlaces { get { return (SignifigantDigits + Exponent); } } // 
		public BigInteger WholeValue { get { return GetWholeDigits(this); } } // 
		public int Length { get { return BigDecimal.GetSignifigantDigits(this.Mantissa) + this.Exponent; } } // 

		public static int Precision = 5000; // Sets the maximum precision of division operations. If AlwaysTruncate is set to true all operations are affected.
		public static bool AlwaysTruncate = false; // Specifies whether the significant digits should be truncated to the given precision after each operation.

		private static BigInteger TenInt;
		private static string NumericCharacters = "-0.1234567890";

		private static NumberFormatInfo BigDecimalNumberFormatInfo = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();

		public BigDecimal(BigInteger value)
			: this(value, 0)
		{
		}

		public BigDecimal(BigDecimal value)
			: this(value.Mantissa, value.Exponent)
		{
		}

		public BigDecimal(BigInteger mantissa, int exponent)
		{
			this.Mantissa = new BigInteger();
			this.Mantissa = mantissa;
			Exponent = exponent;

			if (AlwaysTruncate)
			{
				Truncate();
			}
			else
			{
				Normalize();
			}
		}

		public BigDecimal(bool alwaysTruncate, int precision)
		{
			AlwaysTruncate = alwaysTruncate;
			Precision = precision;
			Mantissa = new BigInteger();
			Exponent = 0;
		}

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

				exponent = ((decimalPlace + 1) - localInput.Length);
				localInput = localInput.Replace(BigDecimalNumberFormatInfo.NumberDecimalSeparator, string.Empty);
			}

			BigInteger mantessa = BigInteger.Parse(localInput);
			if (isNegative)
			{
				mantessa = BigInteger.Negate(mantessa);
			}

			return new BigDecimal(mantessa, exponent);
		}

		#region Truncate/Normalize

		/// <summary>
		/// Removes trailing zeros on the mantissa
		/// </summary>
		public void Normalize()
		{
			if (IsZero)
			{
				return;
			}

			int len = BigIntegerHelper.GetLength(Mantissa);

			if (len > 100)
			{
				string tmp = Mantissa.ToString();
				string trunc = tmp.TrimEnd('0');

				int remain = trunc.Length - tmp.Length;

				Exponent = remain;
				Mantissa = BigInteger.Parse(trunc);

				if (remain > 0)
				{
					throw new Exception("Shouldn't have remainder");
				}
			}

			BigInteger remainder = new BigInteger();
			do
			{
				BigInteger shortened = BigInteger.DivRem(Mantissa, 10, out remainder);
				if (remainder == 0)
				{
					Mantissa = shortened;
					Exponent++;
				}
			}
			while (remainder == 0);
		}

		/// <summary>
		/// Truncate the number to the given precision by removing the least significant digits.
		/// </summary>
		/// <returns>The truncated number</returns>
		public void Truncate(int precision)
		{
			Normalize();

			int sign = System.Math.Sign(Exponent);
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

		public void Truncate()
		{
			Truncate(Precision);
		}

		private static int GetSign(BigDecimal value)
		{
			value.Normalize();

			if (value.Mantissa.IsZero)
			{
				return 0;
			}
			else if (value.Mantissa.Sign == -1)
			{
				if (value.Exponent < 0)
				{
					string mant = value.Mantissa.ToString();
					int length = mant.Length + value.Exponent;
					if (length == 0)
					{
						int tenthsPlace = 0;
						int.TryParse(mant[0].ToString(), out tenthsPlace);
						return (tenthsPlace < 5) ? 0 : 1;
					}

					return (length > 0) ? 1 : 0;
				}
				else
				{
					return -1;
				}
			}
			return 1;
		}

		private int GetDecimalIndex()
		{
			int mantissaLength = BigIntegerHelper.GetLength(this.Mantissa);
			if (this.IsNegative)
			{
				mantissaLength += 1;
			}
			return mantissaLength + this.Exponent;
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
			valueString = valueString.Replace(BigDecimalNumberFormatInfo.NumberDecimalSeparator, string.Empty);

			return valueString.Length;
		}

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

		public static BigInteger GetWholeDigits(BigDecimal value)
		{
			string resultString = string.Empty;
			string decimalString = value.ToString();
			string[] valueSplit = decimalString.Split(new string[] { BigDecimalNumberFormatInfo.NumberDecimalSeparator }, StringSplitOptions.RemoveEmptyEntries);
			if (valueSplit.Length > 0)
			{
				resultString = valueSplit[0];
			}
			return BigInteger.Parse(resultString);
		}

		public BigDecimal GetFractionalPart()
		{
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

		/// <summary>
		/// Returns the mantissa of value, aligned to the exponent of reference.
		/// Assumes the exponent of value is larger than of reference.
		/// </summary>
		private static BigInteger AlignExponent(BigDecimal value, BigDecimal reference)
		{
			return value.Mantissa * BigInteger.Pow(TenInt, (value.Exponent - reference.Exponent));
		}

		#endregion

		#region Conversions

		#region Implicit

		public static implicit operator BigDecimal(BigInteger value)
		{
			return new BigDecimal(value);
		}

		public static implicit operator BigDecimal(int value)
		{
			return new BigDecimal(value, 0);
		}

		public static implicit operator BigDecimal(double value)
		{
			if (double.IsInfinity(value))
			{
				throw new OverflowException("BigDecimal cannot represent infinity");
			}

			BigInteger mantissa = (BigInteger)value;
			int exponent = 0;
			double scaleFactor = 1;
			double abs = 0;
			while ((abs = Math.Abs(value * scaleFactor - (double)mantissa)) > 0)
			{
				exponent -= 1;
				scaleFactor *= 10;
				mantissa = (BigInteger)(value * scaleFactor);
			}
			return new BigDecimal(mantissa, exponent);
		}

		public static implicit operator BigDecimal(decimal value)
		{
			BigInteger mantissa = (BigInteger)value;
			int exponent = 0;
			decimal scaleFactor = 1;
			while ((decimal)mantissa != value * scaleFactor)
			{
				exponent -= 1;
				scaleFactor *= 10;
				mantissa = (BigInteger)(value * scaleFactor);
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
			return (double)value.Mantissa * System.Math.Pow(10, value.Exponent);
		}

		public static explicit operator float(BigDecimal value)
		{
			return Convert.ToSingle((double)value);
		}

		public static explicit operator decimal(BigDecimal value)
		{
			return (decimal)value.Mantissa * (decimal)System.Math.Pow(10, value.Exponent);
		}

		public static explicit operator int(BigDecimal value)
		{
			return (int)(value.Mantissa * BigInteger.Pow(TenInt, value.Exponent));
		}

		public static explicit operator uint(BigDecimal value)
		{
			return (uint)(value.Mantissa * BigInteger.Pow(TenInt, value.Exponent));
		}

		#endregion

		#endregion

		#region Operators

		public static BigDecimal operator +(BigDecimal value)
		{
			return value;
		}

		public static BigDecimal operator -(BigDecimal value)
		{
			value.Mantissa *= -1;
			return value;
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

		#region Comparison

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

		#region Static Math Operations

		public static BigDecimal Add(BigDecimal left, BigDecimal right)
		{
			return left.Exponent > right.Exponent
				? new BigDecimal(AlignExponent(left, right) + right.Mantissa, right.Exponent)
				: new BigDecimal(AlignExponent(right, left) + left.Mantissa, left.Exponent);
		}

		public static BigDecimal Subtract(BigDecimal left, BigDecimal right)
		{
			return Add(left, -(right));
		}

		public static BigDecimal Multiply(BigDecimal left, BigDecimal right)
		{
			return new BigDecimal(left.Mantissa * right.Mantissa, left.Exponent + right.Exponent);
		}

		public static BigDecimal Divide(BigDecimal dividend, BigDecimal divisor)
		{
			if (divisor.Mantissa == 0) { throw new DivideByZeroException(); }

			int exponentChange = Precision - (GetSignifigantDigits(dividend.Mantissa) - GetSignifigantDigits(divisor.Mantissa));
			if (exponentChange < 0)
			{
				exponentChange = 0;
			}
			dividend.Mantissa *= BigInteger.Pow(TenInt, exponentChange);
			BigInteger quotient = BigInteger.Divide(dividend.Mantissa, divisor.Mantissa);
			var difference = (dividend.Exponent - divisor.Exponent - exponentChange);
			return new BigDecimal(quotient, difference);
		}

		#endregion

		#region Additional mathematical functions

		public bool IsZero
		{
			get
			{
				return (Mantissa.IsZero);
			}
		}

		public bool IsPositve
		{
			get
			{
				return (!IsZero && !IsNegative);
			}
		}

		public bool IsNegative
		{
			get
			{
				return (Mantissa.Sign < 0);
			}
		}

		public static BigInteger Round(BigDecimal value)
		{
			return Round(value, MidpointRounding.AwayFromZero);
		}

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

		public static BigDecimal Ceiling(BigDecimal value)
		{
			value.Normalize();
			return new BigDecimal(value.Mantissa);
		}

		public static BigDecimal Floor(BigDecimal value)
		{
			value.Normalize();
			return new BigDecimal(value.Mantissa + 1);
		}

		public static BigDecimal Exp(double exponent)
		{
			BigDecimal tmp = (BigDecimal)1;
			while (System.Math.Abs(exponent) > 100)
			{
				int diff = exponent > 0 ? 100 : -100;
				tmp *= System.Math.Exp(diff);
				exponent -= diff;
			}
			return tmp * System.Math.Exp(exponent);
		}

		public static BigDecimal Exp(BigInteger exponent)
		{
			BigDecimal tmp = (BigDecimal)1;
			while (BigInteger.Abs(exponent) > 100)
			{
				int diff = exponent > 0 ? 100 : -100;
				tmp *= System.Math.Exp(diff);
				exponent -= diff;
			}
			double exp = (double)exponent;
			return (tmp * System.Math.Exp(exp));
		}

		public static BigDecimal Pow(BigInteger value, BigInteger exponent)
		{
			return Pow(new BigDecimal(value), new BigDecimal(exponent));
		}

		public static BigDecimal Pow(BigDecimal value, BigDecimal exponent)
		{
			if (exponent.IsZero)
			{
				return new BigDecimal(1);
			}
			else if (exponent.IsNegative)
			{
				return new BigDecimal(value);
			}

			int diff = 100;
			int sign = GetSign(exponent);
			BigDecimal exp = new BigDecimal(exponent);
			BigDecimal total = new BigDecimal(value);

			int log10 = BigIntegerHelper.GetLength((BigInteger)exp);
			if (log10 >= 5)
			{
				diff = 10000;
			}

			while (Abs(exp) > diff)
			{
				if (exp < 0)
				{   // Divide
					total *= BigDecimal.Divide(BigDecimal.One, BigInteger.Pow(TenInt, diff));
					exp += diff;
				}
				else
				{
					total *= BigInteger.Pow(TenInt, diff);
					exp -= diff;
				}
			}

			if (exp > 0)
			{
				total *= BigInteger.Pow(TenInt, (int)exp);
			}

			return total;
		}

		public static BigDecimal Pow(double basis, double exponent)
		{
			BigDecimal tmp = (BigDecimal)1;
			while (System.Math.Abs(exponent) > 100)
			{
				int diff = exponent > 0 ? 100 : -100;
				tmp *= System.Math.Pow(basis, diff);
				exponent -= diff;
			}
			return (tmp * System.Math.Pow(basis, exponent));
		}

		#endregion

		#region Overrides

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			return obj is BigDecimal && Equals((BigDecimal)obj);
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
			bool isNegative = false;
			string result = Mantissa.ToString();
			if (Mantissa.Sign < 1)
			{
				isNegative = true;
			}

			if (Exponent == 0)
			{
				return result;
			}

			int decimalPosition = GetDecimalIndex();

			if (decimalPosition == 0)
			{
				result = "0" + BigDecimalNumberFormatInfo.NumberDecimalSeparator + result;
			}
			else if (decimalPosition > 0)
			{
				if (decimalPosition > result.Length)
				{
					return result.PadRight(decimalPosition, '0');
				}

				if (decimalPosition < result.Length)
				{
					return result.Insert(decimalPosition, BigDecimalNumberFormatInfo.NumberDecimalSeparator);
				}
			}
			else if (decimalPosition < 0)
			{
				decimalPosition = Math.Abs(decimalPosition);

				result = "0" + BigDecimalNumberFormatInfo.NumberDecimalSeparator + result.PadLeft(Math.Max(result.Length, decimalPosition), '0');
			}

			if (isNegative)
			{
				result = result.Insert(0, BigDecimalNumberFormatInfo.NegativeSign);
			}

			return result;
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

		public int CompareTo(object obj)
		{
			if (ReferenceEquals(obj, null) || !(obj is BigDecimal))
			{
				throw new ArgumentException();
			}
			return CompareTo((BigDecimal)obj);
		}

		public int CompareTo(BigDecimal other)
		{
			return this < other ? -1 : (this > other ? 1 : 0);
		}

		#endregion

	}
}
