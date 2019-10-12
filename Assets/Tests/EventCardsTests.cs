using System.Collections.Generic;

using NUnit.Framework;
using ZMDFQ;
using ZMDFQ.Cards;
using ZMDFQ.PlayerAction;

namespace Tests
{
    public class OfficialCardsTests
    {
        [Test]
        public void G013Test()
        {
            Game game = new Game();
            (game.Database as ConfigManager).AddCard(0xA000, new TestAction_Empty());
            (game.Database as ConfigManager).AddCard(0xA001, new TestAction_Add1Inf());
            (game.Database as ConfigManager).AddCard(0xA002, new TestAction_Add2CS());
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
                .concatRepeat(0xA001, 2).concatRepeat(0xA002,18)//行动
                .concatRepeat(0xC000, 20)//角色
                .concatRepeat(game.getCardID<G_013>(), 20)//官作
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
            game.Answer(new FreeUse() { PlayerId = 0, CardId = 1, Source = new List<int> { 1 } });

            Assert.AreEqual(2, game.Players[0].Size);

            game.Answer(new FreeUse() { PlayerId = 0, CardId = 5, Source = new List<int> { 5 } });

            Assert.AreEqual(4, game.Size);
        }
    }
    public class EventCardsTests
    {
        [Test]
        public void E001Test()
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
                .concatRepeat(148, 20),//事件
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
            game.ChainEventDeck.AddRange(new EventCard[] { new EV_E001(), new EV_E001(), new EV_E001() });
            game.Answer(new ChooseDirectionResponse() { PlayerId = 0, CardId = 61, IfForward = true });

            Assert.AreEqual(4, game.Size);
        }
    }
}
