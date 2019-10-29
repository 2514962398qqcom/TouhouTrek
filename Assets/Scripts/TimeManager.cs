using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ZMDFQ
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance;
        private class Timer
        {
            public float Time;
            public TaskCompletionSource<bool> tcs;
        }
        List<Timer> timers = new List<Timer>();
        private void Awake()
        {
            Instance = this;
        }
        private void Update()
        {
            while (timers.Count > 0 && timers[0].Time < Time.time)
            {
                timers[0].tcs.TrySetResult(true);
                timers.RemoveAt(0);
            }
        }
        public Task WaitAsync(float time, CancellationToken cancellationToken)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            Timer timer = new Timer() { tcs = tcs, Time = Time.time + time };
            addNewTimer(timer);
            cancellationToken.Register(() => { timers.Remove(timer); });
            return tcs.Task;
        }
        public Task WaitAsync(float time)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            Timer timer = new Timer() { tcs = tcs, Time = Time.time + time };
            addNewTimer(timer);
            return tcs.Task;
        }
        public float getRemainTime(Task task)
        {
            foreach (Timer timer in timers)
            {
                if (timer.tcs.Task == task)
                    return timer.Time - Time.time;
            }
            throw new KeyNotFoundException("Task未注册");
        }
        void addNewTimer(Timer timer)
        {
            for (int i = 0; i < timers.Count; i++)
            {
                if (timer.Time < timers[0].Time)
                {
                    timers.Insert(i, timer);
                    return;
                }
            }
            timers.Add(timer);
        }
    }
}
