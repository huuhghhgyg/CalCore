using System;
using System.Collections.Generic;
using System.Text;

namespace CalCore.LP
{
    public enum IterateState
    {
        Success, //成功
        Unbounded, //无界解
        Infeasible, //无可行解
    }
}
