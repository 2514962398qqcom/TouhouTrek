using System.Collections;

using System.Threading.Tasks;
using ZMDFQ.PlayerAction;

namespace ZMDFQ.Cards
{
    /// <summary>
    /// 煽风点火：下一张生效的事件牌效果中的社群规模和个人影响力偏移量+1
    /// </summary>
    public class AT_D009 : ActionCard
    {
        EventCard targetCard { get; set; } = null;
        public override Task DoEffect(Game game, FreeUse useWay)
        {
            g.DelayActionDeck.Add(this);//置入连锁区
            game.EventSystem.Register(EventEnum.BeforeEventCardEffect, -1, beforeEventCardEffect);//注册事件
            return Task.CompletedTask;
        }
        Task beforeEventCardEffect(object[] args)
        {
            Game game = args[0] as Game;
            targetCard = args[1] as EventCard;//设置生效卡片
            game.EventSystem.Register(EventEnum.BeforeGameSizeChange, -1, beforeGameSizeChange);
            game.EventSystem.Register(EventEnum.BeforePlayrSizeChange, -1, beforePlayerSizeChange);
            game.EventSystem.Register(EventEnum.AfterEventCardEffect, -1, afterEventCardEffect);//注册事件
            game.EventSystem.Remove(EventEnum.BeforeEventCardEffect, beforeEventCardEffect);
            return Task.CompletedTask;
        }
        Task beforeGameSizeChange(object[] args)
        {
            EventData<int> value = args[0] as EventData<int>;
            if (targetCard != null && args[1] == targetCard)
            {
                if (value.data > 0)
                    value.data += 1;
                else
                    value.data -= 1;//偏移量+1
            }
            return Task.CompletedTask;
        }
        Task beforePlayerSizeChange(object[] args)
        {
            EventData<int> value = args[2] as EventData<int>;
            if (targetCard != null && args[3] == targetCard)
            {
                if (value.data > 0)
                    value.data += 1;
                else
                    value.data -= 1;//偏移量+1
            }
            return Task.CompletedTask;
        }
        Task afterEventCardEffect(object[] args)
        {
            Game game = args[0] as Game;
            EventCard eventCard = args[1] as EventCard;//生效卡片
            if (eventCard == targetCard)
            {
                targetCard = null;
                game.DelayActionDeck.Remove(this);
                game.UsedActionDeck.Add(this);//进入弃牌区
                game.EventSystem.Remove(EventEnum.BeforeGameSizeChange, beforeGameSizeChange);
                game.EventSystem.Remove(EventEnum.BeforePlayrSizeChange, beforePlayerSizeChange);
                game.EventSystem.Remove(EventEnum.AfterEventCardEffect, afterEventCardEffect);//注销事件
            }
            return Task.CompletedTask;
        }
    }
}
