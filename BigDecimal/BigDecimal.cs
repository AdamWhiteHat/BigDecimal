#nullable enable

namespace ExtendedNumerics;

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Exceptions;
using Properties;
using Reflection;

/// <summary>
///     <para>Arbitrary precision decimal. All operations are exact, except for division.</para>
///     <para>Division never determines more digits than the given precision.</para>
///     <para>Based on code by Jan Christoph Bernack (http://stackoverflow.com/a/4524254 or jc.bernack at gmail.com)</para>
///     <para>Modified and extended by Adam White (https://csharpcodewhisperer.blogspot.com/)</para>
///     <para>Further modified by Rick Harker, Rick.Rick.Harker@gmail.com</para>
/// </summary>
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[Immutable]
public readonly record struct BigDecimal : IComparable<BigDecimal>, IComparable<Int32> {

	private const String NumericCharacters = "-0.1234567890";

	private const String NullString = "(null)";

	static BigDecimal() {
		try {
			Pi = Parse(
				"3.14159265358979323846264338327950288419716939937510582097494459230781640628620899862803482534211706798214808651328230664709384460955058223172535940812848111745028410270193852110555964462294895493038196" );
		}
		catch ( Exception exception ) {
			Debug.WriteLine( exception );
		}

		try {
			π = GetPiDigits();
		}
		catch ( Exception exception ) {
			Debug.WriteLine( exception );
		}

		try {
			E = Parse(
				"2.71828182845904523536028747135266249775724709369995957496696762772407663035354759457138217852516642749193200305992181741359662904357290033429526059563073813232862794349076323382988075319525101901157383" );
		}
		catch ( Exception exception ) {
			Debug.WriteLine( exception );
		}
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

		if ( AlwaysNormalize ) {
			result = Normalize( this );
			this.Mantissa = result.Mantissa;
			this.Exponent = result.Exponent;
		}
	}

	public static BigDecimal Ten => 10;

	public static BigDecimal One => 1;

	public static BigDecimal Zero => 0;

	public static BigDecimal OneHalf => 0.5d;

	public static BigDecimal MinusOne => -1;

	public static BigDecimal E { get; }

	public static BigDecimal Pi { get; }

	public static BigDecimal π { get; }

	private static BigInteger TenInt { get; } = new(10);

	private static NumberFormatInfo BigDecimalNumberFormatInfo { get; } = CultureInfo.InvariantCulture.NumberFormat;

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

	public BigInteger Mantissa { get; init; }

	public Int32 Exponent { get; init; }

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

	/*
	public static explicit operator Single( BigDecimal value ) {
		if ( !Single.TryParse( value.Mantissa.ToString(), out var mantissa ) ) {
			mantissa = Convert.ToSingle( value.Mantissa.ToString() );
		}

		return mantissa * ( Single ) Math.Pow( 10, value.Exponent );
	}
	*/

	private static Lazy<BigDecimal> MaxBigDecimalForDecimal => new(() => Decimal.MaxValue);

	private static Lazy<BigDecimal> MaxBigDecimalForSingle => new(() => Single.MaxValue);

	private static Lazy<BigDecimal> MaxBigDecimalForDouble => new(() => Double.MaxValue);

	private static Lazy<BigDecimal> MaxBigDemicalForInt32 => new(() => Int32.MaxValue);

	private static Lazy<BigDecimal> MaxBigDemicalForUInt32 => new(() => UInt32.MaxValue);

	/*

	/// <summary>
	/// For the old <see cref="IComparable" /> interface.
	/// </summary>
	/// <param name="obj"></param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
	public Int32 CompareTo( Object? obj ) {
		if ( obj is BigDecimal @decimal ) {
			return this.CompareTo( @decimal );
		}

		throw new InvalidOperationException( $"Could not convert object {obj} to a {nameof( BigDecimal )}." );
	}
	*/

	/// <summary>
	///     Compares two BigDecimal values, returning an integer that indicates their relationship.
	/// </summary>
	/// <remarks>For IComparable&lt;BigDecimal&gt;</remarks>
	public Int32 CompareTo( BigDecimal other ) =>
		this > other ? SortingOrder.After :
		this < other ? SortingOrder.Before : SortingOrder.Same;

	public Int32 CompareTo( Int32 other ) =>
		this < other ? SortingOrder.Before :
		this > other ? SortingOrder.After : SortingOrder.Same;

	public Boolean Equals( BigDecimal? other ) => Equals( this, other );

	/// <summary>
	///     Load the value of π up to 1 million digits.
	///     <para>Defaults to 512 digits.</para>
	/// </summary>
	/// <param name="digits"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	/// <exception cref="InvalidOperationException"></exception>
	public static BigDecimal GetPiDigits( Int32 digits = 512 ) {
		digits += "3.".Length;
		if ( digits is <= 0 or > 1_000_000 ) {
			throw new ArgumentOutOfRangeException( nameof( digits ) );
		}

		var s = Resources.PiString?[ ..( 2 + digits ) ];

		if ( String.IsNullOrWhiteSpace( s ) ) {
			throw new InvalidOperationException( $"Unable to load the value of π.[{digits}] from resources." );
		}

		return Parse( s );
	}

	/// <summary>
	///     Static equality test.
	/// </summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	/// <returns></returns>
	public static Boolean Equals( BigDecimal? left, BigDecimal? right ) {
		if ( left is null || right is null ) {
			return false;
		}

		return left.Value.Mantissa.Equals( right.Value.Mantissa ) && left.Value.Exponent.Equals( right.Value.Exponent ) && left.Value.Sign.Equals( right.Value.Sign );
	}

	public Int32 CompareTo( Int32? other ) =>
		other is null || this > other ? SortingOrder.After :
		this < other ? SortingOrder.Before : SortingOrder.Same;

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
	public static BigDecimal Parse( String input ) {
		if ( String.IsNullOrEmpty( input = input.Trim() ) ) {
			return BigInteger.Zero;
		}

		/*

		//TODO Can we just shortcut with these, just use the casts?
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

		if ( input.StartsWith( BigDecimalNumberFormatInfo.NegativeSign ) ) {
			isNegative = true;
			input = input.Replace( BigDecimalNumberFormatInfo.NegativeSign, String.Empty );
		}

		var posE = input.LastIndexOf( 'E' ) + 1;

		if ( posE > 0 ) {
			var sE = input[ posE.. ];

			if ( Int32.TryParse( sE, out exponent ) ) {
				//Trim off the exponent
				input = input[ ..( posE - 1 ) ];
			}
		}

		if ( input.Contains( BigDecimalNumberFormatInfo.NumberDecimalSeparator ) ) {
			var decimalPlace = input.IndexOf( BigDecimalNumberFormatInfo.NumberDecimalSeparator, StringComparison.Ordinal );

			exponent += decimalPlace + 1 - input.Length;
			input = input.Replace( BigDecimalNumberFormatInfo.NumberDecimalSeparator, String.Empty );
		}

		var mantissa = BigInteger.Parse( input );
		if ( isNegative ) {
			mantissa = BigInteger.Negate( mantissa );
		}

		var result = new BigDecimal( mantissa, exponent );

		if ( AlwaysTruncate ) {
			result = Truncate( result, Precision );
		}

		if ( AlwaysNormalize ) {
			result = Normalize( result );
		}

		return result;
	}

	/// <summary>
	///     Truncates the BigDecimal to the given precision by removing the least significant digits.
	/// </summary>
	public static BigDecimal Truncate( BigDecimal value, Int32 precision ) {
		var mantissa = value.Mantissa;
		var exponent = value.Exponent;

		var sign = Math.Sign( exponent );
		var difference = ( precision - GetSignifigantDigits( mantissa ) ) * -1;
		if ( difference >= 1 ) {
			mantissa = BigInteger.Divide( mantissa, BigInteger.Pow( TenInt, difference ) );
			if ( sign != 0 ) {
				exponent += sign * difference;
			}
		}

		return new BigDecimal( mantissa, exponent );
	}

	/// <summary>
	///     Truncate the number to the given precision by removing the least significant digits.
	/// </summary>
	/// <returns>The truncated number</returns>
	public BigDecimal TruncateAlt( Int32 precision ) {
		// save some time because the number of digits is not needed to remove trailing zeros
		var shortened = Normalize( this );

		var mantissa = shortened.Mantissa;
		var exponent = shortened.Exponent;

		// remove the least significant digits, as long as the number of digits is higher than the given Precision
		while ( NumberOfDigits( mantissa ) > precision ) {
			mantissa /= 10;
			++exponent;
		}

		return new BigDecimal( mantissa, exponent );
	}

	public static Int32 NumberOfDigits( BigInteger value ) => ( Int32 ) Math.Ceiling( BigInteger.Log10( value * value.Sign ) );

	/// <summary>
	///     Truncates the BigDecimal to the current precision by removing the least significant digits.
	/// </summary>
	public BigDecimal Truncate() => this.TruncateAlt( Precision );

	/// <summary>
	///     Removes any trailing zeros on the mantissa, adjusts the exponent, and returns a new <see cref="BigDecimal" />.
	/// </summary>
	/// <param name="value"></param>
	[Pure]
	public static BigDecimal Normalize( BigDecimal value ) {
		if ( value.IsZero ) {
			return value;
		}

		var s = value.Mantissa.ToString();
		var pos = s.LastIndexOf( '0', s.Length - 1 );

		if ( pos < s.Length - 1 ) {
			return value;
		}

		var c = s[ pos ];

		while ( pos > 0 && c == '0' ) {
			c = s[ --pos ]; //scan backwards to find the last not-0.
		}

		var mant = s[ ..( pos + 1 ) ];
		var zeros = s[ ( pos + 1 ).. ];

		if ( BigInteger.TryParse( mant, out var mantissa ) ) {
			return value with {
				Mantissa = mantissa,
				Exponent = value.Exponent + zeros.Length
			};
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
	[ThisNeedsTesting]
	public BigInteger GetWholePart() {
		var resultString = String.Empty;
		var decimalString = this.ToString( BigDecimalNumberFormatInfo );

		var valueSplit = decimalString.Split( '.', StringSplitOptions.RemoveEmptyEntries );
		if ( valueSplit.Length > 0 ) {
			resultString = valueSplit[ 0 ];
		}

		var posE = resultString.IndexOf( 'E', StringComparison.Ordinal );
		if ( posE > 0 ) {
			resultString = resultString.Split( 'E', StringSplitOptions.RemoveEmptyEntries )[ 0 ];
		}

		return BigInteger.Parse( resultString );
	}

	/// <summary>
	///     Gets the fractional part of the BigDecimal, setting everything left of the decimal point to zero.
	/// </summary>
	[ThisNeedsTesting]  //TODO Needs tests on E notation, negative numbers, and extra small or large numbers.
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

		var newmantissa = BigInteger.Parse( resultString.TrimStart( '0' ) );
		var result = new BigDecimal( newmantissa, 0 - resultString.Length );
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

		var s = value.ToString();
		if ( String.IsNullOrWhiteSpace( s ) ) {
			return 0;
		}

		s = new String( s.Trim().Where( c => NumericCharacters.Contains( c ) ).ToArray() ); //TODO Why Trim? Why the Where?
		s = s.TrimEnd( '0' )
		     .Replace( BigDecimalNumberFormatInfo.NegativeSign, String.Empty )
		     .Replace( BigDecimalNumberFormatInfo.PositiveSign, String.Empty )
		     .Replace( BigDecimalNumberFormatInfo.NumberDecimalSeparator, String.Empty );

		return s.Length;
	}

	/// <summary>
	///     Returns the mantissa of value, aligned to the exponent of reference. Assumes the exponent of value is larger than
	///     of reference.
	/// </summary>
	private static BigInteger AlignExponent( BigDecimal value, BigDecimal reference ) => value.Mantissa * BigInteger.Pow( TenInt, value.Exponent - reference.Exponent );

	public static implicit operator BigDecimal( BigInteger value ) => new(value, 0);

	public static implicit operator BigDecimal( Byte value ) => new(new BigInteger( value ), 0);

	public static implicit operator BigDecimal( SByte value ) => new(new BigInteger( value ), 0);

	public static implicit operator BigDecimal( UInt32 value ) => new(new BigInteger( value ), 0);

	public static implicit operator BigDecimal( Int32 value ) => new(new BigInteger( value ), 0);

	public static implicit operator BigDecimal( UInt16 value ) => new(new BigInteger( value ), 0);

	public static implicit operator BigDecimal( Int16 value ) => new(new BigInteger( value ), 0);

	public static implicit operator BigDecimal( UInt64 value ) => new(new BigInteger( value ), 0);

	public static implicit operator BigDecimal( Int64 value ) => new(new BigInteger( value ), 0);

	public static implicit operator BigDecimal( Single value ) => new(value);

	//public static implicit operator BigDecimal( Double value ) => new(value);

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

	public static implicit operator BigDecimal( Double value ) {
		var s = value.ToString( "G17", BigDecimalNumberFormatInfo ); //G17 according to the Microsoft docs.
		return Parse( s );

		/*
		var mantissa = ( BigInteger )value;
		var exponent = 0;
		Double scaleFactor = 1;
		while ( Math.Abs( value * scaleFactor - ( Double )mantissa ) > 0 ) {
			exponent--;
			scaleFactor *= 10;
			mantissa = ( BigInteger )( value * scaleFactor );
		}

		return new BigDecimal( mantissa, exponent );
		*/
	}

	public static explicit operator BigInteger( BigDecimal value ) {
		var floored = Floor( value );
		return floored.Mantissa * BigInteger.Pow( 10, floored.Exponent );
	}

	/*
	public static explicit operator BigInteger( BigDecimal value ) {
		if ( value.Exponent < 0 ) {
			var mant = value.Mantissa.ToString();

			var length = value.GetDecimalIndex();
			if ( length > 0 ) {
				return BigInteger.Parse( mant[ ..length ] );
			}

			if ( length == 0 ) {
				var tenthsPlace = Int32.Parse( mant[ 0 ].ToString() );
				return tenthsPlace >= 5 ? new BigInteger( 1 ) : BigInteger.Zero;
			}

			return BigInteger.Zero;
		}

		return BigInteger.Multiply( value.Mantissa, BigInteger.Pow( TenInt, value.Exponent ) );
	}
	*/

	/// <summary>
	///     Converts <paramref name="value" /> to an <see cref="Double" /> if possible, otherwise throws
	///     <see
	///         cref="OutOfRangeException" />
	///     .
	/// </summary>
	/// <param name="value"></param>
	/// <exception cref="OutOfRangeException"></exception>
	public static explicit operator Double( BigDecimal value ) {
		var t = ( BigDecimal ) value.Mantissa * Math.Pow( 10, value.Exponent );
		if ( t > MaxBigDecimalForDouble.Value ) {
			throw new OutOfRangeException( $"{nameof( BigDecimal )} is too large to convert to a {nameof( Double )}." );
		}

		return ( Double ) t;
	}

	/// <summary>
	///     Converts <paramref name="value" /> to an <see cref="Single" /> if possible, otherwise throws
	///     <see
	///         cref="OutOfRangeException" />
	///     .
	/// </summary>
	/// <param name="value"></param>
	/// <exception cref="OutOfRangeException"></exception>
	public static explicit operator Single( BigDecimal value ) {
		var t = ( BigDecimal ) value.Mantissa * Math.Pow( 10, value.Exponent );
		if ( t > MaxBigDecimalForSingle.Value ) {
			throw new OutOfRangeException( $"{nameof( BigDecimal )} is too large to convert to a {nameof( Single )}." );
		}

		return ( Single ) t;
	}

	/// <summary>
	///     Converts <paramref name="value" /> to an <see cref="Decimal" /> if possible, otherwise throws
	///     <see
	///         cref="OutOfRangeException" />
	///     .
	/// </summary>
	/// <param name="value"></param>
	/// <exception cref="OutOfRangeException"></exception>
	public static explicit operator Decimal( BigDecimal value ) {
		var t = ( BigDecimal ) value.Mantissa * Math.Pow( 10, value.Exponent );
		if ( t > MaxBigDecimalForDecimal.Value ) {
			throw new OutOfRangeException( $"{nameof( BigDecimal )} is too large to convert to a {nameof( Decimal )}." );
		}

		return ( Decimal ) t;
	}

	/// <summary>
	///     Converts <paramref name="value" /> to an <see cref="Int32" /> if possible, otherwise throws
	///     <see
	///         cref="OutOfRangeException" />
	///     .
	/// </summary>
	/// <param name="value"></param>
	/// <exception cref="OutOfRangeException"></exception>
	public static explicit operator Int32( BigDecimal value ) {
		var t = value.Mantissa * Pow( 10, value.Exponent );
		if ( t > MaxBigDemicalForInt32.Value ) {
			throw new OutOfRangeException( $"{nameof( BigDecimal )} is too large to convert to a {nameof( Int32 )}." );
		}

		return ( Int32 ) t;
	}

	/// <summary>
	///     Converts <paramref name="value" /> to an <see cref="UInt32" /> if possible, otherwise throws
	///     <see
	///         cref="OutOfRangeException" />
	///     .
	/// </summary>
	/// <param name="value"></param>
	/// <exception cref="OutOfRangeException"></exception>
	public static explicit operator UInt32( BigDecimal value ) {
		var t = value.Mantissa * Pow( 10, value.Exponent );
		if ( t > MaxBigDemicalForUInt32.Value ) {
			throw new OutOfRangeException( $"{nameof( BigDecimal )} is too large to convert to a {nameof( UInt32 )}." );
		}

		return ( UInt32 ) t;
	}

	[ThisNeedsTesting]
	public static BigDecimal operator %( BigDecimal left, BigDecimal right ) => left - Floor( right * ( left / right ) );

	public static BigDecimal operator +( BigDecimal value ) => value;

	public static BigDecimal operator -( BigDecimal value ) => Negate( value );

	public static BigDecimal operator ++( BigDecimal value ) => Add( value, 1 );

	public static BigDecimal operator --( BigDecimal value ) => Subtract( value, 1 );

	public static BigDecimal operator +( BigDecimal left, BigDecimal right ) => Add( left, right );

	public static BigDecimal operator -( BigDecimal left, BigDecimal right ) => Subtract( left, right );

	public static BigDecimal operator *( BigDecimal left, BigDecimal right ) => Multiply( left, right );

	public static BigDecimal operator /( BigDecimal dividend, BigDecimal divisor ) => Divide( dividend, divisor );

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
	public static BigDecimal Negate( BigDecimal value ) =>
		value with {
			Mantissa = BigInteger.Negate( value.Mantissa )
		};

	/// <summary>
	///     Adds two BigDecimal values.
	/// </summary>
	public static BigDecimal Add( BigDecimal left, BigDecimal right ) {
		if ( left.Exponent > right.Exponent ) {
			return new BigDecimal( AlignExponent( left, right ) + right.Mantissa, right.Exponent );
		}

		return new BigDecimal( AlignExponent( right, left ) + left.Mantissa, left.Exponent );
	}

	/// <summary>
	///     Subtracts two BigDecimal values.
	/// </summary>
	public static BigDecimal Subtract( BigDecimal left, BigDecimal right ) => Add( left, Negate( right ) );

	/// <summary>
	///     Multiplies two BigDecimal values.
	/// </summary>
	public static BigDecimal Multiply( BigDecimal left, BigDecimal right ) => new(left.Mantissa * right.Mantissa, left.Exponent + right.Exponent);

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
		if ( divisor == Zero ) {
			throw new DivideByZeroException( nameof( divisor ) );
		}

		// if (dividend > divisor) { return Divide_Positive(dividend, divisor); }

		if ( Abs( dividend ) == 1 ) {
			//TODO This just shortcuts with System.Double doing the division. Not really accurate..

			var doubleDivisor = Double.Parse( divisor.ToString() );
			doubleDivisor = 1d / doubleDivisor;

			return Parse( doubleDivisor.ToString( CultureInfo.CurrentCulture ) );
		}

		var exponentChange = dividend.Exponent - divisor.Exponent;

		var counter = 0;

		var mantissa = BigInteger.DivRem( dividend.Mantissa, divisor.Mantissa, out var remainder );

		while ( remainder != 0 && GetSignifigantDigits( mantissa ) < divisor.SignifigantDigits ) {
			while ( BigInteger.Abs( remainder ) < BigInteger.Abs( divisor.Mantissa ) ) {
				remainder *= 10;
				mantissa *= 10;
				counter++;
			}

			mantissa += BigInteger.DivRem( remainder, divisor.Mantissa, out remainder );
		}

		return new BigDecimal( mantissa, exponentChange - counter );
	}

	/*
	public static BigDecimal operator /( BigDecimal dividend, BigDecimal divisor ) {
		var exponentChange = Precision - ( NumberOfDigits( dividend.Mantissa ) - NumberOfDigits( divisor.Mantissa ) );
		if ( exponentChange < 0 ) {
			exponentChange = 0;
		}

		dividend.Mantissa *= BigInteger.Pow( 10, exponentChange );
		return new BigDecimal( dividend.Mantissa / divisor.Mantissa, dividend.Exponent - divisor.Exponent - exponentChange );
	}
	*/

	/// <summary>
	///     Returns e raised to the specified power
	/// </summary>
	public static BigDecimal Exp( Double exponent ) {
		var tmp = One;
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
		var tmp = One;
		while ( BigInteger.Abs( exponent ) > 100 ) {
			var diff = exponent > 0 ? 100 : -100;
			tmp *= Math.Exp( diff );
			exponent -= diff;
		}

		var exp = ( Double ) exponent;
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
			result *= baseValue;
			exponent--;
		}

		return result;
	}

	/// <summary>
	///     Returns a specified number raised to the specified power.
	/// </summary>
	public static BigDecimal Pow( Double basis, Double exponent ) {
		var tmp = One;
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
	public static BigDecimal Abs( BigDecimal value ) => value.IsNegative ? value * -1 : value;

	/// <summary>
	///     Rounds a BigDecimal value to the nearest integral value.
	/// </summary>
	public static BigInteger Round( BigDecimal value ) => Round( value, MidpointRounding.AwayFromZero );

	/// <summary>
	///     Rounds a BigDecimal value to the nearest integral value. A parameter specifies how to round the value if it is
	///     midway
	///     between two numbers.
	/// </summary>
	public static BigInteger Round( BigDecimal value, MidpointRounding mode ) {
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
	[ThisNeedsTesting]
	public static BigDecimal Ceiling( BigDecimal value ) {
		BigDecimal result = value.WholeValue;

		if ( result != value.Mantissa && value >= 0 ) {
			result += 1;
		}

		return result;
	}

	[ThisNeedsTesting]
	public BigDecimal FloorAlt() => Truncate( NumberOfDigits( this.Mantissa ), this.Exponent );

	public static BigDecimal Floor( BigDecimal value ) {
		BigDecimal result = value.WholeValue;

		if ( result != value.Mantissa && value <= 0 ) {
			result -= 1;
		}

		return result;
	}

	public override Int32 GetHashCode() => HashCode.Combine( this.Mantissa, this.Exponent );

	public override String ToString() {
		var result = this.ToString( BigDecimalNumberFormatInfo );
		return result;
	}

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
				var zeroString = String.Concat( Enumerable.Repeat( formatProvider.NativeDigits[ 0 ], zerosToAdd ) );
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
			var zeroString = String.Concat( Enumerable.Repeat( formatProvider.NativeDigits[ 0 ], absExp ) );
			result += zeroString;
		}

		if ( negativeExponent ) {
			//TODO // Prefix "0."?
		}

		if ( negativeValue ) {
			// Prefix "-"
			result = result.Insert( 0, formatProvider.NegativeSign );
		}

		return result;
	}

	public static String UnNormalize( BigInteger mantissa, Int32 exponent, IFormatProvider provider ) {
		if ( provider is null ) {
			throw new ArgumentNullException( nameof( provider ) );
		}

		var formatProvider = NumberFormatInfo.GetInstance( provider );

		var negativeExponent = Math.Sign( exponent ) == -1;

		var result = BigInteger.Abs( mantissa ).ToString();
		var absExp = Math.Abs( exponent );

		if ( negativeExponent ) {
			if ( absExp > result.Length ) {
				var zerosToAdd = Math.Abs( absExp - result.Length );

				//var zeroString = String.Join( String.Empty, Enumerable.Repeat( formatProvider.NativeDigits[ 0 ], zerosToAdd ) );
				var zeroString = String.Empty.PadRight( zerosToAdd, formatProvider.NativeDigits[ 0 ][ 0 ] );
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
			var zeroString = String.Concat( Enumerable.Repeat( formatProvider.NativeDigits[ 0 ], absExp ) );
			result += zeroString;
		}

		if ( mantissa.Sign == -1 ) {
			return $"{formatProvider.NegativeSign}{result}";
		}

		return result;
	}

	/// <summary>
	///     Allow the BigDecimal to be formatted with the E notation.
	/// </summary>
	/// <param name="bigDecimal"></param>
	/// <returns></returns>
	[ThisNeedsTesting]
	private static String ToScientificENotation( BigDecimal bigDecimal ) {
		// 1.238E456 1.238E-456
		// -1.238E456
		// -1.238E-456 1E456 1E-456
		// -1E456
		// -1E-456
		// -3E-2

		var manString = bigDecimal.Mantissa.ToString(); //Note: will be prefixed with "-" if negative.

		var period = manString.Length - bigDecimal.Exponent + 1;
		if ( bigDecimal.Mantissa.Sign == -1 ) {
			++period;
		}

		//TODO none of this is tested or guaranteed to work yet. Like negatives, or small numbers need the correct logic.

		var result2 = $"{manString.Insert( period, "." )}E{bigDecimal.Exponent:D}";
		return result2;
	}

}