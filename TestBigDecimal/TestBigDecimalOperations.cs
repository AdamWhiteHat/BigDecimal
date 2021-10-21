namespace TestBigDecimal {

	using System;
	using System.Numerics;
	using ExtendedNumerics;
	using NUnit.Framework;

	[TestFixture]
	public class TestBigDecimalOperations {

		[Test]
		public void TestNegate() {
			var expected = BigDecimal.Parse( "1.375" );
			var actual = BigDecimal.Negate( expected );

			Assert.AreEqual( "-1.375", actual.ToString() );
		}

		[Test]
		public void TestAddition() {
			var number1 = BigDecimal.Parse( "1234567890" );
			var expected = BigDecimal.Parse( "3382051537" );

			var actual = number1 + 2147483647;

			Assert.AreEqual( expected, actual );
		}

		[Test]
		public void TestSubtraction() {
			var number = BigDecimal.Parse( "4294967295" );
			var expected = BigDecimal.Parse( "2147483648" );

			var actual = number - 2147483647;

			Assert.AreEqual( expected, actual );
		}

		[Test]
		public void TestMultiply() {
			var expectedResult1 = BigDecimal.Parse( "35794234179725868774991807832568455403003778024228226193532908190484670252364677411513516111204504060317568667" );
			var expectedResult2 = BigDecimal.Parse( "37484040009320200288159018961010536937973891182532366282540247408867702983313960194873589374267102044942786001" );
			var expectedResult3 =
				new BigDecimal( BigInteger.Negate(
					BigInteger.Parse( "61199804023616162130466158636504166524066189692091806226423722790866248079929810268920239053350152436663869784" ) ) );

			//"6119980402361616213046615863650416652406618969209180622642372279086624807992.9810268920239053350152436663869784"

			//expectedResult3.Truncate();
			//BigDecimal.Normalize( expectedResult3 );

			var p = BigDecimal.Parse( "6122421090493547576937037317561418841225758554253106999" );
			var q = BigDecimal.Parse( "5846418214406154678836553182979162384198610505601062333" );

			var result1 = BigDecimal.Multiply( p, q );
			var result2 = p * p;
			var result3 = -1 * p * new BigDecimal( BigInteger.Parse( "9996013524558575221488141657137307396757453940901242216" ), -34 );

			// -1 * 6122421090493547576937037317561418841225758554253106999  *   999601352455857522148.8141657137307396757453940901242216
			// = -6119980402361616213046615863650416652406618969209180622642372279086624807992.9810268920239053350152436663869784
			//                                                                   9996013524558575221488141657137307396757453940901242216

			var matches1 = expectedResult1.Equals( result1 );
			var matches2 = expectedResult2.Equals( result2 );
			var matches3 = expectedResult3.ToString().Equals( result3.ToString().Replace( ".", "" ), StringComparison.Ordinal );

			Assert.IsTrue( matches1 );
			Assert.IsTrue( matches2 );
			Assert.IsTrue( matches3 );
		}

		[Test]
		public void TestDivide000() {
			var dividend = BigDecimal.Parse( "0.63" );
			var divisor = BigDecimal.Parse( "0.09" );

			var actual = BigDecimal.Divide( dividend, divisor );
			var expected = BigDecimal.Parse( "7" );

			Assert.AreEqual( expected, actual );
		}

		[Test]
		public void TestDivide001() {
			var expected = BigDecimal.Parse( "40094690950920881030683735292761468389214899724061" );

			var dividend = BigDecimal.Parse( "1522605027922533360535618378132637429718068114961380688657908494580122963258952897654000350692006139" );
			var divisor = BigDecimal.Parse( "37975227936943673922808872755445627854565536638199" );

			var actual = BigDecimal.Divide( dividend, divisor );

			Assert.AreEqual( expected, actual );
		}

		[Test]
		public void TestDivide002() {

			var resultDividend = BigDecimal.Parse( "0.001" );
			var resultDivisor = BigDecimal.Parse( "0.5" );
			var expectedQuotientResult = BigDecimal.Parse( "0.002" );

			var quotientResult = BigDecimal.Divide( resultDividend, resultDivisor );

			Assert.AreEqual( expectedQuotientResult, quotientResult );
		}

		[Test]
		public void TestDivide003() {
			var divisor = BigDecimal.Parse( "0.90606447789" );
			var actual = BigDecimal.Divide( BigDecimal.One, divisor );
			actual = BigDecimal.Truncate( actual, 100 );
			var expected = BigDecimal.Parse( "1.1036742134828557" );

			Assert.AreEqual( expected, actual );
		}

		[Test]
		public void TestDivide004() {
			var twenty = new BigDecimal( 20 );
			var actual = BigDecimal.Divide( BigDecimal.One, twenty );
			var expected = BigDecimal.Parse( "0.05" );

			Assert.AreEqual( expected, actual );
		}

		[Test]
		public void TestDivide005A() {
			var actual = BigDecimal.Divide( BigDecimal.Parse( "0.5" ), BigDecimal.Parse( "0.01" ) );
			var expected = BigDecimal.Parse( "50" );

			Assert.AreEqual( expected, actual );
		}

		[Test]
		public void TestDivide005B() {
			var actual = BigDecimal.Divide( BigDecimal.Parse( "0.5" ), BigDecimal.Parse( "0.1" ) );
			var expected = BigDecimal.Parse( "5" );

			Assert.AreEqual( expected, actual );
		}

		[Test]
		public void TestDivide005C() {
			var actual = BigDecimal.Divide( BigDecimal.Parse( "0.60" ), BigDecimal.Parse( "0.01" ) );
			var expected = BigDecimal.Parse( "60" );

			Assert.AreEqual( expected, actual );
		}

		[Test]
		public void TestReciprocal001() {
			var dividend = new BigDecimal( 1 );
			var divisor = new BigDecimal( 3 );

			var actual = BigDecimal.Divide( dividend, divisor );
			var expected = BigDecimal.Parse( "0.3333333333333333" );

			Assert.AreEqual( expected, actual );
		}

		[Test]
		public void TestReciprocal003() {
			var expected = BigDecimal.Parse( "12.000000000000005" );

			var dividend = new BigDecimal( 1 );
			var divisor = BigDecimal.Parse( "0.0833333333333333" );

			var actual = BigDecimal.Divide( dividend, divisor );

			Assert.AreEqual( expected, actual );
		}

		[Test]
		public void TestReciprocal004() {
			// 2/0.63661977236758 == 3.1415926535898
			var expected = BigDecimal.Parse( "3.14159265358970" );

			var dividend = new BigDecimal( 2 );
			var divisor = BigDecimal.Parse( "0.63661977236758" );

			var actual = BigDecimal.Divide( dividend, divisor );

			Assert.AreEqual( expected, actual );
		}

		[Test]
		public void TestReciprocal002() {
			// 1/2 = 0.5
			var expected = BigDecimal.Parse( "0.5" );

			var dividend = new BigDecimal( 1 );
			var divisor = new BigDecimal( 2 );

			var actual = BigDecimal.Divide( dividend, divisor );

			Assert.AreEqual( expected, actual );
		}

		[Test]
		public void TestMod() {
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

			var result1 = BigDecimal.Mod( dividend1, divisor1 );
			var result2 = BigDecimal.Mod( dividend2, divisor2 );
			var result3 = BigDecimal.Mod( dividend3, divisor3 );
			//BigDecimal result4 = BigDecimal.Mod(dividend4,divisor4);

			Assert.AreEqual( expectedResult1, result1 );
			Assert.AreEqual( expectedResult2, result2 );
			Assert.AreEqual( expectedResult3, result3 );
			//Assert.AreEqual(expectedResult4, result4);
		}

		[Test]
		public void TestCeiling001() {
			const String expectedCeiling = "4";
			const String expectedStart = "3.14159265";

			var start = BigDecimal.Parse( expectedStart );
			var actualStart = start.ToString();

			Assert.AreEqual( expectedStart, actualStart );

			var ceiling = BigDecimal.Ceiling( start );
			var actualCeiling = ceiling.ToString();

			Assert.AreEqual( expectedCeiling, actualCeiling, $"ceiling({expectedStart}) == {expectedCeiling}" );
		}

		[Test]
		public void TestCeiling002() {
			const String expectedCeiling = "-3";
			const String expectedStart = "-3.14159265";

			var start = BigDecimal.Parse( expectedStart );
			var actualStart = start.ToString();

			Assert.AreEqual( expectedStart, actualStart );

			var ceiling = BigDecimal.Ceiling( start );
			var actualCeiling = ceiling.ToString();

			Assert.AreEqual( expectedCeiling, actualCeiling, $"ceiling({expectedStart}) == {expectedCeiling}" );
		}

		[Test]
		public void TestCeiling003() {
			const String expectedCeiling = "1";
			const String expectedStart = "0.14159265";

			var start = BigDecimal.Parse( expectedStart );
			var actualStart = start.ToString();

			Assert.AreEqual( expectedStart, actualStart );

			var ceiling = BigDecimal.Ceiling( start );
			var actualCeiling = ceiling.ToString();

			Assert.AreEqual( expectedCeiling, actualCeiling, $"ceiling({expectedStart}) == {expectedCeiling}" );
		}

		[Test]
		public void TestCeiling004() {
			const String expectedCeiling = "0";
			const String expectedStart = "-0.14159265";

			var start = BigDecimal.Parse( expectedStart );
			var actualStart = start.ToString();

			Assert.AreEqual( expectedStart, actualStart );

			var ceiling = BigDecimal.Ceiling( start );
			var actualCeiling = ceiling.ToString();

			Assert.AreEqual( expectedCeiling, actualCeiling, $"ceiling({expectedStart}) == {expectedCeiling}" );
		}

		[Test]
		public void TestFloor001() {
			const String expectedFloor = "3";
			const String expectedStart = "3.14159265";

			var start = BigDecimal.Parse( expectedStart );
			var actualStart = start.ToString();

			Assert.AreEqual( expectedStart, actualStart );

			var floor = BigDecimal.Floor( start );
			var actualFloor = floor.ToString();

			Assert.AreEqual( expectedFloor, actualFloor, $"ceiling({expectedStart}) == {expectedFloor}" );
		}

		[Test]
		public void TestFloor002() {
			const String expectedFloor = "-4";
			const String expectedStart = "-3.14159265";

			var start = BigDecimal.Parse( expectedStart );
			var actualStart = start.ToString();

			Assert.AreEqual( expectedStart, actualStart );

			var floor = BigDecimal.Floor( start );
			var actualFloor = floor.ToString();

			Assert.AreEqual( expectedFloor, actualFloor, $"ceiling({expectedStart}) == {expectedFloor}" );
		}

		[Test]
		public void TestFloor003() {
			const String expectedFloor = "-1";
			const String expectedStart = "-0.14159265";

			var start = BigDecimal.Parse( expectedStart );
			var actualStart = start.ToString();

			Assert.AreEqual( expectedStart, actualStart );

			var floor = BigDecimal.Floor( start );
			var actualFloor = floor.ToString();

			Assert.AreEqual( expectedFloor, actualFloor, $"ceiling({expectedStart}) == {expectedFloor}" );
		}

		[Test]
		public void TestFloor004() {
			const String expectedFloor = "0";
			const String expectedStart = "0.14159265";

			var start = BigDecimal.Parse( expectedStart );
			var actualStart = start.ToString();

			Assert.AreEqual( expectedStart, actualStart );

			var floor = BigDecimal.Floor( start );
			var actualFloor = floor.ToString();

			Assert.AreEqual( expectedFloor, actualFloor, $"ceiling({expectedStart}) == {expectedFloor}" );
		}

		[Test]
		public void TestBigDecimalPow() {
			var expected = BigDecimal.Parse( "268637376395543538746286686601216000000000000" );
			// 5040 ^ 12  =  268637376395543538746286686601216000000000000

			var number = BigDecimal.Parse( "5040" );
			var actual = BigDecimal.Pow( number, 12 );

			Assert.AreEqual( expected, actual, "5040 ^ 12  =  268637376395543538746286686601216000000000000" );
		}

		[Test]
		public void TestSqrt() {
			var expected = BigInteger.Parse( "8145408529" );
			// sqrt(66347680104305943841) = 8145408529

			var squareNumber = BigInteger.Parse( "66347680104305943841" );

			var actual = squareNumber.NthRoot( 2, out var remainder );

			Assert.AreEqual( expected, actual, "sqrt(66347680104305943841) = 8145408529" );
		}

	}

}