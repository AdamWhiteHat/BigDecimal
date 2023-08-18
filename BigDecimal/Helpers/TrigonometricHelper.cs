using System;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ExtendedNumerics.Helpers
{
	public static class TrigonometricHelper
	{
		internal static BigDecimal HalfPi = BigDecimal.Pi / 2;

		/// <summary>
		/// Calculates a Taylor Series Sum until the specified precision is met.
		/// Based on its parameters, this can approximate several different functions
		/// including the sin, cos, sinh, cosh, and exp trigonometric functions.
		/// </summary>
		/// <param name="radians">The indeterminant value in the Taylor Series that gets multiplied by each term, raised to some power.</param>
		/// <param name="sumStart">The value to initialize the running total to. Typically this is either zero or one.</param>
		/// <param name="counterStart">The term number to start the series at. Typically this is either zero or one.</param>
		/// <param name="jump">
		/// How much to increment the term index each iteration.
		/// If you want to sum only the even terms, set the counterStart to an even number and this parameter to two. </param>
		/// <param name="multiplier">
		/// Each term is multiplied by a variable called sign. By default, sign is equal to 1. 
		/// Each iteration, sign is set to sign multiplied by this value.
		/// The point of this is to allow every other term to be negative (so subtracted from the sum) by setting this to parameter to -1.
		/// Setting this to parameter to -1 will flip the sign of the sign variable every iteration. 
		/// Since this gets multiplied by the term, the effect is to flip the sign of every other term.
		/// Set this parameter to 1 if all the terms should remain positive.
		/// </param>
		/// <param name="factorialDenominator">
		/// A boolean indicating if the denominator of the term should be passed to the factorial function.
		/// Typically this is true, but sometimes the factorial in the denominator cancels out,
		/// and so we need a way to turn this off.
		/// </param>
		/// <param name="precision">
		/// The required precision to achieve before returning, in terms of the number of correct digits to the right of the decimal point.
		/// </param>
		/// <returns></returns>
		internal static BigDecimal TaylorSeriesSum(
			BigDecimal radians,
			BigDecimal sumStart,
			BigInteger counterStart,
			BigInteger jump,
			BigInteger multiplier,
			bool factorialDenominator,
			int precision)
		{
			BigDecimal targetPrecision = GetPrecisionTarget(precision);

			BigDecimal result = sumStart;
			BigInteger n = counterStart;

			BigDecimal lastResult = -1;
			BigDecimal difference = 1;
			BigDecimal sign = 1;

			int counter = 0;
			do
			{
				if (counter > (precision * 2))
				{
					break;
				}

				BigDecimal denominator = n;
				if (factorialDenominator)
				{
					denominator = new BigDecimal(mantissa: BigIntegerHelper.FastFactorial.Factorial(n));
				}

				BigDecimal left = BigDecimal.One / denominator;
				BigDecimal right = BigDecimal.Pow(radians, n);

				result += (left * right * sign);

				if (lastResult != -1)
				{
					difference = lastResult - result;
				}

				n += jump;
				counter++;
				sign = sign * multiplier;
				lastResult = result;
			}
			while (BigDecimal.Abs(difference) > targetPrecision);

			return result;
		}

		public static BigDecimal Modulus(BigDecimal value, BigDecimal mod)
		{
			BigDecimal left = BigDecimal.Round(BigDecimal.Divide(value, mod));
			return BigDecimal.Subtract(value, BigDecimal.Multiply(left, mod));
		}

		/// <summary>
		/// Common function to generate the target value to compare against to see if 
		/// an operation has reached sufficient precision.
		/// The point of this method instead of having it inline is that we have only
		/// one place to change if we need to increase the value we are adding to
		/// precision to get adjustedPrecision.
		/// </summary>		
		internal static BigDecimal GetPrecisionTarget(int precision)
		{
			int adjustedPrecision = precision + 1;
			return new BigDecimal(mantissa: BigInteger.One, exponent: -adjustedPrecision);
		}

		/// <summary>
		/// Wraps the input into the range:
		/// -π/2 &lt;= θ &lt;= π/2
		/// </summary>
		internal static BigDecimal WrapInput(BigDecimal radians)
		{
			int resultSign = radians.Sign;
			radians = BigDecimal.Abs(radians);

			if (radians == 0)
			{
				return 0;
			}
			else if (radians == HalfPi)
			{
				return 1 * resultSign;
			}
			else if (radians > HalfPi)
			{
				int i = (int)BigDecimal.Round(radians / BigDecimal.Pi, MidpointRounding.ToEven);
				BigDecimal sign = BigInteger.Pow(BigInteger.MinusOne, i);

				BigDecimal sum = radians + HalfPi;
				BigDecimal mod = BigDecimal.Mod(sum, BigDecimal.Pi);
				BigDecimal wrapped = (mod - HalfPi) * sign;
				return wrapped;
			}
			else
			{
				return radians;
			}
		}

		/// <summary>
		/// Return 1 if radians is an odd multiple of π/2, 0 otherwise.
		/// </summary>
		internal static int ModOddHalfPi(BigDecimal radians)
		{
			BigDecimal modPiOverTwo = BigDecimal.Normalize(BigDecimal.Mod(radians, TrigonometricHelper.HalfPi));
			if (modPiOverTwo.IsZero()) // Is divisible by π/2
			{
				BigDecimal divideByPiOverTwo = BigDecimal.Normalize(BigDecimal.Divide(radians, TrigonometricHelper.HalfPi)); // Well how many times is it divisible by π/2

				BigDecimal mod2 = BigDecimal.Normalize(BigDecimal.Mod(divideByPiOverTwo, new BigDecimal(2))); // Is that number of times odd? Remainder of 1 == yes; 0 == no
				return (int)mod2;
			}

			return 0;
		}
	}
}
