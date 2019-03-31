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
                    string token = Guid.NewGuid().ToString();
                    CacheHelper.SetCache(token, user, DateTime.Now.AddDays(7));
                    return new UserOutput
                    {
                        Msg = "登录成功",
                        Flag = true,
                        Token = token
                    };
                }
            }
            catch (Exception)
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
            userRepository.Insert(new User {
                UserName = "ddddd",
                Password = "xucj"
            }); 
            return true;
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
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 判断登录状态
        /// </summary>
        /// <returns></returns>
        public int IsLogin(string token)
        {
            var user = CacheHelper.GetCache<User>(token);
            return user == null ? 0 : user.Id;
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        public void ResetPassword()
        {

        }
    }
}
