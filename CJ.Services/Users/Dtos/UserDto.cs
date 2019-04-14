using CJ.Entities;
using CJ.Services.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Services.Users.Dtos
{
    public class UserDto : IEntityDto
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string NickName { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }

        public string RoleId { get; set; }

        public string RoleName { get; set; }

        public string Avatar { get; set; }
    }
}
