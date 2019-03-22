using AutoMapper;
using CJ.Services.Roles;
using System;
using System.Collections.Generic;
using System.Text;
using CJ.Entities;
using CJ.Services.Users.Dtos;

namespace CJ.Services.AutoMapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<Role, RoleDto>();
            CreateMap<RoleDto, Role>();
        }
    }
}
