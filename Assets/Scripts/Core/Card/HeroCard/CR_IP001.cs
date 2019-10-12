using System;
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
    //public class CR_IP001 : HeroCard
    //{
    //    public override Camp camp => Camp.indivMajor;

    //    public override List<Skill> Skills { get; } = new List<Skill>()
    //    {
    //        new CR_IP001_SK1(),
    //        new CR_IP001_SK2()
    //    };
    //}
    public class CR_IP001_SK1 : Skill
    {
        protected override bool canUse(Game game, Request nowRequest, FreeUse useInfo, out NextRequest nextRequest)
        {
            //throw new NotImplementedException();
            nextRequest = null;
            return true;
        }

        public override void Disable(Game game)
        {
            //throw new NotImplementedException();
        }

        public override Task DoEffect(Game game, FreeUse useInfo)
        {
            //throw new NotImplementedException();
            return Task.CompletedTask;
        }

        public override void Enable(Game game)
        {
            //throw new NotImplementedException();
        }
    }

    public class CR_IP001_SK2 : PassiveSkill
    {
        public override void Disable(Game game)
        {
            game.EventSystem.Remove(EventEnum.BeforePlayrSizeChange, moreChange);
        }

        public override void Enable(Game game)
        {
            game.EventSystem.Register(EventEnum.BeforePlayrSizeChange, game.Players.IndexOf(Hero.Player), moreChange);
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
