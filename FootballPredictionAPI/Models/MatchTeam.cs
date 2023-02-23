using Newtonsoft.Json;

namespace FootballPredictionAPI.Models;

public class MatchTeam
{
    public string? Id { get; set; ***REMOVED***
    public DateTime? Date { get; set; ***REMOVED***
    public string? Team { get; set; ***REMOVED***
    public string? Opponent { get; set; ***REMOVED***
    public double Possession { get; set; ***REMOVED***
    public double Totalshots { get; set; ***REMOVED***
    public double Accuaracy { get; set; ***REMOVED***
    public double Fouls { get; set; ***REMOVED***
    public double Yellowcards { get; set; ***REMOVED***
    public double Redcards { get; set; ***REMOVED***
    public double Offsides { get; set; ***REMOVED***
    public double Cornerstaken { get; set; ***REMOVED***
    public double Goals { get; set; ***REMOVED***
    
    public double OpponentGoals { get; set; ***REMOVED***
    
    public override string ToString()
    {
        string str = Date.ToString() + " - " + Team + " " + Goals + ":" + OpponentGoals + " " + Opponent 
                     + "(" + Possession + "-" + Accuaracy + ", " + Cornerstaken + "-" + Totalshots + ")";
        return str;
    ***REMOVED***
***REMOVED***