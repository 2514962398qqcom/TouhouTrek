using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZMDFQ.PlayerAction;

namespace ZMDFQ.Cards
{
    /// <summary>
    /// 约稿
    /// 社群规模±1，选择一名玩家并给x张牌，你与其个人影响力+x
    /// </summary>
    public class AT_N004 : ActionCard
    {
        protected override bool canUse(Game game, Request nowRequest, FreeUse useInfo, out NextRequest nextRequest)
        {
            if (nowRequest is FreeUseRequest && useInfo.PlayersId.Count < 1)
            {
                nextRequest = new HeroChooseRequest() { PlayerId = useInfo.PlayerId, Number = 1, RequestInfo = "选择一名玩家" };
                return false;
            }
            return base.canUse(game, nowRequest, useInfo, out nextRequest);
        }

        public override async Task DoEffect(Game game, FreeUse useWay)
        {
            Player player = game.GetPlayer(useWay.PlayerId);
            Player targetPlayer = game.GetPlayer(useWay.PlayersId[0]);
            ActionCard source = game.GetCard(useWay.Source[0]) as ActionCard;
            TakeChoiceResponse takeChoice = await game.WaitAnswer(new TakeChoiceRequest()
            {
                Infos = new List<string>() { "+1", "-1" },
                PlayerId = useWay.PlayerId,
                RequsetInfo = "社群规模±1"
            }.SetTimeOut(game.RequestTime)) as TakeChoiceResponse;
            if (takeChoice.Index == 0)
                await game.ChangeSize(1, source);
            else
                await game.ChangeSize(-1, source);

            ChooseSomeCardResponse chooseCard = await game.WaitAnswer(new ChooseSomeCardRequest()
            {
                Count = player.ActionCards.Count,
                EnoughOnly = false,
                PlayerId = useWay.PlayerId,
                RequsetInfo = "选择X张牌给予目标玩家，你与其影响力+X"
            }.SetTimeOut(game.RequestTime)) as ChooseSomeCardResponse;
            List<ActionCard> cards = new List<ActionCard>(chooseCard.Cards.Select(id => player.ActionCards.Find(c => c.Id == id)));
            await player.DropActionCard(game, chooseCard.Cards);
            await player.AddActionCards(game, cards);
            await player.ChangeSize(game, cards.Count, source, player);
            await targetPlayer.ChangeSize(game, cards.Count, source, player);
        }
    }
}
