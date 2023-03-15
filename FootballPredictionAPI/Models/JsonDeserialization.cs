namespace FootballPredictionAPI.Models;

public class JsonDeserialization
{
    public class Results
    {
        public List<Dictionary<string, object>> output1 { get; set; }
    }

    public class Root
    {
        public Results Results { get; set; }
    }
}