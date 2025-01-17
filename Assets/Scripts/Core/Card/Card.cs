﻿using MongoDB.Bson.Serialization.Attributes;
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
        public int trueConfigID
        {
            get { return ConfigId; }
        }
        public string Name;
        public string Tags;
        public string Desc;
        public CardTypeEnum CardType;
        [BsonIgnore]
        Dictionary<string, object> propsDic { get; } = new Dictionary<string, object>();
        public T getProp<T>(string propName)
        {
            if (propsDic.ContainsKey(propName) && propsDic[propName] is T value)
                return value;
            else
                return default;
        }
        public void setProp(string propName, object value)
        {
            propsDic[propName] = value;
        }
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
        public override string ToString()
        {
            return GetType().Name + "(" + Id + ")";
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
