using CJ.Infrastructure;
using CJ.Infrastructure.Repositories;
using System;
using System.Linq;
using AutoMapper;
using CJ.Entities;
using CJ.Services.Users.Dtos;
using CJ.Infrastructure.Cache;

namespace CJ.Services.Users
{
    /// <summary>
    /// 用户操作实现类
    /// </summary>
    public class UserService : IUserService
    {
        private IRepository<User> userRepository;

        private IMapper mapper;

        public UserService(IRepository<User> _userRepository, IMapper _mapper)
        {
            mapper = _mapper;
            userRepository = _userRepository;
        }

        /// <summary>
        /// 用户校验
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UserOutput Login(UserDto userDto)
        {
            try
            {
                User user = userRepository.GetAll()
                    .Where(c => c.UserName == userDto.UserName)
                    .FirstOrDefault();
                if (user == null)
                {
                    return new UserOutput
                    {
                        Msg = "用户不存在",
                        Flag = false
                    };
                }

                if (user.Password != userDto.Password)
                {
                    return new UserOutput
                    {
                        Msg = "密码错误",
                        Flag = false
                    };
                }
                else
                {
                    UserDto model = mapper.Map<UserDto>(user);
                    string token = Guid.NewGuid().ToString();
                    CacheHelper.SetCache(token, model, new TimeSpan(7, 0, 0, 0));
                    return new UserOutput
                    {
                        Msg = "登录成功",
                        Flag = true,
                        Token = token
                    };
                }
            }
            catch (Exception ex)
            {
                return new UserOutput {
                    Msg = "登录异常",
                    Flag = false
                };
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
                User user = userRepository.Get(id);
                UserDto userDto = mapper.Map<UserDto>(user);
                return userDto;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool AddUser()
        {
            User user = userRepository.Get(3);
            userRepository.Delete(user); 
            return true;
        }

        /// <summary>
        /// 用户登出
        /// </summary>
        /// <returns></returns>
        public bool Logout(string token)
        {
            
            var flag = CacheHelper.Remove(token);
            return flag;
           
        }

        /// <summary>
        /// 判断登录状态
        /// </summary>
        /// <returns></returns>
        public UserDto IsLogin(string token)
        {
            var user = CacheHelper.GetCache<UserDto>(token);
            return user;
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        public void ResetPassword()
        {

        }
    }
}
