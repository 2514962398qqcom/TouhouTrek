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
        private void ChooseCardInit()
        {
            m_choose_y.enabled = false;
            m_choose_y.onClick.Add(() =>
            {
                game.Answer(new ChooseSomeCardResponse()
                {
                    PlayerId = self.Id,
                    Cards = selectedCards.Select(x => x.Id).ToList(),
                });
            });
            m_choose_n.onClick.Add(() =>
            {
                game.Answer(new ChooseSomeCardResponse()
                {
                    PlayerId = self.Id,
                    Cards = new List<int>(),
                });
            });
            m_Hand.OnCardClick.Add((evt) =>
            {
                if (!(nowRequest is ChooseSomeCardRequest chooseSomeCardRequest)) return;
                m_choose_y.enabled = chooseSomeCardRequest.EnoughOnly ? selectedCards.Count == chooseSomeCardRequest.Count : selectedCards.Count != 0;
            });
        }

        //[BattleUI(nameof(onRequest))]
        private void chooseCardsRequestHandle()
        {
            //if (nowRequest.PlayerId == self.Id && nowRequest is ChooseSomeCardRequest chooseSomeCardRequest)
            {
                m_Request.selectedIndex = 3;
                //selectedCards.Clear();
                //m_Hand.SetCards(self.ActionCards, selectedCards);
                m_choose_n.visible = !(nowRequest as ChooseSomeCardRequest).EnoughOnly;
            }
        }
    }
}
