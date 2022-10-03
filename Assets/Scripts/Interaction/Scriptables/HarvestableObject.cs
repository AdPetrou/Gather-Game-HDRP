using UnityEngine;
using Unity.Collections;
using GatherGame.Utilities;
using GatherGame.Inventory.Scriptables;
using GatherGame.Inventory.Behaviour;

namespace GatherGame.Interaction
{ 
    [CreateAssetMenu(fileName = "Harvest Object", menuName = "Interactable Objects/Harvest Object")]
    public class HarvestableObject : InteractableObject
    {
        #region Variables
        [SerializeField]
        internal HarvestDrop[] items;

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
        public int getDropAmount(HarvestableItem item)
        {
            for(int index = 0; index < items.Length; index++)
                if(item.Equals(items[index]))
                    return Random.Range(items[index].minDrop, items[index].maxDrop);

            return -1;
        }
        #endregion

        #region Methods
        public override GameObject interact(GameObject gameObject)
        {
            gameObject.SetActive(false);
            InteractionManager.Instance.Timer.CreateTimer(gameObject, respawnTime);

            //for (int i = 0; i < items.Length; i++)           
            //Inventory.InventoryManager.currentBackpack.modStackableItem(items[i].item, getDropAmount(i), setQuality(i));

            return gameObject;
        }

        public GameObject setQuality(int index)
        {
            GameObject ghostObj = Inventory.InventoryManager.currentBackpack.spawnGhostItem(items[index].item);
            items[index].item.setQualityFromSkill(ghostObj.GetComponent<HarvestableItemBehaviour>());
            return ghostObj;
        }
        #endregion

        [System.Serializable]
        internal struct HarvestDrop
        {
            public HarvestableItem item;
            public int minDrop, maxDrop;

            public HarvestDrop(HarvestableItem _item, int _minDrop, int _maxDrop)
            {
                item = _item;
                minDrop = _minDrop;
                maxDrop = _maxDrop;
            }
        }
    }
}
