using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GatherGame.Actors
{
    [CreateAssetMenu(fileName = "Turn State", menuName = "State Machine/States/Actor States/Turn State")]
    public class TurnState : State
    {
        [SerializeField][Range(0f, 720f)]
        protected float rotationSpeed;

        public override void Main(Actors.ActorBehaviour behaviour)
        {
            Turn(behaviour);
        }
        public void Main(Actors.ActorBehaviour behaviour, float duration)
        {
            Turn(behaviour, duration);
        }

        protected void Turn(Actors.ActorBehaviour behvaiour, float duration = 0)
        {
            if (duration == 0)
                duration = rotationSpeed;

            behvaiour.animationController.SmoothAnimate(defaultStateAnimation,  360f / rotationSpeed);
        }
    }
}
