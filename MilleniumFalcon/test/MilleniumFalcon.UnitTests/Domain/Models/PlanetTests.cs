
using Backend.Domain.Models;

namespace Backend.UnitTests.Domain.Models
{
    [TestFixture]
    public class PlanetTests
    {
        public static object[] Ctor_Should_Throw_Invalid_InputCases =
        {
            new object[] { "", new List<Route>() },
            new object[] { null, new List<Route>() },
            new object[] { "Endor", null },
        };

        [Test]
        [TestCaseSource(nameof(Ctor_Should_Throw_Invalid_InputCases))]
        public void Ctor_Should_Throw_Invalid_Input(string name, List<Route> routes)
        {
            Assert.That(() => new Planet(name, routes),
                Throws.Exception);
        }

        [Test]
        public void Ctor_Should_Only_Keep_Route_With_Name_As_Origin()
        {
            Planet planet = new("Endor", new List<Route> { new Route("Endor", "Coruscant", 2), new Route("Tatooine", "Coruscant", 1) });
            Assert.AreEqual(1, planet.PlanetReacheableInformations.Count);
            Assert.AreEqual("Endor", planet.PlanetReacheableInformations[0].Origin);
            Assert.AreEqual("Coruscant", planet.PlanetReacheableInformations[0].Destination);
            Assert.AreEqual(2, planet.PlanetReacheableInformations[0].TravelTime);
        }

        [Test]
        public void Ctor_For_Duplicate_Route_Should_Keep_Shortest_Path()
        {
            Planet planet = new("Endor", new List<Route> { new Route("Endor", "Coruscant", 2), new Route("Endor", "Coruscant", 1) });
            Assert.AreEqual(1, planet.PlanetReacheableInformations.Count);
            Assert.AreEqual(1, planet.PlanetReacheableInformations[0].TravelTime);
        }

        [Test]
        public void UpdateBountyHunterPresence_Should_Add_BountyHunters_PresenceDay()
        {
            Planet planet = new("Endor", new List<Route>());
            List<int> presenceDays = new List<int> { 1, 3, 5 };
            planet.UpdateBountyHunterPresence(presenceDays);

            Assert.AreEqual(presenceDays.Count, planet.DaysWithBountyHunterPresence.Count);

            foreach (int day in presenceDays)
            {
                Assert.IsTrue(planet.DaysWithBountyHunterPresence.Contains(day));
            }
        }

        [Test]
        public void UpdateBountyHunterPresence_Should_Only_Positive_Number_As_Day()
        {
            Planet planet = new("Endor", new List<Route>());
            List<int> presenceDays = new List<int> { -10, -1, 0, 1, 3, 5 };
            planet.UpdateBountyHunterPresence(presenceDays);

            Assert.AreEqual(presenceDays.Where(d => d >= 0).Count(), planet.DaysWithBountyHunterPresence.Count);

            foreach (int day in presenceDays.Where(d => d >= 0))
            {
                Assert.IsTrue(planet.DaysWithBountyHunterPresence.Contains(day));
            }
        }

        [Test]
        public void HasBountyHunter_Should_Returns_When_BountyHunter_Is_Present()
        {
            Planet planet = new("Endor", new List<Route>());
            List<int> presenceDays = new List<int> { 1, 3, 5 };
            planet.UpdateBountyHunterPresence(presenceDays);

            foreach (int day in presenceDays)
            {
                Assert.IsTrue(planet.HasBountyHunter(day));
            }
        }
    }
}