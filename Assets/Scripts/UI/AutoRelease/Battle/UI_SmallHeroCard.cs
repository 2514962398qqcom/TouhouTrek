/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ZMDFQ.UI.Battle
{
	public partial class UI_SmallHeroCard : GComponent
	{
		public GTextField m_Name;

		public const string URL = "ui://oacz4rtmdzy4n";

		public static UI_SmallHeroCard CreateInstance()
		{
			return (UI_SmallHeroCard)UIPackage.CreateObject("Battle","SmallHeroCard");
		}

		public UI_SmallHeroCard()
		{
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			m_Name = (GTextField)this.GetChildAt(2);
			Init();
		}
		partial void Init();
	}
}