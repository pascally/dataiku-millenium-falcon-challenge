
namespace Backend.Application.Config;

public class EmpireConfig
{
    public int Countdown { get; set; }

    public List<BountyHunterConfig> Bounty_Hunters { get; set; }
}

public class BountyHunterConfig
{
    public string Planet { get; set; }

    public int Day { get; set; } 
}
