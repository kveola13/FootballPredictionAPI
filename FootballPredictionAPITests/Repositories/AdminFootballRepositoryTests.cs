using FootballPredictionAPI.DTOs;
using FootballPredictionAPI.Interfaces;
using FootballPredictionAPI.Models;
using FootballPredictionAPI.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics;

namespace FootballPredictionAPITests.Repositories.Tests
{
    [TestClass()]
    public class AdminFootballRepositoryTests
    {
        private readonly Mock<IAdminFootballRepository>? _repository;
        private readonly AdminFootballRepository? _footballRepository;
        private FootballTeam? footballTeam;
        private FootballTeamDTO? footballTeamDTO;

        public AdminFootballRepositoryTests()
        {
            _repository = new Mock<IAdminFootballRepository>();
            _footballRepository = new AdminFootballRepository(null!, null!, null!);
        ***REMOVED***

        [TestInitialize]
        public void Init()
        {
            Debug.WriteLine("Repository tests initialized..");
            footballTeam = new()
            {
                id = "Test-01",
                Name = "Test name",
                Description = "Test 01 team",
                MatchesWon = 3,
                MatchesLost = 2,
                MatchesDraw = 1
            ***REMOVED***;
            footballTeamDTO = new()
            {
                Name = "Test-DTO",
                Description = "Test 02 team",
                MatchesWon = 4,
                MatchesLost = 5,
                MatchesDraw = 6
            ***REMOVED***;
        ***REMOVED***

        [TestCleanup]
        public void Cleanup()
        {
            Debug.WriteLine("Repository tests terminated..");
        ***REMOVED***

        [TestMethod("Get football teams")]
        public void GetFootballTeamsTest()
        {
            var teamList = new List<FootballTeam>() {
                new FootballTeam(),
                new FootballTeam()
            ***REMOVED***;
            _repository!.Setup<Task<IEnumerable<FootballTeam>>>(
                rep => rep.GetFootballTeams()!
            ).Returns(Task.FromResult<IEnumerable<FootballTeam>>(teamList));
            var result = _repository.Object.GetFootballTeams().Result;

            Assert.AreEqual(teamList, result);
            Assert.AreEqual(2, teamList.Count);
        ***REMOVED***

        [TestMethod("Get football team by id")]
        public void GetFootballTeamByIdTest()
        {
            _repository!.Setup<Task<FootballTeam>>(
                rep => rep.GetFootballTeamById("Test-01")!
            ).Returns(Task.FromResult(footballTeam!));

            var result = _repository.Object.GetFootballTeamById("Test-01").Result;

            Assert.AreEqual(footballTeam, result);
        ***REMOVED***

        [TestMethod("Get football team by name")]
        public void GetFootballTeamByNameTest()
        {
            _repository!.Setup<Task<FootballTeam>>(
                rep => rep.GetFootballTeamByName("Test name")!
            ).Returns(Task.FromResult(footballTeam!));
            var result = _repository.Object.GetFootballTeamByName("Test name").Result;

            Assert.AreEqual(footballTeam, result);
        ***REMOVED***

        [TestMethod("Update football team")]
        public void UpdateFootballTeamTest()
        {
            var updatedTeam = new FootballTeam()
            {
                id = "new Test",
                Name = "new test",
                Description = "new test",
                MatchesWon = 2,
                MatchesDraw = 2,
                MatchesLost = 2,
            ***REMOVED***;

            _repository!.Setup<Task<FootballTeam>>(
                rep => rep.UpdateFootballTeam("test", updatedTeam)!
            ).Returns(Task.FromResult(updatedTeam));

            Assert.AreNotEqual(updatedTeam, footballTeam);

            var result = _repository.Object.UpdateFootballTeam("test", updatedTeam).Result;

            Assert.AreEqual(updatedTeam, result);
        ***REMOVED***

        [TestMethod("Add a football team")]
        public void AddFootballTeamTest()
        {
            var teamToAdd = new FootballTeam()
            {
                id = "Add test id",
                Name = "add test",
                Description = "add test",
                MatchesWon = 2,
                MatchesDraw = 2,
                MatchesLost = 2,
            ***REMOVED***;
            var teamToReturn = new FootballTeam()
            {
                id = "Add test id",
                Name = "add test",
                Description = "add test",
                MatchesWon = 2,
                MatchesDraw = 2,
                MatchesLost = 2,
            ***REMOVED***;

            _repository!.Setup<Task<FootballTeam>>(
                rep => rep.AddFootballTeam(teamToAdd)!
            ).Returns(Task.FromResult(teamToReturn));
            var result = _repository.Object.AddFootballTeam(teamToAdd!).Result;

            Assert.AreEqual(result, teamToReturn);
            Assert.AreNotEqual(teamToAdd, teamToReturn);
            Assert.AreNotSame(teamToAdd, teamToReturn);
            Assert.AreSame(result, teamToReturn);
        ***REMOVED***

        [TestMethod("Delete a football team by id")]
        public void DeleteFootballTeamByIdTest()
        {
            Assert.IsNotNull(footballTeam);
            _repository!.Setup<Task<FootballTeam>>(
                rep => rep.DeleteFootballTeamById("Test-01")!
            ).Returns(Task.FromResult(footballTeam!));

            FootballTeam team = null!;

            _repository!.Setup<Task<FootballTeam>>(
                rep => rep.GetFootballTeamById("Test-01")!
            ).Returns(Task.FromResult(team!));

            var result = _repository.Object.DeleteFootballTeamById("Test-01").Result;
            var getTeam = _repository.Object.GetFootballTeamById("Test-01").Result;

            Assert.AreEqual(result, footballTeam);
            Assert.IsNull(getTeam);
        ***REMOVED***

        [TestMethod("Delete a football team by name")]
        public void DeleteFootballTeamByNameTest()
        {
            _repository!.Setup<Task<FootballTeam>>(
                rep => rep.DeleteFootballTeamByName("Test name")!
            ).Returns(Task.FromResult(footballTeam!));
            var result = _repository.Object.DeleteFootballTeamByName("Test name").Result;

            FootballTeam nullValueTeam = null!;

            _repository!.Setup<Task<FootballTeam>>(
                rep => rep.GetFootballTeamByName("Test name")!
            ).Returns(Task.FromResult(nullValueTeam!));

            var getTeam = _repository.Object.GetFootballTeamByName("Test name").Result;

            Assert.AreEqual(result, footballTeam);
            Assert.IsNull(getTeam);
        ***REMOVED***

        [Obsolete("No longer in use")]
        [Ignore("Should not be needed to test")]
        [TestMethod("Seed data test")]
        public void SeedTest()
        {
            Assert.Fail();
        ***REMOVED***

        [TestMethod("Calculate points for a football team")]
        [Priority(9)]
        [TestCategory("Internal logic")]
        public void CalculatePointsTest()
        {
            var calculatedScore = _footballRepository!.CalculatePoints(footballTeam!);
            Assert.AreEqual(10, calculatedScore);
        ***REMOVED***

        [Obsolete("No longer in use")]
        [Ignore("Should not be needed to test")]
        [TestMethod("Check if a football team table exists")]
        public void FootballTeamTableExistsTest()
        {
            Assert.Fail();
        ***REMOVED***
    ***REMOVED***
***REMOVED***
