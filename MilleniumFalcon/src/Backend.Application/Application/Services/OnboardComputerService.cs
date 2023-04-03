using Backend.Application.Config;
using Backend.Application.Interfaces.Common;
using Backend.Application.Interfaces.Repository;
using Backend.Domain.Models;
using Backend.Domain.UseCases;

namespace Backend.Application.Services;

public class OnboardComputerService : OnboardComputerUsecases
{
    private IRoutesRepository _routesRepository;
    private IConfigFileReader _jsonFileReader;

    public Dictionary<string, Planet> Map { get; } = new();
    public string Departure { get; private set; } = string.Empty;
    public string Destination { get; private set; } = string.Empty;

    public int Countdown { get; private set; }

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

        if (configFileReader == null) throw new ArgumentNullException("jsonFileReader"); 
        _jsonFileReader = configFileReader;
    }

    /// <summary>
    /// Returns the odds of Success between Departure and Destination Planets
    /// </summary>
    /// <returns>Odds in percentage</returns>
    public double ComputeOddsToDestination()
    {
        //by default we have one solution with Odds to Get Captured = 1.0
        //any path reaching Destination will have a lower Odd
        //here we ll always have only one element 
        //but doing so we could add more odds later on
        List<double> solution = new() { 1.0 };

        if (!Map.ContainsKey(Destination) || !Map.ContainsKey(Departure) || Countdown == 0)
        {
            return 0.0;
        }

        FindPath(SpaceshipAutonomy, Map[Departure], 0, 0, 0.0, solution);

        return (1 - solution.First()) * 100.0;
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
    /// <param name="currentNbEncounterBountyHunters">Current number of encounter with Bounty Hunters</param>
    /// <param name="currentOddsToGetCaptured">Odds to get captured by Bounty Hunter</param>
    /// <param name="solution">List containing the best solution
    /// </param>
    private void FindPath(
        int currentFuel, 
        Planet currentLocation, 
        int currentDay, 
        int currentNbEncounterBountyHunters, 
        double currentOddsToGetCaptured,
        List<double> solution)
    {
        //everytime we encounter a bounty hunter the odds to get capture
        //increase of 9^k / 10^k+1
        //k being the number of encounter
        if (currentLocation.HasBountyHunter(currentDay))
        {
            currentOddsToGetCaptured += Math.Pow(9, currentNbEncounterBountyHunters) / Math.Pow(10, currentNbEncounterBountyHunters + 1);
            currentNbEncounterBountyHunters++;
        }

        //path with no solution, either countdwon is reached or crew get captured for sure
        if (currentDay > Countdown || currentOddsToGetCaptured >= 1)
        {
            return;
        }

        //if we reached destination
        if (currentLocation.Name == Destination)
        {
            solution[0] = Math.Min(solution[0], currentOddsToGetCaptured);
            return;
        }

        while (currentDay < Countdown)
        {
            //otherwise
            //either for that GIVEN DAY
            //for all routes available starting the current planet within the Millenium Falcon AUTONOMY
            //we decide to MOVE
            foreach (Route route in
                currentLocation.PlanetReacheableInformations.Where(r => SpaceshipAutonomy >= r.TravelTime))
            {
                //take a copy of currentDay + currentFuel + currentNbEncounterBountyHunters + currentOddsToGetCaptured
                //as depending of each route taken, theses values can change
                int fuel = currentFuel;
                int day = currentDay;
                int nbEncounterBountyHunters = currentNbEncounterBountyHunters;
                double oddsToGetCaptured = currentOddsToGetCaptured;

                //if not enough fuel , we have to refuel
                if (route.TravelTime > fuel)
                {
                    fuel = SpaceshipAutonomy;

                    //if bounty hunter present on that day
                    if (currentLocation.HasBountyHunter(day))
                    {
                        oddsToGetCaptured += Math.Pow(9, nbEncounterBountyHunters) / Math.Pow(10, nbEncounterBountyHunters + 1);
                        nbEncounterBountyHunters++;
                    }
                    day++;
                }

                //we decreased the fuel level, change the current location of the spaceship, increase the day by travel time,
                //pass a copy of the list of solutions
                //and the current number of encounter with bounty hunters
                FindPath(fuel - route.TravelTime, Map[route.Destination], day + route.TravelTime, nbEncounterBountyHunters, oddsToGetCaptured, solution);
            }

            //or We WAIT one day
            currentDay++;
        }
    }

    /// <summary>
    /// Load empire config data
    /// </summary>
    /// <param name="pathToEmpireDatas">path to json empire file config</param>
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

            if (infos == null)
            {
                throw new ArgumentException("Failed to read Empire config");
            }

            if (infos.Countdown <= 0)
            {
                throw new ArgumentException("Countdown cannot be under 0");
            }

            Countdown = infos.Countdown;

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
    /// Load Millenium Falcon config file
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

            if (infos == null)
            {
                throw new ArgumentException("Failed to read Millenium falcon config");
            }

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

            string dbPath = Path.IsPathFullyQualified(infos.Routes_Db) ? infos.Routes_Db : Path.Combine(Path.GetDirectoryName(pathToMilleniumFalconDatas), infos.Routes_Db);

            if (!_routesRepository.LoadCacheFromDatabase(dbPath))
            {
                throw new ArgumentException("Failed to load routes database");
            }

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
