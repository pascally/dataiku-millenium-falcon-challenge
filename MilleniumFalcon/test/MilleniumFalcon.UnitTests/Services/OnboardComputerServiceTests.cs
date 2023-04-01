using Backend.Application.Config;
using Backend.Application.Interfaces.Common;
using Backend.Application.Interfaces.Repository;
using Backend.Application.Services;
using Moq;

namespace Backend.UnitTests.Services
{
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
            var mockConfigFileReader = new Mock<IConfigFileReader>();

            mockConfigFileReader.Setup(m => m.ReadEmpireData(It.IsAny<string>()))
                .Returns(new EmpireConfig()
                {
                    Countdown = -1
                });

            OnboardComputerService onboardComputerService = new(Mock.Of<IRoutesRepository>(), mockConfigFileReader.Object);
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
            var mockConfigFileReader = new Mock<IConfigFileReader>();

            mockConfigFileReader.Setup(m => m.ReadMilleniumFalconData(It.IsAny<string>()))
                .Returns(new MilleniumFalconConfig()
                {
                    Autonomy = autonomy,
                    Departure = departure,
                    Arrival = arrival,
                    Routes_Db = routeDb
                });

            OnboardComputerService onboardComputerService = new(Mock.Of<IRoutesRepository>(), mockConfigFileReader.Object);
            Assert.AreEqual(false, onboardComputerService.LoadMilleniumFalconDatas("afile"));
        }

        [Test]
        public void LoadMilleniumFalconDatas_Should_Fill_Spaceship_State_And_TravelObjective()
        {

        }
    }
}
