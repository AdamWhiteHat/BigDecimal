// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright must be kept available alongside any use of my source code.
// This source code belongs to Protiguous unless otherwise specified.
// Donations can be made via PayPal at Protiguous@Protiguous.com
// Contact me via email or GitHub https://github.com/Protiguous
// 
// File "GlobalDefines.cs" last touched on 2024-02-27 at 3:46 AM by Protiguous.

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#define HASSPANS
#endif

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#define HASARRAYPOOL
#endif

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#define HASBUFFERS
#endif

#if NET6_0_OR_GREATER
#define HASRANDOMSHARED
#endif

#if NET5_0_OR_GREATER || NET472_OR_GREATER
#define HASHASHCODE
#endif