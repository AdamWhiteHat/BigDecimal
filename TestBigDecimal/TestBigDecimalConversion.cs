using System;
using System.Numerics;
using ExtendedNumerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestBigDecimal
{
	[TestClass]
	public class TestBigDecimalConversion
	{
		private TestContext m_testContext;
		public TestContext TestContext { get { return m_testContext; } set { m_testContext = value; } }

		[ClassInitialize()]
		public static void Initialize(TestContext context)
		{
			BigDecimal.Precision = 5000;
			BigDecimal.AlwaysTruncate = false;
		}

		[TestProperty("Basic", "Conversion")]
		[TestMethod]
		public void TestConversionFromBigInteger()
		{
			BigDecimal expectedResult = BigDecimal.Parse("22685077023948547418271375393606809233149150201282920942551781108927727789384397020382853");

			BigDecimal result = (BigDecimal)BigInteger.Parse("22685077023948547418271375393606809233149150201282920942551781108927727789384397020382853"); ;

			Assert.AreEqual(expectedResult, result);
		}

		[TestProperty("Basic", "Conversion")]
		[TestMethod]
		public void TestConversionToBigInteger()
		{
			BigInteger expectedResult = BigInteger.Parse("213212221322233233332232232223");

			BigInteger result = (BigInteger)BigDecimal.Parse("213212221322233233332232232223");

			Assert.AreEqual(expectedResult, result);
		}
	}
}
