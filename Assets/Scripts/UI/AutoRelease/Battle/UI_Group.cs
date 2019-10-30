/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ZMDFQ.UI.Battle
{
	public partial class UI_Group : GComponent
	{
		public Controller m_type;

		public const string URL = "ui://oacz4rtmdzy4o";

		public static UI_Group CreateInstance()
		{
			return (UI_Group)UIPackage.CreateObject("Battle","Group");
		}

		public UI_Group()
		{
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			m_type = this.GetControllerAt(0);
			Init();
		}
		partial void Init();
	}
}