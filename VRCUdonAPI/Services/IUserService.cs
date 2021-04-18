using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VRCUdonAPI.Models.Dtos;
using VRCUdonAPI.Models.Entities;

namespace VRCUdonAPI.Services
{
    public interface IUserService : ICrudService<User, UserDto, UserDto, UserDto>
    {
        void Delete(UserDto input);
    }
}
