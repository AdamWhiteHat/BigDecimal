using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using ExtendedNumerics;
using NUnit.Framework;
using System.Runtime.CompilerServices;

using ExtendedNumerics;
using ExtendedNumerics.Helpers;

namespace TestBigDecimal
{
	[TestFixture]
	[NonParallelizable]
	public class TestBigDecimalTrigonometricFunctions
	{
		protected static int Precision = 50;

		[OneTimeSetUp]
		public void SetUp()
		{
			Precision = 50;
			BigDecimal.Precision = Precision;
			BigDecimal.AlwaysTruncate = false;
		}

		#region Ln, Log2, Log10, LogN & Exp Functions

		[Test]
		public static void Test_Ln()
		{
			BigDecimal argument = BigDecimal.Parse("65536");
			string expected = "11.09035488895912495067571394333082508920800214976";

			BigDecimal result = BigDecimal.Ln(argument, Precision);

			string actual = new string(result.ToString().Take(Precision).ToArray());
			Assert.AreEqual(expected, actual, nameof(BigDecimal.Ln));
		}

		[Test]
		public static void Test_Log2()
		{
			BigDecimal argument = BigDecimal.Parse("46340.95002");
			string expected = "15.50000000025398948770312730360932446932123539424";

			BigDecimal result = BigDecimal.Log2(argument, Precision);

			string actual = new string(result.ToString().Take(Precision).ToArray());
			Assert.AreEqual(expected, actual, nameof(BigDecimal.Log2));
		}

		[Test]
		public static void Test_Log10()
		{
			BigDecimal argument = BigDecimal.Parse("2");
			string expected = "0.301029995663981195213738894724493026768189881462";

			BigDecimal result = BigDecimal.Log10(argument, Precision);

			string actual = new string(result.ToString().Take(Precision).ToArray());
			Assert.AreEqual(expected, actual, nameof(BigDecimal.Log10));
		}

		[Test]
		public static void Test_LogN()
		{
			int @base = 3;
			BigDecimal argument = BigDecimal.Parse("65536");
			string expected = "10.09487605714331899359243382948417366879337024211";

			BigDecimal result = BigDecimal.LogN(@base, argument, Precision);

			string actual = new string(result.ToString().Take(Precision).ToArray());
			Assert.AreEqual(expected, actual, nameof(BigDecimal.LogN));
		}

		[Test]
		public static void Test_Exp()
		{
			Test_TrigFunction(BigDecimal.Exp, Math.Exp, 11, Precision, 1);
			Test_TrigFunction(BigDecimal.Exp, Math.Exp, 11, Precision, -1);
		}

		#endregion

		#region Standard Trigonometric Functions

		[Test]
		public static void Test_Sin()
		{
			Test_TrigFunction(BigDecimal.Sin, Math.Sin, 14, Precision, 1);
			Test_TrigFunction(BigDecimal.Sin, Math.Sin, 14, Precision, -1);
		}

		[Test]
		public static void Test_Cos()
		{
			Test_TrigFunction(BigDecimal.Cos, Math.Cos, 14, Precision, 1);
			Test_TrigFunction(BigDecimal.Cos, Math.Cos, 14, Precision, -1);
		}

		[Test]
		public static void Test_Tan()
		{
			Test_TrigFunction(BigDecimal.Tan, Math.Tan, 9, Precision, 1);
			Test_TrigFunction(BigDecimal.Tan, Math.Tan, 9, Precision, -1);
		}

		[Test]
		public static void Test_Cot()
		{
			// cot = cos / sin
			Func<double, double> cot = (x) => Math.Cos(x) / Math.Sin(x);

			Test_TrigFunction(BigDecimal.Cot, cot, 9, Precision, 1);
			Test_TrigFunction(BigDecimal.Cot, cot, 9, Precision, -1);
		}

		[Test]
		public static void Test_Sec()
		{
			// sec = 1 / cos
			Func<double, double> sec = (x) => 1.0d / Math.Cos(x);

			Test_TrigFunction(BigDecimal.Sec, sec, 9, Precision, 1);
			Test_TrigFunction(BigDecimal.Sec, sec, 9, Precision, -1);
		}

		[Test]
		public static void Test_Csc()
		{
			// csc = 1 / sin
			Func<double, double> csc = (x) => 1.0d / Math.Sin(x);

			Test_TrigFunction(BigDecimal.Csc, csc, 9, Precision, 1);
			Test_TrigFunction(BigDecimal.Csc, csc, 9, Precision, -1);
		}

		#endregion

		#region Hyperbolic Trigonometric Functions

		[Test]
		public static void Test_Sinh()
		{
			Test_TrigFunction(BigDecimal.Sinh, Math.Sinh, 11, Precision, 1);
			Test_TrigFunction(BigDecimal.Sinh, Math.Sinh, 11, Precision, -1);
		}

		[Test]
		public static void Test_Cosh()
		{
			Test_TrigFunction(BigDecimal.Cosh, Math.Cosh, 11, Precision, 1);
			Test_TrigFunction(BigDecimal.Cosh, Math.Cosh, 11, Precision, -1);
		}

		[Test]
		public static void Test_Tanh()
		{
			Test_TrigFunction(BigDecimal.Tanh, Math.Tanh, 14, Precision, 1);
			Test_TrigFunction(BigDecimal.Tanh, Math.Tanh, 14, Precision, -1);
		}

		[Test]
		public static void Test_Coth()
		{
			// coth = cosh / sinh
			Func<double, double> coth = (x) => Math.Cosh(x) / Math.Sinh(x);

			Test_TrigFunction(BigDecimal.Coth, coth, 12, Precision, 1);
			Test_TrigFunction(BigDecimal.Coth, coth, 12, Precision, -1);
		}

		[Test]
		public static void Test_Sech()
		{
			// sech = 1 / cosh
			Func<double, double> sech = (x) => 1.0d / Math.Cosh(x);

			Test_TrigFunction(BigDecimal.Sech, sech, 14, Precision, 1);
			Test_TrigFunction(BigDecimal.Sech, sech, 14, Precision, -1);
		}

		#endregion

		#region Inverse Trigonometric Functions

		[Test]
		public static void Test_Arcsin()
		{
			Func<double, double> asin = (x) =>
			{
				double input = Math.Abs(x) % 1;
				return Math.Sign(x) * Math.Atan(input / Math.Sqrt(1.0d - Math.Pow(input, 2)));
			};

			Test_TrigFunction(BigDecimal.Arcsin, asin, 14, Precision * 2, 1);
			Test_TrigFunction(BigDecimal.Arcsin, asin, 14, Precision * 2, -1);
		}

		[Test]
		public static void Test_Arccos()
		{
			Func<double, double> arccos = (x) =>
			{
				double input = Math.Abs(x) % 1;
				double output = Math.Atan(Math.Sqrt(1.0d - Math.Pow(input, 2)) / input);
				if (Math.Sign(x) == -1)
				{
					return Math.PI - output;
				}
				return output;
			};

			Test_TrigFunction(BigDecimal.Arccos, arccos, 14, Precision * 2, 1);
			Test_TrigFunction(BigDecimal.Arccos, arccos, 14, Precision * 2, -1);
		}

		[Test]
		public static void Test_Arctan()
		{
			Test_TrigFunction(BigDecimal.Arctan, Math.Atan, 14, Precision, 1);
			Test_TrigFunction(BigDecimal.Arctan, Math.Atan, 14, Precision, -1);
		}

		[Test]
		public static void Test_Arccot()
		{
			Func<double, double> arccot = (x) => (Math.PI / 2) - Math.Atan(x);

			Test_TrigFunction(BigDecimal.Arccot, arccot, 14, Precision, 1);
			Test_TrigFunction(BigDecimal.Arccot, arccot, 14, Precision, -1);
		}

		[Test]
		public static void Test_Arccsc()
		{
			Func<double, double> arccsc = (x) => Math.Asin(1 / x);

			Test_TrigFunction(BigDecimal.Arccsc, arccsc, 14, Precision, 1);
			Test_TrigFunction(BigDecimal.Arccsc, arccsc, 14, Precision, -1);
		}

		#endregion

		#region Common test method: Test_TrigFunction

		protected static void Test_TrigFunction(Func<BigDecimal, int, BigDecimal> testFunc,
											Func<double, double> comparasonFunc,
											int matchDigits,
											int precision,
											int sign = 1,
											[CallerMemberName] string callerName = null)
		{

			BigDecimal E = BigDecimal.Parse("2.718281828459045235360287471352662497757247093699959574966967627724076630353547594571382178525166427");

			BigDecimal halfPi = BigDecimal.Pi / 2;
			BigDecimal preturbation = BigDecimal.Parse("0.00123");

			BigDecimal tl = sign * (-halfPi - preturbation);
			BigDecimal tg = sign * (-halfPi + preturbation);
			BigDecimal ul = sign * (-preturbation);
			BigDecimal ug = sign * (+preturbation);
			BigDecimal vl = sign * (halfPi - preturbation);
			BigDecimal vg = sign * (halfPi + preturbation);
			BigDecimal wl = sign * (BigDecimal.Pi - preturbation);
			BigDecimal wg = sign * (BigDecimal.Pi + preturbation);
			BigDecimal xl = sign * ((halfPi * 3) - preturbation);
			BigDecimal xg = sign * ((halfPi * 3) + preturbation);
			BigDecimal yl = sign * ((BigDecimal.Pi * 2) - preturbation);
			BigDecimal yg = sign * ((BigDecimal.Pi * 2) + preturbation);
			BigDecimal zl = sign * ((halfPi * 5) - preturbation);
			BigDecimal zg = sign * ((halfPi * 5) + preturbation);

			int lineLength = 47;
			int callerNameLength = (callerName.Length + 3);
			int repeatTimes = (lineLength - 3) / callerNameLength;
			int remainder = lineLength - (repeatTimes * callerNameLength);

			int leftPad = remainder / 2;
			int rightPad = remainder - leftPad;

			string sgn = (sign == -1) ? "-" : "";
			string callerNameBanner = " * ".PadRight(3 + leftPad) + string.Join(" * ", Enumerable.Repeat(callerName, repeatTimes)) + " * ".PadLeft(3 + rightPad);

			TestContext.WriteLine($"<{callerName}>");
			TestContext.WriteLine("");

			if (sign == 1)
			{
				TestContext.WriteLine(callerNameBanner);
				TestContext.WriteLine(" + ++ +++++++++++++ INPUT SIGN: POSITIVE +++++++++++++ ++ + ");
				TestContext.WriteLine(callerNameBanner);
			}
			else
			{
				TestContext.WriteLine(callerNameBanner);
				TestContext.WriteLine(" - -- ------------- INPUT SIGN: NEGATIVE ------------- -- - ");
				TestContext.WriteLine(callerNameBanner);
			}

			TestContext.WriteLine("");
			TestContext.WriteLine($"  ### -π/2 ± ℇ ###  ");
			ExecMethod(tl, testFunc, comparasonFunc, precision, matchDigits, "-π/2 - ℇ", callerName);
			ExecMethod(tg, testFunc, comparasonFunc, precision, matchDigits, "-π/2 + ℇ", callerName);

			TestContext.WriteLine("");
			TestContext.WriteLine($"  ###  0 ± ℇ  ###  ");
			ExecMethod(ul, testFunc, comparasonFunc, precision, matchDigits, "0 - ℇ", callerName);
			ExecMethod(ug, testFunc, comparasonFunc, precision, matchDigits, "0 + ℇ", callerName);

			TestContext.WriteLine("");
			TestContext.WriteLine("  ###  π/2 ± ℇ  ###  ");
			ExecMethod(vl, testFunc, comparasonFunc, precision, matchDigits, "π/2 - ℇ", callerName);
			ExecMethod(vg, testFunc, comparasonFunc, precision, matchDigits, "π/2 + ℇ", callerName);

			TestContext.WriteLine("");
			TestContext.WriteLine("  ###  π ± ℇ  ###  ");
			ExecMethod(wl, testFunc, comparasonFunc, precision, matchDigits, "π - ℇ", callerName);
			ExecMethod(wg, testFunc, comparasonFunc, precision, matchDigits, "π + ℇ", callerName);

			TestContext.WriteLine("");
			TestContext.WriteLine("  ###  ³⁄₂∙π ± ℇ  ###  ");
			ExecMethod(xl, testFunc, comparasonFunc, precision, matchDigits, "³⁄₂∙π - ℇ ", callerName);
			ExecMethod(xg, testFunc, comparasonFunc, precision, matchDigits, "³⁄₂∙π + ℇ ", callerName);

			TestContext.WriteLine("");
			TestContext.WriteLine("  ###  2π ± ℇ  ###  ");
			ExecMethod(yl, testFunc, comparasonFunc, precision, matchDigits, "2π - ℇ", callerName);
			ExecMethod(yg, testFunc, comparasonFunc, precision, matchDigits, "2π + ℇ", callerName);

			TestContext.WriteLine("");
			TestContext.WriteLine("  ###  ⁵⁄₂∙π ± ℇ  ###  ");
			ExecMethod(zl, testFunc, comparasonFunc, precision, matchDigits, "⁵⁄₂∙π - ℇ", callerName);
			ExecMethod(zg, testFunc, comparasonFunc, precision, matchDigits, "⁵⁄₂∙π + ℇ", callerName);

			TestContext.WriteLine("");
			TestContext.WriteLine(" Misc --");
			ExecMethod(sign * BigDecimal.Parse("22.123"), testFunc, comparasonFunc, precision, 5, sgn + "22.123", callerName);

			/*
			ExecMethod(sign * (E / 2), testFunc, comparasonFunc, precision, matchDigits, sgn + "(E / 2)", callerName);
			ExecMethod(E + (sign * preturbation), testFunc, comparasonFunc, precision, matchDigits,  "E + (" + sgn + "preturbation)", callerName);
			ExecMethod(sign * BigDecimal.Parse("1.000000000000187"), testFunc, comparasonFunc, precision, matchDigits, sgn + "1.000000000000187", callerName);
			ExecMethod(sign * BigDecimal.Parse("0.00000000000010641"), testFunc, comparasonFunc, precision, matchDigits, sgn + "0.00000000000010641", callerName);
			ExecMethod(sign * BigDecimal.Parse("0.73908513321516064"), testFunc, comparasonFunc, precision, matchDigits, sgn + "0.73908513321516064", callerName);
			*/

			TestContext.WriteLine("");
			TestContext.WriteLine($"</{callerName}>");
		}

		private static void ExecMethod(BigDecimal input,
										Func<BigDecimal, int, BigDecimal> testFunc,
										Func<double, double> comparasonFunc,
										int precision,
										int matchDigits,
										string testDescription,
										string callerName = null)
		{
			string inputString = $"Input: {input}";
			TestContext.WriteLine(inputString);

			double doubleInput = (double)input;
			double doubleExpected = comparasonFunc(doubleInput);
			if (double.IsNaN(doubleExpected) || double.IsInfinity(doubleExpected))
			{
				TestContext.WriteLine("SKIPPED: The value of this function for this input is undefined.");
				return;
			}

			BigDecimal bigdecimalActual = testFunc(input, precision);
			//bigdecimalActual = BigDecimal.Normalize(bigdecimalActual);

			string stringExpected = doubleExpected.ToString();
			if (stringExpected.Contains('E'))
			{
				stringExpected = doubleExpected.ToString("F16");
			}
			string stringActual = bigdecimalActual.ToString();

			int expectedDecimalOffset = stringExpected.IndexOf('.');
			int actualDecimalOffset = stringActual.IndexOf('.');

			expectedDecimalOffset = (expectedDecimalOffset == -1) ? 0 : expectedDecimalOffset;
			actualDecimalOffset = (actualDecimalOffset == -1) ? 0 : actualDecimalOffset;

			int expectedTruncLength = expectedDecimalOffset + matchDigits + 1;
			int actualTruncLength = actualDecimalOffset + matchDigits + 1;

			if (stringExpected.Length < expectedTruncLength)
			{
				expectedTruncLength = stringExpected.Length;
			}
			if (stringActual.Length < actualTruncLength)
			{
				actualTruncLength = stringActual.Length;
			}

			string truncatedExpecting = stringExpected.Substring(0, expectedTruncLength);
			string truncatedActual = stringActual.Substring(0, actualTruncLength);

			double doubleActual;
			double.TryParse(truncatedActual, out doubleActual);

			string testInfo = $"{callerName}: {testDescription}{Environment.NewLine}{inputString}";
			TestContext.WriteLine($"[{testInfo}]");
			TestContext.WriteLine("");

			bool testPassed = (truncatedExpecting == truncatedActual);
			if (testPassed)
			{
				TestContext.WriteLine($"Expecting: {truncatedExpecting}");
				TestContext.WriteLine($"   Actual: {truncatedActual}");
			}
			else
			{
				string displayString = bigdecimalActual.ToString();
				if (displayString.Length > 65)
				{
					displayString = new string(displayString.Take(65).ToArray()) + $"... ({(displayString.Length - 65)} more digits)";
				}

				TestContext.WriteLine($"Expecting: {doubleExpected}");
				TestContext.WriteLine($"   Actual: {displayString}");
			}

			TestContext.WriteLine("");


			if (testPassed)
			{
				Assert.AreEqual(truncatedExpecting, truncatedActual, testInfo);
			}
			else
			{
				double delta = 0.0001;
				Assert.AreEqual(doubleExpected, doubleActual, delta, testInfo);
			}



		}

		#endregion

	}
}
