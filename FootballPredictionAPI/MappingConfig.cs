 
using AutoMapper;
using FootballPredictionAPI.DTOs;
using FootballPredictionAPI.Models;

namespace FootballPredictionAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<FootballTeam, FootballTeamDTO>();
                config.CreateMap<FootballTeamDTO, FootballTeam>();
                config.CreateMap<CreateFootballTeamDTO, FootballTeam>();
                config.CreateMap<FootballTeam, CreateFootballTeamDTO>();
            });
            return mappingConfig;
        }
    }
}
