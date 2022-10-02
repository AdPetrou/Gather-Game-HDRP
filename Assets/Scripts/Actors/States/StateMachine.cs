using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GatherGame.Actors
{
    [CreateAssetMenu(fileName = "State Machine", menuName = "State Machine/State Machine")]
    public class StateMachine : ScriptableObject
    {
        public State[] States
        {
            get { return states; }
            protected set { states = value; }
        }

        [SerializeField]
        protected State[] states;
    }
}
