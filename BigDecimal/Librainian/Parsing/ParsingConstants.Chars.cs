// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright must be kept available alongside any use of my source code.
// This source code belongs to Protiguous unless otherwise specified.
// Donations can be made via PayPal at Protiguous@Protiguous.com
// Contact me via email or GitHub https://github.com/Protiguous
// 
// File "ParsingConstants.Chars.cs" last touched on 2024-02-29 at 2:52 AM by Protiguous.

namespace Librainian.Parsing;

public static partial class ParsingConstants {

	public static class Arrays {

		public static readonly Char[] ForwardSlash = [ '/' ];

	}

	public static class Brackets {

		public const Char LeftBracket = '[';

		public const Char LeftUpperCornerBracket = '｢';

		public const Char RightBracket = ']';

		public const Char RightLowerCornerBracket = '｣';

	}

	public static class Chars {

		/// <summary>
		///     ´
		/// </summary>
		public const Char AccentAcute = RightSingleQuote;

		/// <summary>
		///     `
		/// </summary>
		public const Char AccentGrave = '`';

		/// <summary>
		///     '
		/// </summary>
		public const Char Apostrophe = '\'';

		/// <summary>
		///     Char x0D
		/// </summary>
		public const Char CR = '\r';

		/// <summary>
		///     The " char as a <see cref="Char" />.
		/// </summary>
		public const Char DoubleQuote = '\"';

		public const Char ForwardSlash = '/';

		/// <summary>
		///     “
		/// </summary>
		public const Char LeftDoubleQuote = '\u201C';

		/// <summary>
		///     `
		/// </summary>
		public const Char LeftSingleQuote = '`';

		/// <summary>
		///     Char x0A
		/// </summary>
		public const Char LF = '\n';

		/// <summary>
		///     ( <see cref="Char" />)
		/// </summary>
		public const Char NullChar = (Char)0x0;

		/// <summary>
		///     ”
		/// </summary>
		public const Char RightDoubleQuote = '\u201D';

		/// <summary>
		///     ´
		/// </summary>
		public const Char RightSingleQuote = '´';

		public const Char SingleQuote = Apostrophe;

		/// <summary>
		///     The tab char as a <see cref="Char" />.
		/// </summary>
		public const Char Tab = '\t';

	}

	public static class ConstantStrings {

		public const String Main = nameof( Main );

		public const String PrimeConnectionString = nameof( PrimeConnectionString );

	}

	public static class Spaces {

		public const Char BrailleBlank = '⠀';

		public const Char EmSpace = ' ';

		public const Char EnSpace = ' ';

		public const Char FigureSpace = ' ';

		public const Char FourPerEmSpace = ' ';

		public const Char HairSpace = ' ';

		public const Char NoBreakSpace = '\u00A0';

		public const Char PunctuationSpace = ' ';

		public const Char SixPerEmSpace = '\u2006';

		/// <summary>
		///     A single space char.
		/// </summary>
		public const Char Space = ' ';

		public const Char ThinSpace = '\u2009';

		public const Char ThreePerEmSpace = ' ';

		public const Char ZeroWidthSpace = '\u200B';

	}

}