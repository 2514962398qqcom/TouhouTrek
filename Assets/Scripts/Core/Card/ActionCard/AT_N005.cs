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
            //���ﲻ�����첽������ֱ�ӷ����������
            ask(args).NoWait();
            return Task.CompletedTask;
        }
        private async Task ask(object[] args)
        {
            Game game = args[0] as Game;
            Player player = args[1] as Player;
            EventData<int> value = args[2] as EventData<int>;
            Player sourcePlayer = args[4] as Player;
            if (player == Owner && value.data < 0 && sourcePlayer != Owner)//������Ҽ���ӵ���ߵ�Ӱ������
            {
                //��������
                flag = new TaskCompletionSource<bool>();
                if (game.Requests[game.Players.IndexOf(Owner)].Any(x => x is UseLimitCardRequest r && r.CardType == CardHelper.getId(typeof(AT_N005))))
                {
                    //˵������Ҵ�����һ������ѯ��
                    return;
                }
                Log.Debug($"ѯ�� { Owner.Name } ʹ�ù���");
                UseLimitCardResponse useLimitCard = (UseLimitCardResponse)await game.WaitAnswer(new UseLimitCardRequest()
                {
                    AllPlayerRequest = true,
                    PlayerId = Owner.Id,
                    CardType = CardHelper.getId(typeof(AT_N005))
                }.SetTimeOut(game.RequestTime));
                if (useLimitCard.Used)
                {
                    Log.Debug($" { Owner.Name } ʹ�ù���");
                    //��Ӧ�� ȡ��������ҵ�ѯ��
                    game.CancelRequests();
                    await Owner.DropActionCard(game, new List<int>() { Id }, true);//����
                    //ʵ���ϵ�Ч��
                    await sourcePlayer.ChangeSize(game, value.data, this, player);
                    Log.Debug("������Ч");
                    //ȡ������
                    flag.TrySetResult(true);
                }
                else
                {
                    Log.Debug($" { Owner.Name } ȡ��ʹ�ù���");
                    if (game.Requests.All(x => x.Count == 0))
                        flag.TrySetResult(true);
                }
            }
        }
    }
}