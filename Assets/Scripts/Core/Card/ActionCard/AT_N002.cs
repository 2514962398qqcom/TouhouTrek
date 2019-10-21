using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZMDFQ.PlayerAction;

namespace ZMDFQ.Cards
{
    /// <summary>
    /// 创作
    /// 个人影响力+1
    /// </summary>
    public class AT_N002 : ActionCard
    {
        public override Task DoEffect(Game game, FreeUse useWay)
        {
            game.GetPlayer(useWay.PlayerId).Size += 1;
            return Task.CompletedTask;
        }
    }
}
