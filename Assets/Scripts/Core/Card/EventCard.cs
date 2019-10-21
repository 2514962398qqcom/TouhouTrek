using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMDFQ
{
    public abstract class EventCard : Card
    {
        public abstract bool ForwardOnly { get; }
        public List<Player> UnaffectedPlayers { get; set; } = new List<Player>();
        public async Task Use(Game game, PlayerAction.ChooseDirectionResponse response)
        {
            await DoEffect(game, response, UnaffectedPlayers);
            UnaffectedPlayers = new List<Player>();
        }
        public abstract Task DoEffect(Game game, PlayerAction.ChooseDirectionResponse response, List<Player> unaffectedPlayers);
    }
}
