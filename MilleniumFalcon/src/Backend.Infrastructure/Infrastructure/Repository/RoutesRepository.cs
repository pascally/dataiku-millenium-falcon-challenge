using Backend.Application.Interfaces.Repository;
using Backend.Domain.Models;
using Microsoft.Data.Sqlite;

namespace Backend.Infrastructure.Repository;

public class RoutesRepository : IRoutesRepository
{
    /// <summary>
    /// Cache of data loaded from DB
    /// </summary>
    public List<Route> Routes { get; } = new();

    public HashSet<string> PlanetsName { get; } = new();

    /// <summary>
    /// Repository of routes loaded from Database
    /// </summary>
    public RoutesRepository() {}

    /// <summary>
    /// Return Routes by origin
    /// </summary>
    /// <param name="origin">origin planet of routes returned</param>
    /// <returns>routes</returns>
    public IEnumerable<Route> GetRoutesByOrigin(string origin)
    {
        return Routes.Where(r => r.Origin == origin).OrderBy(r => r.TravelTime);
    }

    /// <summary>
    /// Load the cache from DB
    /// </summary>
    /// <param name="dbPath">path of the Database</param>
    public bool LoadCacheFromDatabase(string dbPath)
    {
        PlanetsName.Clear();
        Routes.Clear();

        try
        {
            if (string.IsNullOrEmpty(dbPath))
            {
                throw new ArgumentNullException("dbPath");
            }

            if (!File.Exists(dbPath))
            {
                throw new ArgumentException("Database file couldnt be found");
            }

            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
            @"
                    SELECT *
                    FROM routes
                ";

            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var origin = reader.GetString(0);
                var destination = reader.GetString(1);
                var travelTime = reader.GetInt32(2);

                //only load route that are valid
                if (!string.IsNullOrEmpty(origin) && !string.IsNullOrEmpty(destination) && travelTime > 0)
                {
                    Routes.Add(new Route(origin, destination, travelTime));
                    //routes can be travelled both ways
                    Routes.Add(new Route(destination, origin, travelTime));

                    PlanetsName.Add(origin);
                    PlanetsName.Add(destination);
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"LoadCacheFromDatabase {ex.Message}");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Returns list of planets name
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> GetPlanets() => PlanetsName;
}
