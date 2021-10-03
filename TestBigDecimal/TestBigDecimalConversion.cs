namespace TestBigDecimal {

	using System.Numerics;
	using ExtendedNumerics;
	using NUnit.Framework;

	[TestFixture]
	public class TestBigDecimalConversion {


		[Test]
		public void TestConversionFromBigInteger() {
			var expectedResult = BigDecimal.Parse( "22685077023948547418271375393606809233149150201282920942551781108927727789384397020382853" );

			var result = ( BigDecimal )BigInteger.Parse( "22685077023948547418271375393606809233149150201282920942551781108927727789384397020382853" ); ;

			Assert.AreEqual( expectedResult, result );
		}

		[Test]
		public void TestConversionToBigInteger() {
			var expectedResult = BigInteger.Parse( "213212221322233233332232232223" );

			var result = ( BigInteger )BigDecimal.Parse( "213212221322233233332232232223" );

			Assert.AreEqual( expectedResult, result );
		}
	}
}
