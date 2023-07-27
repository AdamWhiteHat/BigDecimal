// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "TestBigDecimalOperations.cs" last formatted on 2022-01-11 at 1:59 AM by Protiguous.

namespace TestBigDecimal;

using System;
using System.Numerics;
using ExtendedNumerics;
using ExtendedNumerics.Helpers;
using NUnit.Framework;

[TestFixture]
public class TestBigDecimalOperations
{

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
		var expected = BigDecimal.Parse("123456.709876543");

		Assert.AreEqual(expected, actual);
	}

	[Test]
	public void TestBigDecimalPow()
	{
		var expected = "268637376395543538746286686601216000000000000";
		// 5040 ^ 12 = 268637376395543538746286686601216000000000000

		var number = BigDecimal.Parse("5040");
		var exp12 = BigDecimal.Pow(number, 12);
		var actual = exp12.ToString();

		Assert.AreEqual(expected, actual, "5040 ^ 12  =  268637376395543538746286686601216000000000000");
	}

	[Test]
	public void TestCeiling001()
	{
		var expected = BigDecimal.Parse("4");

		var actual = BigDecimal.Ceiling(BigDecimal.Pi);

		Assert.AreEqual(expected, actual);
	}

	[Test]
	public void TestCeiling002()
	{
		var expected = BigDecimal.Parse("-3");
		var ceiling = BigDecimal.Ceiling(BigDecimal.Pi);

		Assert.AreNotEqual(expected, ceiling);
	}

	[Test]
	public void TestCeiling003()
	{
		var start = BigDecimal.Parse("0.14159265");
		var ceiling = BigDecimal.Ceiling(start);

		Assert.AreEqual(BigDecimal.One, ceiling);
	}

	[Test]
	public void TestCeiling004()
	{
		var start = BigDecimal.Parse("-0.14159265");
		var ceiling = BigDecimal.Ceiling(start);

		Assert.AreEqual(BigDecimal.Zero, ceiling);
	}

	[Test]
	public void TestDivide000()
	{
		var dividend = BigDecimal.Parse("0.63");
		var divisor = BigDecimal.Parse("0.09");

		var actual = BigDecimal.Divide(dividend, divisor);
		var expected = BigDecimal.Parse("7");

		Assert.AreEqual(expected, actual);
	}

	[Test]
	public void TestDivide001()
	{
		var expected = BigDecimal.Parse("40094690950920881030683735292761468389214899724061");

		var dividend = BigDecimal.Parse("1522605027922533360535618378132637429718068114961380688657908494580122963258952897654000350692006139");
		var divisor = BigDecimal.Parse("37975227936943673922808872755445627854565536638199");

		var actual = BigDecimal.Divide(dividend, divisor);

		Assert.AreEqual(expected, actual);
	}

	[Test]
	public void TestDivide002()
	{
		var resultDividend = BigDecimal.Parse("0.001");
		var resultDivisor = BigDecimal.Parse("0.5");
		var expectedQuotientResult = BigDecimal.Parse("0.002");

		var quotientResult = BigDecimal.Divide(resultDividend, resultDivisor);

		Assert.AreEqual(expectedQuotientResult, quotientResult);
	}

	[Test]
	public void TestDivide003()
	{
		int savePrecision = BigDecimal.Precision;
		BigDecimal.Precision = 11;

		var divisor = BigDecimal.Parse("0.90606447789");
		var actual = BigDecimal.Divide(BigDecimal.One, divisor);
		actual = BigDecimal.Round(actual, 100);

		//var expected = BigDecimal.Parse( "1.1036742134828557" );
		var expected = BigDecimal.Parse("1.1036742134");

		Assert.AreEqual(expected, actual);

		BigDecimal.Precision = savePrecision;
	}

	[Test]
	public void TestDivide004()
	{
		var twenty = new BigDecimal(20);
		var actual = BigDecimal.Divide(BigDecimal.One, twenty);
		var expected = BigDecimal.Parse("0.05");

		Assert.AreEqual(expected, actual);
	}

	[Test]
	public void TestDivide005()
	{
		var a = new BigDecimal(5);
		var b = new BigDecimal(8);

		var actual = BigDecimal.Divide(a, b);
		var expected = BigDecimal.Parse("0.625");

		Assert.AreEqual(expected.ToString(), actual.ToString());
	}

	[Test]
	public void TestDivide006()
	{
		int savePrecision = BigDecimal.Precision;
		BigDecimal.Precision = 12;

		var a = new BigDecimal(1);
		var b = new BigDecimal(7);

		var actual = BigDecimal.Divide(a, b);
		var expected = BigDecimal.Parse("0.142857142857");

		Assert.AreEqual(expected.ToString(), actual.ToString());

		BigDecimal.Precision = savePrecision;
	}

	[Test]
	public void TestDivide005A()
	{
		var value = BigDecimal.Divide(BigDecimal.Parse("0.5"), BigDecimal.Parse("0.01"));
		var expected = "50";
		var actual = value.ToString();

		Assert.AreEqual(expected, actual);
	}

	[Test]
	public void TestDivide005B()
	{
		var actual = BigDecimal.Divide(BigDecimal.Parse("0.5"), BigDecimal.Parse("0.1"));
		var expected = BigDecimal.Parse("5");

		Assert.AreEqual(expected, actual);
	}

	[Test]
	public void TestDivide005C()
	{
		var actual = BigDecimal.Divide(BigDecimal.Parse("0.60"), BigDecimal.Parse("0.01"));
		var expected = BigDecimal.Parse("60");

		Assert.AreEqual(expected, actual);
	}

	[Test]
	public void TestDivide_50by2_001()
	{
		var actual = BigDecimal.Divide(BigDecimal.Parse("50"), BigDecimal.Parse("2"));
		var expected = BigDecimal.Parse("25");

		Assert.AreEqual(expected.ToString(), actual.ToString());
	}

	[Test]
	public void TestDivide_OneOver()
	{
		var numerator = BigDecimal.One;
		var denominator = BigDecimal.Parse("0.141592653589793238462643383279502884197169399375105820974944592307816406286208998628034825342117068");

		int savePrecision = BigDecimal.Precision;
		BigDecimal.Precision = 100;

		var actual1 = BigDecimal.One / denominator;
		var actual2 = numerator / denominator;
		var actual3 = BigDecimal.Divide(BigDecimal.One, denominator);
		var actual4 = BigDecimal.Divide(numerator, denominator);

		string expectedString = "7.062513305931045769793005152570558042734310025145531333998316873555903337580056083503977475916243946";
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
		var expected = BigDecimal.Parse("3");
		var start = BigDecimal.Parse("3.14159265");
		var floor = BigDecimal.Floor(start);

		Assert.AreEqual(expected, floor);
	}

	[Test]
	public void TestFloor002()
	{
		var expected = BigDecimal.Parse("-4");
		var start = BigDecimal.Parse("-3.14159265");
		var floor = BigDecimal.Floor(start);

		Assert.AreEqual(expected, floor);
	}

	[Test]
	public void TestFloor003()
	{
		var start = BigDecimal.Parse("-0.14159265");
		var floor = BigDecimal.Floor(start);

		Assert.AreEqual(BigDecimal.MinusOne, floor);
	}

	[Test]
	public void TestFloor004()
	{
		var start = BigDecimal.Parse("0.14159265");
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
	public void TestMultiply()
	{
		var p = BigDecimal.Parse("-6122421090493547576937037317561418841225758554253106999");
		var actual = p * new BigDecimal(BigInteger.Parse("9996013524558575221488141657137307396757453940901242216"), -34);
		var expected = new BigDecimal(BigInteger.Parse("-61199804023616162130466158636504166524066189692091806226423722790866248079929810268920239053350152436663869784"));

		var matches = expected.ToString().Equals(actual.ToString().Replace(".", ""), StringComparison.Ordinal);

		Assert.IsTrue(matches);
	}

	[Test]
	public void TestMultiply1()
	{
		var p = BigDecimal.Parse("6122421090493547576937037317561418841225758554253106999");
		var q = BigDecimal.Parse("5846418214406154678836553182979162384198610505601062333");
		var expected = BigDecimal.Parse("35794234179725868774991807832568455403003778024228226193532908190484670252364677411513516111204504060317568667");

		var actual = BigDecimal.Multiply(p, q);

		Assert.AreEqual(expected, actual);
	}

	[Test]
	public void TestMultiply2()
	{
		var p = BigDecimal.Parse("6122421090493547576937037317561418841225758554253106999");
		var actual = p * p;
		var expected = BigDecimal.Parse("37484040009320200288159018961010536937973891182532366282540247408867702983313960194873589374267102044942786001");

		Assert.AreEqual(expected, actual);
	}

	[Test]
	public void TestNegate()
	{
		var expected = BigDecimal.Parse("1.375");
		var actual = BigDecimal.Negate(expected);

		Assert.AreEqual("-1.375", actual.ToString());
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
		var expected = BigDecimal.Parse("0.3333333333");

		Assert.AreEqual(expected, actual);

		BigDecimal.Precision = savePrecision;
	}

	[Test]
	public void TestReciprocal002()
	{

		// 1/2 = 0.5
		var expected = BigDecimal.Parse("0.5");

		var dividend = new BigDecimal(1);
		var divisor = new BigDecimal(2);

		var actual = BigDecimal.Divide(dividend, divisor);

		Assert.AreEqual(expected, actual);
	}

	[Test]
	public void TestReciprocal003()
	{
		int savePrecision = BigDecimal.Precision;
		BigDecimal.Precision = 15;

		//var expected = BigDecimal.Parse( "12.000000000000005" );
		var expected = BigDecimal.Parse("12.000000000000004");

		var dividend = new BigDecimal(1);
		var divisor = BigDecimal.Parse("0.0833333333333333");

		var actual = BigDecimal.Divide(dividend, divisor);

		Assert.AreEqual(expected, actual);

		BigDecimal.Precision = savePrecision;
	}

	[Test]
	public void TestReciprocal004()
	{
		int savePrecision = BigDecimal.Precision;
		BigDecimal.Precision = 14;

		// 2/0.63661977236758 == 3.1415926535898
		var expected = BigDecimal.Parse("3.14159265358970");

		var dividend = new BigDecimal(2);
		var divisor = BigDecimal.Parse("0.63661977236758");

		var actual = BigDecimal.Divide(dividend, divisor);

		Assert.AreEqual(expected, actual);

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

		var actual = squared.NthRoot(2, out var remainder);

		Assert.AreEqual(expected, actual, "sqrt(66347680104305943841) = 8145408529");
		TestContext.WriteLine($"And {squared} squaredroot is {actual}.");
	}

	[Test]
	public void TestSubtraction001()
	{
		var number = BigDecimal.Parse("4294967295");
		var expected = BigDecimal.Parse("2147483648");

		var actual = number - 0x7FFFFFFF;

		Assert.AreEqual(expected, actual);
	}

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

	[Test]
	public void TestSubtraction002()
	{
		BigDecimal high = 100.1m;
		BigDecimal low = 25.1m;

		BigDecimal expected = BigDecimal.Parse("75");
		BigDecimal actual = BigDecimal.Subtract(high, low);

		Assert.AreEqual(expected, actual, $"100.1 - 25.1 should equal 75.\nHigh: {TestBigDecimalHelper.GetInternalValues(high)}\nLow: {TestBigDecimalHelper.GetInternalValues(low)}\nResult: {TestBigDecimalHelper.GetInternalValues(actual)}");
	}

	[Test]
	public void TestSubtraction003()
	{
		BigDecimal high = (Double)100.3;
		BigDecimal low = (Double)25.1;

		BigDecimal expected = BigDecimal.Parse("75.199999999999999");
		BigDecimal actual = BigDecimal.Subtract(high, low);

		Assert.AreEqual(expected, actual, $"100.3 - 25.1 should equal 75.199999999999999 because of the lack of type specification, 25.1 is cast as a Double, and doesnt do a good job representing a number such as 25.1, similar how 1/3 isnt represented in base 10 in a clean way.\nHigh: {TestBigDecimalHelper.GetInternalValues(high)}\nLow: {TestBigDecimalHelper.GetInternalValues(low)}\nResult: {TestBigDecimalHelper.GetInternalValues(actual)}");
	}

	[Test]
	public void TestSubtraction004()
	{
		BigDecimal high = (Decimal)100.3;
		BigDecimal low = (Decimal)0.3;

		BigDecimal expected = BigDecimal.Parse("100");
		BigDecimal actual = BigDecimal.Subtract(high, low);

		Assert.AreEqual(expected, actual, $"100.3 - 0.3 should equal 100.\nHigh: {TestBigDecimalHelper.GetInternalValues(high)}\nLow: {TestBigDecimalHelper.GetInternalValues(low)}\nResult: {TestBigDecimalHelper.GetInternalValues(actual)}");
	}

	[Test]
	public void TestSubtraction005()
	{
		BigDecimal high = BigDecimal.Parse("100.001");
		BigDecimal low = BigDecimal.Parse("25.1");

		BigDecimal expected = BigDecimal.Parse("74.901");
		BigDecimal actual = BigDecimal.Subtract(high, low);

		Assert.AreEqual(expected, actual, $"100.001 - 25.1 should equal 74.901.\nHigh: {TestBigDecimalHelper.GetInternalValues(high)}\nLow: {TestBigDecimalHelper.GetInternalValues(low)}\nResult: {TestBigDecimalHelper.GetInternalValues(actual)}");
	}

	[Test]
	public void TestSubtraction006()
	{
		BigDecimal high = BigDecimal.Parse("100.1");
		BigDecimal low = BigDecimal.Parse("25.001");

		BigDecimal expected = BigDecimal.Parse("75.099");
		BigDecimal actual = BigDecimal.Subtract(high, low);

		Assert.AreEqual(expected, actual, $"100.1 - 25.001 should equal 75.099.\nHigh: {TestBigDecimalHelper.GetInternalValues(high)}\nLow: {TestBigDecimalHelper.GetInternalValues(low)}\nResult: {TestBigDecimalHelper.GetInternalValues(actual)}");
	}

	[Test]
	public void TestSubtraction007()
	{
		BigDecimal high = BigDecimal.Parse("100.0648646786764");
		BigDecimal low = BigDecimal.Parse("25.156379516");

		BigDecimal expected = BigDecimal.Parse("74.9084851626764");
		BigDecimal actual = BigDecimal.Subtract(high, low);

		Assert.AreEqual(expected, actual, $"100.0648646786764 - 25.156379516 should equal 74.9084851626764.\nHigh: {TestBigDecimalHelper.GetInternalValues(high)}\nLow: {TestBigDecimalHelper.GetInternalValues(low)}\nResult: {TestBigDecimalHelper.GetInternalValues(actual)}");
	}

	[Test]
	public void TestSubtraction008()
	{
		BigDecimal high = BigDecimal.Parse("0.100001");
		BigDecimal low = BigDecimal.Parse("0.101");

		BigDecimal expected = BigDecimal.Parse("-0.000999");
		BigDecimal actual = BigDecimal.Subtract(high, low);

		Assert.AreEqual(expected, actual, $"0.100001 - 0.101 should equal -0.000999.\nHigh: {TestBigDecimalHelper.GetInternalValues(high)}\nLow: {TestBigDecimalHelper.GetInternalValues(low)}\nResult: {TestBigDecimalHelper.GetInternalValues(actual)}");
	}

	[Test]
	public void TestSquareRoot001()
	{
		BigDecimal value = BigDecimal.Parse("5");
		Int32 root = 2;
		Int32 precision = 30;

		BigDecimal expected = BigDecimal.Parse("2.236067977499789696409173668731");
		BigDecimal actual = BigDecimal.NthRoot(value, root, precision);

		Assert.AreEqual(expected, actual, $"{root}th root of {value} did not return {expected}.");
	}

	[Test]
	public void TestSquareRoot002()
	{
		BigDecimal value = BigDecimal.Parse("0.0981898234602005160423505923443092051160170637298815793320356006279679013343110872318753144061611219225635804218963505102948529140625");
		//                                        "0.0981898234602005160423505923443092051160170637298815793320356006279679013343110872318753144061611219225635804218963505102948529140625");
		Int32 root = 2;
		Int32 precision = 50;

		BigDecimal expected = BigDecimal.Parse("0.31335255457742883389571245385500659019295986107402");
		BigDecimal result = BigDecimal.NthRoot(value, root, precision);

		BigDecimal actual = BigDecimal.Round(result, precision);

		Assert.AreEqual(expected, actual, $"{root}th root of {value} did not return {expected}.");
	}


	[Test]
	public void TestSquareRoot003()
	{
		BigDecimal value = BigDecimal.Parse("9818982346020051604235059234430920511601706372988157933203560062796790133431108723187531440616112192256358042189635051029485291406258555");
		Int32 root = 2;
		Int32 precision = 135;

		BigDecimal expected = BigDecimal.Parse("99090778309689603548815656125983317432034385902667809355596183348807.410596077216611169596571667988328091906450145578959539307248420211367976153463820323404307029425296409616398791728069401888988546189821");
		BigDecimal result = BigDecimal.NthRoot(value, root, precision);

		BigDecimal actual = BigDecimal.Round(result, precision);

		Assert.AreEqual(expected, actual, $"{root}th root of {value} did not return {expected}.");
	}

	[Test]
	public void TestSquareRoot004()
	{
		BigDecimal value = BigDecimal.Parse("9818982346020051604235059234430920511601706372988157933203560062796790133431108723187531440616112192");
		Int32 root = 2;
		Int32 precision = 135;

		BigDecimal expected = BigDecimal.Parse("99090778309689603548815656125983317432034385902667.809355596183348807410596077216611169596571667988326798354988734930975117508103720966474578967977953788831616628961714711683020533839237");
		BigDecimal result = BigDecimal.NthRoot(value, root, precision);

		BigDecimal actual = BigDecimal.Round(result, precision);

		Assert.AreEqual(expected, actual, $"{root}th root of {value} did not return {expected}.");
	}

	[Test]
	public void TestSquareRoot_25_001()
	{
		BigDecimal value = BigDecimal.Parse("25");
		Int32 root = 2;
		Int32 precision = 18;

		BigDecimal expected = BigDecimal.Parse("5");
		BigDecimal actual = BigDecimal.NthRoot(value, root, precision);

		Assert.AreEqual(expected, actual, $"{root}th root of {value} did not return {expected}.");
	}

	[Test]
	public void TestSquareRoot_25_002()
	{

		BigDecimal value = new BigDecimal(-25);
		Int32 root = 2;
		Int32 precision = 18;

		BigDecimal expected = BigDecimal.Parse("5");

		BigDecimal actual;
		TestDelegate testDelegate = new TestDelegate(() =>
		{
			actual = BigDecimal.NthRoot(value, root, precision);
		});

		Assert.Throws(typeof(ArgumentException), testDelegate);
	}

	[Test]
	public void TestSquareRootOfDecimal()
	{
		BigDecimal value = BigDecimal.Parse("0.5");
		Int32 root = 2;
		Int32 precision = 30;

		BigDecimal expected = BigDecimal.Parse("0.707106781186547524400844362104");
		BigDecimal actual = BigDecimal.NthRoot(value, root, precision);

		Assert.AreEqual(expected, actual, $"{root}th root of {value} did not return {expected}.");
	}

	[Test]
	public void TestNthRoot()
	{
		BigDecimal value = BigDecimal.Parse("3");
		Int32 root = 3;
		Int32 precision = 50;

		BigDecimal expected = BigDecimal.Parse("1.44224957030740838232163831078010958839186925349935");
		BigDecimal actual = BigDecimal.NthRoot(value, root, precision);

		Assert.AreEqual(expected, actual, $"{root}th root of {value} did not return {expected}.");
	}

	[Test]
	public void TestNthRootOfDecimal()
	{
		BigDecimal value = BigDecimal.Parse("0.03");
		Int32 root = 3;
		Int32 precision = 50;

		BigDecimal expected = BigDecimal.Parse("0.31072325059538588668776624275223863628549068290674");
		BigDecimal actual = BigDecimal.NthRoot(value, root, precision);

		Assert.AreEqual(expected, actual, $"{root}th root of {value} did not return {expected}.");
	}
}