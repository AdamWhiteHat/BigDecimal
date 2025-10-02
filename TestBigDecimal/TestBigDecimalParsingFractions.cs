using System.Globalization;
using System.Threading;
using ExtendedNumerics;
using ExtendedNumerics.Helpers;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace BigDecimalTests;

[TestFixture]
public class TestBigDecimalParsingFractions
{
	private static readonly string String123450000 = "123450000" + new string('0', ushort.MaxValue);

	private NumberFormatInfo Format => Thread.CurrentThread.CurrentCulture.NumberFormat;

	[Test]
	public void TestFractional1()
	{
		int savePrecision = BigDecimal.Precision;
		BigDecimal.Precision = 10;

		string test = TestBigDecimalHelper.PrepareValue(" 12.1 / 36.3 ", Format);
		var val = TestBigDecimalHelper.PrepareValue("0.3333333333", Format);
		var expected = BigDecimal.Parse(val);

		var parsed = test.TryParseFraction(out var result);
		if (parsed)
		{
			ClassicAssert.AreEqual(expected, result);
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
		var expected = new BigDecimal(123450000, ushort.MaxValue);
		var bd = BigDecimal.Parse(String123450000);

		ClassicAssert.AreEqual(expected, bd);
	}
}