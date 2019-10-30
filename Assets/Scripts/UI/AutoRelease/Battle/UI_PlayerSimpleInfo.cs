/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ZMDFQ.UI.Battle
{
	public partial class UI_PlayerSimpleInfo : GButton
	{
		public UI_SmallHeroCard m_Hero;
		public UI_Group m_Group;
		public GTextField m_Name;
		public GTextField m_ActionCards;
		public GTextField m_EventCards;
		public GTextField m_Size;
		public GTextField m_Cumulative;
		public GList m_Buffs;

		public const string URL = "ui://oacz4rtmgs0sd";

		public static UI_PlayerSimpleInfo CreateInstance()
		{
			return (UI_PlayerSimpleInfo)UIPackage.CreateObject("Battle","PlayerSimpleInfo");
		}

		public UI_PlayerSimpleInfo()
		{
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			m_Hero = (UI_SmallHeroCard)this.GetChildAt(6);
			m_Group = (UI_Group)this.GetChildAt(8);
			m_Name = (GTextField)this.GetChildAt(12);
			m_ActionCards = (GTextField)this.GetChildAt(13);
			m_EventCards = (GTextField)this.GetChildAt(14);
			m_Size = (GTextField)this.GetChildAt(15);
			m_Cumulative = (GTextField)this.GetChildAt(16);
			m_Buffs = (GList)this.GetChildAt(17);
			Init();
		}
		partial void Init();
	}
}