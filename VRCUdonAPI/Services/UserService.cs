using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VRCUdonAPI.Models.Dtos;
using VRCUdonAPI.Models.Entities;
using VRCUdonAPI.Repositories;

namespace VRCUdonAPI.Services
{
    public class UserService : AbstractCrudService<User, UserDto, UserDto, UserDto>, IUserService
    {
        public UserService(VUAContext context, IMapper mapper) : base(context, mapper) { }

        public void Delete(UserDto input)
        {
            User entity = Context.Users.FirstOrDefault(u => u.Id == input.Id);
            Context.Users.Remove(entity);
            Context.SaveChanges();
        }

        public override async Task<UserDto> Create(UserDto input)
        {
            User entity = Mapper.Map<User>(input);
            await Context.AddAsync(entity);
            await Context.SaveChangesAsync();
            return input;
        }

        public override async void Update(UserDto input, params object[] id)
        {
            User entity = await Context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == input.Id);
            entity = Mapper.Map<User>(input);
            Context.Update(entity);
            await Context.SaveChangesAsync();
        }
    }
}
