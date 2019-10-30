using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMDFQ.UI.Battle
{
    public partial class UI_Card
    {
        public Card Card;
        partial void Init()
        {
            
        }
        
        public void SetCard(Card card)
        {
            this.Card = card;

            if (card == null)
            {
                m_type.selectedIndex = 0;
                return;
            }
            m_Name.text = card.Name;
            m_Tags.text = card.Tags;
            m_Desc.text = card.Desc;
            switch (card)
            {
                case ActionCard actionCard:
                    m_type.selectedIndex = 2;
                    break;
                case EventCard eventCard:
                    m_type.selectedIndex = 1;
                    break;
                case HeroCard heroCard:
                    m_type.selectedIndex = 3;
                    break;
            }
        }
    }
}
