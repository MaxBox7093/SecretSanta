using AutoMapper;
using SecretSanta.DTOs;
using SecretSanta.Models;

namespace SecretSanta.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<GroupDto, Group>();
            CreateMap<Group, GroupDto>();
            CreateMap<ParticipantDto, Participant>();
        }
    }
}
