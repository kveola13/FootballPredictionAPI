using AutoFixture;
using FootballPredictionAPI.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics;

namespace FootballPredictionAPI.Controllers.Tests
{
    [TestClass()]
    public class FootballTeamsControllerTests
    {
        private readonly Mock<IFootballRepository>? _repository;
        private readonly Fixture? _fixture;

        public FootballTeamsControllerTests()
        {
            _fixture = new Fixture();
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

        [TestMethod("Test football teams controller")]
        public void FootballTeamsControllerTest()
        {
            Assert.Fail();
        }

        [Obsolete("This is no longer in use")]
        [TestMethod("Test seed football controller")]
        public void SeedFootballTeamTest()
        {
            Assert.Fail();
        }

        [TestMethod("Test get teams controller")]
        public void GetTeamsTest()
        {
            Assert.Fail();
        }

        [TestMethod("Test get football team by id controller")]
        public void GetFootballTeamTest()
        {
            Assert.Fail();
        }

        [TestMethod("Test get football team by name controller")]
        public void GetFootballTeamByNameTest()
        {
            Assert.Fail();
        }

        [TestMethod("Test update football team controller")]
        public void PutFootballTeamTest()
        {
            Assert.Fail();
        }

        [TestMethod("Test post football team controller")]
        public void PostFootballTeamTest()
        {
            Assert.Fail();
        }

        [TestMethod("Test delete football team by id controller")]
        public void DeleteFootballTeamTest()
        {
            Assert.Fail();
        }

        [TestMethod("Test delete football team by name controller")]
        public void DeleteFootballTeamByNameTest()
        {
            Assert.Fail();
        }
    }
}