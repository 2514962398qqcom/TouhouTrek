using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZMDFQ.PlayerAction;

namespace ZMDFQ.Cards
{
    /// <summary>
    /// 造谣：你的个人影响力-1，指定一名玩家的个人影响力-2
    /// </summary>
    public class AT_N008 : ActionCard
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

        public override async Task DoEffect(Game game, FreeUse useWay)
        {
            ActionCard source = game.GetCard(useWay.Source[0]) as ActionCard;
            Player player = game.GetPlayer(useWay.PlayerId);
            Player target = game.GetPlayer(useWay.PlayersId[0]);
            await player.ChangeSize(game, -1, source, player);
            await target.ChangeSize(game, -2, source, player);
        }
    }
}

