using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Entities
{
    /// <summary>
    /// 车站实体类
    /// </summary>
    public class Station
    {
        /// <summary>
        /// 索引
        /// </summary>
        public string Index { get; set; }

        /// <summary>
        /// 拼音首字母
        /// </summary>
        public string CNAbbr { get; set; }

        /// <summary>
        /// 中文名称
        /// </summary>
        public string CNName { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 中文拼音
        /// </summary>
        public string CNPhoneticAlpha { get; set; }
    }
}
