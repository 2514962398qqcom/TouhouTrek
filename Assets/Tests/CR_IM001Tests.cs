using System.Collections.Generic;
using NUnit.Framework;

using ZMDFQ;
using ZMDFQ.Cards;
using ZMDFQ.PlayerAction;

namespace Tests
{
    public class CR_IM001Tests
    {
        [Test]
        public void skill1Test()
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
                Cards = new int[] { }
                .concatRepeat(0xA000, 20)//行动
                .concatRepeat(game.getCardID("冷门爱好者"), 20)//角色
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
            game.Players[0].Hero.Skills[0].AutoRequest = true;
            game.Answer(new EndFreeUseResponse() { PlayerId = 0 });
            game.Answer(new ChooseDirectionResponse() { PlayerId = 0, CardId = 61, IfForward = true });
            game.Players[0].SetSize(-5);
            game.Answer(new ChooseSomeCardResponse() { PlayerId = 0, Cards = new List<int>() { 1, 2 } });

            Assert.AreEqual(-5, game.Players[0].Size);

            game.Answer(new TakeChoiceResponse() { PlayerId = 0, Index = 1 });

            Assert.AreEqual(6, game.Players[0].ActionCards.Count);
        }
        [Test]
        public async void skill2Test()
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
                Cards = new int[] { }
                .concatRepeat(0xA000, 20)//行动
                .concatRepeat(game.getCardID("冷门爱好者"), 20)//角色
                .concatRepeat(0xF000, 20)//官作
                .concatRepeat(0xE000, 20),//事件
                firstPlayer = 0,
                shuffle = false,
                initCommunitySize = 0,
                initInfluence = 0,
                chooseCharacter = true,
                doubleCharacter = false,
            });
            game.StartGame();
            game.Answer(new ChooseHeroResponse() { PlayerId = 0, HeroId = 21 });
            game.Answer(new ChooseHeroResponse() { PlayerId = 1, HeroId = 24 });

            Assert.AreEqual(1, await game.Players[0].HandMax(game));

            await game.Players[0].Hero.FaceUp(game);

            Assert.AreEqual(4, await game.Players[0].HandMax(game));
        }
        [Test]
        public void skill3Test()
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
                    new GameOptions.PlayerInfo(),
                    new GameOptions.PlayerInfo()
                },
                Cards = new int[] { }
                .concatRepeat(0xA000, 20)//行动
                .concatRepeat(game.getCardID("冷门爱好者"), 20)//角色
                .concatRepeat(0xF000, 20)//官作
                .concatRepeat(0xE000, 20),//事件
                firstPlayer = 0,
                shuffle = false,
                initCommunitySize = 0,
                initInfluence = -5,
                chooseCharacter = true,
                doubleCharacter = false,
                endingOfficialCardCount = 1
            });
            game.StartGame();
            game.Answer(new ChooseHeroResponse() { PlayerId = 0, HeroId = 21 });
            game.Answer(new ChooseHeroResponse() { PlayerId = 1, HeroId = 24 });
            game.Answer(new EndFreeUseResponse() { PlayerId = 0 });
            game.Answer(new ChooseDirectionResponse() { PlayerId = 0, CardId = 61, IfSet = true });
            game.Answer(new ChooseSomeCardResponse() { PlayerId = 0, Cards = new List<int>() { 1, 2 } });

            game.Answer(new EndFreeUseResponse() { PlayerId = 1 });
            game.Answer(new ChooseDirectionResponse() { PlayerId = 1, CardId = 62, IfSet = true });
            game.Answer(new ChooseSomeCardResponse() { PlayerId = 1, Cards = new List<int>() { 3, 4 } });

            Assert.AreEqual(2, game.winners.Length);
            Assert.AreEqual(5, game.winners[0].point);
            Assert.AreEqual(5, game.winners[1].point);
        }
    }
}
