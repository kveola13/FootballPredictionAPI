import "./DisplayTeams.css";
import { teams } from "./Teams";

function DisplayTableRow() {
  const teamsMap = teams.map((teamData) => {
    return (
      <tr>
        <td>{teamData.Name}</td>
        <td>{teamData.GoalsScored}</td>
        <td>{teamData.MatchesWon}</td>
        <td>{teamData.MatchesLost}</td>
        <td>{teamData.MatchesDraw}</td>
      </tr>
    );
  });

  return <tbody>{teamsMap}</tbody>;
}

function DisplayTeams() {
  return (
    <div>
      <table>
        <tr>
          <th>Team</th>
          <th>Score</th>
          <th>Wins</th>
          <th>Loss</th>
          <th>Draw</th>
        </tr>
        <DisplayTableRow team={teams} />
      </table>
    </div>
  );
}

export default DisplayTeams;
