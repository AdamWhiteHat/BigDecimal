namespace ExtendedNumerics.Reflection;

using System;

/// <summary>Mark that this class needs testing and unit testing to confirm it works as expected.</summary>
[AttributeUsage( AttributeTargets.All )]
public class ThisNeedsTestingAttribute : Attribute { }