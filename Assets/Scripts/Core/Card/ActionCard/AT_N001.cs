﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZMDFQ.PlayerAction;

namespace ZMDFQ.Cards
{
    /// <summary>
    /// 传教：社群规模±2。
    /// </summary>
    public class AT_N001 : ActionCard
    {
        public override async Task DoEffect(Game game, FreeUse useWay)
        {
            //询问玩家是加或减
            TakeChoiceResponse response = (TakeChoiceResponse)await game.WaitAnswer(new TakeChoiceRequest()
            {
                PlayerId = useWay.PlayerId,
                Infos = new List<string>() { "+2", "-2", }
            });
            //处理实际效果
            if (response.Index == 0)
                await game.ChangeSize(2, this);
            else
                await game.ChangeSize(-2, this);
        }
    }
}