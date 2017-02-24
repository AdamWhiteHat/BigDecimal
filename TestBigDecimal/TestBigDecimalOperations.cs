using System;
using System.Numerics;
using AJRLibray.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace TestBigDecimal
{
	[TestClass]
	public class TestBigDecimalOperations
	{
		[TestMethod]
		public void TestAddition()
		{
			BigDecimal number1 = BigDecimal.Parse("1234567890");
			BigDecimal expectedResult = BigDecimal.Parse("3382051537");

			BigDecimal result = number1 + 2147483647;

			Assert.AreEqual(expectedResult, result);
		}

		[TestMethod]
		public void TestSubtraction()
		{
			BigDecimal number = BigDecimal.Parse("4294967295");
			BigDecimal expectedResult = BigDecimal.Parse("2147483648");

			BigDecimal result = number - 2147483647;

			Assert.AreEqual(expectedResult, result);
		}

		[TestMethod]
		public void TestMultiply()
		{
			BigDecimal expectedResult1 = BigDecimal.Parse("35794234179725868774991807832568455403003778024228226193532908190484670252364677411513516111204504060317568667");
			BigDecimal expectedResult2 = BigDecimal.Parse("37484040009320200288159018961010536937973891182532366282540247408867702983313960194873589374267102044942786001");
			BigDecimal expectedResult3 = new BigDecimal(BigInteger.Negate(BigInteger.Parse("61199804023616162130466158636504166524066189692091806226423722790866248079929810268920239053350152436663869784")));

			//"6119980402361616213046615863650416652406618969209180622642372279086624807992.9810268920239053350152436663869784"

			expectedResult3.Truncate();
			expectedResult3.Normalize();

			BigDecimal p = BigDecimal.Parse("6122421090493547576937037317561418841225758554253106999");
			BigDecimal q = BigDecimal.Parse("5846418214406154678836553182979162384198610505601062333");

			BigDecimal result1 = BigDecimal.Multiply(p, q);
			BigDecimal result2 = p * p;
			BigDecimal result3 = -1 * p * new BigDecimal(BigInteger.Parse("9996013524558575221488141657137307396757453940901242216"), -34);

			// -1 * 6122421090493547576937037317561418841225758554253106999  *   999601352455857522148.8141657137307396757453940901242216
			// = -6119980402361616213046615863650416652406618969209180622642372279086624807992.9810268920239053350152436663869784
			//                                                                   9996013524558575221488141657137307396757453940901242216

			bool matches1 = expectedResult1.Equals(result1);
			bool matches2 = expectedResult2.Equals(result2);
			bool matches3 = expectedResult3.ToString().Equals(result3.ToString().Replace(".", ""));

			Assert.IsTrue(matches1);
			Assert.IsTrue(matches2);
			Assert.IsTrue(matches3);
		}

		[TestMethod]
		public void TestDivide()
		{
			BigDecimal expectedResult = BigDecimal.Parse("40094690950920881030683735292761468389214899724061");
			BigDecimal dividend = BigDecimal.Parse("1522605027922533360535618378132637429718068114961380688657908494580122963258952897654000350692006139");
			BigDecimal divisor = BigDecimal.Parse("37975227936943673922808872755445627854565536638199");

			BigDecimal result = BigDecimal.Divide(dividend, divisor);

			Assert.AreEqual(expectedResult, result);
		}

		[TestMethod]
		public void TestExponent()
		{
			BigDecimal expectedResult = BigDecimal.Parse("268637376395543538746286686601216000000000000");
			// 5040 ^ 12  =  268637376395543538746286686601216000000000000

			BigDecimal number = BigDecimal.Parse("5040");
			BigDecimal result = BigDecimal.Pow(number, 12);

			//string resultString = result.ToString();
			//bool equalsTarget = expectedResult.Equals(result);

			Assert.AreEqual(expectedResult, result, "5040 ^ 12  =  268637376395543538746286686601216000000000000");
		}

		[TestMethod]
		public void TestSqrt()
		{
			BigInteger expectedResult = BigInteger.Parse("8145408529");
			// sqrt(66347680104305943841) = 8145408529   

			BigInteger squareNumber = BigInteger.Parse("66347680104305943841");
			BigInteger remainder = new BigInteger();
			BigInteger result = BigIntegerHelper.NthRoot(squareNumber, 2, ref remainder);

			Assert.AreEqual(expectedResult, result, "sqrt(66347680104305943841) = 8145408529");
		}
	}
}
