using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace FootballPredictionAPI.Models;

public class Match
{
    [JsonProperty(PropertyName = "id")]
    public string? Id { get; set; }
    public DateTime? Date { get; set; }
    public string? HomeTeam { get; set; }
    public string? AwayTeam { get; set; }
    public string? StatsUrl { get; set; }
    
    private const string dateRegex = @"[0-9]{2}.[0-9]{2}.[0-9]{4}";
    private Regex Pattern;
    public void ReadValues(List<string> args) 
    {
        Pattern = new Regex(dateRegex);
        HomeTeam = args[2];
        AwayTeam = args[4];
        Date = DateTime.Parse(Pattern.Match(args[0]).ToString());
        StatsUrl = args[7];
    }
    
    public override string ToString()
    {
        string str = Date.ToString() + " - " + HomeTeam + " vs. " + AwayTeam + "(" + StatsUrl + ")";
        return str;
    }
}