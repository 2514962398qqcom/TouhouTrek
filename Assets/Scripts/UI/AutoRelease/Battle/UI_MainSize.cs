/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ZMDFQ.UI.Battle
{
	public partial class UI_MainSize : GComponent
	{
		public Controller m_size;

		public const string URL = "ui://oacz4rtmdzy4e";

		public static UI_MainSize CreateInstance()
		{
			return (UI_MainSize)UIPackage.CreateObject("Battle","MainSize");
		}

		public UI_MainSize()
		{
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			m_size = this.GetControllerAt(0);
			Init();
		}
		partial void Init();
	}
}