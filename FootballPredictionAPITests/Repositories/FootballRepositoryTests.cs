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

namespace FootballPredictionAPI.Repositories.Tests
{
    [TestClass()]
    public class FootballRepositoryTests : IFootballRepository
    {
        private FootballTeam? footballTeam;
        private FootballRepository? _repository;

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
            ***REMOVED***;
        ***REMOVED***

        [TestCleanup]
        public void Cleanup()
        {
            Debug.WriteLine("Repository tests terminated..");
        ***REMOVED***

        [TestMethod("Test repository")]
        public void FootballRepositoryTest()
        {
            Assert.Fail();
        ***REMOVED***

        [TestMethod("Get football teams")]
        public void GetFootballTeamsTest()
        {
            Assert.Fail();
        ***REMOVED***

        [TestMethod("Get football team by id")]
        public void GetFootballTeamByIdTest()
        {
            Assert.Fail();
        ***REMOVED***

        [TestMethod("Get football team by name")]
        public void GetFootballTeamByNameTest()
        {
            Assert.Fail();
        ***REMOVED***

        [TestMethod("Update football team")]
        public void UpdateFootballTeamTest()
        {
            Assert.Fail();
        ***REMOVED***

        [TestMethod("Add a football team")]
        public void AddFootballTeamTest()
        {
            Assert.Fail();
        ***REMOVED***

        [TestMethod("Delete a football team by id")]
        public void DeleteFootballTeamByIdTest()
        {
            Assert.Fail();
        ***REMOVED***

        [TestMethod("Delete a football team by name")]
        public void DeleteFootballTeamByNameTest()
        {
            Assert.Fail();
        ***REMOVED***

        [Obsolete("No longer in use")]
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
            var calculatedScore = _repository!.CalculatePoints(footballTeam!);
            Assert.AreEqual(10, calculatedScore);
        ***REMOVED***

        [Obsolete("No longer in use")]
        [TestMethod("Check if a football team table exists")]
        public void FootballTeamTableExistsTest()
        {
            Assert.Fail();
        ***REMOVED***

        public Task<IEnumerable<FootballTeamDTO?>> GetFootballTeams()
        {
            throw new NotImplementedException();
        ***REMOVED***

        public Task<FootballTeamDTO?> GetFootballTeamById(string id)
        {
            throw new NotImplementedException();
        ***REMOVED***

        public Task<FootballTeamDTO?> GetFootballTeamByName(string name)
        {
            throw new NotImplementedException();
        ***REMOVED***

        public Task<FootballTeam?> UpdateFootballTeam(string id, FootballTeam footballTeamDto)
        {
            throw new NotImplementedException();
        ***REMOVED***

        public Task<FootballTeam?> AddFootballTeam(FootballTeamDTO footballTeam)
        {
            throw new NotImplementedException();
        ***REMOVED***

        public Task<FootballTeam?> DeleteFootballTeamById(string id)
        {
            throw new NotImplementedException();
        ***REMOVED***

        public Task<FootballTeam?> DeleteFootballTeamByName(string name)
        {
            throw new NotImplementedException();
        ***REMOVED***

        public Task<IEnumerable<FootballTeamDTO>> Seed()
        {
            throw new NotImplementedException();
        ***REMOVED***

        public int CalculatePoints(FootballTeam footballTeam)
        {
            throw new NotImplementedException();
        ***REMOVED***

        public bool FootballTeamTableExists()
        {
            throw new NotImplementedException();
        ***REMOVED***
    ***REMOVED***
***REMOVED***