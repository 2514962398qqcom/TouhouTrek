using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZMDFQ.PlayerAction;

namespace ZMDFQ.Cards
{
    /// <summary>
    /// ���ѣ� ָ��һ����ң������һ����������ж�������Ҹ���Ӱ���������ж��������ֵ��
    /// ��������Ҽ�����ĸ���Ӱ����ʱ����ʹ�ã�����ҵĸ���Ӱ����������ͬ����ֵ��
    /// </summary>
    public class AT_N005 : ActionCard
    {
        static TaskCompletionSource<bool> flag;
        [EventReigster]
        public static void Register(Game game)
        {
            //ע��һ���¼���������
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
            //���ﲻ�����첽������ֱ�ӷ����������
            ask(card, args).NoWait();
            return Task.CompletedTask;
        }
        static async Task ask(ActionCard card, object[] args)
        {
            Game game = args[0] as Game;
            Player player = args[1] as Player;
            EventData<int> value = args[2] as EventData<int>;
            Player sourcePlayer = args[4] as Player;
            if (player == card.Owner && value.data < 0 && sourcePlayer != card.Owner)//������Ҽ���ӵ���ߵ�Ӱ������
            {
                //��������
                flag = new TaskCompletionSource<bool>();
                if (game.Requests[game.Players.IndexOf(card.Owner)].Any(x => x is UseLimitCardRequest r && r.CardType == game.Database.getID(typeof(AT_N005))))
                {
                    //˵������Ҵ�����һ������ѯ��
                    return;
                }
                Log.Game($"ѯ�� { card.Owner.Name } ʹ�ù���");
                UseLimitCardResponse useLimitCard = (UseLimitCardResponse)await game.WaitAnswer(new UseLimitCardRequest()
                {
                    AllPlayerRequest = true,
                    PlayerId = card.Owner.Id,
                    CardType = game.Database.getID(typeof(AT_N005))
                }.SetTimeOut(game.RequestTime));
                if (useLimitCard.Used)
                {
                    Log.Game($" { card.Owner.Name } ʹ�ù���");
                    //��Ӧ�� ȡ��������ҵ�ѯ��
                    game.CancelRequests();
                    await card.Owner.DropActionCard(game, new List<int>() { card.Id }, true);//����
                    //ʵ���ϵ�Ч��
                    await sourcePlayer.ChangeSize(game, value.data, card, player);
                    //ȡ������
                    flag.TrySetResult(true);
                }
                else
                {
                    Log.Game($" { card.Owner.Name } ȡ��ʹ�ù���");
                    if (game.Requests.All(x => x.Count == 0))
                        flag.TrySetResult(true);
                }
            }
        }
    }
}