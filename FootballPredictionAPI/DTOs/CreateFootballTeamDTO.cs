namespace FootballPredictionAPI.DTOs;

public class CreateFootballTeamDTO
{
    public string? Name { get; set; }
    public int MatchesWon { get; set; }
    public int MatchesLost { get; set; }
    public int MatchesDraw { get; set; }
    public string? Description { get; set; }
}