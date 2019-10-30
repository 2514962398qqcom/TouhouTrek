/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ZMDFQ.UI.Battle
{
	public partial class UI_ChangeSeat : GComponent
	{
		public GGraph m_Background;
		public GTextField m_Info;
		public GButton m_Confirm;

		public const string URL = "ui://oacz4rtmsapys";

		public static UI_ChangeSeat CreateInstance()
		{
			return (UI_ChangeSeat)UIPackage.CreateObject("Battle","ChangeSeat");
		}

		public UI_ChangeSeat()
		{
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			m_Background = (GGraph)this.GetChildAt(0);
			m_Info = (GTextField)this.GetChildAt(1);
			m_Confirm = (GButton)this.GetChildAt(2);
			Init();
		}
		partial void Init();
	}
}