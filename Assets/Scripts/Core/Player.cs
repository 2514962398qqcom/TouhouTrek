using System;
using System.Linq;
using System.Collections.Generic;

namespace ZMDFQ
{
    using PlayerAction;
    using System.Threading.Tasks;

    public class Player
    {
        /// <summary>
        /// 玩家全局id
        /// </summary>
        public long PlayerId;
        /// <summary>
        /// 本局游戏使用的id
        /// </summary>
        public int Id;
        public string Name;
        public int Size { get; private set; } = 0;
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
        public bool avoidSetEvent { get; set; } = false;
        public Player(int id)
        {
            Id = id;
        }
        /// <summary>
        /// 将卡牌置入玩家的手牌
        /// </summary>
        /// <param name="game"></param>
        /// <param name="cards"></param>
        /// <returns></returns>
        public async Task AddActionCards(Game game, List<ActionCard> cards)
        {
            foreach (ActionCard card in cards)
            {
                card.Owner = this;
                ActionCards.Add(card);
                card.OnEnterHand(game, this);
            }
            await game.EventSystem.Call(EventEnum.AfterAddCard, game.GetSeat(this), game, this, ActionCards, cards);
        }
        public async Task DrawActionCard(Game game, int count)
        {
            EventData<int> drawCount = new EventData<int>() { data = count };
            await game.EventSystem.Call(EventEnum.BeforDrawActionCard, game.ActivePlayerSeat(), this, drawCount);
            List<ActionCard> drawedCards = new List<ActionCard>();
            for (int i = 0; i < drawCount.data; i++)
            {
                if (game.ActionDeck.Count == 0)//如果没有行动牌了
                {
                    //就把行动弃牌堆洗入行动牌堆
                    List<ActionCard> usedCards = new List<ActionCard>(game.UsedActionDeck);
                    game.ActionDeck.AddRange(usedCards);
                    await game.EventSystem.Call(EventEnum.AfterAddCard, game.GetSeat(this), game, null, game.ActionDeck, usedCards);
                    game.UsedActionDeck.Clear();
                    await game.EventSystem.Call(EventEnum.AfterRemoveCard, game.GetSeat(this), game, null, game.UsedActionDeck, usedCards);
                    game.Reshuffle(game.ActionDeck);
                }
                ActionCard card = game.ActionDeck[0];
                game.ActionDeck.Remove(card);
                await AddActionCards(game, new List<ActionCard>() { card });
                drawedCards.Add(card);
            }
            await game.EventSystem.Call(EventEnum.DrawActionCard, game.ActivePlayerSeat(), this, drawedCards);
        }
        void AddEventCard(EventCard card)
        {
            EventCards.Add(card);
            card.Owner = this;
        }
        internal async Task DrawEventCard(Game game)
        {
            EventCard card = game.EventDeck[0];
            game.EventDeck.Remove(card);
            AddEventCard(card);
            await game.EventSystem.Call(EventEnum.DrawEventCard, game.ActivePlayerSeat(), this, card);
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
        /// </summary>
        /// <param name="game"></param>
        /// <param name="card"></param>
        /// <param name="goUsedDeck">是否丢进弃牌堆</param>
        /// <returns></returns>
        internal async Task DropEventCard(Game game, EventCard card, bool goUsedDeck = false)
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
            card.Owner = null;
            if (goUsedDeck)
                game.UsedEventDeck.Add(card);
        }

        internal Task UseActionCard(Game game, FreeUse useInfo)
        {
            if (useInfo.SkillId < 0)
            {
                //正常用卡
                ActionCard card = ActionCards.Find(x => x.Id == useInfo.CardId);
                if (card == null)
                    return Task.CompletedTask;
                return Effects.UseCard.UseActionCard(game, useInfo, card, card.DoEffect);
                //return card.DoEffect(game, useInfo);
            }
            else
            {
                Skill skill = Hero.Skills.Find(x => SkillHelper.getId(x) == useInfo.SkillId);
                if (skill != null)
                    return skill.DoEffect(game, useInfo);
                else
                {
                    throw new Exception($"玩家不持有的技能：{useInfo.SkillId}");
                }
            }
        }

        /// <summary>
        /// 失去复数张牌
        /// </summary>
        /// <param name="game"></param>
        /// <param name="cards"></param>
        /// <param name="goUsedPile">是否进入弃牌堆,为false时不改变owner</param>
        /// <param name="ifPassive">是否主动丢牌</param>
        /// <returns></returns>
        internal async Task DropActionCard(Game game, List<int> cards, bool goUsedPile = false, bool ifPassive = false)
        {
            if (ifPassive)
            {
                EventData<bool> dropData = new EventData<bool>(true);
                //被动丢牌抛出一个事件
                await game.EventSystem.Call(EventEnum.BeforePassiveDropActionCard, game.GetSeat(this), game, this, dropData);
                if (!dropData.data)
                {
                    //这次丢牌被取消了
                    return;
                }
            }
            List<ActionCard> data = new List<ActionCard>();
            foreach (var cardId in cards)
            {
                ActionCard card = ActionCards.Find(x => x.Id == cardId);
                ActionCards.Remove(card);
                if (goUsedPile)
                {
                    await game.AddUsedActionCard(new List<ActionCard>() { card });
                    card.Owner = null;
                }
                data.Add(card);
                card.OnLeaveHand(game, this);
            }
            await game.EventSystem.Call(EventEnum.DropActionCard, game.GetSeat(this), this, data);
        }
        /// <summary>
        /// 丢弃手牌
        /// </summary>
        /// <param name="game"></param>
        /// <param name="cards"></param>
        /// <returns></returns>
        public Task discard(Game game, List<int> cards)
        {
            return DropActionCard(game, cards, true);
        }
        public async Task ChangeSize(Game game, int Size, object source, Player sourcePlayer)
        {
            var data = new EventData<int>() { data = Size };
            await game.EventSystem.Call(EventEnum.BeforePlayrSizeChange, game.ActivePlayerSeat(), game, this, data, source);
            if (this.Size + data.data > await getSizeMax(game))
                data.data = await getSizeMax(game) - this.Size;
            if (this.Size + data.data < await getSizeMin(game))
                data.data = await getSizeMin(game) - this.Size;
            this.Size += data.data;
            await game.EventSystem.Call(EventEnum.AfterPlayrSizeChange, game.ActivePlayerSeat(), game, this, data, new EventData<object>() { data = source }, sourcePlayer);
        }
        /// <summary>
        /// 直接设置玩家的个人影响力而不经过游戏逻辑和事件触发。这个方法通常用于初始化和测试，不要在游戏效果中调用它。
        /// </summary>
        /// <param name="value"></param>
        public void SetSize(int value)
        {
            Size = value;
        }
        public async Task<int> getSizeMin(Game game)
        {
            EventData<int> min = new EventData<int>() { data = -5 };
            await game.EventSystem.Call(EventEnum.BeforeGetPlayerSizeMin, game.GetSeat(this), game, this, min);
            return min.data;
        }
        public async Task<int> getSizeMax(Game game)
        {
            EventData<int> max = new EventData<int>() { data = 5 };
            await game.EventSystem.Call(EventEnum.BeforeGetPlayerSizeMax, game.GetSeat(this), game, this, max);
            return max.data;
        }
        public async Task<int> HandMax(Game game)
        {
            int result = Size;
            if (result < 1)
                result = 1;
            if (result > 4)
                result = 4;
            EventData<int> max = new EventData<int>() { data = result };
            await game.EventSystem.Call(EventEnum.GetHandMax, game.ActivePlayerSeat(), this, max);
            return max.data;
        }
    }
}
