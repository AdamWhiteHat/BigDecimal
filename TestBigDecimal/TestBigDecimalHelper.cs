using System;
using System.Globalization;
using System.Text;
using ExtendedNumerics;

namespace TestBigDecimal
{

	public static class TestBigDecimalHelper
	{
		public static String GetInternalValues(BigDecimal bigDecimal)
		{
			StringBuilder result = new StringBuilder();

			result.AppendLine($"{bigDecimal.ToString()}");
			result.AppendLine("{");
			result.AppendLine($"	Mantessa: {bigDecimal.Mantissa}");
			result.AppendLine($"	Exponent: {bigDecimal.Exponent}");
			result.AppendLine($"	DecimalIndex: {bigDecimal.GetDecimalIndex()}");
			result.AppendLine($"	Length: {bigDecimal.Length}");
			result.AppendLine($"	SignifigantDigits: {bigDecimal.SignifigantDigits}");
			result.AppendLine("}");

			return result.ToString();
		}

		public static string PrepareValue(string value, NumberFormatInfo numberFormatInfo)
		{
			if (numberFormatInfo.NumberDecimalSeparator != ".")
			{
				return value.Replace(".", numberFormatInfo.NumberDecimalSeparator);
			}

			return value;
		}
	}
}