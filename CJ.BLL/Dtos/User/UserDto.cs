using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Services.Dtos
{
    public class UserDto : IEntityDto
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }
    }
}
