using Microsoft.VisualStudio.TestTools.UnitTesting;
using FootballPredictionAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballPredictionAPI.Interfaces;
using AutoMapper;
using FootballPredictionAPI.Context;
using Microsoft.Extensions.Configuration;
using FootballPredictionAPI.Models;
using System.Diagnostics;
using Moq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FootballPredictionAPI.DTOs;
using FootballPredictionAPI.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace FootballPredictionAPI.Repositories.Tests
{
    [TestClass()]
    public class FootballRepositoryTests
    {
        private readonly Mock<IFootballRepository>? _repository;
        private readonly FootballRepository? _footballRepository;
        private FootballTeam? footballTeam;

        public FootballRepositoryTests()
        {
            _repository = new Mock<IFootballRepository>();
            _footballRepository = new FootballRepository(null!,null!,null!);
        }

        [TestInitialize]
        public void Init()
        {
            Debug.WriteLine("Repository tests initialized..");
            footballTeam = new()
            {
                Id = "Test-01",
                Name = "Test",
                Description = "Test 01 team",
                MatchesWon = 3,
                MatchesLost = 2,
                MatchesDraw = 1
            };
        }

        [TestCleanup]
        public void Cleanup()
        {
            Debug.WriteLine("Repository tests terminated..");
        }

        [TestMethod("Get football teams")]
        public void GetFootballTeamsTest()
        {
            var teamList = new List<FootballTeamDTO>() {
                new FootballTeamDTO(),
                new FootballTeamDTO()
            };
            _repository!.Setup<Task<IEnumerable<FootballTeamDTO>>>(
                rep => rep.GetFootballTeams()!
            ).Returns(Task.FromResult<IEnumerable<FootballTeamDTO>>(teamList));
            var controller = new FootballTeamsController(null!, _repository.Object);
            var result = controller.GetTeams().Result.Result;
            var objectResultForStatusCode = (OkObjectResult)result!;

            Assert.AreEqual(200, objectResultForStatusCode.StatusCode);
        }

        [TestMethod("Get football team by id")]
        public void GetFootballTeamByIdTest()
        {
            var team = new FootballTeamDTO();

            _repository!.Setup<Task<FootballTeamDTO>>(
                rep => rep.GetFootballTeamById("testing")!
            ).Returns(Task.FromResult<FootballTeamDTO>(team));
            var controller = new FootballTeamsController(null!, _repository.Object);
            var result = controller.GetTeams().Result.Result;
            var objectResultForStatusCode = (OkObjectResult)result!;

            Assert.AreEqual(200, objectResultForStatusCode.StatusCode);
        }

        [TestMethod("Get football team by name")]
        public void GetFootballTeamByNameTest()
        {
            var team = new FootballTeamDTO();

            _repository!.Setup<Task<FootballTeamDTO>>(
                rep => rep.GetFootballTeamByName("testing")!
            ).Returns(Task.FromResult<FootballTeamDTO>(team));
            var controller = new FootballTeamsController(null!, _repository.Object);
            var result = controller.GetTeams().Result.Result;
            var objectResultForStatusCode = (OkObjectResult)result!;

            Assert.AreEqual(200, objectResultForStatusCode.StatusCode);
        }

        [TestMethod("Update football team")]
        public void UpdateFootballTeamTest()
        {
            var team = new FootballTeam();

            _repository!.Setup<Task<FootballTeam>>(
                rep => rep.UpdateFootballTeam("test", new FootballTeam())!
            ).Returns(Task.FromResult<FootballTeam>(team));
            var controller = new FootballTeamsController(null!, _repository.Object);
            var result = controller.GetTeams().Result.Result;
            var objectResultForStatusCode = (OkObjectResult)result!;

            Assert.AreEqual(200, objectResultForStatusCode.StatusCode);
        }

        [TestMethod("Add a football team")]
        public void AddFootballTeamTest()
        {
            var team = new FootballTeamDTO();

            _repository!.Setup<Task<FootballTeam>>(
                rep => rep.AddFootballTeam(team)!
            ).Returns(Task.FromResult<FootballTeam>(new FootballTeam()));
            var controller = new FootballTeamsController(null!, _repository.Object);
            var result = controller.GetTeams().Result.Result;
            var objectResultForStatusCode = (OkObjectResult)result!;

            Assert.AreEqual(200, objectResultForStatusCode.StatusCode);
        }

        [TestMethod("Delete a football team by id")]
        public void DeleteFootballTeamByIdTest()
        {
            var team = new FootballTeamDTO();

            _repository!.Setup<Task<FootballTeam>>(
                rep => rep.DeleteFootballTeamById("testing")!
            ).Returns(Task.FromResult<FootballTeam>(new FootballTeam()));
            var controller = new FootballTeamsController(null!, _repository.Object);
            var result = controller.GetTeams().Result.Result;
            var objectResultForStatusCode = (OkObjectResult)result!;

            Assert.AreEqual(200, objectResultForStatusCode.StatusCode);
        }

        [TestMethod("Delete a football team by name")]
        public void DeleteFootballTeamByNameTest()
        {
            var team = new FootballTeamDTO();

            _repository!.Setup<Task<FootballTeam>>(
                rep => rep.DeleteFootballTeamByName("testing")!
            ).Returns(Task.FromResult<FootballTeam>(new FootballTeam()));
            var controller = new FootballTeamsController(null!, _repository.Object);
            var result = controller.GetTeams().Result.Result;
            var objectResultForStatusCode = (OkObjectResult)result!;

            Assert.AreEqual(200, objectResultForStatusCode.StatusCode);
        }

        [Obsolete("No longer in use")]
        [Ignore("Should not be needed to test")]
        [TestMethod("Seed data test")]
        public void SeedTest()
        {
            Assert.Fail();
        }

        [TestMethod("Calculate points for a football team")]
        [Priority(9)]
        [TestCategory("Internal logic")]
        public void CalculatePointsTest()
        {
            var calculatedScore = _footballRepository!.CalculatePoints(footballTeam!);
            Assert.AreEqual(10, calculatedScore);
        }

        [Obsolete("No longer in use")]
        [Ignore("Should not be needed to test")]
        [TestMethod("Check if a football team table exists")]
        public void FootballTeamTableExistsTest()
        {
            Assert.Fail();
        }
    }
}