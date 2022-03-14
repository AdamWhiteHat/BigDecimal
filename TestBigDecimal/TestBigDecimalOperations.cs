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

[Parallelizable(ParallelScope.All)]
[TestFixture]
public class TestBigDecimalOperations {

    [Test]
    public void TestAddition() {
        var number1 = BigDecimal.Parse("1234567890");
        var expected = BigDecimal.Parse("3382051537");

        var actual = number1 + 0x7FFFFFFF;

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestBigDecimalPow() {
        var expected = BigDecimal.Parse("268637376395543538746286686601216000000000000");

        // 5040 ^ 12 = 268637376395543538746286686601216000000000000

        var number = BigDecimal.Parse("5040");
        var actual = BigDecimal.Pow(number, 12);

        Assert.AreEqual(expected, actual, "5040 ^ 12  =  268637376395543538746286686601216000000000000");
    }

    [Test]
    public void TestCeiling001() {
        var expected = BigDecimal.Parse("4");

        var actual = BigDecimal.Ceiling(BigDecimal.Pi);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestCeiling002() {
        var expected = BigDecimal.Parse("-3");
        var ceiling = BigDecimal.Ceiling(BigDecimal.Pi);

        Assert.AreNotEqual(expected, ceiling);
    }

    [Test]
    public void TestCeiling003() {
        var start = BigDecimal.Parse("0.14159265");
        var ceiling = BigDecimal.Ceiling(start);

        Assert.AreEqual(BigDecimal.One, ceiling);
    }

    [Test]
    public void TestCeiling004() {
        var start = BigDecimal.Parse("-0.14159265");
        var ceiling = BigDecimal.Ceiling(start);

        Assert.AreEqual(BigDecimal.Zero, ceiling);
    }

    [Test]
    public void TestDivide000() {
        var dividend = BigDecimal.Parse("0.63");
        var divisor = BigDecimal.Parse("0.09");

        var actual = BigDecimal.Divide(dividend, divisor);
        var expected = BigDecimal.Parse("7");

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestDivide001() {
        var expected = BigDecimal.Parse("40094690950920881030683735292761468389214899724061");

        var dividend = BigDecimal.Parse("1522605027922533360535618378132637429718068114961380688657908494580122963258952897654000350692006139");
        var divisor = BigDecimal.Parse("37975227936943673922808872755445627854565536638199");

        var actual = BigDecimal.Divide(dividend, divisor);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestDivide002() {
        var resultDividend = BigDecimal.Parse("0.001");
        var resultDivisor = BigDecimal.Parse("0.5");
        var expectedQuotientResult = BigDecimal.Parse("0.002");

        var quotientResult = BigDecimal.Divide(resultDividend, resultDivisor);

        Assert.AreEqual(expectedQuotientResult, quotientResult);
    }

    [Test]
    public void TestDivide003() {
        var divisor = BigDecimal.Parse("0.90606447789");
        var actual = BigDecimal.Divide(BigDecimal.One, divisor);
        actual = BigDecimal.Truncate(actual, 100);

        //var expected = BigDecimal.Parse( "1.1036742134828557" );
        var expected = BigDecimal.Parse("1.1036742134");

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestDivide004() {
        var twenty = new BigDecimal(20);
        var actual = BigDecimal.Divide(BigDecimal.One, twenty);
        var expected = BigDecimal.Parse("0.05");

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestDivide005A() {
        var actual = BigDecimal.Divide(BigDecimal.Parse("0.5"), BigDecimal.Parse("0.01"));
        var expected = BigDecimal.Parse("50");

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestDivide005B() {
        var actual = BigDecimal.Divide(BigDecimal.Parse("0.5"), BigDecimal.Parse("0.1"));
        var expected = BigDecimal.Parse("5");

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestDivide005C() {
        var actual = BigDecimal.Divide(BigDecimal.Parse("0.60"), BigDecimal.Parse("0.01"));
        var expected = BigDecimal.Parse("60");

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestFloor001() {
        var expected = BigDecimal.Parse("3");
        var start = BigDecimal.Parse("3.14159265");
        var floor = BigDecimal.Floor(start);

        Assert.AreEqual(expected, floor);
    }

    [Test]
    public void TestFloor002() {
        var expected = BigDecimal.Parse("-4");
        var start = BigDecimal.Parse("-3.14159265");
        var floor = BigDecimal.Floor(start);

        Assert.AreEqual(expected, floor);
    }

    [Test]
    public void TestFloor003() {
        var start = BigDecimal.Parse("-0.14159265");
        var floor = BigDecimal.Floor(start);

        Assert.AreEqual(BigDecimal.MinusOne, floor);
    }

    [Test]
    public void TestFloor004() {
        var start = BigDecimal.Parse("0.14159265");
        var floor = BigDecimal.Floor(start);
        var actual = BigDecimal.Parse(floor.ToString());

        Assert.AreEqual(BigDecimal.Zero, actual);
    }

    [Test]
    public void TestMod1() {

        // 31 % 19 = 12
        BigDecimal dividend = 31;
        BigDecimal divisor = 19;
        BigDecimal expected = 12;

        var actual = BigDecimal.Mod(dividend, divisor);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestMod2() {

        // 1891 % 31 = 0
        BigDecimal dividend = 1891;
        BigDecimal divisor = 31;
        BigDecimal expected = 0;

        var actual = BigDecimal.Mod(dividend, divisor);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestMod3() {

        // 6661 % 60 = 1
        BigDecimal dividend = 6661;
        BigDecimal divisor = 60;
        BigDecimal expected = 1;

        var actual = BigDecimal.Mod(dividend, divisor);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestMod4() {

        //NOTE This test fails if the values are Doubles instead of Decimals.

        // 31 % 3.66666 = 1.66672
        BigDecimal dividend = 31m;
        BigDecimal divisor = 3.66666m;
        BigDecimal expected = 1.66672m;

        var actual = BigDecimal.Mod(dividend, divisor);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestMultiply() {
        var p = BigDecimal.Parse("-6122421090493547576937037317561418841225758554253106999");
        var actual = p * new BigDecimal(BigInteger.Parse("9996013524558575221488141657137307396757453940901242216"), -34);
        var expected = new BigDecimal(BigInteger.Parse("-61199804023616162130466158636504166524066189692091806226423722790866248079929810268920239053350152436663869784"));

        var matches = expected.ToString().Equals(actual.ToString().Replace(".", ""), StringComparison.Ordinal);

        Assert.IsTrue(matches);
    }

    [Test]
    public void TestMultiply1() {
        var p = BigDecimal.Parse("6122421090493547576937037317561418841225758554253106999");
        var q = BigDecimal.Parse("5846418214406154678836553182979162384198610505601062333");
        var expected = BigDecimal.Parse("35794234179725868774991807832568455403003778024228226193532908190484670252364677411513516111204504060317568667");

        var actual = BigDecimal.Multiply(p, q);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestMultiply2() {
        var p = BigDecimal.Parse("6122421090493547576937037317561418841225758554253106999");
        var actual = p * p;
        var expected = BigDecimal.Parse("37484040009320200288159018961010536937973891182532366282540247408867702983313960194873589374267102044942786001");

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestNegate() {
        var expected = BigDecimal.Parse("1.375");
        var actual = BigDecimal.Negate(expected);

        Assert.AreEqual("-1.375", actual.ToString());
    }

    [Test]
    public void TestReciprocal001() {
        var dividend = new BigDecimal(1);
        var divisor = new BigDecimal(3);

        var actual = BigDecimal.Divide(dividend, divisor);

        //var expected = BigDecimal.Parse( "0.3333333333333333" );
        var expected = BigDecimal.Parse("0.3");

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestReciprocal002() {

        // 1/2 = 0.5
        var expected = BigDecimal.Parse("0.5");

        var dividend = new BigDecimal(1);
        var divisor = new BigDecimal(2);

        var actual = BigDecimal.Divide(dividend, divisor);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestReciprocal003() {

        //var expected = BigDecimal.Parse( "12.000000000000005" );
        var expected = BigDecimal.Parse("12.000000000000004");

        var dividend = new BigDecimal(1);
        var divisor = BigDecimal.Parse("0.0833333333333333");

        var actual = BigDecimal.Divide(dividend, divisor);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestReciprocal004() {

        // 2/0.63661977236758 == 3.1415926535898
        var expected = BigDecimal.Parse("3.14159265358970");

        var dividend = new BigDecimal(2);
        var divisor = BigDecimal.Parse("0.63661977236758");

        var actual = BigDecimal.Divide(dividend, divisor);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestSqrt() {
        var expected = BigInteger.Parse("8145408529");
        var expectedSquared = BigInteger.Parse("66347680104305943841");

        var squared = expected * expected;
        TestContext.WriteLine($"{expected} squared is {squared}.");
        Assert.Equals(squared, expectedSquared);

        var actual = squared.NthRoot(2, out var remainder);

        Assert.AreEqual(expected, actual, "sqrt(66347680104305943841) = 8145408529");
        TestContext.WriteLine($"And {squared} squaredroot is {actual}.");
    }

    [Test]
    public void TestSubtraction() {
        var number = BigDecimal.Parse("4294967295");
        var expected = BigDecimal.Parse("2147483648");

        var actual = number - 0x7FFFFFFF;

        Assert.AreEqual(expected, actual);
    }
}