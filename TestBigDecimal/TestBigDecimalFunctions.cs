using System;
using System.Numerics;
using AJRLibray.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestBigDecimal
{
	[TestClass]
	public class TestBigDecimalFunctions
	{
		[TestMethod]
		public void TestGetLength()
		{
			BigDecimal expectedResult = BigDecimal.Parse("2268507702394854741827137539360680923314");
			BigDecimal value = new BigDecimal(BigInteger.Parse("22685077023948547418271375393606809233149150201282920942551781108927727789384397020382853"), -49);

			BigDecimal result = value.WholeValue;

			Assert.AreEqual(expectedResult, result);
		}

		[TestMethod]
		public void TestSignifigantDigits()
		{
			int expectedResult1 = 19;
			int expectedResult2 = 9;

			BigDecimal number1 = new BigDecimal(12345678901234567890, -10);
			BigDecimal number2 = new BigDecimal(123456789, 1);

			int result1 = number1.SignifigantDigits;
			int result2 = number2.SignifigantDigits;

			bool results1 = expectedResult1.Equals(result1);
			bool results2 = expectedResult2.Equals(result2);

			Assert.IsTrue(results1);
			Assert.IsTrue(results2);
		}

		[TestMethod]
		public void TestGetWholeValue()
		{
			BigDecimal expectedResult = BigDecimal.Parse("2268507702394854741827137539360680923314");
			BigDecimal value = new BigDecimal(BigInteger.Parse("22685077023948547418271375393606809233149150201282920942551781108927727789384397020382853"), -49);

			BigDecimal result = value.WholeValue;

			Assert.AreEqual(expectedResult.ToString(), result.ToString());
		}

		[TestMethod]
		public void TestGetFractionalPart()
		{
			BigDecimal expectedResult = new BigDecimal(BigInteger.Parse("9150201282920942551781108927727789384397020382853"), -49);
			BigDecimal value = new BigDecimal(BigInteger.Parse("22685077023948547418271375393606809233149150201282920942551781108927727789384397020382853"), -49);

			BigDecimal result = value.GetFractionalPart();

			Assert.AreEqual<BigDecimal>(expectedResult, result);
		}

		[TestMethod]
		public void TestRounding()
		{
			BigDecimal bd = BigDecimal.Parse("10000000000000000000000000000000000000000000000000001");


			BigDecimal up = new BigDecimal(0.50001d);
			BigDecimal down = new BigDecimal(0.49d);
			BigDecimal oneAndAhalf = new BigDecimal(1.5d);

			BigDecimal negEightPointFive = new BigDecimal(-8.5d);
			BigDecimal negNinePointFive = -9.5d;

			BigDecimal threePointFourNine = new BigDecimal(3.49d);
			BigDecimal threePointFiveOne = new BigDecimal(3.51d);
			BigDecimal sixPointFive = new BigDecimal(6.5d);

			BigInteger one = BigDecimal.Round(up);
			BigInteger zero = BigDecimal.Round(down);
			BigInteger two = BigDecimal.Round(oneAndAhalf);
			BigInteger three = BigDecimal.Round(threePointFourNine);
			BigInteger four = BigDecimal.Round(threePointFiveOne);
			BigInteger six = BigDecimal.Round(sixPointFive, MidpointRounding.ToEven);
			BigInteger negEight = BigDecimal.Round(negEightPointFive, MidpointRounding.ToEven);
			BigInteger negNine = BigDecimal.Round(negEightPointFive, MidpointRounding.AwayFromZero);
			BigInteger negTen = BigDecimal.Round(negNinePointFive, MidpointRounding.ToEven);

			Assert.AreEqual(BigInteger.One, one);
			Assert.AreEqual(BigInteger.Zero, zero);
			Assert.AreEqual(2, two);
			Assert.AreEqual(3, three);
			Assert.AreEqual(4, four);
			Assert.AreEqual(6, six);
			Assert.AreEqual(-8, negEight);
			Assert.AreEqual(-9, negNine);
			Assert.AreEqual(-10, negTen);
		}

		[TestMethod]
		public void TestGetSign()
		{
			BigDecimal zero1 = 0;
			BigDecimal zero2 = new BigDecimal();
			BigDecimal zero3 = new BigDecimal(0);
			BigDecimal zero4 = new BigDecimal(BigInteger.Zero);
			BigDecimal zero5 = new BigDecimal(0, -1);
			BigDecimal zero6 = BigInteger.Subtract(BigInteger.Add(BigInteger.Divide(2, 3), BigInteger.Multiply(-1, BigInteger.Divide(1, 3))), BigInteger.Divide(1, 3));
			BigDecimal zero7 = BigDecimal.Add(BigDecimal.Divide(1, 10), -0.1d);
			BigDecimal zero8 = BigDecimal.Add((new BigDecimal(1, -1)), ((double)-1 / 10));
			BigDecimal zero9 = (new BigDecimal(15274, -7) * 0);

			BigDecimal positive1 = 1;
			BigDecimal positive2 = -1 * -1;

			BigDecimal negative1 = BigDecimal.Multiply(BigDecimal.One, BigDecimal.MinusOne);
			BigDecimal negative2 = BigDecimal.Subtract(BigDecimal.Zero, 3);
			BigDecimal negative3 = BigInteger.Subtract(0, 3);
			BigDecimal negative4 = 10 * -1;

			Assert.AreEqual(0, zero1.Sign, "0");
			Assert.AreEqual(0, zero2.Sign, "new BigDecimal()");
			Assert.AreEqual(0, zero3.Sign, "new BigDecimal(0);");
			Assert.AreEqual(0, zero4.Sign, "new BigDecimal(BigInteger.Zero)");
			Assert.AreEqual(0, zero5.Sign, "new BigDecimal(0, -1);");
			Assert.AreEqual(0, zero6.Sign, "2/3  -1/3 - 1/3");
			Assert.AreEqual(0, zero7.Sign, "1/10 + -1/10");
			Assert.AreEqual(0, zero8.Sign, "1 + -1/10");
			Assert.AreEqual(0, zero9.Sign, "0.0015274");

			Assert.AreEqual(1, positive1.Sign, "1");
			Assert.AreEqual(1, positive2.Sign, "-1 * 1");

			Assert.AreEqual(BigInteger.MinusOne, negative1.Sign, "1 * -1");
			Assert.AreEqual(BigInteger.MinusOne, negative2.Sign, "0 - 3");
			Assert.AreEqual(BigInteger.MinusOne, negative3.Sign, "BigInteger.Subtract(0, 3)");
			Assert.AreEqual(BigInteger.MinusOne, negative4.Sign, "10 * -1;");
		}

		[TestMethod]
		public void TestLCD()
		{
			BigDecimal expectedResult = BigDecimal.Parse("45319990731015");

			BigDecimal result = BigIntegerHelper.LCM(
				new BigInteger[] {
					3, 5, 7, 11, 13, 101, 307, 311, 313
				});

			// 15015,
			// lcm(3, 5, 7, 11, 13, 101, 307, 311, 313) = 45319990731015
			// lcm(4973, 4292, 4978, 4968, 4297, 4287)  = 2822891742340306560

			Assert.AreEqual(expectedResult, result);
		}

		[TestMethod]
		public void TestGCD()
		{
			BigDecimal expectedResult = BigDecimal.Parse("10");

			BigDecimal result = BigIntegerHelper.GCD(new BigInteger[] { 20, 30, 210, 310, 360, 5040, 720720 });

			Assert.AreEqual(expectedResult, result);
		}
	}
}
