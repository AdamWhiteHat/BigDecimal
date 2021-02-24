using System;
using System.Numerics;
using ExtendedNumerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace TestBigDecimal
{
	[TestClass]
	public class TestBigDecimalOperations
	{
		private TestContext m_testContext;
		public TestContext TestContext { get { return m_testContext; } set { m_testContext = value; } }

		[ClassInitialize()]
		public static void Initialize(TestContext context)
		{
			BigDecimal.Precision = 5000;
			BigDecimal.AlwaysTruncate = false;
		}

		[TestProperty("Arithmetic", "Operations")]
		[TestMethod]
		public void TestNegate()
		{
			string expected = "-1.375";

			BigDecimal result = BigDecimal.Negate((double)(1.375));

			Assert.AreEqual(expected, result.ToString());
		}

		[TestProperty("Arithmetic", "Operations")]
		[TestMethod]
		public void TestAddition()
		{
			BigDecimal number1 = BigDecimal.Parse("1234567890");
			BigDecimal expectedResult = BigDecimal.Parse("3382051537");

			BigDecimal result = number1 + 2147483647;

			Assert.AreEqual(expectedResult, result);
		}

		[TestProperty("Arithmetic", "Operations")]
		[TestMethod]
		public void TestSubtraction()
		{
			BigDecimal number = BigDecimal.Parse("4294967295");
			BigDecimal expectedResult = BigDecimal.Parse("2147483648");

			BigDecimal result = number - 2147483647;

			Assert.AreEqual(expectedResult, result);
		}

		[TestProperty("Arithmetic", "Operations")]
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

		[TestProperty("Arithmetic", "Divide")]
		[TestMethod]
		public void TestDivide000()
		{
			BigDecimal expectedResult = BigDecimal.Parse("7");

			BigDecimal dividend = BigDecimal.Parse("0.63");
			BigDecimal divisor = BigDecimal.Parse("0.09");

			BigDecimal result = BigDecimal.Divide(dividend, divisor);

			string expected = expectedResult.ToString();
			string actual = result.ToString();

			Assert.AreEqual(expected, actual);
		}

		[TestProperty("Arithmetic", "Divide")]
		[TestMethod]
		public void TestDivide001()
		{
			BigDecimal expectedResult = BigDecimal.Parse("40094690950920881030683735292761468389214899724061");

			BigDecimal dividend = BigDecimal.Parse("1522605027922533360535618378132637429718068114961380688657908494580122963258952897654000350692006139");
			BigDecimal divisor = BigDecimal.Parse("37975227936943673922808872755445627854565536638199");

			BigDecimal result = BigDecimal.Divide(dividend, divisor);

			string expected = expectedResult.ToString();
			string actual = result.ToString();

			Assert.AreEqual(expected, actual);
		}

		[TestProperty("Arithmetic", "Divide")]
		[TestMethod]
		public void TestDivide002()
		{
			string expectedResultDividend = "0.001";
			string expectedResultDivisor = "0.5";

			string expectedQuotientResult = "0.002";

			BigDecimal resultDividend = BigDecimal.Parse(expectedResultDividend);
			BigDecimal resultDivisor = BigDecimal.Parse(expectedResultDivisor);

			resultDividend.Normalize();
			resultDivisor.Normalize();

			BigDecimal quotientResult = BigDecimal.Divide(resultDividend, resultDivisor);
			quotientResult.Normalize();

			string actualDividend = resultDividend.ToString();
			string actualDivisor = resultDivisor.ToString();
			string actualQuotientResult = quotientResult.ToString();

			Assert.AreEqual(expectedResultDividend, actualDividend);
			Assert.AreEqual(expectedResultDivisor, actualDivisor);
			Assert.AreEqual(expectedQuotientResult, actualQuotientResult);
		}

		[TestProperty("Arithmetic", "Divide")]
		[TestMethod]
		public void TestDivide003()
		{
			string expected = "1.10367421348286";

			BigDecimal divisor = BigDecimal.Parse("0.90606447789");
			BigDecimal result = BigDecimal.Divide(BigDecimal.One, divisor);

			result.Truncate(100);
			string actual = result.ToString();

			Assert.AreEqual(expected, actual);
		}

		[TestProperty("Arithmetic", "Divide")]
		[TestMethod]
		public void TestDivide004()
		{
			BigDecimal expectedResult = BigDecimal.Parse("0.05");

			BigDecimal one = new BigDecimal(1);
			BigDecimal twenty = new BigDecimal(20);
			BigDecimal result = BigDecimal.Divide(one, twenty);

			string expected = expectedResult.ToString();
			string actual = result.ToString();

			Assert.AreEqual(expected, actual);
		}

		[TestProperty("Arithmetic", "Divide")]
		[TestMethod]
		public void TestDivide005A()
		{
			BigDecimal expectedResult3 = BigDecimal.Parse("50");

			BigDecimal result3 = BigDecimal.Divide(BigDecimal.Parse("0.5"), BigDecimal.Parse("0.01"));

			string expected3 = expectedResult3.ToString();
			string actual3 = result3.ToString();

			Assert.AreEqual(expected3, actual3);
		}

		[TestProperty("Arithmetic", "Divide")]
		[TestMethod]
		public void TestDivide005B()
		{
			BigDecimal expectedResult3 = BigDecimal.Parse("5");

			BigDecimal result3 = BigDecimal.Divide(BigDecimal.Parse("0.5"), BigDecimal.Parse("0.1"));

			string expected3 = expectedResult3.ToString();
			string actual3 = result3.ToString();

			Assert.AreEqual(expected3, actual3);
		}

		[TestProperty("Arithmetic", "Divide")]
		[TestMethod]
		public void TestDivide005C()
		{
			BigDecimal expectedResult3 = BigDecimal.Parse("5");

			BigDecimal result3 = BigDecimal.Divide(BigDecimal.Parse("0.05"), BigDecimal.Parse("0.01"));

			string expected3 = expectedResult3.ToString();
			string actual3 = result3.ToString();

			Assert.AreEqual(expected3, actual3);
		}


		[TestProperty("Arithmetic", "Divide")]
		[TestMethod]
		public void TestDivide005D()
		{
			BigDecimal expectedResult3 = BigDecimal.Parse("0.5");

			BigDecimal result3 = BigDecimal.Divide(BigDecimal.Parse("0.05"), BigDecimal.Parse("0.1"));

			string expected3 = expectedResult3.ToString();
			string actual3 = result3.ToString();

			Assert.AreEqual(expected3, actual3);
		}


		[TestProperty("Arithmetic", "Operations")]
		[TestMethod]
		public void TestReciprocal001()
		{
			// 1 / 3 = 0.333333333333333
			BigDecimal expectedResult = BigDecimal.Parse("0.333333333333333");

			BigDecimal dividend = new BigDecimal(1);
			BigDecimal divisor = new BigDecimal(3);

			BigDecimal result = BigDecimal.Divide(dividend, divisor);

			string expected = expectedResult.ToString();
			string actual = result.ToString();

			Assert.AreEqual(expected, actual);
		}

		[TestProperty("Arithmetic", "Operations")]
		[TestMethod]
		public void TestReciprocal003()
		{
			// 1/0.0833333333333333 == 12
			BigDecimal expectedResult = BigDecimal.Parse("12");

			BigDecimal dividend = new BigDecimal(1);
			BigDecimal divisor = BigDecimal.Parse("0.0833333333333333");

			BigDecimal result = BigDecimal.Divide(dividend, divisor);

			string expected = expectedResult.ToString();
			string actual = result.ToString();

			Assert.AreEqual(expected, actual);
		}

		[TestProperty("Arithmetic", "Operations")]
		[TestMethod]
		public void TestReciprocal004()
		{
			// 2/0.63661977236758 == 3.1415926535898
			BigDecimal expectedResult = BigDecimal.Parse("3.14159265358970");

			BigDecimal dividend = new BigDecimal(2);
			BigDecimal divisor = BigDecimal.Parse("0.63661977236758");

			BigDecimal result = BigDecimal.Divide(dividend, divisor);

			string expected = expectedResult.ToString();
			string actual = result.ToString();

			Assert.AreEqual(expected, actual);
		}

		[TestProperty("Arithmetic", "Operations")]
		[TestMethod]
		public void TestReciprocal002()
		{
			// 1/2 = 0.5
			BigDecimal expectedResult = BigDecimal.Parse("0.5");

			BigDecimal dividend = new BigDecimal(1);
			BigDecimal divisor = new BigDecimal(2);

			BigDecimal result = BigDecimal.Divide(dividend, divisor);

			string expected = expectedResult.ToString();
			string actual = result.ToString();

			Assert.AreEqual(expected, actual);
		}

		//[Ignore]
		[TestProperty("Arithmetic", "Divide")]
		[TestMethod]
		public void TestMod()
		{
			BigDecimal expectedResult1 = 12;
			BigDecimal expectedResult2 = 0;
			BigDecimal expectedResult3 = 1;
			//BigDecimal expectedResult4 = 1.66672;


			// 31 % 19 = 12
			BigDecimal dividend1 = 31;
			BigDecimal divisor1 = 19;


			// 1891 %31 = 0
			BigDecimal dividend2 = 1891;
			BigDecimal divisor2 = 31;

			// 6661 % 60 = 1
			BigDecimal dividend3 = 6661;
			BigDecimal divisor3 = 60;


			// 31 % 3.66666 = 1.66672
			//BigDecimal dividend4 = 31;
			//BigDecimal divisor4 = 3.66666;

			BigDecimal result1 = BigDecimal.Mod(dividend1, divisor1);
			BigDecimal result2 = BigDecimal.Mod(dividend2, divisor2);
			BigDecimal result3 = BigDecimal.Mod(dividend3, divisor3);
			//BigDecimal result4 = BigDecimal.Mod(dividend4,divisor4);

			Assert.AreEqual(expectedResult1, result1);
			Assert.AreEqual(expectedResult2, result2);
			Assert.AreEqual(expectedResult3, result3);
			//Assert.AreEqual(expectedResult4, result4);
		}

		[TestProperty("Arithmetic", "Divide")]
		[TestMethod]
		public void TestCeiling001()
		{
			string expectedCeiling = "4";
			string expectedStart = "3.14159265";

			BigDecimal start = BigDecimal.Parse(expectedStart);
			string actualStart = start.ToString();

			Assert.AreEqual(expectedStart, actualStart);

			BigDecimal ceiling = BigDecimal.Ceiling(start);
			string actualCeiling = ceiling.ToString();

			Assert.AreEqual(expectedCeiling, actualCeiling, $"ceiling({expectedStart}) == {expectedCeiling}");
		}

		[TestProperty("Arithmetic", "Divide")]
		[TestMethod]
		public void TestCeiling002()
		{
			string expectedCeiling = "-3";
			string expectedStart = "-3.14159265";

			BigDecimal start = BigDecimal.Parse(expectedStart);
			string actualStart = start.ToString();

			Assert.AreEqual(expectedStart, actualStart);

			BigDecimal ceiling = BigDecimal.Ceiling(start);
			string actualCeiling = ceiling.ToString();

			Assert.AreEqual(expectedCeiling, actualCeiling, $"ceiling({expectedStart}) == {expectedCeiling}");
		}

		[TestProperty("Arithmetic", "Divide")]
		[TestMethod]
		public void TestCeiling003()
		{
			string expectedCeiling = "1";
			string expectedStart = "0.14159265";

			BigDecimal start = BigDecimal.Parse(expectedStart);
			string actualStart = start.ToString();

			Assert.AreEqual(expectedStart, actualStart);

			BigDecimal ceiling = BigDecimal.Ceiling(start);
			string actualCeiling = ceiling.ToString();

			Assert.AreEqual(expectedCeiling, actualCeiling, $"ceiling({expectedStart}) == {expectedCeiling}");
		}

		[TestProperty("Arithmetic", "Divide")]
		[TestMethod]
		public void TestCeiling004()
		{
			string expectedCeiling = "0";
			string expectedStart = "-0.14159265";

			BigDecimal start = BigDecimal.Parse(expectedStart);
			string actualStart = start.ToString();

			Assert.AreEqual(expectedStart, actualStart);

			BigDecimal ceiling = BigDecimal.Ceiling(start);
			string actualCeiling = ceiling.ToString();

			Assert.AreEqual(expectedCeiling, actualCeiling, $"ceiling({expectedStart}) == {expectedCeiling}");
		}


		[TestProperty("Arithmetic", "Divide")]
		[TestMethod]
		public void TestFloor001()
		{
			string expectedFloor = "3";
			string expectedStart = "3.14159265";

			BigDecimal start = BigDecimal.Parse(expectedStart);
			string actualStart = start.ToString();

			Assert.AreEqual(expectedStart, actualStart);

			BigDecimal floor = BigDecimal.Floor(start);
			string actualFloor = floor.ToString();

			Assert.AreEqual(expectedFloor, actualFloor, $"ceiling({expectedStart}) == {expectedFloor}");
		}

		[TestProperty("Arithmetic", "Divide")]
		[TestMethod]
		public void TestFloor002()
		{
			string expectedFloor = "-4";
			string expectedStart = "-3.14159265";

			BigDecimal start = BigDecimal.Parse(expectedStart);
			string actualStart = start.ToString();

			Assert.AreEqual(expectedStart, actualStart);

			BigDecimal floor = BigDecimal.Floor(start);
			string actualFloor = floor.ToString();

			Assert.AreEqual(expectedFloor, actualFloor, $"ceiling({expectedStart}) == {expectedFloor}");
		}

		[TestProperty("Arithmetic", "Divide")]
		[TestMethod]
		public void TestFloor003()
		{
			string expectedFloor = "-1";
			string expectedStart = "-0.14159265";

			BigDecimal start = BigDecimal.Parse(expectedStart);
			string actualStart = start.ToString();

			Assert.AreEqual(expectedStart, actualStart);

			BigDecimal floor = BigDecimal.Floor(start);
			string actualFloor = floor.ToString();

			Assert.AreEqual(expectedFloor, actualFloor, $"ceiling({expectedStart}) == {expectedFloor}");
		}

		[TestProperty("Arithmetic", "Divide")]
		[TestMethod]
		public void TestFloor004()
		{
			string expectedFloor = "0";
			string expectedStart = "0.14159265";

			BigDecimal start = BigDecimal.Parse(expectedStart);
			string actualStart = start.ToString();

			Assert.AreEqual(expectedStart, actualStart);

			BigDecimal floor = BigDecimal.Floor(start);
			string actualFloor = floor.ToString();

			Assert.AreEqual(expectedFloor, actualFloor, $"ceiling({expectedStart}) == {expectedFloor}");
		}


		[TestProperty("Arithmetic", "Operations")]
		[TestMethod]
		public void TestBigDecimalPow()
		{
			BigDecimal expectedResult = BigDecimal.Parse("268637376395543538746286686601216000000000000");
			// 5040 ^ 12  =  268637376395543538746286686601216000000000000

			BigDecimal number = BigDecimal.Parse("5040");
			BigDecimal result = BigDecimal.Pow(number, 12);

			Assert.AreEqual(expectedResult, result, "5040 ^ 12  =  268637376395543538746286686601216000000000000");
		}

		[TestProperty("Arithmetic", "Operations")]
		[TestMethod]
		public void TestSqrt()
		{
			BigInteger expectedResult = BigInteger.Parse("8145408529");
			// sqrt(66347680104305943841) = 8145408529   

			BigInteger squareNumber = BigInteger.Parse("66347680104305943841");
			BigInteger remainder = new BigInteger();
			BigInteger result = squareNumber.NthRoot(2, ref remainder);

			Assert.AreEqual(expectedResult, result, "sqrt(66347680104305943841) = 8145408529");
		}
	}
}
