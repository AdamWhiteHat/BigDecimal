# BigDecimal
Arbitrary precision decimal class

BigDecimal is an arbitrary precision floating point number class.

Like other floating point number implementations, it stores a Mantissa and an Exponent.
The difference is, these values are of type BigInteger, and so can be arbitrary precision.

If you just want the compiled binaries, just include it in your project as a nuget package, or extract the assembly from the nuget package (a .nupkg file is just a .zip file renamed): [https://www.nuget.org/packages/ExtendedNumerics.BigDecimal](https://www.nuget.org/packages/ExtendedNumerics.BigDecimal)

Example usage:
```csharp
Console.WriteLine(BigDecimal.Precision);
// 5000
BigDecimal.Precision = 200; // Tone down the precision for this demo.
Console.WriteLine(BigDecimal.Precision);
// 200

BigDecimal goldenRatio = BigDecimal.Divide(BigDecimal.Add(BigDecimal.One, BigDecimal.Pow(5d, 0.5d)), BigDecimal.Parse("2"));
BigDecimal almostInteger = BigDecimal.Pow(goldenRatio, 23);
Console.WriteLine(almostInteger);
// 64079.000015605783843835009599722600391518338454771405992063505171997949372951472701529422358634915404757740005027416333594519349348824890921372720968246769717009339797514969003242216358994087504831741

Console.WriteLine(almostInteger.Mantissa);
// 64079000015605783843835009599722600391518338454771405992063505171997949372951472701529422358634915404757740005027416333594519349348824890921372720968246769717009339797514969003242216358994087504831741
Console.WriteLine(almostInteger.Exponent);
// -193

BigDecimal X = BigDecimal.Parse("0.000551876379690949227373068432671081677704194260485651214128035320088300220750");
Console.WriteLine(X);
// 0.00055187637969094922737306843267108167770419426048565121412803532008830022075

BigDecimal result = BigDecimal.Divide(BigDecimal.One, X);
Console.WriteLine(result);
// 1812.000000000000000000000000000000000000000000000000000000000000000000000001
```
