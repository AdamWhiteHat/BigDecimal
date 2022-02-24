namespace ExtendedNumerics.Reflection;

using System;

public interface INumberTyped {

	public interface IEquals<in T> {

		Boolean Equals( T a, T b );

	}

	public interface INum<T> : IEquals<T>, IShow<T> {

		T Abs( T a );

		T Add( T a, T b );

		T FromInteger( Int32 x );

		T Multiply( T a, T b );

		T Negate( T a );

		T Signum( T a );

		T Subtract( T a, T b );

	}

	public interface IOrdered<in T> : IEquals<T> {

		Int32 Compare( T a, T b );

	}

	public interface IReal<T> : INum<T>, IOrdered<T> { }

	public interface IShow<in T> {

		String Show( T a );

	}

}