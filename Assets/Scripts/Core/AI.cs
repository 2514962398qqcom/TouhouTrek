using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMDFQ
{
    using PlayerAction;
    public class AI : Player
    {
        public AI(Game game, int id) : base(id)
        {
            Id = id;
            Init(game);
        }
        public void Init(Game game)
        {
            game.OnRequest += doResponse;
        }

        async void doResponse(Game game, Request request)
        {
            if (request.PlayerId != Id) return;
            Log.Debug("AI" + Name + "被询问" + request);
            await game.TimeManager.WaitAsync(0.5f, game.cts.Token);//假装思考0.5s
            Log.Debug("AI" + Name + "回应" + request);
            switch (request)
            {
                case FreeUseRequest useCardRequest:
                    game.Answer(new EndFreeUseResponse() { PlayerId = Id });
                    break;
                case ChooseSomeCardRequest dropCard:
                    List<ActionCard> cards = new List<ActionCard>();
                    for (int i = 0; i < dropCard.Count; i++)
                    {
                        cards.Add(ActionCards[i]);
                    }
                    game.Answer(new ChooseSomeCardResponse()
                    {
                        PlayerId = Id,
                        Cards = cards.Select(x => x.Id).ToList()
                    });
                    break;
                case ChooseHeroRequest chooseHero:
                    game.Answer(new ChooseHeroResponse()
                    {
                        PlayerId = Id,
                        HeroId = chooseHero.HeroIds[0],
                    });
                    break;
                case ChooseDirectionRequest chooseDirectionRequest:
                    game.Answer(new ChooseDirectionResponse() { PlayerId = Id, CardId = EventCards[0].Id, IfSet = false, IfForward = true });
                    break;
                case TakeChoiceRequest takeChoice:
                    game.Answer(new TakeChoiceResponse() { PlayerId = Id, Index = 0 });
                    break;
                case UseLimitCardRequest useLimitCardRequest:
                    game.Answer(new UseLimitCardResponse() { PlayerId = Id, Used = false });
                    break;
                default:
                    Log.Warning($"ai未处理的响应类型:{request.GetType()}");
                    break;
            }
        }
    }
}
