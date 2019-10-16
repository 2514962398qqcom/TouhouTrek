using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMDFQ.PlayerAction
{
    /// <summary>
    /// 选择事件卡的出牌方向
    /// </summary>
    public class ChooseDirectionRequest : Request
    {
        public bool CanSet = true;
        //public EventCard EventCard;
    }
    public class ChooseDirectionResponse : Response
    {
        public int CardId;
        public bool IfForward = true;
        public bool IfSet = false;
    }
}
