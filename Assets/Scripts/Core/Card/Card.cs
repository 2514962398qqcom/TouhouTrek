using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMDFQ
{
    public abstract class Card
    {
        [BsonIgnore]
        public Player Owner;

        public int Id;
        public int ConfigId;
        public string Name;
        public CardTypeEnum CardType;

        //internal abstract PlayerAction.Request GetRequest();

        //internal abstract void DoEffect(Game game, PlayerAction.Response target);
        public static T copyCard<T>(T origin) where T : Card, new()
        {
            T card = new T();
            origin.copyPropTo(card);
            return card;
        }
        protected virtual void copyPropTo(Card target)
        {
            target.Id = Id;
            target.CardType = CardType;
        }
    }

    public enum CardTypeEnum
    {
        Charactor,
        Theme,
        Event,
        Action,
    }
}
