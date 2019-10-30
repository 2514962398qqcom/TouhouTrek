/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ZMDFQ.UI.Battle
{
	public partial class UI_HandCards : GComponent
	{
		public GGraph m_back;

		public const string URL = "ui://oacz4rtmdzy4k";

		public static UI_HandCards CreateInstance()
		{
			return (UI_HandCards)UIPackage.CreateObject("Battle","HandCards");
		}

		public UI_HandCards()
		{
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			m_back = (GGraph)this.GetChildAt(0);
			Init();
		}
		partial void Init();
	}
}