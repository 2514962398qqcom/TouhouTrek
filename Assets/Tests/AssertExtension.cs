using System.Linq;
using NUnit.Framework;

using ZMDFQ;

namespace Tests
{
    static class AssertExtension
    {
        public static void checkActionUse(Game game, Player player, int cardID)
        {
            Assert.IsFalse(player.ActionCards.Any(c => c.Id == cardID));
            Assert.IsTrue(game.UsedActionDeck.Any(c => c.Id == cardID));
        }
    }
}
