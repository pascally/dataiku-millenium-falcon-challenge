using Backend.Domain.Models;

namespace Backend.Application.Interfaces.Repository;

public interface IRoutesRepository
{
    IEnumerable<string> GetPlanets();
    IEnumerable<Route> GetRoutesByOrigin(string origin);
    void UpdateDbPath(string dbPath);
    void LoadCacheFromDatabase();
}
