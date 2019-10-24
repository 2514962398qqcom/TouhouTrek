using System.Linq;
using System.Collections.Generic;

using NUnit.Framework;

using ZMDFQ;
using ZMDFQ.Cards;
using ZMDFQ.PlayerAction;

using UnityEngine;
using System.Threading.Tasks;

namespace Tests
{
    public class ActionCardsTest
    {
        [Test]
        public void AT_N003Test()
        {
            Game game = new Game();
            (game.Database as ConfigManager).RegisterCard(0xA000, new TestAction_Empty());
            (game.Database as ConfigManager).RegisterCard(0xC000, new TestCharacter_Empty());
            (game.Database as ConfigManager).RegisterCard(0xF000, new TestOfficial_Empty());
            (game.Database as ConfigManager).RegisterCard(0xE000, new TestEvent_Empty());
            game.Init(new GameOptions()
            {
                PlayerInfos = new GameOptions.PlayerInfo[]
                {
                    new GameOptions.PlayerInfo() { Id = 0 },
                    new GameOptions.PlayerInfo() { Id = 1 }
                },
                Cards = Enumerable.Empty<int>()
                .concatRepeat(game.getCardID<AT_N003>(), 20)//行动
                .concatRepeat(0xC000, 20)//角色
                .concatRepeat(0xF000, 20)//官作
                .concatRepeat(0xE000, 20),//事件
                firstPlayer = 0,
                shuffle = false,
                initCommunitySize = 0,
                initInfluence = 0,
                chooseCharacter = true,
                doubleCharacter = false
            });
            game.StartGame();
            game.Answer(new ChooseHeroResponse() { PlayerId = 0, HeroId = 21 });
            game.Answer(new ChooseHeroResponse() { PlayerId = 1, HeroId = 24 });
            game.Answer(new FreeUse() { PlayerId = 0, CardId = 1, Source = new List<int>() { 1 } });

            Assert.AreEqual(4, game.Players[0].ActionCards.Count);
        }
        [Test]
        public void AT_N005Test()
        {
            Game game = new Game();
            (game.Database as ConfigManager).RegisterCard(0xA000, new TestAction_Empty());
            (game.Database as ConfigManager).RegisterCard(0xC000, new TestCharacter_Empty());
            (game.Database as ConfigManager).RegisterCard(0xF000, new TestOfficial_Empty());
            (game.Database as ConfigManager).RegisterCard(0xE000, new TestEvent_Empty());
            game.Init(new GameOptions()
            {
                PlayerInfos = new GameOptions.PlayerInfo[]
                {
                    new GameOptions.PlayerInfo() { Id = 0 },
                    new GameOptions.PlayerInfo() { Id = 1 }
                },
                Cards = Enumerable.Empty<int>()
                .concatRepeat(game.getCardID<AT_N005>(), 20)//行动
                .concatRepeat(0xC000, 20)//角色
                .concatRepeat(0xF000, 20)//官作
                .concatRepeat(0xE000, 20),//事件
                firstPlayer = 0,
                shuffle = false,
                initCommunitySize = 0,
                initInfluence = 0,
                chooseCharacter = true,
                doubleCharacter = false
            });
            game.StartGame();
            game.Answer(new ChooseHeroResponse() { PlayerId = 0, HeroId = 21 });
            game.Answer(new ChooseHeroResponse() { PlayerId = 1, HeroId = 24 });
            game.setDice(6);
            int cardID = game.Players[0].ActionCards[0].Id;
            game.Answer(new FreeUse() { PlayerId = 0, CardId = cardID, Source = new List<int>() { cardID }, PlayersId = new List<int>() { 1 } });

            Assert.AreEqual(-2, game.Players[1].Size);

            cardID = game.Players[1].ActionCards[0].Id;
            game.Answer(new UseLimitCardResponse() { PlayerId = 1, CardId = cardID, Source = new List<int>() { cardID }, Used = true });

            Assert.AreEqual(-2, game.Players[0].Size);
        }
        [Test]
        public void AT_D009Test()
        {
            Game game = new Game();
            (game.Database as ConfigManager).RegisterCard(0xA000, new TestAction_Empty());
            (game.Database as ConfigManager).RegisterCard(0xC000, new TestCharacter_Empty());
            (game.Database as ConfigManager).RegisterCard(0xF000, new TestOfficial_Empty());
            (game.Database as ConfigManager).RegisterCard(0xE000, new TestEvent_Empty());
            (game.Database as ConfigManager).RegisterCard(0xE002, new TestEvent_AddCSAndInf());
            game.Init(new GameOptions()
            {
                PlayerInfos = new GameOptions.PlayerInfo[]
                {
                    new GameOptions.PlayerInfo() { Id = 0 },
                    new GameOptions.PlayerInfo() { Id = 1 }
                },
                Cards = Enumerable.Empty<int>()
                .concatRepeat(game.getCardID<AT_D009>(), 20)//行动
                .concatRepeat(0xC000, 20)//角色
                .concatRepeat(0xF000, 1).concatRepeat(game.getCardID<G_013>(), 19)//官作
                .concatRepeat(0xE002, 20),//事件
                firstPlayer = 0,
                shuffle = false,
                initCommunitySize = 0,
                initInfluence = 0,
                chooseCharacter = true,
                doubleCharacter = false
            });
            game.StartGame();
            game.Answer(new ChooseHeroResponse() { PlayerId = 0, HeroId = 21 });
            game.Answer(new ChooseHeroResponse() { PlayerId = 1, HeroId = 24 });
            //正常使用测试
            int cardID = game.Players[0].ActionCards[0].Id;
            game.Answer(new FreeUse() { PlayerId = 0, CardId = cardID, Source = new List<int>() { cardID } });
            Assert.AreEqual(cardID, game.DelayActionDeck[0].Id);
            Assert.False(game.Players[0].ActionCards.Any(c => c.Id == cardID));//测试是否进入延迟区
            game.Answer(new EndFreeUseResponse() { PlayerId = 0 });
            cardID = game.Players[0].EventCards[0].Id;
            game.Answer(new ChooseDirectionResponse() { PlayerId = 0, CardId = cardID });
            Assert.AreEqual(2, game.Size);
            Assert.AreEqual(2, game.Players[0].Size);//偏移量+1
            Assert.AreEqual(0, game.DelayActionDeck.Count);
            Assert.True(game.UsedActionDeck.Any(c => c.configID == game.getCardID<AT_D009>()));//结算完毕进入弃牌区
            //当前回合不发动事件
            cardID = game.Players[1].ActionCards[0].Id;
            game.Answer(new FreeUse() { PlayerId = 1, CardId = cardID, Source = new List<int>() { cardID } });
            game.Answer(new EndFreeUseResponse() { PlayerId = 1 });
            cardID = game.Players[1].EventCards[0].Id;
            game.Answer(new ChooseDirectionResponse() { PlayerId = 1, CardId = cardID, IfSet = true });
            Assert.AreEqual(2, game.Size);
            Assert.AreEqual(0, game.Players[1].Size);
            game.Answer(new ChooseSomeCardResponse() { PlayerId = 1, Cards = new List<int>(game.Players[1].ActionCards.Take(1).Select(c => c.Id)) });
            //与辉针城互动
            cardID = game.Players[0].ActionCards[0].Id;
            game.Answer(new FreeUse() { PlayerId = 0, CardId = cardID, Source = new List<int>() { cardID } });
            Assert.AreEqual(2, game.DelayActionDeck.Count);
            game.Answer(new EndFreeUseResponse() { PlayerId = 0 });
            cardID = game.Players[0].EventCards[0].Id;
            game.Answer(new ChooseDirectionResponse() { PlayerId = 0, CardId = cardID });
            Assert.AreEqual(0, game.DelayActionDeck.Count);
            Assert.AreEqual(4, game.UsedActionDeck.Count);
            Assert.AreEqual(8, game.Size);
            Assert.AreEqual(5, game.Players[0].Size);
            //与盖卡互动
            cardID = game.Players[1].ActionCards[0].Id;
            game.Answer(new FreeUse() { PlayerId = 1, CardId = cardID, Source = new List<int>() { cardID } });
            game.Answer(new EndFreeUseResponse() { PlayerId = 1 });
            cardID = game.Players[1].EventCards[0].Id;
            game.Answer(new ChooseDirectionResponse() { PlayerId = 1, CardId = cardID, IfSet = true });
            Assert.AreEqual(10, game.Size);
            Assert.AreEqual(4, game.Players[1].Size);
        }
        [Test]
        public void AT_G013Test()
        {
            Game game = new Game();
            (game.Database as ConfigManager).RegisterCard(0xA000, new TestAction_Empty());
            (game.Database as ConfigManager).RegisterCard(0xC000, new TestCharacter_Empty());
            (game.Database as ConfigManager).RegisterCard(0xF000, new TestOfficial_Empty());
            (game.Database as ConfigManager).RegisterCard(0xE000, new TestEvent_Empty());
            game.Init(new GameOptions()
            {
                PlayerInfos = new GameOptions.PlayerInfo[]
                {
                    new GameOptions.PlayerInfo() { Id = 0 },
                    new GameOptions.PlayerInfo() { Id = 1 },
                    new GameOptions.PlayerInfo() { Id = 2 },
                    new GameOptions.PlayerInfo() { Id = 3 }
                },
                Cards = Enumerable.Empty<int>()
                .concatRepeat(game.getCardID<AT_G013>(), 20)//行动
                .concatRepeat(0xC000, 20)//角色
                .concatRepeat(0xF000, 20)//官作
                .concatRepeat(0xE000, 20),//事件
                firstPlayer = 0,
                shuffle = false,
                initCommunitySize = 0,
                initInfluence = 0,
                chooseCharacter = true,
                doubleCharacter = false
            });
            game.StartGame();
            game.Answer(new ChooseHeroResponse() { PlayerId = 0, HeroId = 21 });
            game.Answer(new ChooseHeroResponse() { PlayerId = 1, HeroId = 24 });
            game.Answer(new ChooseHeroResponse() { PlayerId = 2, HeroId = 27 });
            game.Answer(new ChooseHeroResponse() { PlayerId = 3, HeroId = 30 });
            //1人使用众筹：+1影响力
            int cardID = game.Players[0].ActionCards[0].Id;
            game.Answer(new FreeUse() { PlayerId = 0, CardId = cardID, Source = new List<int>() { cardID } });
            game.Answer(new ChooseSomeCardResponse() { PlayerId = 1, Cards = new List<int>() });
            game.Answer(new ChooseSomeCardResponse() { PlayerId = 2, Cards = new List<int>() });
            game.Answer(new ChooseSomeCardResponse() { PlayerId = 3, Cards = new List<int>() });
            Assert.AreEqual(1, game.Players[0].Size);
        }
        [Test]
        public void AT_N014Test()
        {
            Game game = new Game();
            (game.Database as ConfigManager).RegisterCard(0xA000, new TestAction_Empty());
            (game.Database as ConfigManager).RegisterCard(0xC000, new TestCharacter_Empty());
            (game.Database as ConfigManager).RegisterCard(0xF000, new TestOfficial_Empty());
            (game.Database as ConfigManager).RegisterCard(0xE000, new TestEvent_Empty());
            (game.Database as ConfigManager).RegisterCard(0xE001, new TestEvent_AddAllPlayerInf());
            game.Init(new GameOptions()
            {
                PlayerInfos = new GameOptions.PlayerInfo[]
                {
                    new GameOptions.PlayerInfo() { Id = 0 },
                    new GameOptions.PlayerInfo() { Id = 1 }
                },
                Cards = Enumerable.Empty<int>()
                .concatRepeat(game.getCardID<AT_N014>(), 20)//行动
                .concatRepeat(0xC000, 20)//角色
                .concatRepeat(0xF000, 20)//官作
                .concatRepeat(0xE001, 20),//事件
                firstPlayer = 0,
                shuffle = false,
                initCommunitySize = 0,
                initInfluence = 0,
                chooseCharacter = true,
                doubleCharacter = false
            });
            game.StartGame();
            game.Answer(new ChooseHeroResponse() { PlayerId = 0, HeroId = 21 });
            game.Answer(new ChooseHeroResponse() { PlayerId = 1, HeroId = 24 });

            game.Answer(new EndFreeUseResponse() { PlayerId = 0 });
            int cardID = game.Players[0].EventCards[0].Id;
            game.Answer(new ChooseDirectionResponse() { PlayerId = 0, CardId = cardID, IfForward = true });
            Assert.AreEqual(1, game.Players[0].Size);
            Assert.AreEqual(1, game.Players[1].Size);
            game.Answer(new ChooseSomeCardResponse() { PlayerId = 0, Cards = new List<int>(game.Players[0].ActionCards.Take(2).Select(c => c.Id)) });

            cardID = game.Players[1].ActionCards[0].Id;
            game.Answer(new FreeUse() { PlayerId = 1, CardId = cardID, Source = new List<int>() { cardID } });
            game.Answer(new DiscoverResponse() { PlayerId = 1, SelectedCards = new List<int>() { game.UsedEventDeck[0].Id } });
            game.Answer(new EndFreeUseResponse() { PlayerId = 1 });
            cardID = game.Players[1].EventCards[0].Id;
            game.Answer(new ChooseDirectionResponse() { PlayerId = 1, CardId = cardID, IfForward = true });
            Assert.AreEqual(1, game.Players[0].Size);
            Assert.AreEqual(2, game.Players[1].Size);
        }
        [Test]
        public void AT_N020()
        {
            Game game = new Game();
            (game.Database as ConfigManager).RegisterCard(0xA000, new TestAction_Empty());
            (game.Database as ConfigManager).RegisterCard(0xA001, new TestAction_Add1Inf());
            (game.Database as ConfigManager).RegisterCard(0xC000, new TestCharacter_Empty());
            (game.Database as ConfigManager).RegisterCard(0xF000, new TestOfficial_Empty());
            (game.Database as ConfigManager).RegisterCard(0xE000, new TestEvent_Empty());
            (game.Database as ConfigManager).RegisterCard(0xE001, new TestEvent_DoubleOrZeroPlayerInf());
            game.Init(new GameOptions()
            {
                PlayerInfos = new GameOptions.PlayerInfo[]
                {
                    new GameOptions.PlayerInfo() { Id = 0 },
                    new GameOptions.PlayerInfo() { Id = 1 }
                },
                Cards = Enumerable.Empty<int>()
                .concatRepeat(0xA001, 1).concatRepeat(game.getCardID<AT_N020>(), 19)//行动
                .concatRepeat(0xC000, 20)//角色
                .concatRepeat(0xF000, 20)//官作
                .concatRepeat(0xE001, 20),//事件
                firstPlayer = 0,
                shuffle = false,
                initCommunitySize = 0,
                initInfluence = 0,
                chooseCharacter = true,
                doubleCharacter = false
            });
            game.StartGame();
            game.Answer(new ChooseHeroResponse() { PlayerId = 0, HeroId = 21 });
            game.Answer(new ChooseHeroResponse() { PlayerId = 1, HeroId = 24 });

            int cardID = game.Players[0].ActionCards[0].Id;
            game.Answer(new FreeUse() { PlayerId = 0, CardId = cardID, Source = new List<int>() { cardID } });
            Assert.AreEqual(1, game.Players[0].Size);
            game.Answer(new EndFreeUseResponse() { PlayerId = 0 });
            cardID = game.Players[0].EventCards[0].Id;
            game.Answer(new ChooseDirectionResponse() { PlayerId = 0, CardId = cardID, IfForward = true });
            game.Answer(new UseLimitCardResponse() { PlayerId = 0, Used = false });
            cardID = game.Players[1].ActionCards[0].Id;
            game.Answer(new UseLimitCardResponse() { PlayerId = 1, Used = true, CardId = cardID, Source = new List<int>() { cardID } });
            Assert.AreEqual(0, game.Players[0].Size);
        }
        [Test]
        public void AT_N023()
        {
            Game game = new Game();
            (game.Database as ConfigManager).RegisterCard(0xA000, new TestAction_Empty());
            (game.Database as ConfigManager).RegisterCard(0xA001, new TestAction_Add1Inf());
            (game.Database as ConfigManager).RegisterCard(0xA002, new TestAction_DelayDraw());
            (game.Database as ConfigManager).RegisterCard(0xC000, new TestCharacter_Empty());
            (game.Database as ConfigManager).RegisterCard(0xF000, new TestOfficial_Empty());
            (game.Database as ConfigManager).RegisterCard(0xE000, new TestEvent_Empty());
            (game.Database as ConfigManager).RegisterCard(0xE001, new TestEvent_DoubleOrZeroPlayerInf());
            game.Init(new GameOptions()
            {
                PlayerInfos = new GameOptions.PlayerInfo[]
                {
                    new GameOptions.PlayerInfo() { Id = 0 },
                    new GameOptions.PlayerInfo() { Id = 1 }
                },
                Cards = Enumerable.Empty<int>()
                .concatRepeat(game.getCardID<AT_N023>(), 1).concatRepeat(0xA001, 1).concatRepeat(0xA002, 3).concatRepeat(game.getCardID<AT_N020>(), 1).concatRepeat(0xA001, 14)//行动
                .concatRepeat(0xC000, 20)//角色
                .concatRepeat(0xF000, 20)//官作
                .concatRepeat(0xE001, 20),//事件
                firstPlayer = 0,
                shuffle = false,
                initCommunitySize = 0,
                initInfluence = 0,
                chooseCharacter = true,
                doubleCharacter = false
            });
            game.StartGame();
            game.Answer(new ChooseHeroResponse() { PlayerId = 0, HeroId = 21 });
            game.Answer(new ChooseHeroResponse() { PlayerId = 1, HeroId = 24 });

            //复读普通的行动牌
            game.useAction(0, game.Players[0].findHand<TestAction_Add1Inf>());//影响力+1
            Assert.AreEqual(1, game.Players[0].Size);
            game.useAction(0, game.Players[0].findHand<AT_N023>());//复读机
            Assert.AreEqual(2, game.Players[0].Size);
            Assert.IsFalse(game.UsedActionDeck.Any(c => c is AT_N023));//复读机没有进弃牌堆
            game.useAction(0, game.Players[0].findHand<TestAction_DelayDraw>());//延迟行动牌
            game.endAction(0);
            game.useEvent(0, false);
            game.tryDiscard(0);
            //复读延迟行动牌
            Assert.AreEqual(1, game.Players[1].ActionCards.Where(c => c is AT_N023).Count());//复读机传递成功
            Assert.AreEqual(1, game.Players[0].ActionCards.Count);
            Assert.AreEqual(4, game.Players[1].ActionCards.Count);
            Assert.IsInstanceOf<TestAction_DelayDraw>(game.UsedActionDeck[game.UsedActionDeck.Count - 1]);
            game.useAction(1, game.Players[1].findHand<AT_N023>());//复读延迟牌
            Assert.AreEqual(1, game.DelayActionDeck.Count);
            Assert.IsInstanceOf<AT_N023>(game.DelayActionDeck[0]);//复读机被挂上去了
            game.endAction(1);
            game.Players[1].Size = 5;//干，不能让他弃牌，不然弃牌就会替代墨菲。而且还必须写在useEvent之前，不然一路就进弃牌阶段了。
            game.useEvent(1, false);
            int cardID = game.Players[0].ActionCards.Find(c => c is AT_N020).Id;
            game.Answer(new UseLimitCardResponse() { PlayerId = 0, CardId = cardID, Source = new List<int>() { cardID }, Used = true });//发动墨菲
            game.tryDiscard(1);
            //复读墨菲定理
            Assert.AreEqual(4, game.Players[1].ActionCards.Count);//复读机延迟生效
            Assert.AreEqual(1, game.Players[0].ActionCards.Where(c => c is AT_N023).Count());//复读机传递成功
            game.Players[0].Size = 1;//用于测试事件的影响力
            game.endAction(0);
            Assert.IsInstanceOf<AT_N020>(game.UsedActionDeck[game.UsedActionDeck.Count - 1]);
            game.useEvent(0, true);
            cardID = game.Players[0].ActionCards.Find(c => c is AT_N023).Id;
            game.Answer(new UseLimitCardResponse() { PlayerId = 0, CardId = cardID, Source = new List<int>(cardID), Used = true });//复读墨菲
            Assert.AreEqual(0, game.Players[0].Size);
        }
    }
    class TestAction_DelayDraw : ActionCard
    {
        public override bool isDelay => true;
        public override Task DoEffect(Game game, FreeUse useWay)
        {
            ActionCard source = game.GetCard(useWay.Source[0]) as ActionCard;
            source.setProp("onTurnStart", new CardCallback(source, onTurnStart));
            source.setProp("player", game.GetPlayer(useWay.PlayerId));
            game.EventSystem.Register(EventEnum.TurnStart, game.GetSeat(game.GetPlayer(useWay.PlayerId)), source.getProp<CardCallback>("onTurnStart").call);
            Debug.Log(source + "注册延迟效果");
            return Task.CompletedTask;
        }
        async Task onTurnStart(Card thisCard, object[] args)
        {
            Debug.Log(thisCard + "延迟效果发动");
            Game game = args[0] as Game;
            Player player = thisCard.getProp<Player>("player");
            await player.DrawActionCard(game, 1);
            game.EventSystem.Remove(EventEnum.TurnStart, thisCard.getProp<CardCallback>("onTurnStart").call);
            game.DelayActionDeck.Remove(thisCard as ActionCard);
            await (thisCard as ActionCard).onEffected(game);
        }
    }
    class TestEvent_AddCSAndInf : EventCard
    {
        public override bool ForwardOnly => true;
        public override Task DoEffect(Game game, ChooseDirectionResponse response, List<Player> unaffectedPlayers)
        {
            return ZMDFQ.Effects.UseCard.UseEventCard(game, response, this, async (g, r) =>
            {
                await g.ChangeSize(1, this);
                await g.GetPlayer(r.PlayerId).ChangeSize(game, 1, this, g.GetPlayer(r.PlayerId));
            });
        }
    }
    class TestEvent_AddAllPlayerInf : EventCard
    {
        public override bool ForwardOnly => true;

        public override Task DoEffect(Game game, ChooseDirectionResponse response, List<Player> unaffectedPlayers)
        {
            return ZMDFQ.Effects.UseCard.UseEventCard(game, response, this, async (g, r) =>
            {
                foreach (Player player in g.Players.Where(p => !unaffectedPlayers.Contains(p)))
                {
                    await player.ChangeSize(g, 1, this, g.GetPlayer(r.PlayerId));
                }
            });
        }
    }
}