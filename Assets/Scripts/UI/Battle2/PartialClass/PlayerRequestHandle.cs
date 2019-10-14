using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FairyGUI;

namespace ZMDFQ.UI.Battle
{
    using PlayerAction;
    public partial class UI_Main2
    {
        [BattleUI(nameof(Init))]
        private void requestHandleInit()
        {
            m_requestList.onClickItem.Add(x =>
            {
                GObject item = x.data as GObject;
                int index = m_requestList.GetChildIndex(item);
                nowRequest = game.Requests[game.Players.IndexOf(self)][index];
            });
        }
        [BattleUI(nameof(OnUpdate))]
        private void requestHandleUpdate()
        {
            int requestCount = game.Requests[game.Players.IndexOf(self)].Count;
            if (requestCount == 0)
            {
                nowRequest = null;
                m_Request.selectedIndex = 0;
            }
            else if (requestCount > 1)
            {
                if (nowRequest == null)
                {
                    m_Request.selectedIndex = 6;
                    m_requestList.RemoveChildrenToPool();
                    foreach (var requset in game.Requests[game.Players.IndexOf(self)])
                    {
                        var btn = m_requestList.AddItemFromPool().asButton;
                        btn.text = requset.RequsetInfo;
                    }
                    m_requestList.ResizeToFit(m_requestList.numChildren);
                }
            }
            else
            {
                nowRequest = game.Requests[game.Players.IndexOf(self)][0];
            }
            if (nowRequest == null) return;
            switch (nowRequest)
            {
                case ChooseHeroRequest chooseHeroRequest:
                    this.chooseHeroRequestHandle();
                    break;
                case FreeUseRequest freeUseRequest:
                    freeUseRequestHandle();
                    break;
                case TakeChoiceRequest takeChoiceRequest:
                    m_Request.selectedIndex = 2;
                    m_choiceList.RemoveChildrenToPool();
                    for (int i = 0; i < takeChoiceRequest.Infos.Count; i++)
                    {
                        int k = i;
                        var g = m_choiceList.AddItemFromPool();
                        g.text = takeChoiceRequest.Infos[i];
                        g.onClick.Set(() => game.Answer(new TakeChoiceResponse()
                        {
                            Index = k,
                            PlayerId = self.Id,
                        }));
                    }
                    m_choiceList.ResizeToFit(m_choiceList.numItems);
                    break;
                case ChooseSomeCardRequest chooseSomeCardRequest:
                    chooseCardsRequestHandle();
                    break;
                case ChooseDirectionRequest chooseDirectionRequest:
                    chooseDirectionRequestHandle();
                    break;
                case UseLimitCardRequest useLimitCardRequest:
                    m_Request.selectedIndex = 4;
                    break;
                default:
                    Log.Warning($"未处理的request:{nowRequest.GetType().Name}");
                    break;
            }
        }

        //[BattleUI(nameof(onRequest))]
        private void handleRequest()
        {
            if (nowRequest.PlayerId==self.Id)
                switch (nowRequest)
                {
                    case ChooseHeroRequest chooseHeroRequest:
                        //由ChooseHero模块处理
                        break;
                    case FreeUseRequest freeUseRequest:
                        //由FreeUseHandle处理
                        break;
                    case TakeChoiceRequest takeChoiceRequest:
                        m_Request.selectedIndex = 2;
                        m_choiceList.RemoveChildrenToPool();
                        for (int i = 0; i < takeChoiceRequest.Infos.Count; i++)
                        {
                            int k = i;
                            var g= m_choiceList.AddItemFromPool();
                            g.text = takeChoiceRequest.Infos[i];
                            g.onClick.Set(() => game.Answer(new TakeChoiceResponse()
                            {
                                Index = k,
                                PlayerId = self.Id,
                            }));
                        }
                        m_choiceList.ResizeToFit(m_choiceList.numItems);
                        break;
                    case ChooseSomeCardRequest chooseSomeCardRequest:
                       
                        break;
                    case ChooseDirectionRequest chooseDirectionRequest:
                        
                        break;
                    case UseLimitCardRequest useLimitCardRequest:
                        m_Request.selectedIndex = 4;
                        break;
                    default:
                        Log.Warning($"未处理的request:{nowRequest.GetType().Name}");
                        break;
                }
        }
    }
}
