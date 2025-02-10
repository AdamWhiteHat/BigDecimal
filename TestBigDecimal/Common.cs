using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExtendedNumerics;
using NUnit.Framework;

namespace TestBigDecimal
{
	public static class TestExtensionMethods
	{
		/// <summary>
		/// Truncates a string. Used to limit the length of a result when writing out to the TestContext.
		/// </summary>
		/// <param name="value">The string to truncate, if needed.</param>
		/// <param name="digits">The maximum number of digits to display before truncating, or, if the default of -1 is passed in, it truncates 10 digits after the decimal point.</param>
		/// <returns></returns>
		public static string TruncateAt(this string value, int digits = -1)
		{
			int length = digits;
			if (digits == -1)
			{
				int decimalPlace = value.IndexOf('.');
				if (decimalPlace == -1)
				{
					length = value.Length;
				}
				else
				{
					length = decimalPlace + 10;
				}
			}
			return value.Substring(0, length);
		}

		public static string TruncateAt(this BigDecimal value, int digits = -1)
		{
			return TruncateAt(value.ToString(), digits);
		}
	}

	public static class Debug
	{
		public static class Assert
		{
			public static void AreEqual(string expected, string actual, int minimumCorrectDigits)
			{
				string _expected = expected.Substring(0, minimumCorrectDigits);
				string _actual = actual.Substring(0, minimumCorrectDigits);

				NUnit.Framework.Assert.AreEqual(_expected, _actual);
			}
		}
	}

	public class Common
	{
		private static Dictionary<char, string> BoldedNumeralDictionary = new Dictionary<char, string>()
		{
			{'-',"-"},
			{'.',"."},
			{'0',"𝟬"},
			{'1',"𝟭"},
			{'2',"𝟮"},
			{'3',"𝟯"},
			{'4',"𝟰"},
			{'5',"𝟱"},
			{'6',"𝟲"},
			{'7',"𝟳"},
			{'8',"𝟴"},
			{'9',"𝟵"}
		};

		/// <summary>
		/// Visual display of matching digits. Returns number of correct digits.
		/// </summary>
		public static int HightlightDiffControl(string expected, string actual, int indent = 10)
		{
			int index = 0;
			int maxLength = Math.Min(actual.Length, expected.Length);

			int correctDigitCount = 0;
			bool matchSoFar = true;
			string indentation = new string(Enumerable.Repeat(' ', indent).ToArray());
			StringBuilder result = new StringBuilder(indentation);
			StringBuilder result2 = new StringBuilder(indentation);
			foreach (char c in actual)
			{
				char? cmp = (index < maxLength) ? expected[index] : null;
				if (matchSoFar && cmp.HasValue && c == cmp)
				{
					result.Append(c);
					result2.Append('▀');
					correctDigitCount++;
				}
				else
				{
					if (matchSoFar)
					{
						matchSoFar = false;
						if (result.ToString().Contains('.'))
						{
							correctDigitCount--;
						}
						result2.Append($" ({correctDigitCount} digits)");
					}
					else
					{
						break;
					}
				}

				index++;
			}
			TestContext.WriteLine(result.ToString());
			TestContext.WriteLine(result2.ToString());

			return correctDigitCount;
		}
	}
}
