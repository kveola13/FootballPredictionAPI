

using Newtonsoft.Json;

namespace FootballPredictionAPI.Models;

public class FootballMatch : Match
{
    public double HTPossession { get; set; }
    public double HTTotalshots { get; set; }
    public double HTAccuaracy { get; set; }
    public double HTFouls { get; set; }
    public double HTYellowcards { get; set; }
    public double HTRedcards { get; set; }
    public double HTOffsides { get; set; }
    public double HTCornerstaken { get; set; }
    public double HTGoals { get; set; }

    public double ATPossession { get; set; }
    public double ATTotalshots { get; set; }
    public double ATAccuaracy { get; set; }
    public double ATFouls { get; set; }
    public double ATYellowcards { get; set; }
    public double ATRedcards { get; set; }
    public double ATOffsides { get; set; }
    public double ATCornerstaken { get; set; }
    public double ATGoals { get; set; }

    public FootballMatch() : base()
    {
        
    }
    
    public override string ToString()
    {
        string str = Date.ToString() + " - " + HomeTeam + " " + HTGoals + ":" + ATGoals + " " + AwayTeam + "(" + HTPossession + "-" + ATPossession + ")";
        return str;
    }
}
