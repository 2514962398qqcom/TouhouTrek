using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ZMDFQ.PlayerAction;

namespace ZMDFQ.Cards
{
    /// <summary>
    /// 火星：弃掉你手中的事件牌，从事件牌弃牌堆中选择选择一张事件牌放入手中，这张事件牌本回合必须打出且效果对其他玩家无效。
    /// </summary>
    public class AT_N014 : ActionCard
    {
        protected override bool canUse(Game game, Request nowRequest, FreeUse useInfo, out NextRequest nextRequest)
        {
            if (game.UsedEventDeck.Count < 1)
            {
                nextRequest = null;
                return false;
            }
            return base.canUse(game, nowRequest, useInfo, out nextRequest);
        }
        public override async Task DoEffect(Game game, FreeUse useWay)
        {
            Player player = game.GetPlayer(useWay.PlayerId);
            await player.DropEventCard(game, player.EventCards[0], true);
            DiscoverResponse respond = await game.WaitAnswer(new DiscoverRequest()
            {
                Count = 1,
                EnoughOnly = true,
                PlayerId = useWay.PlayerId,
                RequsetInfo = "从事件弃牌堆中选择一张事件牌放入手中",
                SelectableCards = new List<int>(game.UsedEventDeck.Select(c => c.Id))
            }.SetTimeOut(game.RequestTime)) as DiscoverResponse;
            EventCard eventCard = game.UsedEventDeck.Find(c => c.Id == respond.SelectedCards[0]);
            game.UsedEventDeck.Remove(eventCard);
            player.EventCards.Add(eventCard);
            player.avoidSetEvent = true;
            eventCard.UnaffectedPlayers = new List<Player>(game.Players.Where(p => p != player));
        }
    }
}
