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
        [BattleUI(nameof(Init))]
        private void chooseDirectionInit()
        {
            m_forward.onClick.Add(() =>
            {
                game.Answer(new ChooseDirectionResponse()
                {
                    CardId = self.EventCards[0].Id,
                    IfForward = true,
                    IfSet = false,
                    PlayerId = self.Id,
                });
            });
            m_backward.onClick.Add(() =>
            {
                game.Answer(new ChooseDirectionResponse()
                {
                    CardId = self.EventCards[0].Id,
                    IfForward = false,
                    IfSet = false,
                    PlayerId = self.Id,
                });
            });
            m_setcard.onClick.Add(() =>
            {
                game.Answer(new ChooseDirectionResponse()
                {
                    CardId = self.EventCards[0].Id,
                    IfForward = false,
                    IfSet = true,
                    PlayerId = self.Id,
                });
            });
        }

        //[BattleUI(nameof(onRequest))]
        private void chooseDirectionRequestHandle()
        {
            //if (nowRequest.PlayerId==self.Id&&nowRequest is ChooseDirectionRequest chooseDirectionRequest)
            {
                m_Request.selectedIndex = 5;
                m_setcard.enabled = (nowRequest as ChooseDirectionRequest).CanSet;
            }
        }
    }
}
