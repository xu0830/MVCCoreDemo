using CJ.Entities;
using CJ.Services.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Services.Users
{
    public interface IUserService
    {
        bool CheckUser(UserDto userDto);
        UserDto GetUserById(int id);
        bool Logout();
        bool IsLogin();
        void ResetPassword();
    }
}
