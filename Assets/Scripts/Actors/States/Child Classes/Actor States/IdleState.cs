using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GatherGame.Actors
{
    [CreateAssetMenu(fileName = "Idle State", menuName = "State Machine/States/Actor States/Idle State")]
    public class IdleState : State
    {
        public override void Main(Actors.ActorBehaviour behaviour)
        {
            behaviour.animationController.Animate(defaultStateAnimation);
        }
    }
}
