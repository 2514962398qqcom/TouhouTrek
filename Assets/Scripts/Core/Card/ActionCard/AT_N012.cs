using System.Collections.Generic;
using System.Threading.Tasks;
using ZMDFQ.PlayerAction;

namespace ZMDFQ.Cards
{
    /// <summary>
    /// 出警：指定一名玩家并进行一次两点点数判定，判定结果为X，该玩家影响力-X，社群规模±X。
    /// </summary>
    public class AT_N012 : ActionCard
    {
        protected override bool canUse(Game game, Request nowRequest, FreeUse useInfo, out NextRequest nextRequest)
        {
            if (useInfo.PlayersId.Count < 1)
            {
                nextRequest = new HeroChooseRequest();
                return false;
            }
            return base.canUse(game, nowRequest, useInfo, out nextRequest);
        }
        public override async Task DoEffect(Game game, FreeUse useWay)
        {
            ActionCard source = game.GetCard(useWay.Source[0]) as ActionCard;
            int x = game.twoPointCheck();
            await game.GetPlayer(useWay.PlayersId[0]).ChangeSize(game, -x, source, game.GetPlayer(useWay.PlayerId));
            TakeChoiceResponse response = await game.WaitAnswer(new TakeChoiceRequest() { PlayerId = useWay.PlayerId, Infos = new List<string>() { "+2", "-2" } }) as TakeChoiceResponse;
            if (response.Index == 0)
                await game.ChangeSize(2, source);
            else
                await game.ChangeSize(-2, source);
        }
    }
}
