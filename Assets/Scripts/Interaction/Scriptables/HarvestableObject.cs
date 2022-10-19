using UnityEngine;
using Unity.Collections;
using GatherGame.Utilities;
using GatherGame.Inventory.Scriptables;

namespace GatherGame.Interaction
{ 
    [CreateAssetMenu(fileName = "Harvest Object", menuName = "Interactable Objects/Harvest Object")]
    public class HarvestableObject : InteractableObject
    {
        #region Variables
        [MyBox.Separator][Header("--Harvestable Data--")]
        [SerializeField]internal HarvestDrop[] items;

        public override void OnValidate()
        {
            foreach (HarvestDrop item in items)
                item.item.skillType = type;
            base.OnValidate();
        }

        public int getDropAmount(int index)
        {
            return Random.Range(items[index].minDrop, items[index].maxDrop);
        }
        public int getDropAmount(HarvestableItemScriptable item)
        {
            for(int index = 0; index < items.Length; index++)
                if(item.Equals(items[index]))
                    return Random.Range(items[index].minDrop, items[index].maxDrop);

            return -1;
        }
        #endregion

        #region Methods
        public override GameObject interact(GameObject gameObject, Actors.ActorBehaviour actor)
        {
            gameObject.SetActive(false);
            InteractionManager.Instance.Timer.CreateTimer(gameObject, respawnTime);

            for (int i = 0; i < items.Length; i++)    
                for(int u = 0; u < items[i].GetDrop(); u++)
                    items[i].item.CreateItem(actor.GetComponentInChildren<Inventory.InventoryBehaviour>(true));

            return gameObject;
        }
        #endregion

        [System.Serializable]
        internal struct HarvestDrop
        {
            public HarvestableItemScriptable item;
            public int minDrop, maxDrop;

            public HarvestDrop(HarvestableItemScriptable _item, int _minDrop, int _maxDrop)
            {
                item = _item;
                minDrop = _minDrop;
                maxDrop = _maxDrop;
            }

            public int GetDrop()
            {
                return Random.Range(minDrop, maxDrop + 1);
            }
        }
    }
}
