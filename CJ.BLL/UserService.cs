using CJ.Infrastructure;
using CJ.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CJ.Services
{
    public static class UserService
    {
        private static DefaultDbContext context = new DefaultDbContext();

        public static bool CheckUser(string username, string password)
        {
            try
            {
                User user = context.Users.Where(c => c.UserName == username && c.Password == password).FirstOrDefault();

                SessionHelper.SetSession("user", password);

                return user == null ? false : true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static User GetUserById(int id)
        {
            try
            {
                User user = context.Users.Where(c => c.Id == id).FirstOrDefault();
                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool Logout()
        {
            try
            {
                SessionHelper.RemoveSession("user");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool IsLogin()
        {
            var user = SessionHelper.GetSession("user");
            return String.IsNullOrEmpty(user) || user == "" ? false: true;
        }
    }
}
