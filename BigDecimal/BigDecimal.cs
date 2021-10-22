#nullable enable

namespace ExtendedNumerics {

	using System;
	using System.CodeDom;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.Globalization;
	using System.Linq;
	using System.Numerics;
	using System.Runtime.CompilerServices;
	using System.Threading.Tasks;
	using Properties;

	/// <summary>
	///     Arbitrary precision decimal.
	///     All operations are exact, except for division. Division never determines more digits than the given precision.
	///     Based on code by Jan Christoph Bernack (http://stackoverflow.com/a/4524254 or jc.bernack at googlemail.com)
	///     Modified and extended by Adam White https://csharpcodewhisperer.blogspot.com
	/// </summary>
	public record BigDecimal : IComparable<BigDecimal>, IComparable<Int32>, IComparable {

		private const String NumericCharacters = "-0.1234567890";

		private const String NullString = "(null)";

		public static readonly BigDecimal Ten = new( 10, 0 );

		public static readonly BigDecimal One = new( 1 );

		public static readonly BigDecimal Zero = new( 0 );

		public static readonly BigDecimal OneHalf = 0.5d;

		public static readonly BigDecimal MinusOne;

		public static readonly BigDecimal E;

		public static readonly BigDecimal Pi;

		public static readonly BigDecimal π;

		private static readonly BigInteger TenInt = new( 10 );

		private static readonly NumberFormatInfo BigDecimalNumberFormatInfo;

		static BigDecimal() {
			BigDecimalNumberFormatInfo = CultureInfo.InvariantCulture.NumberFormat;

			MinusOne = new BigDecimal( BigInteger.MinusOne, 0 );

			E = new BigDecimal(
				BigInteger.Parse(
					"271828182845904523536028747135266249775724709369995957496696762772407663035354759457138217852516642749193200305992181741359662904357290033429526059563073813232862794349076323382988075319525101901157383" ),
				1 );
			Pi = new BigDecimal(
				BigInteger.Parse(
					"314159265358979323846264338327950288419716939937510582097494459230781640628620899862803482534211706798214808651328230664709384460955058223172535940812848111745028410270193852110555964462294895493038196" ),
				1 );
			π = GetPiDigits();
		}

		public BigDecimal( Int32 value ) : this( new BigInteger( value ), 0 ) { }

		public BigDecimal( BigInteger mantissa, Int32 exponent = 0 ) {
			this.Mantissa = mantissa;
			this.Exponent = exponent;

			BigDecimal result;
			if ( AlwaysTruncate ) {
				result = Truncate( this, Precision );
				this.Mantissa = result.Mantissa;
				this.Exponent = result.Exponent;
			}
			else {
				if ( AlwaysNormalize ) {
					result = Normalize( this );
					this.Mantissa = result.Mantissa;
					this.Exponent = result.Exponent;
				}
			}
		}

		public BigDecimal( Double value ) {
			if ( Double.IsInfinity( value ) ) {
				throw new OverflowException( "BigDecimal cannot represent infinity." );
			}

			if ( Double.IsNaN( value ) ) {
				throw new NotFiniteNumberException( $"{nameof( value )} is not a number (double.NaN)." );
			}

			var mantissa = new BigInteger( value );
			var exponent = 0;
			Double scaleFactor = 1;
			while ( Math.Abs( value * scaleFactor - Double.Parse( mantissa.ToString() ) ) > 0 ) {
				exponent--;
				scaleFactor *= 10;
				mantissa = new BigInteger( value * scaleFactor );
			}

			this.Mantissa = mantissa;
			this.Exponent = exponent;

			BigDecimal result;
			if ( AlwaysTruncate ) {
				result = Truncate( this, Precision );
				this.Mantissa = result.Mantissa;
				this.Exponent = result.Exponent;
			}
			else {
				if ( AlwaysNormalize ) {
					result = Normalize( this );
					this.Mantissa = result.Mantissa;
					this.Exponent = result.Exponent;
				}
			}
		}

		/// <summary>
		///     Sets the maximum precision of division operations. If AlwaysTruncate is set to true all operations are affected.
		/// </summary>
		public static Int32 Precision { get; set; } = 5000;

		/// <summary>
		///     Specifies whether the significant digits should be truncated to the given precision after each operation.
		/// </summary>
		public static Boolean AlwaysTruncate { get; set; } = false;

		/// <summary>
		///     TODO Describe this.
		/// </summary>
		public static Boolean AlwaysNormalize { get; set; } = true;

		public BigInteger Mantissa { get; private set; }

		public Int32 Exponent { get; private set; }

		public Int32 Sign => this.GetSign();

		public Int32 SignifigantDigits => GetSignifigantDigits( this.Mantissa );

		public Int32 DecimalPlaces => this.SignifigantDigits + this.Exponent;

		public BigInteger WholeValue => this.GetWholePart();

		public Int32 Length => GetSignifigantDigits( this.Mantissa ) + this.Exponent;

		/// <summary>
		///     This method returns true if the BigDecimal is equal to zero, false otherwise.
		/// </summary>
		public Boolean IsZero => this.Mantissa.IsZero;

		/// <summary>
		///     This method returns true if the BigDecimal is greater than zero, false otherwise.
		/// </summary>
		public Boolean IsPositve => !this.IsZero && !this.IsNegative;

		/// <summary>
		///     This method returns true if the BigDecimal is less than zero, false otherwise.
		/// </summary>
		public Boolean IsNegative => this.Mantissa.Sign < 0;

		public Int32 CompareTo( Object? obj ) {
			if ( obj is not Int32 other || this > other ) {
				return 1;
			}

			if ( this < other ) {
				return -1;
			}

			return 0;
		}

		/// <summary>
		///     Compares two BigDecimal values, returning an integer that indicates their relationship.
		/// </summary>
		public Int32 CompareTo( BigDecimal? other ) {
			if ( other is null || this > other ) {
				return 1;
			}

			if ( this < other ) {
				return -1;
			}

			return 0;
		}

		public Int32 CompareTo( Int32 other ) =>
			this < other ? -1 :
			this > other ? 1 : 0;

		public virtual Boolean Equals( BigDecimal? other ) => Equals( this, other );

		/// <summary>
		/// Load the value of π up to 1 million digits.
		/// <para>Defaults to 512 digits.</para>
		/// </summary>
		/// <param name="digits"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public static BigDecimal GetPiDigits( Int32 digits = 512 ) {
			if ( digits is <= 0 or > 1_000_000) {
				throw new ArgumentOutOfRangeException( nameof( digits ) );
			}

			var s = Resources.PiString?[ ..( 2 + digits ) ];

			if ( String.IsNullOrWhiteSpace( s ) ) {
				throw new InvalidOperationException( $"Unable to load the value of π.[{digits}] from resources." );
			}

			return Parse( s );
		}

		public static Boolean Equals( BigDecimal? left, BigDecimal? right ) {
			if ( left is null || right is null ) {
				return false;
			}

			return left.Mantissa.Equals( right.Mantissa ) && left.Exponent.Equals( right.Exponent ) && left.Sign.Equals( right.Sign );
		}

		public Int32 CompareTo( Int32? other ) {
			if ( other is null || this > other ) {
				return 1;
			}

			if ( this < other ) {
				return -1;
			}

			return 0;
		}

		/// <summary>
		///     Converts the string representation of a decimal to the BigDecimal equivalent.
		/// </summary>
		public static BigDecimal Parse( Double input ) => Parse( input.ToString( CultureInfo.CurrentCulture ) );

		/// <summary>
		///     Converts the string representation of a decimal to the BigDecimal equivalent.
		/// </summary>
		public static BigDecimal Parse( Decimal input ) => Parse( input.ToString( CultureInfo.CurrentCulture ) );

		/// <summary>
		///     Converts the string representation of a decimal to the BigDecimal equivalent.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		/// <exception cref=""></exception>
		/// <exception cref=""></exception>
		public static BigDecimal Parse( String input ) {
			if ( String.IsNullOrEmpty( input ) || !Char.IsDigit( input[ 0 ] ) || !Char.IsDigit( input[ ^1 ] ) ) {
				input = input.Trim();
			}

			if ( String.IsNullOrEmpty( input ) ) {
				return BigInteger.Zero;
			}

			/*
			 //TODO If lengths are short enough and decimal points are in the right places, can we just shortcut the code further down with these?
			if ( BigInteger.TryParse( input, out var bigInteger ) ) {
				return bigInteger;
			}

			if ( Decimal.TryParse( input, out var decimalResult ) ) {
				return decimalResult;
			}

			if ( Double.TryParse( input, out var doubleResult ) ) {
				return doubleResult;
			}
			*/

			var exponent = 0;
			var isNegative = false;
			//var localInput = input;//new String( input.Trim().Where( c => NumericCharacters.Contains( c ) ).ToArray() );

			if ( input.StartsWith( BigDecimalNumberFormatInfo.NegativeSign ) ) {
				isNegative = true;
				input = input.Replace( BigDecimalNumberFormatInfo.NegativeSign, String.Empty );
			}

			if ( input.Contains( BigDecimalNumberFormatInfo.NumberDecimalSeparator ) ) {
				var decimalPlace = input.IndexOf( BigDecimalNumberFormatInfo.NumberDecimalSeparator, StringComparison.Ordinal );

				exponent = decimalPlace + 1 - input.Length;
				input = input.Replace( BigDecimalNumberFormatInfo.NumberDecimalSeparator, String.Empty );
			}

			var mantessa = BigInteger.Parse( input );
			if ( isNegative ) {
				mantessa = BigInteger.Negate( mantessa );
			}

			return new BigDecimal( mantessa, exponent );
		}

		/// <summary>
		///     Truncates the BigDecimal to the given precision by removing the least significant digits.
		/// </summary>
		public static BigDecimal Truncate( BigDecimal value, Int32 precision ) {
			var sign = Math.Sign( value.Exponent );
			var difference = ( precision - GetSignifigantDigits( value.Mantissa ) ) * -1;
			if ( difference >= 1 ) {
				value.Mantissa = BigInteger.Divide( value.Mantissa, BigInteger.Pow( TenInt, difference ) );
				if ( sign != 0 ) {
					value.Exponent += sign * difference;
				}
			}

			return Normalize( value );
		}

		/*
		/// <summary>
		///     Truncates the BigDecimal to the current precision by removing the least significant digits.
		/// </summary>
		public void Truncate() {
			Truncate( this, Precision );
		}
		*/

		/// <summary>
		///     Removes any trailing zeros on the mantissa, adjusts the exponent, and returns a new <see cref="BigDecimal" />.
		/// </summary>
		/// <param name="value"></param>
		[Pure]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static BigDecimal Normalize( BigDecimal value ) {
			if ( value.IsZero ) {
				return value;
			}

			//var sign = Math.Sign( this.Exponent );

			//positive number?

			var s = value.Mantissa.ToString();
			var pos = s.LastIndexOf( '0', s.Length - 1 );
			if ( pos >= s.Length - 1 ) {
				var c = s[ pos ];

				while ( pos > 0 && c == '0' ) {
					c = s[ --pos ]; //scan backwards to find the not-last 0.
				}

				var mant = s.Substring( 0, pos + 1 );
				var zeros = s.Substring( pos + 1 );

				if ( BigInteger.TryParse( mant, out var mantissa ) ) {
					value = value with {
						Mantissa = mantissa,
						Exponent = value.Exponent + zeros.Length
					};
				}
			}

			return value;
		}

		/// <summary>
		///     Returns the zero-based index of the decimal point, if the BigDecimal were rendered as a string.
		/// </summary>
		public Int32 GetDecimalIndex() => GetDecimalIndex( this.Mantissa, this.Exponent );

		/// <summary>
		///     Returns the whole number integer part of the BigDecimal, dropping anything right of the decimal point. Essentially
		///     behaves like Math.Truncate(). For example, GetWholePart() would return 3 for Math.PI.
		/// </summary>
		public BigInteger GetWholePart() {
			var resultString = String.Empty;
			var decimalString = ToString( this.Mantissa, this.Exponent, BigDecimalNumberFormatInfo );
			var valueSplit = decimalString.Split( '.', StringSplitOptions.RemoveEmptyEntries );
			if ( valueSplit.Length > 0 ) {
				resultString = valueSplit[ 0 ];
			}

			return BigInteger.Parse( resultString );
		}

		/// <summary>
		///     Gets the fractional part of the BigDecimal, setting everything left of the decimal point to zero.
		/// </summary>
		public BigDecimal GetFractionalPart() {
			var resultString = String.Empty;
			var decimalString = this.ToString();

			var valueSplit = decimalString.Split( '.' );
			if ( valueSplit.Length == 1 ) {
				return Zero; //BUG Is this right?
			}

			if ( valueSplit.Length == 2 ) {
				resultString = valueSplit[ 1 ];
			}

			var newMantessa = BigInteger.Parse( resultString.TrimStart( '0' ) );
			var result = new BigDecimal( newMantessa, 0 - resultString.Length );
			return result;
		}

		private Int32 GetSign() {
			if ( this.Mantissa.IsZero ) {
				return 0;
			}

			if ( this.Mantissa.Sign == 1 ) {
				return 1; //BUG Is this correct?
			}

			if ( this.Exponent >= 0 ) {
				return -1; //BUG Is this correct?
			}

			var mant = this.Mantissa.ToString();
			var length = mant.Length + this.Exponent;
			if ( length == 0 ) {
				Int32.TryParse( mant[ 0 ].ToString(), out var tenthsPlace );
				if ( tenthsPlace >= 5 ) {
					return 1;
				}

				return 0;
			}

			if ( length > 0 ) {
				return 1;
			}

			return 0;
		}

		private static Int32 GetDecimalIndex( BigInteger mantissa, Int32 exponent ) {
			var mantissaLength = mantissa.GetLength();
			if ( mantissa.Sign < 0 ) {
				mantissaLength++;
			}

			return mantissaLength + exponent;
		}

		private static Int32 GetSignifigantDigits( BigInteger value ) {
			if ( value.IsZero ) {
				return 0;
			}

			var valueString = value.ToString();
			if ( String.IsNullOrWhiteSpace( valueString ) ) {
				return 0;
			}

			//TODO Why Trim? Why the Where?
			valueString = new String( valueString.Trim().Where( c => NumericCharacters.Contains( c ) ).ToArray() );

			valueString = valueString.Replace( BigDecimalNumberFormatInfo.NegativeSign, String.Empty );

			//TODO Why "+"?
			valueString = valueString.Replace( BigDecimalNumberFormatInfo.PositiveSign, String.Empty );
			valueString = valueString.TrimEnd( '0' );

			//TODO Can BigInteger ever have a "." to replace?
			valueString = valueString.Replace( BigDecimalNumberFormatInfo.NumberDecimalSeparator, String.Empty );

			return valueString.Length;
		}

		/// <summary>
		///     Returns the mantissa of value, aligned to the exponent of reference. Assumes the exponent of value is larger than
		///     of reference.
		/// </summary>
		private static BigInteger AlignExponent( BigDecimal value, BigDecimal reference ) => value.Mantissa * BigInteger.Pow( TenInt, value.Exponent - reference.Exponent );

		public static implicit operator BigDecimal( BigInteger value ) => new( value, 0 );

		public static implicit operator BigDecimal( Byte value ) => new( new BigInteger( value ), 0 );

		public static implicit operator BigDecimal( SByte value ) => new( new BigInteger( value ), 0 );

		public static implicit operator BigDecimal( UInt32 value ) => new( new BigInteger( value ), 0 );

		public static implicit operator BigDecimal( Int32 value ) => new( new BigInteger( value ), 0 );

		public static implicit operator BigDecimal( UInt16 value ) => new( new BigInteger( value ), 0 );

		public static implicit operator BigDecimal( Int16 value ) => new( new BigInteger( value ), 0 );

		public static implicit operator BigDecimal( UInt64 value ) => new( new BigInteger( value ), 0 );

		public static implicit operator BigDecimal( Int64 value ) => new( new BigInteger( value ), 0 );

		public static implicit operator BigDecimal( Single value ) => new( value );

		public static implicit operator BigDecimal( Double value ) => new( value );

		public static implicit operator BigDecimal( Decimal value ) {
			var mantissa = new BigInteger( value );
			var exponent = 0;
			Decimal scaleFactor = 1;
			while ( Decimal.Parse( mantissa.ToString() ) != value * scaleFactor ) {
				exponent--;
				scaleFactor *= 10;
				mantissa = new BigInteger( value * scaleFactor );
			}

			return new BigDecimal( mantissa, exponent );
		}

		public static explicit operator BigInteger( BigDecimal value ) {
			if ( value is null ) {
				throw new ArgumentNullException( nameof( value ) );
			}

			//Normalize( value );
			if ( value.Exponent < 0 ) {
				var mant = value.Mantissa.ToString();

				var length = value.GetDecimalIndex();
				if ( length > 0 ) {
					var s = mant[ ..length ];
					if ( s != null ) {
						return BigInteger.Parse( s );
					}
				}

				if ( length == 0 ) {
					var tenthsPlace = Int32.Parse( mant[ 0 ].ToString() );
					return tenthsPlace >= 5 ? new BigInteger( 1 ) : BigInteger.Zero;
				}

				return BigInteger.Zero;
			}

			return BigInteger.Multiply( value.Mantissa, BigInteger.Pow( TenInt, value.Exponent ) );
		}

		public static explicit operator Double( BigDecimal value ) {
			if ( !Double.TryParse( value.Mantissa.ToString(), out var mantissa ) ) {
				mantissa = Convert.ToDouble( value.Mantissa.ToString() );
			}

			return mantissa * Math.Pow( 10, value.Exponent );
		}

		public static explicit operator Single( BigDecimal value ) {
			if ( !Single.TryParse( value.Mantissa.ToString(), out var mantissa ) ) {
				mantissa = Convert.ToSingle( value.Mantissa.ToString() );
			}

			return mantissa * ( Single )Math.Pow( 10, value.Exponent );
		}

		public static explicit operator Decimal( BigDecimal value ) {
			if ( !Decimal.TryParse( value.Mantissa.ToString(), out var mantissa ) ) {
				mantissa = Convert.ToDecimal( value.Mantissa.ToString() );
			}

			return mantissa * ( Decimal )Math.Pow( 10, value.Exponent );
		}

		public static explicit operator Int32( BigDecimal value ) {
			if ( !Int32.TryParse( value.Mantissa.ToString(), out var mantissa ) ) {
				mantissa = Convert.ToInt32( value.Mantissa.ToString() );
			}

			return mantissa * ( Int32 )BigInteger.Pow( TenInt, value.Exponent );
		}

		public static explicit operator UInt32( BigDecimal value ) {
			if ( !UInt32.TryParse( value.Mantissa.ToString(), out var mantissa ) ) {
				mantissa = Convert.ToUInt32( value.Mantissa.ToString() );
			}

			return mantissa * ( UInt32 )BigInteger.Pow( TenInt, value.Exponent );
		}

		public static BigDecimal operator +( BigDecimal value ) => value;

		public static BigDecimal operator -( BigDecimal value ) => Negate( value );

		public static BigDecimal operator ++( BigDecimal value ) => Add( value, 1 );

		public static BigDecimal operator --( BigDecimal value ) => Subtract( value, 1 );

		public static BigDecimal operator +( BigDecimal left, BigDecimal right ) => Add( left, right );

		public static BigDecimal operator -( BigDecimal left, BigDecimal right ) => Subtract( left, right );

		public static BigDecimal operator *( BigDecimal left, BigDecimal right ) => Multiply( left, right );

		public static BigDecimal operator /( BigDecimal dividend, BigDecimal divisor ) => Divide( dividend, divisor );

		//public static Boolean operator ==( BigDecimal left, BigDecimal right ) => left.Exponent == right.Exponent && left.Mantissa == right.Mantissa;

		//public static Boolean operator !=( BigDecimal left, BigDecimal right ) => left.Exponent != right.Exponent || left.Mantissa != right.Mantissa;

		public static Boolean operator <( BigDecimal left, BigDecimal right ) =>
			left.Exponent > right.Exponent ? AlignExponent( left, right ) < right.Mantissa : left.Mantissa < AlignExponent( right, left );

		public static Boolean operator >( BigDecimal left, BigDecimal right ) =>
			left.Exponent > right.Exponent ? AlignExponent( left, right ) > right.Mantissa : left.Mantissa > AlignExponent( right, left );

		public static Boolean operator <=( BigDecimal left, BigDecimal right ) =>
			left.Exponent > right.Exponent ? AlignExponent( left, right ) <= right.Mantissa : left.Mantissa <= AlignExponent( right, left );

		public static Boolean operator >=( BigDecimal left, BigDecimal right ) =>
			left.Exponent > right.Exponent ? AlignExponent( left, right ) >= right.Mantissa : left.Mantissa >= AlignExponent( right, left );

		/// <summary>
		///     Returns the result of multiplying a BigDecimal by negative one.
		/// </summary>
		public static BigDecimal Negate( BigDecimal value ) {
			if ( value is null ) {
				throw new ArgumentNullException( nameof( value ) );
			}

			return value with {
				Mantissa = BigInteger.Negate( value.Mantissa )
			};
		}

		/// <summary>
		///     Adds two BigDecimal values.
		/// </summary>
		public static BigDecimal Add( BigDecimal left, BigDecimal right ) {
			if ( left is null ) {
				throw new ArgumentNullException( nameof( left ) );
			}

			if ( right is null ) {
				throw new ArgumentNullException( nameof( right ) );
			}

			return left.Exponent > right.Exponent ? new BigDecimal( AlignExponent( left, right ) + right.Mantissa, right.Exponent ) :
				new BigDecimal( AlignExponent( right, left ) + left.Mantissa, left.Exponent );
		}

		/// <summary>
		///     Subtracts two BigDecimal values.
		/// </summary>
		public static BigDecimal Subtract( BigDecimal left, BigDecimal right ) => Add( left, Negate( right ) );

		/// <summary>
		///     Multiplies two BigDecimal values.
		/// </summary>
		public static BigDecimal Multiply( BigDecimal left, BigDecimal right ) => new( left.Mantissa * right.Mantissa, left.Exponent + right.Exponent );

		/// <summary>
		///     Divides two BigDecimal values, returning the remainder and discarding the quotient.
		/// </summary>
		public static BigDecimal Mod( BigDecimal value, BigDecimal mod ) {
			// x – q * y
			var quotient = Divide( value, mod );
			var floor = Floor( quotient );
			return Subtract( value, Multiply( floor, mod ) );
		}

		/// <summary>
		///     Divides two BigDecimal values.
		/// </summary>
		public static BigDecimal Divide( BigDecimal dividend, BigDecimal divisor ) {
			if ( dividend is null ) {
				throw new ArgumentNullException( nameof( dividend ) );
			}

			if ( divisor is null ) {
				throw new ArgumentNullException( nameof( divisor ) );
			}

			if ( divisor == Zero ) {
				throw new DivideByZeroException( nameof( divisor ) );
			}

			//Normalize( dividend );
			//Normalize( divisor );

			//	if (dividend > divisor) { return Divide_Positive(dividend, divisor); }

			if ( Abs( dividend ) == 1 ) {
				var doubleDivisor = Double.Parse( divisor.ToString() );
				doubleDivisor = 1d / doubleDivisor;

				return Parse( doubleDivisor.ToString( CultureInfo.CurrentCulture ) );
			}

			//var remString = "";
			//var mantissaString = "";
			//var dividendMantissaString = dividend.Mantissa.ToString();
			//var divisorMantissaString = divisor.Mantissa.ToString();

			//var dividendMantissaLength = dividend.DecimalPlaces;
			//var divisorMantissaLength = divisor.DecimalPlaces;
			var exponentChange = dividend.Exponent - divisor.Exponent; //(dividendMantissaLength - divisorMantissaLength);

			var counter = 0;
			BigDecimal result = 0;
			result.Mantissa = BigInteger.DivRem( dividend.Mantissa, divisor.Mantissa, out var remainder );
			while ( remainder != 0 && result.SignifigantDigits < divisor.SignifigantDigits ) {
				while ( BigInteger.Abs( remainder ) < BigInteger.Abs( divisor.Mantissa ) ) {
					remainder *= 10;
					result.Mantissa *= 10;
					counter++;
					//remString = remainder.ToString();
					//mantissaString = result.Mantissa.ToString();
				}

				result.Mantissa += BigInteger.DivRem( remainder, divisor.Mantissa, out remainder );

				//remString = remainder.ToString();
				//mantissaString = result.Mantissa.ToString();
			}

			result.Exponent = exponentChange - counter;
			return result;
		}

		/// <summary>
		///     Returns e raised to the specified power
		/// </summary>
		public static BigDecimal Exp( Double exponent ) {
			var tmp = ( BigDecimal )1;
			while ( Math.Abs( exponent ) > 100 ) {
				var diff = exponent > 0 ? 100 : -100;
				tmp *= Math.Exp( diff );
				exponent -= diff;
			}

			return tmp * Math.Exp( exponent );
		}

		/// <summary>
		///     Returns e raised to the specified power
		/// </summary>
		public static BigDecimal Exp( BigInteger exponent ) {
			var tmp = ( BigDecimal )1;
			while ( BigInteger.Abs( exponent ) > 100 ) {
				var diff = exponent > 0 ? 100 : -100;
				tmp *= Math.Exp( diff );
				exponent -= diff;
			}

			var exp = ( Double )exponent;
			return tmp * Math.Exp( exp );
		}

		/// <summary>
		///     Returns a specified number raised to the specified power.
		/// </summary>
		public static BigDecimal Pow( BigDecimal baseValue, BigInteger exponent ) {
			if ( exponent.IsZero ) {
				return One;
			}

			if ( exponent.Sign < 0 ) {
				if ( baseValue == Zero ) {
					throw new NotSupportedException( "Cannot raise zero to a negative power" );
				}

				// n^(-e) -> (1/n)^e
				baseValue = One / baseValue;
				exponent = BigInteger.Negate( exponent );
			}

			var result = baseValue;
			while ( exponent > BigInteger.One ) {
				result = result * baseValue;
				exponent--;
			}

			return result;
		}

		/// <summary>
		///     Returns a specified number raised to the specified power.
		/// </summary>
		public static BigDecimal Pow( Double basis, Double exponent ) {
			var tmp = ( BigDecimal )1;
			while ( Math.Abs( exponent ) > 100 ) {
				var diff = exponent > 0 ? 100 : -100;
				tmp *= Math.Pow( basis, diff );
				exponent -= diff;
			}

			return tmp * Math.Pow( basis, exponent );
		}

		/// <summary>
		///     Returns the absolute value of the BigDecimal
		/// </summary>
		public static BigDecimal Abs( BigDecimal value ) {
			if ( value.IsNegative ) {
				return value * -1;
			}

			return value;
		}

		/// <summary>
		///     Rounds a BigDecimal value to the nearest integral value.
		/// </summary>
		public static BigInteger Round( BigDecimal value ) => Round( value, MidpointRounding.AwayFromZero );

		/// <summary>
		///     Rounds a BigDecimal value to the nearest integral value. A parameter specifies how to round the value if it is
		///     midway between two numbers.
		/// </summary>
		public static BigInteger Round( BigDecimal value, MidpointRounding mode ) {
			//Normalize( value );

			var wholePart = value.WholeValue;
			var decimalPart = value.GetFractionalPart();

			BigInteger addOne = value.IsNegative ? -1 : 1;

			if ( decimalPart > OneHalf ) {
				wholePart += addOne;
			}
			else if ( decimalPart == OneHalf ) {
				if ( mode == MidpointRounding.AwayFromZero ) {
					wholePart += addOne;
				}
				else // MidpointRounding.ToEven
				{
					if ( !wholePart.IsEven ) {
						wholePart += addOne;
					}
				}
			}

			return wholePart;
		}

		/// <summary>
		///     Rounds a BigDecimal to an integer value. The BigDecimal argument is rounded towards positive infinity.
		/// </summary>
		public static BigDecimal Ceiling( BigDecimal value ) {
			//Normalize( value );

			BigDecimal result = value.WholeValue;

			if ( result != value.Mantissa && value >= 0 ) {
				result += 1;
			}

			return result;
		}

		public static BigDecimal Floor( BigDecimal value ) {
			//Normalize( value );

			BigDecimal result = value.WholeValue;

			if ( result != value.Mantissa && value <= 0 ) {
				result -= 1;
			}

			return result;
		}

		[SuppressMessage( "ReSharper", "NonReadonlyMemberInGetHashCode" )]
		public override Int32 GetHashCode() => HashCode.Combine( this.Mantissa, this.Exponent );

		public override String ToString() => this.ToString( BigDecimalNumberFormatInfo );

		public String ToString( IFormatProvider provider ) => ToString( this.Mantissa, this.Exponent, provider );

		private static String ToString( BigInteger mantissa, Int32 exponent, IFormatProvider provider ) {
			if ( provider is null ) {
				throw new ArgumentNullException( nameof( provider ) );
			}

			var formatProvider = NumberFormatInfo.GetInstance( provider );

			var negativeValue = mantissa.Sign == -1;
			var negativeExponent = Math.Sign( exponent ) == -1;

			var result = BigInteger.Abs( mantissa ).ToString();
			var absExp = Math.Abs( exponent );

			if ( negativeExponent ) {
				if ( absExp > result.Length ) {
					var zerosToAdd = Math.Abs( absExp - result.Length );
					var zeroString = String.Join( String.Empty, Enumerable.Repeat( formatProvider.NativeDigits[ 0 ], zerosToAdd ) );
					result = zeroString + result;
					result = result.Insert( 0, formatProvider.NumberDecimalSeparator );
					result = result.Insert( 0, formatProvider.NativeDigits[ 0 ] );
				}
				else {
					var indexOfRadixPoint = Math.Abs( absExp - result.Length );
					result = result.Insert( indexOfRadixPoint, formatProvider.NumberDecimalSeparator );
					if ( indexOfRadixPoint == 0 ) {
						result = result.Insert( 0, formatProvider.NativeDigits[ 0 ] );
					}
				}

				result = result.TrimEnd( '0' );
				if ( result.Last().ToString() == formatProvider.NumberDecimalSeparator ) {
					result = result[ ..^1 ];
				}
			}
			else {
				var zeroString = String.Join( String.Empty, Enumerable.Repeat( formatProvider.NativeDigits[ 0 ], absExp ) );
				result += zeroString;
			}

			if ( negativeExponent ) {
				//TODO Prefix "0."
			}

			if ( negativeValue ) {
				// Prefix "-"
				return result?.Insert( 0, formatProvider.NegativeSign ) ?? String.Empty;
			}

			return result ?? String.Empty;
		}

	}

}