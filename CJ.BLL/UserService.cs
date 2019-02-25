using CJ.Infrastructure;
using CJ.Models;
using CJ.Infrastructure.Repositories;
using System;
using System.Linq;
using CJ.Services.Dtos;
using AutoMapper;

namespace CJ.Services
{
    public class UserService : IUserService
    {
        private static DefaultDbContext context = new DefaultDbContext();

        private IRepository<User> _userRepository;

        private IMapper _mapper;

        public UserService(IRepository<User> userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// 用户登录接口
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CheckUser(UserDto userDto)
        {
            if (userDto.token != "")
            {
                return token.CompareTo(SessionHelper.GetSession("token")) == 0;
            }
            else
            {
                try
                {
                    User user = _userRepository.GetAll().Where(c => c.UserName == username && c.Password == MD5Encrypt.Getmd5(RSAHelper.Decrypt(WebConfig.PrivateKey, password))).FirstOrDefault();
                    token = Guid.NewGuid().ToString();
                    SessionHelper.SetSession("user", password);
                    SessionHelper.SetSession("token", token);

                    return user == null ? false : true;
                }
                catch (Exception ex)
                {
                    return false;
                }

            }
        }
        
        /// <summary>
        /// 根据Id获取用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserDto GetUserById(int id)
        {
            try
            {
                User user = _userRepository.Get(id);
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
        public bool Logout()
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
        public bool IsLogin()
        {
            var user = SessionHelper.GetSession("user");
            return String.IsNullOrEmpty(user) || user == "" ? false: true;
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        public void ResetPassword()
        {

        }

     
    }
}
