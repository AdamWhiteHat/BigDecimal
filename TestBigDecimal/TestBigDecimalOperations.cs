﻿using System;
using System.Globalization;
using System.Numerics;
using System.Threading;
using ExtendedNumerics;
using ExtendedNumerics.Helpers;
using NUnit.Framework;

namespace TestBigDecimal
{

	[TestFixture]
	[Culture("en-US,ru-RU")]
	public class TestBigDecimalOperations
	{
		private NumberFormatInfo Format { get { return Thread.CurrentThread.CurrentCulture.NumberFormat; } }

		[Test]
		public void TestAddition001()
		{
			var number1 = BigDecimal.Parse("1234567890");
			var expected = BigDecimal.Parse("3382051537");

			var actual = number1 + 0x7FFFFFFF;

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestAddition002()
		{
			var A = new BigDecimal(new BigInteger(1234567), -1);
			var B = new BigDecimal(new BigInteger(9876543), -9);

			var actual = BigDecimal.Add(A, B);
			var expected = TestBigDecimalHelper.PrepareValue("123456.709876543", this.Format);

			Assert.AreEqual(expected, actual.ToString());
		}

		[Test]
		public void TestBigDecimalPow()
		{
			string expected = "268637376395543538746286686601216000000000000";
			// 5040 ^ 12 = 268637376395543538746286686601216000000000000

			var number = BigDecimal.Parse("5040");
			var exp12 = BigDecimal.Pow(number, 12);
			string actual = exp12.ToString();

			Assert.AreEqual(expected, actual, "5040 ^ 12  =  268637376395543538746286686601216000000000000");
		}

		[Test]
		public void TestCeiling001()
		{
			string expected = "4";

			var actual = BigDecimal.Ceiling(BigDecimal.Pi);

			Assert.AreEqual(expected, actual.ToString());
		}

		[Test]
		public void TestCeiling002()
		{
			string expected = "-3";
			var ceiling = BigDecimal.Ceiling(BigDecimal.Pi);

			Assert.AreNotEqual(expected, ceiling.ToString());
		}

		[Test]
		public void TestCeiling003()
		{
			var val = TestBigDecimalHelper.PrepareValue("0.14159265", this.Format);
			var start = BigDecimal.Parse(val);
			var ceiling = BigDecimal.Ceiling(start);

			Assert.AreEqual(BigDecimal.One, ceiling);
		}

		[Test]
		public void TestCeiling004()
		{
			var val = TestBigDecimalHelper.PrepareValue("-0.14159265", this.Format);
			var start = BigDecimal.Parse(val);
			var ceiling = BigDecimal.Ceiling(start);

			Assert.AreEqual(BigDecimal.Zero, ceiling);
		}

		[Test]
		public void TestDivide000()
		{
			var val1 = TestBigDecimalHelper.PrepareValue("0.63", this.Format);
			var val2 = TestBigDecimalHelper.PrepareValue("0.09", this.Format);

			var dividend = BigDecimal.Parse(val1);
			var divisor = BigDecimal.Parse(val2);

			var actual = BigDecimal.Divide(dividend, divisor);
			string expected = "7";

			Assert.AreEqual(expected, actual.ToString());
		}

		[Test]
		public void TestDivide001()
		{
			var expected = "40094690950920881030683735292761468389214899724061";

			var dividend = BigDecimal.Parse("1522605027922533360535618378132637429718068114961380688657908494580122963258952897654000350692006139");
			var divisor = BigDecimal.Parse("37975227936943673922808872755445627854565536638199");

			var actual = BigDecimal.Divide(dividend, divisor);

			Assert.AreEqual(expected, actual.ToString());
		}

		[Test]
		public void TestDivide002()
		{
			var resultDividend = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.001", this.Format));
			var resultDivisor = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.5", this.Format));
			var expectedQuotientResult = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.002", this.Format));

			var quotientResult = BigDecimal.Divide(resultDividend, resultDivisor);

			Assert.AreEqual(expectedQuotientResult, quotientResult);
		}

		[Test]
		public void TestDivide003()
		{
			int savePrecision = BigDecimal.Precision;
			BigDecimal.Precision = 11;

			var divisor = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.90606447789", this.Format));
			var actual = BigDecimal.Divide(BigDecimal.One, divisor);
			actual = BigDecimal.Round(actual, 100);

			//var expected = BigDecimal.Parse( "1.1036742134828557" );
			string expected = TestBigDecimalHelper.PrepareValue("1.1036742134", this.Format);

			Assert.AreEqual(expected, actual.ToString());

			BigDecimal.Precision = savePrecision;
		}

		[Test]
		public void TestDivide004()
		{
			var twenty = new BigDecimal(20);
			var actual = BigDecimal.Divide(BigDecimal.One, twenty);
			string expected = TestBigDecimalHelper.PrepareValue("0.05", this.Format);

			Assert.AreEqual(expected, actual.ToString());
		}

		[Test]
		public void TestDivide005()
		{
			var a = new BigDecimal(5);
			var b = new BigDecimal(8);

			var actual = BigDecimal.Divide(a, b);
			string expected = TestBigDecimalHelper.PrepareValue("0.625", this.Format);

			Assert.AreEqual(expected, actual.ToString());
		}

		[Test]
		public void TestDivide006()
		{
			int savePrecision = BigDecimal.Precision;
			BigDecimal.Precision = 12;

			var a = new BigDecimal(1);
			var b = new BigDecimal(7);

			var actual = BigDecimal.Divide(a, b);
			string expected = TestBigDecimalHelper.PrepareValue("0.142857142857", this.Format);

			Assert.AreEqual(expected, actual.ToString());

			BigDecimal.Precision = savePrecision;
		}

		[Test]
		public void TestDivide005A()
		{
			var val1 = TestBigDecimalHelper.PrepareValue("0.5", this.Format);
			var val2 = TestBigDecimalHelper.PrepareValue("0.01", this.Format);

			var value = BigDecimal.Divide(BigDecimal.Parse(val1), BigDecimal.Parse(val2));
			string expected = "50";
			string actual = value.ToString();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestDivide005B()
		{
			var val1 = TestBigDecimalHelper.PrepareValue("0.5", this.Format);
			var val2 = TestBigDecimalHelper.PrepareValue("0.1", this.Format);

			var actual = BigDecimal.Divide(BigDecimal.Parse(val1), BigDecimal.Parse(val2));
			string expected = "5";

			Assert.AreEqual(expected, actual.ToString());
		}

		[Test]
		public void TestDivide005C()
		{
			var val1 = TestBigDecimalHelper.PrepareValue("0.60", this.Format);
			var val2 = TestBigDecimalHelper.PrepareValue("0.01", this.Format);

			var actual = BigDecimal.Divide(BigDecimal.Parse(val1), BigDecimal.Parse(val2));
			string expected = "60";

			Assert.AreEqual(expected, actual.ToString());
		}

		[Test]
		public void TestDivide_50by2_001()
		{
			var actual = BigDecimal.Divide(BigDecimal.Parse("50"), BigDecimal.Parse("2"));
			string expected = "25";

			Assert.AreEqual(expected.ToString(), actual.ToString());
		}

		[Test]
		public void TestDivide_OneOver()
		{
			var numerator = BigDecimal.One;
			var val = TestBigDecimalHelper.PrepareValue("0.141592653589793238462643383279502884197169399375105820974944592307816406286208998628034825342117068", this.Format);
			var denominator = BigDecimal.Parse(val);

			int savePrecision = BigDecimal.Precision;
			BigDecimal.Precision = 100;

			var actual1 = BigDecimal.One / denominator;
			var actual2 = numerator / denominator;
			var actual3 = BigDecimal.Divide(BigDecimal.One, denominator);
			var actual4 = BigDecimal.Divide(numerator, denominator);

			string expectedString = TestBigDecimalHelper.PrepareValue("7.062513305931045769793005152570558042734310025145531333998316873555903337580056083503977475916243946", this.Format);
			var expected = BigDecimal.Parse(expectedString);

			Assert.AreEqual(expectedString, actual1.ToString(), "expectedString != actual1.ToString()");
			Assert.AreEqual(expected.ToString(), actual1.ToString(), "expected.ToString() != actual1.ToString()");
			Assert.AreEqual(expected, actual1, "expected != ( BigDecimal.One / denominator )");
			Assert.AreEqual(expected, actual2, "expected != ( numerator / denominator )");
			Assert.AreEqual(expected, actual3, "expected != ( BigDecimal.Divide(BigDecimal.One, denominator) )");
			Assert.AreEqual(expected, actual4, "expected != ( BigDecimal.Divide(numerator, denominator) )");

			BigDecimal.Precision = savePrecision;
		}

		[Test]
		public void TestFloor001()
		{
			string expected = "3";
			var start = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("3.14159265", this.Format));
			var floor = BigDecimal.Floor(start);

			Assert.AreEqual(expected, floor.ToString());
		}

		[Test]
		public void TestFloor002()
		{
			string expected = "-4";
			var start = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-3.14159265", this.Format));
			var floor = BigDecimal.Floor(start);

			Assert.AreEqual(expected, floor.ToString());
		}

		[Test]
		public void TestFloor003()
		{
			var start = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-0.14159265", this.Format));
			var floor = BigDecimal.Floor(start);

			Assert.AreEqual(BigDecimal.MinusOne, floor);
		}

		[Test]
		public void TestFloor004()
		{
			var start = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.14159265", this.Format));
			var floor = BigDecimal.Floor(start);
			var actual = BigDecimal.Parse(floor.ToString());

			Assert.AreEqual(BigDecimal.Zero, actual);
		}

		[Test]
		public void TestMod1()
		{

			// 31 % 19 = 12
			BigDecimal dividend = 31;
			BigDecimal divisor = 19;
			BigDecimal expected = 12;

			var actual = BigDecimal.Mod(dividend, divisor);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestMod2()
		{
			// 1891 % 31 = 0
			BigDecimal dividend = 1891;
			BigDecimal divisor = 31;
			BigDecimal expected = 0;

			var actual = BigDecimal.Mod(dividend, divisor);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestMod3()
		{

			// 6661 % 60 = 1
			BigDecimal dividend = 6661;
			BigDecimal divisor = 60;
			BigDecimal expected = 1;

			var actual = BigDecimal.Mod(dividend, divisor);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestMod4()
		{

			//NOTE This test fails if the values are Doubles instead of Decimals.

			// 31 % 3.66666 = 1.66672
			BigDecimal dividend = 31m;
			BigDecimal divisor = 3.66666m;
			BigDecimal expected = 1.66672m;

			var actual = BigDecimal.Mod(dividend, divisor);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestMod5()
		{
			// 240 % 2 = 0
			BigDecimal dividend = 240;
			BigDecimal divisor = 2;
			string expectedString = "0";
			BigDecimal expected = 0;

			var actual = BigDecimal.Mod(dividend, divisor);
			string actualString = actual.ToString();

			Assert.AreEqual(expected, actual, $"{dividend} % {divisor} = {actual}");
			Assert.AreEqual(expectedString, actualString, $"{dividend} % {divisor} = {actual}");

			TestContext.WriteLine($"{dividend} % {divisor} = {actual}");
		}

		[Test]
		public void TestMultiply()
		{
			var p = BigDecimal.Parse("-6122421090493547576937037317561418841225758554253106999");
			var actual = p * new BigDecimal(BigInteger.Parse("9996013524558575221488141657137307396757453940901242216"), -34);
			var expected = new BigDecimal(BigInteger.Parse("-61199804023616162130466158636504166524066189692091806226423722790866248079929810268920239053350152436663869784"));

			var matches = expected.ToString().Equals(actual.ToString().Replace(this.Format.NumberDecimalSeparator, ""), StringComparison.Ordinal);

			Assert.IsTrue(matches);
		}

		[Test]
		public void TestMultiply1()
		{
			var p = BigDecimal.Parse("6122421090493547576937037317561418841225758554253106999");
			var q = BigDecimal.Parse("5846418214406154678836553182979162384198610505601062333");
			var expected = "35794234179725868774991807832568455403003778024228226193532908190484670252364677411513516111204504060317568667";

			var actual = BigDecimal.Multiply(p, q);

			Assert.AreEqual(expected, actual.ToString());
		}

		[Test]
		public void TestMultiply2()
		{
			var p = BigDecimal.Parse("6122421090493547576937037317561418841225758554253106999");
			var actual = p * p;
			var expected = "37484040009320200288159018961010536937973891182532366282540247408867702983313960194873589374267102044942786001";

			Assert.AreEqual(expected, actual.ToString());
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

			Assert.AreEqual(expected1, result1.ToString());
			Assert.AreEqual(expected2, result2.ToString());
			Assert.AreEqual(expected3, result3.ToString().Substring(0, 16));
			Assert.AreEqual(expected4, result4.ToString());
			Assert.AreEqual(expected5, result5.ToString().Substring(0, 16));
			Assert.AreEqual(expected6, result6.ToString().Substring(0, 16));
			Assert.AreEqual(expected7, result7.ToString().Substring(0, 16));
			Assert.AreEqual(expected8, result8.ToString());
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

			Assert.AreEqual(expected1, result1.ToString());
			Assert.AreEqual(expected2, result2.ToString());
			Assert.AreEqual(expected3, result3.ToString());
		}

		[Test]
		public void TestNegate()
		{
			string expected = TestBigDecimalHelper.PrepareValue("-1.375", this.Format);
			var actual = BigDecimal.Negate(BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("1.375", this.Format)));

			Assert.AreEqual(expected, actual.ToString());
		}

		[Test]
		public void TestReciprocal001()
		{
			int savePrecision = BigDecimal.Precision;
			BigDecimal.Precision = 10;

			var dividend = new BigDecimal(1);
			var divisor = new BigDecimal(3);

			var actual = BigDecimal.Divide(dividend, divisor);

			//var expected = BigDecimal.Parse( "0.3333333333333333" );
			string expected = TestBigDecimalHelper.PrepareValue("0.3333333333", this.Format);

			Assert.AreEqual(expected, actual.ToString());

			BigDecimal.Precision = savePrecision;
		}

		[Test]
		public void TestReciprocal002()
		{

			// 1/2 = 0.5
			var expected = TestBigDecimalHelper.PrepareValue("0.5", this.Format);

			var dividend = new BigDecimal(1);
			var divisor = new BigDecimal(2);

			var actual = BigDecimal.Divide(dividend, divisor);

			Assert.AreEqual(expected, actual.ToString());
		}

		[Test]
		public void TestReciprocal003()
		{
			int savePrecision = BigDecimal.Precision;
			BigDecimal.Precision = 15;

			//var expected = BigDecimal.Parse( "12.000000000000005" );
			string expected = TestBigDecimalHelper.PrepareValue("12.000000000000004", this.Format);

			var dividend = new BigDecimal(1);
			var divisor = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.0833333333333333", this.Format));

			var actual = BigDecimal.Divide(dividend, divisor);

			Assert.AreEqual(expected, actual.ToString());

			BigDecimal.Precision = savePrecision;
		}

		[Test]
		public void TestReciprocal004()
		{
			int savePrecision = BigDecimal.Precision;
			BigDecimal.Precision = 14;

			// 2/0.63661977236758 == 3.1415926535898
			string expected = TestBigDecimalHelper.PrepareValue("3.1415926535897", this.Format);

			var dividend = new BigDecimal(2);
			var divisor = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.63661977236758", this.Format));

			var actual = BigDecimal.Divide(dividend, divisor);

			Assert.AreEqual(expected, actual.ToString());

			BigDecimal.Precision = savePrecision;
		}

		[Test]
		public void TestSqrt()
		{
			var expected = BigInteger.Parse("8145408529");
			var expectedSquared = BigInteger.Parse("66347680104305943841");

			var squared = expected * expected;
			TestContext.WriteLine($"{expected} squared is {squared}.");
			Assert.AreEqual(squared, expectedSquared);
			var actual = squared.NthRoot(2, out _);

			Assert.AreEqual(expected, actual, "sqrt(66347680104305943841) = 8145408529");
			TestContext.WriteLine($"And {squared} squaredroot is {actual}.");
		}

		[Test]
		public void TestSubtraction001()
		{
			var number = BigDecimal.Parse("4294967295");
			BigDecimal expected = BigDecimal.Parse("2147483648");

			var actual = number - 0x7FFFFFFF;

			Assert.AreEqual(expected, actual);
		}
		/*
		[Test]
		[NonParallelizable]
		public void TestSubtractionRandom(
		[Random(-8.98846567431158E+300D, 8.98846567431158E+300D, 3)] Double b,
		[Random(-8.98846567431158E+300D, 8.98846567431158E+300D, 3)] Double d)
		{
			var strB = $"{b:R}";
			var strD = $"{d:R}";

			TestContext.WriteLine($"{b:R} = {strB}");
			TestContext.WriteLine($"{d:R} = {strD}");

			var bigB = BigDecimal.Parse(strB);
			var bigD = BigDecimal.Parse(strD);

			TestContext.WriteLine(Environment.NewLine);
			TestContext.WriteLine($"bigB = {bigB}");
			TestContext.WriteLine($"bigD = {bigD}");

			var result1 = BigDecimal.Subtract(bigB, bigD);
			var result2 = bigB - bigD;

			Assert.AreEqual(result1, result2);
		}
		*/
		[Test]
		public void TestSubtraction002()
		{
			BigDecimal high = 100.1m;
			BigDecimal low = 25.1m;

			string expected = "75";
			BigDecimal actual = BigDecimal.Subtract(high, low);

			Assert.AreEqual(expected, actual.ToString(), $"100.1 - 25.1 should equal 75.\nHigh: {TestBigDecimalHelper.GetInternalValues(high)}\nLow: {TestBigDecimalHelper.GetInternalValues(low)}\nResult: {TestBigDecimalHelper.GetInternalValues(actual)}");
		}

		[Test]
		public void TestSubtraction003()
		{
			BigDecimal high = (Double)100.3;
			BigDecimal low = (Double)25.1;

			string expected = TestBigDecimalHelper.PrepareValue("75.2", this.Format);
			BigDecimal actual = BigDecimal.Subtract(high, low);

			Assert.AreEqual(expected, actual.ToString(), $"100.3 - 25.1 should equal 75.2\nHigh: {TestBigDecimalHelper.GetInternalValues(high)}\nLow: {TestBigDecimalHelper.GetInternalValues(low)}\nResult: {TestBigDecimalHelper.GetInternalValues(actual)}");
		}

		[Test]
		public void TestSubtraction004()
		{
			BigDecimal high = (Decimal)100.3;
			BigDecimal low = (Decimal)0.3;

			string expected = "100";
			BigDecimal actual = BigDecimal.Subtract(high, low);

			Assert.AreEqual(expected, actual.ToString(), $"100.3 - 0.3 should equal 100.\nHigh: {TestBigDecimalHelper.GetInternalValues(high)}\nLow: {TestBigDecimalHelper.GetInternalValues(low)}\nResult: {TestBigDecimalHelper.GetInternalValues(actual)}");
		}

		[Test]
		public void TestSubtraction005()
		{
			BigDecimal high = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("100.001", this.Format));
			BigDecimal low = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("25.1", this.Format));

			string expected = TestBigDecimalHelper.PrepareValue("74.901", this.Format);
			BigDecimal actual = BigDecimal.Subtract(high, low);

			Assert.AreEqual(expected, actual.ToString(), $"100.001 - 25.1 should equal 74.901.\nHigh: {TestBigDecimalHelper.GetInternalValues(high)}\nLow: {TestBigDecimalHelper.GetInternalValues(low)}\nResult: {TestBigDecimalHelper.GetInternalValues(actual)}");
		}

		[Test]
		public void TestSubtraction006()
		{
			BigDecimal high = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("100.1", this.Format));
			BigDecimal low = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("25.001", this.Format));

			string expected = TestBigDecimalHelper.PrepareValue("75.099", this.Format);
			BigDecimal actual = BigDecimal.Subtract(high, low);

			Assert.AreEqual(expected, actual.ToString(), $"100.1 - 25.001 should equal 75.099.\nHigh: {TestBigDecimalHelper.GetInternalValues(high)}\nLow: {TestBigDecimalHelper.GetInternalValues(low)}\nResult: {TestBigDecimalHelper.GetInternalValues(actual)}");
		}

		[Test]
		public void TestSubtraction007()
		{
			BigDecimal high = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("100.0648646786764", this.Format));
			BigDecimal low = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("25.156379516", this.Format));

			string expected = TestBigDecimalHelper.PrepareValue("74.9084851626764", this.Format);
			BigDecimal actual = BigDecimal.Subtract(high, low);

			Assert.AreEqual(expected, actual.ToString(), $"100.0648646786764 - 25.156379516 should equal 74.9084851626764.\nHigh: {TestBigDecimalHelper.GetInternalValues(high)}\nLow: {TestBigDecimalHelper.GetInternalValues(low)}\nResult: {TestBigDecimalHelper.GetInternalValues(actual)}");
		}

		[Test]
		public void TestSubtraction008()
		{
			BigDecimal high = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.100001", this.Format));
			BigDecimal low = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.101", this.Format));

			string expected = TestBigDecimalHelper.PrepareValue("-0.000999", this.Format);
			BigDecimal actual = BigDecimal.Subtract(high, low);

			Assert.AreEqual(expected, actual.ToString(), $"0.100001 - 0.101 should equal -0.000999.\nHigh: {TestBigDecimalHelper.GetInternalValues(high)}\nLow: {TestBigDecimalHelper.GetInternalValues(low)}\nResult: {TestBigDecimalHelper.GetInternalValues(actual)}");
		}

		[Test]
		public void TestSubtraction009()
		{
			BigDecimal high = BigDecimal.Parse("240");
			BigDecimal low = BigDecimal.Parse("240");
			string expected = "0";

			BigDecimal result = BigDecimal.Subtract(high, low);
			string actual = result.ToString();

			Assert.AreEqual(expected, actual.ToString(), $"240 - 240 should equal 0. Instead got: {TestBigDecimalHelper.GetInternalValues(result)}");
		}

		[Test]
		public void TestSquareRoot001()
		{
			BigDecimal value = BigDecimal.Parse("5");
			Int32 root = 2;
			Int32 precision = 30;

			string expected = TestBigDecimalHelper.PrepareValue("2.236067977499789696409173668731", this.Format);
			BigDecimal actual = BigDecimal.NthRoot(value, root, precision);

			Assert.AreEqual(expected, actual.ToString(), $"{root}th root of {value} did not return {expected}.");
		}

		[Test]
		public void TestSquareRoot002()
		{
			BigDecimal value = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.0981898234602005160423505923443092051160170637298815793320356006279679013343110872318753144061611219225635804218963505102948529140625", this.Format));
			//                                        "0.0981898234602005160423505923443092051160170637298815793320356006279679013343110872318753144061611219225635804218963505102948529140625");
			Int32 root = 2;
			Int32 precision = 50;

			string expected = TestBigDecimalHelper.PrepareValue("0.31335255457742883389571245385500659019295986107402", this.Format);
			BigDecimal result = BigDecimal.NthRoot(value, root, precision);

			BigDecimal actual = BigDecimal.Round(result, precision);

			Assert.AreEqual(expected, actual.ToString(), $"{root}th root of {value} did not return {expected}.");
		}

		[Test]
		public void TestSquareRoot003()
		{
			BigDecimal value = BigDecimal.Parse("9818982346020051604235059234430920511601706372988157933203560062796790133431108723187531440616112192256358042189635051029485291406258555");
			Int32 root = 2;
			Int32 precision = 135;

			string expected = TestBigDecimalHelper.PrepareValue("99090778309689603548815656125983317432034385902667809355596183348807.410596077216611169596571667988328091906450145578959539307248420211367976153463820323404307029425296409616398791728069401888988546189821", this.Format);
			BigDecimal result = BigDecimal.NthRoot(value, root, precision);

			BigDecimal actual = BigDecimal.Round(result, precision);

			Assert.AreEqual(expected, actual.ToString(), $"{root}th root of {value} did not return {expected}.");
		}

		[Test]
		public void TestSquareRoot004()
		{
			BigDecimal value = BigDecimal.Parse("9818982346020051604235059234430920511601706372988157933203560062796790133431108723187531440616112192");
			Int32 root = 2;
			Int32 precision = 135;

			string expected = TestBigDecimalHelper.PrepareValue("99090778309689603548815656125983317432034385902667.809355596183348807410596077216611169596571667988326798354988734930975117508103720966474578967977953788831616628961714711683020533839237", this.Format);
			BigDecimal result = BigDecimal.NthRoot(value, root, precision);

			BigDecimal actual = BigDecimal.Round(result, precision);

			Assert.AreEqual(expected, actual.ToString(), $"{root}th root of {value} did not return {expected}.");
		}

		[Test]
		public void TestSquareRoot_25_001()
		{
			BigDecimal value = BigDecimal.Parse("25");
			Int32 root = 2;
			Int32 precision = 18;

			string expected = "5";
			BigDecimal actual = BigDecimal.NthRoot(value, root, precision);

			Assert.AreEqual(expected, actual.ToString(), $"{root}th root of {value} did not return {expected}.");
		}

		[Test]
		public void TestSquareRoot_25_002()
		{

			BigDecimal value = new BigDecimal(-25);
			Int32 root = 2;
			Int32 precision = 18;

			BigDecimal actual;
			TestDelegate testDelegate = new TestDelegate(() => actual = BigDecimal.NthRoot(value, root, precision));

			Assert.Throws(typeof(ArgumentException), testDelegate);
		}

		[Test]
		public void TestSquareRootOfDecimal()
		{
			BigDecimal value = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.5", this.Format));
			Int32 root = 2;
			Int32 precision = 30;

			string expected = TestBigDecimalHelper.PrepareValue("0.707106781186547524400844362104", this.Format);
			BigDecimal actual = BigDecimal.NthRoot(value, root, precision);

			Assert.AreEqual(expected, actual.ToString(), $"{root}th root of {value} did not return {expected}.");
		}

		[Test]
		public void TestNthRoot()
		{
			BigDecimal value = BigDecimal.Parse("3");
			Int32 root = 3;
			Int32 precision = 50;

			string expected = TestBigDecimalHelper.PrepareValue("1.44224957030740838232163831078010958839186925349935", this.Format);
			BigDecimal actual = BigDecimal.NthRoot(value, root, precision);

			Assert.AreEqual(expected, actual.ToString(), $"{root}th root of {value} did not return {expected}.");
		}

		[Test]
		public void TestNthRootOfDecimal()
		{
			BigDecimal value = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.03", this.Format));
			Int32 root = 3;
			Int32 precision = 50;

			string expected = TestBigDecimalHelper.PrepareValue("0.31072325059538588668776624275223863628549068290674", this.Format);
			BigDecimal actual = BigDecimal.NthRoot(value, root, precision);

			Assert.AreEqual(expected, actual.ToString(), $"{root}th root of {value} did not return {expected}.");
		}

		[Test]
		public void TestGreaterThan001()
		{
			BigDecimal left = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-3.001", this.Format));
			BigDecimal right = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-3.002", this.Format));

			bool actual = left > right;
			bool expected = true;

			Assert.AreEqual(expected, actual, $"{left} > {right} == {expected}");
		}

		[Test]
		public void TestGreaterThan002()
		{
			BigDecimal left = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("3.000002", this.Format));
			BigDecimal right = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("3.000001", this.Format));

			bool actual = left > right;
			bool expected = true;

			Assert.AreEqual(expected, actual, $"{left} > {right} == {expected}");
		}

		[Test]
		public void TestGreaterThanOrEqualTo001()
		{
			BigDecimal left = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-0.001", this.Format));
			BigDecimal right = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-0.001", this.Format));

			bool actual = left >= right;
			bool expected = true;

			Assert.AreEqual(expected, actual, $"{left} > {right} == {expected}");
		}

		[Test]
		public void TestGreaterThanOrEqualTo002()
		{
			BigDecimal left = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-0.001", this.Format));
			BigDecimal right = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-0.002", this.Format));

			bool actual = left >= right;
			bool expected = true;

			Assert.AreEqual(expected, actual, $"{left} > {right} == {expected}");
		}

		[Test]
		public void TestGreaterThanOrEqualTo003()
		{
			BigDecimal left = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.001", this.Format));
			BigDecimal right = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.0001", this.Format));

			bool actual = left >= right;
			bool expected = true;

			Assert.AreEqual(expected, actual, $"{left} > {right} == {expected}");
		}

		[Test]
		public void TestLessThan001()
		{
			BigDecimal left = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-300000.02", this.Format));
			BigDecimal right = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-300000.01", this.Format));

			bool actual = left < right;
			bool expected = true;

			Assert.AreEqual(expected, actual, $"{left} > {right} == {expected}");
		}

		[Test]
		public void TestLessThan002()
		{
			BigDecimal left = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("3000000.00000001", this.Format));
			BigDecimal right = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("3000000.0000001", this.Format));

			bool actual = left < right;
			bool expected = true;

			Assert.AreEqual(expected, actual, $"{left} > {right} == {expected}");
		}

		[Test]
		public void TestLessThanOrEqualTo001()
		{
			BigDecimal left = BigDecimal.Parse("3");
			BigDecimal right = BigDecimal.Parse("3");

			bool actual = left <= right;
			bool expected = true;

			Assert.AreEqual(expected, actual, $"{left} > {right} == {expected}");
		}

		[Test]
		public void TestLessThanOrEqualTo002()
		{
			BigDecimal left = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-3.0000002", this.Format));
			BigDecimal right = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-3.0000001", this.Format));

			bool actual = left <= right;
			bool expected = true;

			Assert.AreEqual(expected, actual, $"{left} > {right} == {expected}");
		}

		[Test]
		public void TestLessThanOrEqualTo003()
		{
			BigDecimal left = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-3.0000001", this.Format));
			BigDecimal right = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-3.00000001", this.Format));

			bool actual = left <= right;
			bool expected = true;

			Assert.AreEqual(expected, actual, $"{left} > {right} == {expected}");
		}

		[Test]
		public void TestLessThanOrEqualTo004()
		{
			BigDecimal left = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("3.000000201", this.Format));
			BigDecimal right = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("30.00000201", this.Format));

			bool actual = left <= right;
			bool expected = true;

			Assert.AreEqual(expected, actual, $"{left} > {right} == {expected}");
		}

		[Test]
		public void TestDoubleCasting()
		{
			BigDecimal value1 = BigDecimal.Pow(10, -1);
			BigDecimal value2 = new BigDecimal(99.00000001);
			BigDecimal res = BigDecimal.Divide(value2, value1);

			double delta = 0.0000001;
			double expected = 990.0000001;
			double actual = (double)res;

			Assert.AreEqual(expected, actual, delta, $"{expected} != {actual}");
		}

		[Test]
		public void TestSingleCasting()
		{
			BigDecimal value1 = BigDecimal.Pow(10, -1);
			BigDecimal value2 = new BigDecimal(99.00000001);
			BigDecimal res = BigDecimal.Divide(value2, value1);

			double delta = 0.0000001;
			Single expected = 990.0000001f;
			Single actual = (Single)res;

			Assert.AreEqual(expected, actual, delta, $"{expected} != {actual}");
		}

		[Test]
		public void TestDecimalCasting()
		{
			BigDecimal value1 = BigDecimal.Pow(10, -1);
			BigDecimal value2 = new BigDecimal(99.00000001);
			BigDecimal res = BigDecimal.Divide(value2, value1);

			double delta = 0.0000001;
			double expected = 990.0000001;
			decimal actual = (decimal)res;

			Assert.AreEqual(expected, Convert.ToDouble(actual), delta, $"{expected} != {actual}");
		}
	}
}