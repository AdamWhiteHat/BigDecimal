namespace TestBigDecimal {

	using System;
	using System.Numerics;
	using ExtendedNumerics;
	using NUnit.Framework;

	[TestFixture]
	public class TestBigDecimalConversion {

		private const String LongNumbers =
			"22685077023934456384565345644576454645163485274775618673867785678763896936969078786987890789798927897383149150201282920942551781108927727789384397020382853" +
			"22685077023934456384565345644576454645163485274775618673867785678763896936969078786987890789798927897383149150201282920942551781108927727789384397020382853" +
			"22685077023934456384565345644576454645163485274775618673867785678763896936969078786987890789798927897383149150201282920942551781108927727789384397020382853";

		[Test]
		public void TestConversionFromBigInteger() {
			var expected = BigInteger.Parse( LongNumbers );

			var bigDecimal = BigDecimal.Parse( LongNumbers );
			var actual = bigDecimal.WholeValue;

			Assert.AreEqual( expected, actual );
		}

		[Test]
		public void TestConversionToBigInteger() {
			var expected = BigInteger.Parse( LongNumbers );
			var bigDecimal = BigDecimal.Parse( LongNumbers );
			var actual = ( BigInteger )bigDecimal;

			Assert.AreEqual( expected, actual );
		}

	}

}