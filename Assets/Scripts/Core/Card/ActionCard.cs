﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZMDFQ.PlayerAction;

namespace ZMDFQ
{
    public abstract class ActionCard : Card
    {
        /// <summary>
        /// 是否是群体行动牌？默认不是。
        /// </summary>
        public virtual bool isGroup
        {
            get { return false; }
        }
        /// <summary>
        /// 是否是延迟行动牌？默认不是。
        /// </summary>
        public virtual bool isDelay
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
        /// <summary>
        /// 能否使用卡牌，默认没有任何条件，但是会对要求使用特定类型卡片的情况进行检查，所以在重写该方法的时候除了特殊情况应该返回base.canUse
        /// </summary>
        /// <param name="game"></param>
        /// <param name="nowRequest"></param>
        /// <param name="useInfo"></param>
        /// <param name="nextRequest"></param>
        /// <returns></returns>
        protected virtual bool canUse(Game game, Request nowRequest, FreeUse useInfo, out NextRequest nextRequest)
        {
            nextRequest = null;
            switch (nowRequest)
            {
                case UseLimitCardRequest useLimitCard:
                    return Effects.UseWayResponse.CheckLimit(game, useLimitCard, useInfo, ref nextRequest, game.GetCard(useInfo.Source[0]));
                case FreeUseRequest _:
                    return true;
            }
            return false;
        }
        public virtual bool isValidTarget(Game game, FreeUse useWay, Player player, out string invalidInfo)
        {
            invalidInfo = string.Empty;
            return true;
        }
        public virtual bool isValidTarget(Game game, FreeUse useWay, ActionCard card, out string invalidInfo)
        {
            invalidInfo = string.Empty;
            return true;
        }
        public abstract Task DoEffect(Game game, FreeUse useWay);
        internal virtual void OnEnterHand(Game game, Player player)
        {

        }
        internal virtual void OnLeaveHand(Game game, Player player)
        {

        }
        /// <summary>
        /// 当行动牌生效后，默认将其置入弃牌堆。
        /// </summary>
        /// <param name="game"></param>
        public virtual async Task onEffected(Game game)
        {
            await game.AddUsedActionCard(new List<ActionCard>() { this });
        }
    }
}
