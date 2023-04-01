using Backend.Application.Config;
using Backend.Application.Interfaces.Common;
using Backend.Application.Interfaces.Repository;
using Backend.Domain.Models;
using Backend.Domain.UseCases;

namespace Backend.Application.Services
{
    public class OnboardComputerService : OnboardComputerUsecases
    {
        private IRoutesRepository _routesRepository;
        private IConfigFileReader _jsonFileReader;

        private Dictionary<string, Planet> Map { get; } = new();
        private string Departure { get; set; }
        private string Destination { get; set; }

        private int Countdown { get; set; }

        public int SpaceshipAutonomy { get; private set; }

        /// <summary>
        /// Implements the onboard computer service and its usecases
        /// </summary>
        /// <param name="routesRepository">Repository to access Routes database</param>
        /// <param name="configFileReader">Config file reader</param>
        /// <exception cref="ArgumentNullException">if any input is null</exception>
        public OnboardComputerService(IRoutesRepository routesRepository, IConfigFileReader configFileReader) 
        {
            if (routesRepository == null) throw new ArgumentNullException("routesRepository");
            _routesRepository = routesRepository;
            routesRepository.LoadCacheFromDatabase();

            if (configFileReader == null) throw new ArgumentNullException("jsonFileReader"); 
            _jsonFileReader = configFileReader;
        }

        /// <summary>
        /// Returns the odds of Success between Departure and Destination Planets
        /// </summary>
        /// <returns>Odds</returns>
        public double ComputeOddsToDestination()
        {
            //use a min heap
            //with the best solution on the top
            PriorityQueue<List<Planet>, double> solutions = new();

            if (!Map.ContainsKey(Destination) || !Map.ContainsKey(Departure))
            {
                return 0.0;
            }

            FindPath(SpaceshipAutonomy, Map[Departure], 0, 0, 0.0, new List<Planet>(), solutions);

            if (solutions.Count > 0)
            {
                //we dont care about the list of planets just the priority, ie the odds to be captured here
                solutions.TryDequeue(out var _, out double priority);
                return 1 - priority;
            }

            return 0.0;
        }

        /// <summary>
        /// Find a path to Destination planet within the CountDown + the fact that the Odds to get Captured cannot goes above 100% constraints
        /// Routes taken needs to have their travel time within the Spaceship Autonomy reach
        /// Otherwise routes can be taken in any order, and multiple times
        /// FindPath traverse the 'graph' of Planets, with a Planet being a Node, and the routes being the link between Planets
        /// </summary>
        /// <param name="currentFuel">Current level of Fuel</param>
        /// <param name="currentLocation">Current Planet location</param>
        /// <param name="currentDay">Current Day</param>
        /// <param name="nbEncounterBountyHunters">Current number of encounter with Bounty Hunters</param>
        /// <param name="oddsToGetCaptured">Odds to get captured by Bounty Hunter</param>
        /// <param name="planets">Current Planets visited</param>
        /// <param name="solutions">List of all possibles path from Departure and Destination planets</param>
        private void FindPath(int currentFuel, Planet currentLocation, int currentDay, int nbEncounterBountyHunters, double oddsToGetCaptured, List<Planet> planets, PriorityQueue<List<Planet>, double> solutions)
        {
            //path with no solution, either countdwon is reached or crew get captured for sure
            if (currentDay > Countdown || oddsToGetCaptured >= 1)
            {
                return;
            }

            //add current location of list of planet traversed
            planets.Add(currentLocation);

            //if we reached destination
            if (currentLocation.Name == Destination)
            {
                //enqueue a copy of travel path + nb encounters with bounty hunters associated
                solutions.Enqueue(planets, nbEncounterBountyHunters);
                return;
            }

            //otherwise for all routes available starting the current planet within the Millenium Falcon AUTONOMY
            foreach (Route route in currentLocation.PlanetReacheableInformations.Where(r => SpaceshipAutonomy >= r.TravelTime))
            {
                //everytime we encounter a bounty hunter even if Han Solo shoot first, the odds to get capture increase of 9^k / 10^k+1
                //k being the number of encounter
                if (currentLocation.HasBountyHunter(currentDay))
                {
                    oddsToGetCaptured += Math.Pow(9, nbEncounterBountyHunters) / Math.Pow(10, nbEncounterBountyHunters + 1);
                    nbEncounterBountyHunters++;
                }

                //if not enough fuel , we have to refuel
                if (route.TravelTime > currentFuel)
                {
                    currentFuel = SpaceshipAutonomy;

                    //if bounty hunter present on that day
                    if (currentLocation.HasBountyHunter(currentDay))
                    {
                        oddsToGetCaptured += Math.Pow(9, nbEncounterBountyHunters) / Math.Pow(10, nbEncounterBountyHunters + 1);
                        nbEncounterBountyHunters++;
                    }
                }

                //we decreased the fuel level, change the current location of the spaceship, increase the day by travel time, pass the list of solutions
                //and the current number of encounter with bounty hunters
                //we pass a COPY of the current list of planets traversed up until now
                FindPath(currentFuel - route.TravelTime, Map[route.Destination], currentDay + route.TravelTime, nbEncounterBountyHunters, oddsToGetCaptured, new(planets), solutions);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathToEmpireDatas"></param>
        /// <returns>
        /// False if pathToEmpireDatas is null or empty
        /// False if Countdown read is < 0
        /// </returns>
        public bool LoadEmpireDatas(string pathToEmpireDatas)
        {
            EmpireConfig infos = null;

            try
            {
                if (string.IsNullOrEmpty(pathToEmpireDatas))
                {
                    throw new ArgumentNullException("pathToEmpireDatas");
                }

                infos = _jsonFileReader.ReadEmpireData(pathToEmpireDatas);

                if (infos.Countdown <= 0)
                {
                    throw new ArgumentException("Countdown cannot be under 0");
                }

                Dictionary<string, List<int>> dayWithBountyHuntersByPlanet = new();

                //group presence day by planet
                foreach (var bountyHunterConfig in infos.Bounty_Hunters)
                {
                    if (dayWithBountyHuntersByPlanet.ContainsKey(bountyHunterConfig.Planet))
                    {
                        dayWithBountyHuntersByPlanet[bountyHunterConfig.Planet].Add(bountyHunterConfig.Day);
                    }
                    else
                    {
                        dayWithBountyHuntersByPlanet.Add(bountyHunterConfig.Planet, new List<int>() { bountyHunterConfig.Day });
                    }
                }

                //update Map information to linked it with the Planet object
                foreach (var planet in dayWithBountyHuntersByPlanet.Keys)
                {
                    if (Map.ContainsKey(planet))
                    {
                        Map[planet].UpdateBountyHunterPresence(dayWithBountyHuntersByPlanet[planet]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

            
            return infos != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathToMilleniumFalconDatas">path to json file containing Millenium Falcon informations</param>
        /// <returns>
        /// False if pathToMilleniumFalconDatas is null or empty
        /// False if Departure is null or empty
        /// False if Arrival is null or empty
        /// False if RouteDb Path is null or empty
        /// </returns>
        public bool LoadMilleniumFalconDatas(string pathToMilleniumFalconDatas)
        {
            Map.Clear();
            MilleniumFalconConfig infos = null;

            try
            {
                if (string.IsNullOrEmpty(pathToMilleniumFalconDatas))
                {
                    throw new ArgumentNullException("pathToMilleniumFalconDatas");
                }

                infos = _jsonFileReader.ReadMilleniumFalconData(pathToMilleniumFalconDatas);

                if (infos.Autonomy <= 0)
                {
                    throw new ArgumentException("autonomy cannot be under 0");
                }

                if (string.IsNullOrEmpty(infos.Departure))
                {
                    throw new ArgumentException("Departure planet cannot be empty");
                }

                if (string.IsNullOrEmpty(infos.Arrival))
                {
                    throw new ArgumentException("Arrival planet cannot be empty");
                }

                if (string.IsNullOrEmpty(infos.Routes_Db))
                {
                    throw new ArgumentException("Path to Routes Databases cannot be empty");
                }

                _routesRepository.UpdateDbPath(infos.Routes_Db);
                _routesRepository.LoadCacheFromDatabase();

                foreach (string name in _routesRepository.GetPlanets())
                {
                    var planet = new Planet(name: name, routes: _routesRepository.GetRoutesByOrigin(name));
                    Map.Add(name, planet);
                }

                SpaceshipAutonomy = infos.Autonomy;
                Departure = infos.Departure;
                Destination = infos.Arrival;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

            return infos != null;
        }
    }
}
