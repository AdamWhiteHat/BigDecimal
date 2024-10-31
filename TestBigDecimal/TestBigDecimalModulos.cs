using System.Globalization;
using System.Threading;
using ExtendedNumerics;
using NUnit.Framework;

namespace TestBigDecimal
{
	[Parallelizable]
	[TestFixture(Category = "modulo", TestOf = typeof(BigDecimal))]
	[Culture("en-US,ru-RU")]
	public static class TestBigDecimalModulos
	{
		internal static NumberFormatInfo NumberFormat { get; } = Thread.CurrentThread.CurrentCulture.NumberFormat;

		[Test]
		public static void TestModulo1()
		{
			// 31 % 19 = 12
			BigDecimal dividend = 31;
			BigDecimal divisor = 19;
			BigDecimal expected = 12;

			BigDecimal actual = BigDecimal.Mod(dividend, divisor);

			Assert.IsTrue(expected == actual);
		}

		[Test]
		public static void TestModulo2()
		{
			// 1891 % 31 = 0
			BigDecimal dividend = 1891;
			BigDecimal divisor = 31;
			BigDecimal expected = 0;

			BigDecimal actual = BigDecimal.Mod(dividend, divisor);

			Assert.IsTrue(expected == actual);
		}

		[Test]
		public static void TestModulo3()
		{
			// 6661 % 60 = 1
			BigDecimal dividend = 6661;
			BigDecimal divisor = 60;
			BigDecimal expected = 1;

			BigDecimal actual = BigDecimal.Mod(dividend, divisor);

			Assert.IsTrue(expected == actual);
		}

		[Test]
		public static void TestModulo4Decimals()
		{
			//NOTE This test fails if the values are Doubles instead of Decimals?

			// 31 % 3.66666 = 1.66672

			BigDecimal dividend = 31m;
			BigDecimal divisor = 3.66666m;
			BigDecimal expected = 1.66672m;

			BigDecimal actual = BigDecimal.Mod(dividend, divisor);

			Assert.IsTrue(expected == actual);
		}

		[Test]
		public static void TestModulo4Doubles()
		{
			//NOTE This test fails if the values are Doubles instead of Decimals?

			// 31 % 3.66666 = 1.66672

			BigDecimal dividend = 31d;
			BigDecimal divisor = 3.66666d;
			BigDecimal expected = 1.66672d;

			BigDecimal actual = BigDecimal.Mod(dividend, divisor);

			Assert.IsTrue(expected == actual);
		}

		[Test]
		public static void TestModulo5()
		{
			// 240 % 2 = 0
			BigDecimal dividend = 240;
			BigDecimal divisor = 2;
			string expectedString = "0";
			BigDecimal expected = 0;

			BigDecimal actual = BigDecimal.Mod(dividend, divisor);
			string actualString = actual.ToString();

			TestContext.WriteLine($"{dividend} % {divisor} = {actual}");

			Assert.IsTrue(expected == actual, $"{dividend} % {divisor} = {actual}");
			Assert.AreEqual(expectedString, actualString, $"{dividend} % {divisor} = {actual}");
		}


		[Test]
		public static void TestModuloNegative157766400With60()
		{
			//Added for issue https://github.com/AdamWhiteHat/BigDecimal/issues/61
			BigDecimal expected = BigDecimal.Zero;

			BigDecimal number = -157766400;
			int modby = 60;
			BigDecimal actual = number % modby;

			Assert.AreEqual(expected, actual, $"{number} modulo {modby} should result in {expected}.");
		}

		[Test]
		public static void TestModulo157766400With60()
		{
			//Added for issue https://github.com/AdamWhiteHat/BigDecimal/issues/61
			BigDecimal expected = BigDecimal.Zero;

			BigDecimal number = 157766400;
			int modby = 60;
			BigDecimal actual = number % modby;

			Assert.AreEqual(expected, actual, $"{number} modulo {modby} should result in {expected}.");
		}
	

		[Test]
		public static void TestModuloNegative157766400With120()
		{
			//Added for issue https://github.com/AdamWhiteHat/BigDecimal/issues/61
			BigDecimal expected = BigDecimal.Zero;

			BigDecimal number = -157766400;
			int modby = 120;
			BigDecimal actual = number % modby;

			Assert.AreEqual(expected, actual, $"{number} modulo {modby} should result in {expected}.");
		}

		[Test]
		public static void TestModulo157766400With120()
		{
			//Added for issue https://github.com/AdamWhiteHat/BigDecimal/issues/61
			BigDecimal expected = BigDecimal.Zero;

			BigDecimal number = 157766400;
			int modby = 120;
			BigDecimal actual = number % modby;

			Assert.AreEqual(expected, actual, $"{number} modulo {modby} should result in {expected}.");
		}
	}
}