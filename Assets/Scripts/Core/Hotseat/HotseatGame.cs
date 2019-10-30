using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZMDFQ.PlayerAction;

//using UnityEngine;

namespace ZMDFQ.Hotseat
{
    class HotseatGame : Game
    {
        public override async Task<Task<Response>[]> waitAnswerAll(List<Player> players, Func<Player, Task<Response>> callback)
        {
            Task<Response>[] responses = new Task<Response>[players.Count];
            for (int i = 0; i < players.Count; i++)
            {
                responses[i] = callback(players[i]);
                await responses[i];
            }
            return responses;
        }
    }
}