using System;
using System.Linq;
using System.Collections.Generic;

using System.Threading.Tasks;
using ZMDFQ.PlayerAction;

namespace ZMDFQ.Cards
{
    /// <summary>
    /// 众筹：你成为众筹者，其他玩家可以弃置一张手牌成为众筹者。根据众筹者人数X，依次产生如下效果：
    /// 一人：每位众筹者+1影响力；
    /// 二人：社群规模±X；
    /// 三人及以上：每位众筹者抽一张行动牌；
    /// 若全部玩家成为众筹者，则众筹被叫停，不产生效果。
    /// </summary>
    public class AT_G013 : ActionCard
    {
        public override bool isGroup => true;
        public override async Task DoEffect(Game game, FreeUse useWay)
        {
            ActionCard source = game.GetCard(useWay.Source[0]) as ActionCard;
            List<Player> crowdfunders = new List<Player>() { game.GetPlayer(useWay.PlayerId) };
            Task<Response>[] tasks = await game.waitAnswerAll(new List<Player>(game.Players.Where(p => p.Id != useWay.PlayerId)), p => game.WaitAnswer(new ChooseSomeCardRequest()
            {
                AllPlayerRequest = true,
                Count = 1,
                EnoughOnly = false,
                PlayerId = p.Id,
                RequsetInfo = "是否丢弃手牌成为众筹者？"
            }.SetTimeOut(game.RequestTime)));
            foreach (var task in tasks)
            {
                ChooseSomeCardResponse response = task.Result as ChooseSomeCardResponse;
                if (response.Cards.Count > 0)
                    crowdfunders.Add(game.GetPlayer(response.PlayerId));
            }
            if (crowdfunders.Count == game.Players.Count)
                return;
            if (crowdfunders.Count > 0)
            {
                foreach (Player player in crowdfunders)
                {
                    await player.ChangeSize(game, 1, source, game.GetPlayer(useWay.PlayerId));
                }
            }
            if (crowdfunders.Count > 1)
            {
                TakeChoiceResponse response = await game.WaitAnswer(new TakeChoiceRequest()
                {
                    PlayerId = useWay.PlayerId,
                    Infos = new List<string>() { "+" + crowdfunders.Count, "-" + crowdfunders.Count }
                }.SetTimeOut(game.RequestTime)) as TakeChoiceResponse;
                if (response.Index == 0)
                    await game.ChangeSize(crowdfunders.Count, source);
                else
                    await game.ChangeSize(-crowdfunders.Count, source);
            }
            if (crowdfunders.Count > 2)
            {
                foreach (Player player in crowdfunders)
                {
                    await player.DrawActionCard(game, 1);
                }
            }
        }
    }
}
