// Copyright � Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "MutableBaseException.cs" last formatted on 2021-03-05 at 12:50 PM.

namespace ExtendedNumerics.Exceptions;

using System;
using System.Runtime.Serialization;

[Serializable]
public class MutableBaseException : ImmutableFailureException {

	protected MutableBaseException( SerializationInfo serializationInfo, StreamingContext streamingContext ) : base( serializationInfo, streamingContext ) { }

	internal MutableBaseException( Type type, Exception inner ) : base( type, FormatMessage( type ), inner ) {
		if ( type is null ) {
			throw new ArgumentNullException( nameof( type ) );
		}

		if ( inner is null ) {
			throw new ArgumentNullException( nameof( inner ) );
		}
	}

	public MutableBaseException() { }

	public MutableBaseException( String? message ) : base( message ) { }

	public MutableBaseException( String? message, Exception? innerException ) : base( message, innerException ) { }

	public MutableBaseException( Type? type, String? message, Exception? inner ) : base( type, message, inner ) { }

	public MutableBaseException( Type? type, String? message ) : base( type, message ) { }

	private static String FormatMessage( Type type ) => $"'{type}' is mutable because its base type ('[{type.BaseType}]') is mutable.";

}