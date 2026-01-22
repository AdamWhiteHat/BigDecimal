using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using ExtendedNumerics;
using ExtendedNumerics.Helpers;
using NUnit.Framework;
using NUnit.Framework.Internal;
using NUnit.Framework.Legacy;
using TestBigDecimal;

namespace BigDecimalTests;

[TestFixture]
[NonParallelizable]
public class TestBigDecimalTrigonometricFunctions
{
	private static readonly Dictionary<string, Func<BigDecimal, bool>> FunctionDomainTestDictionary = new()
	{
		{ "Arccsc", new Func<BigDecimal, bool>((radians) => { return (radians > -1 && radians < 1); })},
		{ "Arcsec", new Func<BigDecimal, bool>((radians) => { return (radians > -1 && radians < 1); })},
		{ "Arccos", new Func<BigDecimal, bool>((radians) => { return (radians < -1 || radians > BigDecimal.One); })},
		{ "Arcsin", new Func<BigDecimal, bool>((radians) => { return (radians <= -1 || radians >= BigDecimal.One); })},
		{ "Csch", new Func<BigDecimal, bool>((radians) => { return ( BigDecimal.Normalize(radians).IsZero()); })},
		{ "Coth", new Func<BigDecimal, bool>((radians) => { return ( BigDecimal.Normalize(radians).IsZero()); })}
	};

	private const int Precision = 50;

	[OneTimeSetUp]
	public void SetUp()
	{
		BigDecimal.Precision = 50;
		BigDecimal.AlwaysTruncate = false;
	}

	private static void ExecMethod(
		BigDecimal input,
		Func<BigDecimal, int, BigDecimal> testFunc,
		Func<double, double> comparisonFunc,
		int precision,
		int matchDigits,
		string testDescription,
		string? callerName = null)
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

		var doubleInput = (double)input;
		var doubleExpected = comparisonFunc(doubleInput);

		if (double.IsNaN(doubleExpected) || double.IsInfinity(doubleExpected))
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

		double.TryParse(truncatedActual, out var doubleActual);

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
				displayString = new string(displayString.Take(65).ToArray()) + $"... ({displayString.Length - 65} more digits)";
			}

			TestContext.WriteLine($"Expecting: {doubleExpected}");
			TestContext.WriteLine($"   Actual: {displayString}");
		}

		TestContext.WriteLine("");

		if (testPassed)
		{
			ClassicAssert.AreEqual(truncatedExpecting, truncatedActual, testInfo);
		}
		else
		{
			var delta = 0.0001;
			ClassicAssert.AreEqual(doubleExpected, doubleActual, delta, testInfo);
		}
	}

	protected static void Test_TrigFunction(
		Func<BigDecimal, int, BigDecimal> testFunc,
		Func<double, double> comparisonFunc,
		int matchDigits,
		int precision,
		int sign = 1,
		[CallerMemberName] string callerName = "")
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

		var callerNameBanner = " * ".PadRight(3 + leftPad) + string.Join(" * ", Enumerable.Repeat(callerName, repeatTimes)) +
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

	[Test]
	public static void Test_Arccos1()
	{
		Test_TrigFunction(BigDecimal.Arccos, Func, 14, Precision * 2, 1);

		return;

		static double Func(double x)
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

		static double Func(double x)
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

		static double Arccot(double x) => (Math.PI / 2) - Math.Atan(x);
	}

	[Test]
	public static void Test_Arccsc()
	{
		Test_TrigFunction(BigDecimal.Arccsc, Arccsc, 14, Precision, 1);
		Test_TrigFunction(BigDecimal.Arccsc, Arccsc, 14, Precision, -1);

		return;

		static double Arccsc(double x) => Math.Asin(1 / x);
	}

	[Test]
	public static void Test_Arcsin()
	{
		Test_TrigFunction(BigDecimal.Arcsin, Asin, 14, Precision * 2, 1);
		Test_TrigFunction(BigDecimal.Arcsin, Asin, 14, Precision * 2, -1);

		return;

		static double Asin(double x)
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
		static double Cot(double x) => Math.Cos(x) / Math.Sin(x);
	}

	[Test]
	public static void Test_Coth()
	{
		Test_TrigFunction(BigDecimal.Coth, Coth, 12, Precision, 1);
		Test_TrigFunction(BigDecimal.Coth, Coth, 12, Precision, -1);

		return;

		// coth = cosh / sinh
		static double Coth(double x) => Math.Cosh(x) / Math.Sinh(x);
	}

	[Test]
	public static void Test_Csc()
	{
		Test_TrigFunction(BigDecimal.Csc, Csc, 9, Precision, 1);
		Test_TrigFunction(BigDecimal.Csc, Csc, 9, Precision, -1);

		return;

		// csc = 1 / sin
		static double Csc(double x) => 1.0d / Math.Sin(x);
	}

	[Test]
	public static void Test_Csch()
	{
		Test_TrigFunction(BigDecimal.Csch, Csch, 11, Precision, 1);
		Test_TrigFunction(BigDecimal.Csch, Csch, 11, Precision, -1);

		return;

		// csch = 1 / sinh
		static double Csch(double x) => 1.0d / Math.Sinh(x);
	}

	[Test]
	public static void Test_Exp()
	{
		Test_TrigFunction(BigDecimal.Exp, Math.Exp, 11, Precision, 1);
		Test_TrigFunction(BigDecimal.Exp, Math.Exp, 11, Precision, -1);
	}

	[Test]
	public static void Test_LogNatural()
	{
		int precision = 50;
		BigDecimal.Precision = precision * 2;
		BigDecimal.AlwaysTruncate = false;
		BigDecimal.AlwaysNormalize = false;

		Tuple<string, string>[] questionAnswerValues =
		[
			new("0.000000001",   "-20.72326583694641115616192309215927786840991339765895678"),
			new("0.000777", "-7.16007020759662666323925507670903264742195605720039"),
			new("0.073155", "-2.61517480115201143841773779457374933266203002783292"),
			new("0.50",     "-0.69314718055994530941723212145817656807550013436025"),
			new("0.57731",  "-0.54937589504899085530907326478753939476837352807873"),
			new("0.65",     "-0.43078291609245425738173613457722217087133367822882"),
			new("0.66",     "-0.41551544396166582316156197302289684265750543113712"),
			new("0.975311", "-0.02499888448682096802712749612065005723649133082854"),
			new("1.01575",  "0.01562725588569907632425960516558987307170263378188"),
			new("1.22605",  "0.20379761971667207412745062140592521363644795989009"),
			new("1.32835",  "0.28393757054679798984200829263947239665011340940242"),
			new("1.33",     "0.28517894223366239707839726596230485167226101362344"),
			new("1.33165",  "0.28641877482725127078618464736645894088507708401364"),
			new("1.34",     "0.29266961396282000105132120845317090344023006032460"),
			new("1.499",    "0.40479821912046065192222057024840164526003750773195"),
			new("1.50",     "0.40546510810816438197801311546434913657199042346249"),
			new("1.533371", "0.42746857974261091761922170608850145379554792901141"),
			new("1.7997",   "0.58761998434502004983067406992099718856024990068669"),
			new("1.997755", "0.69202405008497071018459774938875984871561069648604"),
			new("2.57",     "0.94390589890712843031581140539252703641252185172939"),
			new("3.14159265358900100002000010000001", "1.14472988584914799681491911248160367461176901448316"),
			new("31.41592653589",                     "3.44731497884319336251665815374030592476402235854262"),
			new("3141.592653589",                     "8.05248516483128473055264106310903433996622533580016"),
			new("314159.265358900100002000010000001", "12.65765535081937641690487638590342471261727645762702"),
			new("31415926535.8900100002000010000001", "24.17058081578960483699483365932524575062278390077089"),
			new("1409368056606849087457015313568.21404846132236496737", "69.42069420694206942069420694206942069420694206942069")
		];

		LogNaturalTimeElapsed = TimeSpan.Zero;
		Console.WriteLine($"Beginning of test...");
		foreach (Tuple<string, string> qaValue in questionAnswerValues)
		{
			Test_Single(qaValue, precision);
		}
		Console.WriteLine($"End of test.");

		Console.WriteLine($"");
		Console.WriteLine($"LogNatural Total Elapsed Milliseconds: {LogNaturalTimeElapsed.TotalMilliseconds} ms");
		Console.WriteLine($"");
	}

	private static readonly Stopwatch LogNaturalTimer = Stopwatch.StartNew();
	private static TimeSpan LogNaturalTimeElapsed = TimeSpan.Zero;

	private static void Test_Single(Tuple<string, string> questionAnswerValue, int precision)
	{
		BigDecimal input = BigDecimal.Parse(questionAnswerValue.Item1);
		string expected = questionAnswerValue.Item2;

		LogNaturalTimer.Restart();
		BigDecimal result1 = BigDecimal.Ln(input, precision);
		LogNaturalTimeElapsed = LogNaturalTimeElapsed.Add(LogNaturalTimer.Elapsed);

		string actual = result1.ToString().Substring(0, expected.Length);

		int matchCount = CharactersMatchCount(expected, actual);

		Console.WriteLine($"-------- {nameof(BigDecimal)}.{nameof(BigDecimal.Ln)} --------");
		Console.WriteLine($"   Input: {input}");
		Console.WriteLine($"Expected: {expected}");
		Console.WriteLine($"  Actual: {actual}");
		Common.HightlightDiffControl(expected, actual, 10);
		Console.WriteLine($"Match Count: {matchCount}");
		Console.WriteLine($"");

		// 48 out of the 50 digits to the right of the decimal point must be correct.
		ClassicAssert.GreaterOrEqual(matchCount, 48, $"Expected/Actual:{Environment.NewLine}{expected}{Environment.NewLine}{actual}");
	}

	private static int CharactersMatchCount(string expected, string actual)
	{
		int index = 0;
		int counter = 0;
		bool seenDecimalPoint = false;
		foreach (char c in expected)
		{
			if (c != actual[index])
			{
				break;
			}

			if (seenDecimalPoint)
			{
				counter++;
			}
			else
			{
				if (c == '.')
				{
					seenDecimalPoint = true;
					counter = 0;
				}
			}

			index++;
		}
		return counter;
	}


	private static readonly Dictionary<char, string> BoldedNumeralDictionary = new()
	{
		{'-',"-"},
		{'.',"."},
		{'0',"𝟬"},
		{'1',"𝟭"},
		{'2',"𝟮"},
		{'3',"𝟯"},
		{'4',"𝟰"},
		{'5',"𝟱"},
		{'6',"𝟲"},
		{'7',"𝟳"},
		{'8',"𝟴"},
		{'9',"𝟵"}
	};

	private static string HightlightDiffControl(string expected, string actual)
	{
		int index = 0;
		int maxLength = Math.Min(actual.Length, expected.Length);

		bool matchSoFar = true;
		StringBuilder result = new();
		StringBuilder result2 = new();
		foreach (char c in actual)
		{
			char? cmp = (index < maxLength) ? expected[index] : null;
			if (matchSoFar && cmp.HasValue && c == cmp)
			{
				result.Append(BoldedNumeralDictionary[c]);
				result2.Append('▀');
			}
			else
			{
				matchSoFar = false;
				result.Append(' ');
			}

			index++;
		}
		return result.ToString() + Environment.NewLine + result2.ToString();
	}

	[Test]
	public static void Test_Ln()
	{
		var argument = BigDecimal.Parse("65536");
		var format = Thread.CurrentThread.CurrentCulture.NumberFormat;
		var expected = TestBigDecimalHelper.PrepareValue("11.09035488895912495067571394333082508920800214976", format);

		var result = BigDecimal.Ln(argument, Precision);

		var actual = new string(result.ToString().Take(Precision).ToArray());
		ClassicAssert.AreEqual(expected, actual, nameof(BigDecimal.Ln));
	}


	[Test]
	public static void Test_Ln_ChunkValue()
	{
		var argument = BigDecimal.Parse("13763555136");
		var format = Thread.CurrentThread.CurrentCulture.NumberFormat;

		var expected = TestBigDecimalHelper.PrepareValue("23.34529000352234960050576234615311770969963762614", format);

		var result = BigDecimal.Ln(argument, Precision);

		var actual = new string(result.ToString().Take(Precision).ToArray());
		ClassicAssert.AreEqual(expected, actual, nameof(BigDecimal.Ln));
	}

	[Test]
	public static void Test_Log10()
	{
		var argument = BigDecimal.Parse("2");

		var format = Thread.CurrentThread.CurrentCulture.NumberFormat;
		var expected = TestBigDecimalHelper.PrepareValue("0.301029995663981195213738894724493026768189881462", format);

		var result = BigDecimal.Log10(argument, Precision);

		var actual = new string(result.ToString().Take(Precision).ToArray());
		ClassicAssert.AreEqual(expected, actual, nameof(BigDecimal.Log10));
	}

	[Test]
	public static void Test_Log2()
	{
		var format = Thread.CurrentThread.CurrentCulture.NumberFormat;
		var val = TestBigDecimalHelper.PrepareValue("46340.95002", format);

		var argument = BigDecimal.Parse(val);
		var expected = TestBigDecimalHelper.PrepareValue("15.50000000025398948770312730360932446932123539424", format);

		var result = BigDecimal.Log2(argument, Precision);

		var actual = new string(result.ToString().Take(Precision).ToArray());
		ClassicAssert.AreEqual(expected, actual, nameof(BigDecimal.Log2));
	}

	[Test]
	public static void Test_LogN()
	{
		var @base = 3;
		var argument = BigDecimal.Parse("65536");

		var format = Thread.CurrentThread.CurrentCulture.NumberFormat;
		var expected = TestBigDecimalHelper.PrepareValue("10.09487605714331899359243382948417366879337024211", format);

		var result = BigDecimal.LogN(@base, argument, Precision);

		var actual = new string(result.ToString().Take(Precision).ToArray());
		ClassicAssert.AreEqual(expected, actual, nameof(BigDecimal.LogN));
	}

	[Test]
	public static void Test_Sec()
	{
		Test_TrigFunction(BigDecimal.Sec, Sec, 9, Precision, 1);
		Test_TrigFunction(BigDecimal.Sec, Sec, 9, Precision, -1);

		return;

		// sec = 1 / cos
		static double Sec(double x) => 1.0d / Math.Cos(x);
	}

	[Test]
	public static void Test_Sech()
	{
		Test_TrigFunction(BigDecimal.Sech, Sech, 14, Precision, 1);
		Test_TrigFunction(BigDecimal.Sech, Sech, 14, Precision, -1);

		return;

		// sech = 1 / cosh
		static double Sech(double x) => 1.0d / Math.Cosh(x);
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



	internal class ResultObject
	{
		public int Precision { get; set; }
		public int N_of_ExpN { get; set; }
		public override string ToString() => $"{Precision}, {N_of_ExpN}";
	}


	[Test]
	public static void Test_Exp_EdgeCases()
	{
		Stopwatch timer = Stopwatch.StartNew();

		BigDecimal exp10 = BigDecimal.Exp(10);

		TimeSpan timeElapsed_10 = timer.Elapsed;
		TestContext.WriteLine($"Exp(10) - Time Elapsed (ms): {timeElapsed_10.TotalMilliseconds}");

		timer.Reset();
		timer.Start();

		BigDecimal exp11 = BigDecimal.Exp(11);

		TimeSpan timeElapsed_11 = timer.Elapsed;
		TestContext.WriteLine($"Exp(11) - Time Elapsed (ms): {timeElapsed_11.TotalMilliseconds}");

		timer.Reset();
		timer.Start();

		BigDecimal exp12 = BigDecimal.Exp(12);

		TimeSpan timeElapsed_12 = timer.Elapsed;
		TestContext.WriteLine($"Exp(12) - Time Elapsed (ms): {timeElapsed_12.TotalMilliseconds}");

		TestContext.WriteLine();
		TestContext.WriteLine($"Expected Exp({10}): {Expected.ValuesOfExp[10]}");
		TestContext.WriteLine($"  Actual Exp({10}): {exp10}");
		Common.HightlightDiffControl(Expected.ValuesOfExp[10], exp10.ToString(), 20);

		TestContext.WriteLine();
		TestContext.WriteLine($"Expected Exp({11}): {Expected.ValuesOfExp[11]}");
		TestContext.WriteLine($"  Actual Exp({11}): {exp11}");
		Common.HightlightDiffControl(Expected.ValuesOfExp[11], exp11.ToString(), 20);

		TestContext.WriteLine();
		TestContext.WriteLine($"Expected Exp({12}): {Expected.ValuesOfExp[12]}");
		TestContext.WriteLine($"  Actual Exp({12}): {exp12}");
		Common.HightlightDiffControl(Expected.ValuesOfExp[12], exp12.ToString(), 20);
	}

	[Test]
	public static void Test_Exp_Large()
	{
		int precision = 355;

		BigDecimal valueToTest = 813; // 1973

		Stopwatch timer = Stopwatch.StartNew();

		BigDecimal actual = BigDecimal.Exp(valueToTest, precision);

		TimeSpan timeElapsed_exp = timer.Elapsed;
		TestContext.WriteLine($"Exp({valueToTest}) - Time Elapsed (ms): {timeElapsed_exp.TotalMilliseconds}");

		string expected = "120618462233518981433238080308535803757557687664846828848443535448779741815217057681027697506649569601439291868793154656577261476254713167816544696153161438135707491747718691667694545847458665277046673289298561918077097160570572162793147114386756467587949281832249428245023200573458995225450885521592507359241589853364093350639258320452351869351505285572.228772209";

		TestContext.WriteLine();
		TestContext.WriteLine($"Expected Exp({valueToTest}): {expected}");
		TestContext.WriteLine($"  Actual Exp({valueToTest}): {actual}");
		Common.HightlightDiffControl(expected, actual.ToString(), 20);

		ClassicAssert.AreEqual(expected, actual.ToString().Substring(0, 352));
	}

	[Test]
	public static void Test_Exp_Timing()
	{
		int precision = 200;

		BigDecimal OneTenth = new BigDecimal(11, -2);
		BigDecimal One = new BigDecimal(113, -2);
		BigDecimal Ten = new BigDecimal(111, -1);
		BigDecimal Twenty = new BigDecimal(214, -1);
		BigDecimal TwoHundred = new BigDecimal(2011, -1);

		string strOneTenth = $"{OneTenth}";
		string strOne = $"{One}";
		string strTen = $"{Ten}";
		string strTwenty = $"{Twenty}";
		string strTwoHundred = $"{TwoHundred}";

		Stopwatch timer = Stopwatch.StartNew();

		timer.Reset();
		timer.Start();

		BigDecimal.Exp(OneTenth, precision);

		TimeSpan timeElapsed_1_10 = timer.Elapsed;
		TestContext.WriteLine($"Exp({strOneTenth}) - Time Elapsed (ms): {timeElapsed_1_10.TotalMilliseconds}");
		TestContext.WriteLine();


		timer.Reset();
		timer.Start();

		BigDecimal.Exp(One, precision);

		TimeSpan timeElapsed_1 = timer.Elapsed;
		TestContext.WriteLine($"Exp({strOne}) - Time Elapsed (ms): {timeElapsed_1.TotalMilliseconds}");
		TestContext.WriteLine();

		timer.Reset();
		timer.Start();

		BigDecimal.Exp(Ten, precision);

		TimeSpan timeElapsed_10 = timer.Elapsed;
		TestContext.WriteLine($"Exp({strTen}) - Time Elapsed (ms): {timeElapsed_10.TotalMilliseconds}");
		TestContext.WriteLine();

		timer.Reset();
		timer.Start();

		BigDecimal.Exp(Twenty, precision);

		TimeSpan timeElapsed_20 = timer.Elapsed;
		TestContext.WriteLine($"Exp({strTwenty}) - Time Elapsed (ms): {timeElapsed_20.TotalMilliseconds}");
		TestContext.WriteLine();

		timer.Reset();
		timer.Start();

		BigDecimal.Exp(TwoHundred, precision);

		TimeSpan timeElapsed_200 = timer.Elapsed;
		TestContext.WriteLine($"Exp({strTwoHundred}) - Time Elapsed (ms): {timeElapsed_200.TotalMilliseconds}");
		TestContext.WriteLine();
	}

	public static void Test_Exp_1011(int precision, int minimumCorrectDigits)
	{
		BigDecimal valueToTest = 1011;
		string expected = "11795631706967431106257568260721165052418669208780627463056386271682463550358281615243401604609427915490736977871001536553653167580626736331516842400752123331182209012286512505603075898850283249210504933496598653773386608180010349926378964239687044699518479654589142697769694489316266803535363535669997608301616494716924943498510232177924309996046591344152647854591693443689927304679068837084128565236405425159892099980215918759545864400126.589108911252832854643467068425822441076055894309627977798308921397204006850026005868932906663258362001654798686230087439212347234353932817803561857750948087363466040372013875880017089820431467652456239183514905552905216567577472414761661275479518819815574615064577702742876563261954130633564844288154413735772184900268161683927069887941863946739210263149971011543245322251049639950750819714535497409650895327643718951562796414333875859164935571124632733571526580436080298067974076793724713503046953482413423252055159193987878999180903172603138472277097851121";

		Test_Exp_Variable(precision, minimumCorrectDigits, valueToTest, expected);
	}

	public static void Test_Exp_2011(int precision, int minimumCorrectDigits)
	{
		BigDecimal valueToTest = 2011;
		string expected = "2323823329748012862284724519461635711939560569765110356699112127030180484033973095063071499919965288199448948756287133798394499682902037462246237766454879628391773259648467719149972443214873411985429519464819962531782997822916278506973646261971785480143712304203162494968930428639752003764803942015565512940652276121044306831501917578696364968824880352708813969372371518658461046325562298095423146083470918524272244805026769336959711817570542440957951676629381518687976452083498994073690257881699796035450140231255790118784990526505141719814623220214032890748733685240673070408163489016448785761642985195061416769378997675967190710707236340059633603820453410225070331540278035278661756344793194276131411085364783963887102226233316721198151568554382794577241196740812007792353017548086760471957441181119527833301970007176054171439972748742171476694637291770547688322854967781.199815973820403248083653985489213684726396130770592743925176192357540212994182656884498750131746203274680174451505495399603449";

		Test_Exp_Variable(precision, minimumCorrectDigits, valueToTest, expected);
	}

	public static void Test_Exp_Variable(int precision, int minimumCorrectDigits, BigDecimal valueToTest, string expected)
	{
		int savePrecision = BigDecimal.Precision;

		BigDecimal.Precision = precision;
		int truncateAt = precision + 50;

		Stopwatch timer = Stopwatch.StartNew();

		BigDecimal actual = BigDecimal.Exp(valueToTest, precision);

		TimeSpan timeElapsed_exp = timer.Elapsed;
		double elapsedDisplay = (timeElapsed_exp.TotalMilliseconds) / 1000d;
		TestContext.WriteLine($"Exp({valueToTest}): Time_Elapsed: {elapsedDisplay} sec.");

		//TestContext.WriteLine();
		//TestContext.WriteLine($"Expected Exp({valueToTest}): {expected}");
		//TestContext.WriteLine($"  Actual Exp({valueToTest}): {actual.TruncateAt(truncateAt)}");
		Common.HightlightDiffControl(expected, actual.ToString(), 20);

		ClassicAssert.AreEqual(expected, actual.ToString().Substring(0, minimumCorrectDigits));

		BigDecimal.Precision = savePrecision;
	}

	public static void Test_ForRecursionThreshold(int max = 200)
	{
		int n = 11;

		while (n < max)
		{
			bool success = LoopInternalTest(n);

			if (!success)
			{
				break;
			}

			n++;
		}
	}

	public static bool LoopInternalTest(int n)
	{
		string expected = "";

		if (n > 1000)
		{
			expected = Expected.ValuesOfExp_Extended[n];
		}
		else
		{
			expected = Expected.ValuesOfExp[n];
		}

		int precision = expected.IndexOf('.') + 1;

		Stopwatch timer = Stopwatch.StartNew();
		BigDecimal actualValue = BigDecimal.Exp(n, precision);
		TimeSpan timeElapsed_exp = timer.Elapsed;
		double elapsedDisplay = (timeElapsed_exp.TotalMilliseconds) / 1000d;
		elapsedDisplay = Math.Round(elapsedDisplay, 2);

		string actual = actualValue.ToString();

		string _expected = expected.Substring(0, precision);
		string _actual = actual.Substring(0, precision);

		bool success = string.Equals(_expected, _actual);

		string message = "SUCCESS!";

		if (!success)
		{
			message = "fail";
		}

		TestContext.WriteLine();
		TestContext.WriteLine($"   {message}   ");
		TestContext.WriteLine($" Expected  Exp({n}): {_expected}");
		TestContext.WriteLine($"  Actual   Exp({n}): {_actual}");
		TestContext.WriteLine($" Precision         : {precision}");
		TestContext.WriteLine($"---  Elapsed: {elapsedDisplay} sec ---");

		return success;
	}

	private static int PrecisionTestRange_Start = 70;
	private static int PrecisionTestRange_End = 120;
	private static int PrecisionTestRange_StepSize = 10;

	private static int N_Start = 180;
	private static int N_Stop = 220;
	private static int N_StepSize = 5;

	private static int Tolerance = 3;
	private static int Strikes = 5;

	[Test]
	public static void Test_Exp_TaylorSeriesAccuracy()
	{
		List<ResultObject> results = new List<ResultObject>();

		int precision = PrecisionTestRange_Start;
		int maxN = -1;
		while (precision <= PrecisionTestRange_End)
		{
			maxN = Internal_Exp_TaylorSeriesAccuracy(precision);
			results.Add(new ResultObject() { Precision = precision, N_of_ExpN = maxN });
			precision += PrecisionTestRange_StepSize;
		}

		TestContext.WriteLine();
		TestContext.WriteLine($"<RESULTS>");
		TestContext.WriteLine();
		foreach (ResultObject result in results)
		{
			TestContext.WriteLine(result.ToString());
		}
		TestContext.WriteLine();
		TestContext.WriteLine($"</RESULTS>");
		TestContext.WriteLine();
	}

	internal static int Internal_Exp_TaylorSeriesAccuracy(int precision)
	{
		TestContext.WriteLine();
		TestContext.WriteLine($"<PRECISION  DIGITS=\"{precision}\" >");

		int outOfRangeCount = 0;
		int lastCorrectN = -1;

		int n = N_Start;
		while (n < N_Stop)
		{
			int correctDigits = Internal_Exp_N(precision, n);

			if (correctDigits < (precision - Tolerance))
			{
				outOfRangeCount++;
			}
			else
			{
				lastCorrectN = n;
			}

			if (outOfRangeCount >= Strikes)
			{
				break;
			}

			n += N_StepSize;
		}

		TestContext.WriteLine();
		TestContext.WriteLine($"</PRECISION> // {precision}");
		TestContext.WriteLine();
		TestContext.WriteLine();

		return lastCorrectN;
	}

	internal static int Internal_Exp_N(int precision, int N)
	{
		BigDecimal expN = BigDecimal.Exp(N, precision);

		string actualN = Expected.ValuesOfExp[N];

		TestContext.WriteLine();
		TestContext.WriteLine($"BigDecimal.Exp({N}): {expN}");
		TestContext.WriteLine($"  Actual   Exp({N}): {actualN}");
		int correctDigits = Common.HightlightDiffControl(actualN, expN.ToString(), 20);

		return correctDigits;
	}

	[Test]
	public static void Test_Exp_200()
	{
		BigDecimal.Precision = 100;

		int twoHundred = 200;
		double dExp = (double)BigDecimal.Exp(twoHundred);
		double dPowE = (double)BigDecimal.Pow(Math.E, twoHundred);
		double dPowEB = (double)BigDecimal.Pow(BigDecimal.E, twoHundred);
		double mathPow = Math.Pow(Math.E, twoHundred);
		double mathExp = Math.Exp(twoHundred);

		BigDecimal bd_200 = new BigDecimal(mantissa: 200, exponent: 0);

		BigDecimal bdExpd = BigDecimal.Exp(twoHundred);
		BigDecimal bdPowEd = BigDecimal.Pow(Math.E, twoHundred);
		BigDecimal bdPowEBd = BigDecimal.Pow(BigDecimal.E, twoHundred);


		Stopwatch timer = Stopwatch.StartNew();

		BigDecimal bdExpbd = BigDecimal.Exp(bd_200);

		TimeSpan timeElapsed_exp = timer.Elapsed;
		TestContext.WriteLine($"Exp(200) - Time Elapsed (ms): {timeElapsed_exp.TotalMilliseconds}");

		string expected = "722597376812574925817747704218930569735687442852731928403269789123221909361473891661561.9265890";

		TestContext.WriteLine($"(double) BigDecimal.Exp(200):                   {dExp}");
		TestContext.WriteLine($"(double) BigDecimal.Pow(Math.E, 200):           {dPowE}");
		TestContext.WriteLine($"(double) BigDecimal.Pow(BigDecimal.E, 200):     {dPowEB}");
		TestContext.WriteLine($"(double) Math.Pow(Math.E, 200):                 {mathPow}");
		TestContext.WriteLine($"(double) Math.Exp(200):                         {mathExp}");
		TestContext.WriteLine();
		TestContext.WriteLine($"(BigDecimal) BigDecimal.Exp((double)200):               {bdExpd}");
		TestContext.WriteLine($"(BigDecimal) BigDecimal.Pow(Math.E, (double)200):       {bdPowEd}");
		TestContext.WriteLine($"(BigDecimal) BigDecimal.Pow(BigDecimal.E, (double)200): {bdPowEBd}");
		TestContext.WriteLine();
		TestContext.WriteLine($"(BigDecimal) BigDecimal.Exp((BigDecimal)200):                   {bdExpbd}");
		TestContext.WriteLine();
		TestContext.WriteLine($"Actual:                                         722597376812574925817747704218930569735687442852731928403269789123221909361473891661561.926589062570557468402043101429418177110677119368226480983077273278800...");
		TestContext.WriteLine();
		TestContext.WriteLine($"BigDecimal(200):");
		TestContext.WriteLine($"ToString: {bd_200}");
		TestContext.WriteLine($"Mantissa: {bd_200.Mantissa}");
		TestContext.WriteLine($"Exponent: {bd_200.Exponent}");
		TestContext.WriteLine();

		ClassicAssert.AreEqual(expected, bdExpbd.ToString().Substring(0, expected.Length));
	}

	[Test]
	public void TestExponentiation1()
	{
		double exp = 0.052631578947368421d;
		double phi = (1.0d + Math.Sqrt(5)) / 2.0d;

		BigDecimal result1 = BigDecimal.Pow(9.0d, 0.5d);
		BigDecimal result2 = BigDecimal.Pow(16.0d, 0.25d);
		string expected1 = "3";
		string expected2 = "2";

		BigDecimal result3 = BigDecimal.Pow(phi, 13);
		string expected3 = "521.001919378725";

		BigDecimal result4 = BigDecimal.Pow(1162261467, exp);
		BigDecimal result5 = BigDecimal.Pow(9349, exp);
		string expected4 = "3";
		string expected5 = "1.61803398777557";

		BigDecimal result6 = BigDecimal.Pow(1.618034d, 16.000000256d);
		BigDecimal result7 = BigDecimal.Pow(phi, 20.0000000128d);
		string expected6 = "2207.00006429941";
		string expected7 = "15127.0000270679";

		BigDecimal result8 = BigDecimal.Pow(29192926025390625d, 0.07142857142857142d);
		string expected8 = "14.999999999999998";

		ClassicAssert.AreEqual(expected1, result1.ToString());
		ClassicAssert.AreEqual(expected2, result2.ToString());
		ClassicAssert.AreEqual(expected3, result3.ToString().Substring(0, 16));
		ClassicAssert.AreEqual(expected4, result4.ToString());
		ClassicAssert.AreEqual(expected5, result5.ToString().Substring(0, 16));
		ClassicAssert.AreEqual(expected6, result6.ToString().Substring(0, 16));
		ClassicAssert.AreEqual(expected7, result7.ToString().Substring(0, 16));
		ClassicAssert.AreEqual(expected8, result8.ToString());
	}

	[Test]
	public void TestExponentiation2()
	{
		BigDecimal result1 = BigDecimal.Pow(new BigDecimal(16), new BigInteger(16));
		BigDecimal result2 = BigDecimal.Pow(new BigDecimal(101), new BigInteger(13));
		string expected1 = "18446744073709551616";
		string expected2 = "113809328043328941786781301";

		BigDecimal result3 = BigDecimal.Pow(new BigDecimal(0.25), 2);
		string expected3 = "0.0625";

		ClassicAssert.AreEqual(expected1, result1.ToString());
		ClassicAssert.AreEqual(expected2, result2.ToString());
		ClassicAssert.AreEqual(expected3, result3.ToString());
	}
}