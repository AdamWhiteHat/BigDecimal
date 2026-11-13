using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using ExtendedNumerics.Helpers;

namespace TestBigDecimal
{
	[TestFixture]
	public class TestBigIntegerHelper
	{
		private NumberFormatInfo Format { get { return Thread.CurrentThread.CurrentCulture.NumberFormat; } }

		[Test]
		public void TestGetLength000()
		{
			BigInteger number = BigInteger.Zero;
			int expected = 0;

			int actual = number.GetLength();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestGetLength001()
		{
			BigInteger number = BigInteger.One;
			int expected = 1;

			int actual = number.GetLength();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestGetLength002()
		{
			BigInteger number = BigInteger.Parse("10");
			int expected = 2;

			int actual = number.GetLength();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestGetLength003()
		{
			BigInteger number = BigInteger.Parse("-10");
			int expected = 2;

			int actual = number.GetLength();

			Assert.AreEqual(expected, actual);
		}


		[Test]
		public void TestGetLength004()
		{
			BigInteger number = BigInteger.Parse("999");
			int expected = 3;

			int actual = number.GetLength();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestGetLength005()
		{
			BigInteger number = BigInteger.Parse("-999");
			int expected = 3;

			int actual = number.GetLength();

			Assert.AreEqual(expected, actual);
		}
	}
}
