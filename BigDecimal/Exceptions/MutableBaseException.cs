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