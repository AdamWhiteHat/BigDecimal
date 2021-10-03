namespace ExtendedNumerics {

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Numerics;

	/// <summary>
	///     Sqrt and NRoot acquired from http://mjs5.com/2016/01/20/c-biginteger-helper-constructors
	/// </summary>
	public static class BigIntegerHelper {

		public static IEnumerable<BigInteger> GetRange( BigInteger min, BigInteger max ) {
			var counter = min;

			while ( counter < max ) {
				yield return counter;
				counter++;
			}
		}

		public static Boolean IsCoprime( BigInteger value1, BigInteger value2 ) => GCD( value1, value2 ) == 1;

		public static BigInteger GCD( IEnumerable<BigInteger> numbers ) => numbers.Aggregate( GCD );

		public static BigInteger LCM( IEnumerable<BigInteger> numbers ) => numbers.Aggregate( LCM );

		public static BigInteger LCM( BigInteger num1, BigInteger num2 ) {
			var absValue1 = BigInteger.Abs( num1 );
			var absValue2 = BigInteger.Abs( num2 );
			return absValue1 * absValue2 / GCD( absValue1, absValue2 );
		}

		public static BigInteger GCD( BigInteger value1, BigInteger value2 ) {
			var absValue1 = BigInteger.Abs( value1 );
			var absValue2 = BigInteger.Abs( value2 );

			while ( absValue1 != 0 && absValue2 != 0 ) {
				if ( absValue1 > absValue2 ) {
					absValue1 %= absValue2;
				}
				else {
					absValue2 %= absValue1;
				}
			}

			return BigInteger.Max( absValue1, absValue2 );
		}

		public static Int32 GetLength( this BigInteger source ) {
			var result = 0;
			var copy = source;
			while ( copy > 0 ) {
				copy /= 10;
				result++;
			}

			return result;
		}

		public static BigInteger Square( this BigInteger input ) => input * input;

		public static BigInteger SquareRoot( BigInteger input ) {
			if ( input.IsZero ) {
				return BigInteger.Zero;
			}

			var n = BigInteger.Zero;
			var p = BigInteger.Zero;
			var low = BigInteger.Zero;
			var high = BigInteger.Abs( input );

			while ( high > low + 1 ) {
				n = ( high + low ) >> 1;
				p = n * n;
				if ( input < p ) {
					high = n;
				}
				else if ( input > p ) {
					low = n;
				}
				else {
					break;
				}
			}

			return input == p ? n : low;
		}

		// Returns the NTHs root of a BigInteger with Remainder.
		// The root must be greater than or equal to 1 or value must be a positive integer.
		public static BigInteger NthRoot( this BigInteger value, Int32 root, out BigInteger remainder ) {
			if ( root < 1 ) {
				throw new ArgumentException( "root must be greater than or equal to 1", nameof( root ) );
			}

			if ( value.Sign == -1 ) {
				throw new ArgumentException( "value must be a positive integer", nameof( value ) );
			}

			if ( value == BigInteger.One ) {
				remainder = 0;
				return BigInteger.One;
			}

			if ( value == BigInteger.Zero ) {
				remainder = 0;
				return BigInteger.Zero;
			}

			if ( root == 1 ) {
				remainder = 0;
				return value;
			}

			var upperbound = value;
			var lowerbound = BigInteger.Zero;

			while ( true ) {
				var nval = ( upperbound + lowerbound ) >> 1;
				var tstsq = BigInteger.Pow( nval, root );
				if ( tstsq > value ) {
					upperbound = nval;
				}

				if ( tstsq < value ) {
					lowerbound = nval;
				}

				if ( tstsq == value ) {
					lowerbound = nval;
					break;
				}

				if ( lowerbound == upperbound - 1 ) {
					break;
				}
			}

			remainder = value - BigInteger.Pow( lowerbound, root );
			return lowerbound;
		}

	}

}