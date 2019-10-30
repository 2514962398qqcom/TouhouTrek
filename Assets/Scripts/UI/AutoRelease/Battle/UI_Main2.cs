/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ZMDFQ.UI.Battle
{
	public partial class UI_Main2 : GComponent
	{
		public Controller m_ChooseHero;
		public Controller m_Request;
		public Controller m_ChangeSeat;
		public UI_PlayerSimpleInfo m_PlayerInfo0;
		public UI_PlayerSimpleInfo m_PlayerInfo1;
		public UI_PlayerSimpleInfo m_PlayerInfo2;
		public UI_PlayerSimpleInfo m_PlayerInfo3;
		public UI_PlayerSimpleInfo m_PlayerInfo4;
		public UI_PlayerSimpleInfo m_PlayerInfo5;
		public UI_PlayerSimpleInfo m_PlayerInfo6;
		public UI_PlayerSimpleInfo m_PlayerInfo7;
		public GTextField m_EventDeckCount;
		public GTextField m_ActionDeckCount;
		public UI_MainSize m_MainSize;
		public GTextField m_RoundInfo;
		public GTextField m_TurnInfo;
		public UI_Card m_NowEvent;
		public GList m_DelayAction;
		public GTextField m_ActionDropDeck;
		public GTextField m_EventDropDeckCount;
		public UI_Card m_PlayerHero;
		public UI_Log m_GameInfo;
		public UI_HandCards m_Hand;
		public UI_Card m_SetEvent;
		public GComponent m_NowTheme;
		public GTextField m_ThemeDeckCount;
		public GTextField m_ActivePlayer;
		public GProgressBar m_TimeBar;
		public UI_Card m_NowAction;
		public GTextField m_UseTip;
		public GList m_skills;
		public GButton m_useCard;
		public GButton m_Endturn;
		public GGroup m_freeUse;
		public GButton m_choose_y;
		public GButton m_choose_n;
		public GGroup m_chooseCard;
		public GList m_choiceList;
		public GGroup m_takeChoice;
		public GList m_requestList;
		public GGroup m_ChooseRequest;
		public GButton m_limitUse;
		public GButton m_limitUseCancel;
		public GButton m_forward;
		public GButton m_backward;
		public GButton m_setcard;
		public GGroup m_chooseDirection;
		public UI_ChooseHero m_HeroChooseWindow;
		public UI_ChangeSeat m_Background;

		public const string URL = "ui://oacz4rtmgs0sc";

		public static UI_Main2 CreateInstance()
		{
			return (UI_Main2)UIPackage.CreateObject("Battle","Main2");
		}

		public UI_Main2()
		{
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			m_ChooseHero = this.GetControllerAt(0);
			m_Request = this.GetControllerAt(1);
			m_ChangeSeat = this.GetControllerAt(2);
			m_PlayerInfo0 = (UI_PlayerSimpleInfo)this.GetChildAt(1);
			m_PlayerInfo1 = (UI_PlayerSimpleInfo)this.GetChildAt(2);
			m_PlayerInfo2 = (UI_PlayerSimpleInfo)this.GetChildAt(3);
			m_PlayerInfo3 = (UI_PlayerSimpleInfo)this.GetChildAt(4);
			m_PlayerInfo4 = (UI_PlayerSimpleInfo)this.GetChildAt(5);
			m_PlayerInfo5 = (UI_PlayerSimpleInfo)this.GetChildAt(6);
			m_PlayerInfo6 = (UI_PlayerSimpleInfo)this.GetChildAt(7);
			m_PlayerInfo7 = (UI_PlayerSimpleInfo)this.GetChildAt(8);
			m_EventDeckCount = (GTextField)this.GetChildAt(13);
			m_ActionDeckCount = (GTextField)this.GetChildAt(17);
			m_MainSize = (UI_MainSize)this.GetChildAt(19);
			m_RoundInfo = (GTextField)this.GetChildAt(20);
			m_TurnInfo = (GTextField)this.GetChildAt(21);
			m_NowEvent = (UI_Card)this.GetChildAt(26);
			m_DelayAction = (GList)this.GetChildAt(27);
			m_ActionDropDeck = (GTextField)this.GetChildAt(29);
			m_EventDropDeckCount = (GTextField)this.GetChildAt(32);
			m_PlayerHero = (UI_Card)this.GetChildAt(34);
			m_GameInfo = (UI_Log)this.GetChildAt(36);
			m_Hand = (UI_HandCards)this.GetChildAt(37);
			m_SetEvent = (UI_Card)this.GetChildAt(38);
			m_NowTheme = (GComponent)this.GetChildAt(41);
			m_ThemeDeckCount = (GTextField)this.GetChildAt(46);
			m_ActivePlayer = (GTextField)this.GetChildAt(48);
			m_TimeBar = (GProgressBar)this.GetChildAt(50);
			m_NowAction = (UI_Card)this.GetChildAt(51);
			m_UseTip = (GTextField)this.GetChildAt(62);
			m_skills = (GList)this.GetChildAt(63);
			m_useCard = (GButton)this.GetChildAt(65);
			m_Endturn = (GButton)this.GetChildAt(66);
			m_freeUse = (GGroup)this.GetChildAt(67);
			m_choose_y = (GButton)this.GetChildAt(69);
			m_choose_n = (GButton)this.GetChildAt(70);
			m_chooseCard = (GGroup)this.GetChildAt(71);
			m_choiceList = (GList)this.GetChildAt(73);
			m_takeChoice = (GGroup)this.GetChildAt(74);
			m_requestList = (GList)this.GetChildAt(76);
			m_ChooseRequest = (GGroup)this.GetChildAt(77);
			m_limitUse = (GButton)this.GetChildAt(78);
			m_limitUseCancel = (GButton)this.GetChildAt(79);
			m_forward = (GButton)this.GetChildAt(81);
			m_backward = (GButton)this.GetChildAt(82);
			m_setcard = (GButton)this.GetChildAt(83);
			m_chooseDirection = (GGroup)this.GetChildAt(84);
			m_HeroChooseWindow = (UI_ChooseHero)this.GetChildAt(86);
			m_Background = (UI_ChangeSeat)this.GetChildAt(87);
			Init();
		}
		partial void Init();
	}
}