using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Animancer;

namespace GatherGame.Actors
{
    public class AnimationController
    {
        #region Variables
        public AnimancerComponent animator { get; private set; }
        public string currentState { get; private set; }

        public AnimationController(AnimancerComponent _animator)
        {
            animator = _animator;
        }
        #endregion

        #region Methods
        // By default most animation blends will use an index of 1
        // Most animations are only going to blend between action and idle until I add jumping
        // That's gonna be a pain in the ass, might skip free jumping and have it as a scripted event
        public void SmoothAnimate(AnimationClip animation, float duration)
        {      
            changeAnimationState(animation, duration);
            //Debug.Log(duration);
        }

        public void Animate(AnimationClip animation)
        {
            changeAnimationState(animation);
        }

        private void changeAnimationState(AnimationClip newState, 
            float duration = AnimancerPlayable.DefaultFadeDuration)
        {
            if (currentState == newState.name)
                return;

            currentState = newState.name;
            animator.Play(newState, duration);
        }
      

        #region IK
        // The actual IK detection will be in the main script 
        // This is just the calculation code
        //public void DoIK(AvatarIKGoal bone, float rotationOffset = 1f)
        //{
        //    float boneWeight = animator.GetFloat(bone.ToString() + "Weight");

        //    animator.SetIKPositionWeight(bone, boneWeight);
        //    animator.SetIKRotationWeight(bone, boneWeight / rotationOffset);

        //    RaycastHit hit;
        //    Ray ray = new Ray(animator.GetIKPosition(bone) + Vector3.up, Vector3.down);
        //    if (Physics.Raycast(ray, out hit, distanceToGround + 1f, layerMask))
        //    {
        //        Vector3 footPosition = hit.point;
        //        footPosition.y += distanceToGround;
        //        animator.SetIKPosition(bone, footPosition);
        //        Vector3 forward = Vector3.ProjectOnPlane(animator.transform.forward, hit.normal);
        //        animator.SetIKRotation(bone, Quaternion.LookRotation(forward, hit.normal));
        //    }
        //}
        #endregion

        #endregion
    }
}
