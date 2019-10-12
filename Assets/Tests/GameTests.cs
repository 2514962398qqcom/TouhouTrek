using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using ZMDFQ;
using ZMDFQ.Cards;
using ZMDFQ.Effects;
using ZMDFQ.PlayerAction;

namespace Tests
{
    public class OtherTests
    {
        [UnityTest]
        public IEnumerator getTypeTest()
        {
            Assert.AreEqual(typeof(EV_E001), typeof(Card).Assembly.GetTypes().First(t => t.Name == nameof(EV_E001)));
            yield break;
        }
    }
    public class GameTests
    {
        [UnityTest]
        public IEnumerator initTest()
        {
            Game game = new Game();
            (game.Database as ConfigManager).AddCard(0xA000, new TestAction_Empty());
            (game.Database as ConfigManager).AddCard(0xC000, new TestCharacter_Empty());
            (game.Database as ConfigManager).AddCard(0xF000, new TestOfficial_Empty());
            (game.Database as ConfigManager).AddCard(0xE000, new TestEvent_Empty());
            game.Init(new GameOptions()
            {
                PlayerInfos = new GameOptions.PlayerInfo[]
                {
                    new GameOptions.PlayerInfo() { Id = 0 },
                    new GameOptions.PlayerInfo() { Id = 1 }
                },
                Cards = new int[] { }
                .concatRepeat(0xA000, 20)//行动
                .concatRepeat(0xC000, 20)//角色
                .concatRepeat(0xF000, 20)//官作
                .concatRepeat(0xE000, 20),//事件
                shuffle = false,
                initCommunitySize = 0,
                initInfluence = 0,
            });

            Assert.AreEqual(2, game.Players.Count);
            Assert.IsInstanceOf<Player>(game.Players[0]);
            Assert.IsInstanceOf<Player>(game.Players[1]);
            Assert.AreEqual(20, game.ActionDeck.Count);
            Assert.AreEqual(20, game.ThemeDeck.Count);
            Assert.AreEqual(20, game.EventDeck.Count);
            Assert.AreEqual(0, game.Size);
            yield break;
        }
        [UnityTest]
        public IEnumerator startGameTest()
        {
            Game game = new Game();
            (game.Database as ConfigManager).AddCard(0xA000, new TestAction_Empty());
            (game.Database as ConfigManager).AddCard(0xC000, new TestCharacter_Empty());
            (game.Database as ConfigManager).AddCard(0xF000, new TestOfficial_Empty());
            (game.Database as ConfigManager).AddCard(0xE000, new TestEvent_Empty());
            game.Init(new GameOptions()
            {
                PlayerInfos = new GameOptions.PlayerInfo[]
                {
                    new GameOptions.PlayerInfo() { Id = 0 },
                    new GameOptions.PlayerInfo() { Id = 1 }
                },
                Cards = new int[] { }
                .concatRepeat(0xA000, 20)//行动
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

            Assert.AreEqual(0, game.ActivePlayer.Id);
            Assert.AreEqual(0, game.Players[0].Size);
            Assert.IsInstanceOf<TestCharacter_Empty>(game.Players[0].Hero);
            Assert.AreEqual(0, game.Players[1].Size);
            Assert.IsInstanceOf<TestCharacter_Empty>(game.Players[1].Hero);
            Assert.IsInstanceOf<TestOfficial_Empty>(game.ActiveTheme);
            Assert.AreEqual(0, game.Size);
            Assert.AreEqual(1, game.ActivePlayer.EventCards.Count);
            Assert.IsInstanceOf<TestEvent_Empty>(game.ActivePlayer.EventCards[0]);
            Assert.AreEqual(3, game.ActivePlayer.ActionCards.Count);
            Assert.IsInstanceOf<TestAction_Empty>(game.ActivePlayer.ActionCards[0]);
            Assert.IsInstanceOf<TestAction_Empty>(game.ActivePlayer.ActionCards[1]);
            Assert.IsInstanceOf<TestAction_Empty>(game.ActivePlayer.ActionCards[2]);
            yield break;
        }
        [UnityTest]
        public IEnumerator useActionTest()
        {
            Game game = new Game();
            (game.Database as ConfigManager).AddCard(0xA000, new TestAction_Empty());
            (game.Database as ConfigManager).AddCard(0xA001, new TestAction_Add1Inf());
            (game.Database as ConfigManager).AddCard(0xC000, new TestCharacter_Empty());
            (game.Database as ConfigManager).AddCard(0xF000, new TestOfficial_Empty());
            (game.Database as ConfigManager).AddCard(0xE000, new TestEvent_Empty());
            (game.Database as ConfigManager).AddCard(0xE001, new TestEvent_DoubleOrZeroPlayerInf());
            game.Init(new GameOptions()
            {
                PlayerInfos = new GameOptions.PlayerInfo[]
                {
                    new GameOptions.PlayerInfo() { Id = 0 },
                    new GameOptions.PlayerInfo() { Id = 1 }
                },
                Cards = new int[] { }
                .concatRepeat(0xA001, 20)//行动
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

            AssertExtension.checkActionUse(game, game.Players[0], 1);
            Assert.AreEqual(1, game.Players[0].Size);
            yield break;
        }
        [UnityTest]
        public IEnumerator useEventTest()
        {
            Game game = new Game();
            (game.Database as ConfigManager).AddCard(0xA000, new TestAction_Empty());
            (game.Database as ConfigManager).AddCard(0xA001, new TestAction_Add1Inf());
            (game.Database as ConfigManager).AddCard(0xC000, new TestCharacter_Empty());
            (game.Database as ConfigManager).AddCard(0xF000, new TestOfficial_Empty());
            (game.Database as ConfigManager).AddCard(0xE000, new TestEvent_Empty());
            (game.Database as ConfigManager).AddCard(0xE001, new TestEvent_DoubleOrZeroPlayerInf());
            game.Init(new GameOptions()
            {
                PlayerInfos = new GameOptions.PlayerInfo[]
                {
                    new GameOptions.PlayerInfo() { Id = 0 },
                    new GameOptions.PlayerInfo() { Id = 1 }
                },
                Cards = new int[] { }
                .concatRepeat(0xA001, 20)//行动
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
            game.Answer(new FreeUse() { PlayerId = 0, CardId = 1, Source = new List<int>() { 1 } });
            game.Answer(new FreeUse() { PlayerId = 0, CardId = 2, Source = new List<int>() { 2 } });
            game.Answer(new EndFreeUseResponse() { PlayerId = 0 });
            game.Answer(new ChooseDirectionResponse() { PlayerId = 0, CardId = 61, IfForward = true });

            Assert.AreEqual(4, game.Players[0].Size);
            Assert.IsFalse(game.Players[0].EventCards.Any(c => c.Id == 61));
            Assert.IsTrue(game.UsedEventDeck.Any(c => c.Id == 61));
            yield break;
        }
        [UnityTest]
        public IEnumerator useEventReverseTest()
        {
            Game game = new Game();
            (game.Database as ConfigManager).AddCard(0xA000, new TestAction_Empty());
            (game.Database as ConfigManager).AddCard(0xA001, new TestAction_Add1Inf());
            (game.Database as ConfigManager).AddCard(0xC000, new TestCharacter_Empty());
            (game.Database as ConfigManager).AddCard(0xF000, new TestOfficial_Empty());
            (game.Database as ConfigManager).AddCard(0xE000, new TestEvent_Empty());
            (game.Database as ConfigManager).AddCard(0xE001, new TestEvent_DoubleOrZeroPlayerInf());
            game.Init(new GameOptions()
            {
                PlayerInfos = new GameOptions.PlayerInfo[]
                {
                    new GameOptions.PlayerInfo() { Id = 0 },
                    new GameOptions.PlayerInfo() { Id = 1 }
                },
                Cards = new int[] { }
                .concatRepeat(0xA001, 20)//行动
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
            game.Answer(new FreeUse() { PlayerId = 0, CardId = 1, Source = new List<int>() { 1 } });
            game.Answer(new FreeUse() { PlayerId = 0, CardId = 2, Source = new List<int>() { 2 } });
            game.Answer(new EndFreeUseResponse() { PlayerId = 0 });
            game.Answer(new ChooseDirectionResponse() { PlayerId = 0, CardId = 61, IfForward = false });

            Assert.AreEqual(0, game.Players[0].Size);
            yield break;
        }
        [UnityTest]
        public IEnumerator setEventTest()
        {
            Game game = new Game();
            (game.Database as ConfigManager).AddCard(0xA000, new TestAction_Empty());
            (game.Database as ConfigManager).AddCard(0xA001, new TestAction_Add1Inf());
            (game.Database as ConfigManager).AddCard(0xC000, new TestCharacter_Empty());
            (game.Database as ConfigManager).AddCard(0xF000, new TestOfficial_Empty());
            (game.Database as ConfigManager).AddCard(0xE000, new TestEvent_Empty());
            (game.Database as ConfigManager).AddCard(0xE001, new TestEvent_DoubleOrZeroPlayerInf());
            game.Init(new GameOptions()
            {
                PlayerInfos = new GameOptions.PlayerInfo[]
                {
                    new GameOptions.PlayerInfo() { Id = 0 },
                    new GameOptions.PlayerInfo() { Id = 1 }
                },
                Cards = new int[] { }
                .concatRepeat(0xA001, 20)//行动
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
            game.Answer(new ChooseDirectionResponse() { PlayerId = 0, CardId = 61, IfSet = true });
            game.Answer(new ChooseSomeCardResponse() { PlayerId = 0, Cards = new List<int>() { 1, 2 } });

            Assert.IsInstanceOf<TestEvent_DoubleOrZeroPlayerInf>(game.Players[0].SaveEvent);
            Assert.AreEqual(61, game.Players[0].SaveEvent.Id);

            game.Answer(new EndFreeUseResponse() { PlayerId = 1 });
            game.Answer(new ChooseDirectionResponse() { PlayerId = 1, CardId = 62, IfSet = true });
            game.Answer(new ChooseSomeCardResponse() { PlayerId = 1, Cards = new List<int>() { 3, 4 } });

            Assert.IsInstanceOf<TestEvent_DoubleOrZeroPlayerInf>(game.Players[1].SaveEvent);
            Assert.AreEqual(62, game.Players[1].SaveEvent.Id);

            game.Answer(new FreeUse() { PlayerId = 0, CardId = 5, Source = new List<int>() { 5 } });
            game.Answer(new EndFreeUseResponse() { PlayerId = 0 });
            game.Answer(new ChooseDirectionResponse() { PlayerId = 0, CardId = 63, IfSet = true });

            Assert.AreEqual(2, game.Players[0].Size);
            Assert.IsInstanceOf<TestEvent_DoubleOrZeroPlayerInf>(game.Players[0].SaveEvent);
            Assert.AreEqual(63, game.Players[0].SaveEvent.Id);

            yield break;
        }
        [UnityTest]
        public IEnumerator discardTest()
        {
            Game game = new Game();
            (game.Database as ConfigManager).AddCard(0xA000, new TestAction_Empty());
            (game.Database as ConfigManager).AddCard(0xA001, new TestAction_Add1Inf());
            (game.Database as ConfigManager).AddCard(0xC000, new TestCharacter_Empty());
            (game.Database as ConfigManager).AddCard(0xF000, new TestOfficial_Empty());
            (game.Database as ConfigManager).AddCard(0xE000, new TestEvent_Empty());
            (game.Database as ConfigManager).AddCard(0xE001, new TestEvent_DoubleOrZeroPlayerInf());
            game.Init(new GameOptions()
            {
                PlayerInfos = new GameOptions.PlayerInfo[]
                {
                    new GameOptions.PlayerInfo() { Id = 0 },
                    new GameOptions.PlayerInfo() { Id = 1 }
                },
                Cards = new int[] { }
                .concatRepeat(0xA001, 20)//行动
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
            game.Answer(new FreeUse() { PlayerId = 0, CardId = 1, Source = new List<int>() { 1 } });
            game.Answer(new EndFreeUseResponse() { PlayerId = 0 });
            game.Answer(new ChooseDirectionResponse() { PlayerId = 0, CardId = 61, IfForward = true });

            Assert.AreEqual(2, game.Players[0].Size);
            Assert.AreEqual(2, game.Players[0].ActionCards[0].Id);
            Assert.AreEqual(5, game.Players[0].ActionCards[1].Id);
            Assert.AreEqual(2, game.Players[0].ActionCards.Count);

            game.Answer(new EndFreeUseResponse() { PlayerId = 1 });
            game.Answer(new ChooseDirectionResponse() { PlayerId = 1, CardId = 62, IfSet = true });
            game.Answer(new ChooseSomeCardResponse() { PlayerId = 1, Cards = new List<int>() { 3, 4 } });

            Assert.AreEqual(1, game.Players[1].ActionCards.Count);
            Assert.AreEqual(1, game.UsedActionDeck[0].Id);
            Assert.AreEqual(3, game.UsedActionDeck[1].Id);
            Assert.AreEqual(4, game.UsedActionDeck[2].Id);
            Assert.AreEqual(3, game.UsedActionDeck.Count);
            yield break;
        }
        [UnityTest]
        public IEnumerator winTest()
        {
            Game game = new Game();
            (game.Database as ConfigManager).AddCard(0xA000, new TestAction_Empty());
            (game.Database as ConfigManager).AddCard(0xA001, new TestAction_Add1Inf());
            (game.Database as ConfigManager).AddCard(0xC000, new TestCharacter_Empty());
            (game.Database as ConfigManager).AddCard(0xF000, new TestOfficial_Empty());
            (game.Database as ConfigManager).AddCard(0xE000, new TestEvent_Empty());
            (game.Database as ConfigManager).AddCard(0xE001, new TestEvent_DoubleOrZeroPlayerInf());
            game.Init(new GameOptions()
            {
                PlayerInfos = new GameOptions.PlayerInfo[]
                {
                    new GameOptions.PlayerInfo() { Id = 0 },
                    new GameOptions.PlayerInfo() { Id = 1 }
                },
                Cards = new int[] { }
                .concatRepeat(0xA001, 20)//行动
                .concatRepeat(0xC000, 20)//角色
                .concatRepeat(0xF000, 20)//官作
                .concatRepeat(0xE001, 20),//事件
                firstPlayer = 0,
                shuffle = false,
                initCommunitySize = 1,
                initInfluence = 0,
                chooseCharacter = true,
                doubleCharacter = false,
                endingOfficialCardCount = 1
            });
            game.StartGame();
            game.Answer(new ChooseHeroResponse() { PlayerId = 0, HeroId = 21 });
            game.Answer(new ChooseHeroResponse() { PlayerId = 1, HeroId = 24 });
            game.Answer(new FreeUse() { PlayerId = 0, CardId = 1, Source = new List<int>() { 1 } });
            game.Answer(new EndFreeUseResponse() { PlayerId = 0 });
            game.Answer(new ChooseDirectionResponse() { PlayerId = 0, CardId = 61, IfForward = true });

            game.Answer(new EndFreeUseResponse() { PlayerId = 1 });
            game.Answer(new ChooseDirectionResponse() { PlayerId = 1, CardId = 62, IfSet = true });
            game.Answer(new ChooseSomeCardResponse() { PlayerId = 1, Cards = new List<int>() { 3, 4 } });

            Assert.AreEqual(2, game.GetPlayer(0).Size);
            Assert.AreEqual(2, game.winners.Length);
            Assert.AreEqual(1, game.winners[0].point);
            Assert.AreEqual(1, game.winners[0].point);
            yield break;
        }
    }
    class TestAction_Add2CS : ActionCard
    {
        protected override bool canUse(Game game, Request nowRequest, FreeUse useInfo, out NextRequest nextRequest)
        {
            nextRequest = null;
            return true;
        }
        public override async Task DoEffect(Game game, FreeUse useWay)
        {
            await UseCard.UseActionCard(game, useWay, this, async (g, r) =>
            {
                await game.ChangeSize(2, this);
            });
        }
    }
    class TestAction_Add1Inf : ActionCard
    {
        protected override bool canUse(Game game, Request nowRequest, FreeUse useInfo, out NextRequest nextRequest)
        {
            nextRequest = null;
            return true;
        }
        public override async Task DoEffect(Game game, FreeUse useWay)
        {
            await UseCard.UseActionCard(game, useWay, this, async (g, r) =>
            {
                await game.GetPlayer(useWay.PlayerId).ChangeSize(game, 1, this, Owner);
            });
        }
    }
    class TestEvent_DoubleOrZeroPlayerInf : EventCard
    {
        public override bool ForwardOnly => false;
        public override async Task Use(Game game, ChooseDirectionResponse response)
        {
            await UseCard.UseEventCard(game, response, this, effect);
        }
        Task effect(Game game, ChooseDirectionResponse response)
        {
            if (response.IfForward)
                game.Players.Find(p => p.Id == response.PlayerId).Size *= 2;
            else
                game.Players.Find(p => p.Id == response.PlayerId).Size = 0;
            return Task.CompletedTask;
        }
    }
    class TestCharacter_Empty : HeroCard
    {
    }
    class TestSkill : Skill
    {
        public override void Disable(Game game)
        {
            throw new System.NotImplementedException();
        }

        public override void Enable(Game game)
        {
            throw new System.NotImplementedException();
        }

        protected override bool canUse(Game game, Request nowRequest, FreeUse useInfo, out NextRequest nextRequest)
        {
            nextRequest = null;
            return true;
        }

        public override Task DoEffect(Game game, FreeUse useInfo)
        {
            throw new System.NotImplementedException();
        }
    }
    class TestAction_Empty : ActionCard
    {
        protected override bool canUse(Game game, Request nowRequest, FreeUse useInfo, out NextRequest nextRequest)
        {
            nextRequest = null;
            return true;
        }
        public Func<Game, FreeUse, TestAction_Empty, Task> effect { get; set; } = null;
        public override async Task DoEffect(Game game, FreeUse useWay)
        {
            await UseCard.UseActionCard(game, useWay, this, (g, r) =>
            {
                return Task.CompletedTask;
            });
        }
    }
    class TestOfficial_Empty : ThemeCard
    {
        public override void Enable(Game game)
        {
        }
        public override void Disable(Game game)
        {
        }
    }
    class TestEvent_Empty : EventCard
    {
        public override bool ForwardOnly => false;
        public override async Task Use(Game game, ChooseDirectionResponse response)
        {
            await UseCard.UseEventCard(game, response, this, (g, r) =>
            {
                return Task.CompletedTask;
            });
        }
    }
}
