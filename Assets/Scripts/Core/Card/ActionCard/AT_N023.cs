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
        public override int configID
        {
            get
            {
                if (_changeTarget != null)
                    return _changeTarget.configID;
                return base.configID;
            }
        }
        internal override void OnDraw(Game game, Player player)
        {
            base.OnDraw(game, player);
            if (game.UsedActionDeck.Count > 0)
                _changeTarget = game.UsedActionDeck[game.UsedActionDeck.Count - 1];//变形成已经在弃牌堆的最后一张牌
            game.EventSystem.Register(EventEnum.AfterAddCard, game.GetSeat(player), afterAddCard);
            game.EventSystem.Register(EventEnum.AfterRemoveCard, game.GetSeat(player), afterRemoveCard);
        }
        internal override void OnLeaveHand(Game game, Player player)
        {
            base.OnLeaveHand(game, player);
            _changeTarget = null;//变回你原来的样子！
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
        public override async Task DoEffect(Game game, FreeUse useWay)
        {
            if (_changeTarget != null)
                await _changeTarget.DoEffect(game, useWay);
        }
        public override void onEffected(Game game)
        {
            game.EventSystem.Register(EventEnum.TurnStart, game.GetSeat(Owner), onTurnStart);
            //TODO:支持变形成延迟牌
        }
        async Task onTurnStart(object[] args)
        {
            Game game = args[1] as Game;
            Player player = game.Players[(int)args[0]];
            await player.AddActionCards(game, new List<ActionCard>() { this });
        }
    }
}
