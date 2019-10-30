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
                    ActionCard card = player.ActionCards.Find(c => c.configID == game.Database.getID(typeof(AT_N005)));
                    if (card != null)
                        afterPlayerSizeChange(card, args);
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
            if (nowRequest is FreeUseRequest && useInfo.PlayersId.Count < 1)
            {
                nextRequest = new HeroChooseRequest() { };
                return false;
            }
            return base.canUse(game, nowRequest, useInfo, out nextRequest);
        }
        public override async Task DoEffect(Game game, FreeUse useWay)
        {
            ActionCard source = game.GetCard(useWay.Source[0]) as ActionCard;
            await game.GetPlayer(useWay.PlayersId[0]).ChangeSize(game, -game.twoPointCheck(), source, Owner);
        }
        static Task afterPlayerSizeChange(ActionCard card, object[] args)
        {
            //这里不能用异步阻塞，直接返回任务完成
            ask(card, args).NoWait();
            return Task.CompletedTask;
        }
        static async Task ask(ActionCard card, object[] args)
        {
            Game game = args[0] as Game;
            Player player = args[1] as Player;
            EventData<int> value = args[2] as EventData<int>;
            Player sourcePlayer = args[4] as Player;
            if (player == card.Owner && value.data < 0 && sourcePlayer != card.Owner)//其他玩家减少拥有者的影响力。
            {
                //启用阻塞
                flag = new TaskCompletionSource<bool>();
                if (game.Requests[game.Players.IndexOf(card.Owner)].Any(x => x is UseLimitCardRequest r && r.CardType == game.Database.getID(typeof(AT_N005))))
                {
                    //说明该玩家处于另一个挂裱询问
                    return;
                }
                Log.Game($"询问 { card.Owner.Name } 使用挂裱");
                UseLimitCardResponse useLimitCard = (UseLimitCardResponse)await game.WaitAnswer(new UseLimitCardRequest()
                {
                    AllPlayerRequest = true,
                    PlayerId = card.Owner.Id,
                    CardType = game.Database.getID(typeof(AT_N005))
                }.SetTimeOut(game.RequestTime));
                if (useLimitCard.Used)
                {
                    Log.Game($" { card.Owner.Name } 使用挂裱");
                    //响应后 取消其他玩家的询问
                    game.CancelRequests();
                    await card.Owner.DropActionCard(game, new List<int>() { card.Id }, true);//弃牌
                    //实际上的效果
                    await sourcePlayer.ChangeSize(game, value.data, card, player);
                    //取消阻塞
                    flag.TrySetResult(true);
                }
                else
                {
                    Log.Game($" { card.Owner.Name } 取消使用挂裱");
                    if (game.Requests.All(x => x.Count == 0))
                        flag.TrySetResult(true);
                }
            }
        }
    }
}