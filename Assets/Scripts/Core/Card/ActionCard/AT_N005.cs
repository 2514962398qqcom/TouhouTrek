using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZMDFQ.PlayerAction;

namespace ZMDFQ.Cards
{
    /// <summary>
    /// 挂裱： 指定一名玩家，你进行一次两点点数判定，该玩家个人影响力减少判定结果的数值。
    /// 当其他玩家减少你的个人影响力时可以使用，该玩家的个人影响力减少相同的数值。
    /// </summary>
    public class AT_N005 : ActionCard
    {
        static TaskCompletionSource<bool> flag;
        [EventReigster]
        public static void Register(Game game)
        {
            //注册一个事件用于阻塞
            game.EventSystem.Register(EventEnum.AfterPlayrSizeChange, -1, args =>
            {
                flag = null;
                foreach (var player in game.Players)
                {
                    AT_N005 card = (AT_N005)player.ActionCards.Find(c => c.GetType() == typeof(AT_N005));
                    if (card != null)
                        card.afterPlayerSizeChange(args);
                }
                return Task.CompletedTask;
            });
            game.EventSystem.Register(EventEnum.AfterPlayrSizeChange, 100, args =>
            {
                if (flag != null)
                    return flag.Task;
                else
                    return Task.CompletedTask;
            });
        }
        protected override bool canUse(Game game, Request nowRequest, FreeUse useInfo, out NextRequest nextRequest)
        {
            nextRequest = null;
            switch (nowRequest)
            {
                case UseLimitCardRequest useLimitCard:
                    return Effects.UseWayResponse.CheckLimit(game, useLimitCard, useInfo, ref nextRequest, this);
                case FreeUseRequest _:
                    if (useInfo.PlayersId.Count < 1)
                    {
                        nextRequest = new HeroChooseRequest() { };
                        return false;
                    }
                    else
                        return true;
            }
            return false;
        }

        public override async Task DoEffect(Game game, FreeUse useWay)
        {
            await Effects.UseCard.UseActionCard(game, useWay, this, async (g, r) =>
            {
                await game.GetPlayer(useWay.PlayersId[0]).ChangeSize(game, -game.twoPointCheck(), this, Owner);
            });
        }
        private Task afterPlayerSizeChange(object[] args)
        {
            //这里不能用异步阻塞，直接返回任务完成
            ask(args).NoWait();
            return Task.CompletedTask;
        }
        private async Task ask(object[] args)
        {
            Game game = args[0] as Game;
            Player player = args[1] as Player;
            EventData<int> value = args[2] as EventData<int>;
            Player sourcePlayer = args[4] as Player;
            if (player == Owner && value.data < 0 && sourcePlayer != Owner)//其他玩家减少拥有者的影响力。
            {
                //启用阻塞
                flag = new TaskCompletionSource<bool>();
                if (game.Requests[game.Players.IndexOf(Owner)].Any(x => x is UseLimitCardRequest r && r.CardType == CardHelper.getId(typeof(AT_N005))))
                {
                    //说明该玩家处于另一个挂裱询问
                    return;
                }
                Log.Debug($"询问 { Owner.Name } 使用挂裱");
                UseLimitCardResponse useLimitCard = (UseLimitCardResponse)await game.WaitAnswer(new UseLimitCardRequest()
                {
                    AllPlayerRequest = true,
                    PlayerId = Owner.Id,
                    CardType = CardHelper.getId(typeof(AT_N005))
                }.SetTimeOut(game.RequestTime));
                if (useLimitCard.Used)
                {
                    Log.Debug($" { Owner.Name } 使用挂裱");
                    //响应后 取消其他玩家的询问
                    game.CancelRequests();
                    await Owner.DropActionCard(game, new List<int>() { Id }, true);//弃牌
                    //实际上的效果
                    await sourcePlayer.ChangeSize(game, value.data, this, player);
                    Log.Debug("挂裱生效");
                    //取消阻塞
                    flag.TrySetResult(true);
                }
                else
                {
                    Log.Debug($" { Owner.Name } 取消使用挂裱");
                    if (game.Requests.All(x => x.Count == 0))
                        flag.TrySetResult(true);
                }
            }
        }
    }
}