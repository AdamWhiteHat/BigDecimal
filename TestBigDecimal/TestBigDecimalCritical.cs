using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading;
using ExtendedNumerics;
using NUnit.Framework;

namespace TestBigDecimal
{

	[NonParallelizable]
	[TestFixture]
	[Culture("en-US,ru-RU")]
	public class TestBigDecimalCritical
	{

		private NumberFormatInfo Format { get { return Thread.CurrentThread.CurrentCulture.NumberFormat; } }

		[Test]
		public void Test47()
		{
			var π1 = 1 * BigDecimal.π;
			var π2 = 2 * BigDecimal.π;
			var π4 = 4 * BigDecimal.π;
			var π8 = 8 * BigDecimal.π;
			var sum = π1 + π2 + π4 + π8;
			var actual = sum.WholeValue;
			var expected = (BigInteger)47;

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestConstructor0()
		{
			BigDecimal expected = 0;
			var actual = new BigDecimal(0);

			Assert.AreEqual(expected, actual);
			Assert.AreEqual("0", actual.ToString());
		}

		[Test]
		public void TestConstructor1()
		{
			BigDecimal expected = 0;
			var actual = new BigDecimal(0, 0);

			Assert.AreEqual(expected, actual);
			Assert.AreEqual("0", actual.ToString());
		}

		[Test]
		public void TestConstructor2()
		{
			BigDecimal expected = 0;
			var actual = new BigDecimal(0, 1);

			Assert.AreEqual(expected, actual);
			Assert.AreEqual("0", actual.ToString());
		}

		[Test]
		public void TestConstructor001D()
		{
			var val1 = TestBigDecimalHelper.PrepareValue("0.5", this.Format);
			var val2 = TestBigDecimalHelper.PrepareValue("0.5", this.Format);

			var i = BigDecimal.Parse(val1);
			var j = BigDecimal.Parse(val2);

			Assert.AreEqual(val1, i.ToString());
			Assert.AreEqual(val2, j.ToString());
		}

		[Test]
		public void TestConstructor_Float()
		{
			string expected1 = "0.3486328";
			float d1 = 0.3486328125f;
			TestContext.WriteLine($"R: \"{expected1}\" decimal: {d1.ToString("R")}");
			TestContext.WriteLine($"E: \"{expected1}\" decimal: {d1.ToString("E")}");
			TestContext.WriteLine($"G9: \"{expected1}\" decimal: {d1.ToString("G9")}");
			TestContext.WriteLine($"G10: \"{expected1}\" decimal: {d1.ToString("G10")}");
			TestContext.WriteLine($"F9: \"{expected1}\" decimal: {d1.ToString("F9")}");
			var actual1 = new BigDecimal(d1);

			Assert.AreEqual(expected1, actual1.ToString());
			TestContext.WriteLine($"{expected1} == {actual1}");
		}

		[Test]
		public void TestConstructor_Double()
		{
			//TestConstructor_Double("0", 0);
			//TestConstructor_Double("0", 0.0);
			////TestConstructor_Double("0", -0.0);
			//
			//TestConstructor_Double("7976931348623157", 7976931348623157);
			//TestConstructor_Double("-7976931348623157", -7976931348623157);
			//
			//TestConstructor_Double("1000000000000000", 1000000000000000);
			//TestConstructor_Double("-1000000000000000", -1000000000000000);

			TestConstructor_Double("1.7976931348623157", 1.7976931348623157);
			TestConstructor_Double("-1.7976931348623157", -1.7976931348623157);

			TestConstructor_Double("0.0000000008623157", 0.0000000008623157);
			TestConstructor_Double("-0.0000000000623157", -0.0000000000623157);

			TestConstructor_Double("0.0101010101010101", 0.0101010101010101);
			TestConstructor_Double("-0.0101010101010101", -0.0101010101010101);
		}

		private void TestConstructor_Double(string expected, Double value)
		{
			var actual = new BigDecimal(value);
			Assert.AreEqual(expected, actual.ToString());
			TestContext.WriteLine($"{expected} == {actual}");
		}

		[Test]
		public void TestConstructor001WriteLineA()
		{
			var expected1 = TestBigDecimalHelper.PrepareValue("3.141592793238462", this.Format);
			var expected2 = TestBigDecimalHelper.PrepareValue("3.141592793238462", this.Format);
			var π = (BigDecimal)3.141592793238462m;
			var d = new BigDecimal(BigInteger.Parse("3141592793238462"), -15);
			var actual1 = π.ToString();
			var actual2 = d.ToString();

			TestContext.WriteLine("π = " + actual1);
			TestContext.WriteLine("d = " + actual2);
			Assert.AreEqual(expected1, actual1);
			Assert.AreEqual(expected2, actual2);
		}

		[Test]
		public void TestCastingDecimal()
		{
			Decimal m = 0.0000000000000001m;

			var e = new BigDecimal(1000000000, -25);
			var h = (BigDecimal)m;

			TestContext.WriteLine("m = " + m);
			TestContext.WriteLine("e = " + e);
			TestContext.WriteLine("h = " + h);

			Assert.AreEqual(h.ToString(), e.ToString());
		}

		[Test]
		public void TestCastingDouble()
		{
			Double m = 0.0000000000000001d;

			var e = new BigDecimal(1000000000, -25);
			var h = (BigDecimal)m;

			TestContext.WriteLine("m = " + m);
			TestContext.WriteLine("e = " + e);
			TestContext.WriteLine("h = " + h);

			Assert.AreEqual(h.ToString(), e.ToString());
		}

		[Test]
		public void TestConstructor002()
		{
			var f = new BigDecimal(-3, -2);
			var expected = TestBigDecimalHelper.PrepareValue("-0.03", this.Format);
			Assert.AreEqual(expected, f.ToString());
		}

		[Test]
		public void TestConstructor003()
		{
			var g = new BigDecimal(0, -1);
			Assert.AreEqual("0", g.ToString());
		}

		[Test]
		public void TestConstructor004()
		{
			var value = TestBigDecimalHelper.PrepareValue("-0.0012345", this.Format);

			var h = BigDecimal.Parse(value);
			Assert.AreEqual(value, h.ToString());
		}

		[Test]
		public void TestConstructorToString123456789()
		{
			const Int32 numbers = 123456789;
			var expected = numbers.ToString();
			var actual = new BigDecimal(numbers).ToString();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestNormalizeB()
		{
			var expected = "1000000";
			BigDecimal bigDecimal = new BigDecimal(1000000000, -3);

			var actual = bigDecimal.ToString();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestParse001()
		{
			string expected = TestBigDecimalHelper.PrepareValue("0.00501", this.Format);
			var result = BigDecimal.Parse(expected);
			var actual = result.ToString();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestParse002()
		{
			var result1 = BigDecimal.Parse("");
			Assert.AreEqual(result1, BigDecimal.Zero);

			var result2 = BigDecimal.Parse("0");
			Assert.AreEqual(result2, BigDecimal.Zero);

			var result3 = BigDecimal.Parse("-0");
			Assert.AreEqual(result3, BigDecimal.Zero);
		}

		[Test]
		public void TestParse0031()
		{
			string expected = "-123456789";
			var actual = BigDecimal.Parse(expected).ToString();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestParse0032()
		{
			string expected = "123456789";
			var bigDecimal = BigDecimal.Parse(expected);
			var actual = bigDecimal.ToString();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestParse0033()
		{
			string expected = TestBigDecimalHelper.PrepareValue("1234.56789", this.Format);
			var actual = BigDecimal.Parse(expected).ToString();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestParseScientific()
		{
			string toParse = TestBigDecimalHelper.PrepareValue("123.123E-20", this.Format);
			string expected = TestBigDecimalHelper.PrepareValue("0.00000000000000000123123", this.Format);
			BigDecimal parsed = BigDecimal.Parse(toParse);
			string actual = parsed.ToString();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestParseNegativeScientific()
		{
			string toParse = TestBigDecimalHelper.PrepareValue("-123.123E-20", this.Format);
			string expected = TestBigDecimalHelper.PrepareValue("-0.00000000000000000123123", this.Format);
			BigDecimal parsed = BigDecimal.Parse(toParse);
			string actual = parsed.ToString();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestParseEpsilon()
		{
			string expected = TestBigDecimalHelper.PrepareValue("4.9406564584124654E-324", this.Format);
			BigDecimal actual = BigDecimal.Parse(expected);

			Assert.AreEqual(expected, BigDecimal.ToScientificENotation(actual));
		}

		[Test]
		public void TestParseLarge()
		{
			string expected = TestBigDecimalHelper.PrepareValue("4.9406564584124654E+324", this.Format);
			BigDecimal actual = BigDecimal.Parse(expected);

			Assert.AreEqual(expected, BigDecimal.ToScientificENotation(actual));
		}


		[Test]
		public void TestParseRoundTrip()
		{
			double dval = 0.6822871999174d;
			var val = TestBigDecimalHelper.PrepareValue("0.6822871999174", this.Format);
			var actual = BigDecimal.Parse(val);
			var expected = (BigDecimal)dval;
			Assert.AreEqual(expected, actual);
		}

		[Test]
		[NonParallelizable]
		public void TestAlwaysTruncate()
		{
			var savePrecision = BigDecimal.Precision;
			var expected1 = TestBigDecimalHelper.PrepareValue("3.1415926535", this.Format);
			var expected2 = TestBigDecimalHelper.PrepareValue("-3.1415926535", this.Format);
			var expected3 = TestBigDecimalHelper.PrepareValue("-0.0000031415", this.Format);
			var expected4 = "-3";

			var actual1 = "";
			var actual2 = "";
			var actual3 = "";
			var actual4 = "";

			try
			{
				BigDecimal.Precision = 10;
				BigDecimal.AlwaysTruncate = true;

				var val1 = TestBigDecimalHelper.PrepareValue("3.14159265358979323846264338327950288419716939937510582097494459230781640628620899862803482534211706798214808651328230664709384460955058223172535", this.Format);
				BigDecimal parsed1 = BigDecimal.Parse(val1);

				var val2 = TestBigDecimalHelper.PrepareValue("-3.14159265358979323846264338327950288419716939937510582097494459230781640628620899862803482534211706798214808651328230664709384460955058223172535", this.Format);
				BigDecimal parsed2 = BigDecimal.Parse(val2);

				var val3 = TestBigDecimalHelper.PrepareValue("-0.00000314159265358979323846264338327950288419716939937510582097494459230781640628620899862803482534211706798214808651328230664709384460955058223172535", this.Format);
				BigDecimal parsed3 = BigDecimal.Parse(val3);

				BigDecimal parsed4 = BigDecimal.Parse("-3");

				actual1 = parsed1.ToString();
				actual2 = parsed2.ToString();
				actual3 = parsed3.ToString();
				actual4 = parsed4.ToString();
			}
			finally
			{
				BigDecimal.Precision = savePrecision;
				BigDecimal.AlwaysTruncate = false;
			}

			Assert.AreEqual(expected1, actual1, "#: 1");
			Assert.AreEqual(expected2, actual2, "#: 2");
			Assert.AreEqual(expected3, actual3, "#: 3");
			Assert.AreEqual(expected4, actual4, "#: 4");
			Assert.AreEqual(5000, BigDecimal.Precision, "Restore Precision to 5000");
		}

		[Test]
		[NonParallelizable]
		public void TestTruncateOnAllArithmeticOperations()
		{
			var savePrecision = BigDecimal.Precision;

			BigDecimal mod1 = BigDecimal.Parse("3141592653589793238462643383279502");
			BigDecimal mod2 = BigDecimal.Parse("27182818284590452");
			//BigDecimal neg1 = BigDecimal.Parse("-3.141592653589793238462643383279502884197169399375105820974944592307816406286208998628034825342117067982148086513282306647");

			var val1 = TestBigDecimalHelper.PrepareValue("3.141592653589793238462643383279502884197169399375105820974944592307816406286208998628034825342117067982148086513282306647", this.Format);
			BigDecimal lrg1 = BigDecimal.Parse(val1);

			var val2 = TestBigDecimalHelper.PrepareValue("2.718281828459045235360287471352662497757247093699959574966967", this.Format);
			BigDecimal lrg2 = BigDecimal.Parse(val2);

			var expected1 = TestBigDecimalHelper.PrepareValue("5.859874482", this.Format);
			var expected2 = TestBigDecimalHelper.PrepareValue("0.423310825", this.Format);
			var expected3 = TestBigDecimalHelper.PrepareValue("8.539734222", this.Format);
			var expected4 = TestBigDecimalHelper.PrepareValue("0.865255979", this.Format);
			var expected5 = TestBigDecimalHelper.PrepareValue("9.869604401", this.Format);
			var expected6 = TestBigDecimalHelper.PrepareValue("148.4131591", this.Format);
			var expected7 = "80030773195";
			var expected8 = TestBigDecimalHelper.PrepareValue("-3.14159265", this.Format);
			var expected9 = "3";
			var expected10 = "4";
			var expected11 = TestBigDecimalHelper.PrepareValue("3.141592653", this.Format);

			var actual1 = "";
			var actual2 = "";
			var actual3 = "";
			var actual4 = "";
			var actual5 = "";
			var actual6 = "";
			var actual7 = "";
			var actual8 = "";
			var actual9 = "";
			var actual10 = "";
			var actual11 = "";

			try
			{
				BigDecimal.Precision = 50;
				BigDecimal.AlwaysTruncate = false;

				TestContext.WriteLine($"E = {BigDecimal.E}");
				TestContext.WriteLine($"{new BigDecimal(lrg1.Mantissa, lrg1.Exponent)}");
				TestContext.WriteLine($"{new BigDecimal(lrg2.Mantissa, lrg2.Exponent)}");

				BigDecimal result1 = BigDecimal.Add(lrg1, lrg2);
				BigDecimal result2 = BigDecimal.Subtract(lrg1, lrg2);
				BigDecimal result3 = BigDecimal.Multiply(lrg1, lrg2);
				BigDecimal result4 = BigDecimal.Divide(lrg2, lrg1);
				BigDecimal result5 = BigDecimal.Pow(lrg1, 2);
				BigDecimal result6 = BigDecimal.Exp(new BigInteger(5));
				BigDecimal result7 = BigDecimal.Mod(mod1, mod2);
				BigDecimal result8 = BigDecimal.Negate(lrg1);
				BigDecimal result9 = BigDecimal.Floor(lrg1);
				BigDecimal result10 = BigDecimal.Ceiling(lrg1);
				BigDecimal result11 = BigDecimal.Abs(lrg1);

				actual1 = new string(result1.ToString().Take(11).ToArray());
				actual2 = new string(result2.ToString().Take(11).ToArray());
				actual3 = new string(result3.ToString().Take(11).ToArray());
				actual4 = new string(result4.ToString().Take(11).ToArray());
				actual5 = new string(result5.ToString().Take(11).ToArray());
				actual6 = new string(result6.ToString().Take(11).ToArray());
				actual7 = new string(result7.ToString().Take(11).ToArray());
				actual8 = new string(result8.ToString().Take(11).ToArray());
				actual9 = new string(result9.ToString().Take(11).ToArray());
				actual10 = new string(result10.ToString().Take(11).ToArray());
				actual11 = new string(result11.ToString().Take(11).ToArray());
			}
			finally
			{
				BigDecimal.Precision = savePrecision;
				BigDecimal.AlwaysTruncate = false;
			}

			Assert.AreEqual(expected1, actual1, $"Test Truncate On All Arithmetic Operations  - #1: ");
			Assert.AreEqual(expected2, actual2, $"Test Truncate On All Arithmetic Operations  - #2: ");
			Assert.AreEqual(expected3, actual3, $"Test Truncate On All Arithmetic Operations  - #3: ");
			Assert.AreEqual(expected4, actual4, $"Test Truncate On All Arithmetic Operations  - #4: ");
			Assert.AreEqual(expected5, actual5, $"Test Truncate On All Arithmetic Operations  - #5: ");
			StringAssert.StartsWith(expected6, actual6, $"Test Truncate On All Arithmetic Operations  - #6: ");
			Assert.AreEqual(expected7, actual7, $"Test Truncate On All Arithmetic Operations  - #7: ");
			Assert.AreEqual(expected8, actual8, $"Test Truncate On All Arithmetic Operations  - #8: ");
			Assert.AreEqual(expected9, actual9, $"Test Truncate On All Arithmetic Operations  - #9: ");
			Assert.AreEqual(expected10, actual10, $"Test Truncate On All Arithmetic Operations - #10: ");
			Assert.AreEqual(expected11, actual11, $"Test Truncate On All Arithmetic Operations - #11: ");

			Assert.AreEqual(5000, BigDecimal.Precision, "Restore Precision to 5000");
		}
	}

}