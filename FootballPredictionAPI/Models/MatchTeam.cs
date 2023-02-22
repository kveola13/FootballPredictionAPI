using Newtonsoft.Json;

namespace FootballPredictionAPI.Models;

public class MatchTeam
{
    public string? Id { get; set; }
    public DateTime? Date { get; set; }
    public string? Team { get; set; }
    public string? Opponent { get; set; }
    public double Possession { get; set; }
    public double Totalshots { get; set; }
    public double Accuaracy { get; set; }
    public double Fouls { get; set; }
    public double Yellowcards { get; set; }
    public double Redcards { get; set; }
    public double Offsides { get; set; }
    public double Cornerstaken { get; set; }
    public double Goals { get; set; }
    
    public double OpponentGoals { get; set; }
    
    public override string ToString()
    {
        string str = Date.ToString() + " - " + Team + " " + Goals + ":" + OpponentGoals + " " + Opponent 
                     + "(" + Possession + "-" + Accuaracy + ", " + Cornerstaken + "-" + Totalshots + ")";
        return str;
    }
}