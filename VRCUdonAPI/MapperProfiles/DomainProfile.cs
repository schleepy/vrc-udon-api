using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VRCUdonAPI.Models.Dtos;
using VRCUdonAPI.Models.Entities;

namespace VRCUdonAPI.MapperProfiles
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<Query, QueryDto>();
            CreateMap<QueryDto, Query>();
            CreateMap<Error, ErrorDto>();
            CreateMap<ErrorDto, Error>();
        }
    }
}
