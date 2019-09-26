﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace ZMDFQ
{
    using PlayerAction;
    using System.Threading.Tasks;

    public class Player
    {
        public int Id;
        public int Size;
        /// <summary>
        /// 手牌
        /// </summary>
        public List<ActionCard> ActionCards = new List<ActionCard>();

        /// <summary>
        /// 玩家手上的事件牌
        /// </summary>
        public List<EventCard> EventCards = new List<EventCard>();

        /// <summary>
        /// 被玩家扣住的事件
        /// </summary>
        public EventCard SaveEvent;

        public HeroCard Hero;
        /// <summary>
        /// 得分
        /// </summary>
        public int point { get; set; } = 0;
        public Player(int id)
        {
            Id = id;
        }

        internal async Task DrawActionCard(Game game, int count)
        {
            EventData<int> drawCount = new EventData<int>() { data = count };
            await game.EventSystem.Call(EventEnum.BeforDrawActionCard, this, drawCount);
            List<ActionCard> drawedCards = new List<ActionCard>();
            for (int i = 0; i < drawCount.data; i++)
            {
                if (game.Deck.Count == 0)//如果没有行动牌了
                {
                    //就把行动弃牌堆洗入行动牌堆
                    game.Deck.AddRange(game.UsedActionDeck);
                    game.UsedActionDeck.Clear();
                    game.Reshuffle(game.Deck);
                }
                ActionCards.Add(game.Deck[0]);
                drawedCards.Add(game.Deck[0]);
                game.Deck.RemoveAt(0);
            }
            await game.EventSystem.Call(EventEnum.DrawActionCard, this, drawedCards);
        }

        internal async Task DrawEventCard(Game game)
        {
            EventCard card = game.EventDeck[0];
            EventCards.Add(card);
            game.EventDeck.Remove(card);
            await game.EventSystem.Call(EventEnum.DrawEventCard, this, card);
        }

        internal Task UseEventCard(Game game, ChooseDirectionResponse response)
        {
            if (response.IfSet)
            {
                return SetEventCard(game, response);
            }
            else
            {
                //默认玩家手上一定是一张事件卡，有其他情况再改
                EventCard card = EventCards.Find(c => c.Id == response.CardId);
                if (card != null)
                {
                    return card.Use(game, response);
                }
                else
                {
                    Log.Error("未找到卡片(" + response.CardId + ")");
                    return Task.CompletedTask;
                }
            }
        }

        private async Task SetEventCard(Game game, ChooseDirectionResponse response)
        {
            if (SaveEvent != null)
            {
                await SaveEvent.Use(game, response);
            }
            SaveEvent = EventCards[0];
            EventCards.RemoveAt(0);
        }

        /// <summary>
        /// 失去一张事件卡
        /// 注意没有进弃牌堆
        /// </summary>
        /// <param name="game"></param>
        /// <param name="card"></param>
        /// <returns></returns>
        internal async Task DropEventCard(Game game, EventCard card)
        {
            if (card == SaveEvent)
            {
                //game.UsedEventDeck.Add(card);
                SaveEvent = null;
            }
            else
            {
                //game.UsedEventDeck.Add(card);
                EventCards.Remove(card);
            }
        }

        internal Task UseActionCard(Game game, FreeUse useInfo)
        {
            if (useInfo.SkillId < 0)
            {
                //正常用卡
                ActionCard card = ActionCards.Find(x => x.Id == useInfo.CardId);
                if (card == null) return Task.CompletedTask;
                return card.DoEffect(game, useInfo);
            }
            else
            {
                Skill skill = Hero.Skills.Find(x => SkillHelper.getId(x) == useInfo.SkillId);
                if (skill != null)
                    return skill.DoEffect(game, useInfo);
                else
                {
                    throw new System.Exception($"玩家不持有的技能：{useInfo.SkillId}");
                }
            }
        }

        /// <summary>
        /// 失去一张行动牌
        /// 注意不进弃牌堆
        /// </summary>
        /// <param name="game"></param>
        /// <param name="cards"></param>
        internal async Task DropActionCard(Game game, List<int> cards, bool goUsedPile = false)
        {
            List<ActionCard> data = new List<ActionCard>();
            foreach (var cardId in cards)
            {
                ActionCard card = ActionCards.Find(x => x.Id == cardId);
                ActionCards.Remove(card);
                if (goUsedPile)
                    game.UsedActionDeck.Add(card);
                data.Add(card);
            }
            await game.EventSystem.Call(EventEnum.DropActionCard, this, data);
        }

        internal async Task ChangeSize(Game game, int Size, object source)
        {
            var data = new EventData<int>() { data = Size };
            await game.EventSystem.Call(EventEnum.OnPlayrSizeChange, game, this, data);
            this.Size += data.data;
        }

        public async Task<int> HandMax(Game game)
        {
            int result = Size;
            //属性修正
            //foreach (IPropertyModifier<int> modifier in Hero.Skills.Where(s => s is IPropertyModifier<int> m && m.propName == nameof(HandMax)))
            //{
            //    modifier.modify(ref result);
            //}
            if (result < 1)
                result = 1;
            if (result > 4)
                result = 4;
            EventData<int> max = new EventData<int>() { data = result };
            await game.EventSystem.Call(EventEnum.GetHandMax, this, max);
            return max.data;
        }
    }
}
