using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ZMDFQ
{
    using PlayerAction;
    public class RequestTimeoutManager : MonoBehaviour, IRequestManager
    {
        public Game Game { get; set; }
        public bool DoLog = false;
        List<Request> requests = new List<Request>();

        private bool destoryed = false;

        private void Update()
        {
            foreach (var request in requests.ToArray())
            {
                request.RemainTime -= Time.deltaTime;
                if (request.RemainTime < 0)
                {
                    Log.Debug($"{request.PlayerId}超时了{request.GetType().Name}询问");
                    requests.Remove(request);
                    timeoutAnswer(request);
                }
            }
        }
        Dictionary<Request, CancellationTokenSource> requestCancelDic { get; } = new Dictionary<Request, CancellationTokenSource>();
        public void Register(Request request)
        {
            if (destoryed) return;
            doLog($"注册了{request.PlayerId}的 {request.GetType().Name}事件，超时：{request.TimeOut}s ");

            requestCancelDic.Add(request, new CancellationTokenSource());
            Task waitTask = Game.TimeManager.WaitAsync(request.TimeOut, requestCancelDic[request].Token);
            Task.Run(() =>
            {
                while (true)
                {
                    if (waitTask.IsCompleted)
                    {
                        //超时
                        Log.Debug("超时：" + request);
                        timeoutAnswer(request);
                        requestCancelDic.Remove(request);
                    }
                    else if (waitTask.IsCanceled)
                    {
                        //取消询问
                        Log.Debug("取消：" + request);
                        requestCancelDic.Remove(request);
                    }
                    else if(waitTask.IsFaulted)
                    {
                        //异常
                        Log.Error(waitTask.Exception);
                        requestCancelDic.Remove(request);
                    }
                    request.RemainTime = Game.TimeManager.getRemainTime(waitTask);//更新剩余时间
                    Log.Debug("剩余时间：" + request.RemainTime);
                }
            });
            //waitTask.ContinueWith(t =>
            //{
            //    timeoutAnswer(request);
            //});

            //await Task.Run(async () =>
            //{
            //    await Game.TimeManager.WaitAsync(request.TimeOut, Game.cts.Token);
            //    timeoutAnswer(request);
            //});

            //Task.Run(async () =>
            //{
            //    await Game.TimeManager.WaitAsync(request.TimeOut, Game.cts.Token);
            //    timeoutAnswer(request);
            //});

            //await Game.TimeManager.WaitAsync(request.TimeOut, Game.cts.Token);
            //timeoutAnswer(request);

            //requests.Add(request);
        }

        public void Cancel(Request request)
        {
            doLog($"取消了{request.PlayerId}的 {request.GetType().Name}事件，剩余：{request.TimeOut}s ");
            requests.Remove(request);
        }

        void timeoutAnswer(Request request)
        {
            Player player = Game.GetPlayer(request.PlayerId);
            switch (request)
            {
                case FreeUseRequest useCardRequest:
                    Game.Answer(new EndFreeUseResponse() { PlayerId = request.PlayerId });
                    break;
                case ChooseSomeCardRequest dropCard:
                    List<ActionCard> cards = new List<ActionCard>();
                    for (int i = 0; i < dropCard.Count; i++)
                    {
                        cards.Add(player.ActionCards[i]);
                    }
                    Game.Answer(new ChooseSomeCardResponse()
                    {
                        PlayerId = request.PlayerId,
                        Cards = cards.Select(x => x.Id).ToList()
                    });
                    break;
                case ChooseHeroRequest chooseHero:
                    Game.Answer(new ChooseHeroResponse()
                    {
                        PlayerId = request.PlayerId,
                        HeroId = chooseHero.HeroIds[0],
                    });
                    break;
                case ChooseDirectionRequest chooseDirectionRequest:
                    Game.Answer(new ChooseDirectionResponse() { PlayerId = request.PlayerId, CardId = player.EventCards[0].Id, IfSet = false, IfForward = true });
                    break;
                case TakeChoiceRequest takeChoiceRequest:
                    Game.Answer(new TakeChoiceResponse() { PlayerId = request.PlayerId, Index = takeChoiceRequest.Infos.Count - 1 });
                    break;
                default:
                    Log.Warning($"超时未处理的响应类型:{request.GetType()}");
                    break;
            }
        }

        void doLog(string s)
        {
            if (DoLog) Log.Debug(s);
        }

        private void OnDestroy()
        {
            destoryed = true;
            foreach (var request in requests.ToArray())
            {
                Cancel(request);
            }
        }
    }
}
