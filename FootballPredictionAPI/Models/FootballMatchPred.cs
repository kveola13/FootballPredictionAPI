namespace FootballPredictionAPI.Models;

public class FootballMatchPred
{
    public int Id { get; set; }
    public string? HomeTeam { get; set; }
    public string? AwayTeam { get; set; }
    public double HTShotsOnTarget { get; set; }
    public double ATShotsOnTarget { get; set; }
    
    public double HTShotsTotal { get; set; }
    
    public double ATShotsTotal { get; set; }
    
    public double HTPossesion { get; set; }
    
    public double ATPossesion { get; set; }
    
    public double HTPassesTotal { get; set; }
    public double ATPassesTotal { get; set; }
    public double HTPassingAccuracy { get; set; }
    public double ATPassingAccuracy { get; set; }
    public string? HTResult { get; set; }
    public int? HTPoints { get; set; }
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
    
    public int? ATMatchesDraw { get; set; }


}