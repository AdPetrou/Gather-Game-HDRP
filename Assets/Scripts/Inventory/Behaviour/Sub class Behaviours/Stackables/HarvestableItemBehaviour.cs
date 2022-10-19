using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GatherGame.Inventory.Behaviour
{
    public class HarvestableItemBehaviour : GenericItemBehaviour
    {
        public Quality quality;

        public override bool compareObjects(GameObject obj)
        {
            bool result = base.compareObjects(obj);

            if (result && obj.GetComponent<HarvestableItemBehaviour>().quality != quality)
                result = false;

            return result;
        }
    } 
}
