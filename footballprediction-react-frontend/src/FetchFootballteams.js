import { useState, useEffect } from "react";

function FetchTeams() {
  const [team, setTeam] = useState([]);
  const fetchedData = async () => {
    const response = await fetch(
      "https://footballpredictionapi-test.azurewebsites.net/api/FootballTeams"
    );
    const data = await response.json();
    return setTeam(data);
  };
  useEffect(() => {
    fetchedData();
  }, []);
  team.sort((a, b) => {
    if (a.points < b.points) {
      return 1;
    }
    if (a.points > b.points) {
      return -1;
    }
    return 0;
  });
}

export default FetchTeams;
