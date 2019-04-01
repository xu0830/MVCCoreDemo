using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CJ.Services.Quartz.Enum
{
    public enum JobStatus
    {
        [Description]
        已启用,

        [Description]
        待运行,

        [Description]
        执行中,

        [Description]
        执行完成,

        [Description]
        执行任务计划中,

        [Description]
        已停止
    }
}
