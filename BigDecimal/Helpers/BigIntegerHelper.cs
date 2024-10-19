using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ExtendedNumerics.Properties;

namespace ExtendedNumerics.Helpers
{
	public static class BigIntegerHelper
	{
		public static BigInteger GCD(this IEnumerable<BigInteger> numbers) => numbers.Aggregate(GCD);

		public static BigInteger GCD(BigInteger value1, BigInteger value2)
		{
			var absValue1 = BigInteger.Abs(value1);
			var absValue2 = BigInteger.Abs(value2);

			while ((absValue1 != BigInteger.Zero) && (absValue2 != BigInteger.Zero))
			{
				if (absValue1 > absValue2)
				{
					absValue1 %= absValue2;
				}
				else
				{
					absValue2 %= absValue1;
				}
			}
			return BigInteger.Max(absValue1, absValue2);
		}

		public static Int32 GetLength(this BigInteger source)
		{
			var result = 0;
			var copy = BigInteger.Abs(source);
			while (copy > BigInteger.Zero)
			{
				copy /= Ten;
				result++;
			}
			return result;
		}

		public static IEnumerable<BigInteger> GetRange(BigInteger min, BigInteger max)
		{
			while (min < max)
			{
				yield return min;
				min++;
			}
		}

		public static Int32 GetSignificantDigits(this BigInteger value)
		{
			if (value.IsZero)
			{
				return 0;
			}

			var valueString = value.ToString().TrimEnd('0'); //CONFIRM Is this correct?

			if (String.IsNullOrEmpty(valueString))
			{
				return 0;
			}
			if (value < BigInteger.Zero)
			{
				return valueString.Length - 1;
			}
			return valueString.Length;
		}

		public static Boolean IsCoprime(BigInteger value1, BigInteger value2) => GCD(value1, value2) == BigInteger.One;

		public static BigInteger LCM(IEnumerable<BigInteger> numbers) => numbers.Aggregate(LCM);

		public static BigInteger LCM(BigInteger num1, BigInteger num2)
		{
			var absValue1 = BigInteger.Abs(num1);
			var absValue2 = BigInteger.Abs(num2);
			return (absValue1 * absValue2) / GCD(absValue1, absValue2);
		}

		/// <summary>
		///<para>Returns the NTHs root of a <see cref="BigInteger"/> with <paramref name="remainder"/>.</para>
		/// <para>The root must be greater than or equal to 1 or value must be a positive integer.</para>
		/// </summary>
		/// <param name="value"></param>
		/// <param name="root"></param>
		/// <param name="remainder"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public static BigInteger NthRoot(this BigInteger value, Int32 root, out BigInteger remainder)
		{
			if (root < 1)
			{
				throw new ArgumentException(LanguageResources.Arg_MustBeGreaterThanOrEqualToOne, nameof(root));
			}

			if (value.Sign == -1)
			{
				throw new ArgumentException(LanguageResources.Arg_MustBeAPositiveInteger, nameof(value));
			}

			if (value == BigInteger.One)
			{
				remainder = BigInteger.Zero;
				return BigInteger.One;
			}

			if (value == BigInteger.Zero)
			{
				remainder = BigInteger.Zero;
				return BigInteger.Zero;
			}

			if (root == 1)
			{
				remainder = BigInteger.Zero;
				return value;
			}

			var upperbound = value;
			var lowerbound = BigInteger.Zero;

			do
			{
				var nval = (upperbound + lowerbound) >> 1;

				var tstsq = BigInteger.Pow(nval, root);

				if (tstsq > value)
				{
					upperbound = nval;
				}

				if (tstsq < value)
				{
					lowerbound = nval;
				}

				if (tstsq == value)
				{
					lowerbound = nval;
					break;
				}
			}
			while (lowerbound != (upperbound - BigInteger.One));

			remainder = value - BigInteger.Pow(lowerbound, root);
			return lowerbound;
		}

		public static BigInteger Square(this BigInteger input) => input * input;

		public static BigInteger SquareRoot(this BigInteger input)
		{
			if (input.IsZero)
			{
				return BigInteger.Zero;
			}

			var n = BigInteger.Zero;
			var p = BigInteger.Zero;
			var low = BigInteger.Zero;
			var high = BigInteger.Abs(input);

			while (high > (low + BigInteger.One))
			{
				n = (high + low) >> 1;
				p = n * n;
				if (input < p)
				{
					high = n;
				}
				else if (input > p)
				{
					low = n;
				}
				else
				{
					break;
				}
			}

			return input == p ? n : low;
		}

		/// <summary>
		/// Attempt to parse a fraction from a String.
		/// </summary>
		/// <example>" 1234.45 / 346.456 "</example>
		/// <param name="numberString"></param>
		/// <param name="result"></param>
		/// <exception cref="OverflowException">Uncomment this if you want an exception instead of a Boolean.</exception>
		public static Boolean TryParseFraction(this String numberString, out BigDecimal? result)
		{
			result = default(BigDecimal?);

			if (String.IsNullOrWhiteSpace(numberString))
			{
				return false;
			}

#if NET5_0_OR_GREATER || NETCOREAPP || NETSTANDARD2_1_OR_GREATER
		var parts = numberString.Split('/', StringSplitOptions.RemoveEmptyEntries).Select(static s => s.Trim() ).ToList();
#else
			List<string> parts = numberString.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Select(static s => s.Trim()).ToList();
#endif

			if (parts.Count != 2)
			{
				return false;
			}

			try
			{
				var numerator = BigDecimal.Parse(parts[0]);
				var denominator = BigDecimal.Parse(parts[1]);

				result = BigDecimal.Divide(numerator, denominator);
				return true;
			}
			catch (Exception)
			{
				//throw new OverflowException(LanguageResources.Overflow_Fraction);
				return false;
			}
		}


		/// <summary>
		/// <para>
		/// Calculates a factorial by the divide and conquer method.
		/// This is faster than repeatedly multiplying the next value by a running product
		/// by not repeatedly multiplying by large values.
		/// Essentially, this multiplies every number in the array with its neighbor,
		/// returning an array half as long of products of two numbers.
		/// We then take that array and multiply each pair of values in the array
		/// with its neighbor, resulting in another array half the length of the previous one, and so on...
		/// This results in many multiplications of small, equally sized operands 
		/// and only a few multiplications of larger operands.
		/// In the limit, this is more efficient.
		/// </para>
		/// <para>
		/// The factorial function is used during the calculation of trigonometric functions to arbitrary precision.
		/// </para>
		/// </summary>
		public static class FastFactorial
		{
			public static BigInteger Factorial(BigInteger value)
			{
				if ((value == BigInteger.Zero) || (value == BigInteger.One)) { return BigInteger.One; }
				return MultiplyRange(Two, value);
			}

			/// <summary>Divide the range of numbers to multiply in half recursively.</summary>
			/// <param name="from"></param>
			/// <param name="to"></param>
			private static BigInteger MultiplyRange(BigInteger from, BigInteger to)
			{
				var diff = to - from;
				if (diff == BigInteger.One) { return from * to; }
				if (diff == BigInteger.Zero) { return from; }

				var half = (from + to) / Two;
				return BigInteger.Multiply(MultiplyRange(from, half), MultiplyRange(half + BigInteger.One, to));
			}
		}

		public static readonly BigInteger Two = 2;
		public static readonly BigInteger Ten = 10;
	}
}