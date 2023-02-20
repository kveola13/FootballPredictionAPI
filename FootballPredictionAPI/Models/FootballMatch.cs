

using Newtonsoft.Json;

namespace FootballPredictionAPI.Models;

public class FootballMatch : Match
{
    public double HTPossession { get; set; ***REMOVED***
    public double HTTotalshots { get; set; ***REMOVED***
    public double HTAccuaracy { get; set; ***REMOVED***
    public double HTFouls { get; set; ***REMOVED***
    public double HTYellowcards { get; set; ***REMOVED***
    public double HTRedcards { get; set; ***REMOVED***
    public double HTOffsides { get; set; ***REMOVED***
    public double HTCornerstaken { get; set; ***REMOVED***
    public double HTGoals { get; set; ***REMOVED***

    public double ATPossession { get; set; ***REMOVED***
    public double ATTotalshots { get; set; ***REMOVED***
    public double ATAccuaracy { get; set; ***REMOVED***
    public double ATFouls { get; set; ***REMOVED***
    public double ATYellowcards { get; set; ***REMOVED***
    public double ATRedcards { get; set; ***REMOVED***
    public double ATOffsides { get; set; ***REMOVED***
    public double ATCornerstaken { get; set; ***REMOVED***
    public double ATGoals { get; set; ***REMOVED***

    public FootballMatch() : base()
    {
        
    ***REMOVED***
    
    public override string ToString()
    {
        string str = Date.ToString() + " - " + HomeTeam + " " + HTGoals + ":" + ATGoals + " " + AwayTeam 
                     + "(" + HTPossession + "-" + ATPossession + ", " + HTCornerstaken + "-" + ATCornerstaken + ")";
        return str;
    ***REMOVED***
***REMOVED***
