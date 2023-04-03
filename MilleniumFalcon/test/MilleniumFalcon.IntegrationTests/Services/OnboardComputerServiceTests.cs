using Backend.Application.Services;
using Backend.Infrastructure.Common;
using Backend.Infrastructure.Repository;

namespace Backend.IntegrationTests.Services;

[TestFixture]
public class OnboardComputerServiceTests
{
    [Test]
    [TestCase("millennium-falcon1.json", "empire1.json", 0)]
    [TestCase("millennium-falcon1.json", "empire2.json", 81.0)]
    [TestCase("millennium-falcon1.json", "empire3.json", 90.0)]
    [TestCase("millennium-falcon1.json", "empire4.json", 100.0)]
    public void ComputeOddsToDestination(string milleniumFalconDataPath, string empireDataPath, double expectedOdds)
    {
        OnboardComputerService onboardComputerService = new(new RoutesRepository(), new JsonFileReader());
        onboardComputerService.LoadMilleniumFalconDatas($"{AppContext.BaseDirectory}{milleniumFalconDataPath}");
        onboardComputerService.LoadEmpireDatas($"{AppContext.BaseDirectory}{empireDataPath}");

        double odds = onboardComputerService.ComputeOddsToDestination();

        Assert.AreEqual(expectedOdds, odds);
    }
}
