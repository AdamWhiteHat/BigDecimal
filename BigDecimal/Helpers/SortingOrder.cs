namespace ExtendedNumerics.Helpers;

/// <summary>
/// <seealso cref="Librainian.Types.CompareToOrder"/>
/// </summary>
public static class SortingOrder
{

	/// <summary>1</summary>
	public const SByte After = 1;

	/// <summary>-1</summary>
	public const SByte Before = -1;

	/// <summary>Default to <see cref="NullsFirst" /> in a sort operation.</summary>
	public const SByte NullsDefault = NullsFirst;

	/// <summary>Return nulls first in a sort operation.</summary>
	public const SByte NullsFirst = Before;

	/// <summary>Return nulls last in a sort operation.</summary>
	public const SByte NullsLast = After;

	/// <summary>0</summary>
	public const SByte Same = 0;
}