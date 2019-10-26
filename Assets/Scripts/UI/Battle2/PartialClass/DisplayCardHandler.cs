using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ZMDFQ.PlayerAction;

namespace ZMDFQ.UI.Battle
{
    //用于处理场上能看见的卡
    partial class UI_Main2
    {
        [BattleUI(nameof(Init), true)]
        private void DisplayCard_Init()
        {
            //这里必须先行其他注册事件，否则其他位置的手牌会出错
            //点选卡片
            m_Hand.OnCardClick.Add(evt =>
            {
                if (nowRequest is FreeUseRequest)
                    return;
                UI_Card uI_Card = evt.data as UI_Card;
                ActionCard card = uI_Card.Card as ActionCard;
                if (selectedCards.Contains(card))
                    selectedCards.Remove(card);
                else
                    selectedCards.Add(card);
                m_Hand.SetCards(self.ActionCards, selectedCards);
            });
        }

        //[BattleUI(nameof(SetGame))]
        //private void DisplayCard_setGame()
        //{
        //    //出完牌后，刷新ui
        //    game.EventSystem.Register(EventEnum.CardUsed, 100, (x) =>
        //    {
        //        //flush();
        //        return Task.CompletedTask;
        //    });
        //}

        [BattleUI(nameof(flush))]
        private void DisplayCardflush()
        {
            m_MainSize.m_size.selectedIndex = game.Size + 10;
            m_EventDeckCount.text = game.EventDeck.Count.ToString();
            m_ActionDeckCount.text = game.ActionDeck.Count.ToString();
            m_ThemeDeckCount.text = game.UsedThemeDeck.Count.ToString();
            m_EventDropDeckCount.text = game.UsedEventDeck.Count.ToString();
            m_ActionDeckCount.text = game.UsedActionDeck.Count.ToString();
            m_NowAction.SetCard(game.UsingCards.Count > 0 ? game.UsingCards[0] as ActionCard : null);
            m_NowEvent.SetCard(game.UsingCards.Count > 0 ? game.UsingCards[0] as EventCard : null);
            m_SetEvent.SetCard(self.SaveEvent);

            m_DelayAction.RemoveChildrenToPool();
            foreach (var card in game.DelayActionDeck)
            {
                var ui_card = m_DelayAction.AddItemFromPool() as UI_Card;
                ui_card.SetCard(card);
            }
            //m_NowTheme game.ActiveTheme
        }

        [BattleUI(nameof(onResponse))]
        private void DisplayCard_UseCard()
        {
            m_Hand.SetCards(self.ActionCards, selectedCards);
            flush();
        }

        [BattleUI(nameof(SetGame))]
        private void DisplayCard_NewGame()
        {
            game.EventSystem.Register(EventEnum.DrawActionCard, 100, (x) =>
             {
                 Player player = x[0] as Player;
                 if (player.Id == self.Id)
                 {
                     selectedCards.Clear();
                     m_Hand.SetCards(player.ActionCards, selectedCards);
                 }
                 return Task.CompletedTask;
             });
        }
    }
}
