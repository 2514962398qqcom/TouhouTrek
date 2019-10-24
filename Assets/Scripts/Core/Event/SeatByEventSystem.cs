using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMDFQ
{
    public class SeatByEventSystem
    {
        public int MaxSeat;

        public Game game;

        Dictionary<EventEnum, List<EventItem>> dic = new Dictionary<EventEnum, List<EventItem>>();
        struct EventItem
        {
            public Func<object[], Task> action;
            public int Seat;
            public int sortIndex;
            public string name;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventEnum"></param>
        /// <param name="seat">小于0视为优先发动</param>
        /// <param name="action"></param>
        /// <param name="sortIndex">执行优先度，注意seat优先于sortIndex</param>
        public void Register(EventEnum eventEnum, int seat, Func<object[], Task> action, int sortIndex = 0, string name = null)
        {
            List<EventItem> list;
            if (!dic.TryGetValue(eventEnum, out list))
            {
                list = new List<EventItem>();
                dic.Add(eventEnum, list);
            }
            list.Add(new EventItem()
            {
                action = action,
                sortIndex = sortIndex,
                Seat = seat,
                name = name,
            });
        }
        public void Remove(EventEnum eventEnum, Func<object[], Task> action)
        {
            List<EventItem> list;
            if (dic.TryGetValue(eventEnum, out list))
            {
                list.RemoveAll(x => x.action == action);
            }
        }
        /// <summary>
        /// 当前正在结算的事件
        /// </summary>
        public EventEnum currentEvent
        {
            get
            {
                if (eventStack.Count > 0)
                    return eventStack.Peek();
                return EventEnum.Nothing;
            }
        }
        Stack<EventEnum> eventStack { get; } = new Stack<EventEnum>();
        public async Task Call(EventEnum eventEnum, int seat, params object[] param)
        {
            //Log.Debug($"触发了{eventEnum}事件");
            List<EventItem> list;
            if (dic.TryGetValue(eventEnum, out list))
            {
                List<EventItem> items = new List<EventItem>(list);//对注册的回调进行排序
                items.Sort((x, y) =>
                {
                    int xindex = getDistance(seat, x.Seat);
                    int yindex = getDistance(seat, y.Seat);
                    if (x.Seat - y.Seat == 0)
                    {
                        return x.sortIndex - y.sortIndex;
                    }
                    else
                    {
                        return xindex - yindex;
                    }
                });
                eventStack.Push(eventEnum);
                while (items.Count > 0)
                {
                    //取出所有和第一个事件同一座次和排序的事件
                    EventItem next = items[0];
                    List<EventItem> nextLists = new List<EventItem>();
                    while (items.Count > 0 && items[0].Seat == next.Seat && items[0].sortIndex == next.sortIndex)
                    {
                        nextLists.Add(items[0]);
                        items.RemoveAt(0);
                    }

                    await Task.WhenAll(nextLists.Select(x => x.action(param)));

                    //if (seat >= 0 && seat < game.Players.Count)
                    //{
                    //    while (nextLists.Count > 1)
                    //    {
                    //        //说明同一玩家在此处注册了多个事件
                    //        PlayerAction.TakeChoiceResponse response = (PlayerAction.TakeChoiceResponse)await game.WaitAnswer(new PlayerAction.TakeChoiceRequest()
                    //        {
                    //            Infos = nextLists.Select(x => x.name).ToList(),
                    //            PlayerId = game.Players[next.Seat].Id,
                    //        }.SetTimeOut(game.RequestTime));
                    //        await nextLists[response.Index].action(param);
                    //        nextLists.RemoveAt(response.Index);
                    //    }
                    //    await nextLists[0].action(param);
                    //}
                    //else
                    //{
                    //    //并非玩家注册的，直接按顺序执行
                    //    foreach (var eventItem in items)
                    //    {
                    //        await eventItem.action(param);
                    //    }
                    //}
                }//按照顺序进行结算
                eventStack.Pop();
            }
        }

        int getDistance(int startSeat, int seat)
        {
            if (seat < 0) return seat;
            int result = seat - startSeat;
            if (result < 0) result += MaxSeat;
            return result;
        }
    }
}
