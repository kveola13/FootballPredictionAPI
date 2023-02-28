using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace FootballPredictionAPI.Models;

public class Match
{
    [JsonProperty(PropertyName = "id")]
    public string? id { get; set; ***REMOVED***
    public DateTime? Date { get; set; ***REMOVED***
    public string? HomeTeam { get; set; ***REMOVED***
    public string? AwayTeam { get; set; ***REMOVED***
    public string? StatsUrl { get; set; ***REMOVED***
    
    private const string dateRegex = @"[0-9]{2***REMOVED***.[0-9]{2***REMOVED***.[0-9]{4***REMOVED***";
    private Regex Pattern;
    public void ReadValues(List<string> args) 
    {
        Pattern = new Regex(dateRegex);
        HomeTeam = args[2];
        AwayTeam = args[4];
        Date = DateTime.Parse(Pattern.Match(args[0]).ToString());
        StatsUrl = args[7];
    ***REMOVED***
    
    public override string ToString()
    {
        string str = Date.ToString() + " - " + HomeTeam + " vs. " + AwayTeam + "(" + StatsUrl + ")";
        return str;
    ***REMOVED***
***REMOVED***