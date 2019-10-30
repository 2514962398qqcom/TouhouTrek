/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ZMDFQ.UI.Battle
{
	public partial class UI_Log : GComponent
	{
		public GList m_Log;

		public const string URL = "ui://oacz4rtmpbiet";

		public static UI_Log CreateInstance()
		{
			return (UI_Log)UIPackage.CreateObject("Battle","Log");
		}

		public UI_Log()
		{
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			m_Log = (GList)this.GetChildAt(0);
			Init();
		}
		partial void Init();
	}
}