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

        /// <summary>
        /// 用户登录接口
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool CheckUser(string username, string password)
        {
            try
            {
                User user = context.Users.Where(c => c.UserName == username && c.Password == RSAHelper.Decrypt(RSAHelper.GetRSAKey().PrivateKey, password)).FirstOrDefault();

                SessionHelper.SetSession("user", password);

                return user == null ? false : true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        
        /// <summary>
        /// 根据Id获取用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 用户登出
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 判断登录状态
        /// </summary>
        /// <returns></returns>
        public static bool IsLogin()
        {
            var user = SessionHelper.GetSession("user");
            return String.IsNullOrEmpty(user) || user == "" ? false: true;
        }
    }
}
