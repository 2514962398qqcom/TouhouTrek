using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMDFQ.Effects
{
    public class UseCard
    {
        /// <summary>
        /// 正常的结算流程：结算牌放结算区，结算完毕后进入弃牌堆
        /// </summary>
        /// <param name="game"></param>
        /// <param name="useEventCard"></param>
        /// <param name="card"></param>
        /// <param name="effect"></param>
        //public static async Task NormalUse(Game game, PlayerAction.FreeUse useOneCard, Card card, Func<Game, PlayerAction.FreeUse, Task> effect)
        //{
        //    if (card is EventCard eventCard)
        //        await UseEventCard(game, useOneCard, eventCard, effect);
        //    else if (card is ActionCard actionCard)
        //        await UseActionCard(game, useOneCard, actionCard, effect);
        //}

        public static async Task UseEventCard<T>(Game game, T useEventCard, EventCard card, Func<Game, T, Task> effect) where T : PlayerAction.ChooseDirectionResponse
        {
            Player player = game.GetPlayer(useEventCard.PlayerId);
            await player.DropEventCard(game, card);
            game.AddUsingCard(card);
            game.AddUsingInfo(new UsingInfo()
            {
                Card = card,
                Info = $"{player.Hero.Name}使用了{card.Name}",
            });
            await game.EventSystem.Call(EventEnum.changeEventDirection, game.ActivePlayerSeat(), game, useEventCard);
            await game.EventSystem.Call(EventEnum.BeforeEventCardEffect, game.ActivePlayerSeat(), game, card);
            await effect(game, useEventCard);
            await game.EventSystem.Call(EventEnum.AfterEventCardEffect, game.ActivePlayerSeat(), game, card);
            game.RemoveUsingCard(card);
            if (!game.ChainEventDeck.Contains(card))
                game.UsedEventDeck.Add(card);
        }
        public static async Task UseActionCard<T>(Game game, T useActionCard, ActionCard card, Func<Game, T, Task> effect) where T : PlayerAction.FreeUse
        {
            Player player = game.GetPlayer(useActionCard.PlayerId);
            await player.DropActionCard(game, useActionCard.Source);
            //由于行动牌可能是由多张牌转换过来的，要根据source处理
            UsingInfo usingInfo = new UsingInfo()
            {
                Card = card,
                Info = $"{player.Hero.Name}使用了{card.Name}",
            };
            //结算前把牌放入结算区
            foreach (var cardId in useActionCard.Source)
            {
                var c = game.GetCard(cardId);
                game.AddUsingCard(c);
                usingInfo.Source.Add(c);
            }
            game.AddUsingInfo(usingInfo);
            if (card.isDelay)
                game.DelayActionDeck.Add(card);//延迟牌置入延迟区
            await effect(game, useActionCard);
            foreach (var cardId in useActionCard.Source)
            {
                var c = game.GetCard(cardId) as ActionCard;
                game.RemoveUsingCard(c);
                if (!card.isDelay)//延迟牌不会结算完毕
                    c.onEffected(game);
                c.Owner = null;
            }
        }
        [Obsolete("时请使用UseActionCard和UseEventCard作为替代")]
        public static async Task NormalUse<T>(Game game, T useOneCard, Card card, Func<Game, T, Task> effect) where T : PlayerAction.FreeUse
        {
            Player player = game.GetPlayer(useOneCard.PlayerId);
            if (card is ActionCard actionCard)
                await player.DropActionCard(game, useOneCard.Source);
            else if (card is EventCard eventCard)
                await player.DropEventCard(game, eventCard);
            //结算前把牌放入结算区
            UsingInfo usingInfo = new UsingInfo()
            {
                Card = card,
                Info = $"{player.Hero.Name}使用了{card.Name}",
            };
            foreach (var cardId in useOneCard.Source)
            {
                var c = game.GetCard(cardId);
                game.AddUsingCard(c);
                usingInfo.Source.Add(c);
            }
            game.AddUsingInfo(usingInfo);
            await effect(game, useOneCard);
            game.RemoveUsingCard(card);
            //结算完毕进入弃牌堆
            if (card is ActionCard actionCard1)
                game.UsedActionDeck.Add(actionCard1);
            else if (card is EventCard eventCard1 && !game.ChainEventDeck.Contains(eventCard1))
                game.UsedEventDeck.Add(eventCard1);
        }
    }
}
