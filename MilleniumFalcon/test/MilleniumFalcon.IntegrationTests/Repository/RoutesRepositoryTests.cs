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
            Assert.DoesNotThrow(() => repo.LoadCacheFromDatabase());

            Assert.AreEqual(2, repo.GetRoutesByOrigin("Tatooine").Count());
            Assert.AreEqual(2, repo.GetRoutesByOrigin("Dagobah").Count());
            Assert.AreEqual(1, repo.GetRoutesByOrigin("Hoth").Count());
        }

        [Test]
        public void LoadCacheFromDatabase_Should_Load_PlanetsName()
        {
            RoutesRepository repo = new($"{AppContext.BaseDirectory}universe.db");
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