using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Entities
{
    /// <summary>
    /// 数据库表Role实体类
    /// </summary>
    public class Role : IEntity
    {
        public virtual int Id { get; set; }

        public string RoleName { get; set; }
    }
}
