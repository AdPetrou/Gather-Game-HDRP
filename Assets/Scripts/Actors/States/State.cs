using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GatherGame
{
    public abstract class State : ScriptableObject
    {
        public StateType StateRef 
        { 
            get 
            { 
                if(stateRef == StateType.None)
                    stateRef = (StateType)System.Enum.Parse(typeof(StateType), name);

                return stateRef;
            }
            protected set { stateRef = value; }
        }
        [SerializeField]
        [MyBox.ReadOnly]
        private StateType stateRef = StateType.None;
        [SerializeField]
        protected AnimationClip defaultStateAnimation;
        [SerializeField]
        protected State[] possibleStates;
        [SerializeField]
        protected bool hasExitTime;

        public State[] PossibleStates
        {
            get { return possibleStates; }
            private set{ possibleStates = value; }
        }
        public abstract void Main(Actors.ActorBehaviour behaviour);

        public void OnValidate()
        {
            if(stateRef == StateType.None)
                stateRef = (StateType)System.Enum.Parse(typeof(StateType), name.Replace(" ", ""));
        }
    }
}
