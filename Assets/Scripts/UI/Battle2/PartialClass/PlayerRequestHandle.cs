using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMDFQ.UI.Battle
{
    using PlayerAction;
    public partial class UI_Main2
    {
        [BattleUI(nameof(onRequest))]
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
