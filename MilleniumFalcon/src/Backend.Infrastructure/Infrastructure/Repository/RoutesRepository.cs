using Backend.Application.Interfaces.Repository;
using Backend.Domain.Models;
using Microsoft.Data.Sqlite;

namespace Backend.Infrastructure.Repository
{
    public class RoutesRepository : IRoutesRepository
    {
        /// <summary>
        /// path to DB can be relative
        /// </summary>
        private string _dbPath;

        /// <summary>
        /// Cache of data loaded from DB
        /// </summary>
        private List<Route> _routes = new();

        public HashSet<string> PlanetsName { get; } = new();

        /// <summary>
        /// Repository of routes loaded from Database
        /// </summary>
        /// <param name="dbPath"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RoutesRepository(string dbPath)
        {
            UpdateDbPath(dbPath);
        }

        public IEnumerable<Route> GetRoutesByOrigin(string origin)
        {
            return _routes.Where(r => r.Origin == origin).OrderBy(r => r.TravelTime);
        }

        public void UpdateDbPath(string dbPath)
        {
            if (string.IsNullOrEmpty(dbPath))
            {
                throw new ArgumentNullException("dbPath");
            }
            _dbPath = dbPath;
        }

        public void LoadCacheFromDatabase()
        {
            PlanetsName.Clear();
            _routes.Clear();

            try
            {
                using var connection = new SqliteConnection($"Data Source={_dbPath}");
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
                        _routes.Add(new Route(origin, destination, travelTime));
                        PlanetsName.Add(origin);
                        PlanetsName.Add(destination);
                    }
                }

            }
            catch (Exception)
            {
                //log
            }
        }

        public IEnumerable<string> GetPlanets() => PlanetsName;
    }
}
