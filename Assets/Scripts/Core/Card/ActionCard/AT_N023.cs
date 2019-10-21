using System;
using System.Threading.Tasks;
using ZMDFQ.PlayerAction;

namespace ZMDFQ.Cards
{
    /// <summary>
    /// 人类的本质：该牌视作上一张进入弃牌堆的的行动牌的复制。此牌结算完成后不进入弃牌堆，而是交给使用者的下一名玩家，并在下一个回合开始时加入该玩家手牌。
    /// </summary>
    public class AT_N023 : ActionCard
    {
        public override Type getType(Game game)
        {
            if (game.UsedActionDeck.Count > 0)
                return game.UsedActionDeck[game.UsedActionDeck.Count - 1].getTrueType();//在弃牌堆里应该不能视作变形牌了
            return base.getType(game);
        }
        protected override bool canUse(Game game, Request nowRequest, FreeUse useInfo, out NextRequest nextRequest)
        {
            if (game.UsedActionDeck.Count < 1)
            {
                nextRequest = null;
                return false;//不能使用，因为没有可供复读的对象。
            }
            return game.UsedActionDeck[game.UsedActionDeck.Count - 1].CanUse(game, nowRequest, useInfo, out nextRequest);//复读上一张卡的条件
        }
        public override Task DoEffect(Game game, FreeUse useWay)
        {
            return game.UsedActionDeck[game.UsedActionDeck.Count - 1].DoEffect(game, useWay);//复读上一张卡的效果
        }
    }
}
