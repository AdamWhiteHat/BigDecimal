#nullable enable

namespace ExtendedNumerics.Exceptions;

using System;
using System.Runtime.Serialization;

[Serializable]
public class ImmutableFailureException : Exception {

	public ImmutableFailureException() { }

	public ImmutableFailureException( String? message ) : base( message ) { }

	public ImmutableFailureException( String? message, Exception? innerException ) : base( message, innerException ) { }

	public ImmutableFailureException( SerializationInfo serializationInfo, StreamingContext streamingContext ) : base( serializationInfo, streamingContext ) {
		if ( serializationInfo is null ) {
			throw new ArgumentNullException( nameof( serializationInfo ) );
		}
	}

	public ImmutableFailureException( Type? type, String? message, Exception? inner ) : base( message, inner ) => this.Type = type;

	public ImmutableFailureException( Type? type, String? message ) : base( message ) => this.Type = type;

	public Type? Type { get; }

}