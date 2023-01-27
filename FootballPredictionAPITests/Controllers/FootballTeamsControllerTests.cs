using FootballPredictionAPI.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
        ***REMOVED***

        [TestInitialize]
        public void Init()
        {
            Debug.WriteLine("Controller tests started...");
        ***REMOVED***

        [TestCleanup]
        public void Cleanup()
        {
            Debug.WriteLine("Controller tests terminated...");
        ***REMOVED***

        [Obsolete("This is no longer in use")]
        [Ignore("This should be ignored in the testing")]
        [TestMethod("Test seed football controller")]
        public void SeedFootballTeamTest()
        {
            Assert.Fail();
        ***REMOVED***

        [TestMethod("Test get teams controller")]
        public void GetTeamsTest()
        {
            throw new NotImplementedException();
        ***REMOVED***

        [TestMethod("Test get football team by id controller")]
        public void GetFootballTeamTest()
        {
            throw new NotImplementedException();
        ***REMOVED***

        [TestMethod("Test get football team by name controller")]
        public void GetFootballTeamByNameTest()
        {
            throw new NotImplementedException();
        ***REMOVED***

        [TestMethod("Test update football team controller")]
        public void PutFootballTeamTest()
        {
            throw new NotImplementedException();
        ***REMOVED***

        [TestMethod("Test post football team controller")]
        public void PostFootballTeamTest()
        {
            throw new NotImplementedException();
        ***REMOVED***

        [TestMethod("Test delete football team by id controller")]
        public void DeleteFootballTeamTest()
        {
            throw new NotImplementedException();
        ***REMOVED***

        [TestMethod("Test delete football team by name controller")]
        public void DeleteFootballTeamByNameTest()
        {
            throw new NotImplementedException();
        ***REMOVED***
    ***REMOVED***
***REMOVED***