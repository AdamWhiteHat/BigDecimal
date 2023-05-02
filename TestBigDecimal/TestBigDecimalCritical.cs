using System;
using System.Numerics;
using ExtendedNumerics;
using NUnit.Framework;
using NUnit.Framework.Api;
using NUnit.Framework.Constraints;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Execution;

namespace TestBigDecimal;
[NonParallelizable]
[TestFixture]
public class TestBigDecimalCritical
{

	[Test]
	public void Test47()
	{
		var π1 = 1 * BigDecimal.π;
		var π2 = 2 * BigDecimal.π;
		var π4 = 4 * BigDecimal.π;
		var π8 = 8 * BigDecimal.π;
		var sum = π1 + π2 + π4 + π8;
		var actual = sum.WholeValue;
		var expected = (BigInteger)47;

		Assert.AreEqual(expected, actual);
	}

	[Test]
	public void TestConstructor0()
	{
		BigDecimal expected = 0;
		var actual = new BigDecimal(0);

		Assert.AreEqual(expected, actual);
	}

	[Test]
	public void TestConstructor00()
	{
		BigDecimal expected = 0;
		var actual = new BigDecimal(0, 0);

		Assert.AreEqual(expected, actual);
	}

	[Test]
	public void TestConstructor001D()
	{
		var i = BigDecimal.Parse("0.5");
		var j = BigDecimal.Parse("0.01");

		Assert.AreEqual("0.5", i.ToString());
		Assert.AreEqual("0.01", j.ToString());
	}

	[Test]
	public void TestConstructor001WriteLineA()
	{
		var expected1 = "3.141592793238462";
		var expected2 = "3.141592793238462";
		var π = (BigDecimal)3.141592793238462m;
		var d = new BigDecimal(BigInteger.Parse("3141592793238462"), -15);
		var actual1 = π.ToString();
		var actual2 = d.ToString();

		TestContext.WriteLine("π = " + actual1);
		TestContext.WriteLine("d = " + actual2);
		Assert.AreEqual(expected1, actual1);
		Assert.AreEqual(expected2, actual2);
	}

	[Test]
	public void TestConstructor001WriteLineB()
	{
		const Decimal m = 0.0000000000000001m;

		var e = new BigDecimal(1000000000, -25);
		var h = (BigDecimal)m;

		TestContext.WriteLine("m = " + m);
		TestContext.WriteLine("e = " + e);
		TestContext.WriteLine("h = " + h);

		Assert.AreEqual(h.ToString(), e.ToString());
	}

	[Test]
	public void TestConstructor002()
	{
		var f = new BigDecimal(-3, -2);
		Assert.AreEqual("-0.03", f.ToString());
	}

	[Test]
	public void TestConstructor003()
	{
		var g = new BigDecimal(0, -1);
		Assert.AreEqual("0", g.ToString());
	}

	[Test]
	public void TestConstructor004()
	{
		var h = BigDecimal.Parse("-0.0012345");
		Assert.AreEqual("-0.0012345", h.ToString());
	}

	[Test]
	public void TestConstructorToString123456789()
	{
		const Int32 numbers = 123456789;
		var expected = numbers.ToString();
		var actual = new BigDecimal(numbers).ToString();

		Assert.AreEqual(expected, actual);
	}

	[Test]
	public void TestNormalizeB()
	{
		var expected = "1000000";
		BigDecimal bigDecimal = new BigDecimal(1000000000, -3);

		var actual = bigDecimal.ToString();
		Assert.AreEqual(expected, actual);
	}

	[Test]
	public void TestParse001()
	{
		const String expected = "0.00501";
		var result = BigDecimal.Parse(expected);
		var actual = result.ToString();

		Assert.AreEqual(expected, actual);
	}

	[Test]
	public void TestParse002()
	{
		var result1 = BigDecimal.Parse("");
		Assert.AreEqual(result1, BigDecimal.Zero);

		var result2 = BigDecimal.Parse("0");
		Assert.AreEqual(result2, BigDecimal.Zero);

		var result3 = BigDecimal.Parse("-0");
		Assert.AreEqual(result3, BigDecimal.Zero);
	}

	[Test]
	public void TestParse0031()
	{
		const String expected = "-123456789";
		var actual = BigDecimal.Parse(expected).ToString();

		Assert.AreEqual(expected, actual);
	}

	[Test]
	public void TestParse0032()
	{
		const String expected = "123456789";
		var bigDecimal = BigDecimal.Parse(expected);
		var actual = bigDecimal.ToString();
		Assert.AreEqual(expected, actual);
	}

	[Test]
	public void TestParse0033()
	{
		const String expected = "1234.56789";
		var actual = BigDecimal.Parse(expected).ToString();
		Assert.AreEqual(expected, actual);
	}

	[Test]
	public void TestParseEpsilon()
	{
		var actual = BigDecimal.Parse("4.9406564584124654E-324");
		var expected = (BigDecimal)Double.Epsilon;
		Assert.AreEqual(expected, actual);
	}

	[Test]
	[NonParallelizable]
	public void TestTruncate()
	{
		const String inputTruncated =
			"0.38776413731534507341472294220970933835515664718260518542692164892369393388454765429965711304132864249950074173248631118139885711281403156400182208418498132380665348582256048635378814909035638369142648772177618951899185003568005598389270883746269077440991532220847404333505059368816860680653357748237545067181074698997348124273540082967040205337039719556204791774654186626822192852578297197622945023468587167940717672154626982847038945027431144177383552390076867181200131087852865589018597759807623800948540502708501473286123912110702619773244550322465985979980114779581215743799790210865866959716136152785422203785552850816565888483726280027736811701443283167094373785256268739306209472514414456698923382789454032363968616464876677737866600848986505927023714735496267888826964325695603484817243244809072199216323431074501911199589021095576091452848741385260278621154863084476153732935785975553768625991893151359011912157996422994871709298494233782632174324940552077998861058801035481527689611495569489001108047129667715138204276438217877458404549511313153150137527893798615996618488836664617057038126333313180040094667868763537391421370724952266262848120654117339947389714860375532634890853303061644123428823851175161042458377024247370039795707768407904842511280809838660155394320788005292339449327116941969519022963362256288633034183673183754543670394109495242494711094883817203892173617367313695468521390931623680173196403022932833410798171066411176995128779282872081932608267999112302172207922731784899893348412676098162467010668924433588685153611407005617772276741793479524433622378470125354022566116327819435029065557564148488746638319658731270457607183892425161850287824787546065413294231650473976473355046501500793677901782339691542233183741598103696583351788651177203938936918102755367072014549821942367309956671236707350861545099496206538228683951185018840006137763162355709495445928668480960580978979870379511703883251713690511544429859593313279574155504139941107166963497890833932811052504269372145803660626639428643564562691059910703703938694915154537936003382455188656514686359660013747580119285264755448830584594983111162605867224680013454700621697086948523549156403848856543212816956769085216390639154261614649538130954560421673680672884105498050605587531872704107707071402689983600332112655608194612408217782173036018661692139351433658340756975168361107372727516912020823362368253159826937134217107045868191298957690827125630453728790792408734840661702578638598543186544910552465999106381802375938701350575940262569041045146526024334627822715612658351899764042223444201035443823410277761971257862200600465373558428055133799307959576455801692979753194304758921759067399106319456847661528054181651013888120488047974670158855437555210689546049958555745098303660202886404313365902203237775035723926097742965028613593632230336269392684340085274710999024668887638930755250701806345477524832568256645103704878731032912768646402146422301881142289323523789305126831904241622042944333916620344863470012778933196413192781253025453531244850133026071231714118351386262249150472643870800725983523611903791303553632632769972142502483519860983067322477753824959399886980031912842700140970151007657989042261109130704991895244868527969247414974047405237324669264878742391500642753525622057641241164177505839173992651361990366480244195157062835803031557544691492841007028723179639729081951702197292799161437892952439082270465575308762112590993865133052593362045638622447863872110087219994330766670422412140283392118259566085972052360790645394540700438378734059789109046910356858343004387656915432928337709841252916626015752013241699464443045041876948902728601721842214670716585909801092203893128618468720651888522728597430373030188565238122801065278124235661294292641028550276301054915567825793810248724267437857461336921376742513529432313053995421425528496990787018582251366776291943999044323970133345610820834058391982655766601126736285624213085882525085728598384700565577250732861707158419417137322187913601105221450993534840307771350787020353312910993312574109077271828643659506792514058623896881407687463008239648545371730757776422468606770212802349464720937428992320859723193781752582479518699133569129895070994026410312951649922900688489934852155962598169718199217867461287501481443450777777718726084902136902441480397119384141792970385831367927360530354214563521320900336914169681532166791668676942898880184098720787172114194029069821244937592713815214434788393095503048740886117426353441330676447598548976011441527165748380891340472246800001389307364429687469295246232117792720007673578989468325170179570094184486525355114468774857635615955720968054081874458733938769018227387365842825259166694681823057556598910704367318366050815517174086712448729791746859581490981539042615521145996146036468018904747552880641671500884541141688250485043379587617571474356672696577799617797264760021142950373293666619603196041523466051054935554966409263947312788960948665394281916627352113060683470291466105925";
		const String inputOverflow = "919919200639429489197056";

		var expected = inputTruncated;

		var longValue = BigDecimal.Parse(String.Concat(inputTruncated, inputOverflow));
		var result = BigDecimal.Round(longValue, 5000);

		var actual = result.ToString();

		Assert.AreEqual(expected, actual);
	}

	[Test]
	[NonParallelizable]
	public void TestAlwaysTruncate()
	{
		var savePrecision = BigDecimal.Precision;
		var expected1 = "3.1415926535";
		var expected2 = "-3.1415926535";
		var expected3 = "-0.0000031415";
		var expected4 = "-3";

		var actual1 = "";
		var actual2 = "";
		var actual3 = "";
		var actual4 = "";

		try
		{
			BigDecimal.Precision = 10;
			BigDecimal.AlwaysTruncate = true;
			BigDecimal parsed1 = BigDecimal.Parse("3.14159265358979323846264338327950288419716939937510582097494459230781640628620899862803482534211706798214808651328230664709384460955058223172535");
			BigDecimal parsed2 = BigDecimal.Parse("-3.14159265358979323846264338327950288419716939937510582097494459230781640628620899862803482534211706798214808651328230664709384460955058223172535");
			BigDecimal parsed3 = BigDecimal.Parse("-0.00000314159265358979323846264338327950288419716939937510582097494459230781640628620899862803482534211706798214808651328230664709384460955058223172535");
			BigDecimal parsed4 = BigDecimal.Parse("-3");

			actual1 = parsed1.ToString();
			actual2 = parsed2.ToString();
			actual3 = parsed3.ToString();
			actual4 = parsed4.ToString();
		}
		finally
		{
			BigDecimal.Precision = savePrecision;
			BigDecimal.AlwaysTruncate = false;
		}

		Assert.AreEqual(expected1, actual1, "#: 1");
		Assert.AreEqual(expected2, actual2, "#: 2");
		Assert.AreEqual(expected3, actual3, "#: 3");
		Assert.AreEqual(expected4, actual4, "#: 4");
		Assert.AreEqual(5000, BigDecimal.Precision, "Restore Precision to 5000");
	}

	[Test]
	[NonParallelizable]
	public void TestTruncateOnAllArithmeticOperations()
	{
		var savePrecision = BigDecimal.Precision;

		BigDecimal mod1 = BigDecimal.Parse("3141592653589793238462643383279502");
		BigDecimal mod2 = BigDecimal.Parse("27182818284590452");
		BigDecimal neg1 = BigDecimal.Parse("-3.141592653589793238462643383279502884197169399375105820974944592307816406286208998628034825342117067982148086513282306647");
		BigDecimal lrg1 = BigDecimal.Parse("3.141592653589793238462643383279502884197169399375105820974944592307816406286208998628034825342117067982148086513282306647");
		BigDecimal lrg2 = BigDecimal.Parse("2.718281828459045235360287471352662497757247093699959574966967");

		var expected1 = "5.859874482";
		var expected2 = "0.4233108251";
		var expected3 = "8.5397342226";
		var expected4 = "0.8652559794";
		var expected5 = "9.869604401";
		var expected6 = "148.4131591";
		var expected7 = "8003077319547306";
		var expected8 = "-3.1415926535";
		var expected9 = "3";
		var expected10 = "4";
		var expected11 = "3.1415926535";

		var actual1 = "";
		var actual2 = "";
		var actual3 = "";
		var actual4 = "";
		var actual5 = "";
		var actual6 = "";
		var actual7 = "";
		var actual8 = "";
		var actual9 = "";
		var actual10 = "";
		var actual11 = "";

		try
		{
			BigDecimal.Precision = 10;
			BigDecimal.AlwaysTruncate = true;

			TestContext.WriteLine($"E = {BigDecimal.E}");
			TestContext.WriteLine($"{new BigDecimal(lrg1.Mantissa, lrg1.Exponent)}");
			TestContext.WriteLine($"{new BigDecimal(lrg2.Mantissa, lrg2.Exponent)}");

			BigDecimal result1 = BigDecimal.Add(lrg1, lrg2);
			BigDecimal result2 = BigDecimal.Subtract(lrg1, lrg2);
			BigDecimal result3 = BigDecimal.Multiply(lrg1, lrg2);
			BigDecimal result4 = BigDecimal.Divide(lrg2, lrg1);
			BigDecimal result5 = BigDecimal.Pow(lrg1, 2);
			BigDecimal result6 = BigDecimal.Exp(new BigInteger(5));
			BigDecimal result7 = BigDecimal.Mod(mod1, mod2);
			BigDecimal result8 = BigDecimal.Negate(lrg1);
			BigDecimal result9 = BigDecimal.Floor(lrg1);
			BigDecimal result10 = BigDecimal.Ceiling(lrg1);
			BigDecimal result11 = BigDecimal.Abs(lrg1);

			actual1 = result1.ToString();
			actual2 = result2.ToString();
			actual3 = result3.ToString();
			actual4 = result4.ToString();
			actual5 = result5.ToString();
			actual6 = result6.ToString();
			actual7 = result7.ToString();
			actual8 = result8.ToString();
			actual9 = result9.ToString();
			actual10 = result10.ToString();
			actual11 = result11.ToString();
		}
		finally
		{
			BigDecimal.Precision = savePrecision;
			BigDecimal.AlwaysTruncate = false;
		}

		Assert.AreEqual(expected1, actual1, $"Test Truncate On All Arithmetic Operations  - #1: ");
		Assert.AreEqual(expected2, actual2, $"Test Truncate On All Arithmetic Operations  - #2: ");
		Assert.AreEqual(expected3, actual3, $"Test Truncate On All Arithmetic Operations  - #3: ");
		Assert.AreEqual(expected4, actual4, $"Test Truncate On All Arithmetic Operations  - #4: ");
		Assert.AreEqual(expected5, actual5, $"Test Truncate On All Arithmetic Operations  - #5: ");
		StringAssert.StartsWith(expected6, actual6, $"Test Truncate On All Arithmetic Operations  - #6: ");
		Assert.AreEqual(expected7, actual7, $"Test Truncate On All Arithmetic Operations  - #7: ");
		Assert.AreEqual(expected8, actual8, $"Test Truncate On All Arithmetic Operations  - #8: ");
		Assert.AreEqual(expected9, actual9, $"Test Truncate On All Arithmetic Operations  - #9: ");
		Assert.AreEqual(expected10, actual10, $"Test Truncate On All Arithmetic Operations - #10: ");
		Assert.AreEqual(expected11, actual11, $"Test Truncate On All Arithmetic Operations - #11: ");

		Assert.AreEqual(5000, BigDecimal.Precision, "Restore Precision to 5000");
	}
}
