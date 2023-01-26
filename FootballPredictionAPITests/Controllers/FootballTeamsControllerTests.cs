using AutoFixture;
using FootballPredictionAPI.DTOs;
using FootballPredictionAPI.Interfaces;
using FootballPredictionAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuGet.Protocol;
using System.Diagnostics;

namespace FootballPredictionAPI.Controllers.Tests
{
    [TestClass()]
    public class FootballTeamsControllerTests
    {
        private readonly Mock<FootballTeamsController>? _controller;
        private readonly Mock<IFootballRepository>? _repository;

        public FootballTeamsControllerTests()
        {
            _controller = new Mock<FootballTeamsController>();
            _repository = new Mock<IFootballRepository>();
        }

        [TestInitialize]
        public void Init()
        {
            Debug.WriteLine("Controller tests started...");
        }

        [TestCleanup]
        public void Cleanup()
        {
            Debug.WriteLine("Controller tests terminated...");
        }

        [Obsolete("This is no longer in use")]
        [Ignore("This should be ignored in the testing")]
        [TestMethod("Test seed football controller")]
        public void SeedFootballTeamTest()
        {
            Assert.Fail();
        }

        [TestMethod("Test get teams controller")]
        public void GetTeamsTest()
        {
            var teamList = new List<FootballTeamDTO>() {
                new FootballTeamDTO(),
                new FootballTeamDTO()
            };
            Assert.IsNotNull(teamList);
        }

        [TestMethod("Test get football team by id controller")]
        public void GetFootballTeamTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod("Test get football team by name controller")]
        public void GetFootballTeamByNameTest()
        {
            throw new NotImplementedException()
        }

        [TestMethod("Test update football team controller")]
        public void PutFootballTeamTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod("Test post football team controller")]
        public void PostFootballTeamTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod("Test delete football team by id controller")]
        public void DeleteFootballTeamTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod("Test delete football team by name controller")]
        public void DeleteFootballTeamByNameTest()
        {
            throw new NotImplementedException();
        }
    }
}