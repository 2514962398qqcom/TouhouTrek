using System.Collections.Generic;
using NUnit.Framework;

using ZMDFQ;
using ZMDFQ.Cards;
using ZMDFQ.PlayerAction;

namespace Tests
{
    public class CR_CM001Tests
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
                .concatRepeat(game.getCardID("东方警察"), 20)//角色
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
            game.Answer(new FreeUse() { PlayerId = 0, SkillId = SkillHelper.getId(game.Players[0].Hero.Skills[0]), Source = new List<int>() { 1 }, PlayersId = new List<int>() { 1 } });
            game.Answer(new TakeChoiceResponse() { PlayerId = 0, Index = 0 });

            Assert.AreEqual(-2, game.Players[1].Size);
            Assert.AreEqual(2, game.Size);

            game.setDice(6);
            game.Answer(new FreeUse() { PlayerId = 0, SkillId = SkillHelper.getId(game.Players[0].Hero.Skills[0]), Source = new List<int>() { 2 }, PlayersId = new List<int>() { 1 } });
            game.Answer(new TakeChoiceResponse() { PlayerId = 0, Index = 0 });

            Assert.AreEqual(-4, game.Players[1].Size);
            Assert.AreEqual(4, game.Size);
            Assert.False(game.Players[0].Hero.Skills[0].CanUse(
                game,
                null,
                new FreeUse() { PlayerId = 0, SkillId = SkillHelper.getId(game.Players[0].Hero.Skills[0]), Source = new List<int>() { 5 }, PlayersId = new List<int>() { 1 } },
                out _));
        }
    }
}
