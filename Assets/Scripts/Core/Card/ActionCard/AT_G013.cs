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
        protected override bool canUse(Game game, Request nowRequest, FreeUse useInfo, out NextRequest nextRequest)
        {
            nextRequest = null;
            return true;
        }
        public override async Task DoEffect(Game game, FreeUse useWay)
        {
            List<Player> crowdfunders = new List<Player>() { game.GetPlayer(useWay.PlayerId) };
            Task<Response>[] tasks = game.Players.Where(p => p.Id != useWay.PlayerId).Select(p => game.WaitAnswer(new ChooseSomeCardRequest()
            {
                AllPlayerRequest = true,
                Count = 1,
                EnoughOnly = false,
                PlayerId = p.Id,
                RequsetInfo = "是否丢弃手牌成为众筹者？"
            }.SetTimeOut(game.RequestTime))).ToArray();
            await Task.WhenAll(tasks);
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
                    await player.ChangeSize(game, 1, this, game.GetPlayer(useWay.PlayerId));
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
                    await game.ChangeSize(crowdfunders.Count, this);
                else
                    await game.ChangeSize(-crowdfunders.Count, this);
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
