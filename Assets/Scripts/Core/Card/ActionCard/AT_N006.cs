using System;
using System.Text;
using System.Threading.Tasks;
using ZMDFQ.PlayerAction;

namespace ZMDFQ.Cards
{
    /// <summary>
    /// 互撕：指定玩家个人影响力-1，从该玩家开始，轮流弃一张牌使对方个人影响力-1。
    /// </summary>
    public class AT_N006 : ActionCard
    {
        protected override bool canUse(Game game, Request nowRequest, FreeUse useInfo, out NextRequest nextRequest)
        {
            if (nowRequest is FreeUseRequest && useInfo.PlayersId.Count < 1)
            {
                nextRequest = new HeroChooseRequest() { PlayerId = useInfo.PlayerId, Number = 1, RequestInfo = "选择目标玩家" };
                return false;
            }
            return base.canUse(game, nowRequest, useInfo, out nextRequest);
        }
        /// <summary>
        /// 不能自己撕自己
        /// </summary>
        /// <param name="game"></param>
        /// <param name="useWay"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public override bool isValidTarget(Game game, FreeUse useWay, Player player, out string invalidInfo)
        {
            if (player.Id != useWay.PlayerId)
            {
                invalidInfo = string.Empty;
                return true;
            }
            else
            {
                invalidInfo = "目标不能是自己";
                return false;
            }
        }
        public override async Task DoEffect(Game game, FreeUse useWay)
        {
            ActionCard source = game.GetCard(useWay.Source[0]) as ActionCard;
            Player player = game.GetPlayer(useWay.PlayerId);
            Player target = game.GetPlayer(useWay.PlayersId[0]);
            await target.ChangeSize(game, -1, source, player);
            Player now = target;
            while (true)
            {
                ChooseSomeCardResponse chooseSomeCardResponse =
                    (ChooseSomeCardResponse)await game.WaitAnswer(new ChooseSomeCardRequest()
                    { Count = 1, PlayerId = now.Id, TimeOut = game.RequestTime, EnoughOnly = false });
                if (chooseSomeCardResponse.Cards.Count == 0)
                    break;
                else
                {
                    Player nowTarget = now == player ? target : player;
                    await now.discard(game, chooseSomeCardResponse.Cards);
                    await nowTarget.ChangeSize(game, -1, source, now);
                    now = nowTarget;
                }
            }
        }
    }
}
