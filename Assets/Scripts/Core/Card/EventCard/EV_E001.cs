using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ZMDFQ.PlayerAction;

namespace ZMDFQ.Cards
{
    /// <summary>
    /// 人气投票，连锁事件，活动事件
    /// 正：社区规模+X，X为连锁事件区中“人气投票”的张数+1
    /// 逆：社区规模-X，X为连锁事件区中“人气投票”的张数+1
    /// </summary>
    public class EV_E001 : EventCard
    {
        public override bool ForwardOnly => false;
        public override async Task DoEffect(Game game, ChooseDirectionResponse response, List<Player> unaffectedPlayers)
        {
            await Effects.UseCard.UseEventCard(game, response, this, async (g, r) =>
            {
                if (response.IfForward)
                    await game.ChangeSize(game.ChainEventDeck.Where(c => c is EV_E001).Count() + 1, this);
                else
                    await game.ChangeSize(-game.ChainEventDeck.Where(c => c is EV_E001).Count() + 1, this);
                game.ChainEventDeck.Add(this);
            });
        }
    }
}
