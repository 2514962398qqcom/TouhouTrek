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
        private void UseLimitInit()
        {
            m_limitUse.onClick.Add(() =>
            {
                var useinfo = getFreeUseInfo<UseLimitCardResponse>();
                useinfo.Used = true;
                game.Answer(useinfo);
            });
            m_limitUseCancel.onClick.Add(() =>
            {
                game.Answer(new UseLimitCardResponse() { PlayerId = self.Id, Used = false });
            });
            m_Hand.OnCardClick.Add(checkLimitUseAble);
            m_skills.onClickItem.Add(checkLimitUseAble);
        }

        [BattleUI(nameof(onRequest))]
        private void UseLimitRequestHandle()
        {
            //Log.Debug(MongoHelper.ToJson(nowRequest));
            if (nowRequest.PlayerId == self.Id && nowRequest is UseLimitCardRequest useLimitCardRequest)
            {
                m_UseTip.text = "是否使用" + CardHelper.getType(useLimitCardRequest.CardType).Name;
                m_Request.selectedIndex = 4;
            }
        }

        private void checkLimitUseAble()
        {
            if (nowRequest == null || nowRequest.PlayerId != self.Id || !(nowRequest is UseLimitCardRequest)) return;
            if (selectedSkill != null)
            {
                NextRequest nextRequest;
                if (selectedSkill.CanUse(game, nowRequest, getFreeUseInfo<UseLimitCardResponse>(), out nextRequest))
                {
                    m_limitUse.enabled = true;
                    m_UseTip.text = "";//应该是确认是否使用
                }
                else
                {
                    m_limitUse.enabled = false;
                    m_UseTip.text = nextRequest.RequestInfo;
                }
            }
            else if (selectedCards.Count == 1)
            {
                NextRequest nextRequest;
                if (selectedCards[0].CanUse(game, nowRequest, getFreeUseInfo<UseLimitCardResponse>(), out nextRequest))
                {
                    m_limitUse.enabled = true;
                    m_UseTip.text = "";//应该是确认是否使用
                }
                else
                {
                    m_limitUse.enabled = false;
                    if (nextRequest != null)
                        m_UseTip.text = nextRequest.RequestInfo;
                }
            }
            else
            {
                m_limitUse.enabled = false;
                m_UseTip.text = "";
            }
        }
    }
}
