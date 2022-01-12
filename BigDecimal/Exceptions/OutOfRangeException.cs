#nullable enable

namespace ExtendedNumerics.Exceptions;

using System;
using System.Runtime.Serialization;

/// <summary>
///     Use when a value is out of range. (Too low or too high)
/// </summary>
[Serializable]
public class OutOfRangeException : Exception {

	/// <summary>
	///     Disallow no message.
	/// </summary>
	private OutOfRangeException() { }

	protected OutOfRangeException( SerializationInfo serializationInfo, StreamingContext streamingContext ) => throw new NotImplementedException();

	public OutOfRangeException( String? message ) : base( message ) {
		if ( String.IsNullOrEmpty( message ) ) {
			throw new NullException( "A message must be provided." );
		}
	}

	public OutOfRangeException( String? message, Exception? inner ) : base( message, inner ) {
		if ( String.IsNullOrEmpty( message ) ) {
			throw new NullException( "A message must be provided." );
		}
	}

}