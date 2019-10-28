using System.Threading.Tasks;
using ZMDFQ.PlayerAction;

namespace ZMDFQ.Cards
{
    /// <summary>
    /// 盈利：抽两张行动牌。
    /// </summary>
    public class AT_N003 : ActionCard
    {
        public override async Task DoEffect(Game game, FreeUse useWay)
        {
            await game.GetPlayer(useWay.PlayerId).DrawActionCard(game, 2);
        }
    }
}
