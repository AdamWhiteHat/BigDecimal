using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Threading;
using ExtendedNumerics;
using ExtendedNumerics.Helpers;
using NUnit.Framework;

namespace TestBigDecimal
{

	[TestFixture]
	[Culture("en-US,ru-RU")]
	public class TestBigDecimalOperations
	{
		private NumberFormatInfo Format { get { return Thread.CurrentThread.CurrentCulture.NumberFormat; } }

		[Test]
		public void TestAddition001()
		{
			var number1 = BigDecimal.Parse("1234567890");
			var expected = BigDecimal.Parse("3382051537");

			var actual = number1 + 0x7FFFFFFF;

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestAddition002()
		{
			var A = new BigDecimal(new BigInteger(1234567), -1);
			var B = new BigDecimal(new BigInteger(9876543), -9);

			var actual = BigDecimal.Add(A, B);
			var expected = TestBigDecimalHelper.PrepareValue("123456.709876543", this.Format);

			Assert.AreEqual(expected, actual.ToString());
		}

		[Test]
		public void TestBigDecimalPow()
		{
			string expected = "268637376395543538746286686601216000000000000";
			// 5040 ^ 12 = 268637376395543538746286686601216000000000000

			var number = BigDecimal.Parse("5040");
			var exp12 = BigDecimal.Pow(number, 12);
			string actual = exp12.ToString();

			Assert.AreEqual(expected, actual, "5040 ^ 12  =  268637376395543538746286686601216000000000000");
		}


		[Test]
		public void TestIsPositive()
		{
			Assert.IsTrue(BigDecimal.One.IsPositive());
			Assert.IsTrue(BigDecimal.Parse("0.001").IsPositive());
			Assert.IsFalse(BigDecimal.Parse("0").IsPositive());
			Assert.IsFalse(BigDecimal.Parse("-0.00000001").IsPositive());
			Assert.IsFalse(BigDecimal.MinusOne.IsPositive());
		}

		[Test]
		public void TestTruncate_Zero()
		{
			string expected = "0";
			var actual = BigDecimal.Truncate(BigDecimal.Zero);
			Assert.AreEqual(expected, actual.ToString());
		}

		[Test]
		[NonParallelizable]
		public void TestTruncate001()
		{
			string inputTruncated =
				TestBigDecimalHelper.PrepareValue("0.38776413731534507341472294220970933835515664718260518542692164892369393388454765429965711304132864249950074173248631118139885711281403156400182208418498132380665348582256048635378814909035638369142648772177618951899185003568005598389270883746269077440991532220847404333505059368816860680653357748237545067181074698997348124273540082967040205337039719556204791774654186626822192852578297197622945023468587167940717672154626982847038945027431144177383552390076867181200131087852865589018597759807623800948540502708501473286123912110702619773244550322465985979980114779581215743799790210865866959716136152785422203785552850816565888483726280027736811701443283167094373785256268739306209472514414456698923382789454032363968616464876677737866600848986505927023714735496267888826964325695603484817243244809072199216323431074501911199589021095576091452848741385260278621154863084476153732935785975553768625991893151359011912157996422994871709298494233782632174324940552077998861058801035481527689611495569489001108047129667715138204276438217877458404549511313153150137527893798615996618488836664617057038126333313180040094667868763537391421370724952266262848120654117339947389714860375532634890853303061644123428823851175161042458377024247370039795707768407904842511280809838660155394320788005292339449327116941969519022963362256288633034183673183754543670394109495242494711094883817203892173617367313695468521390931623680173196403022932833410798171066411176995128779282872081932608267999112302172207922731784899893348412676098162467010668924433588685153611407005617772276741793479524433622378470125354022566116327819435029065557564148488746638319658731270457607183892425161850287824787546065413294231650473976473355046501500793677901782339691542233183741598103696583351788651177203938936918102755367072014549821942367309956671236707350861545099496206538228683951185018840006137763162355709495445928668480960580978979870379511703883251713690511544429859593313279574155504139941107166963497890833932811052504269372145803660626639428643564562691059910703703938694915154537936003382455188656514686359660013747580119285264755448830584594983111162605867224680013454700621697086948523549156403848856543212816956769085216390639154261614649538130954560421673680672884105498050605587531872704107707071402689983600332112655608194612408217782173036018661692139351433658340756975168361107372727516912020823362368253159826937134217107045868191298957690827125630453728790792408734840661702578638598543186544910552465999106381802375938701350575940262569041045146526024334627822715612658351899764042223444201035443823410277761971257862200600465373558428055133799307959576455801692979753194304758921759067399106319456847661528054181651013888120488047974670158855437555210689546049958555745098303660202886404313365902203237775035723926097742965028613593632230336269392684340085274710999024668887638930755250701806345477524832568256645103704878731032912768646402146422301881142289323523789305126831904241622042944333916620344863470012778933196413192781253025453531244850133026071231714118351386262249150472643870800725983523611903791303553632632769972142502483519860983067322477753824959399886980031912842700140970151007657989042261109130704991895244868527969247414974047405237324669264878742391500642753525622057641241164177505839173992651361990366480244195157062835803031557544691492841007028723179639729081951702197292799161437892952439082270465575308762112590993865133052593362045638622447863872110087219994330766670422412140283392118259566085972052360790645394540700438378734059789109046910356858343004387656915432928337709841252916626015752013241699464443045041876948902728601721842214670716585909801092203893128618468720651888522728597430373030188565238122801065278124235661294292641028550276301054915567825793810248724267437857461336921376742513529432313053995421425528496990787018582251366776291943999044323970133345610820834058391982655766601126736285624213085882525085728598384700565577250732861707158419417137322187913601105221450993534840307771350787020353312910993312574109077271828643659506792514058623896881407687463008239648545371730757776422468606770212802349464720937428992320859723193781752582479518699133569129895070994026410312951649922900688489934852155962598169718199217867461287501481443450777777718726084902136902441480397119384141792970385831367927360530354214563521320900336914169681532166791668676942898880184098720787172114194029069821244937592713815214434788393095503048740886117426353441330676447598548976011441527165748380891340472246800001389307364429687469295246232117792720007673578989468325170179570094184486525355114468774857635615955720968054081874458733938769018227387365842825259166694681823057556598910704367318366050815517174086712448729791746859581490981539042615521145996146036468018904747552880641671500884541141688250485043379587617571474356672696577799617797264760021142950373293666619603196041523466051054935554966409263947312788960948665394281916627352113060683470291466105925", this.Format);
			string inputOverflow = "919919200639429489197056";

			var expected = inputTruncated;

			var longValue = BigDecimal.Parse(String.Concat(inputTruncated, inputOverflow));
			var result = BigDecimal.Round(longValue, 5000);

			var actual = result.ToString();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestTruncate002()
		{
			string expected_floor = Math.Floor(-15.5d).ToString();
			string expected_truncate = Math.Truncate(-15.5d).ToString();

			string actual_floor = BigDecimal.Floor(new BigDecimal(-15.5)).ToString(); // -16
			string actual_truncate = BigDecimal.Truncate(new BigDecimal(-15.5)).ToString(); // -16 (should be -15)

			Assert.AreEqual(expected_floor, actual_floor, "BigDecimal.Floor(-15.5)");
			Assert.AreEqual(expected_truncate, actual_truncate, "BigDecimal.Truncate(-15.5)");
		}

		[Test]
		public void TestCeiling_Zero()
		{
			string expected = "0";
			var actual = BigDecimal.Ceiling(BigDecimal.Zero);
			Assert.AreEqual(expected, actual.ToString(), "Ceiling(0)");
		}

		[Test]
		public void TestCeiling001()
		{
			string expected = "4";

			var input = BigDecimal.Pi;
			BigDecimal actual = BigDecimal.Ceiling(input);

			string info = $"Ceiling({input.ToString().Substring(0, 5)}...) = {actual} ({expected})";
			TestContext.WriteLine(info);
			Assert.AreEqual(expected, actual.ToString(), info);
		}

		[Test]
		public void TestCeiling002()
		{
			string expected = "-3";
			var input = -BigDecimal.Pi;
			string actual = BigDecimal.Ceiling(input).ToString();

			string info = $"Ceiling({input.ToString().Substring(0, 5)}...) = {actual} ({expected})";
			TestContext.WriteLine(info);
			Assert.AreEqual(expected, actual, info);
		}

		[Test]
		public void TestCeiling003()
		{
			var val = TestBigDecimalHelper.PrepareValue("0.14159265", this.Format);
			var start = BigDecimal.Parse(val);
			var ceiling = BigDecimal.Ceiling(start);

			Assert.AreEqual(BigDecimal.One, ceiling);
		}

		[Test]
		public void TestCeiling004()
		{
			var val = TestBigDecimalHelper.PrepareValue("-0.14159265", this.Format);
			var start = BigDecimal.Parse(val);
			var ceiling = BigDecimal.Ceiling(start);

			Assert.AreEqual(BigDecimal.Zero, ceiling);
		}

		[Test]
		public void TestDivide000()
		{
			var val1 = TestBigDecimalHelper.PrepareValue("0.63", this.Format);
			var val2 = TestBigDecimalHelper.PrepareValue("0.09", this.Format);

			var dividend = BigDecimal.Parse(val1);
			var divisor = BigDecimal.Parse(val2);

			var actual = BigDecimal.Divide(dividend, divisor);
			string expected = "7";

			Assert.AreEqual(expected, actual.ToString());
		}

		[Test]
		public void TestDivide001()
		{
			var expected = "40094690950920881030683735292761468389214899724061";

			var dividend = BigDecimal.Parse("1522605027922533360535618378132637429718068114961380688657908494580122963258952897654000350692006139");
			var divisor = BigDecimal.Parse("37975227936943673922808872755445627854565536638199");

			var actual = BigDecimal.Divide(dividend, divisor);

			Assert.AreEqual(expected, actual.ToString());
		}

		[Test]
		public void TestDivide002()
		{
			var resultDividend = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.001", this.Format));
			var resultDivisor = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.5", this.Format));
			var expectedQuotientResult = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.002", this.Format));

			var quotientResult = BigDecimal.Divide(resultDividend, resultDivisor);

			Assert.AreEqual(expectedQuotientResult, quotientResult);
		}

		[Test]
		public void TestDivide003()
		{
			int savePrecision = BigDecimal.Precision;
			BigDecimal.Precision = 11;

			var divisor = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.90606447789", this.Format));
			var actual = BigDecimal.Divide(BigDecimal.One, divisor);
			actual = BigDecimal.Round(actual, 100);

			//var expected = BigDecimal.Parse( "1.1036742134828557" );
			string expected = TestBigDecimalHelper.PrepareValue("1.1036742134", this.Format);

			Assert.AreEqual(expected, actual.ToString());

			BigDecimal.Precision = savePrecision;
		}

		[Test]
		public void TestDivide004()
		{
			var twenty = new BigDecimal(20);
			var actual = BigDecimal.Divide(BigDecimal.One, twenty);
			string expected = TestBigDecimalHelper.PrepareValue("0.05", this.Format);

			Assert.AreEqual(expected, actual.ToString());
		}

		[Test]
		public void TestDivide005()
		{
			var a = new BigDecimal(5);
			var b = new BigDecimal(8);

			var actual = BigDecimal.Divide(a, b);
			string expected = TestBigDecimalHelper.PrepareValue("0.625", this.Format);

			Assert.AreEqual(expected, actual.ToString());
		}

		[Test]
		public void TestDivide006()
		{
			int savePrecision = BigDecimal.Precision;
			BigDecimal.Precision = 12;

			var a = new BigDecimal(1);
			var b = new BigDecimal(7);

			var actual = BigDecimal.Divide(a, b);
			string expected = TestBigDecimalHelper.PrepareValue("0.142857142857", this.Format);

			Assert.AreEqual(expected, actual.ToString());

			BigDecimal.Precision = savePrecision;
		}

		[Test]
		public void TestDivide005A()
		{
			var val1 = TestBigDecimalHelper.PrepareValue("0.5", this.Format);
			var val2 = TestBigDecimalHelper.PrepareValue("0.01", this.Format);

			var value = BigDecimal.Divide(BigDecimal.Parse(val1), BigDecimal.Parse(val2));
			string expected = "50";
			string actual = value.ToString();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestDivide005B()
		{
			var val1 = TestBigDecimalHelper.PrepareValue("0.5", this.Format);
			var val2 = TestBigDecimalHelper.PrepareValue("0.1", this.Format);

			var actual = BigDecimal.Divide(BigDecimal.Parse(val1), BigDecimal.Parse(val2));
			string expected = "5";

			Assert.AreEqual(expected, actual.ToString());
		}

		[Test]
		public void TestDivide005C()
		{
			var val1 = TestBigDecimalHelper.PrepareValue("0.60", this.Format);
			var val2 = TestBigDecimalHelper.PrepareValue("0.01", this.Format);

			var actual = BigDecimal.Divide(BigDecimal.Parse(val1), BigDecimal.Parse(val2));
			string expected = "60";

			Assert.AreEqual(expected, actual.ToString());
		}

		[Test]
		public void TestDivide_50by2_001()
		{
			var actual = BigDecimal.Divide(BigDecimal.Parse("50"), BigDecimal.Parse("2"));
			string expected = "25";

			Assert.AreEqual(expected.ToString(), actual.ToString());
		}

		[Test]
		public void TestDivide_OneOver()
		{
			var numerator = BigDecimal.One;
			var val = TestBigDecimalHelper.PrepareValue("0.141592653589793238462643383279502884197169399375105820974944592307816406286208998628034825342117068", this.Format);
			var denominator = BigDecimal.Parse(val);

			int savePrecision = BigDecimal.Precision;
			BigDecimal.Precision = 100;

			var actual1 = BigDecimal.One / denominator;
			var actual2 = numerator / denominator;
			var actual3 = BigDecimal.Divide(BigDecimal.One, denominator);
			var actual4 = BigDecimal.Divide(numerator, denominator);

			string expectedString = TestBigDecimalHelper.PrepareValue("7.062513305931045769793005152570558042734310025145531333998316873555903337580056083503977475916243946", this.Format);
			var expected = BigDecimal.Parse(expectedString);

			Assert.AreEqual(expectedString, actual1.ToString(), "expectedString != actual1.ToString()");
			Assert.AreEqual(expected.ToString(), actual1.ToString(), "expected.ToString() != actual1.ToString()");
			Assert.AreEqual(expected, actual1, "expected != ( BigDecimal.One / denominator )");
			Assert.AreEqual(expected, actual2, "expected != ( numerator / denominator )");
			Assert.AreEqual(expected, actual3, "expected != ( BigDecimal.Divide(BigDecimal.One, denominator) )");
			Assert.AreEqual(expected, actual4, "expected != ( BigDecimal.Divide(numerator, denominator) )");

			BigDecimal.Precision = savePrecision;
		}

		[Test]
		public void TestFloor_Zero()
		{
			string expected = "0";
			var actual0 = BigDecimal.Floor(BigDecimal.Zero);
			Assert.AreEqual(expected, actual0.ToString(), "Floor(0)");

			var actual1 = BigDecimal.Floor(BigDecimal.Parse("0.01"));
			Assert.AreEqual(expected, actual1.ToString(), "Floor(0.01)");

			var actual2 = BigDecimal.Floor(BigDecimal.Parse("-0.01"));
			Assert.AreEqual("-1", actual2.ToString(), "Floor(-0.01)");
		}

		[Test]
		public void TestFloor001()
		{
			string expected = "3";
			var start = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("3.14159265", this.Format));
			var floor = BigDecimal.Floor(start);

			Assert.AreEqual(expected, floor.ToString());
		}

		[Test]
		public void TestFloor002()
		{
			string expected = "-4";
			var start = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-3.14159265", this.Format));
			var floor = BigDecimal.Floor(start);

			Assert.AreEqual(expected, floor.ToString());
		}

		[Test]
		public void TestFloor003()
		{
			var start = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-0.14159265", this.Format));
			var floor = BigDecimal.Floor(start);

			Assert.AreEqual(BigDecimal.MinusOne, floor);
		}

		[Test]
		public void TestFloor004()
		{
			var start = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.14159265", this.Format));
			var floor = BigDecimal.Floor(start);
			var actual = BigDecimal.Parse(floor.ToString());

			Assert.AreEqual(BigDecimal.Zero, actual);
		}

		[Test]
		public void TestFloor005()
		{
			float start_compare = -2629440f;
			BigDecimal start_test = BigDecimal.Parse("-2629440");

			BigDecimal actualFloor = BigDecimal.Floor(start_test);

			float expectedFloor = (float)Math.Floor(start_compare);

			Assert.AreEqual(expectedFloor.ToString(), actualFloor.ToString());
		}

		[Test]
		public void TestCeiling005()
		{
			float start_compare = -2629440f;
			BigDecimal start_test = BigDecimal.Parse("-2629440");

			BigDecimal actualCeil = BigDecimal.Ceiling(start_test);

			float expectedCeil = (float)Math.Ceiling(start_compare);

			Assert.AreEqual(expectedCeil.ToString(), actualCeil.ToString());
		}

		[Test]
		public void TestMod1()
		{

			// 31 % 19 = 12
			BigDecimal dividend = 31;
			BigDecimal divisor = 19;
			BigDecimal expected = 12;

			var actual = BigDecimal.Mod(dividend, divisor);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestMod2()
		{
			// 1891 % 31 = 0
			BigDecimal dividend = 1891;
			BigDecimal divisor = 31;
			BigDecimal expected = 0;

			var actual = BigDecimal.Mod(dividend, divisor);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestMod3()
		{

			// 6661 % 60 = 1
			BigDecimal dividend = 6661;
			BigDecimal divisor = 60;
			BigDecimal expected = 1;

			var actual = BigDecimal.Mod(dividend, divisor);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestMod4()
		{

			//NOTE This test fails if the values are Doubles instead of Decimals.

			// 31 % 3.66666 = 1.66672
			BigDecimal dividend = 31m;
			BigDecimal divisor = 3.66666m;
			BigDecimal expected = 1.66672m;

			var actual = BigDecimal.Mod(dividend, divisor);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestMod5()
		{
			// 240 % 2 = 0
			BigDecimal dividend = 240;
			BigDecimal divisor = 2;
			string expectedString = "0";
			BigDecimal expected = 0;

			var actual = BigDecimal.Mod(dividend, divisor);
			string actualString = actual.ToString();

			Assert.AreEqual(expected, actual, $"{dividend} % {divisor} = {actual}");
			Assert.AreEqual(expectedString, actualString, $"{dividend} % {divisor} = {actual}");

			TestContext.WriteLine($"{dividend} % {divisor} = {actual}");
		}

		[Test]
		public void TestMod6()
		{
			BigDecimal NumberBDParse = BigDecimal.Parse("-157766400");
			TestContext.WriteLine($"{NumberBDParse}");
			BigDecimal actual = NumberBDParse % 60;
			TestContext.WriteLine(actual);
			Assert.AreEqual("0", actual.ToString(), "BigDecimal.Parse"); // ???

			TestContext.WriteLine("---");

			BigDecimal NumberBD = -157766400;
			TestContext.WriteLine($"{NumberBD}");
			actual = NumberBD % 60;
			TestContext.WriteLine(actual);
			Assert.AreEqual("0", actual.ToString(), "BigDecimal implicit cast to decimal or double"); // 60 (incorrect)

			TestContext.WriteLine("---");

			BigInteger NumberBI = -157766400;
			TestContext.WriteLine($"{NumberBI}");
			BigInteger actual2 = NumberBI % 60;
			TestContext.WriteLine(actual2);
			Assert.AreEqual("0", actual2.ToString(), "BigInteger"); // 0 (correct)

			TestContext.WriteLine("---");

			float NumberFL = -157766400;
			TestContext.WriteLine($"{NumberFL}");
			float actual3 = NumberFL % 60;
			TestContext.WriteLine(actual3);
			Assert.AreEqual("0", Math.Abs(actual3).ToString(), "float"); // 0 (correct)
		}

		[Test]
		public void TestMultiply()
		{
			var p = BigDecimal.Parse("-6122421090493547576937037317561418841225758554253106999");
			var actual = p * new BigDecimal(BigInteger.Parse("9996013524558575221488141657137307396757453940901242216"), -34);
			var expected = new BigDecimal(BigInteger.Parse("-61199804023616162130466158636504166524066189692091806226423722790866248079929810268920239053350152436663869784"));

			var matches = expected.ToString().Equals(actual.ToString().Replace(this.Format.NumberDecimalSeparator, ""), StringComparison.Ordinal);

			Assert.IsTrue(matches);
		}

		[Test]
		public void TestMultiply1()
		{
			var p = BigDecimal.Parse("6122421090493547576937037317561418841225758554253106999");
			var q = BigDecimal.Parse("5846418214406154678836553182979162384198610505601062333");
			var expected = "35794234179725868774991807832568455403003778024228226193532908190484670252364677411513516111204504060317568667";

			var actual = BigDecimal.Multiply(p, q);

			Assert.AreEqual(expected, actual.ToString());
		}

		[Test]
		public void TestMultiply2()
		{
			var p = BigDecimal.Parse("6122421090493547576937037317561418841225758554253106999");
			var actual = p * p;
			var expected = "37484040009320200288159018961010536937973891182532366282540247408867702983313960194873589374267102044942786001";

			Assert.AreEqual(expected, actual.ToString());
		}


		internal class ResultObject
		{
			public int Precision { get; set; }
			public int N_of_ExpN { get; set; }
			public override string ToString() => $"{Precision}, {N_of_ExpN}";
		}


		[Test]
		public static void Test_Exp_EdgeCases()
		{
			Stopwatch timer = Stopwatch.StartNew();

			BigDecimal exp10 = BigDecimal.Exp(10);

			TimeSpan timeElapsed_10 = timer.Elapsed;
			TestContext.WriteLine($"Exp(10) - Time Elapsed (ms): {timeElapsed_10.TotalMilliseconds}");

			timer.Reset();
			timer.Start();

			BigDecimal exp11 = BigDecimal.Exp(11);

			TimeSpan timeElapsed_11 = timer.Elapsed;
			TestContext.WriteLine($"Exp(11) - Time Elapsed (ms): {timeElapsed_11.TotalMilliseconds}");

			timer.Reset();
			timer.Start();

			BigDecimal exp12 = BigDecimal.Exp(12);

			TimeSpan timeElapsed_12 = timer.Elapsed;
			TestContext.WriteLine($"Exp(12) - Time Elapsed (ms): {timeElapsed_12.TotalMilliseconds}");

			TestContext.WriteLine();
			TestContext.WriteLine($"Expected Exp({10}): {Expected.ValuesOfExp[10]}");
			TestContext.WriteLine($"  Actual Exp({10}): {exp10}");
			Common.HightlightDiffControl(Expected.ValuesOfExp[10], exp10.ToString(), 20);

			TestContext.WriteLine();
			TestContext.WriteLine($"Expected Exp({11}): {Expected.ValuesOfExp[11]}");
			TestContext.WriteLine($"  Actual Exp({11}): {exp11}");
			Common.HightlightDiffControl(Expected.ValuesOfExp[11], exp11.ToString(), 20);

			TestContext.WriteLine();
			TestContext.WriteLine($"Expected Exp({12}): {Expected.ValuesOfExp[12]}");
			TestContext.WriteLine($"  Actual Exp({12}): {exp12}");
			Common.HightlightDiffControl(Expected.ValuesOfExp[12], exp12.ToString(), 20);
		}

		[Test]
		public static void Test_Exp_Large()
		{
			int precision = 355;

			BigDecimal valueToTest = 813; // 1973

			Stopwatch timer = Stopwatch.StartNew();

			BigDecimal actual = BigDecimal.Exp(valueToTest, precision);

			TimeSpan timeElapsed_exp = timer.Elapsed;
			TestContext.WriteLine($"Exp({valueToTest}) - Time Elapsed (ms): {timeElapsed_exp.TotalMilliseconds}");

			string expected = "120618462233518981433238080308535803757557687664846828848443535448779741815217057681027697506649569601439291868793154656577261476254713167816544696153161438135707491747718691667694545847458665277046673289298561918077097160570572162793147114386756467587949281832249428245023200573458995225450885521592507359241589853364093350639258320452351869351505285572.228772209";

			TestContext.WriteLine();
			TestContext.WriteLine($"Expected Exp({valueToTest}): {expected}");
			TestContext.WriteLine($"  Actual Exp({valueToTest}): {actual}");
			Common.HightlightDiffControl(expected, actual.ToString(), 20);

			Debug.Assert.AreEqual(expected, actual.ToString(), 352);
		}

		[Test]
		public static void Test_Exp_Timing()
		{
			int precision = 200;

			BigDecimal OneTenth = new BigDecimal(11, -2);
			BigDecimal One = new BigDecimal(113, -2);
			BigDecimal Ten = new BigDecimal(111, -1);
			BigDecimal Twenty = new BigDecimal(214, -1);
			BigDecimal TwoHundred = new BigDecimal(2011, -1);

			string strOneTenth = $"{OneTenth}";
			string strOne = $"{One}";
			string strTen = $"{Ten}";
			string strTwenty = $"{Twenty}";
			string strTwoHundred = $"{TwoHundred}";

			Stopwatch timer = Stopwatch.StartNew();

			timer.Reset();
			timer.Start();

			BigDecimal.Exp(OneTenth, precision);

			TimeSpan timeElapsed_1_10 = timer.Elapsed;
			TestContext.WriteLine($"Exp({strOneTenth}) - Time Elapsed (ms): {timeElapsed_1_10.TotalMilliseconds}");
			TestContext.WriteLine();


			timer.Reset();
			timer.Start();

			BigDecimal.Exp(One, precision);

			TimeSpan timeElapsed_1 = timer.Elapsed;
			TestContext.WriteLine($"Exp({strOne}) - Time Elapsed (ms): {timeElapsed_1.TotalMilliseconds}");
			TestContext.WriteLine();

			timer.Reset();
			timer.Start();

			BigDecimal.Exp(Ten, precision);

			TimeSpan timeElapsed_10 = timer.Elapsed;
			TestContext.WriteLine($"Exp({strTen}) - Time Elapsed (ms): {timeElapsed_10.TotalMilliseconds}");
			TestContext.WriteLine();

			timer.Reset();
			timer.Start();

			BigDecimal.Exp(Twenty, precision);

			TimeSpan timeElapsed_20 = timer.Elapsed;
			TestContext.WriteLine($"Exp({strTwenty}) - Time Elapsed (ms): {timeElapsed_20.TotalMilliseconds}");
			TestContext.WriteLine();

			timer.Reset();
			timer.Start();

			BigDecimal.Exp(TwoHundred, precision);

			TimeSpan timeElapsed_200 = timer.Elapsed;
			TestContext.WriteLine($"Exp({strTwoHundred}) - Time Elapsed (ms): {timeElapsed_200.TotalMilliseconds}");
			TestContext.WriteLine();
		}

		public static void Test_Exp_1011(int precision, int minimumCorrectDigits)
		{
			BigDecimal valueToTest = 1011;
			string expected = "11795631706967431106257568260721165052418669208780627463056386271682463550358281615243401604609427915490736977871001536553653167580626736331516842400752123331182209012286512505603075898850283249210504933496598653773386608180010349926378964239687044699518479654589142697769694489316266803535363535669997608301616494716924943498510232177924309996046591344152647854591693443689927304679068837084128565236405425159892099980215918759545864400126.589108911252832854643467068425822441076055894309627977798308921397204006850026005868932906663258362001654798686230087439212347234353932817803561857750948087363466040372013875880017089820431467652456239183514905552905216567577472414761661275479518819815574615064577702742876563261954130633564844288154413735772184900268161683927069887941863946739210263149971011543245322251049639950750819714535497409650895327643718951562796414333875859164935571124632733571526580436080298067974076793724713503046953482413423252055159193987878999180903172603138472277097851121";

			Test_Exp_Variable(precision, minimumCorrectDigits, valueToTest, expected);
		}

		public static void Test_Exp_2011(int precision, int minimumCorrectDigits)
		{
			BigDecimal valueToTest = 2011;
			string expected = "2323823329748012862284724519461635711939560569765110356699112127030180484033973095063071499919965288199448948756287133798394499682902037462246237766454879628391773259648467719149972443214873411985429519464819962531782997822916278506973646261971785480143712304203162494968930428639752003764803942015565512940652276121044306831501917578696364968824880352708813969372371518658461046325562298095423146083470918524272244805026769336959711817570542440957951676629381518687976452083498994073690257881699796035450140231255790118784990526505141719814623220214032890748733685240673070408163489016448785761642985195061416769378997675967190710707236340059633603820453410225070331540278035278661756344793194276131411085364783963887102226233316721198151568554382794577241196740812007792353017548086760471957441181119527833301970007176054171439972748742171476694637291770547688322854967781.199815973820403248083653985489213684726396130770592743925176192357540212994182656884498750131746203274680174451505495399603449";

			Test_Exp_Variable(precision, minimumCorrectDigits, valueToTest, expected);
		}

		public static void Test_Exp_Variable(int precision, int minimumCorrectDigits, BigDecimal valueToTest, string expected)
		{
			int savePrecision = BigDecimal.Precision;

			BigDecimal.Precision = precision;
			int truncateAt = precision + 50;

			Stopwatch timer = Stopwatch.StartNew();

			BigDecimal actual = BigDecimal.Exp(valueToTest, precision);

			TimeSpan timeElapsed_exp = timer.Elapsed;
			double elapsedDisplay = (timeElapsed_exp.TotalMilliseconds) / 1000d;
			TestContext.WriteLine($"Exp({valueToTest}): Time_Elapsed: {elapsedDisplay} sec.");

			//TestContext.WriteLine();
			//TestContext.WriteLine($"Expected Exp({valueToTest}): {expected}");
			//TestContext.WriteLine($"  Actual Exp({valueToTest}): {actual.TruncateAt(truncateAt)}");
			Common.HightlightDiffControl(expected, actual.ToString(), 20);

			Debug.Assert.AreEqual(expected, actual.ToString(), minimumCorrectDigits);

			BigDecimal.Precision = savePrecision;
		}

		public static void Test_ForRecursionThreshold(int max = 200)
		{
			int n = 11;

			while (n < max)
			{
				bool success = LoopInternalTest(n);

				if (!success)
				{
					break;
				}

				n++;
			}
		}

		public static bool LoopInternalTest(int n)
		{
			string expected = "";

			if (n > 1000)
			{
				expected = Expected.ValuesOfExp_Extended[n];
			}
			else
			{
				expected = Expected.ValuesOfExp[n];
			}

			int precision = expected.IndexOf('.') + 1;

			Stopwatch timer = Stopwatch.StartNew();
			BigDecimal actualValue = BigDecimal.Exp(n, precision);
			TimeSpan timeElapsed_exp = timer.Elapsed;
			double elapsedDisplay = (timeElapsed_exp.TotalMilliseconds) / 1000d;
			elapsedDisplay = Math.Round(elapsedDisplay, 2);

			string actual = actualValue.ToString();

			string _expected = expected.Substring(0, precision);
			string _actual = actual.Substring(0, precision);

			bool success = string.Equals(_expected, _actual);

			string message = "SUCCESS!";

			if (!success)
			{
				message = "fail";
			}

			TestContext.WriteLine();
			TestContext.WriteLine($"   {message}   ");
			TestContext.WriteLine($" Expected  Exp({n}): {_expected}");
			TestContext.WriteLine($"  Actual   Exp({n}): {_actual}");
			TestContext.WriteLine($" Precision         : {precision}");
			TestContext.WriteLine($"---  Elapsed: {elapsedDisplay} sec ---");

			return success;
		}

		private static int PrecisionTestRange_Start = 70;
		private static int PrecisionTestRange_End = 120;
		private static int PrecisionTestRange_StepSize = 10;

		private static int N_Start = 180;
		private static int N_Stop = 220;
		private static int N_StepSize = 5;

		private static int Tolerance = 3;
		private static int Strikes = 5;

		[Test]
		public static void Test_Exp_TaylorSeriesAccuracy()
		{
			List<ResultObject> results = new List<ResultObject>();

			int precision = PrecisionTestRange_Start;
			int maxN = -1;
			while (precision <= PrecisionTestRange_End)
			{
				maxN = Internal_Exp_TaylorSeriesAccuracy(precision);
				results.Add(new ResultObject() { Precision = precision, N_of_ExpN = maxN });
				precision += PrecisionTestRange_StepSize;
			}

			TestContext.WriteLine();
			TestContext.WriteLine($"<RESULTS>");
			TestContext.WriteLine();
			foreach (ResultObject result in results)
			{
				TestContext.WriteLine(result.ToString());
			}
			TestContext.WriteLine();
			TestContext.WriteLine($"</RESULTS>");
			TestContext.WriteLine();
		}

		internal static int Internal_Exp_TaylorSeriesAccuracy(int precision)
		{
			TestContext.WriteLine();
			TestContext.WriteLine($"<PRECISION  DIGITS=\"{precision}\" >");

			int outOfRangeCount = 0;
			int lastCorrectN = -1;

			int n = N_Start;
			while (n < N_Stop)
			{
				int correctDigits = Internal_Exp_N(precision, n);

				if (correctDigits < (precision - Tolerance))
				{
					outOfRangeCount++;
				}
				else
				{
					lastCorrectN = n;
				}

				if (outOfRangeCount >= Strikes)
				{
					break;
				}

				n += N_StepSize;
			}

			TestContext.WriteLine();
			TestContext.WriteLine($"</PRECISION> // {precision}");
			TestContext.WriteLine();
			TestContext.WriteLine();

			return lastCorrectN;
		}

		internal static int Internal_Exp_N(int precision, int N)
		{
			BigDecimal expN = BigDecimal.Exp(N, precision);

			string actualN = Expected.ValuesOfExp[N];

			TestContext.WriteLine();
			TestContext.WriteLine($"BigDecimal.Exp({N}): {expN}");
			TestContext.WriteLine($"  Actual   Exp({N}): {actualN}");
			int correctDigits = Common.HightlightDiffControl(actualN, expN.ToString(), 20);

			return correctDigits;
		}

		[Test]
		public static void Test_Exp_200()
		{
			int twoHundred = 200;
			double dExp = (double)BigDecimal.Exp(twoHundred);
			double dPowE = (double)BigDecimal.Pow(Math.E, twoHundred);
			double dPowEB = (double)BigDecimal.Pow(BigDecimal.E, twoHundred);
			double mathPow = Math.Pow(Math.E, twoHundred);
			double mathExp = Math.Exp(twoHundred);

			BigDecimal bd_200 = new BigDecimal(mantissa: 200, exponent: 0);

			BigDecimal bdExpd = BigDecimal.Exp(twoHundred);
			BigDecimal bdPowEd = BigDecimal.Pow(Math.E, twoHundred);
			BigDecimal bdPowEBd = BigDecimal.Pow(BigDecimal.E, twoHundred);


			Stopwatch timer = Stopwatch.StartNew();

			BigDecimal bdExpbd = BigDecimal.Exp(bd_200);

			TimeSpan timeElapsed_exp = timer.Elapsed;
			TestContext.WriteLine($"Exp(200) - Time Elapsed (ms): {timeElapsed_exp.TotalMilliseconds}");

			string expected = "722597376812574925817747704218930569735687442852731928403269789123221909361473891661561.9265890";

			TestContext.WriteLine($"(double) BigDecimal.Exp(200):                   {dExp}");
			TestContext.WriteLine($"(double) BigDecimal.Pow(Math.E, 200):           {dPowE}");
			TestContext.WriteLine($"(double) BigDecimal.Pow(BigDecimal.E, 200):     {dPowEB}");
			TestContext.WriteLine($"(double) Math.Pow(Math.E, 200):                 {mathPow}");
			TestContext.WriteLine($"(double) Math.Exp(200):                         {mathExp}");
			TestContext.WriteLine();
			TestContext.WriteLine($"(BigDecimal) BigDecimal.Exp((double)200):               {bdExpd}");
			TestContext.WriteLine($"(BigDecimal) BigDecimal.Pow(Math.E, (double)200):       {bdPowEd}");
			TestContext.WriteLine($"(BigDecimal) BigDecimal.Pow(BigDecimal.E, (double)200): {bdPowEBd}");
			TestContext.WriteLine();
			TestContext.WriteLine($"(BigDecimal) BigDecimal.Exp((BigDecimal)200):                   {bdExpbd}");
			TestContext.WriteLine();
			TestContext.WriteLine($"Actual:                                         722597376812574925817747704218930569735687442852731928403269789123221909361473891661561.926589062570557468402043101429418177110677119368226480983077273278800...");
			TestContext.WriteLine();
			TestContext.WriteLine($"BigDecimal(200):");
			TestContext.WriteLine($"ToString: {bd_200}");
			TestContext.WriteLine($"Mantissa: {bd_200.Mantissa}");
			TestContext.WriteLine($"Exponent: {bd_200.Exponent}");
			TestContext.WriteLine();

			Debug.Assert.AreEqual(expected, bdExpbd.ToString(), 94);
		}

		[Test]
		public void TestExponentiation1()
		{
			double exp = 0.052631578947368421d;
			double phi = (1.0d + Math.Sqrt(5)) / 2.0d;

			BigDecimal result1 = BigDecimal.Pow(9.0d, 0.5d);
			BigDecimal result2 = BigDecimal.Pow(16.0d, 0.25d);
			string expected1 = "3";
			string expected2 = "2";

			BigDecimal result3 = BigDecimal.Pow(phi, 13);
			string expected3 = "521.001919378725";

			BigDecimal result4 = BigDecimal.Pow(1162261467, exp);
			BigDecimal result5 = BigDecimal.Pow(9349, exp);
			string expected4 = "3";
			string expected5 = "1.61803398777557";

			BigDecimal result6 = BigDecimal.Pow(1.618034d, 16.000000256d);
			BigDecimal result7 = BigDecimal.Pow(phi, 20.0000000128d);
			string expected6 = "2207.00006429941";
			string expected7 = "15127.0000270679";

			BigDecimal result8 = BigDecimal.Pow(29192926025390625d, 0.07142857142857142d);
			string expected8 = "14.999999999999998";

			Assert.AreEqual(expected1, result1.ToString());
			Assert.AreEqual(expected2, result2.ToString());
			Assert.AreEqual(expected3, result3.ToString().Substring(0, 16));
			Assert.AreEqual(expected4, result4.ToString());
			Assert.AreEqual(expected5, result5.ToString().Substring(0, 16));
			Assert.AreEqual(expected6, result6.ToString().Substring(0, 16));
			Assert.AreEqual(expected7, result7.ToString().Substring(0, 16));
			Assert.AreEqual(expected8, result8.ToString());
		}

		[Test]
		public void TestExponentiation2()
		{
			BigDecimal result1 = BigDecimal.Pow(new BigDecimal(16), new BigInteger(16));
			BigDecimal result2 = BigDecimal.Pow(new BigDecimal(101), new BigInteger(13));
			string expected1 = "18446744073709551616";
			string expected2 = "113809328043328941786781301";

			BigDecimal result3 = BigDecimal.Pow(new BigDecimal(0.25), 2);
			string expected3 = "0.0625";

			Assert.AreEqual(expected1, result1.ToString());
			Assert.AreEqual(expected2, result2.ToString());
			Assert.AreEqual(expected3, result3.ToString());
		}

		[Test]
		public void TestNegate()
		{
			string expected = TestBigDecimalHelper.PrepareValue("-1.375", this.Format);
			var actual = BigDecimal.Negate(BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("1.375", this.Format)));

			Assert.AreEqual(expected, actual.ToString());
		}

		[Test]
		public void TestReciprocal001()
		{
			int savePrecision = BigDecimal.Precision;
			BigDecimal.Precision = 10;

			var dividend = new BigDecimal(1);
			var divisor = new BigDecimal(3);

			var actual = BigDecimal.Divide(dividend, divisor);

			//var expected = BigDecimal.Parse( "0.3333333333333333" );
			string expected = TestBigDecimalHelper.PrepareValue("0.3333333333", this.Format);

			Assert.AreEqual(expected, actual.ToString());

			BigDecimal.Precision = savePrecision;
		}

		[Test]
		public void TestReciprocal002()
		{

			// 1/2 = 0.5
			var expected = TestBigDecimalHelper.PrepareValue("0.5", this.Format);

			var dividend = new BigDecimal(1);
			var divisor = new BigDecimal(2);

			var actual = BigDecimal.Divide(dividend, divisor);

			Assert.AreEqual(expected, actual.ToString());
		}

		[Test]
		public void TestReciprocal003()
		{
			int savePrecision = BigDecimal.Precision;
			BigDecimal.Precision = 15;

			//var expected = BigDecimal.Parse( "12.000000000000005" );
			string expected = TestBigDecimalHelper.PrepareValue("12.000000000000004", this.Format);

			var dividend = new BigDecimal(1);
			var divisor = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.0833333333333333", this.Format));

			var actual = BigDecimal.Divide(dividend, divisor);

			Assert.AreEqual(expected, actual.ToString());

			BigDecimal.Precision = savePrecision;
		}

		[Test]
		public void TestReciprocal004()
		{
			int savePrecision = BigDecimal.Precision;
			BigDecimal.Precision = 14;

			// 2/0.63661977236758 == 3.1415926535898
			string expected = TestBigDecimalHelper.PrepareValue("3.1415926535897", this.Format);

			var dividend = new BigDecimal(2);
			var divisor = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.63661977236758", this.Format));

			var actual = BigDecimal.Divide(dividend, divisor);

			Assert.AreEqual(expected, actual.ToString());

			BigDecimal.Precision = savePrecision;
		}

		[Test]
		public void TestSqrt()
		{
			var expected = BigInteger.Parse("8145408529");
			var expectedSquared = BigInteger.Parse("66347680104305943841");

			var squared = expected * expected;
			TestContext.WriteLine($"{expected} squared is {squared}.");
			Assert.AreEqual(squared, expectedSquared);
			var actual = squared.NthRoot(2, out _);

			Assert.AreEqual(expected, actual, "sqrt(66347680104305943841) = 8145408529");
			TestContext.WriteLine($"And {squared} squared root is {actual}.");
		}

		[Test]
		public void TestSubtraction001()
		{
			var number = BigDecimal.Parse("4294967295");
			BigDecimal expected = BigDecimal.Parse("2147483648");

			var actual = number - 0x7FFFFFFF;

			Assert.AreEqual(expected, actual);
		}
		/*
		[Test]
		[NonParallelizable]
		public void TestSubtractionRandom(
		[Random(-8.98846567431158E+300D, 8.98846567431158E+300D, 3)] Double b,
		[Random(-8.98846567431158E+300D, 8.98846567431158E+300D, 3)] Double d)
		{
			var strB = $"{b:R}";
			var strD = $"{d:R}";

			TestContext.WriteLine($"{b:R} = {strB}");
			TestContext.WriteLine($"{d:R} = {strD}");

			var bigB = BigDecimal.Parse(strB);
			var bigD = BigDecimal.Parse(strD);

			TestContext.WriteLine(Environment.NewLine);
			TestContext.WriteLine($"bigB = {bigB}");
			TestContext.WriteLine($"bigD = {bigD}");

			var result1 = BigDecimal.Subtract(bigB, bigD);
			var result2 = bigB - bigD;

			Assert.AreEqual(result1, result2);
		}
		*/
		[Test]
		public void TestSubtraction002()
		{
			BigDecimal high = 100.1m;
			BigDecimal low = 25.1m;

			string expected = "75";
			BigDecimal actual = BigDecimal.Subtract(high, low);

			Assert.AreEqual(expected, actual.ToString(), $"100.1 - 25.1 should equal 75.\nHigh: {TestBigDecimalHelper.GetInternalValues(high)}\nLow: {TestBigDecimalHelper.GetInternalValues(low)}\nResult: {TestBigDecimalHelper.GetInternalValues(actual)}");
		}

		[Test]
		public void TestSubtraction003()
		{
			BigDecimal high = (Double)100.3;
			BigDecimal low = (Double)25.1;

			string expected = TestBigDecimalHelper.PrepareValue("75.2", this.Format);
			BigDecimal actual = BigDecimal.Subtract(high, low);

			Assert.AreEqual(expected, actual.ToString(), $"100.3 - 25.1 should equal 75.2\nHigh: {TestBigDecimalHelper.GetInternalValues(high)}\nLow: {TestBigDecimalHelper.GetInternalValues(low)}\nResult: {TestBigDecimalHelper.GetInternalValues(actual)}");
		}

		[Test]
		public void TestSubtraction004()
		{
			BigDecimal high = (Decimal)100.3;
			BigDecimal low = (Decimal)0.3;

			string expected = "100";
			BigDecimal actual = BigDecimal.Subtract(high, low);

			Assert.AreEqual(expected, actual.ToString(), $"100.3 - 0.3 should equal 100.\nHigh: {TestBigDecimalHelper.GetInternalValues(high)}\nLow: {TestBigDecimalHelper.GetInternalValues(low)}\nResult: {TestBigDecimalHelper.GetInternalValues(actual)}");
		}

		[Test]
		public void TestSubtraction005()
		{
			BigDecimal high = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("100.001", this.Format));
			BigDecimal low = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("25.1", this.Format));

			string expected = TestBigDecimalHelper.PrepareValue("74.901", this.Format);
			BigDecimal actual = BigDecimal.Subtract(high, low);

			Assert.AreEqual(expected, actual.ToString(), $"100.001 - 25.1 should equal 74.901.\nHigh: {TestBigDecimalHelper.GetInternalValues(high)}\nLow: {TestBigDecimalHelper.GetInternalValues(low)}\nResult: {TestBigDecimalHelper.GetInternalValues(actual)}");
		}

		[Test]
		public void TestSubtraction006()
		{
			BigDecimal high = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("100.1", this.Format));
			BigDecimal low = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("25.001", this.Format));

			string expected = TestBigDecimalHelper.PrepareValue("75.099", this.Format);
			BigDecimal actual = BigDecimal.Subtract(high, low);

			Assert.AreEqual(expected, actual.ToString(), $"100.1 - 25.001 should equal 75.099.\nHigh: {TestBigDecimalHelper.GetInternalValues(high)}\nLow: {TestBigDecimalHelper.GetInternalValues(low)}\nResult: {TestBigDecimalHelper.GetInternalValues(actual)}");
		}

		[Test]
		public void TestSubtraction007()
		{
			BigDecimal high = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("100.0648646786764", this.Format));
			BigDecimal low = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("25.156379516", this.Format));

			string expected = TestBigDecimalHelper.PrepareValue("74.9084851626764", this.Format);
			BigDecimal actual = BigDecimal.Subtract(high, low);

			Assert.AreEqual(expected, actual.ToString(), $"100.0648646786764 - 25.156379516 should equal 74.9084851626764.\nHigh: {TestBigDecimalHelper.GetInternalValues(high)}\nLow: {TestBigDecimalHelper.GetInternalValues(low)}\nResult: {TestBigDecimalHelper.GetInternalValues(actual)}");
		}

		[Test]
		public void TestSubtraction008()
		{
			BigDecimal high = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.100001", this.Format));
			BigDecimal low = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.101", this.Format));

			string expected = TestBigDecimalHelper.PrepareValue("-0.000999", this.Format);
			BigDecimal actual = BigDecimal.Subtract(high, low);

			Assert.AreEqual(expected, actual.ToString(), $"0.100001 - 0.101 should equal -0.000999.\nHigh: {TestBigDecimalHelper.GetInternalValues(high)}\nLow: {TestBigDecimalHelper.GetInternalValues(low)}\nResult: {TestBigDecimalHelper.GetInternalValues(actual)}");
		}

		[Test]
		public void TestSubtraction009()
		{
			BigDecimal high = BigDecimal.Parse("240");
			BigDecimal low = BigDecimal.Parse("240");
			string expected = "0";

			BigDecimal result = BigDecimal.Subtract(high, low);
			string actual = result.ToString();

			Assert.AreEqual(expected, actual.ToString(), $"240 - 240 should equal 0. Instead got: {TestBigDecimalHelper.GetInternalValues(result)}");
		}

		[Test]
		public void TestSquareRoot001()
		{
			BigDecimal value = BigDecimal.Parse("5");
			Int32 root = 2;
			Int32 precision = 30;

			string expected = TestBigDecimalHelper.PrepareValue("2.236067977499789696409173668731", this.Format);
			BigDecimal actual = BigDecimal.NthRoot(value, root, precision);

			Assert.AreEqual(expected, actual.ToString(), $"{root}th root of {value} did not return {expected}.");
		}

		[Test]
		public void TestSquareRoot002()
		{
			BigDecimal value = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.0981898234602005160423505923443092051160170637298815793320356006279679013343110872318753144061611219225635804218963505102948529140625", this.Format));
			//                                        "0.0981898234602005160423505923443092051160170637298815793320356006279679013343110872318753144061611219225635804218963505102948529140625");
			Int32 root = 2;
			Int32 precision = 50;

			string expected = TestBigDecimalHelper.PrepareValue("0.31335255457742883389571245385500659019295986107402", this.Format);
			BigDecimal result = BigDecimal.NthRoot(value, root, precision);

			BigDecimal actual = BigDecimal.Round(result, precision);

			Assert.AreEqual(expected, actual.ToString(), $"{root}th root of {value} did not return {expected}.");
		}

		[Test]
		public void TestSquareRoot003()
		{
			BigDecimal value = BigDecimal.Parse("9818982346020051604235059234430920511601706372988157933203560062796790133431108723187531440616112192256358042189635051029485291406258555");
			Int32 root = 2;
			Int32 precision = 135;

			string expected = TestBigDecimalHelper.PrepareValue("99090778309689603548815656125983317432034385902667809355596183348807.410596077216611169596571667988328091906450145578959539307248420211367976153463820323404307029425296409616398791728069401888988546189821", this.Format);
			BigDecimal result = BigDecimal.NthRoot(value, root, precision);

			BigDecimal actual = BigDecimal.Round(result, precision);

			Assert.AreEqual(expected, actual.ToString(), $"{root}th root of {value} did not return {expected}.");
		}

		[Test]
		public void TestSquareRoot004()
		{
			BigDecimal value = BigDecimal.Parse("9818982346020051604235059234430920511601706372988157933203560062796790133431108723187531440616112192");
			Int32 root = 2;
			Int32 precision = 135;

			string expected = TestBigDecimalHelper.PrepareValue("99090778309689603548815656125983317432034385902667.809355596183348807410596077216611169596571667988326798354988734930975117508103720966474578967977953788831616628961714711683020533839237", this.Format);
			BigDecimal result = BigDecimal.NthRoot(value, root, precision);

			BigDecimal actual = BigDecimal.Round(result, precision);

			Assert.AreEqual(expected, actual.ToString(), $"{root}th root of {value} did not return {expected}.");
		}

		[Test]
		public void TestSquareRoot_25_001()
		{
			BigDecimal value = BigDecimal.Parse("25");
			Int32 root = 2;
			Int32 precision = 18;

			string expected = "5";
			BigDecimal actual = BigDecimal.NthRoot(value, root, precision);

			Assert.AreEqual(expected, actual.ToString(), $"{root}th root of {value} did not return {expected}.");
		}

		[Test]
		public void TestSquareRoot_25_002()
		{

			BigDecimal value = new BigDecimal(-25);
			Int32 root = 2;
			Int32 precision = 18;

			BigDecimal actual;
			TestDelegate testDelegate = new TestDelegate(() => actual = BigDecimal.NthRoot(value, root, precision));

			Assert.Throws(typeof(ArgumentException), testDelegate);
		}

		[Test]
		public void TestSquareRootOfDecimal()
		{
			BigDecimal value = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.5", this.Format));
			Int32 root = 2;
			Int32 precision = 30;

			string expected = TestBigDecimalHelper.PrepareValue("0.707106781186547524400844362104", this.Format);
			BigDecimal actual = BigDecimal.NthRoot(value, root, precision);

			Assert.AreEqual(expected, actual.ToString(), $"{root}th root of {value} did not return {expected}.");
		}

		[Test]
		public void TestNthRoot()
		{
			BigDecimal value = BigDecimal.Parse("3");
			Int32 root = 3;
			Int32 precision = 50;

			string expected = TestBigDecimalHelper.PrepareValue("1.44224957030740838232163831078010958839186925349935", this.Format);
			BigDecimal actual = BigDecimal.NthRoot(value, root, precision);

			Assert.AreEqual(expected, actual.ToString(), $"{root}th root of {value} did not return {expected}.");
		}

		[Test]
		public void TestNthRootOfDecimal()
		{
			BigDecimal value = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.03", this.Format));
			Int32 root = 3;
			Int32 precision = 50;

			string expected = TestBigDecimalHelper.PrepareValue("0.31072325059538588668776624275223863628549068290674", this.Format);
			BigDecimal actual = BigDecimal.NthRoot(value, root, precision);

			Assert.AreEqual(expected, actual.ToString(), $"{root}th root of {value} did not return {expected}.");
		}

		[Test]
		public void TestGreaterThan001()
		{
			BigDecimal left = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-3.001", this.Format));
			BigDecimal right = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-3.002", this.Format));

			bool actual = left > right;
			bool expected = true;

			Assert.AreEqual(expected, actual, $"{left} > {right} == {expected}");
		}

		[Test]
		public void TestGreaterThan002()
		{
			BigDecimal left = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("3.000002", this.Format));
			BigDecimal right = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("3.000001", this.Format));

			bool actual = left > right;
			bool expected = true;

			Assert.AreEqual(expected, actual, $"{left} > {right} == {expected}");
		}

		[Test]
		public void TestGreaterThanOrEqualTo001()
		{
			BigDecimal left = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-0.001", this.Format));
			BigDecimal right = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-0.001", this.Format));

			bool actual = left >= right;
			bool expected = true;

			Assert.AreEqual(expected, actual, $"{left} > {right} == {expected}");
		}

		[Test]
		public void TestGreaterThanOrEqualTo002()
		{
			BigDecimal left = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-0.001", this.Format));
			BigDecimal right = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-0.002", this.Format));

			bool actual = left >= right;
			bool expected = true;

			Assert.AreEqual(expected, actual, $"{left} > {right} == {expected}");
		}

		[Test]
		public void TestGreaterThanOrEqualTo003()
		{
			BigDecimal left = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.001", this.Format));
			BigDecimal right = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("0.0001", this.Format));

			bool actual = left >= right;
			bool expected = true;

			Assert.AreEqual(expected, actual, $"{left} > {right} == {expected}");
		}

		[Test]
		public void TestLessThan001()
		{
			BigDecimal left = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-300000.02", this.Format));
			BigDecimal right = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-300000.01", this.Format));

			bool actual = left < right;
			bool expected = true;

			Assert.AreEqual(expected, actual, $"{left} > {right} == {expected}");
		}

		[Test]
		public void TestLessThan002()
		{
			BigDecimal left = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("3000000.00000001", this.Format));
			BigDecimal right = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("3000000.0000001", this.Format));

			bool actual = left < right;
			bool expected = true;

			Assert.AreEqual(expected, actual, $"{left} > {right} == {expected}");
		}

		[Test]
		public void TestLessThanOrEqualTo001()
		{
			BigDecimal left = BigDecimal.Parse("3");
			BigDecimal right = BigDecimal.Parse("3");

			bool actual = left <= right;
			bool expected = true;

			Assert.AreEqual(expected, actual, $"{left} > {right} == {expected}");
		}

		[Test]
		public void TestLessThanOrEqualTo002()
		{
			BigDecimal left = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-3.0000002", this.Format));
			BigDecimal right = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-3.0000001", this.Format));

			bool actual = left <= right;
			bool expected = true;

			Assert.AreEqual(expected, actual, $"{left} > {right} == {expected}");
		}

		[Test]
		public void TestLessThanOrEqualTo003()
		{
			BigDecimal left = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-3.0000001", this.Format));
			BigDecimal right = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("-3.00000001", this.Format));

			bool actual = left <= right;
			bool expected = true;

			Assert.AreEqual(expected, actual, $"{left} > {right} == {expected}");
		}

		[Test]
		public void TestLessThanOrEqualTo004()
		{
			BigDecimal left = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("3.000000201", this.Format));
			BigDecimal right = BigDecimal.Parse(TestBigDecimalHelper.PrepareValue("30.00000201", this.Format));

			bool actual = left <= right;
			bool expected = true;

			Assert.AreEqual(expected, actual, $"{left} > {right} == {expected}");
		}

		[Test]
		public void TestDoubleCasting()
		{
			BigDecimal value1 = BigDecimal.Pow(10, -1);
			BigDecimal value2 = new BigDecimal(99.00000001);
			BigDecimal res = BigDecimal.Divide(value2, value1);

			double delta = 0.0000001;
			double expected = 990.0000001;
			double actual = (double)res;

			Assert.AreEqual(expected, actual, delta, $"{expected} != {actual}");
		}

		[Test]
		public void TestSingleCasting()
		{
			BigDecimal value1 = BigDecimal.Pow(10, -1);
			BigDecimal value2 = new BigDecimal(99.00000001);
			BigDecimal res = BigDecimal.Divide(value2, value1);

			double delta = 0.0000001;
			Single expected = 990.0000001f;
			Single actual = (Single)res;

			Assert.AreEqual(expected, actual, delta, $"{expected} != {actual}");
		}

		[Test]
		public void TestDecimalCasting()
		{
			BigDecimal value1 = BigDecimal.Pow(10, -1);
			BigDecimal value2 = new BigDecimal(99.00000001);
			BigDecimal res = BigDecimal.Divide(value2, value1);

			double delta = 0.0000001;
			double expected = 990.0000001;
			decimal actual = (decimal)res;

			Assert.AreEqual(expected, Convert.ToDouble(actual), delta, $"{expected} != {actual}");
		}
	}
}