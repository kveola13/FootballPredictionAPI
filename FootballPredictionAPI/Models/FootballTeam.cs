namespace FootballPredictionAPI.Models
{
    public class FootballTeam
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Points { get; set; }
        public int MatchesWon { get; set; }
        public int MatchesLost { get; set; }
        public int MatchesDraw { get; set; }
        public string? Description { get; set; }
    }
}
