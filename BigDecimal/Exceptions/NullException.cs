namespace ExtendedNumerics.Exceptions;

using System.Runtime.Serialization;

public class NullException : Exception {

	private NullException() {
		//Disallow no message.
	}

	protected NullException( SerializationInfo serializationInfo, StreamingContext streamingContext ) : base( serializationInfo, streamingContext ) { }

	/// <summary>
	///     Don't pass in a null nameof() lol.
	/// </summary>
	/// <param name="nameOfObject"></param>
	public NullException( String nameOfObject ) {
		if ( Debugger.IsAttached ) {
			Debug.WriteLine( $"{nameOfObject}." );
		}
	}

	public NullException( String? message, Exception? innerException ) : base( message, innerException ) { }

}