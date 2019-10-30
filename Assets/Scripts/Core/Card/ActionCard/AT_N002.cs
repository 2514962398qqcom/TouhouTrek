using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZMDFQ.PlayerAction;

namespace ZMDFQ.Cards
{
    /// <summary>
    /// 创作：个人影响力+1。
    /// </summary>
    public class AT_N002 : ActionCard
    {
        public override async Task DoEffect(Game game, FreeUse useWay)
        {
            ActionCard source = game.GetCard(useWay.Source[0]) as ActionCard;
            Player player = game.GetPlayer(useWay.PlayerId);
            await player.ChangeSize(game, 1, source, player);
        }
    }
}
