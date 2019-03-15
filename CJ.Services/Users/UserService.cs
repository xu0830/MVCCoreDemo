using CJ.Infrastructure;
using CJ.Infrastructure.Repositories;
using System;
using System.Linq;
using CJ.Services.Dtos;
using AutoMapper;
using CJ.Entities;

namespace CJ.Services.Users
{
    /// <summary>
    /// 用户操作实现类
    /// </summary>
    public class UserService : IUserService
    {
        private IRepository<User> _userRepository;

        private IMapper _mapper;

        public UserService(IRepository<User> userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        /// <summary>
        /// 用户校验
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CheckUser(UserDto userDto)
        {
            try
            {
                //User user = _userRepository.GetAll().Where(c => c.UserName == userDto.UserName && c.Password == MD5Encrypt.Getmd5(RSAHelper.Decrypt(WebConfig.PrivateKey, userDto.Password))).FirstOrDefault();

                User user = _userRepository.GetAll().Where(c => c.UserName == userDto.UserName && c.Password == userDto.Password).FirstOrDefault();

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
        public UserDto GetUserById(int id)
        {
            try
            {
                User user = _userRepository.Get(id);
                UserDto userDto = _mapper.Map<UserDto>(user);
                return userDto;
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
