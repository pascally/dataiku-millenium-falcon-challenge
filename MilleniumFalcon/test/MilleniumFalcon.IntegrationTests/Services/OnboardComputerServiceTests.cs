using Backend.Application.Interfaces.Common;
using Backend.Application.Interfaces.Repository;
using Backend.Application.Services;
using Moq;

namespace Backend.IntegrationTests.Services
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
    }
}
