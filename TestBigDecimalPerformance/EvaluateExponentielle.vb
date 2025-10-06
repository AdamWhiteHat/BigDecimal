Imports System.IO
Imports System.Reflection
Imports ExtendedNumerics
Imports ExtendedNumerics.Helpers
Imports MathNet.Numerics
Imports MathNet.Numerics.RootFinding

Public Module EvaluateExponentielle

    Public iNrDecimals = 100 'BigDecimal.Precision
    Public swLog As StreamWriter
    Public sLogFileName As String
    Public dStart As DateTime
    Public dEnd As DateTime

    Public Sub Main()

        Dim sDate As String = Format(Now, "yyyy-MM-dd")
        Dim sTime As String = Format(Now, "HHmmss")
        sLogFileName = "@Trace." & sDate & "." & sTime & ".log"
        swLog = New StreamWriter(sLogFileName)

        BigDecimal.Precision = 100
        BigDecimal.AlwaysTruncate = True

        BigDecimal.ResetVariablePrecision()

        Dim sExplication As String
        sExplication =
            <text>
Ajout du calcul de e^n lorsque n est un entier
pour vérifier si la fonction BigDecimal.exp(x) est correcte lorsque x > 20 !

BigDecimal.Precision: <%= BigDecimal.Precision %>
BigDecimal.AlwaysTruncate: <%= BigDecimal.AlwaysTruncate %>
ExtendedNumerics.Version: string assemblyVersion = Assembly.LoadFile('your assembly file').GetName().Version.ToString();
            </text>

        swLog.WriteLine(sExplication)

        Dim dStart As DateTime = DateTime.Now

        PrintExpValues(0.0008521094)
        PrintExpValues(0.1)
        PrintExpValues(0.11)
        PrintExpValues(0.2)
        PrintExpValues(0.3)
        PrintExpValues(0.4)
        PrintExpValues(0.4563876236)
        PrintExpValues(0.5)
        PrintExpValues(0.6)
        PrintExpValues(0.7)
        PrintExpValues(0.8123831236)
        PrintExpValues(0.9)
        PrintExpValues(0.9999999999)
        PrintExpValues(1.0)
        PrintExpValues(1.13)
        PrintExpValues(2.0)
        PrintExpValues(BigDecimal.E)
        PrintExpValues(3.0)
        PrintExpValues(BigDecimal.Pi)
        PrintExpValues(BigDecimal.Pi + 0.8)
        PrintExpValues(11.1)
        PrintExpValues(21.4)
        PrintExpValues(201.678)
        PrintExpValues(813)
        PrintExpValues(813 + BigDecimal.Pi)

        PrintExpValues(-0.2008521094)
        PrintExpValues(-0.5)
        PrintExpValues(-10.5)

        Dim dEnd As DateTime = DateTime.Now
        swLog.WriteLine("duration: " & (dEnd - dStart).ToString())
        swLog.Flush()
    End Sub

    Private Function GetNumberString(z As BigDecimal) As String
        Dim sValue As String

        sValue = Left(z.ToString(), iNrDecimals + 1 + BigDecimal.Floor(z).ToString().Length)

        Return sValue
    End Function

    Public Sub PrintExpValues(x As BigDecimal)
        Dim z As BigDecimal
        Dim zFast As BigDecimal

        Console.WriteLine("x: " & Left(x.ToString(), 20))

        swLog.WriteLine("x: " & Left(x.ToString(), 20))

        dStart = DateTime.Now
        zFast = BigDecimal.Exp_Fast_and_Accurate(x)
        dEnd = DateTime.Now

        swLog.WriteLine("  Exp_FAST()  : " & GetNumberString(zFast))
        swLog.WriteLine("    nExp: " & BigDecimal.nExpPrecision)
        swLog.WriteLine("    duration: " & (dEnd - dStart).ToString())

        dStart = DateTime.Now
        zFast = BigDecimal.Exp_Fast2_and_Accurate(x)
        dEnd = DateTime.Now

        swLog.WriteLine("  Exp_FAST2() : " & GetNumberString(zFast))
        swLog.WriteLine("    nExp: " & BigDecimal.nExpPrecision)
        swLog.WriteLine("    duration: " & (dEnd - dStart).ToString())

        dStart = DateTime.Now
        z = BigDecimal.Exp(x)
        dEnd = DateTime.Now

        swLog.WriteLine("  Exp()       : " & GetNumberString(z))
        swLog.WriteLine("    nExp: " & BigDecimal.nExpPrecision)
        swLog.WriteLine("    duration: " & (dEnd - dStart).ToString())

        swLog.WriteLine("  x             : " & GetNumberString(x))
        zFast = BigDecimal.Ln(zFast)
        swLog.WriteLine("    Ln(z_FAST)  : " & GetNumberString(zFast))
        z = BigDecimal.Ln(z)
        swLog.WriteLine("    Ln(z)       : " & GetNumberString(z))
    End Sub

End Module
