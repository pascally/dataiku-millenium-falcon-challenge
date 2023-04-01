
namespace Backend.Domain.Models
{
    /// <summary>
    /// Represent a Planet and the informations related to it
    /// Days where Bounty hunter are present on it
    /// The planets reachable from it and how long it takes
    /// </summary>
    public class Planet
    {
        public string Name { get; }

        public SortedSet<int> DaysWithBountyHunterPresence { get; } = new(); //ctor by default

        /// <summary>
        /// Planet that can be reach
        /// min-heap used here: travel time with the lowest distance get dequeued first
        /// </summary>
        public List<Route> PlanetReacheableInformations { get; } = new(); //ctor by default

        public Planet(string name, IEnumerable<Route> routes) 
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            if (routes == null)
            {
                throw new ArgumentNullException("routes");
            }

            Name = name;

            HashSet<string> destination = new ();

            //for a given planet only keep the route that we are at the origin order by travel time
            //in case of data corruption with multiple routes toward same planet
            //we keep the one with shortest travel path
            foreach (Route route in routes.Where(r => r.Origin == name).OrderBy(r => r.TravelTime))
            {
                if (!destination.Contains(route.Destination))
                {
                    PlanetReacheableInformations.Add(route);
                    destination.Add(route.Destination);
                }
            }
        }

        /// <summary>
        /// Returns if bounty hunter is present on the Planet on a particular day
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public bool HasBountyHunter(int day)
        {
            return DaysWithBountyHunterPresence.Contains(day);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="days"></param>
        public void UpdateBountyHunterPresence(List<int> days)
        {
            //we only keep value >= 0
            foreach (var day in days.Where(d => d >= 0))
            {
                DaysWithBountyHunterPresence.Add(day);
            }
        }
    }
}
