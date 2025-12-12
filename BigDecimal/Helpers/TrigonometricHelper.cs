// BigDecimal is copyright by Adam White.
// MIT License.
// https://github.com/AdamWhiteHat/BigDecimal

using System;
using System.Numerics;

namespace ExtendedNumerics.Helpers
{
	public static class TrigonometricHelper
	{
		/// <summary>
		/// Common function to generate the target value to compare against to see if
		/// an operation has reached sufficient precision.
		/// The point of this method instead of having it in-line is that we have only
		/// one place to change if we need to increase the value we are adding to
		/// precision to get adjustedPrecision.
		/// </summary>
		internal static BigDecimal GetPrecisionTarget(int precision)
		{
			var adjustedPrecision = precision;
			return new BigDecimal(BigInteger.One, -adjustedPrecision);
		}

		/// <summary>
		/// Return 1 if radians is an odd multiple of π/2, 0 otherwise.
		/// </summary>
		internal static int ModOddHalfPi(BigDecimal radians)
		{
			var modPiOverTwo = BigDecimal.Normalize(BigDecimal.Mod(radians, HalfPi));

			if (modPiOverTwo.IsZero()) // Is divisible by π/2
			{
				var divideByPiOverTwo = BigDecimal.Normalize(BigDecimal.Divide(radians, HalfPi)); // Well how many times is it divisible by π/2
				var mod2 = BigDecimal.Normalize(BigDecimal.Mod(divideByPiOverTwo, new BigDecimal(2))); // Is that number of times odd? Remainder of 1 == yes; 0 == no
				return (int)mod2;
			}
			return 0;
		}

		/// <summary>
		/// Calculates a Taylor Series Sum until the specified precision is met.
		/// Based on its parameters, this can approximate several different functions
		/// including the sin, cos, sinh, cosh, and exp trigonometric functions.
		/// </summary>
		/// <param name="radians">
		/// The indeterminate value in the Taylor Series that gets multiplied by each term, raised to some
		/// power.
		/// </param>
		/// <param name="sumStart">The value to initialize the running total to. Typically, this is either zero or one.</param>
		/// <param name="counterStart">The term number to start the series at. Typically, this is either zero or one.</param>
		/// <param name="jump">
		/// How much to increment the term index each iteration.
		/// If you want to sum only the even terms, set the counterStart to an even number and this parameter to two.
		/// </param>
		/// <param name="multiplier">
		/// Each term is multiplied by a variable called sign. By default, sign is equal to 1.
		/// Each iteration, sign is set to sign multiplied by this value.
		/// The point of this is to allow every other term to be negative (so subtracted from the sum) by setting this to
		/// parameter to -1.
		/// Setting this to parameter to -1 will flip the sign of the sign variable every iteration.
		/// Since this gets multiplied by the term, the effect is to flip the sign of every other term.
		/// Set this parameter to 1 if all the terms should remain positive.
		/// </param>
		/// <param name="factorialDenominator">
		/// A boolean indicating if the denominator of the term should be passed to the factorial function.
		/// Typically, this is true, but sometimes the factorial in the denominator cancels out,
		/// and so we need a way to turn this off.
		/// </param>
		/// <param name="precision">
		/// The required precision to achieve before returning, in terms of the number of correct digits to the right of the
		/// decimal point.
		/// </param>
		internal static BigDecimal TaylorSeriesSum(
				BigDecimal radians,
				BigDecimal sumStart,
				BigInteger counterStart,
				BigInteger jump,
				BigInteger multiplier,
				bool factorialDenominator,
				int precision)
		{
			var targetPrecision = GetPrecisionTarget(precision);

			var result = sumStart;
			var n = counterStart;

			BigDecimal lastResult = -1;
			BigDecimal difference = 1;
			BigDecimal sign = 1;

			var counter = 0;

			do
			{
				if (counter > (precision * 2))
				{
					break;
				}

				BigDecimal denominator = n;

				if (factorialDenominator)
				{
					denominator = new BigDecimal(BigIntegerHelper.FastFactorial.Factorial(n));
				}

				var left = BigDecimal.One / denominator;
				var right = BigDecimal.Pow(radians, n);

				result += left * right * sign;

				if (lastResult != -1)
				{
					difference = lastResult - result;
				}

				n += jump;
				counter++;
				sign *= multiplier;
				lastResult = result;
			} while (BigDecimal.Abs(difference) > targetPrecision);

			return result;
		}

		/// <summary>
		/// Wraps the input into the range:
		/// -π/2 &lt;= θ &lt;= π/2
		/// </summary>
		internal static BigDecimal WrapInput(BigDecimal radians)
		{
			var resultSign = radians.Sign;
			radians = BigDecimal.Abs(radians);

			if (radians == 0)
			{
				return 0;
			}

			if (radians == HalfPi)
			{
				return 1 * resultSign;
			}

			if (radians > HalfPi)
			{
				var i = (int)BigDecimal.Round(radians / BigDecimal.Pi, RoundingStrategy.ToEven);
				BigDecimal sign = BigInteger.Pow(BigInteger.MinusOne, i);

				var sum = radians + HalfPi;
				var mod = BigDecimal.Mod(sum, BigDecimal.Pi);
				var wrapped = (mod - HalfPi) * sign;

				return wrapped;
			}

			return radians;
		}

		public static BigDecimal HalfPi = BigDecimal.Pi / 2.0D;

		public static BigDecimal Modulus(this BigDecimal value, BigDecimal mod)
		{
			BigDecimal left = BigDecimal.Round(BigDecimal.Divide(value, mod));
			return BigDecimal.Subtract(value, BigDecimal.Multiply(left, mod));
		}

		public static BigDecimal TwicePi = BigDecimal.Pi * 2.0D;
	}
}