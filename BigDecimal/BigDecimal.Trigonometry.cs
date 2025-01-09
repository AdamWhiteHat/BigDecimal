using System;
using System.Linq;
using System.Numerics;
using ExtendedNumerics.Helpers;
using ExtendedNumerics.Properties;

namespace ExtendedNumerics
{
	public readonly partial record struct BigDecimal : IComparable, IComparable<BigDecimal>, IComparable<Int32>, IComparable<Int32?>, IComparable<Decimal>, IComparable<Double>, IComparable<Single>
	{
		#region Constants Approximations

		public static BigDecimal ApproximateE(int precision)
		{
			return Exp(BigDecimal.One, precision);
		}

		public static BigDecimal ApproximatePi(int precision)
		{
			BigInteger mantissa = 3;
			int exponent = 0;

			BigInteger @base = 10;
			BigInteger k = 1;
			BigInteger twiceKplus1 = 3;
			BigInteger outputBuffer = 3;
			BigInteger magnitude = 1;
			BigInteger r = 0;
			BigInteger productOfOddNumbers = 1;

			// skip integer part
			BigInteger nextR = @base * (r - ( productOfOddNumbers * outputBuffer ));
			outputBuffer = ( ( @base * (( 3 * magnitude ) + r) ) / productOfOddNumbers ) - ( @base * outputBuffer );
			magnitude *= @base;

			int counter = 1;
			do
			{
				r = nextR;
				BigInteger nextDigitThreshold = productOfOddNumbers * outputBuffer;
				if (( ( ( 4 * magnitude ) + r ) - productOfOddNumbers ) < nextDigitThreshold)
				{
					mantissa *= 10;
					mantissa += outputBuffer;
					exponent = counter * -1;
					counter++;
					nextR = @base * (r - nextDigitThreshold);
					outputBuffer = ( ( @base * (( 3 * magnitude ) + r) ) / productOfOddNumbers ) - ( @base * outputBuffer );
					magnitude *= @base;
				}
				else
				{
					productOfOddNumbers *= twiceKplus1;
					nextR = (( 2 * magnitude ) + r) * twiceKplus1;
					outputBuffer = (( magnitude * (7 * k) ) + 2 + ( r * twiceKplus1 )) / productOfOddNumbers;
					magnitude *= k;
					twiceKplus1 += 2;
					++k;
				}

			}
			while (counter <= precision);

			return new BigDecimal(mantissa, exponent);
		}

		#endregion

		#region Standard Trigonometric Functions

		/// <summary>
		/// Arbitrary precision sine function.
		/// </summary>
		/// <param name="radians">An angle, measured in radians.</param>
		/// <returns>The sine of <paramref name="radians"/>, in radians.</returns>
		public static BigDecimal Sin(BigDecimal radians)
		{
			return Sin(radians, Precision);
		}

		/// <summary>
		/// Arbitrary precision sine function.
		/// </summary>
		/// <param name="radians">An angle, measured in radians.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		/// <returns>The sine of <paramref name="radians"/>, in radians.</returns>
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
		/// <param name="radians">An angle, measured in radians.</param>
		/// <returns>The cosine of <paramref name="radians"/>, in radians.</returns>
		public static BigDecimal Cos(BigDecimal radians)
		{
			return Cos(radians, Precision);
		}

		/// <summary>
		/// Arbitrary precision cosine function.
		/// </summary>
		/// <param name="radians">An angle, measured in radians.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		/// <returns>The cosine of <paramref name="radians"/>, in radians.</returns>
		public static BigDecimal Cos(BigDecimal radians, int precision)
		{
			return Sin(radians + TrigonometricHelper.HalfPi, precision);
		}

		/// <summary>
		/// Arbitrary precision tangent function. 
		/// The input must not be π/2 or 3π/2, as the tangent is undefined at that value.
		/// </summary>
		/// <param name="radians">An angle, measured in radians.</param>
		/// <returns>The tangent of <paramref name="radians"/>, in radians.</returns>
		public static BigDecimal Tan(BigDecimal radians)
		{
			return Tan(radians, Precision);
		}

		/// <summary>
		/// Arbitrary precision tangent function. 
		/// The input must not be π/2 or 3π/2, as the tangent is undefined at that value.
		/// </summary>
		/// <param name="radians">An angle, measured in radians.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		/// <returns>The tangent of <paramref name="radians"/>, in radians.</returns>
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
		/// <param name="radians">An angle, measured in radians, which is not zero, π or a multiple of π.</param>
		/// <returns>The cotangent of <paramref name="radians"/>, in radians.</returns>
		/// <exception cref="ArgumentException">Argument <paramref name="radians"/> cannot be zero.</exception>
		/// <exception cref="ArgumentException">Argument <paramref name="radians"/> cannot be π or a multiple of π.</exception>
		public static BigDecimal Cot(BigDecimal radians)
		{
			return Cot(radians, Precision);
		}

		/// <summary>
		/// Arbitrary precision cotangent function. 
		/// The input must not be zero, as the cotangent is undefined at that value.
		/// </summary>
		/// <param name="radians">An angle, measured in radians, which is not zero, π or a multiple of π.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		/// <returns>The cotangent of <paramref name="radians"/>, in radians.</returns>
		/// <exception cref="ArgumentException">Argument <paramref name="radians"/> cannot be zero.</exception>
		/// <exception cref="ArgumentException">Argument <paramref name="radians"/> cannot be π or a multiple of π.</exception>
		public static BigDecimal Cot(BigDecimal radians, int precision)
		{
			if (radians.IsZero())
			{
				throw new ArgumentException(string.Format(LanguageResources.Arg_CannotBeZero, nameof(radians)), nameof(radians));
			}

			BigDecimal modPi = Normalize(Mod(radians, Pi));
			if (modPi.IsZero())
			{
				throw new ArgumentException(string.Format(LanguageResources.Arg_CannotBePiMultiple, nameof(radians)), nameof(radians));
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
		/// <param name="radians">An angle, measured in radians.</param>
		/// <returns>The secant of <paramref name="radians"/>, in radians.</returns>
		public static BigDecimal Sec(BigDecimal radians)
		{
			return Sec(radians, Precision);
		}

		/// <summary>
		/// Arbitrary precision secant function. 
		/// The input must not be (2*n + 1)*π/2 (an odd multiple of π/2), as the secant is undefined at that value.
		/// </summary>
		/// <param name="radians">An angle, measured in radians.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		/// <returns>The secant of <paramref name="radians"/>, in radians.</returns>
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
		/// <param name="radians">An angle, measured in radians, which is not zero, π or a multiple of π.</param>
		/// <returns>The cosecant of <paramref name="radians"/>, in radians.</returns>
		/// <exception cref="ArgumentException">Argument <paramref name="radians"/> cannot be zero.</exception>
		/// <exception cref="ArgumentException">Argument <paramref name="radians"/> cannot be π or a multiple of π.</exception>
		public static BigDecimal Csc(BigDecimal radians)
		{
			return Csc(radians, Precision);
		}

		/// <summary>
		/// Arbitrary precision cosecant function. 
		/// The input must not be zero or π, as the cosecant is undefined at that value.
		/// </summary>
		/// <param name="radians">An angle, measured in radians, which is not zero, π or a multiple of π.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		/// <returns>The cosecant of <paramref name="radians"/>, in radians.</returns>
		/// <exception cref="ArgumentException">Argument <paramref name="radians"/> cannot be zero.</exception>
		/// <exception cref="ArgumentException">Argument <paramref name="radians"/> cannot be π or a multiple of π.</exception>
		public static BigDecimal Csc(BigDecimal radians, int precision)
		{
			if (radians.IsZero())
			{
				throw new ArgumentException(string.Format(LanguageResources.Arg_CannotBeZero, nameof(radians)), nameof(radians));
			}

			BigDecimal modPi = Normalize(Mod(radians, Pi));
			if (modPi.IsZero())
			{
				throw new ArgumentException(string.Format(LanguageResources.Arg_CannotBePiMultiple, nameof(radians)), nameof(radians));
			}

			// Wrap around at 2π
			BigDecimal twoPi = Normalize(Mod(radians, 2 * Pi));

			BigDecimal sin = Sin(twoPi, precision);
			return One / sin; // csc = 1 / sin
		}

		#endregion

		#region Hyperbolic Trigonometric Functions

		/// <summary>Arbitrary precision hyperbolic sine function.</summary>
		/// <param name="radians">An angle, measured in radians.</param>
		/// <returns>The cosecant of <paramref name="radians"/>, in radians.</returns>
		public static BigDecimal Sinh(BigDecimal radians)
		{
			return Sinh(radians, Precision);
		}

		/// <summary>
		/// Arbitrary precision hyperbolic sine function.
		/// </summary>
		/// <param name="radians">An angle, measured in radians.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		/// <returns>The cosecant of <paramref name="radians"/>, in radians.</returns>
		public static BigDecimal Sinh(BigDecimal radians, int precision)
		{
			BigDecimal sumStart = 0;
			BigInteger counterStart = 1;
			BigInteger jump = 2;
			BigInteger multiplier = 1;
			bool factorialDenominator = true;

			return TrigonometricHelper.TaylorSeriesSum(radians, sumStart, counterStart, jump, multiplier, factorialDenominator, precision);
		}

		/// <summary>Arbitrary precision hyperbolic cosine function.</summary>
		/// <param name="radians">An angle, measured in radians.</param>
		/// <returns>The hyperbolic cosine of <paramref name="radians"/>, in radians.</returns>
		public static BigDecimal Cosh(BigDecimal radians)
		{
			return Cosh(radians, Precision);
		}

		/// <summary>
		/// Arbitrary precision hyperbolic cosine function.
		/// </summary>
		/// <param name="radians">An angle, measured in radians.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		/// <returns>The hyperbolic cosine of <paramref name="radians"/>, in radians.</returns>
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
		/// <param name="radians">An angle, measured in radians.</param>
		/// <returns>The hyperbolic tangent of <paramref name="radians"/>, in radians.</returns>
		public static BigDecimal Tanh(BigDecimal radians)
		{
			return Tanh(radians, Precision);
		}

		/// <summary>Arbitrary precision hyperbolic tangent function.</summary>
		/// <param name="radians">An angle, measured in radians.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		/// <returns>The hyperbolic tangent of <paramref name="radians"/>, in radians.</returns>
		public static BigDecimal Tanh(BigDecimal radians, int precision)
		{
			BigDecimal sinh = Sinh(radians, precision);
			BigDecimal cosh = Cosh(radians, precision);

			return sinh / cosh; // tan = sinh / cosh
		}

		/// <summary>Arbitrary precision hyperbolic cotangent function.</summary>
		/// <param name="radians">An angle, measured in radians. <paramref name="radians"/> cannot be zero.</param>
		/// <returns>The hyperbolic cotangent of <paramref name="radians"/>, in radians.</returns>
		/// <exception cref="ArgumentException">Argument <paramref name="radians"/> cannot be zero.</exception>
		public static BigDecimal Coth(BigDecimal radians)
		{
			return Coth(radians, Precision);
		}

		/// <summary>
		/// Arbitrary precision hyperbolic cotangent function.
		/// The input must not be zero.
		/// </summary>
		/// <param name="radians">An angle, measured in radians. <paramref name="radians"/> cannot be zero.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		/// <returns>The hyperbolic cotangent of <paramref name="radians"/>, in radians.</returns>
		/// <exception cref="ArgumentException">Argument <paramref name="radians"/> cannot be zero.</exception>
		public static BigDecimal Coth(BigDecimal radians, int precision)
		{
			BigDecimal input = Normalize(radians);
			if (input.IsZero())
			{
				throw new ArgumentException(string.Format(LanguageResources.Arg_CannotBeZero, nameof(radians)), nameof(radians));
			}

			BigDecimal cosh = Cosh(radians, precision);
			BigDecimal sinh = Sinh(radians, precision);

			return cosh / sinh; // coth = cosh / sinh
		}

		/// <summary>Arbitrary precision hyperbolic secant function.</summary>
		/// <param name="radians">An angle, measured in radians.</param>
		/// <returns>The hyperbolic secant of <paramref name="radians"/>, in radians.</returns>
		public static BigDecimal Sech(BigDecimal radians)
		{
			return Sech(radians, Precision);
		}

		/// <summary>Arbitrary precision hyperbolic secant function.</summary>
		/// <param name="radians">An angle, measured in radians.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		/// <returns>The hyperbolic secant of <paramref name="radians"/>, in radians.</returns>
		public static BigDecimal Sech(BigDecimal radians, int precision)
		{
			BigDecimal cosh = Cosh(radians, precision);

			return One / cosh;   // sech = 1 / cosh
		}

		/// <summary>
		/// Arbitrary precision hyperbolic cosecant function.
		/// The input must not be zero.
		/// </summary>
		/// <param name="radians">An angle, measured in radians. <paramref name="radians"/> cannot be zero.</param>
		/// <returns>The hyperbolic cosecant of <paramref name="radians"/>, in radians.</returns>
		/// <exception cref="ArgumentException">Argument <paramref name="radians"/> cannot be zero.</exception>
		public static BigDecimal Csch(BigDecimal radians)
		{
			return Csch(radians, Precision);
		}

		/// <summary>
		/// Arbitrary precision hyperbolic cosecant function.
		/// The input must not be zero.
		/// </summary>
		/// <param name="radians">An angle, measured in radians. <paramref name="radians"/> cannot be zero.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		/// <returns>The hyperbolic cosecant of <paramref name="radians"/>, in radians.</returns>
		/// <exception cref="ArgumentException">Argument <paramref name="radians"/> cannot be zero.</exception>
		public static BigDecimal Csch(BigDecimal radians, int precision)
		{
			BigDecimal input = Normalize(radians);
			if (input.IsZero())
			{
				throw new ArgumentException(string.Format(LanguageResources.Arg_CannotBeZero, nameof(radians)), nameof(radians));
			}

			BigDecimal sinh = Sinh(input, precision);

			return One / sinh;   // csch = 1 / sinh
		}

		#endregion

		#region Inverse Trigonometric Functions

		/// <summary>Arbitrary precision inverse sine function.</summary>
		/// <param name="radians">An angle, measured in radians, in the domain of -1 &lt; x &lt; 1</param>
		/// <returns>The inverse sine of <paramref name="radians"/>, in radians.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Argument <paramref name="radians"/> outside the domain of -1 &lt; x &lt; 1.</exception>
		public static BigDecimal Arcsin(BigDecimal radians)
		{
			return Arcsin(radians, Precision);
		}

		/// <summary>Arbitrary precision inverse sine function.</summary>
		/// <param name="radians">An angle, measured in radians, in the domain of -1 &lt; x &lt; 1</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		/// <exception cref="ArgumentOutOfRangeException">The domain of <paramref name="radians" /> is -1 &lt; x &lt; 1</exception>
		/// <returns>The inverse sine of <paramref name="radians"/>, in radians.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Argument <paramref name="radians"/> outside the domain of -1 &lt; x &lt; 1.</exception>
		public static BigDecimal Arcsin(BigDecimal radians, int precision)
		{
			if (( radians < -1 ) || ( radians > One ))
			{
				throw new ArgumentOutOfRangeException(nameof(radians), string.Format(LanguageResources.Arg_TheDomainOf_0_Is_1, nameof(Arcsin), "-1 < x < 1"));
			}

			int sign = radians.Sign;
			BigDecimal input = Abs(radians);
			BigDecimal denominator = SquareRoot(One - (input * input), precision);
			BigDecimal quotient = input / denominator;
			return sign * Arctan(quotient, precision);
		}

		/// <summary>Arbitrary precision inverse cosine function.</summary>
		/// <param name="radians">An angle, measured in radians, in the domain of -1 &lt; x &lt; 1</param>
		/// <returns>The inverse cosine of <paramref name="radians"/>, in radians.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Argument <paramref name="radians"/> outside the domain of -1 &lt; x &lt; 1.</exception>
		public static BigDecimal Arccos(BigDecimal radians)
		{
			return Arccos(radians, Precision);
		}

		/// <summary>Arbitrary precision inverse cosine function.</summary>
		/// <param name="radians">An angle, measured in radians, in the domain of -1 &lt; x &lt; 1</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		/// <returns>The inverse cosine of <paramref name="radians"/>, in radians.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Argument <paramref name="radians"/> outside the domain of -1 &lt; x &lt; 1.</exception>
		public static BigDecimal Arccos(BigDecimal radians, int precision)
		{
			if (( radians < -1 ) || ( radians > One ))
			{
				throw new ArgumentOutOfRangeException(nameof(radians), string.Format(LanguageResources.Arg_TheDomainOf_0_Is_1, nameof(Arccos), "-1 < x < 1"));
			}

			int sign = radians.Sign;
			BigDecimal input = Abs(radians);
			BigDecimal denominator = SquareRoot(One - (input * input), precision);
			BigDecimal quotient = denominator / input;
			if (sign == -1)
			{
				return Pi - Arctan(quotient, precision);
			}
			return Arctan(quotient, precision);
		}

		/// <summary>Arbitrary precision inverse tangent function.</summary>
		/// <param name="radians">An angle, measured in radians.</param>
		/// <returns>The inverse tangent of <paramref name="radians"/>, in radians.</returns>
		public static BigDecimal Arctan(BigDecimal radians)
		{
			return Arctan(radians, Precision);
		}

		/// <summary>Arbitrary precision inverse tangent function.</summary>
		/// <param name="radians">An angle, measured in radians.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		/// <returns>The inverse tangent of <paramref name="radians"/>, in radians.</returns>
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
		/// <param name="radians">An angle, measured in radians.</param>
		/// <returns>The inverse cotangent of <paramref name="radians"/>, in radians.</returns>
		public static BigDecimal Arccot(BigDecimal radians)
		{
			return Arccot(radians, Precision);
		}

		/// <summary>Arbitrary precision inverse cotangent function.</summary>
		/// <param name="radians">An angle, measured in radians.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		/// <returns>The inverse cotangent of <paramref name="radians"/>, in radians.</returns>
		public static BigDecimal Arccot(BigDecimal radians, int precision)
		{
			return TrigonometricHelper.HalfPi - Arctan(radians, precision);
		}

		/// <summary>Arbitrary precision inverse secant function.</summary>
		/// <param name="radians">An angle, measured in radians, in the domain of -∞ &lt; x &lt;= -1 ∪ 1 &lt;= x &lt; ∞.</param>
		/// <returns>The inverse secant of <paramref name="radians"/>, in radians.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Argument <paramref name="radians"/> outside the domain of -∞ &lt; x &lt;= -1 ∪ 1 &lt;= x &lt; ∞.</exception>
		public static BigDecimal Arcsec(BigDecimal radians)
		{
			return Arcsec(radians, Precision);
		}

		/// <summary>Arbitrary precision inverse secant function.</summary>
		/// <param name="radians">An angle, measured in radians, in the domain of -∞ &lt; x &lt;= -1 ∪ 1 &lt;= x &lt; ∞.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		/// <returns>The inverse secant of <paramref name="radians"/>, in radians.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Argument <paramref name="radians"/> outside the domain of -∞ &lt; x &lt;= -1 ∪ 1 &lt;= x &lt; ∞.</exception>
		public static BigDecimal Arcsec(BigDecimal radians, int precision)
		{
			if (( radians > -1 ) && ( radians < 1 ))
			{
				throw new ArgumentOutOfRangeException(nameof(radians), string.Format(LanguageResources.Arg_TheDomainOf_0_Is_1, nameof(Arcsec), "-∞ < x <= -1 ∪ 1 <= x < ∞"));
			}

			return Arccos(One / radians, precision);
		}

		/// <summary>Arbitrary precision inverse cosecant function.</summary>
		/// <param name="radians">An angle, measured in radians, in the domain of -∞ &lt; x &lt;= -1 ∪ 1 &lt;= x &lt; ∞.</param>
		/// <returns>The inverse cosecant of <paramref name="radians"/>, in radians.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Argument <paramref name="radians"/> outside the domain of -∞ &lt; x &lt;= -1 ∪ 1 &lt;= x &lt; ∞.</exception>
		public static BigDecimal Arccsc(BigDecimal radians)
		{
			return Arccsc(radians, Precision);
		}

		/// <summary>Arbitrary precision inverse cosecant function.</summary>
		/// <param name="radians">An angle, measured in radians, in the domain of -∞ &lt; x &lt;= -1 ∪ 1 &lt;= x &lt; ∞.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		/// <returns>The inverse cosecant of <paramref name="radians"/>, in radians.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Argument <paramref name="radians"/> outside the domain of -∞ &lt; x &lt;= -1 ∪ 1 &lt;= x &lt; ∞.</exception>
		public static BigDecimal Arccsc(BigDecimal radians, int precision)
		{
			if (( radians > -1 ) && ( radians < 1 ))
			{
				throw new ArgumentOutOfRangeException(nameof(radians), string.Format(LanguageResources.Arg_TheDomainOf_0_Is_1, nameof(Arccsc), "-∞ < x <= -1 ∪ 1 <= x < ∞"));
			}

			return Arcsin(One / radians, precision);
		}

		#endregion

		#region Natural Log & Exponentiation Function

		/// <summary>Calculates e^x to arbitrary precision.</summary>
		/// <returns>The number <see langword="e"/> raised to the specified power.</returns>
		public static BigDecimal Exp(BigDecimal x)
		{
			return Exp(x, Precision);
		}

		/// <summary>Calculates e^x to arbitrary precision.</summary>
		/// <param name="x">The exponent to raise e to the power of.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		/// <returns>The number <see langword="e"/> raised to the specified power.</returns>
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
		/// <returns>The natural (base <see langword="e" />) logarithm of the specified number.</returns>
		public static BigDecimal Ln(BigDecimal argument)
		{
			return Ln(argument, Precision);
		}

		/// <summary>
		/// Returns the natural logarithm of the input to a specified precision.
		/// </summary>
		/// <param name="argument">The argument to take the natural logarithm of.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>	
		/// <returns>The natural (base <see langword="e" />) logarithm of the specified number.</returns>
		public static BigDecimal Ln(BigDecimal argument, int precision)
		{
			if (argument.Equals(BigDecimal.One))
			{
				return BigDecimal.Zero;
			}

			int sign = argument.Sign;
			BigDecimal input = BigDecimal.Abs(argument);

			if (( input <= 0.66d ) || ( input >= 1.33d ))
			{
				BigDecimal cubeRoot = BigDecimal.NthRoot(input, 3, precision);
				BigDecimal lnCubeRoot = Ln(cubeRoot, precision + 1);

				return sign * (lnCubeRoot * 3); // Because ln(a * b) = ln(a) + ln(b)
			}
			else
			{
				return sign * LogNatural(input, precision);
			}
		}

		/// <summary>
		/// Internal implementation of the natural log function to arbitrary precision.
		/// </summary>	
		/// <param name="argument">The argument to take the natural logarithm of.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		/// <returns>The natural (base <see langword="e" />) logarithm of the specified number.</returns>
		internal static BigDecimal LogNatural(BigDecimal argument, int precision)
		{
			BigDecimal rads = argument - 1;
			BigDecimal sumStart = 0;
			BigInteger counterStart = 1;
			BigInteger jump = 1;
			BigInteger multiplier = -1;
			bool factorialDenominator = false;

			return TrigonometricHelper.TaylorSeriesSum(rads, sumStart, counterStart, jump, multiplier, factorialDenominator, precision);
		}

		#endregion

		#region Arbitrary Base Logarithm

		/// <summary>
		/// Returns the logarithm of an argument in an arbitrary base to the number of digits specified in <see cref="Precision"/>.
		/// </summary>
		/// <param name="base">The base of the logarithm.</param>
		/// <param name="argument">The argument to take the logarithm of.</param>
		/// <returns>The logarithm of the argument in the specified base to the number of digits specified in <see cref="Precision"/>..</returns>
		public static BigDecimal Log(BigDecimal argument, int @base)
		{
			return Log(argument, @base, Precision);
		}

		/// <summary>
		/// Returns the logarithm of an argument in an arbitrary base.
		/// </summary>
		/// <param name="base">The base of the logarithm.</param>
		/// <param name="argument">The argument to take the logarithm of.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		/// <returns>The logarithm of the argument in the specified base.</returns>
		public static BigDecimal Log(BigDecimal argument, int @base, int precision)
		{
			return LogN(@base, argument, precision + 1);
		}

		/// <summary>
		/// Returns the logarithm of an argument in an arbitrary base to the number of digits specified in <see cref="Precision"/>.
		/// </summary>
		/// <param name="base">The base of the logarithm.</param>
		/// <param name="argument">The argument to take the logarithm of.</param>
		/// <returns>The logarithm of the argument in the specified base to the number of digits specified in <see cref="Precision"/>..</returns>
		public static BigDecimal LogN(int @base, BigDecimal argument)
		{
			return LogN(@base, argument, Precision);
		}

		/// <summary>
		/// Returns the logarithm of an argument in an arbitrary base.
		/// </summary>
		/// <param name="base">The base of the logarithm.</param>
		/// <param name="argument">The argument to take the logarithm of.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		/// <returns>The logarithm of the argument in the specified base.</returns>
		public static BigDecimal LogN(int @base, BigDecimal argument, int precision)
		{
			// Use change of base formula: logn(b, a) = ln(a) / ln(b)
			return Ln(argument, precision + 1) / Ln(@base, precision + 1);
		}

		/// <summary>
		/// Returns the base-2 logarithm of an argument to the number of digits specified in <see cref="Precision"/>.
		/// </summary>
		/// <param name="argument">The argument to take the base-2 logarithm of.</param>
		/// /// <returns>The base 2 logarithm of the argument to the number of digits specified in <see cref="Precision"/>.</returns>
		public static BigDecimal Log2(BigDecimal argument)
		{
			return Log2(argument, Precision);
		}

		/// <summary>
		/// Returns the base-2 logarithm of an argument.
		/// </summary>
		/// <param name="argument">The argument to take the base-2 logarithm of.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		/// <returns>The base 2 logarithm of the argument.</returns>
		public static BigDecimal Log2(BigDecimal argument, int precision)
		{
			return LogN(2, argument, precision);
		}

		/// <summary>
		/// Returns the base-10 logarithm of an argument.
		/// </summary>
		/// <param name="argument">The argument to take the base-10 logarithm of.</param>
		/// <returns>The base 10 logarithm of the argument.</returns>
		public static BigDecimal Log10(BigDecimal argument)
		{
			return Log10(argument, Precision);
		}

		/// <summary>
		/// Returns the base-10 logarithm of an argument.
		/// </summary>
		/// <param name="argument">The argument to take the base-10 logarithm of.</param>
		/// <param name="precision">The desired precision in terms of the number of digits to the right of the decimal.</param>
		/// <returns>The base 10 logarithm of the argument.</returns>
		public static BigDecimal Log10(BigDecimal argument, int precision)
		{
			return LogN(10, argument, precision);
		}

		#endregion
	}
}