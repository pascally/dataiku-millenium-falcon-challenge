using Backend.Application.Services;
using Backend.Infrastructure.Common;
using Backend.Infrastructure.Repository;

namespace Backend.IntegrationTests.Services;

[TestFixture]
public class OnboardComputerServiceTests
{
    [Test]
    [TestCase("millennium-falcon1.json", "empire1.json", 0)]
    [TestCase("millennium-falcon1.json", "empire2.json", 0.81)]
    [TestCase("millennium-falcon1.json", "empire3.json", 0.9)]
    [TestCase("millennium-falcon1.json", "empire4.json", 1)]
    public void ComputeOddsToDestination(string milleniumFalconDataPath, string empireDataPath, double expectedOdds)
    {
        OnboardComputerService onboardComputerService = new(new RoutesRepository("universe.db"), new JsonFileReader());
        onboardComputerService.LoadMilleniumFalconDatas($"{AppContext.BaseDirectory}{milleniumFalconDataPath}");
        onboardComputerService.LoadEmpireDatas($"{AppContext.BaseDirectory}{empireDataPath}");

        double odds = onboardComputerService.ComputeOddsToDestination();

        Assert.AreEqual(expectedOdds, odds);
    }
}
