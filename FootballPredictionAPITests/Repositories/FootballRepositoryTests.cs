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
            };
        }

        [TestCleanup]
        public void Cleanup()
        {
            Debug.WriteLine("Repository tests terminated..");
        }

        [TestMethod("Test repository")]
        public void FootballRepositoryTest()
        {
            Assert.Fail();
        }

        [TestMethod("Get football teams")]
        public void GetFootballTeamsTest()
        {
            Assert.Fail();
        }

        [TestMethod("Get football team by id")]
        public void GetFootballTeamByIdTest()
        {
            Assert.Fail();
        }

        [TestMethod("Get football team by name")]
        public void GetFootballTeamByNameTest()
        {
            Assert.Fail();
        }

        [TestMethod("Update football team")]
        public void UpdateFootballTeamTest()
        {
            Assert.Fail();
        }

        [TestMethod("Add a football team")]
        public void AddFootballTeamTest()
        {
            Assert.Fail();
        }

        [TestMethod("Delete a football team by id")]
        public void DeleteFootballTeamByIdTest()
        {
            Assert.Fail();
        }

        [TestMethod("Delete a football team by name")]
        public void DeleteFootballTeamByNameTest()
        {
            Assert.Fail();
        }

        [Obsolete("No longer in use")]
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
            var calculatedScore = _repository!.CalculatePoints(footballTeam!);
            Assert.AreEqual(10, calculatedScore);
        }

        [Obsolete("No longer in use")]
        [TestMethod("Check if a football team table exists")]
        public void FootballTeamTableExistsTest()
        {
            Assert.Fail();
        }

        public Task<IEnumerable<FootballTeamDTO?>> GetFootballTeams()
        {
            throw new NotImplementedException();
        }

        public Task<FootballTeamDTO?> GetFootballTeamById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<FootballTeamDTO?> GetFootballTeamByName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<FootballTeam?> UpdateFootballTeam(string id, FootballTeam footballTeamDto)
        {
            throw new NotImplementedException();
        }

        public Task<FootballTeam?> AddFootballTeam(FootballTeamDTO footballTeam)
        {
            throw new NotImplementedException();
        }

        public Task<FootballTeam?> DeleteFootballTeamById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<FootballTeam?> DeleteFootballTeamByName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FootballTeamDTO>> Seed()
        {
            throw new NotImplementedException();
        }

        public int CalculatePoints(FootballTeam footballTeam)
        {
            throw new NotImplementedException();
        }

        public bool FootballTeamTableExists()
        {
            throw new NotImplementedException();
        }
    }
}