/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ZMDFQ.UI.Battle
{
	public partial class UI_Card : GComponent
	{
		public Controller m_type;
		public GTextField m_Name;
		public GTextField m_Tags;
		public GTextField m_Desc;

		public const string URL = "ui://oacz4rtmdzy4h";

		public static UI_Card CreateInstance()
		{
			return (UI_Card)UIPackage.CreateObject("Battle","Card");
		}

		public UI_Card()
		{
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			m_type = this.GetControllerAt(0);
			m_Name = (GTextField)this.GetChildAt(2);
			m_Tags = (GTextField)this.GetChildAt(4);
			m_Desc = (GTextField)this.GetChildAt(5);
			Init();
		}
		
		partial void Init();
	}
}