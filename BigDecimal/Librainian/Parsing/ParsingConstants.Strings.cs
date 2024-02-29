// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright must be kept available alongside any use of my source code.
// This source code belongs to Protiguous unless otherwise specified.
// Donations can be made via PayPal at Protiguous@Protiguous.com
// Contact me via email or GitHub https://github.com/Protiguous
// 
// File "ParsingConstants.Strings.cs" last touched on 2024-02-29 at 2:51 AM by Protiguous.

namespace Librainian.Parsing;

public static partial class ParsingConstants {

	public static class Strings {

		/// <summary>
		///     ~`!@#$%^&amp;amp;*()-_=+?:,./\[]{}|'
		/// </summary>
		public const String AFewSymbols = @"~`!@#$%^&*()-_=+<>?:,./\[]{}|'";

		/// <summary>
		///     Char x0D
		/// </summary>
		public const String CarriageReturn = "\r";

		public const String CRLF = CarriageReturn + LineFeed;

		/// <summary>
		///     The " char
		/// </summary>
		public const String DoubleQuote = "\"";

		/// <summary>
		///     Two spaces as a <see cref="String" />.
		/// </summary>
		public const String DoubleSpace = SingleSpace + SingleSpace;

		/// <summary>
		///     Char x0A
		/// </summary>
		public const String LineFeed = "\n";

		/// <summary>
		///     The ' char as a <see cref="String" />.
		/// </summary>
		public const String SingleQuote = "'";

		/// <summary>
		///     A single space char as a <see cref="String" />.
		/// </summary>
		public const String SingleSpace = " ";

		/// <summary>
		///     The tab char as a <see cref="String" />.
		/// </summary>
		public const String Tab = "\t";

	}

}