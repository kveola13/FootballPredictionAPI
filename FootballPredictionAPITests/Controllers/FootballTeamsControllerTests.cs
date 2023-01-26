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

        [TestMethod("Test football teams controller")]
        public void FootballTeamsControllerTest()
        {
            Assert.Fail();
        ***REMOVED***

        [Obsolete("This is no longer in use")]
        [TestMethod("Test seed football controller")]
        public void SeedFootballTeamTest()
        {
            Assert.Fail();
        ***REMOVED***

        [TestMethod("Test get teams controller")]
        public void GetTeamsTest()
        {
            Assert.Fail();
        ***REMOVED***

        [TestMethod("Test get football team by id controller")]
        public void GetFootballTeamTest()
        {
            Assert.Fail();
        ***REMOVED***

        [TestMethod("Test get football team by name controller")]
        public void GetFootballTeamByNameTest()
        {
            Assert.Fail();
        ***REMOVED***

        [TestMethod("Test update football team controller")]
        public void PutFootballTeamTest()
        {
            Assert.Fail();
        ***REMOVED***

        [TestMethod("Test post football team controller")]
        public void PostFootballTeamTest()
        {
            Assert.Fail();
        ***REMOVED***

        [TestMethod("Test delete football team by id controller")]
        public void DeleteFootballTeamTest()
        {
            Assert.Fail();
        ***REMOVED***

        [TestMethod("Test delete football team by name controller")]
        public void DeleteFootballTeamByNameTest()
        {
            Assert.Fail();
        ***REMOVED***
    ***REMOVED***
***REMOVED***