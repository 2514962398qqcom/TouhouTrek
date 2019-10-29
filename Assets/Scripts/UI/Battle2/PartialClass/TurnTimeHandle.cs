using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace ZMDFQ.UI.Battle
{
    //用于处理倒计时
    partial class UI_Main2
    {
        [BattleUI(nameof(SetGame))]
        void turnTimeInit()
        {
            game.EventSystem.Register(EventEnum.TurnStart, 100, (x) =>
             {
                 m_ActivePlayer.SetVar("p", game.ActivePlayer.Id == self.Id ? "你" : game.ActivePlayer.Id.ToString());
                 return Task.CompletedTask;
             });
        }

        [BattleUI(nameof(OnUpdate))]
        private void timeUpdate()
        {
            if (nowRequest != null)
            {
                m_TimeBar.max = nowRequest.TimeOut;
                m_TimeBar.value = nowRequest.TimeOut - Time.time + nowRequest.StartTime;
            }
        }
    }
}
