namespace ExtendedNumerics.Reflection;

using System.Collections.Generic;
using System.Reflection;

public static class ReflectionHelper
{

	/// <summary>Find all types in 'assembly' that derive from 'baseType'</summary>
	/// <owner>jayBaz</owner>
	public static IEnumerable<Type> FindAllTypesThatDeriveFrom<TBase>( this Assembly assembly )
	{
		if ( assembly is null )
		{
			throw new ArgumentNullException( nameof( assembly ) );
		}

		return assembly.GetTypes().Where( type => type.IsSubclassOf( typeof( TBase ) ) );
	}

	public static IEnumerable<FieldInfo> GetAllDeclaredInstanceFields( this Type type )
	{
		if ( type is null )
		{
			throw new ArgumentNullException( nameof( type ) );
		}

		return type.GetFields( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly );
	}

	/// <summary>A typesafe wrapper for Attribute.GetCustomAttribute</summary>
	/// <remarks>TODO: add overloads for Assembly, Module, and ParameterInfo</remarks>
	public static TAttribute? GetCustomAttribute<TAttribute>( this MemberInfo element ) where TAttribute : Attribute
	{
		if ( element is null )
		{
			throw new ArgumentNullException( nameof( element ) );
		}

		return Attribute.GetCustomAttribute( element, typeof( TAttribute ) ) as TAttribute;
	}

	/// <summary>All types across multiple assemblies</summary>
	public static IEnumerable<Type> GetTypes( this IEnumerable<Assembly> assemblies )
	{
		if ( assemblies is null )
		{
			throw new ArgumentNullException( nameof( assemblies ) );
		}

		return assemblies.SelectMany( assembly => assembly.GetTypes() );
	}

	/// <summary>Check if the given type has the given attribute on it. Don't look at base classes.</summary>
	/// <owner>jayBaz</owner>
	public static Boolean TypeHasAttribute<TAttribute>( this Type type ) where TAttribute : Attribute
	{
		if ( type is null )
		{
			throw new ArgumentNullException( nameof( type ) );
		}

		return Attribute.IsDefined( type, typeof( TAttribute ) );
	}

}