using CJ.Entities;
using CJ.Services.Users.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Services.Users
{
    /// <summary>
    /// 用户操作接口
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 检验用户是否存在
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        bool CheckUser(UserDto userDto);

        /// <summary>
        /// 获取UserDto
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        UserDto GetUserById(int id);

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        bool Logout();

        /// <summary>
        /// 是否登录
        /// </summary>
        /// <returns></returns>
        bool IsLogin();

        /// <summary>
        /// 重设密码
        /// </summary>
        void ResetPassword();
    }
}
