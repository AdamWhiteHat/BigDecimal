namespace TestBigDecimal;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Threading;
using ExtendedNumerics;
using ExtendedNumerics.Helpers;
using NUnit.Framework;

[TestFixture]
[Culture("en-US,ru-RU")]
[NonParallelizable]
public class TestBigDecimalTrigonometricFunctions
{
	private static Dictionary<string, Func<BigDecimal, bool>> FunctionDomainTestDictionary = new Dictionary<string, Func<BigDecimal, bool>>
	{
		{ "Arccsc", new Func<BigDecimal, bool>((radians) => { return (radians > -1 && radians < 1); })},
		{ "Arcsec", new Func<BigDecimal, bool>((radians) => { return (radians > -1 && radians < 1); })},
		{ "Arccos", new Func<BigDecimal, bool>((radians) => { return (radians < -1 || radians > BigDecimal.One); })},
		{ "Arcsin", new Func<BigDecimal, bool>((radians) => { return (radians <= -1 || radians >= BigDecimal.One); })},
		{ "Csch", new Func<BigDecimal, bool>((radians) => { return ( BigDecimal.Normalize(radians).IsZero()); })},
		{ "Coth", new Func<BigDecimal, bool>((radians) => { return ( BigDecimal.Normalize(radians).IsZero()); })}
	};

	private static void ExecMethod(
		BigDecimal input,
		Func<BigDecimal, Int32, BigDecimal> testFunc,
		Func<Double, Double> comparisonFunc,
		Int32 precision,
		Int32 matchDigits,
		String testDescription,
		String? callerName = null)
	{
		var matches = FunctionDomainTestDictionary.Where(kvp => callerName.Contains(kvp.Key));
		if (matches.Any())
		{
			Func<BigDecimal, bool> argumentInRangePredicate = matches.First().Value;

			if (argumentInRangePredicate.Invoke(input))
			{
				return;
			}
		}

		var inputString = $"Input: {input}";
		TestContext.WriteLine(inputString);

		var doubleInput = (Double)input;
		var doubleExpected = comparisonFunc(doubleInput);

		if (Double.IsNaN(doubleExpected) || Double.IsInfinity(doubleExpected))
		{
			TestContext.WriteLine("SKIPPED: The value of this function for this input is undefined.");

			return;
		}

		var bigdecimalActual = testFunc(input, precision);

		//bigdecimalActual = BigDecimal.Normalize(bigdecimalActual);

		var stringExpected = doubleExpected.ToString();

		if (stringExpected.Contains('E'))
		{
			stringExpected = doubleExpected.ToString("F16");
		}

		var stringActual = bigdecimalActual.ToString();

		var expectedDecimalOffset = stringExpected.IndexOf('.');
		var actualDecimalOffset = stringActual.IndexOf('.');

		expectedDecimalOffset = expectedDecimalOffset == -1 ? 0 : expectedDecimalOffset;
		actualDecimalOffset = actualDecimalOffset == -1 ? 0 : actualDecimalOffset;

		var expectedTruncLength = expectedDecimalOffset + matchDigits + 1;
		var actualTruncLength = actualDecimalOffset + matchDigits + 1;

		if (stringExpected.Length < expectedTruncLength)
		{
			expectedTruncLength = stringExpected.Length;
		}

		if (stringActual.Length < actualTruncLength)
		{
			actualTruncLength = stringActual.Length;
		}

		var truncatedExpecting = stringExpected.Substring(0, expectedTruncLength);
		var truncatedActual = stringActual.Substring(0, actualTruncLength);

		Double.TryParse(truncatedActual, out var doubleActual);

		var testInfo = $"{callerName}: {testDescription}{Environment.NewLine}{inputString}";
		TestContext.WriteLine($"[{testInfo}]");
		TestContext.WriteLine("");

		var testPassed = truncatedExpecting == truncatedActual;

		if (testPassed)
		{
			TestContext.WriteLine($"Expecting: {truncatedExpecting}");
			TestContext.WriteLine($"   Actual: {truncatedActual}");
		}
		else
		{
			var displayString = bigdecimalActual.ToString();

			if (displayString.Length > 65)
			{
				displayString = new String(displayString.Take(65).ToArray()) + $"... ({displayString.Length - 65} more digits)";
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
			var delta = 0.0001;
			Assert.AreEqual(doubleExpected, doubleActual, delta, testInfo);
		}
	}

	protected static Int32 Precision = 50;

	protected static void Test_TrigFunction(
		Func<BigDecimal, Int32, BigDecimal> testFunc,
		Func<Double, Double> comparisonFunc,
		Int32 matchDigits,
		Int32 precision,
		Int32 sign = 1,
		[CallerMemberName] String callerName = "")
	{

		//var e = BigDecimal.Parse("2.718281828459045235360287471352662497757247093699959574966967627724076630353547594571382178525166427");

		var halfPi = TrigonometricHelper.HalfPi;

		var format = Thread.CurrentThread.CurrentCulture.NumberFormat;
		var val = TestBigDecimalHelper.PrepareValue("0.00123", format);
		var perturbation = BigDecimal.Parse(val);

		var negHalfPi = -halfPi;

		var tl = sign * (negHalfPi - perturbation);
		var tg = sign * (negHalfPi + perturbation);
		var ul = sign * -perturbation;
		var ug = sign * +perturbation;
		var vl = sign * (halfPi - perturbation);
		var vg = sign * (halfPi + perturbation);
		var wl = sign * (BigDecimal.Pi - perturbation);
		var wg = sign * (BigDecimal.Pi + perturbation);

		var halfPiTimes3 = halfPi * 3;

		var xl = sign * (halfPiTimes3 - perturbation);
		var xg = sign * (halfPiTimes3 + perturbation);
		var yl = sign * (TrigonometricHelper.TwicePi - perturbation);
		var yg = sign * (TrigonometricHelper.TwicePi + perturbation);

		var halfPiTimes5 = halfPi * 5;

		var zl = sign * (halfPiTimes5 - perturbation);
		var zg = sign * (halfPiTimes5 + perturbation);

		var lineLength = 47;
		var callerNameLength = callerName.Length + 3;
		var repeatTimes = (lineLength - 3) / callerNameLength;
		var remainder = lineLength - (repeatTimes * callerNameLength);

		var leftPad = remainder / 2;
		var rightPad = remainder - leftPad;

		var sgn = sign == -1 ? "-" : "";

		var callerNameBanner = " * ".PadRight(3 + leftPad) + String.Join(" * ", Enumerable.Repeat(callerName, repeatTimes)) +
							   " * ".PadLeft(3 + rightPad);

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
		ExecMethod(tl, testFunc, comparisonFunc, precision, matchDigits, "-π/2 - ℇ", callerName);
		ExecMethod(tg, testFunc, comparisonFunc, precision, matchDigits, "-π/2 + ℇ", callerName);

		TestContext.WriteLine("");
		TestContext.WriteLine($"  ###  0 ± ℇ  ###  ");
		ExecMethod(ul, testFunc, comparisonFunc, precision, matchDigits, "0 - ℇ", callerName);
		ExecMethod(ug, testFunc, comparisonFunc, precision, matchDigits, "0 + ℇ", callerName);

		TestContext.WriteLine("");
		TestContext.WriteLine("  ###  π/2 ± ℇ  ###  ");
		ExecMethod(vl, testFunc, comparisonFunc, precision, matchDigits, "π/2 - ℇ", callerName);
		ExecMethod(vg, testFunc, comparisonFunc, precision, matchDigits, "π/2 + ℇ", callerName);

		TestContext.WriteLine("");
		TestContext.WriteLine("  ###  π ± ℇ  ###  ");
		ExecMethod(wl, testFunc, comparisonFunc, precision, matchDigits, "π - ℇ", callerName);
		ExecMethod(wg, testFunc, comparisonFunc, precision, matchDigits, "π + ℇ", callerName);

		TestContext.WriteLine("");
		TestContext.WriteLine("  ###  ³⁄₂∙π ± ℇ  ###  ");
		ExecMethod(xl, testFunc, comparisonFunc, precision, matchDigits, "³⁄₂∙π - ℇ ", callerName);
		ExecMethod(xg, testFunc, comparisonFunc, precision, matchDigits, "³⁄₂∙π + ℇ ", callerName);

		TestContext.WriteLine("");
		TestContext.WriteLine("  ###  2π ± ℇ  ###  ");
		ExecMethod(yl, testFunc, comparisonFunc, precision, matchDigits, "2π - ℇ", callerName);
		ExecMethod(yg, testFunc, comparisonFunc, precision, matchDigits, "2π + ℇ", callerName);

		TestContext.WriteLine("");
		TestContext.WriteLine("  ###  ⁵⁄₂∙π ± ℇ  ###  ");
		ExecMethod(zl, testFunc, comparisonFunc, precision, matchDigits, "⁵⁄₂∙π - ℇ", callerName);
		ExecMethod(zg, testFunc, comparisonFunc, precision, matchDigits, "⁵⁄₂∙π + ℇ", callerName);

		TestContext.WriteLine("");
		TestContext.WriteLine(" Misc --");

		val = TestBigDecimalHelper.PrepareValue("22.123", format);
		ExecMethod(sign * BigDecimal.Parse(val), testFunc, comparisonFunc, precision, 5, sgn + val, callerName);

		/*
		ExecMethod(sign * (E / 2), testFunc, comparisonFunc, precision, matchDigits, sgn + "(E / 2)", callerName);
		ExecMethod(E + (sign * perturbation), testFunc, comparisonFunc, precision, matchDigits,  "E + (" + sgn + "perturbation)", callerName);
		ExecMethod(sign * BigDecimal.Parse("1.000000000000187"), testFunc, comparisonFunc, precision, matchDigits, sgn + "1.000000000000187", callerName);
		ExecMethod(sign * BigDecimal.Parse("0.00000000000010641"), testFunc, comparisonFunc, precision, matchDigits, sgn + "0.00000000000010641", callerName);
		ExecMethod(sign * BigDecimal.Parse("0.73908513321516064"), testFunc, comparisonFunc, precision, matchDigits, sgn + "0.73908513321516064", callerName);
		*/

		TestContext.WriteLine("");
		TestContext.WriteLine($"</{callerName}>");
	}

	[OneTimeSetUp]
	public void SetUp()
	{
		Precision = 50;
		BigDecimal.Precision = Precision;
		BigDecimal.AlwaysTruncate = false;
	}

	[Test]
	public static void Test_Arccos1()
	{
		Test_TrigFunction(BigDecimal.Arccos, Func, 14, Precision * 2, 1);

		return;

		static Double Func(Double x)
		{
			var input = Math.Abs(x) % 1;
			var output = Math.Atan(Math.Sqrt(1.0d - Math.Pow(input, 2)) / input);

			if (Math.Sign(x) == -1)
			{
				return Math.PI - output;
			}

			return output;
		}
	}

	[Test]
	public static void Test_ArccosNeg1()
	{
		Test_TrigFunction(BigDecimal.Arccos, Func, 14, Precision * 2, -1);

		return;

		static Double Func(Double x)
		{
			var input = Math.Abs(x) % 1;
			var output = Math.Atan(Math.Sqrt(1.0d - Math.Pow(input, 2)) / input);

			if (Math.Sign(x) == -1)
			{
				return Math.PI - output;
			}

			return output;
		}
	}

	[Test]
	public static void Test_Arccot()
	{
		Test_TrigFunction(BigDecimal.Arccot, Arccot, 14, Precision, 1);
		Test_TrigFunction(BigDecimal.Arccot, Arccot, 14, Precision, -1);

		return;

		static Double Arccot(Double x) => (Math.PI / 2) - Math.Atan(x);
	}

	[Test]
	public static void Test_Arccsc()
	{
		Test_TrigFunction(BigDecimal.Arccsc, Arccsc, 14, Precision, 1);
		Test_TrigFunction(BigDecimal.Arccsc, Arccsc, 14, Precision, -1);

		return;

		static Double Arccsc(Double x) => Math.Asin(1 / x);
	}

	[Test]
	public static void Test_Arcsin()
	{
		Test_TrigFunction(BigDecimal.Arcsin, Asin, 14, Precision * 2, 1);
		Test_TrigFunction(BigDecimal.Arcsin, Asin, 14, Precision * 2, -1);

		return;

		static Double Asin(Double x)
		{
			var input = Math.Abs(x) % 1;

			return Math.Sign(x) * Math.Atan(input / Math.Sqrt(1.0d - Math.Pow(input, 2)));
		}
	}

	[Test]
	public static void Test_Arctan()
	{
		Test_TrigFunction(BigDecimal.Arctan, Math.Atan, 14, Precision, 1);
		Test_TrigFunction(BigDecimal.Arctan, Math.Atan, 14, Precision, -1);
	}

	[Test]
	public static void Test_Cos()
	{
		Test_TrigFunction(BigDecimal.Cos, Math.Cos, 14, Precision, 1);
		Test_TrigFunction(BigDecimal.Cos, Math.Cos, 14, Precision, -1);
	}

	[Test]
	public static void Test_Cosh()
	{
		Test_TrigFunction(BigDecimal.Cosh, Math.Cosh, 11, Precision, 1);
		Test_TrigFunction(BigDecimal.Cosh, Math.Cosh, 11, Precision, -1);
	}

	[Test]
	public static void Test_Cot()
	{
		Test_TrigFunction(BigDecimal.Cot, Cot, 9, Precision, 1);
		Test_TrigFunction(BigDecimal.Cot, Cot, 9, Precision, -1);

		return;

		// cot = cos / sin
		static Double Cot(Double x) => Math.Cos(x) / Math.Sin(x);
	}

	[Test]
	public static void Test_Coth()
	{
		Test_TrigFunction(BigDecimal.Coth, Coth, 12, Precision, 1);
		Test_TrigFunction(BigDecimal.Coth, Coth, 12, Precision, -1);

		return;

		// coth = cosh / sinh
		static Double Coth(Double x) => Math.Cosh(x) / Math.Sinh(x);
	}

	[Test]
	public static void Test_Csc()
	{
		Test_TrigFunction(BigDecimal.Csc, Csc, 9, Precision, 1);
		Test_TrigFunction(BigDecimal.Csc, Csc, 9, Precision, -1);

		return;

		// csc = 1 / sin
		static Double Csc(Double x) => 1.0d / Math.Sin(x);
	}

	[Test]
	public static void Test_Csch()
	{
		Test_TrigFunction(BigDecimal.Csch, Csch, 11, Precision, 1);
		Test_TrigFunction(BigDecimal.Csch, Csch, 11, Precision, -1);

		return;

		// csch = 1 / sinh
		static Double Csch(Double x) => 1.0d / Math.Sinh(x);
	}

	[Test]
	public static void Test_Exp()
	{
		Test_TrigFunction(BigDecimal.Exp, Math.Exp, 11, Precision, 1);
		Test_TrigFunction(BigDecimal.Exp, Math.Exp, 11, Precision, -1);
	}

	[Test]
	public static void Test_Ln()
	{
		var argument = BigDecimal.Parse("65536");
		var format = Thread.CurrentThread.CurrentCulture.NumberFormat;
		var expected = TestBigDecimalHelper.PrepareValue("11.09035488895912495067571394333082508920800214976", format);

		var result = BigDecimal.Ln(argument, Precision);

		var actual = new String(result.ToString().Take(Precision).ToArray());
		Assert.AreEqual(expected, actual, nameof(BigDecimal.Ln));
	}

	[Test]
	public static void Test_Log10()
	{
		var argument = BigDecimal.Parse("2");

		var format = Thread.CurrentThread.CurrentCulture.NumberFormat;
		var expected = TestBigDecimalHelper.PrepareValue("0.301029995663981195213738894724493026768189881462", format);

		var result = BigDecimal.Log10(argument, Precision);

		var actual = new String(result.ToString().Take(Precision).ToArray());
		Assert.AreEqual(expected, actual, nameof(BigDecimal.Log10));
	}

	[Test]
	public static void Test_Log2()
	{
		var format = Thread.CurrentThread.CurrentCulture.NumberFormat;
		var val = TestBigDecimalHelper.PrepareValue("46340.95002", format);

		var argument = BigDecimal.Parse(val);
		var expected = TestBigDecimalHelper.PrepareValue("15.50000000025398948770312730360932446932123539424", format);

		var result = BigDecimal.Log2(argument, Precision);

		var actual = new String(result.ToString().Take(Precision).ToArray());
		Assert.AreEqual(expected, actual, nameof(BigDecimal.Log2));
	}

	[Test]
	public static void Test_LogN()
	{
		var @base = 3;
		var argument = BigDecimal.Parse("65536");

		var format = Thread.CurrentThread.CurrentCulture.NumberFormat;
		var expected = TestBigDecimalHelper.PrepareValue("10.09487605714331899359243382948417366879337024211", format);

		var result = BigDecimal.LogN(@base, argument, Precision);

		var actual = new String(result.ToString().Take(Precision).ToArray());
		Assert.AreEqual(expected, actual, nameof(BigDecimal.LogN));
	}

	[Test]
	public static void Test_Sec()
	{
		Test_TrigFunction(BigDecimal.Sec, Sec, 9, Precision, 1);
		Test_TrigFunction(BigDecimal.Sec, Sec, 9, Precision, -1);

		return;

		// sec = 1 / cos
		static Double Sec(Double x) => 1.0d / Math.Cos(x);
	}

	[Test]
	public static void Test_Sech()
	{
		Test_TrigFunction(BigDecimal.Sech, Sech, 14, Precision, 1);
		Test_TrigFunction(BigDecimal.Sech, Sech, 14, Precision, -1);

		return;

		// sech = 1 / cosh
		static Double Sech(Double x) => 1.0d / Math.Cosh(x);
	}

	[Test]
	public static void Test_Sin()
	{
		Test_TrigFunction(BigDecimal.Sin, Math.Sin, 14, Precision, 1);
		Test_TrigFunction(BigDecimal.Sin, Math.Sin, 14, Precision, -1);
	}

	[Test]
	public static void Test_Sinh()
	{
		Test_TrigFunction(BigDecimal.Sinh, Math.Sinh, 11, Precision, 1);
		Test_TrigFunction(BigDecimal.Sinh, Math.Sinh, 11, Precision, -1);
	}

	[Test]
	public static void Test_Tan()
	{
		Test_TrigFunction(BigDecimal.Tan, Math.Tan, 9, Precision, 1);
		Test_TrigFunction(BigDecimal.Tan, Math.Tan, 9, Precision, -1);
	}

	[Test]
	public static void Test_Tanh()
	{
		Test_TrigFunction(BigDecimal.Tanh, Math.Tanh, 14, Precision, 1);
		Test_TrigFunction(BigDecimal.Tanh, Math.Tanh, 14, Precision, -1);
	}
}