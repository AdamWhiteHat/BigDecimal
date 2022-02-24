namespace ExtendedNumerics.Exceptions;

using System;
using System.Reflection;
using System.Runtime.Serialization;

[Serializable]
public class WritableFieldException : ImmutableFailureException {

	protected WritableFieldException( SerializationInfo serializationInfo, StreamingContext streamingContext ) : base( serializationInfo, streamingContext ) {
		if ( serializationInfo is null ) {
			throw new ArgumentNullException( nameof( serializationInfo ) );
		}
	}

	public WritableFieldException( FieldInfo fieldInfo ) : base( fieldInfo.DeclaringType, FormatMessage( fieldInfo ) ) {
		if ( fieldInfo is null ) {
			throw new ArgumentNullException( nameof( fieldInfo ) );
		}
	}

	public WritableFieldException() {
	}

	public WritableFieldException( String? message ) : base( message ) {
	}

	public WritableFieldException( String? message, Exception? innerException ) : base( message, innerException ) {
	}

	public WritableFieldException( Type? type, String? message, Exception? inner ) : base( type, message, inner ) {
	}

	public WritableFieldException( Type? type, String? message ) : base( type, message ) {
	}

	internal static String FormatMessage( FieldInfo fieldInfo ) => $"'{fieldInfo.DeclaringType}' is mutable because field '{fieldInfo.Name}' is not marked 'get'.";
}