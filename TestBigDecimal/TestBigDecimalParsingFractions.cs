using System;
using System.Globalization;
using System.Threading;
using ExtendedNumerics;
using ExtendedNumerics.Helpers;
using NUnit.Framework;

namespace TestBigDecimal
{

	[TestFixture]
	[Culture("en-US,ru-RU")]
	public class TestBigDecimalParsingFractions
	{

		private static readonly String String123450000;

		private NumberFormatInfo Format { get { return Thread.CurrentThread.CurrentCulture.NumberFormat; } }

		static TestBigDecimalParsingFractions() => String123450000 = "123450000" + new String('0', UInt16.MaxValue);

		[Test]
		public void TestFractional1()
		{
			int savePrecision = BigDecimal.Precision;
			BigDecimal.Precision = 10;

			string test = TestBigDecimalHelper.PrepareValue(" 12.1 / 36.3 ", this.Format);
			var val = TestBigDecimalHelper.PrepareValue("0.3333333333", this.Format);
			var expected = BigDecimal.Parse(val);

			var parsed = test.TryParseFraction(out var result);
			if (parsed)
			{
				Assert.AreEqual(expected, result);
			}
			else
			{
				Assert.Fail("Only 3 3s.");
			}

			BigDecimal.Precision = savePrecision;
		}

		[Test]
		public void TestNormalize123450000UInt16()
		{
			var expected = new BigDecimal(123450000, UInt16.MaxValue);
			var bd = BigDecimal.Parse(String123450000);

			Assert.AreEqual(expected, bd);
		}
	}
}