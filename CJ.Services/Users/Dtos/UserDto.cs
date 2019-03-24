using CJ.Entities;
using CJ.Services.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Services.Users.Dtos
{
    public class UserDto : IEntityDto
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }

        public string RoleId { get; set; }

        public string RoleName { get; set; }
    }
}
