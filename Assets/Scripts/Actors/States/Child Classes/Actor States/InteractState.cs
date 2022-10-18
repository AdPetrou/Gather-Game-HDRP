using GatherGame.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GatherGame.Actors
{
    [CreateAssetMenu(fileName = "Interact State", menuName = "State Machine/States/Actor States/Interact State")]
    public class InteractState : State
    {
        public override void Main(ActorBehaviour behaviour)
        {
            StaticCoroutine.Start(Interact(behaviour));
        }

        protected IEnumerator Interact(ActorBehaviour behaviour)
        {
            behaviour.canChangeSwitch(false);
            GameObject obj = behaviour.interactableObject;

            if (obj == null){ behaviour.canChangeSwitch(true); yield break; }

            InteractableObject thisObject = InteractionManager.Instance.GetObjectFromTag(obj.tag);
            Vector3 objPos = obj.transform.position, behaviourPos = behaviour.transform.position;
            if (thisObject && thisObject.GetDistance() < Mathf.Abs(Vector3.Distance
                (new Vector3(objPos.x, behaviourPos.y, objPos.z), behaviourPos)))
            { behaviour.canChangeSwitch(true); yield break; }

            //behaviour.LookAtPosition(obj.transform.position);

            Animate(behaviour.animationController, 0.25f);
            yield return new WaitForSeconds(thisObject.GetInteractTime());
            thisObject.interact(obj, behaviour);
            // This calls the Behaviour to run the Raycast Job again to reset the closest Interactable Object
            behaviour.GetInteractableObject(true); 

            ExpRules(behaviour, obj, thisObject);
            behaviour.canChangeSwitch(true);
            yield break;
        }

        public void ExpRules(ActorBehaviour behaviour, GameObject obj, InteractableObject thisObject)
        {
            return;
        }

        public void Animate(AnimationController animationController, float duration)
        {
            animationController.SmoothAnimate(defaultStateAnimation, duration);
        }
    }
}
