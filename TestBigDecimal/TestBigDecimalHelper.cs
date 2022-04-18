namespace TestBigDecimal
{
	using System;
	using System.Text;
	using ExtendedNumerics;
	using ExtendedNumerics.Helpers;
	using NUnit.Framework;

	public static class TestBigDecimalHelper
	{
		public static String GetInternalValues( BigDecimal bigDecimal )
		{
			StringBuilder result = new StringBuilder();

			result.AppendLine( $"{bigDecimal.ToString()}" );
			result.AppendLine( "{" );
			result.AppendLine( $"	Mantessa: {bigDecimal.Mantissa}" );
			result.AppendLine( $"	Exponent: {bigDecimal.Exponent}" );
			result.AppendLine( $"	DecimalIndex: { bigDecimal.GetDecimalIndex()}" );
			result.AppendLine( $"	Length: {bigDecimal.Length}" );
			result.AppendLine( $"	SignifigantDigits: {bigDecimal.SignifigantDigits}" );
			result.AppendLine( "}" );

			return result.ToString();
		}
	}
}
