using Backend.Infrastructure.Common;

namespace Backend.IntegrationTests.Common;

[TestFixture]
public class JSonFileReaderTests
{
    private JsonFileReader _jsonFileReader;

    [SetUp]
    public void Setup()
    {
        _jsonFileReader = new JsonFileReader();
    }

    [Test]
    public void ReadMilleniumFalconData_Should_Throw_If_FilePath_IsNullOrEmpty()
    {
        Assert.IsNull(_jsonFileReader.ReadMilleniumFalconData(string.Empty));
        Assert.IsNull(_jsonFileReader.ReadMilleniumFalconData(null));
    }

    [Test]
    public void ReadEmpireData_Should_Throw_If_FilePath_IsNullOrEmpty()
    {
        Assert.IsNull(_jsonFileReader.ReadEmpireData(string.Empty));
        Assert.IsNull(_jsonFileReader.ReadEmpireData(null));
    }

    [Test]
    public void ReadEmpireData_Should_Return_EmpireConfig()
    {
        var config = _jsonFileReader.ReadEmpireData($"{AppContext.BaseDirectory}empire1.json");
        Assert.IsNotNull(config);

        Assert.AreEqual(7, config.Countdown);
        Assert.AreEqual(3, config.Bounty_Hunters.Count);
        Assert.AreEqual(3, config.Bounty_Hunters.Where(b => b.Planet == "Hoth").Count());
    }

    [Test]
    public void ReadMilleniumFalconData_Should_Return_MilleniumFalconConfig()
    {
        var config = _jsonFileReader.ReadMilleniumFalconData($"{AppContext.BaseDirectory}millennium-falcon1.json");
        Assert.IsNotNull(config);
        Assert.AreEqual(6, config.Autonomy);
        Assert.AreEqual("Tatooine", config.Departure);
        Assert.AreEqual("Endor", config.Arrival);
        Assert.AreEqual("universe.db", config.Routes_Db);
    }

}
