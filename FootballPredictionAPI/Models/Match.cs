namespace FootballPredictionAPI.Models;

public class Match
{
    public DateTime Date { get; set; }
    public string HomeTeam { get; set; }
    public string AwayTeam { get; set; }
    public string StatsUrl { get; set; }
}