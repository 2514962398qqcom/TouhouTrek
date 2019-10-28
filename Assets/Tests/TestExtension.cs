using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using ZMDFQ;
using ZMDFQ.PlayerAction;

namespace Tests
{
    static class TestExtension
    {
        public static int getCardID<T>(this Game game) where T : Card
        {
            return (game.Database as ConfigManager).Cards.First(p => p.Value is T).Key;
        }
        public static int getCardID(this Game game, string name)
        {
            return (game.Database as ConfigManager).Cards.First(p => p.Value.Name == name).Key;
        }
        public static IEnumerable<T> concatRepeat<T>(this IEnumerable<T> first, T value, int count)
        {
            return first.Concat(Enumerable.Repeat(value, count));
        }
        public static int findHand(this Player player, int configID)
        {
            return player.ActionCards.Find(c => c.trueConfigID == configID).Id;
        }
        public static int findHand<T>(this Player player) where T : ActionCard
        {
            return player.ActionCards.Find(c => c is T).Id;
        }
        public static void useAction(this Game game, int playerID, int cardID)
        {
            game.Answer(new FreeUse() { PlayerId = playerID, CardId = cardID, Source = new List<int>() { cardID } });
        }
        public static void endAction(this Game game, int playerID)
        {
            game.Answer(new EndFreeUseResponse() { PlayerId = playerID });
        }
        public static void useEvent(this Game game, int playerID, bool isForward = true, bool isSet = false)
        {
            game.Answer(new ChooseDirectionResponse() { PlayerId = playerID, CardId = game.GetPlayer(playerID).EventCards[0].Id, IfForward = isForward, IfSet = isSet });
        }
        public static void tryDiscard(this Game game, int playerID, List<int> cards = null)
        {
            if (game.getRequest(game.GetPlayer(playerID)) is ChooseSomeCardRequest)
            {
                if (cards == null)
                {
                    Player player = game.GetPlayer(playerID);
                    if (player.ActionCards.Count > player.HandMax(game).Result)
                        game.Answer(new ChooseSomeCardResponse() { PlayerId = playerID, Cards = new List<int>(player.ActionCards.Take(player.ActionCards.Count - player.HandMax(game).Result).Select(c => c.Id)) });
                }
                else
                    game.Answer(new ChooseSomeCardResponse() { PlayerId = playerID, Cards = cards });
            }
        }
        public static void printRequests(this Game game)
        {
            Debug.Log("所有玩家Request：");
            for (int i = 0; i < game.Requests.Length; i++)
            {
                foreach (Request request in game.Requests[i])
                {
                    Debug.Log("玩家" + game.Players[i].Id + "：" + request);
                }
            }
        }
    }
}
