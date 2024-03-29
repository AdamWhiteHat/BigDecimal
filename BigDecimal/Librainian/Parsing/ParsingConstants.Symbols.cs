// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright must be kept available alongside any use of my source code.
// This source code belongs to Protiguous unless otherwise specified.
// Donations can be made via PayPal at Protiguous@Protiguous.com
// Contact me via email or GitHub https://github.com/Protiguous
//
// File "ParsingConstants.Symbols.cs" last touched on 2024-02-25 at 11:53 PM by Protiguous.

namespace Librainian.Parsing;

public static partial class ParsingConstants {

	public static class Symbols {

		public const String BallotBox = "☐";

		public const String BallotBoxWithCheck = "☑";

		public const String BallotBoxWithX = "☒";

		public const String BlackStar = "★";

		public const String CheckMark = "✓";

		public const String CheckMarkHeavy = "✔";

		/// <summary>
		///     ❕
		/// </summary>
		public const String Error = "❕";

		/// <summary>
		///     ❌
		/// </summary>
		public const String Exception = FailBig;

		public const String Fail = "🗙";

		public const String FailBig = "❌";

		public const String Interobang1 = "‽";

		public const String Interobang2 = "⁈";

		/// <summary>
		///     Symbol for NAK
		/// </summary>
		public const String NegativeAcknowledge = "␕";

		public const String No = "🚫";

		/// <summary>
		///     N/A
		/// </summary>
		public const String NotApplicable = "ⁿ̸ₐ";

		/// <summary>
		///     N/A
		/// </summary>
		public const String NotApplicableHeavy = "ⁿ/ₐ";

		public const String Null = "␀";

		public const String Pipe = "|";

		public const String SkullAndCrossbones = "☠";

		public const String StopSign = "🛑";

		public const String Timeout = "⌛";

		public const String TriplePipes = "⦀";

		public const Char TripleTilde = '≋';

		public const String TwoPipes = "‖";

		public const String Underscore = "_";

		public const String Unknown = "�";

		public const String VerticalEllipsis = "⋮";

		public const String Warning = "⚠";

		public const String WhiteStar = "☆";

		public static class Dice {

			public const String Five = "⚄";

			public const String Four = "⚃";

			public const String One = "⚀";

			public const String Six = "⚅";

			public const String Three = "⚂";

			public const String Two = "⚁";
		}

		public static class Left {

			public const Char DoubleAngle = '«';

			public const Char DoubleParenthesis = '⸨';
		}

		public static class Right {

			public const Char DoubleAngle = '»';

			public const Char DoubleParenthesis = '⸩';
		}
	}

	/// <summary>
	///     Attempts at using text/emoji to make animations - display 1 char at a time from each string.
	/// </summary>
	public static class TextAnimations {

		public const String BallotBoxCheck = Symbols.BallotBox + Symbols.BallotBoxWithCheck;

		public const String BallotBoxUnCheck = Symbols.BallotBoxWithCheck + Symbols.BallotBox;

		public const String Hearts = "🤍💖💓💗💛💙💚🧡💜🖤";

		public const String HorizontalDots = "‥…";

		public const String Pipes = Symbols.VerticalEllipsis + Symbols.Pipe + Symbols.TwoPipes + Symbols.TriplePipes;

		public const String StarBurst = "⭐⁕⁑⁂✨";

		public const String VerticalDots = "․⁞:⁚⁝";
	}
}