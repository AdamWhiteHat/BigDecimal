﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Threading;
using ExtendedNumerics;
using ExtendedNumerics.Helpers;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace TestBigDecimal
{

	[TestFixture]
	[Culture("en-US,ru-RU")]
	public class TestBigDecimalFunctions
	{
		private NumberFormatInfo Format { get { return Thread.CurrentThread.CurrentCulture.NumberFormat; } }

		[Test]
		public void TestGCD()
		{
			var expected = BigDecimal.Parse("10");

			BigDecimal result = new BigInteger[] {
			20, 30, 210, 310, 360, 5040, 720720
		}.GCD();

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void TestMin()
		{
			string expected = "30303";

			var left = BigDecimal.Parse(expected);
			var right1 = BigDecimal.Parse(expected);

			var val = TestBigDecimalHelper.PrepareValue("30303.5", this.Format);
			var right2 = BigDecimal.Parse(val);
			var right3 = BigDecimal.Parse("30304");

			var actual1 = BigDecimal.Min(left, right1);
			var actual2 = BigDecimal.Min(left, right2);
			var actual3 = BigDecimal.Min(left, right3);

			Assert.AreEqual(expected, actual1.ToString());
			Assert.AreEqual(expected, actual2.ToString());
			Assert.AreEqual(expected, actual3.ToString());
		}

		[Test]
		public void TestMax()
		{
			var expected = TestBigDecimalHelper.PrepareValue("30304.1", this.Format);

			var left1 = BigDecimal.Parse("30304");

			var val2 = TestBigDecimalHelper.PrepareValue("-30304.2", this.Format);
			var left2 = BigDecimal.Parse(val2);

			var val3 = TestBigDecimalHelper.PrepareValue("30304.01", this.Format);
			var left3 = BigDecimal.Parse(val3);
			var right = BigDecimal.Parse(expected);

			var actual1 = BigDecimal.Max(left1, right);
			var actual2 = BigDecimal.Max(left2, right);
			var actual3 = BigDecimal.Max(left3, right);

			Assert.AreEqual(expected, actual1.ToString());
			Assert.AreEqual(expected, actual2.ToString());
			Assert.AreEqual(expected, actual3.ToString());
		}

		[Test]
		public void TestGetLength()
		{
			var expected = BigDecimal.Parse("2268507702394854741827137539360680923314");
			var value = new BigDecimal(BigInteger.Parse("22685077023948547418271375393606809233149150201282920942551781108927727789384397020382853"), -49);

			Assert.AreEqual(expected.Length, value.Length);
		}

		[Test]
		public void TestGetSign()
		{
			BigDecimal zero1 = 0;
			Assert.AreEqual(0, zero1.Sign, "0");

			var zero2 = BigDecimal.Zero;
			Assert.AreEqual(0, zero2.Sign, "new BigDecimal()");

			var zero3 = new BigDecimal(0);
			Assert.AreEqual(0, zero3.Sign, "new BigDecimal(0);");

			var zero4 = new BigDecimal(BigInteger.Zero);
			Assert.AreEqual(0, zero4.Sign, "new BigDecimal(BigInteger.Zero)");

			var zero5 = new BigDecimal(0, -1);
			Assert.AreEqual(0, zero5.Sign, "new BigDecimal(0, -1);");

			BigDecimal zero6 = BigInteger.Subtract(BigInteger.Add(BigInteger.Divide(2, 3), BigInteger.Multiply(-1, BigInteger.Divide(1, 3))), BigInteger.Divide(1, 3));
			Assert.AreEqual(0, zero6.Sign, "2/3  -1/3 - 1/3");

			var oneTenth = BigDecimal.Divide(BigDecimal.One, new BigDecimal(10));
			BigDecimal pointZeroOne = 0.1m;
			var zero7 = BigDecimal.Subtract(oneTenth, pointZeroOne);
			Assert.AreEqual(0, zero7.Sign, "1/10 - 1/10");

			var zero8 = BigDecimal.Add(new BigDecimal(1, -1), -1m / 10m);
			Assert.AreEqual(0, zero8.Sign, "1 + -1/10");

			var zero9 = new BigDecimal(15274, -7) * 0;
			Assert.AreEqual(0, zero9.Sign, "0.0015274");

			BigDecimal positive1 = 1;
			Assert.AreEqual(1, positive1.Sign, "1");

			BigDecimal positive2 = -1 * -1;
			Assert.AreEqual(1, positive2.Sign, "-1 * 1");

			var negative1 = BigDecimal.Multiply(BigDecimal.One, BigDecimal.MinusOne);
			Assert.AreEqual(BigInteger.MinusOne.Sign, negative1.Sign, "1 * -1");

			var negative2 = BigDecimal.Subtract(BigDecimal.Zero, 3);
			Assert.AreEqual(BigInteger.MinusOne.Sign, negative2.Sign, "0 - 3");

			BigDecimal negative3 = BigInteger.Subtract(0, 3);
			Assert.AreEqual(BigInteger.MinusOne.Sign, negative3.Sign, "BigInteger.Subtract(0, 3)");

			BigDecimal negative4 = 10 * -1;
			Assert.AreEqual(BigInteger.MinusOne.Sign, negative4.Sign, "10 * -1;");

			string largeNegativeString = "-22685077023948547418271375393606809233149150201282920942551781108927727789384397020382853";
			BigDecimal largeNegative = BigDecimal.Parse(largeNegativeString);
			Assert.AreEqual(BigInteger.MinusOne.Sign, largeNegative.Sign, largeNegativeString);

			string highPrecisionNegativeString = TestBigDecimalHelper.PrepareValue("-2.2685077023948547418271375393606809233149150201282920942551781108927727789384397020382853", this.Format);
			BigDecimal highPrecisionNegative = BigDecimal.Parse(highPrecisionNegativeString);
			Assert.AreEqual(BigInteger.MinusOne.Sign, highPrecisionNegative.Sign, highPrecisionNegativeString);

			string smallNegativeString = TestBigDecimalHelper.PrepareValue("-0.000000000000000000000000000022680000000000000000000000000000150201282920942551781108927727789384397020382853", this.Format);
			BigDecimal smallNegative = BigDecimal.Parse(smallNegativeString);
			Assert.AreEqual(BigInteger.MinusOne.Sign, smallNegative.Sign, smallNegativeString);
		}

		[Test]
		public void TestGoldenIrrational()
		{
			var val = TestBigDecimalHelper.PrepareValue("1.6180339887498948482045868343656381177203091798057628621354486227052604628189024497072072041893911374847540880753868917521266338622235369317931800607667263544333890865959395829056383226613199282902678806752087668925017116962070322210432162695486262963136144381497587012203408058879544547492461856953648644492", this.Format);
			var goldenRatio = BigDecimal.Parse(val);

			TestContext.Write("GoldenRatio: ");
			TestContext.WriteLine(goldenRatio.ToString());
		}

		[Test]
		public void TestLeastCommonDivisor()
		{
			var expected = "45319990731015";

			BigDecimal actual = BigIntegerHelper.LCM(new BigInteger[] {
			3, 5, 7, 11, 13, 101, 307, 311, 313
		});

			// 15015, lcm(3, 5, 7, 11, 13, 101, 307, 311, 313) = 45319990731015 lcm(4973, 4292, 4978, 4968, 4297, 4287) = 2822891742340306560

			Assert.AreEqual(expected, actual.ToString());
		}

		[Test]
		public void TestRounding001()
		{
			var up = new BigDecimal(0.50001d);
			var down = new BigDecimal(0.49m);

			var val = TestBigDecimalHelper.PrepareValue("1.5", this.Format);
			var oneAndAhalf = BigDecimal.Parse(val);

			var negEightPointFive = new BigDecimal(-8.5d);
			BigDecimal negNinePointFive = -9.5d;

			var threePointFourNine = new BigDecimal(3.49d);
			var threePointFiveOne = new BigDecimal(3.51m);
			var sixPointFive = new BigDecimal(6.5);

			var zero = BigDecimal.Round(down);
			Assert.AreEqual(BigInteger.Zero, zero);

			var one = BigDecimal.Round(up);
			Assert.AreEqual(BigInteger.One, one);

			var two = BigDecimal.Round(oneAndAhalf);
			Assert.AreEqual(one + one, two);

			var three = BigDecimal.Round(threePointFourNine);
			Assert.AreEqual(one + two, three);

			var four = BigDecimal.Round(threePointFiveOne);
			Assert.AreEqual(one + three, four);
			Assert.AreEqual(two + two, four);

			var six = BigDecimal.Round(sixPointFive, MidpointRounding.ToEven);
			Assert.AreEqual(two + four, six);

			var negEight = BigDecimal.Round(negEightPointFive, MidpointRounding.ToEven);
			Assert.AreEqual(-four + -four, negEight);

			var negNine = BigDecimal.Round(negEightPointFive, MidpointRounding.AwayFromZero);
			Assert.AreEqual(-four + -four - one, negNine);

			var negTen = BigDecimal.Round(negNinePointFive, MidpointRounding.ToEven);
			Assert.AreEqual(-four + -four - two, negTen);
		}

		[Test]
		public void TestRounding002()
		{
			BigDecimal threeNine = new BigDecimal(3.9999);
			TestContext.WriteLine($"{threeNine}");
			TestContext.WriteLine("---------");
			BigDecimal result1 = BigDecimal.Round(threeNine, 0);
			BigDecimal result2 = BigDecimal.Round(threeNine, 1);

			TestContext.WriteLine($"{result1}");
			TestContext.WriteLine($"{result2}");

			string expected = "4";
			string actual1 = result1.ToString();
			string actual2 = result2.ToString();

			Assert.AreEqual(expected, actual1, $"{expected} != {actual1}");
			Assert.AreEqual(expected, actual2, $"{expected} != {actual2}");
		}

		[Test]
		public void TestTruncate_OverloadWithPrecisionArgument_001()
		{
			BigDecimal a = BigDecimal.Parse("31415.92654");

			var result6 = BigDecimal.Truncate(a, 6).ToString();
			var result5 = BigDecimal.Truncate(a, 5).ToString();
			var result4 = BigDecimal.Truncate(a, 4).ToString();
			var result3 = BigDecimal.Truncate(a, 3).ToString();
			var result2 = BigDecimal.Truncate(a, 2).ToString();
			var result1 = BigDecimal.Truncate(a, 1).ToString();
			var result0 = BigDecimal.Truncate(a, 0).ToString();
			var result_1 = BigDecimal.Truncate(a, -1).ToString();
			var result_2 = BigDecimal.Truncate(a, -2).ToString();
			var result_3 = BigDecimal.Truncate(a, -3).ToString();
			var result_4 = BigDecimal.Truncate(a, -4).ToString();
			var result_5 = BigDecimal.Truncate(a, -5).ToString();
			var result_6 = BigDecimal.Truncate(a, -6).ToString();
			var result_7 = BigDecimal.Truncate(a, -7).ToString();

			TestContext.WriteLine($"{result6}");
			TestContext.WriteLine($"{result5}");
			TestContext.WriteLine($"{result4}");
			TestContext.WriteLine($"{result3}");
			TestContext.WriteLine($"{result2}");
			TestContext.WriteLine($"{result1}");
			TestContext.WriteLine($"{result0}");
			TestContext.WriteLine($"{result_1}");
			TestContext.WriteLine($"{result_2}");
			TestContext.WriteLine($"{result_3}");
			TestContext.WriteLine($"{result_4}");
			TestContext.WriteLine($"{result_5}");
			TestContext.WriteLine($"{result_6}");
			TestContext.WriteLine($"{result_7}");

			Assert.AreEqual("31415.92654", result6);
			Assert.AreEqual("31415.92654", result5);
			Assert.AreEqual("31415.9265", result4);
			Assert.AreEqual("31415.926", result3);
			Assert.AreEqual("31415.92", result2);
			Assert.AreEqual("31415.9", result1);
			Assert.AreEqual("31415", result0);
			Assert.AreEqual("31410", result_1);
			Assert.AreEqual("31400", result_2);
			Assert.AreEqual("31000", result_3);
			Assert.AreEqual("30000", result_4);
			Assert.AreEqual("0", result_5);
			Assert.AreEqual("0", result_6);
			Assert.AreEqual("0", result_7);
		}

		[Test]
		public void TestRoundingWithPrecision()
		{
			BigDecimal.AlwaysNormalize = true;
			BigDecimal.AlwaysTruncate = false;

			//Test(7.54m, 1, RoundingStrategy.AwayFromZero, "7.5");
			Test(7.50m, 1, RoundingStrategy.AwayFromZero, "7.6");
			Test(7.55m, 1, RoundingStrategy.AwayFromZero, "7.6");

			Test(-7.54m, 1, RoundingStrategy.AwayFromZero, "-7.5");
			Test(-7.56m, 1, RoundingStrategy.AwayFromZero, "-7.6");
			Test(-7.55m, 1, RoundingStrategy.AwayFromZero, "-7.6");

			Test(7.54m, 1, RoundingStrategy.TowardZero, "7.5");
			Test(7.56m, 1, RoundingStrategy.TowardZero, "7.6");
			Test(7.55m, 1, RoundingStrategy.TowardZero, "7.5");

			Test(-7.54m, 1, RoundingStrategy.TowardZero, "-7.5");
			Test(-7.56m, 1, RoundingStrategy.TowardZero, "-7.6");
			Test(-7.55m, 1, RoundingStrategy.TowardZero, "-7.5");

			Test(-7.551m, 2, RoundingStrategy.AwayFromZero, "-7.55");
			Test(-7.559m, 2, RoundingStrategy.AwayFromZero, "-7.56");
			Test(-7.555m, 2, RoundingStrategy.AwayFromZero, "-7.56");
		}

		private static void Test(decimal value, int precision, RoundingStrategy roundingStrategy, string expected)
		{
			BigDecimal actual = BigDecimal.Round(value, precision, roundingStrategy);

			Assert.AreEqual(expected, actual.ToString(), $"Value: {value}\tRounding: {Enum.GetName(typeof(RoundingStrategy), roundingStrategy)}\tExpected: {expected}\tActual: {actual}");
		}

		[Test]
		public void TestGetWholeValue()
		{
			var value1 = new BigDecimal(BigInteger.Parse("22685077023948547418271375393606809233149150201282920942551781108927727789384397020382853"), -49);
			BigDecimal result1 = value1.WholeValue;
			var value2 = new BigDecimal(BigInteger.Parse("-22685077023948547418271375393606809233149150201282920942551781108927727789384397020382853"), -49);
			BigDecimal result2 = value2.WholeValue;

			var value3 = new BigDecimal(3, -1);
			BigDecimal result3 = value3.WholeValue;
			var value4 = new BigDecimal(-3, -1);
			BigDecimal result4 = value4.WholeValue;

			var expected1 = "2268507702394854741827137539360680923314";
			var actual1 = result1.ToString();

			var expected2 = "-2268507702394854741827137539360680923314";
			var actual2 = result2.ToString();

			var expected3 = "0";
			var actual3 = result3.ToString();

			var expected4 = "0";
			var actual4 = result4.ToString();

			Assert.AreEqual(expected1, actual1);
			Assert.AreEqual(expected2, actual2);
			Assert.AreEqual(expected3, actual3);
			Assert.AreEqual(expected4, actual4);
		}

		[Test]
		public void TestGetFractionalPart()
		{
			BigDecimal parsed1 = new BigDecimal(BigInteger.Parse("31415926535"), -10);
			BigDecimal parsed2 = new BigDecimal(BigInteger.Parse("-31415926535"), -10);
			BigDecimal parsed3 = new BigDecimal(BigInteger.Parse("31415926535"), -15);
			BigDecimal parsed4 = new BigDecimal(BigInteger.Parse("-31415926535"), -15);
			BigDecimal parsed5 = new BigDecimal(3, 0);
			BigDecimal parsed6 = new BigDecimal(-3, 0);
			BigDecimal parsed7 = new BigDecimal(3, -1);
			BigDecimal parsed8 = new BigDecimal(-3, -1);

			var expected1 = TestBigDecimalHelper.PrepareValue("0.1415926535", this.Format);
			var expected2 = TestBigDecimalHelper.PrepareValue("0.1415926535", this.Format);
			var expected3 = TestBigDecimalHelper.PrepareValue("0.000031415926535", this.Format);
			var expected4 = TestBigDecimalHelper.PrepareValue("0.000031415926535", this.Format);
			var expected5 = "0";
			var expected6 = "0";
			var expected7 = TestBigDecimalHelper.PrepareValue("0.3", this.Format);
			var expected8 = TestBigDecimalHelper.PrepareValue("0.3", this.Format);

			var result1 = parsed1.GetFractionalPart();
			var result2 = parsed2.GetFractionalPart();
			var result3 = parsed3.GetFractionalPart();
			var result4 = parsed4.GetFractionalPart();
			var result5 = parsed5.GetFractionalPart();
			var result6 = parsed6.GetFractionalPart();
			var result7 = parsed7.GetFractionalPart();
			var result8 = parsed8.GetFractionalPart();

			var actual1 = result1.ToString();
			var actual2 = result2.ToString();
			var actual3 = result3.ToString();
			var actual4 = result4.ToString();
			var actual5 = result5.ToString();
			var actual6 = result6.ToString();
			var actual7 = result7.ToString();
			var actual8 = result8.ToString();

			Assert.AreEqual(expected1, actual1, $"GetFractionalPart - #1: {parsed1}");
			Assert.AreEqual(expected2, actual2, $"GetFractionalPart - #2: {parsed2}");
			Assert.AreEqual(expected3, actual3, $"GetFractionalPart - #3: {parsed3}");
			Assert.AreEqual(expected4, actual4, $"GetFractionalPart - #4: {parsed4}");
			Assert.AreEqual(expected5, actual5, $"GetFractionalPart - #5: {parsed5}");
			Assert.AreEqual(expected6, actual6, $"GetFractionalPart - #6: {parsed6}");
			Assert.AreEqual(expected7, actual7, $"GetFractionalPart - #7: {parsed7}");
			Assert.AreEqual(expected8, actual8, $"GetFractionalPart - #8: {parsed8}");
		}

		[Test]
		public void TestSignifigantDigits()
		{
			const Int32 expected1 = 19;
			var number1 = new BigDecimal(12345678901234567890, -10);
			var result1 = number1.SignificantDigits;
			Assert.AreEqual(expected1, result1);

			const Int32 expected2 = 9;
			var number2 = new BigDecimal(123456789, 1);
			var result2 = number2.SignificantDigits;
			Assert.AreEqual(expected2, result2);

			const Int32 expected3 = 19;
			var number3 = new BigDecimal(BigInteger.Parse("-12345678901234567890"), -10);
			var result3 = number3.SignificantDigits;
			Assert.AreEqual(expected3, result3);

			const Int32 expected4 = 9;
			var number4 = new BigDecimal(-123456789, 1);
			var result4 = number4.SignificantDigits;
			Assert.AreEqual(expected4, result4);
		}

		[Test(TestOf = typeof(BigDecimal))]
		public void TestPlacesRightOfDecimal()
		{
			MethodInfo placesRightOfDecimal = typeof(BigDecimal).GetMethod("PlacesRightOfDecimal", BindingFlags.NonPublic | BindingFlags.Static);

			BigDecimal parsed1 = new BigDecimal(BigInteger.Parse("314159265358979323846"), -20);
			BigDecimal parsed2 = new BigDecimal(BigInteger.Parse("-314159265358979323846"), -20);
			BigDecimal parsed3 = new BigDecimal(BigInteger.Parse("31415926535897932384600000"), -25);
			BigDecimal parsed4 = new BigDecimal(BigInteger.Parse("314159265358979323846"), -25);
			BigDecimal parsed5 = new BigDecimal(BigInteger.Parse("-314159265358979323846"), -25);
			BigDecimal parsed6 = new BigDecimal(BigInteger.Parse("31415926535897932384600000"), -30);
			BigDecimal parsed7 = new BigDecimal(3, 0);
			BigDecimal parsed8 = new BigDecimal(-3, 0);
			BigDecimal parsed9 = new BigDecimal(3, -1);
			BigDecimal parsed0 = new BigDecimal(-3, -1);
			BigDecimal parsed11 = new BigDecimal(BigInteger.Parse("10000000000000031400000"), -25);

			var expected1 = "20";
			var expected2 = "20";
			var expected3 = "20";
			var expected4 = "25";
			var expected5 = "25";
			var expected6 = "25";
			var expected7 = "0";
			var expected8 = "0";
			var expected9 = "1";
			var expected0 = "1";
			var expected11 = "20";

			var result1 = placesRightOfDecimal.Invoke(obj: null, new object[] { parsed1 });
			var result2 = placesRightOfDecimal.Invoke(obj: null, new object[] { parsed2 });
			var result3 = placesRightOfDecimal.Invoke(obj: null, new object[] { parsed3 });
			var result4 = placesRightOfDecimal.Invoke(obj: null, new object[] { parsed4 });
			var result5 = placesRightOfDecimal.Invoke(obj: null, new object[] { parsed5 });
			var result6 = placesRightOfDecimal.Invoke(obj: null, new object[] { parsed6 });
			var result7 = placesRightOfDecimal.Invoke(obj: null, new object[] { parsed7 });
			var result8 = placesRightOfDecimal.Invoke(obj: null, new object[] { parsed8 });
			var result9 = placesRightOfDecimal.Invoke(obj: null, new object[] { parsed9 });
			var result0 = placesRightOfDecimal.Invoke(obj: null, new object[] { parsed0 });
			var result11 = placesRightOfDecimal.Invoke(obj: null, new object[] { parsed11 });

			var actual1 = result1.ToString();
			var actual2 = result2.ToString();
			var actual3 = result3.ToString();
			var actual4 = result4.ToString();
			var actual5 = result5.ToString();
			var actual6 = result6.ToString();
			var actual7 = result7.ToString();
			var actual8 = result8.ToString();
			var actual9 = result9.ToString();
			var actual0 = result0.ToString();
			var actual11 = result11.ToString();

			Assert.AreEqual(expected1, actual1, $"PlacesRightOfDecimal - #1: {parsed1}");
			Assert.AreEqual(expected2, actual2, $"PlacesRightOfDecimal - #2: {parsed2}");
			Assert.AreEqual(expected3, actual3, $"PlacesRightOfDecimal - #3: {parsed3}");
			Assert.AreEqual(expected4, actual4, $"PlacesRightOfDecimal - #4: {parsed4}");
			Assert.AreEqual(expected5, actual5, $"PlacesRightOfDecimal - #5: {parsed5}");
			Assert.AreEqual(expected6, actual6, $"PlacesRightOfDecimal - #6: {parsed6}");
			Assert.AreEqual(expected7, actual7, $"PlacesRightOfDecimal - #7: {parsed7}");
			Assert.AreEqual(expected8, actual8, $"PlacesRightOfDecimal - #8: {parsed8}");
			Assert.AreEqual(expected9, actual9, $"PlacesRightOfDecimal - #9: {parsed9}");
			Assert.AreEqual(expected0, actual0, $"PlacesRightOfDecimal - #10: {parsed0}");
			Assert.AreEqual(expected11, actual11, $"PlacesRightOfDecimal - #11: {parsed11}");
		}

		[Test]
		public void TestApproximateE()
		{
			BigDecimal.Precision = 1000;
			//BigDecimal.AlwaysTruncate = true;

			int precision = 1000;
			// This value comes from a different application; precise calculator
			string expected = "2.718281828459045235360287471352662497757247093699959574966967627724076630353547594571382178525166427427466391932003059921817413596629043572900334295260595630738132328627943490763233829880753195251019011573834187930702154089149934884167509244761460668082264800168477411853742345442437107539077744992069551702761838606261331384583000752044933826560297606737113200709328709127443747047230696977209310141692836819025515108657463772111252389784425056953696770785449969967946864454905987931636889230098793127736178215424999229576351482208269895193668033182528869398496465105820939239829488793320362509443117301238197068416140397019837679320683282376464804295311802328782509819455815301756717361332069811250996181881593041690351598888519345807273866738589422879228499892086805825749279610484198444363463244968487560233624827041978623209002160990235304369941849146314093431738143640546253152096183690888707016768396424378140592714563549061303107208510383750510115747704171898610687396965521267154688957035035";

			Stopwatch timer = Stopwatch.StartNew();

			string actual = BigDecimal.Round(BigDecimal.ApproximateE(precision), precision).ToString();

			TimeSpan timeElapsed = timer.Elapsed;
			TestContext.WriteLine($"Time Elapsed (ms): {timeElapsed.TotalMilliseconds}");

			actual = actual.Substring(0, expected.Length);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestApproximatePi()
		{
			BigDecimal.Precision = 1000;
			//BigDecimal.AlwaysTruncate = true;

			int precision = 1000;
			// This value comes from a different application; precise calculator
			string expected = "3.141592653589793238462643383279502884197169399375105820974944592307816406286208998628034825342117067982148086513282306647093844609550582231725359408128481117450284102701938521105559644622948954930381964428810975665933446128475648233786783165271201909145648566923460348610454326648213393607260249141273724587006606315588174881520920962829254091715364367892590360011330530548820466521384146951941511609433057270365759591953092186117381932611793105118548074462379962749567351885752724891227938183011949129833673362440656643086021394946395224737190702179860943702770539217176293176752384674818467669405132000568127145263560827785771342757789609173637178721468440901224953430146549585371050792279689258923542019956112129021960864034418159813629774771309960518707211349999998372978049951059731732816096318595024459455346908302642522308253344685035261931188171010003137838752886587533208381420617177669147303598253490428755468731159562863882353787593751957781857780532171226806613001927876611195909216420199";

			Stopwatch timer = Stopwatch.StartNew();

			string actual = BigDecimal.Round(BigDecimal.ApproximatePi(precision), precision).ToString();

			TimeSpan timeElapsed = timer.Elapsed;
			TestContext.WriteLine($"Time Elapsed (ms): {timeElapsed.TotalMilliseconds}");

			actual = actual.Substring(0, expected.Length);

			Assert.AreEqual(expected, actual);
		}
	}
}
