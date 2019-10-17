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

        protected abstract bool canUse(Game game, Request nowRequest, FreeUse useInfo, out NextRequest nextRequest);

        public abstract Task DoEffect(Game game, FreeUse useWay);

        internal virtual void OnDraw(Game game, Player player)
        {

        }
        internal virtual void OnLeaveHand(Game game,Player player)
        {

        }
    }
}
