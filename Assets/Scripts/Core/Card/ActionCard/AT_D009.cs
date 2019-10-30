using System.Collections;

using System.Threading.Tasks;
using ZMDFQ.PlayerAction;

namespace ZMDFQ.Cards
{
    /// <summary>
    /// 煽风点火：下一张生效的事件牌效果中的社群规模和个人影响力偏移量+1。
    /// </summary>
    public class AT_D009 : ActionCard
    {
        public override bool isDelay => true;
        public override Task DoEffect(Game game, FreeUse useWay)
        {
            ActionCard source = game.GetCard(useWay.Source[0]) as ActionCard;
            source.setProp("beforeEventCardEffect", new CardCallback(source, beforeEventCardEffect));
            game.EventSystem.Register(EventEnum.BeforeEventCardEffect, -1, source.getProp<CardCallback>("beforeEventCardEffect").call);//注册事件
            return Task.CompletedTask;
        }
        static Task beforeEventCardEffect(Card thisCard, object[] args)
        {
            Game game = args[0] as Game;
            thisCard.setProp("targetCard", args[1] as EventCard);//设置生效卡片
            thisCard.setProp("beforeGameSizeChange", new CardCallback(thisCard, beforeGameSizeChange));
            game.EventSystem.Register(EventEnum.BeforeGameSizeChange, -1, new CardCallback(thisCard, beforeGameSizeChange).call);
            thisCard.setProp("beforePlayerSizeChange", new CardCallback(thisCard, beforePlayerSizeChange));
            game.EventSystem.Register(EventEnum.BeforePlayrSizeChange, -1, new CardCallback(thisCard, beforePlayerSizeChange).call);
            thisCard.setProp("afterEventCardEffect", new CardCallback(thisCard, afterEventCardEffect));
            game.EventSystem.Register(EventEnum.AfterEventCardEffect, -1, new CardCallback(thisCard, afterEventCardEffect).call);//注册事件
            game.EventSystem.Remove(EventEnum.BeforeEventCardEffect, thisCard.getProp<CardCallback>("beforeEventCardEffect").call);
            return Task.CompletedTask;
        }
        static Task beforeGameSizeChange(Card thisCard, object[] args)
        {
            EventData<int> value = args[0] as EventData<int>;
            if (thisCard.getProp<EventCard>("targetCard") != null && args[1] == thisCard.getProp<EventCard>("targetCard"))
            {
                if (value.data > 0)
                    value.data += 1;
                else
                    value.data -= 1;//偏移量+1
            }
            return Task.CompletedTask;
        }
        static Task beforePlayerSizeChange(Card thisCard, object[] args)
        {
            EventData<int> value = args[2] as EventData<int>;
            if (thisCard.getProp<EventCard>("targetCard") != null && args[3] == thisCard.getProp<EventCard>("targetCard"))
            {
                if (value.data > 0)
                    value.data += 1;
                else
                    value.data -= 1;//偏移量+1
            }
            return Task.CompletedTask;
        }
        static async Task afterEventCardEffect(Card thisCard, object[] args)
        {
            Game game = args[0] as Game;
            EventCard eventCard = args[1] as EventCard;//生效卡片
            if (eventCard == thisCard.getProp<EventCard>("targetCard"))
            {
                thisCard.setProp("targetCard", null);
                game.EventSystem.Remove(EventEnum.BeforeGameSizeChange, thisCard.getProp<CardCallback>("beforeGameSizeChange").call);
                game.EventSystem.Remove(EventEnum.BeforePlayrSizeChange, thisCard.getProp<CardCallback>("beforePlayerSizeChange").call);
                game.EventSystem.Remove(EventEnum.AfterEventCardEffect, thisCard.getProp<CardCallback>("beforeEventCardEffect").call);//注销事件
                game.DelayActionDeck.Remove(thisCard as ActionCard);
                await (thisCard as ActionCard).onEffected(game);
            }
        }
    }
}
