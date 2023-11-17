namespace ExtendedNumerics.Exceptions;

using System.Reflection;
using System.Runtime.Serialization;

[Serializable]
internal class MutableFieldException : ImmutableFailureException {

	protected MutableFieldException( SerializationInfo serializationInfo, StreamingContext streamingContext ) : base( serializationInfo, streamingContext ) {
		if ( serializationInfo is null ) {
			throw new ArgumentNullException( nameof( serializationInfo ) );
		}
	}

	internal MutableFieldException( FieldInfo fieldInfo, Exception? inner ) : base( fieldInfo.DeclaringType, FormatMessage( fieldInfo ), inner ) {
		if ( fieldInfo is null ) {
			throw new ArgumentNullException( nameof( fieldInfo ) );
		}
	}

	public MutableFieldException() { }

	public MutableFieldException( String? message ) : base( message ) { }

	public MutableFieldException( String? message, Exception? innerException ) : base( message, innerException ) { }

	public MutableFieldException( Type? type, String? message, Exception? inner ) : base( type, message, inner ) { }

	public MutableFieldException( Type? type, String? message ) : base( type, message ) { }

	private static String FormatMessage( FieldInfo fieldInfo ) {
		if ( fieldInfo is null ) {
			throw new ArgumentNullException( nameof( fieldInfo ) );
		}

		return $"'{fieldInfo.DeclaringType}' is mutable because '{fieldInfo.Name}' of type '{fieldInfo.FieldType}' is mutable.";
	}

}