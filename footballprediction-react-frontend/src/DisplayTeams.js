import "./DisplayTeams.css";
import { teams } from "./Teams";
import React, { useState } from "react";

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

function PaginateTableRow(props) {
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage] = useState(10);

  const indexOfLastItem = currentPage * itemsPerPage;
  const indexOfFirstItem = indexOfLastItem - itemsPerPage;

  const teams = props.team.slice(indexOfFirstItem, indexOfLastItem);

  const teamsMap = teams.map((teamData) => {
    return (
      <tr key={teamData.Name}>
        <td>{teamData.Name}</td>
        <td>{teamData.GoalsScored}</td>
        <td>{teamData.MatchesWon}</td>
        <td>{teamData.MatchesLost}</td>
        <td>{teamData.MatchesDraw}</td>
      </tr>
    );
  });

  const totalPages = Math.ceil(props.team.length / itemsPerPage);

  const pageNumbers = [];
  for (let i = 1; i <= totalPages; i++) {
    pageNumbers.push(i);
  }

  return (
    <div>
      <table>
        <thead>
          <tr>
            <th>Team</th>
            <th>Score</th>
            <th>Wins</th>
            <th>Loss</th>
            <th>Draw</th>
          </tr>
        </thead>
        <tbody>{teamsMap}</tbody>
      </table>
      <div>
        {pageNumbers.map((number) => (
          <button
            key={number}
            onClick={() => setCurrentPage(number)}
            disabled={number === currentPage}
          >
            {number}
          </button>
        ))}
      </div>
    </div>
  );
}

function DisplayTeams() {
  return (
    <div>
      <PaginateTableRow team={teams} />
    </div>
  );
}

export default DisplayTeams;
