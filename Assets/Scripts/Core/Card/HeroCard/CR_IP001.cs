﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZMDFQ.PlayerAction;

namespace ZMDFQ.Cards
{
    /// <summary>
    /// 社团主催
    /// 你可以把任意手牌当约稿使用，一回合一次
    /// 你个人影响力因为创作或者约稿增加时，摸一张行动牌
    /// </summary>
    public class CR_IP001 : HeroCard
    {
        public override Camp camp => Camp.indivMajor;

        public override List<Skill> Skills => throw new NotImplementedException();
    }
    public class CR_IP001_SK1 : Skill
    {
        public override bool CanUse(Game game, Request nowRequest, FreeUse useInfo, out UseRequest nextRequest)
        {
            throw new NotImplementedException();
        }

        public override void Disable(Game game)
        {
            throw new NotImplementedException();
        }

        public override Task DoEffect(Game game, FreeUse useInfo)
        {
            throw new NotImplementedException();
        }

        public override void Enable(Game game)
        {
            throw new NotImplementedException();
        }
    }

    public class CR_IP001_SK2 : PassiveSkill
    {
        public override void Disable(Game game)
        {
            game.EventSystem.Remove(EventEnum.OnPlayrSizeChange, moreChange);
        }

        public override void Enable(Game game)
        {
            game.EventSystem.Register(EventEnum.OnPlayrSizeChange, moreChange);
        }

        private Task moreChange(object[] param)
        {
            Player player = param[1] as Player;
            var sizeChange = param[2] as EventData<int>;
            if (player.Hero == Hero && sizeChange.data > 0)
            {
                sizeChange.data++;
            }
            return Task.CompletedTask;
        }
    }
}