using Backend.Infrastructure.Repository;

namespace Backend.IntegrationTests.Repository;

[TestFixture]
public class RoutesRepositoryTests
{
    [Test]
    public void LoadCacheFromDatabase_Should_Load_Routes()
    {
        RoutesRepository repo = new();

        Assert.IsNotNull(repo.Routes);
        Assert.AreEqual(0, repo.Routes.Count);

        Assert.IsTrue(repo.LoadCacheFromDatabase($"{AppContext.BaseDirectory}universe.db"));

        Assert.AreEqual(2, repo.GetRoutesByOrigin("Tatooine").Count());
        Assert.AreEqual(3, repo.GetRoutesByOrigin("Dagobah").Count());
        Assert.AreEqual(3, repo.GetRoutesByOrigin("Hoth").Count());
        Assert.AreEqual(2, repo.GetRoutesByOrigin("Endor").Count());
    }

    [Test]
    public void GetRoutesByOrigin_Should_Load_Routes_That_Can_Be_Travelled_From_Origin()
    {
        RoutesRepository repo = new();
        Assert.IsTrue(repo.LoadCacheFromDatabase($"{AppContext.BaseDirectory}universe.db"));

        Assert.AreEqual(2, repo.GetRoutesByOrigin("Tatooine").Count());
        Assert.AreEqual(3, repo.GetRoutesByOrigin("Dagobah").Count());
        Assert.AreEqual(3, repo.GetRoutesByOrigin("Hoth").Count());
        Assert.AreEqual(2, repo.GetRoutesByOrigin("Endor").Count());
    }

    [Test]
    public void LoadCacheFromDatabase_Should_Load_PlanetsName()
    {
        RoutesRepository repo = new();

        Assert.IsNotNull(repo.PlanetsName);
        Assert.AreEqual(0, repo.PlanetsName.Count);

        Assert.IsTrue(repo.LoadCacheFromDatabase($"{AppContext.BaseDirectory}universe.db"));

        Assert.AreEqual(4, repo.PlanetsName.Count);
        Assert.IsTrue(repo.PlanetsName.Contains("Tatooine"));
        Assert.IsTrue(repo.PlanetsName.Contains("Dagobah"));
        Assert.IsTrue(repo.PlanetsName.Contains("Hoth"));
        Assert.IsTrue(repo.PlanetsName.Contains("Endor"));
    }

    [Test]
    public void LoadCacheFromDatabase_Should_Return_False_If_DbPath_IsNullOrEmpty()
    {
        RoutesRepository repo = new();

        Assert.IsFalse(repo.LoadCacheFromDatabase(""));
        Assert.IsFalse(repo.LoadCacheFromDatabase(null));
    }

    [Test]
    public void LoadCacheFromDatabase_Should_Return_False_If_DbPath_IsNotFound()
    {
        RoutesRepository repo = new();
        Assert.IsFalse(repo.LoadCacheFromDatabase("anyfile"));
    }

    [Test]
    public void LoadCacheFromDatabase_Should_Update_DbSource()
    {
        RoutesRepository repo = new();
        Assert.DoesNotThrow(() => repo.LoadCacheFromDatabase($"{AppContext.BaseDirectory}universe.db"));
        Assert.AreEqual(2, repo.GetRoutesByOrigin("Tatooine").Count());

        Assert.DoesNotThrow(() => repo.LoadCacheFromDatabase($"{AppContext.BaseDirectory}wrong.db"));
        Assert.AreEqual(0, repo.GetRoutesByOrigin("Tatooine").Count());
    }
}