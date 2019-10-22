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
        /// <summary>
        /// 卡片在配置表中的ID，相当于卡片的类型。通过该属性获取到的卡片类型不一定是真实的，这一张卡也有可能是其他卡变形而来的。
        /// </summary>
        public virtual int configID
        {
            get { return ConfigId; }
        }
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
            target.Owner = Owner;
            target.Id = Id;
            target.ConfigId = ConfigId;
            target.Name = Name;
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
