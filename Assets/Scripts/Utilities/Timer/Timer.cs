using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using GatherGame.Timers.Internal;

namespace GatherGame.Timers.Internal
{
    // This interface is needed to store Timers of any type without the need for a base abstract class
    public interface IThreadedTimerHack
    {
        // The ID is used to find the right Timer Object to return to after the Timer has ran on the thread
        public int ID { get; set; }
        public void AddResult(int result);
    }

    // This is the functional part of the Timer
    public struct TimerFunction<T>
    {
        private float time;
        private T returnVal;

        internal TimerFunction(T input, float _time)
        { time = _time; returnVal = input; }

        public async Task<T> Wait()
        {
            for (int i = 0; i < time; i++)
                await Task.Delay(1000);

            return returnVal;
        }
    }
}

namespace GatherGame.Timers
{
    // This will function the same as TimerList but will be limited to only one object
    public interface IThreadedTimer<T>
    {
        public ThreadedTimer<T> Timer { get; }
        public void ProcessTimer();
    }

    public interface IThreadedTimerList<T>
    {
        public ThreadedTimerList<T> Timer { get; }
        public void ProcessTimer();
    }


    public class ThreadedTimer<T> : ThreadedTimerList<T>
    {
        public T result 
        { get { if (results.Count > 0) return results.Dequeue(); 
                else throw(new NullReferenceException()); } }


        [Obsolete("Use [result] instead", true)]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
        public override Queue<T> results { get; protected set; } = new Queue<T>();
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member

        public ThreadedTimer()
        {
            objList = new List<Tuple<T, int>>(1);
            TimerThread.Instance.AddThreadedObject(this);
        }
        public override void CreateTimer(T obj, float time)
        {
            objList.Clear();
            base.CreateTimer(obj, time);
        }
    }

    public class ThreadedTimerList<T> : IThreadedTimerHack
    {
        public int ID { get; set; }
        public virtual Queue<T> results { get; protected set; } = new Queue<T>();
        protected List<Tuple<T, int>> objList = new List<Tuple<T, int>>();
        // To avoid messy casting and Generic Lists the Timer Thread only accepts integer Timers
        // This list will store the return object as a tuple coupled with an Int which is used as an ID
        // When the timer is created it will return the ID which can then be processed and added to the queue


        public ThreadedTimerList()
        {
            // Adds to the List of other Threaded Timer objects so it can get a unique ID
            // Once added it is not removed
            TimerThread.Instance.AddThreadedObject(this);
        }

        public virtual void CreateTimer(T obj, float time) // This will queue the timer to activate
        {
            int listIndex = GetUniqueIndex();
            objList.Add(Tuple.Create(obj, listIndex));
            TimerThread.Instance.AddTimer(new TimerFunction<int>(listIndex, time), ID);
        }

        protected int GetUniqueIndex()
        {
            int highest = -1;
            foreach(var tuple in objList)
            {
                if (tuple.Item2 > highest)
                    highest = tuple.Item2;
            }

            return highest + 1;
        }

        protected int GetObjectIndexFromID(int objectID)
        {
            for(int i = 0; i < objList.Count; i++)
            {
                if (objList[i].Item2 == objectID)
                    return i;
            }

            Debug.LogError("Error, Timer result out of bounds");
            return -1;
        }

        public void AddResult(int result)
        {
            int index = GetObjectIndexFromID(result);
            results.Enqueue(objList[index].Item1);
            objList.RemoveAt(index);
        }
    }
}
