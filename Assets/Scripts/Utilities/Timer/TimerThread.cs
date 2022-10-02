using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using GatherGame.Timers.Internal;
using System.Threading.Tasks;

namespace GatherGame.Timers 
{
    public class TimerThread : Utilities.Singleton<TimerThread>
    {
        private List<IThreadedTimerHack> threadedTimerList = new List<IThreadedTimerHack>();
        private Queue<Tuple<TimerFunction<int>, int>> timerQueueList = new Queue<Tuple<TimerFunction<int>, int>>();

        Thread thread;

        public void Start()
        {
            thread = new Thread(Run);
            thread.Start();
        }

        public void AddTimer(TimerFunction<int> timer, int ID)
        {
            lock (timerQueueList)
            {
                timerQueueList.Enqueue(Tuple.Create(timer, ID));
                return;
            }
        }

        public void AddThreadedObject(IThreadedTimerHack threadedTimer)
        {
            threadedTimer.ID = threadedTimerList.Count;
            threadedTimerList.Add(threadedTimer);
        }

        private void Run()
        {
            while (true)
            {
                if (timerQueueList.Count > 0)
                {
                    lock (timerQueueList)
                    {
                        var timerContainer = timerQueueList.Dequeue();
                        _ = ActivateTimer(timerContainer.Item1, timerContainer.Item2);
                    }
                }
            }
        }

        private async Task ActivateTimer(TimerFunction<int> timer, int ID)
        {
            int index = await timer.Wait();
            threadedTimerList[ID].AddResult(index);
        }

        public void OnApplicationQuit()
        {
            thread.Abort();
        }
    } 
}
