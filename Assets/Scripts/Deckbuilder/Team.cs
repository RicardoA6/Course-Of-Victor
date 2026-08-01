using System.Collections.Generic;
using System.Linq;

public class Team
{
    public string TeamName;
    public List<Player> Players = new List<Player>();

    public Team(string teamName, IEnumerable<Player> players)
    {
        TeamName = teamName;
        Players.AddRange(players);
    }

    public bool IsDefeated => Players.All(p => !p.IsAlive);

    public List<Player> GetAlivePlayers()
    {
        return Players.Where(p => p.IsAlive).ToList();
    }
}
