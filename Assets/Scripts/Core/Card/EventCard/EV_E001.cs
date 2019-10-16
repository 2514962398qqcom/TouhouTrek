using System;
using System.Linq;
using System.Threading.Tasks;
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
        public override async Task Use(Game game, ChooseDirectionResponse response)
        {
            await Effects.UseCard.UseEventCard(game, response, this, async (g,r)=>
            {
                if (response.IfForward)
                    game.Size += game.ChainEventDeck.Where(c => c is EV_E001).Count() + 1;
                else
                    game.Size -= game.ChainEventDeck.Where(c => c is EV_E001).Count() + 1;
                game.ChainEventDeck.Add(this);
            });
        }
    }
}
