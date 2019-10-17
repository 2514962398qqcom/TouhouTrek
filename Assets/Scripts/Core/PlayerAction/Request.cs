using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMDFQ.PlayerAction
{
    /// <summary>
    /// 向一名玩家请求完成一个操作,玩家需要返回对应的Response
    /// </summary>
    public class Request
    {
        /// <summary>
        /// 剩余时间
        /// </summary>
        public float RemainTime = 5;
        /// <summary>
        /// 超时时间
        /// </summary>
        public float TimeOut = 5;
        /// <summary>
        /// 询问的目标玩家
        /// </summary>
        public int PlayerId = -1;

        /// <summary>
        /// 是否是向所有玩家进行的询问，true时表示玩家拼手速出牌（意思是不知道被询问的玩家是谁）
        /// </summary>
        public bool AllPlayerRequest = false;

        /// <summary>
        /// 询问的提示信息
        /// </summary>
        public string RequsetInfo;

        public Request SetTimeOut(float time)
        {
            RemainTime = time;
            TimeOut = time;
            return this;
        }
    }
}
