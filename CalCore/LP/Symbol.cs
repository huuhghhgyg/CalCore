using System;
using System.Collections.Generic;
using System.Text;

namespace CalCore.LP
{
    public enum Symbol //约束函数符号
    {
        LE, // <=
        EQ, // ==
        GE, // >=
    }

    public enum Target //目标函数求解目标
    {
        max, //求最大（默认）
        min  //求最小
    }
}
