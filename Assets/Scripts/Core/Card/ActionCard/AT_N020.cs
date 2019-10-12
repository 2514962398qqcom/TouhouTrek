using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZMDFQ.PlayerAction;

namespace ZMDFQ.Cards
{
    /// <summary>
    /// 墨菲定理：任意玩家决定事件牌发生方向时使用，你的个人影响力-1，事件牌的发生方向逆转(不影响扣置发生的事件)
    /// </summary>
    public class AT_N020 : ActionCard
    {
        static TaskCompletionSource<bool> flag;
        [EventReigster]
        public static void Register(Game game)
        {
            //注册一个事件用于阻塞
            game.EventSystem.Register(EventEnum.changeEventDirection, 0, (x) =>
            {
                foreach (var player in game.Players)
                {
                    AT_N020 card = (AT_N020)player.ActionCards.Find(c => c.GetType() == typeof(AT_N020));
                    if (card!=null)
                    {
                        card.effect(x);
                    }
                }
                return Task.CompletedTask;
            });

            game.EventSystem.Register(EventEnum.changeEventDirection, 100, (x) =>
            {
                if (flag != null)
                    return flag.Task;
                else
                    return Task.CompletedTask;
            });
        }

        public override Task DoEffect(Game game, FreeUse useWay)
        {
            return Task.CompletedTask;
        }

        protected override bool canUse(Game game, Request nowRequest, FreeUse useInfo, out NextRequest nextRequest)
        {
            nextRequest = null;
            switch (nowRequest)
            {
                case UseLimitCardRequest useLimitCard:
                    return Effects.UseWayResponse.CheckLimit(game, useLimitCard, useInfo, ref nextRequest, this);
                case FreeUseRequest freeUse:
                    nextRequest = new NextRequest()
                    {
                        RequestInfo = "无法主动打出"
                    };
                    break;
            }
            
            return false;
        }

        internal override void OnDraw(Game game, Player player)
        {
            //game.EventSystem.Register(EventEnum.changeEventDirection, game.GetSeat(player), effect, 0, "使用墨菲定律");
        }

        internal override void OnDrop(Game game, Player player)
        {
            //game.EventSystem.Remove(EventEnum.changeEventDirection, effect);
        }

        private Task effect(object[] args)
        {
            Game game = args[0] as Game;
            //这里不能用异步阻塞，直接返回任务完成
            doeffect(args, game).NoWait();
            return Task.CompletedTask;
        }

        private async Task doeffect(object[] args,Game game)
        {
            ChooseDirectionResponse response = args[1] as ChooseDirectionResponse;
            if (!response.IfSet)
            {
                //启用阻塞
                flag = new TaskCompletionSource<bool>();
                if (game.Requests[ game.Players.IndexOf(Owner)] != null)
                {
                    //说明该玩家处于另一个询问，多半是另一张墨菲定律
                    return;
                }
                Log.Debug($"询问{Owner.Name}使用定律");
                UseLimitCardResponse response1 = (UseLimitCardResponse)await game.WaitAnswer(new UseLimitCardRequest()
                {
                    AllPlayerRequest = true,
                    PlayerId = Owner.Id,
                    CardType = CardHelper.getId(typeof(AT_N020))
                }.SetTimeOut(game.RequestTime));
                if (response1.Used)
                {
                    Log.Debug($"{Owner.Name}取消使用定律");
                    //响应后 取消其他玩家的询问
                    game.CancelRequests();
                    //这段会有问题，丢卡后卡的owner变成null,个人影响力变化会找不到来源玩家(理论上是自己)
                    Player owner = Owner;
                    await Owner.DropActionCard(game, new List<int>() { this.Id }, true);
                    await owner.ChangeSize(game, -1, this, Owner);
                    //把方向置为反向
                    response.IfForward = !response.IfForward;
                    //取消阻塞
                    flag.TrySetResult(true);
                }
                else
                {
                    Log.Debug($"{Owner.Name}取消使用墨菲定律");
                    if (game.Requests.All(x => x == null))
                    {
                        flag.TrySetResult(true);
                    }
                }
            }
        }
    }
}
