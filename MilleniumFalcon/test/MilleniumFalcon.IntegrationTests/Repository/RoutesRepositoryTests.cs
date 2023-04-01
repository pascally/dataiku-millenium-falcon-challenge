using Backend.Infrastructure.Repository;

namespace Backend.IntegrationTests.Repository
{
    [TestFixture]
    public class RoutesRepositoryTests
    {
        [Test]
        public void Ctor_Should_Throw_If_PathToDb_Is_NullOrEmpty()
        {
            Assert.Throws<ArgumentNullException>(() => new RoutesRepository(""));
            Assert.Throws<ArgumentNullException>(() => new RoutesRepository(null));
        }

        [Test]
        public void LoadCacheFromDatabase_Should_Load_Routes()
        {
            RoutesRepository repo = new($"{AppContext.BaseDirectory}universe.db");

            Assert.IsNotNull(repo.Routes);
            Assert.AreEqual(0, repo.Routes.Count);

            Assert.DoesNotThrow(() => repo.LoadCacheFromDatabase());

            Assert.AreEqual(2, repo.GetRoutesByOrigin("Tatooine").Count());
            Assert.AreEqual(3, repo.GetRoutesByOrigin("Dagobah").Count());
            Assert.AreEqual(3, repo.GetRoutesByOrigin("Hoth").Count());
            Assert.AreEqual(2, repo.GetRoutesByOrigin("Endor").Count());
        }

        [Test]
        public void GetRoutesByOrigin_Should_Load_Routes_That_Can_Be_Travelled_From_Origin()
        {
            RoutesRepository repo = new($"{AppContext.BaseDirectory}universe.db");
            Assert.DoesNotThrow(() => repo.LoadCacheFromDatabase());

            Assert.AreEqual(2, repo.GetRoutesByOrigin("Tatooine").Count());
            Assert.AreEqual(3, repo.GetRoutesByOrigin("Dagobah").Count());
            Assert.AreEqual(3, repo.GetRoutesByOrigin("Hoth").Count());
            Assert.AreEqual(2, repo.GetRoutesByOrigin("Endor").Count());
        }

        [Test]
        public void LoadCacheFromDatabase_Should_Load_PlanetsName()
        {
            RoutesRepository repo = new($"{AppContext.BaseDirectory}universe.db");

            Assert.IsNotNull(repo.PlanetsName);
            Assert.AreEqual(0, repo.PlanetsName.Count);

            Assert.DoesNotThrow(() => repo.LoadCacheFromDatabase());

            Assert.AreEqual(4, repo.PlanetsName.Count);
            Assert.IsTrue(repo.PlanetsName.Contains("Tatooine"));
            Assert.IsTrue(repo.PlanetsName.Contains("Dagobah"));
            Assert.IsTrue(repo.PlanetsName.Contains("Hoth"));
            Assert.IsTrue(repo.PlanetsName.Contains("Endor"));
        }

        [Test]
        public void UpdateDbPath_Should_Throw_If_DbPath_IsNullOrEmpty()
        {
            RoutesRepository repo = new($"{AppContext.BaseDirectory}universe.db");

            Assert.Throws<ArgumentNullException>(() => repo.UpdateDbPath(""));
            Assert.Throws<ArgumentNullException>(() => repo.UpdateDbPath(null));

        }

        [Test]
        public void UpdateDbPath_Should_UpdateDbPath()
        {
            RoutesRepository repo = new($"{AppContext.BaseDirectory}universe.db");
            Assert.DoesNotThrow(() => repo.LoadCacheFromDatabase());
            Assert.AreEqual(2, repo.GetRoutesByOrigin("Tatooine").Count());

            repo.UpdateDbPath("wrong.db");
            Assert.DoesNotThrow(() => repo.LoadCacheFromDatabase());
            Assert.AreEqual(0, repo.GetRoutesByOrigin("Tatooine").Count());
        }
    }
}