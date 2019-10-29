using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FairyGUI;
using ZMDFQ.UI;
using ZMDFQ.PlayerAction;

namespace ZMDFQ
{
    public class UI_Battle:MonoBehaviour
    {
        public GComponent _main;
        public Game Game;
        public List<UI_Player> SelectedPlayers = new List<UI_Player>();
        public List<ActionCard> SelectedCards = new List<ActionCard>();
        public UI_Hands HandCards;
        public Player Self;

        private Request nowRequest;
        private NextRequest nowUseWay;
        private Skill nowSkill;

        private void Awake()
        {
            FairyGUIRegister.Register();
        }
        private void Start()
        {
            _main = GetComponent<UIPanel>().ui;
            HandCards = _main.GetChild("HandCards") as UI_Hands;

            _main.GetChild("n17").onClick.Add(takeUse);

            _main.GetChild("EndTurn").onClick.Add(() => Game.Answer(new EndFreeUseResponse() { PlayerId = Self.Id }));

            _main.GetChild("n30").onClick.Add(useForward);

            _main.GetChild("n31").onClick.Add(useBackward);

            _main.GetChild("n32").onClick.Add(saveEventCard);

            _main.GetChild("n33").onClick.Add(() =>
            {
                nowRequest = null;
                Game.Answer(new ChooseSomeCardResponse() { PlayerId = Self.Id, Cards = SelectedCards.Select(x=>x.Id).ToList() });
                FlushView(Game);
            });

            _main.GetChild("n34").asList.onClickItem.Add((x) =>
            {
                var takeChoiceRequest = _main.GetChild("n34").data as TakeChoiceRequest;
                int index = _main.GetChild("n34").asList.GetChildIndex(x.data as GButton);
                Game.Answer(new TakeChoiceResponse() { PlayerId = takeChoiceRequest.PlayerId, Index = index });
                FlushView(Game);
            });

            _main.GetChild("Skills").asList.onClickItem.Add((x) =>
            {
                var ui_skill = x.data as GButton;
                Skill skill = ui_skill.data as Skill;
                if (nowSkill == null || nowSkill != skill) nowSkill = skill;
                else nowSkill = null;
                if (nowSkill != null) nowSkill.CanUse(Game, nowRequest, getNowUseInfo(), out nowUseWay);
                FlushView(Game);
            });

            for (int i = 0; i < 3; i++)
            {
                var ui_hero = _main.GetChild("n28").asCom.GetChild("hero" + i);
                ui_hero.onClick.Add((x) =>
                {
                    nowRequest = null;
                    Game.Answer(new ChooseHeroResponse()
                    {
                        PlayerId = Self.Id,
                        HeroId = int.Parse(ui_hero.text),
                    });
                    _main.GetController("ChooseHero").selectedIndex = 0;
                });
            }

            Game = new Game();
            Game.RequestManager = GetComponent<RequestTimeoutManager>();

            for (int i = 0; i < 8; i++)
            {
                var ui_player = _main.GetChild("Player" + i) as UI_Player;
                ui_player.changeStateOnClick = false;
                ui_player.onClick.Add(() =>
                {
                    if (nowUseWay is HeroChooseRequest request && request.Number > 1)
                    {
                        //复选玩家模式
                        if (SelectedPlayers.Contains(ui_player))
                            SelectedPlayers.Remove(ui_player);
                        else
                            SelectedPlayers.Add(ui_player);
                    }
                    else
                    {
                        if (SelectedPlayers.Count == 0) SelectedPlayers.Add(ui_player);
                        else if (SelectedPlayers[0] == ui_player) SelectedPlayers.Clear();
                        else SelectedPlayers[0] = ui_player;
                    }
                    FlushView(Game);
                });
            }
            HandCards.OnCardClick.Add((x) =>
            {
                var ui_card = x.data as UI_ActionCard;
                if (SelectedCards.Contains(ui_card.Card))
                {
                    SelectedCards.Remove(ui_card.Card);
                }
                else
                {
                    SelectedCards.Add(ui_card.Card);
                }
                FlushView(Game);
            });

            Game.Init();

            Self = Game.Players[1];

            //Game.EventSystem.Register(EventEnum.DrawActionCard, onDrawCard);

            //Game.EventSystem.Register(EventEnum.DropActionCard, onDropCard);

            Game.OnRequest += OnRequest;

            Game.StartGame();

            FlushView(Game);
        }

        public void FlushView(Game game)
        {
            this.Game = game;
            int playerIndex = game.Players.IndexOf(Self);
            for (int i = 0; i < game.Players.Count; i++)
            {
                var ui_player = _main.GetChild("Player" + (7 - i)) as UI_Player;
                ui_player.SetPlayerCard(game.Players[i]);
                ui_player.selected = SelectedPlayers.Contains(ui_player);
            }
            HandCards.SetCards(Self.ActionCards, SelectedCards);

            checkRequest(nowRequest);

            _main.GetChild("Deck").text = game.ActionDeck.Count.ToString();
            _main.GetChild("Size").text = game.Size.ToString();
            _main.GetChild("n16").text = Self.EventCards.Count > 0 ? Self.EventCards[0].Name : "无";

            var skillList = _main.GetChild("Skills").asList;
            skillList.RemoveChildrenToPool();
            if (Self.Hero != null)
                foreach (var skill in Self.Hero.Skills)
                {
                    var ui_skill = skillList.AddItemFromPool().asButton;
                    ui_skill.text = skill.Name;
                    ui_skill.data = skill;
                    ui_skill.selected = skill != null && skill == nowSkill;
                }
        }

        /// <summary>
        /// 被询问时
        /// </summary>
        /// <param name="game"></param>
        /// <param name="request"></param>
        void OnRequest(Game game,Request request)
        {
            if (request.PlayerId != Self.Id)
            {
                return;
            }
            checkRequest(request);
        }
        /// <summary>
        /// 根据询问刷新UI
        /// </summary>
        /// <param name="request"></param>
        void checkRequest(Request request)
        {
            nowRequest = request;
            Controller c = _main.GetController("State");
            c.selectedIndex = 0;
            _main.GetController("ChooseHero").selectedIndex = 0;
            switch (request)
            {
                case FreeUseRequest useCardRequest:
                    Debug.Log("出牌");
                    c.selectedIndex = 1;
                    _main.GetChild("n17").asButton.enabled = false;
                    if (SelectedCards.Count == 1)
                    {
                        checkUse(SelectedCards[0]);
                    }
                    break;
                case ChooseDirectionRequest chooseDirectionRequest:
                    c.selectedIndex = 2;
                    break;
                case ChooseHeroRequest chooseHeroRequest:
                    _main.GetController("ChooseHero").selectedIndex = 1;
                    for (int i = 0; i < 3; i++)
                    {
                        _main.GetChild("n28").asCom.GetChild("hero" + i).text = chooseHeroRequest.HeroIds[i].ToString();
                    }
                    break;
                case ChooseSomeCardRequest chooseCardsRequest:
                    c.selectedIndex = 3;
                    _main.GetChild("n33").asButton.enabled = SelectedCards.Count == chooseCardsRequest.Count;
                    break;
                case TakeChoiceRequest takeChoiceRequest:
                    c.selectedIndex = 4;
                    GList choiceList = _main.GetChild("n34").asList;
                    choiceList.data = takeChoiceRequest;
                    choiceList.RemoveChildrenToPool();
                    foreach (var s in takeChoiceRequest.Infos)
                    {
                        var btn = choiceList.AddItemFromPool().asButton;
                        btn.text = s;
                    }
                    break;
                
            }
        }
        /// <summary>
        /// 根据出牌方式刷新UI
        /// </summary>
        /// <param name="useway"></param>
        void checkUse(ActionCard actionCard)
        {
            if (SelectedCards[0].CanUse(Game, nowRequest, getNowUseInfo(), out nowUseWay))
            {
                _main.GetChild("n17").asButton.enabled = true;
            }
            //switch (useway)
            //{
            //    case SimpleRequest simpleRequest:
            //        _main.GetChild("n17").asButton.enabled = true;
            //        break;
            //    case ChooseSomeoneRequest chooseSomeoneRequest:
            //        _main.GetChild("n17").asButton.enabled = SelectedPlayers.Count == chooseSomeoneRequest.Number;
            //        break;
            //}
        }

        void takeUse()
        {
            nowRequest = null;
            Game.Answer(getNowUseInfo());
            //switch (nowUseWay)
            //{
                //case SimpleRequest simpleRequest:
                //    Game.Answer(new SimpleResponse() { PlayerId = Self.Id, CardId = SelectedCards[0].Id });
                //    break;
                //case ChooseSomeoneRequest chooseSomeoneRequest:
                //    Game.Answer(new ChooseSomeoneResponse() { PlayerId = Self.Id, Targets = SelectedPlayers.Select(x => x.Player.Id).ToList() });
                //    break;
                //case ChooseDirectionRequest chooseDirectionRequest:
                //    break;
                //case ChooseSomeCardRequest  dropCardRequest:
                //    Game.DoAction(new ChooseSomeCardResponse() { PlayerId = Self.Id, Cards = SelectedCards });
                //    break;
            //}
            SelectedCards.Clear();
            FlushView(Game);
        }

        Task onDrawCard(object[] param)
        {
            string s = string.Empty;
            s += (param[0] as Player).Id.ToString()+"抽到了";
            foreach (ActionCard card in (param[1] as List<ActionCard>))
            {
                s += card.Name + ",";
            }
            Log.Debug(s);
            FlushView(Game);
            return Task.CompletedTask;
        }

        Task onDropCard(object[] param)
        {
            string s = string.Empty;
            s += (param[0] as Player).Id.ToString() + "丢掉了";
            foreach (ActionCard card in (param[1] as List<ActionCard>))
            {
                s +=  card.Name + ",";
            }
            Log.Debug(s);
            FlushView(Game);
            return Task.CompletedTask;
        }
        void useForward()
        {
            nowRequest = null;
            Game.Answer(new ChooseDirectionResponse() { PlayerId = Self.Id, CardId = Self.EventCards[0].Id, IfForward = true, IfSet = false });
            FlushView(Game);
        }

        void useBackward()
        {
            nowRequest = null;
            Game.Answer(new ChooseDirectionResponse() { PlayerId = Self.Id, CardId = Self.EventCards[0].Id, IfForward = false, IfSet = false });
            FlushView(Game);
        }

        void saveEventCard()
        {
            nowRequest = null;
            Game.Answer(new ChooseDirectionResponse() { PlayerId = Self.Id, CardId = Self.EventCards[0].Id, IfSet = true });
            FlushView(Game);
        }

        FreeUse getNowUseInfo()
        {
            FreeUse result = new FreeUse()
            {
                PlayerId = Self.Id,
                Source = SelectedCards.Select(x => x.Id).ToList(),
                CardId = SelectedCards.Count > 0 ? SelectedCards[0].Id : 0,
                PlayersId = SelectedPlayers.Select(x => x.Player.Id).ToList(),
            };
            return result;
        }
    }
}
