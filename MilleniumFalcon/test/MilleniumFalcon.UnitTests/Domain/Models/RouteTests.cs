
using Backend.Domain.Models;

namespace Backend.UnitTests.Domain.Models
{
    [TestFixture]
    public class RouteTests
    {
        [Test]
        [TestCase("", "Endor", 1)]
        [TestCase("Endor", "", 1)]
        [TestCase(null, "Endor", 1)]
        [TestCase("", null, 1)]
        [TestCase(null, null, 1)]
        [TestCase("Endor", "Endor", 1)]
        [TestCase("Endor", "Coruscant", 0)]
        [TestCase("Endor", "Coruscant", -1)]
        public void Ctor_Should_Throw_Invalid_Input(string origin, string destination, int travelTime)
        {
            Assert.That(() => new Route(origin, destination, travelTime),
                Throws.Exception);
        }

        [Test]
        public void Route_Should_Allow_Getter_On_Its_Property()
        {
            Route r = new("origin", "destination", 1);

            Assert.AreEqual("origin", r.Origin);
            Assert.AreEqual("destination", r.Destination);
            Assert.AreEqual(1, r.TravelTime);
        }
    }
}
