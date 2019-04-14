using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Entities
{
    /// <summary>
    /// 数据库表Users实体类
    /// </summary>
    public class User : IEntity
    {
        public virtual int Id { get; set; }

        public string UserName { get; set; }

        public string NickName { get; set; }

        public string Password { get; set; }

        public string Avatar { get; set; }
    }
}
