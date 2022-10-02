using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GatherGame.Actors
{
    [CreateAssetMenu(fileName = "Movement State", menuName = "State Machine/States/Actor States/Movement State")]
    public class MovementState : State
    {
        [Header("Movement Variables")]
        [SerializeField][Range(0f, 5f)]
        protected float speed;
        [SerializeField][Range(2f, 8f)]
        protected float maxSpeed;
        [SerializeField][Range(-5f, 0f)]
        protected float stopSpeed;
        [SerializeField][Range(0f, 720f)]
        protected float rotationSpeed;

        public override void Main(Actors.ActorBehaviour behaviour)
        {           
            Walk(behaviour, behaviour.direction);
        }

        private void Walk(Actors. ActorBehaviour behaviour, Vector3 direction)
        {
            Transform transform = behaviour.transform;

            float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
            if (!Mathf.Approximately(angle, 0))
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + (Time.deltaTime * rotationSpeed * (angle / Mathf.Abs(angle))), 0);

            // Multiplies Speed and Max Speed (in m/s) by the time between frames, so it becomes meters per frame (m/f).
            float metersPerFrame = speed * Time.deltaTime;
            float maxMetersPerFrame = maxSpeed * Time.deltaTime;

            if (direction == Vector3.zero)
                maxMetersPerFrame = 0;

            bool deccelerating = false;
            // Deccelerate if the Velocity is greater than the Max Speed i.e. sprinting to walking
            if (maxMetersPerFrame < behaviour.velocity)
            {
                deccelerating = true;
                metersPerFrame = stopSpeed * Time.deltaTime;
            }

            if (Mathf.Approximately(behaviour.velocity, maxMetersPerFrame))
                behaviour.SetVelocity(maxMetersPerFrame);

            behaviour.SetVelocity(behaviour.velocity + metersPerFrame);
            if (behaviour.velocity < 0)
                behaviour.SetVelocity(0);

            if (behaviour.velocity > maxMetersPerFrame && !deccelerating)
                behaviour.SetVelocity(maxMetersPerFrame);
            else if (deccelerating && behaviour.velocity < maxMetersPerFrame)
                behaviour.SetVelocity(maxMetersPerFrame);

            transform.GetComponent<CharacterController>().Move(transform.forward * behaviour.velocity);

            if (direction == Vector3.zero)
                Animate(behaviour, Mathf.Abs(behaviour.velocity / metersPerFrame) / 4f, true);
            else
                Animate(behaviour, Mathf.Abs((maxMetersPerFrame - behaviour.velocity) / metersPerFrame) / 4f, false);
        }

        private void Animate(Actors.ActorBehaviour behaviour, float duration, bool idle = false)
        {
            if (!idle)
                behaviour.animationController.SmoothAnimate(defaultStateAnimation, duration);
            else
                behaviour.DefaultState();
        }
    }
}
