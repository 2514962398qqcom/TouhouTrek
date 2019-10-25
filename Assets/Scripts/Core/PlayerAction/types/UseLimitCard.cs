using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMDFQ.PlayerAction
{
    /// <summary>
    /// 让玩家打出某一种的卡,只能一张，这一张可以由其他方式转化过来
    /// </summary>
    public class UseLimitCardRequest : Request
    {
        public int CardType;
    }

    public class UseLimitCardResponse : FreeUse
    {
        public bool Used;
    }
}
