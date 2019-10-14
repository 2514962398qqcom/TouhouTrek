using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMDFQ.UI.Battle
{
    /// <summary>
    /// 处理选英雄逻辑
    /// </summary>
    public partial class UI_Main2
    {
        /// <summary>
        /// 当前的选择英雄请求
        /// </summary>
        PlayerAction.ChooseHeroRequest chooseHeroRequset;

        [BattleUI(nameof(Init))]
        private void chooseHeroInit()
        {
            for (int i = 0; i < 3; i++)
            {
                var card = (m_HeroChooseWindow.GetChild("p" + i) as UI_Card);
                card.onClick.Add(() =>
                {
                    game.Answer(new PlayerAction.ChooseHeroResponse()
                    {
                        HeroId = card.Card.Id,
                        PlayerId = self.Id,
                    });
                });
            }
        }

        [BattleUI(nameof(OnUpdate))]
        private void updateHeroChooseTime()
        {
            if (chooseHeroRequset != null)
            {
                m_HeroChooseWindow.m_time.max = chooseHeroRequset.TimeOut;
                m_HeroChooseWindow.m_time.value = chooseHeroRequset.RemainTime;
            }
        }

        //[BattleUI(nameof(onRequest))]
        private void chooseHeroRequestHandle()
        {
            //if ((nowRequest.PlayerId == self.Id) && nowRequest is PlayerAction.ChooseHeroRequest chooseHeroRequset)
            {
                this.chooseHeroRequset = nowRequest as PlayerAction.ChooseHeroRequest;
                m_ChooseHero.selectedIndex = 1;
                for (int i = 0; i < chooseHeroRequset.HeroIds.Count; i++)
                {
                    int id = chooseHeroRequset.HeroIds[i];
                    Log.Debug($"可选择英雄id:{id}");
                    (m_HeroChooseWindow.GetChild("p" + i) as UI_Card).SetCard(game.GetCard(id));
                }
            }
        }

        [BattleUI(nameof(onResponse))]
        private void chooseHeroReponse()
        {
            if (nowResponse.PlayerId==self.Id&&nowResponse is PlayerAction.ChooseHeroResponse chooseHeroResponse)
            {
                m_PlayerHero.SetCard(game.GetCard(chooseHeroResponse.HeroId));
                this.chooseHeroRequset = null;
                m_ChooseHero.selectedIndex = 0;

                m_skills.RemoveChildrenToPool();
                foreach (var skill in self.Hero.Skills)
                {
                    var ui_skill = m_skills.AddItemFromPool().asButton;
                    ui_skill.changeStateOnClick = false;
                    ui_skill.data = skill;
                    ui_skill.text = skill.Name;
                    ui_skill.enabled = !(skill is PassiveSkill);//技能的可使用性，要改
                }
            }
        }
    }
}
