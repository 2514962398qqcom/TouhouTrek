using System.Threading.Tasks;
using System.Collections.Generic;

using ZMDFQ.PlayerAction;

namespace ZMDFQ.Cards
{
    /// <summary>
    /// 人类的本质：该牌视作上一张进入弃牌堆的的行动牌的复制。此牌结算完成后不进入弃牌堆，而是交给使用者的下一名玩家，并在下一个回合开始时加入该玩家手牌。
    /// </summary>
    public class AT_N023 : ActionCard
    {
        ActionCard _changeTarget = null;
        public override bool isDelay
        {
            get
            {
                if (_changeTarget != null)
                    return _changeTarget.isDelay;
                return false;
            }
        }
        public override bool isGroup
        {
            get
            {
                if (_changeTarget != null)
                    return _changeTarget.isGroup;
                return false;
            }
        }
        public override int configID
        {
            get
            {
                if (_changeTarget != null)
                    return _changeTarget.configID;
                return base.configID;
            }
        }
        internal override void OnEnterHand(Game game, Player player)
        {
            base.OnEnterHand(game, player);
            if (game.UsedActionDeck.Count > 0)
                _changeTarget = game.UsedActionDeck[game.UsedActionDeck.Count - 1];//变形成已经在弃牌堆的最后一张牌
            game.EventSystem.Register(EventEnum.AfterAddCard, game.GetSeat(player), afterAddCard);
            game.EventSystem.Register(EventEnum.AfterRemoveCard, game.GetSeat(player), afterRemoveCard);
        }
        internal override void OnLeaveHand(Game game, Player player)
        {
            base.OnLeaveHand(game, player);
            game.EventSystem.Remove(EventEnum.AfterAddCard, afterAddCard);
            game.EventSystem.Remove(EventEnum.AfterRemoveCard, afterRemoveCard);
        }
        Task afterAddCard(object[] args)
        {
            Game game = args[0] as Game;
            if (args[1] == null && args[2] == game.UsedActionDeck)//当行动牌进入弃牌堆
            {
                if (game.UsedActionDeck.Count > 0)
                    _changeTarget = game.UsedActionDeck[game.UsedActionDeck.Count - 1];//变形成最后进入弃牌堆的卡牌
            }
            return Task.CompletedTask;
        }
        Task afterRemoveCard(object[] args)
        {
            Game game = args[0] as Game;
            if (args[1] == null && args[2] == game.UsedActionDeck)//当行动牌移出弃牌堆
            {
                if (game.UsedActionDeck.Count > 0)
                    _changeTarget = game.UsedActionDeck[game.UsedActionDeck.Count - 1];//变形成最后进入弃牌堆的卡牌
            }
            return Task.CompletedTask;
        }
        protected override bool canUse(Game game, Request nowRequest, FreeUse useInfo, out NextRequest nextRequest)
        {
            if (_changeTarget == null)
            {
                nextRequest = null;
                return false;
            }
            else
                return _changeTarget.CanUse(game, nowRequest, useInfo, out nextRequest);
        }
        public override bool isValidTarget(Game game, FreeUse useWay, ActionCard card, out string invalidInfo)
        {
            if (_changeTarget != null)
                return _changeTarget.isValidTarget(game, useWay, card, out invalidInfo);
            return base.isValidTarget(game, useWay, card, out invalidInfo);
        }
        public override bool isValidTarget(Game game, FreeUse useWay, Player player, out string invalidInfo)
        {
            if (_changeTarget != null)
                return _changeTarget.isValidTarget(game, useWay, player, out invalidInfo);
            return base.isValidTarget(game, useWay, player, out invalidInfo);
        }
        public override async Task DoEffect(Game game, FreeUse useWay)
        {
            if (_changeTarget != null)
            {
                setProp("user", game.GetPlayer(useWay.PlayerId));
                await _changeTarget.DoEffect(game, useWay);
                Log.Debug("复读机复读" + _changeTarget);
            }
        }
        public override async Task onEffected(Game game)
        {
            if (game.EventSystem.currentEvent == EventEnum.TurnStart)
            {
                //如果现在就在回合开始阶段，直接置入玩家手牌
                await game.getNextPlayer(getProp<Player>("user")).AddActionCards(game, new List<ActionCard>() { this });
            }
            else
            {
                //不在回合开始阶段，注册回合开始回调
                setProp("onTurnStart", new CardCallback(this, onTurnStart));
                game.EventSystem.Register(EventEnum.TurnStart, game.GetSeat(Owner), getProp<CardCallback>("onTurnStart").call, 100);
            }
            Log.Debug("复读机结算完毕");
        }
        static async Task onTurnStart(Card thisCard, object[] args)
        {
            Game game = args[0] as Game;
            Player player = game.getNextPlayer(thisCard.getProp<Player>("user"));
            await player.AddActionCards(game, new List<ActionCard>() { thisCard as ActionCard });
            Log.Debug("复读机传递给玩家" + player.Id);
            game.EventSystem.Remove(EventEnum.TurnStart, thisCard.getProp<CardCallback>("onTurnStart").call);
        }
    }
}
