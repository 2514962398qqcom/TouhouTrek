using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZMDFQ.PlayerAction;

namespace ZMDFQ
{
    public abstract class ActionCard : Card
    {
        public virtual bool isGroup
        {
            get { return false; }
        }

        /// <summary>
        /// 获取卡片的类型，通常都会直接获取到卡片的类型，但是如果卡片是变形牌之类的卡片，那么会获取到变形后的类型。
        /// </summary>
        /// <returns></returns>
        public virtual Type getType(Game game)
        {
            return getTrueType();
        }

        /// <summary>
        /// 获取卡片真正的类型，即使是变形牌也会获得它本来的类型。
        /// </summary>
        /// <returns></returns>
        public Type getTrueType()
        {
            return GetType();
        }

        public bool CanUse(Game game, Request nowRequest, FreeUse useInfo, out NextRequest nextRequest)
        {
            NextRequest request;
            bool result = canUse(game, nowRequest, useInfo, out request);
            EventData<bool> boolData = new EventData<bool>() { data = result };
            EventData<NextRequest> nextRequestData = new EventData<NextRequest>() { data = request };
            Task task = game.EventSystem.Call(EventEnum.onCheckCanUse, game.ActivePlayerSeat(), this, boolData, nextRequestData);
            if (!task.GetAwaiter().IsCompleted)
            {
                Log.Error($"EventEnum.onCheckCanUse必须同步运行");
                nextRequest = request;
                return result;
            }
            task.GetAwaiter().GetResult();
            nextRequest = nextRequestData.data;
            return boolData.data;
        }

        protected virtual bool canUse(Game game, Request nowRequest, FreeUse useInfo, out NextRequest nextRequest)
        {
            nextRequest = null;
            switch (nowRequest)
            {
                case UseLimitCardRequest useLimitCard:
                    return Effects.UseWayResponse.CheckLimit(game, useLimitCard, useInfo, ref nextRequest, this);
                case FreeUseRequest _:
                    return true;
            }
            return false;
        }

        public abstract Task DoEffect(Game game, FreeUse useWay);

        internal virtual void OnDraw(Game game, Player player)
        {

        }
        internal virtual void OnLeaveHand(Game game, Player player)
        {

        }
    }
}
