using GatherGame.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GatherGame.Actors
{
    [CreateAssetMenu(fileName = "Gather State", menuName = "State Machine/States/Actor States/Gather State")]
    public class GatherState : State
    {
        public override void Main(ActorBehaviour behaviour)
        {
            StaticCoroutine.Start(Harvest(behaviour));
        }

        protected IEnumerator Harvest(ActorBehaviour behaviour)
        {
            behaviour.canChangeSwitch(false);
            GameObject obj = behaviour.interactableObject;

            if (obj == null){ behaviour.canChangeSwitch(true); yield break; }

            InteractableObject thisObject = InteractionManager.Instance.GetObjectFromTag(obj.tag);
            Vector3 objPos = obj.transform.position, behaviourPos = behaviour.transform.position;
            if (thisObject && thisObject.harvestDistance < Mathf.Abs(Vector3.Distance
                (new Vector3(objPos.x, behaviourPos.y, objPos.z), behaviourPos)))
            { behaviour.canChangeSwitch(true); yield break; }

            //behaviour.LookAtPosition(obj.transform.position);

            Animate(behaviour.animationController, thisObject.harvestTime / 4f);
            yield return new WaitForSeconds(thisObject.harvestTime);
            thisObject.interact(obj);
            // This calls the Behaviour to run the Raycast Job again to reset the closest Interactable Object
            behaviour.GetInteractableObject(true); 

            ExpRules(behaviour, obj, thisObject);
            behaviour.canChangeSwitch(true);
            yield break;
        }

        public void ExpRules(ActorBehaviour behaviour, GameObject obj, InteractableObject thisObject)
        {
            SkillType type = thisObject.type;
            if (type == SkillType.Looting)
            {
                Inventory.Behaviour.InventoryBehaviour invBehaviour =   //Get Inventory Behaviour if it exists in a child object
                    obj.GetComponentInChildren<Inventory.Behaviour.InventoryBehaviour>();

                // Looting will not add EXP if the inventory has been opened before its refreshed or if it is a persistant inventory
                if (invBehaviour.isFirstOpen() && !invBehaviour.gameObject.GetComponent<Blocker>())
                    behaviour.gameObject.GetComponent<Stats.StatBehaviour>().addExp(type, thisObject.expGain);
            }
            else if (type != SkillType.None)                                         // Most Skill Types will have equal rules, this will filter out special rule cases
                behaviour.gameObject.GetComponent<Stats.StatBehaviour>().addExp(type, thisObject.expGain);
        }

        public void Animate(AnimationController animationController, float duration)
        {
            animationController.SmoothAnimate(defaultStateAnimation, duration);
        }
    }
}
