/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ZMDFQ.UI.Battle
{
	public partial class UI_ChooseHero : GComponent
	{
		public UI_Card m_p0;
		public UI_Card m_p1;
		public UI_Card m_p2;
		public GProgressBar m_time;

		public const string URL = "ui://oacz4rtmsqtfq";

		public static UI_ChooseHero CreateInstance()
		{
			return (UI_ChooseHero)UIPackage.CreateObject("Battle","ChooseHero");
		}

		public UI_ChooseHero()
		{
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			m_p0 = (UI_Card)this.GetChildAt(1);
			m_p1 = (UI_Card)this.GetChildAt(2);
			m_p2 = (UI_Card)this.GetChildAt(3);
			m_time = (GProgressBar)this.GetChildAt(5);
			Init();
		}
		partial void Init();
	}
}