namespace ExtendedNumerics.Reflection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Exceptions;

/// <summary>http://blogs.msdn.com/b/kevinpilchbisson/archive/2007/11/20/enforcing-immutability-in-code.aspx</summary>
[AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct )]
public sealed class ImmutableAttribute : Attribute {

	public Boolean OnFaith { get; set; }

	private static Boolean IsMarkedImmutable( Type type ) {
		if ( type is null ) {
			throw new ArgumentNullException( nameof( type ) );
		}

		return type.TypeHasAttribute<ImmutableAttribute>();
	}

	private static Boolean IsWhiteListed( Type type ) {
		if ( type is null ) {
			throw new ArgumentNullException( nameof( type ) );
		}

		// Boolean, int, etc.
		if ( type.IsPrimitive ) {
			return true;
		}

		if ( type == typeof( Object ) ) {
			return true;
		}

		if ( type == typeof( String ) ) {
			return true;
		}

		if ( type == typeof( Guid ) ) {
			return true;
		}

		if ( type.IsEnum ) {
			return true;
		}

		// override all checks on this type if [ImmutableAttribute(OnFaith=true)] is set
		var immutableAttribute = type.GetCustomAttribute<ImmutableAttribute>();

		return immutableAttribute is {OnFaith: true};
	}

	/// <summary>Ensures that 'type' follows the rules for immutability</summary>
	/// <exception cref="ImmutableFailureException">Thrown if a mutability issue appears.</exception>
	public static void VerifyTypeIsImmutable( Type type, IEnumerable<Type> whiteList ) {
		if ( type is null ) {
			throw new ArgumentNullException( nameof( type ) );
		}

		if ( type.BaseType is null ) {
			throw new ArgumentNullException( nameof( type ) );
		}

		if ( whiteList is null ) {
			throw new ArgumentNullException( nameof( whiteList ) );
		}

		var list = whiteList as IList<Type> ?? whiteList.ToList();

		if ( list.Contains( type ) ) {
			return;
		}

		if ( IsWhiteListed( type ) ) {
			return;
		}

		try {
			VerifyTypeIsImmutable( type.BaseType, list );
		}
		catch ( ImmutableFailureException ex ) {
			throw new MutableBaseException( type, ex );
		}

		foreach ( var fieldInfo in type.GetAllDeclaredInstanceFields() ) {
			if ( ( fieldInfo.Attributes & FieldAttributes.InitOnly ) == 0 ) {
				throw new WritableFieldException( fieldInfo );
			}

			// if it's marked with [Immutable], that's good enough, as we can be sure that these tests will all be applied to
			// this type
			if ( IsMarkedImmutable( fieldInfo.FieldType ) ) {
				continue;
			}

			try {
				VerifyTypeIsImmutable( fieldInfo.FieldType, list );
			}
			catch ( ImmutableFailureException ex ) {
				throw new MutableFieldException( fieldInfo, ex );
			}
		}
	}

	/// <summary>Ensures that all types in 'assemblies' that are marked [Immutable] follow the rules for immutability.</summary>
	/// <exception cref="ImmutableFailureException">Thrown if a mutability issue appears.</exception>
	public static void VerifyTypesAreImmutable( IEnumerable<Assembly> assemblies, params Type[] whiteList ) {
		if ( assemblies is null ) {
			throw new ArgumentNullException( nameof( assemblies ) );
		}

		if ( whiteList == null ) {
			throw new ArgumentNullException( nameof( whiteList ) );
		}

		var typesMarkedImmutable = from type in assemblies.GetTypes() where IsMarkedImmutable( type ) select type;

		foreach ( var type in typesMarkedImmutable ) {
			VerifyTypeIsImmutable( type, whiteList );
		}
	}

}