using Backend.Application.Config;
using Backend.Application.Interfaces.Common;
using Backend.Application.Interfaces.Repository;
using Backend.Application.Services;
using Backend.Domain.Models;
using Moq;

namespace Backend.UnitTests.Services;


[TestFixture]
public class OnboardComputerServiceTests
{
    [Test]
    public void Ctor_Should_Throw_If_Invalid_InputArgs()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            OnboardComputerService onboardComputerService = new(null, Mock.Of<IConfigFileReader>());
        });
        Assert.Throws<ArgumentNullException>(() =>
        {
            OnboardComputerService onboardComputerService = new(Mock.Of<IRoutesRepository>(), null);
        });

        Assert.DoesNotThrow(() =>
        {
            OnboardComputerService onboardComputerService = new(Mock.Of<IRoutesRepository>(), Mock.Of<IConfigFileReader>());
        });
    }

    [Test]
    [TestCase("")]
    [TestCase(null)]
    public void LoadEmpireDatas_Should_Return_False_If_InvalidInput(string pathToEmpireDatas)
    {
        OnboardComputerService onboardComputerService = new(Mock.Of<IRoutesRepository>(), Mock.Of<IConfigFileReader>());
        Assert.AreEqual(false, onboardComputerService.LoadEmpireDatas(pathToEmpireDatas));
    }

    [Test]
    public void LoadEmpireDatas_Should_Return_False_If_Config_Countdown_Is_Inferior_To_0()
    {
        //setups
        var mockConfigFileReader = new Mock<IConfigFileReader>();

        mockConfigFileReader.Setup(m => m.ReadEmpireData(It.IsAny<string>()))
            .Returns(new EmpireConfig()
            {
                Countdown = -1
            });

        //arrange
        OnboardComputerService onboardComputerService = new(Mock.Of<IRoutesRepository>(), mockConfigFileReader.Object);
        
        //assert
        Assert.AreEqual(false, onboardComputerService.LoadEmpireDatas("afile"));
    }

    [Test]
    [TestCase("")]
    [TestCase(null)]
    public void LoadMilleniumFalconDatas_Should_Throw_If_InvalidInput(string pathToMilleniumFalconDatas)
    {
        OnboardComputerService onboardComputerService = new(Mock.Of<IRoutesRepository>(), Mock.Of<IConfigFileReader>());
        Assert.AreEqual(false, onboardComputerService.LoadMilleniumFalconDatas(pathToMilleniumFalconDatas));
    }

    [Test]
    [TestCase(0, "departure", "arrival", "routeDb")]
    [TestCase(1, "", "arrival", "routeDb")]
    [TestCase(1, null, "arrival", "routeDb")]
    [TestCase(1, "departure", "", "routeDb")]
    [TestCase(1, "departure", null, "routeDb")]
    [TestCase(1, "departure", "arrival", "")]
    [TestCase(1, "departure", "arrival", null)]
    public void LoadMilleniumFalconDatas_Should_Throw_If_InvalidConfig(int autonomy, string departure, string arrival, string routeDb)
    {
        //setups
        var mockConfigFileReader = new Mock<IConfigFileReader>();

        mockConfigFileReader.Setup(m => m.ReadMilleniumFalconData(It.IsAny<string>()))
            .Returns(new MilleniumFalconConfig()
            {
                Autonomy = autonomy,
                Departure = departure,
                Arrival = arrival,
                Routes_Db = routeDb
            });

        //arrange
        OnboardComputerService onboardComputerService = new(Mock.Of<IRoutesRepository>(), mockConfigFileReader.Object);
        
        //assert
        Assert.AreEqual(false, onboardComputerService.LoadMilleniumFalconDatas("afile"));
    }

    [Test]
    public void LoadMilleniumFalconDatas_Should_Fill_Spaceship_State_And_TravelObjective()
    {
        //setups
        var mockConfigFileReader = new Mock<IConfigFileReader>();
        var mockRoutesRepository = new Mock<IRoutesRepository>();

        mockConfigFileReader.Setup(m => m.ReadMilleniumFalconData(It.IsAny<string>()))
            .Returns(new MilleniumFalconConfig()
            {
                Autonomy = 1,
                Departure = "Endor",
                Arrival = "Coruscant",
                Routes_Db = "routeDb"
            });
        mockRoutesRepository.Setup(m => m.GetPlanets())
            .Returns(new List<string>()
            {
                "Endor",
                "Coruscant"
            });
        mockRoutesRepository.Setup(m => m.GetRoutesByOrigin("Endor"))
            .Returns(new List<Route>()
            {
                new Route("Endor", "Coruscant", 1)
            });
        mockRoutesRepository.Setup(m => m.GetRoutesByOrigin("Coruscant"))
            .Returns(new List<Route>());

        //arrange
        OnboardComputerService onboardComputerService = new(mockRoutesRepository.Object, mockConfigFileReader.Object);
        bool isOk = onboardComputerService.LoadMilleniumFalconDatas("anyfile");

        //assert
        Assert.IsTrue(isOk);
        Assert.AreEqual("Endor", onboardComputerService.Departure);
        Assert.AreEqual("Coruscant", onboardComputerService.Destination);
        Assert.AreEqual(1, onboardComputerService.SpaceshipAutonomy);
        Assert.AreEqual(2, onboardComputerService.Map.Count);
        Assert.AreEqual(1, onboardComputerService.Map["Endor"].PlanetReacheableInformations.Count);
        Assert.AreEqual(0, onboardComputerService.Map["Coruscant"].PlanetReacheableInformations.Count);
    }

    [Test]
    [TestCase("C:\\directory\\myfiles\\routeDb.db", "C:\\directory\\millenium-falcon.json", "C:\\directory\\myfiles\\routeDb.db")]
    [TestCase("routeDb.db", "C:\\directory\\millenium-falcon.json", "C:\\directory\\routeDb.db")]
    public void LoadMilleniumFalconData_Should_Pass_Complete_Path_To_RouteRepository(string routeDb, string milleniumFalconFile, string expectedPathToDb)
    {
        //setups
        var mockConfigFileReader = new Mock<IConfigFileReader>();
        var mockRoutesRepository = new Mock<IRoutesRepository>();

        mockConfigFileReader.Setup(m => m.ReadMilleniumFalconData(It.IsAny<string>()))
            .Returns(new MilleniumFalconConfig()
            {
                Autonomy = 1,
                Departure = "Endor",
                Arrival = "Coruscant",
                Routes_Db = routeDb
            });

        //arrange
        OnboardComputerService onboardComputerService = new(mockRoutesRepository.Object, mockConfigFileReader.Object);
        bool isOk = onboardComputerService.LoadMilleniumFalconDatas(milleniumFalconFile);

        //assert
        Assert.IsTrue(isOk);
        mockRoutesRepository.Verify(m => m.UpdateDbPath(expectedPathToDb), Times.Once);
    }

    [Test]
    public void LoadEmpireDatas_Should_Complete_Spaceship_Countdown_And_BountyHunter_Informations()
    {
        //setups
        var mockConfigFileReader = new Mock<IConfigFileReader>();
        var mockRoutesRepository = new Mock<IRoutesRepository>();

        mockConfigFileReader.Setup(m => m.ReadMilleniumFalconData(It.IsAny<string>()))
            .Returns(new MilleniumFalconConfig()
            {
                Autonomy = 1,
                Departure = "Endor",
                Arrival = "Coruscant",
                Routes_Db = "routeDb"
            });
        mockConfigFileReader.Setup(m => m.ReadEmpireData(It.IsAny<string>()))
            .Returns(new EmpireConfig()
            {
                Countdown = 2,
                Bounty_Hunters = new List<BountyHunterConfig> { 
                    new BountyHunterConfig() { 
                        Planet = "Endor",
                        Day = 1
                    },
                }
            });

        mockRoutesRepository.Setup(m => m.GetPlanets())
            .Returns(new List<string>()
            {
                "Endor",
                "Coruscant"
            });
        mockRoutesRepository.Setup(m => m.GetRoutesByOrigin("Endor"))
            .Returns(new List<Route>()
            {
                new Route("Endor", "Coruscant", 1)
            });
        mockRoutesRepository.Setup(m => m.GetRoutesByOrigin("Coruscant"))
            .Returns(new List<Route>());

        //arrange
        OnboardComputerService onboardComputerService = new(mockRoutesRepository.Object, mockConfigFileReader.Object);
        bool isOk = onboardComputerService.LoadMilleniumFalconDatas("anyfile");
        isOk &= onboardComputerService.LoadEmpireDatas("anyfile");

        //assert
        Assert.IsTrue(isOk);
        Assert.AreEqual(2, onboardComputerService.Countdown);

        Assert.IsFalse(onboardComputerService.Map["Endor"].DaysWithBountyHunterPresence.Contains(0));
        Assert.IsTrue(onboardComputerService.Map["Endor"].DaysWithBountyHunterPresence.Contains(1));
        Assert.IsFalse(onboardComputerService.Map["Endor"].DaysWithBountyHunterPresence.Contains(2));
        Assert.IsFalse(onboardComputerService.Map["Coruscant"].DaysWithBountyHunterPresence.Contains(0));
        Assert.IsFalse(onboardComputerService.Map["Coruscant"].DaysWithBountyHunterPresence.Contains(1));
        Assert.IsFalse(onboardComputerService.Map["Coruscant"].DaysWithBountyHunterPresence.Contains(2));
    }

    [Test]
    public void LoadEmpireDatas_Can_Be_Called_Without_Planet_Data()
    {
        //setups
        var mockConfigFileReader = new Mock<IConfigFileReader>();
        var mockRoutesRepository = new Mock<IRoutesRepository>();

        mockConfigFileReader.Setup(m => m.ReadEmpireData(It.IsAny<string>()))
            .Returns(new EmpireConfig()
            {
                Countdown = 2,
                Bounty_Hunters = new List<BountyHunterConfig> {
                    new BountyHunterConfig() {
                        Planet = "Endor",
                        Day = 1
                    },
                }
            });

        //arrange
        OnboardComputerService onboardComputerService = new(mockRoutesRepository.Object, mockConfigFileReader.Object);
        bool isOk = onboardComputerService.LoadEmpireDatas("anyfile");

        //assert
        Assert.IsTrue(isOk);
        Assert.AreEqual(2, onboardComputerService.Countdown);
        Assert.AreEqual(0, onboardComputerService.Map.Count);
    }

    [Test]
    public void ComputeOddsToDestination_Should_Return_0_If_Lacking_Countdown()
    {
        //setups
        var mockConfigFileReader = new Mock<IConfigFileReader>();
        var mockRoutesRepository = new Mock<IRoutesRepository>();

        mockConfigFileReader.Setup(m => m.ReadMilleniumFalconData(It.IsAny<string>()))
            .Returns(new MilleniumFalconConfig()
            {
                Autonomy = 1,
                Departure = "Endor",
                Arrival = "Coruscant",
                Routes_Db = "routeDb"
            });

        //arrange
        OnboardComputerService onboardComputerService = new(mockRoutesRepository.Object, mockConfigFileReader.Object);
        bool isOk = onboardComputerService.LoadMilleniumFalconDatas("anyfile");

        //assert
        Assert.AreEqual(0, onboardComputerService.ComputeOddsToDestination());
    }

    [Test]
    public void ComputeOddsToDestination_Should_Return_0_If_Lacking_Departure_Or_Destination()
    {
        //setups
        var mockConfigFileReader = new Mock<IConfigFileReader>();
        var mockRoutesRepository = new Mock<IRoutesRepository>();

        mockConfigFileReader.Setup(m => m.ReadEmpireData(It.IsAny<string>()))
            .Returns(new EmpireConfig()
            {
                Countdown = 2,
                Bounty_Hunters = new List<BountyHunterConfig> {
                    new BountyHunterConfig() {
                        Planet = "Endor",
                        Day = 1
                    },
                }
            });

        //arrange
        OnboardComputerService onboardComputerService = new(mockRoutesRepository.Object, mockConfigFileReader.Object);
        bool isOk = onboardComputerService.LoadEmpireDatas("anyfile");

        //assert
        Assert.AreEqual(0, onboardComputerService.ComputeOddsToDestination());
    }

    [Test]
    public void ComputeOddsToDestination_Should_Return_Succes_Odds_To_Reach_Destination()
    {
        //setups
        var mockConfigFileReader = new Mock<IConfigFileReader>();
        var mockRoutesRepository = new Mock<IRoutesRepository>();

        mockConfigFileReader.Setup(m => m.ReadMilleniumFalconData(It.IsAny<string>()))
            .Returns(new MilleniumFalconConfig()
            {
                Autonomy = 1,
                Departure = "Endor",
                Arrival = "Coruscant",
                Routes_Db = "routeDb"
            });
        mockConfigFileReader.Setup(m => m.ReadEmpireData(It.IsAny<string>()))
            .Returns(new EmpireConfig()
            {
                Countdown = 2,
                Bounty_Hunters = new List<BountyHunterConfig>()
            });

        mockRoutesRepository.Setup(m => m.GetPlanets())
            .Returns(new List<string>()
            {
                "Endor",
                "Coruscant"
            });
        mockRoutesRepository.Setup(m => m.GetRoutesByOrigin("Endor"))
            .Returns(new List<Route>()
            {
                new Route("Endor", "Coruscant", 1)
            });
        mockRoutesRepository.Setup(m => m.GetRoutesByOrigin("Coruscant"))
            .Returns(new List<Route>());

        //arrange
        OnboardComputerService onboardComputerService = new(mockRoutesRepository.Object, mockConfigFileReader.Object);
        bool isOk = onboardComputerService.LoadMilleniumFalconDatas("anyfile");
        isOk &= onboardComputerService.LoadEmpireDatas("anyfile");

        //assert
        Assert.AreEqual(1, onboardComputerService.ComputeOddsToDestination());
    }

    [Test]
    //waiting on a Planet wont help here
    [TestCase(1, 0.9)]
    //has the time to wait one day on Endor to avoid the Bounty Hunter
    [TestCase(2, 1)]
    public void ComputeOddsToDestination_Should_Decrease_Odds_When_Encountering_BountyHunters(int countdown, double expectedOdds)
    {
        //setups
        var mockConfigFileReader = new Mock<IConfigFileReader>();
        var mockRoutesRepository = new Mock<IRoutesRepository>();

        mockConfigFileReader.Setup(m => m.ReadMilleniumFalconData(It.IsAny<string>()))
            .Returns(new MilleniumFalconConfig()
            {
                Autonomy = 1,
                Departure = "Endor",
                Arrival = "Coruscant",
                Routes_Db = "routeDb"
            });
        mockConfigFileReader.Setup(m => m.ReadEmpireData(It.IsAny<string>()))
            .Returns(new EmpireConfig()
            {
                Countdown = countdown,
                Bounty_Hunters = new List<BountyHunterConfig>()
                {
                    new BountyHunterConfig()
                    {
                        Planet = "Coruscant",
                        Day = 1
                    },
                }
            });

        mockRoutesRepository.Setup(m => m.GetPlanets())
            .Returns(new List<string>()
            {
                "Endor",
                "Coruscant"
            });
        mockRoutesRepository.Setup(m => m.GetRoutesByOrigin("Endor"))
            .Returns(new List<Route>()
            {
                new Route("Endor", "Coruscant", 1)
            });
        mockRoutesRepository.Setup(m => m.GetRoutesByOrigin("Coruscant"))
            .Returns(new List<Route>());

        //arrange
        OnboardComputerService onboardComputerService = new(mockRoutesRepository.Object, mockConfigFileReader.Object);
        bool isOk = onboardComputerService.LoadMilleniumFalconDatas("anyfile");
        isOk &= onboardComputerService.LoadEmpireDatas("anyfile");

        //assert
        Assert.AreEqual(expectedOdds, onboardComputerService.ComputeOddsToDestination());
    }

    [Test]
    public void ComputeOddsToDestination_Should_Decrease_Odds_When_Refueling_And_Encountering_BountyHunters()
    {
        //setups
        var mockConfigFileReader = new Mock<IConfigFileReader>();
        var mockRoutesRepository = new Mock<IRoutesRepository>();

        mockConfigFileReader.Setup(m => m.ReadMilleniumFalconData(It.IsAny<string>()))
            .Returns(new MilleniumFalconConfig()
            {
                Autonomy = 1,
                Departure = "Endor",
                Arrival = "Coruscant",
                Routes_Db = "routeDb"
            });
        mockConfigFileReader.Setup(m => m.ReadEmpireData(It.IsAny<string>()))
            .Returns(new EmpireConfig()
            {
                Countdown = 3,
                Bounty_Hunters = new List<BountyHunterConfig>()
                {
                    new BountyHunterConfig()
                    {
                        Planet = "Naboo",
                        Day = 1
                    }
                }
            });

        mockRoutesRepository.Setup(m => m.GetPlanets())
            .Returns(new List<string>()
            {
                "Endor",
                "Naboo",
                "Coruscant"
            });
        mockRoutesRepository.Setup(m => m.GetRoutesByOrigin("Endor"))
            .Returns(new List<Route>()
            {
                new Route("Endor", "Naboo", 1)
            });
        mockRoutesRepository.Setup(m => m.GetRoutesByOrigin("Naboo"))
            .Returns(new List<Route>()
            {
                new Route("Naboo", "Coruscant", 1)
            });
        mockRoutesRepository.Setup(m => m.GetRoutesByOrigin("Coruscant"))
            .Returns(new List<Route>());

        //arrange
        OnboardComputerService onboardComputerService = new(mockRoutesRepository.Object, mockConfigFileReader.Object);
        bool isOk = onboardComputerService.LoadMilleniumFalconDatas("anyfile");
        isOk &= onboardComputerService.LoadEmpireDatas("anyfile");

        //assert
        Assert.AreEqual(1 - Math.Pow(9, 0) / Math.Pow(10, 1) - Math.Pow(9, 1) / Math.Pow(10, 2), 
            onboardComputerService.ComputeOddsToDestination());
    }


    [Test]
    [TestCase(2, 0)]
    [TestCase(3, 1)]
    public void ComputeOddsToDestination_Should_Increase_NbTravelDay_When_Refueling_And_Encountering_BountyHunters(int countdown, double expectedOdds)
    {
        //setups
        var mockConfigFileReader = new Mock<IConfigFileReader>();
        var mockRoutesRepository = new Mock<IRoutesRepository>();

        mockConfigFileReader.Setup(m => m.ReadMilleniumFalconData(It.IsAny<string>()))
            .Returns(new MilleniumFalconConfig()
            {
                Autonomy = 1,
                Departure = "Endor",
                Arrival = "Coruscant",
                Routes_Db = "routeDb"
            });
        mockConfigFileReader.Setup(m => m.ReadEmpireData(It.IsAny<string>()))
            .Returns(new EmpireConfig()
            {
                Countdown = countdown,
                //no bounty hunters
            });

        mockRoutesRepository.Setup(m => m.GetPlanets())
            .Returns(new List<string>()
            {
                "Endor",
                "Naboo",
                "Coruscant"
            });
        mockRoutesRepository.Setup(m => m.GetRoutesByOrigin("Endor"))
            .Returns(new List<Route>()
            {
                new Route("Endor", "Naboo", 1)
            });
        mockRoutesRepository.Setup(m => m.GetRoutesByOrigin("Naboo"))
            .Returns(new List<Route>()
            {
                new Route("Naboo", "Coruscant", 1)
            });
        mockRoutesRepository.Setup(m => m.GetRoutesByOrigin("Coruscant"))
            .Returns(new List<Route>());

        //arrange
        OnboardComputerService onboardComputerService = new(mockRoutesRepository.Object, mockConfigFileReader.Object);
        bool isOk = onboardComputerService.LoadMilleniumFalconDatas("anyfile");
        isOk &= onboardComputerService.LoadEmpireDatas("anyfile");

        //assert
        Assert.AreEqual(expectedOdds,
            onboardComputerService.ComputeOddsToDestination());
    }
}
