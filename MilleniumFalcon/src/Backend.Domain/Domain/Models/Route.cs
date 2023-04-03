namespace Backend.Domain.Models;

/// <summary>
/// Information related to a route between two planets
/// </summary>
public class Route
{
    public string Origin { get; }
    public string Destination { get; }
    public int TravelTime { get; }

    /// <summary>
    /// Route constructor
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="destination"></param>
    /// <param name="travelTime"></param>
    /// <exception cref="ArgumentNullException">if origin/desitination are null or empty</exception>
    /// <exception cref="ArgumentException">if origin / destination are equal, or traveltime <= 0</exception>
    public Route(string origin, string destination, int travelTime) 
    {
        if (string.IsNullOrEmpty(origin))
        {
            throw new ArgumentNullException(nameof(origin));
        }

        if (string.IsNullOrEmpty(destination))
        {
            throw new ArgumentNullException(nameof(destination));
        }

        if (origin == destination)
        {
            throw new ArgumentException("origin and destination are the same");
        }

        if (travelTime <= 0)
        {
            throw new ArgumentException(nameof(travelTime));
        }

        Origin = origin;
        Destination = destination;
        TravelTime = travelTime;
    }
}
