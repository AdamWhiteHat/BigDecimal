using System;
using System.Numerics;
using AJRLibray.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestBigDecimal
{
	[TestClass]
	public class TestBigDecimalConversion
	{
		[TestMethod]
		public void TestConstructor()
		{
			BigDecimal a = 0;
			BigDecimal b = new BigDecimal(0, 0);
			BigDecimal c = new BigDecimal(123456789);
			BigDecimal d = new BigDecimal(3141592793238462, -15);
			BigDecimal e = new BigDecimal(1000000000, -25);
			BigDecimal f = new BigDecimal(-1, -2);
			BigDecimal g = new BigDecimal(0, -1);
			BigDecimal h = new BigDecimal(-0.0012345);
			BigDecimal i = 0.05d;

			Assert.IsInstanceOfType(a, typeof(BigDecimal), "Constructor parameters: '0'");
			Assert.IsInstanceOfType(b, typeof(BigDecimal), "Constructor parameters: '0, 0'");
			Assert.IsInstanceOfType(c, typeof(BigDecimal), "Constructor parameters: '123456789'");
			Assert.IsInstanceOfType(d, typeof(BigDecimal), "Constructor parameters: '3141592793238462, -15'");
			Assert.IsInstanceOfType(e, typeof(BigDecimal), "Constructor parameters: '1000000000, -25'");
			Assert.IsInstanceOfType(f, typeof(BigDecimal), "Constructor parameters: '-1, -2'");
			Assert.IsInstanceOfType(g, typeof(BigDecimal), "Constructor parameters: ' 0, -1'");
			Assert.IsInstanceOfType(h, typeof(BigDecimal), "Constructor parameters: '-0.0012345'");
			Assert.AreEqual(0, a);
			Assert.AreEqual(0, b);
			Assert.AreEqual(123456789, c);
			Assert.AreEqual(-0.01d, f);
			Assert.AreEqual("-0.0", g.ToString());
			Assert.AreEqual(-0.0012345, h);
			Assert.AreEqual(0.05d, i);
		}

		[TestMethod]
		public void TestConversionFromBigInteger()
		{
			BigDecimal expectedResult = BigDecimal.Parse("22685077023948547418271375393606809233149150201282920942551781108927727789384397020382853");

			BigDecimal result = (BigDecimal)BigInteger.Parse("22685077023948547418271375393606809233149150201282920942551781108927727789384397020382853"); ;

			Assert.AreEqual(expectedResult, result);
		}

		[TestMethod]
		public void TestConversionToBigInteger()
		{
			BigInteger expectedResult = BigInteger.Parse("213212221322233233332232232223");

			BigInteger result = (BigInteger)BigDecimal.Parse("213212221322233233332232232223");

			Assert.AreEqual(expectedResult, result);
		}
	}
}
