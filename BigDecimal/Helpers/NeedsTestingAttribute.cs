namespace ExtendedNumerics.Helpers;

using System;

/// <summary>Mark that this class needs unit testing to confirm it works as expected.</summary>
[AttributeUsage( AttributeTargets.All )]
internal class NeedsTestingAttribute : Attribute { }