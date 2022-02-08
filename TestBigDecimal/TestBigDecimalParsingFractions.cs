// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries,
// repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper licenses and/or copyrights.
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "TestBigDecimalParsingFractions.cs" last formatted on 2022-02-08 at 3:11 AM by Protiguous.

namespace TestBigDecimal;

using ExtendedNumerics;
using ExtendedNumerics.Helpers;
using NUnit.Framework;

[TestFixture]
public class TestBigDecimalParsingFractions {

	private static readonly String String123450000;

	static TestBigDecimalParsingFractions() => String123450000 = "123450000" + new String( '0', UInt16.MaxValue );

	[Test]
	public void TestFractional1() {
		const String test = " 12.1 / 36.3 ";
		var expected = BigDecimal.Parse( "0.333" );

		var parsed = test.TryParseFraction( out var result );
		if ( parsed ) {
			Assert.AreEqual( expected, result );
		}
		else {
			Assert.Fail( "Only 3 3s." );
		}
	}

	[Test]
	public void TestNormalize123450000UInt16() {
		var expected = new BigDecimal( 123450000, UInt16.MaxValue );
		var bd = BigDecimal.Parse( String123450000 );

		Assert.AreEqual( expected, bd );
	}
}