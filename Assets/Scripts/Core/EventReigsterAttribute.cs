using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMDFQ
{
    /// <summary>
    /// 在类的静态方法上使用，参数为game，在游戏一开始注册对应事件
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    internal class EventReigsterAttribute:Attribute
    {

    }
}
