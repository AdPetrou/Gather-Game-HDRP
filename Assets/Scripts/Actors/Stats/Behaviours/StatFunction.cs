using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GatherGame.Actors.Stats 
{
    public abstract class StatFunction : MonoBehaviour
    {
        public bool isEnabled { get; protected set; }
        protected float preValue;
        protected IntStatBehaviour elementUI;
        public float value { get; protected set; }

        public abstract void main();
    } 
}
