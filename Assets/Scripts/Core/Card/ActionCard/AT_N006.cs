using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZMDFQ.PlayerAction;

namespace ZMDFQ.Cards
{
    /// <summary>
    /// 互撕：指定玩家个人影响力-1，从该玩家开始，轮流弃一张牌使对方个人影响力-1
    /// </summary>
    public class AT_N006 : ActionCard
    {
        protected override bool canUse(Game game, Request nowRequest, FreeUse useInfo, out NextRequest nextRequest)
        {
            nextRequest = null;
            switch (nowRequest)
            {
                case UseLimitCardRequest useLimitCard:
                    return Effects.UseWayResponse.CheckLimit(game, useLimitCard, useInfo, ref nextRequest, this);
                case FreeUseRequest freeUse:
                    if (useInfo.PlayersId.Count < 1)
                    {
                        nextRequest = new HeroChooseRequest() { };
                        return false;
                    }
                    else
                    {
                        return true;
                    }
            }
            return false;
        }

        public override Task DoEffect(Game game, FreeUse useWay)
        {
            return Effects.UseCard.UseActionCard(game, useWay, this, effect);
        }
        private async Task effect(Game game, FreeUse useWay)
        {
            Player target = game.GetPlayer(useWay.PlayersId[0]);
            Player user = game.GetPlayer(useWay.PlayerId);
            await target.ChangeSize(game, -1, this, Owner);
            Player now = target;
            while (true)
            {
                ChooseSomeCardResponse chooseSomeCardResponse =
                    (ChooseSomeCardResponse)await game.WaitAnswer(new ChooseSomeCardRequest()
                    { Count = 1, PlayerId = now.Id, TimeOut = game.RequestTime, EnoughOnly = false });
                if (chooseSomeCardResponse.Cards.Count == 0)
                {
                    break;
                }
                else
                {
                    Player nowTarget = now == user ? target : user;
                    await nowTarget.ChangeSize(game, -1, this, Owner);
                    now = nowTarget;
                }
            }
        }
    }
}
