using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMDFQ
{
    [Obsolete("请使用Card.configID来获取卡片的类型ID")]
    public static class CardHelper
    {
        static List<Type> list = new List<Type>();

        static CardHelper()
        {
            foreach (var type in typeof(Game).Assembly.GetTypes())
            {
                if (typeof(Card).IsAssignableFrom(type)&&!type.IsAbstract)
                {
                    //Log.Debug($"获取到卡片实例类型:{type.Name},Id:{list.Count - 1}");
                    list.Add(type);
                }
            }
        }
        public static int getId(Card card)
        {
            return list.IndexOf(card.GetType());
        }

        public static int getId(Type type)
        {
            return list.IndexOf(type);
        }

        public static Type getType(int id)
        {
            return list[id];
        }
    }
}
