using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZMDFQ
{
    public static class EventHelper
    {
        static List<Action<Game>> actions = new List<Action<Game>>();
        static EventHelper()
        {
            foreach (var type in typeof(Game).Assembly.GetTypes())
                foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly))
                {
                    foreach (EventReigsterAttribute attr in method.GetCustomAttributes(typeof(EventReigsterAttribute), true))
                    {
                        //Log.Debug(method.Name);
                        Action<Game> action = method.CreateDelegate(typeof(Action<Game>), null) as Action<Game>;
                        actions.Add(action);
                    }
                }
        }

        public static void RegisterEvent(Game game)
        {
            foreach (var action in actions)
            {
                action(game);
            }
        }
    }
}
