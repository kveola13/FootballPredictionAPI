

using Newtonsoft.Json;

namespace FootballPredictionAPI.Models;

public class FootballMatch
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; ***REMOVED***
    public int Week { get; set; ***REMOVED***
    public DateTime Date { get; set; ***REMOVED***
    public String HomeTeam { get; set; ***REMOVED***
    public String Score { get; set; ***REMOVED***
    public String AwayTeam { get; set; ***REMOVED***
    public int Attendence { get; set; ***REMOVED***
    public int HTShotsOnTarget { get; set; ***REMOVED***
    public int ATShotsOnTarget { get; set; ***REMOVED***
    
    public int HTShotsTotal { get; set; ***REMOVED***
    
    public int ATShotsTotal { get; set; ***REMOVED***
    
    public int HTPossesion { get; set; ***REMOVED***
    
    public int ATPossesion { get; set; ***REMOVED***
    
    public int HTPassesTotal { get; set; ***REMOVED***
    public int ATPassesTotal { get; set; ***REMOVED***
    public int HTPassingAccuracy { get; set; ***REMOVED***
    public int ATPassingAccuracy { get; set; ***REMOVED***
    public string HTResult { get; set; ***REMOVED***

***REMOVED***