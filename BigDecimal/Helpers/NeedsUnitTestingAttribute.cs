﻿// BigDecimal is copyright by Adam White.
// MIT License.
// https://github.com/AdamWhiteHat/BigDecimal

namespace ExtendedNumerics.Extensions;

/// <summary>
/// Mark that this needs unit testing to confirm it works as expected.
/// </summary>
[AttributeUsage( AttributeTargets.All )]
internal class NeedsUnitTestingAttribute : Attribute
{

	/// <summary>
	/// Mark that this needs unit testing to confirm it works as expected.
	/// </summary>
	public NeedsUnitTestingAttribute()
	{
#if DEBUG
#pragma warning disable CS1030 // #warning directive
#warning This code needs to have tests written to confirm it works as intended.
#pragma warning restore CS1030 // #warning directive
#endif
	}

	/// <summary>
	/// Mark that this needs unit testing to confirm it works as expected for the given <paramref name="reason"/>.
	/// </summary>
	/// <param name="reason"></param>
	public NeedsUnitTestingAttribute( String reason )
	{
#if DEBUG
#pragma warning disable CS1030 // #warning directive
#warning This code needs to have tests written to confirm it works as intended.
#pragma warning restore CS1030 // #warning directive
#endif

		//TestContext.WriteLine( reason ); // Would need the NUnit or Microsoft nuget..
		//reason.WriteLineIfDebugging( true );
	}

}