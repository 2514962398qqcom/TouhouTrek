using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZMDFQ.PlayerAction;

//using UnityEngine;

namespace ZMDFQ.Hotseat
{
    class HotseatGame : Game
    {
        public override async Task whenAll(IEnumerable<Task> tasks)
        {
            foreach (Task task in tasks)
            {
                await task;
            }
        }
        public override async Task<Task<Response>[]> waitAnswerAll(List<Player> players, Func<Player, Task<Response>> selector, Func<Task<Response>, Task> callback = null)
        {
            Task<Response>[] responses = new Task<Response>[players.Count];
            for (int i = 0; i < players.Count; i++)
            {
                responses[i] = selector(players[i]);
                await responses[i];
                if (callback != null)
                    await callback(responses[i]);
            }
            return responses;
        }
    }
}