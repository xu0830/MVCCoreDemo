using CJ.Models;
using CJ.Services.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Services
{
    public interface IUserService : ITransientDependency
    {
        bool CheckUser(string username, string password, string token = "");
        UserDto GetUserById(int id);
        bool Logout();
        bool IsLogin();
        void ResetPassword();
    }
}
