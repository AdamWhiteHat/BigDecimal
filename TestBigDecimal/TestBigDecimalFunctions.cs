namespace TestBigDecimal;

using System;
using System.Numerics;
using ExtendedNumerics;
using ExtendedNumerics.Helpers;
using NUnit.Framework;

[TestFixture]
public class TestBigDecimalFunctions
{

	[Test]
	public void TestGCD()
	{
		var expected = BigDecimal.Parse( "10" );

		BigDecimal result = new BigInteger[] {
			20, 30, 210, 310, 360, 5040, 720720
		}.GCD();

		Assert.AreEqual( expected, result );
	}

	[Test]
	public void TestGetFractionalPart()
	{
		var expected = new BigDecimal( BigInteger.Parse( "9150201282920942551781108927727789384397020382853" ), -49 );
		var value = new BigDecimal( BigInteger.Parse( "22685077023948547418271375393606809233149150201282920942551781108927727789384397020382853" ), -49 );

		var result = value.GetFractionalPart();

		Assert.AreEqual( expected, result );
	}

	[Test]
	public void TestGetLength()
	{
		var expected = BigDecimal.Parse( "2268507702394854741827137539360680923314" );
		var value = new BigDecimal( BigInteger.Parse( "22685077023948547418271375393606809233149150201282920942551781108927727789384397020382853" ), -49 );

		Assert.AreEqual( expected.Length, value.Length );
	}

	[Test]
	public void TestGetSign()
	{
		BigDecimal zero1 = 0;
		Assert.AreEqual( 0, zero1.Sign, "0" );

		var zero2 = BigDecimal.Zero;
		Assert.AreEqual( 0, zero2.Sign, "new BigDecimal()" );

		var zero3 = new BigDecimal( 0 );
		Assert.AreEqual( 0, zero3.Sign, "new BigDecimal(0);" );

		var zero4 = new BigDecimal( BigInteger.Zero );
		Assert.AreEqual( 0, zero4.Sign, "new BigDecimal(BigInteger.Zero)" );

		var zero5 = new BigDecimal( 0, -1 );
		Assert.AreEqual( 0, zero5.Sign, "new BigDecimal(0, -1);" );

		BigDecimal zero6 = BigInteger.Subtract( BigInteger.Add( BigInteger.Divide( 2, 3 ), BigInteger.Multiply( -1, BigInteger.Divide( 1, 3 ) ) ), BigInteger.Divide( 1, 3 ) );
		Assert.AreEqual( 0, zero6.Sign, "2/3  -1/3 - 1/3" );

		var oneTenth = BigDecimal.Divide( BigDecimal.One, new BigDecimal( 10 ) );
		BigDecimal pointZeroOne = 0.1d;
		var zero7 = BigDecimal.Subtract( oneTenth, pointZeroOne );
		Assert.AreEqual( 0, zero7.Sign, "1/10 - 1/10" );

		var zero8 = BigDecimal.Add( new BigDecimal( 1, -1 ), -1d / 10d );
		Assert.AreEqual( 0, zero8.Sign, "1 + -1/10" );

		var zero9 = new BigDecimal( 15274, -7 ) * 0;
		Assert.AreEqual( 0, zero9.Sign, "0.0015274" );

		BigDecimal positive1 = 1;
		Assert.AreEqual( 1, positive1.Sign, "1" );

		BigDecimal positive2 = -1 * -1;
		Assert.AreEqual( 1, positive2.Sign, "-1 * 1" );

		var negative1 = BigDecimal.Multiply( BigDecimal.One, BigDecimal.MinusOne );
		Assert.AreEqual( BigInteger.MinusOne.Sign, negative1.Sign, "1 * -1" );

		var negative2 = BigDecimal.Subtract( BigDecimal.Zero, 3 );
		Assert.AreEqual( BigInteger.MinusOne.Sign, negative2.Sign, "0 - 3" );

		BigDecimal negative3 = BigInteger.Subtract( 0, 3 );
		Assert.AreEqual( BigInteger.MinusOne.Sign, negative3.Sign, "BigInteger.Subtract(0, 3)" );

		BigDecimal negative4 = 10 * -1;
		Assert.AreEqual( BigInteger.MinusOne.Sign, negative4.Sign, "10 * -1;" );
	}

	[Test]
	public void TestGetWholeValue()
	{
		var expected = BigDecimal.Parse( "2268507702394854741827137539360680923314" );
		var value = new BigDecimal( BigInteger.Parse( "22685077023948547418271375393606809233149150201282920942551781108927727789384397020382853" ), -49 );

		BigDecimal result = value.WholeValue;

		Assert.AreEqual( expected.ToString(), result.ToString() );
	}

	[Test]
	public void TestGoldenIrrational()
	{
		var goldenRatio = BigDecimal.Parse(
			"1.6180339887498948482045868343656381177203091798057628621354486227052604628189024497072072041893911374847540880753868917521266338622235369317931800607667263544333890865959395829056383226613199282902678806752087668925017116962070322210432162695486262963136144381497587012203408058879544547492461856953648644492" );

		TestContext.Write( "GoldenRatio: " );
		TestContext.WriteLine( goldenRatio.ToString() );
	}

	[Test]
	public void TestLeastCommonDivisor()
	{
		var expected = BigDecimal.Parse( "45319990731015" );

		BigDecimal actual = BigIntegerHelper.LCM( new BigInteger[] {
			3, 5, 7, 11, 13, 101, 307, 311, 313
		} );

		// 15015, lcm(3, 5, 7, 11, 13, 101, 307, 311, 313) = 45319990731015 lcm(4973, 4292, 4978, 4968, 4297, 4287) = 2822891742340306560

		Assert.AreEqual( expected, actual );
	}

	[Test]
	public void TestRandomDivision(
		[Random( Int16.MinValue, Int16.MaxValue, 3 )] Int16 a,
		[Random( Int16.MinValue, Int16.MaxValue, 3 )] Int16 b,
		[Random( Int16.MinValue, Int16.MaxValue, 3 )] Int16 c,
		[Random( Int16.MinValue, Int16.MaxValue, 3 )] Int16 d
	)
	{
		var top = new BigDecimal( a, b );
		var bottom = new BigDecimal( c, d );
		var expected = BigDecimal.Divide( top, bottom );
		var actual = top / bottom;

		Assert.AreEqual( expected, actual );

		//TestContext.WriteLine( actual );
	}

	[Test]
	public void TestRandomEquals( [Random( Int16.MinValue, Int16.MaxValue, 10 )] Int16 a, [Random( Int16.MinValue, Int16.MaxValue, 10 )] Int16 b )
	{
		BigDecimal x = a;
		BigDecimal y = b;

		if ( x == y )
		{
			Assert.AreEqual( y, x );
		}
		else if ( x != y )
		{
			Assert.AreNotEqual( x, y );
		}
		else
		{
			Assert.Fail( "Equals failed." );
		}
	}

	[Test]
	public void TestRounding()
	{
		var up = BigDecimal.Parse( 0.50001 );
		var down = BigDecimal.Parse( 0.49 );
		var oneAndAhalf = BigDecimal.Parse( "1.5" );

		var negEightPointFive = BigDecimal.Parse( -8.5 );
		BigDecimal negNinePointFive = -9.5d;

		var threePointFourNine = BigDecimal.Parse( 3.49 );
		var threePointFiveOne = BigDecimal.Parse( 3.51 );
		var sixPointFive = BigDecimal.Parse( 6.5 );

		var zero = BigDecimal.Round( down );
		Assert.AreEqual( BigInteger.Zero, zero );

		var one = BigDecimal.Round( up );
		Assert.AreEqual( BigInteger.One, one );

		var two = BigDecimal.Round( oneAndAhalf );
		Assert.AreEqual( one + one, two );

		var three = BigDecimal.Round( threePointFourNine );
		Assert.AreEqual( one + two, three );

		var four = BigDecimal.Round( threePointFiveOne );
		Assert.AreEqual( one + three, four );
		Assert.AreEqual( two + two, four );

		var six = BigDecimal.Round( sixPointFive, MidpointRounding.ToEven );
		Assert.AreEqual( two + four, six );

		var negEight = BigDecimal.Round( negEightPointFive, MidpointRounding.ToEven );
		Assert.AreEqual( -four + -four, negEight );

		var negNine = BigDecimal.Round( negEightPointFive, MidpointRounding.AwayFromZero );
		Assert.AreEqual( -four + -four - one, negNine );

		var negTen = BigDecimal.Round( negNinePointFive, MidpointRounding.ToEven );
		Assert.AreEqual( -four + -four - two, negTen );
	}

	[Test]
	public void TestSignifigantDigits()
	{
		const Int32 expected1 = 19;
		var number1 = new BigDecimal( 12345678901234567890, -10 );
		var result1 = number1.SignifigantDigits;
		Assert.AreEqual( expected1, result1 );

		const Int32 expected2 = 9;
		var number2 = new BigDecimal( 123456789, 1 );
		var result2 = number2.SignifigantDigits;
		Assert.AreEqual( expected2, result2 );
	}
}