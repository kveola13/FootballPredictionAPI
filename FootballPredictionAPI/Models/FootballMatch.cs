

using Newtonsoft.Json;

namespace FootballPredictionAPI.Models;

public class FootballMatch
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    public int Week { get; set; }
    public DateTime Date { get; set; }
    public String HomeTeam { get; set; }
    public String Score { get; set; }
    public String AwayTeam { get; set; }
    public int Attendence { get; set; }
    public int HTShotsOnTarget { get; set; }
    public int ATShotsOnTarget { get; set; }
    
    public int HTShotsTotal { get; set; }
    
    public int ATShotsTotal { get; set; }
    
    public int HTPossesion { get; set; }
    
    public int ATPossesion { get; set; }
    
    public int HTPassesTotal { get; set; }
    public int ATPassesTotal { get; set; }
    public int HTPassingAccuracy { get; set; }
    public int ATPassingAccuracy { get; set; }
    public string HTResult { get; set; }
    /*public int? HTPoints { get; set; }
    public int? HTGoalsScored { get; set; }
    public int? HTGoalsAgainst { get; set; }
    public int? HTGoalDifference { get; set; }
    public int? HTMatchesWon { get; set; }
    public int? HTMatchesLost { get; set; }
    public int? HTMatchesDraw { get; set; }
    public int? ATPoints { get; set; }
    public int? ATGoalsScored { get; set; }
    public int? ATGoalsAgainst { get; set; }
    
    public int? ATGoalDifference { get; set; }
    
    public int? ATMatchesWon { get; set; }
    
    public int? ATMatchesLost { get; set; }
    
    public int? ATMatchesDraw { get; set; }*/
    
    
}