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
        private void freeUseInit()
        {
            m_useCard.enabled = false;
            m_useCard.onClick.Add(() =>
            {
                var useinfo = getFreeUseInfo<FreeUse>();
                resetSelection();
                game.Answer(useinfo);
            });
            m_Endturn.onClick.Add(() =>
            {
                game.Answer(new EndFreeUseResponse()
                {
                    PlayerId = self.Id,
                });
            });
            m_Hand.OnCardClick.Add(freeUse_CardClick);
            m_skills.onClickItem.Add(checkFreeUseAble);
            for (int i = 0; i < 8; i++)
            {
                playersSimpleInfo[i].onClick.Add(freeUse_HeroClick);
            }
        }
        /// <summary>
        /// 重置选中玩家，卡片和技能等等。
        /// </summary>
        private void resetSelection()
        {
            selectedPlayers.Clear();
            flushSelectPlayer();//清空选中玩家

            selectedCards.Clear();
            m_Hand.SetCards(self.ActionCards, selectedCards);

            selectedSkill = null;
            flushSkills();
        }

        //[BattleUI(nameof(flush))]
        //private void freeUseFlush()
        //{

        //}

        //[BattleUI(nameof(onRequest))]
        private void freeUseRequestHandle()
        {
            m_Request.selectedIndex = 1;
            //自由出牌
            //if (nowRequest.PlayerId == self.Id && nowRequest is FreeUseRequest && !(nowRequest is UseLimitCardRequest))
            //{
            //    m_Request.selectedIndex = 1;
            //    selectedCards.Clear();
            //    m_Hand.SetCards(self.ActionCards, selectedCards);
            //}
        }

        void flushSkills()
        {
            foreach (GButton ui_skill in m_skills.GetChildren())
            {
                ui_skill.selected = selectedSkill == ui_skill.data;
            }
        }

        void freeUse_HeroClick(EventContext evt)
        {
            UI_PlayerSimpleInfo playerSimpleInfo = evt.sender as UI_PlayerSimpleInfo;
            if (selectedCards[0].isValidTarget(game, getFreeUseInfo<FreeUse>(), playerSimpleInfo.Player, out string invalidInfo))//是否是可以选中的目标？
            {
                selectedPlayers.Clear();
                selectedPlayers.Add(playerSimpleInfo.Player);
                //TODO:支持选择多个玩家
                //if (selectedPlayers.Contains(playerSimpleInfo.Player))
                //    selectedPlayers.Remove(playerSimpleInfo.Player);
                //else
                //    selectedPlayers.Add(playerSimpleInfo.Player);
                flushSelectPlayer();
                checkFreeUseAble();
            }
            else
            {
                selectedPlayers.Clear();
                flushSelectPlayer();
                checkFreeUseAble();
                if (!string.IsNullOrEmpty(invalidInfo))
                    m_UseTip.text = invalidInfo;
            }
        }

        void freeUse_CardClick(EventContext evt)
        {
            if (nowRequest is FreeUseRequest request && request.PlayerId == self.Id)//是自己在出牌
            {
                ActionCard card = (evt.data as UI_Card).Card as ActionCard;
                selectedCards = new List<ActionCard>() { card };
                m_Hand.SetCards(self.ActionCards, selectedCards);
                checkFreeUseAble();
                //TODO:支持选择作为Cost的其他卡片
            }
        }

        private void checkFreeUseAble()
        {
            if (nowRequest == null || nowRequest.PlayerId != self.Id || !(nowRequest is FreeUseRequest) || nowRequest is UseLimitCardRequest)
                return;
            if (selectedSkill != null)
            {
                NextRequest nextRequest;
                if (selectedSkill.CanUse(game, nowRequest, getFreeUseInfo<FreeUse>(), out nextRequest))
                {
                    m_useCard.enabled = true;
                    m_UseTip.text = "";//应该是确认是否使用
                }
                else
                {
                    m_useCard.enabled = false;
                    if (nextRequest != null)
                        m_UseTip.text = nextRequest.RequestInfo;
                }
            }
            else if (selectedCards.Count == 1)
            {
                NextRequest nextRequest;
                if (selectedCards[0].CanUse(game, nowRequest, getFreeUseInfo<FreeUse>(), out nextRequest))
                {
                    m_useCard.enabled = true;
                    m_UseTip.text = "";//应该是确认是否使用
                }
                else
                {
                    m_useCard.enabled = false;
                    if (nextRequest != null)
                        m_UseTip.text = nextRequest.RequestInfo;
                }
            }
            else
            {
                m_useCard.enabled = false;
                m_UseTip.text = "";
            }
        }

        T getFreeUseInfo<T>() where T : FreeUse, new()
        {
            return new T()
            {
                PlayerId = self.Id,
                Source = selectedCards.Select(x => x.Id).ToList(),
                CardId = selectedSkill != null || selectedCards.Count != 1 ? -1 : selectedCards[0].Id,
                SkillId = selectedSkill == null ? -1 : SkillHelper.getId(selectedSkill),
                PlayersId = selectedPlayers.Select(x => x.Id).ToList(),
            };
        }
    }
}
