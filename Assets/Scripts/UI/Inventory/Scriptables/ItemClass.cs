using UnityEngine;
using UnityEngine.UI;
using GatherGame.Utilities;
using GatherGame.Inventory.Behaviour;

namespace GatherGame.Inventory.Scriptables
{
    public abstract class ItemClass : ScriptableObject
    {
        #region Variables
        public string itemName;
        public Sprite itemIcon;
        public int width, height;
        public Vector2 itemOffset { get; private set; }
        public Tier tier;

        public virtual ItemClass GetItem() { return this; }
        #endregion

        #region Unity Functions
        protected virtual void Awake()
        {

        }
        #endregion

        #region Create Prefab
        public virtual GameObject createItem(InventoryBehaviour inventory)
        {
            // If the item offset isn't set then big items will end up off center and look stupid
            itemOffset = new Vector2
        ((InventoryManager.offset / 2) * (width - 1),
        (InventoryManager.offset / 2) * (height - 1));

            GameObject obj = new GameObject();            
            obj.transform.SetParent(inventory.transform.GetChild(inventory.transform.childCount - 1));
            obj.name = itemName;

            obj.AddComponent<Image>();
            obj.GetComponent<Image>().sprite = itemIcon;
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.pivot = new Vector2(0.5f, 0.5f); // Pivot is the centre
            rect.sizeDelta = new Vector2(width * InventoryManager.offset,
                height * InventoryManager.offset);

            addBehaviour(obj);

            return obj;
        }

        #region Behaviour
        protected virtual void addBehaviour(GameObject obj)
        {
            ItemBehaviour data = obj.AddComponent<ItemBehaviour>();
            data.setStats(this);
        }
        #endregion
        #endregion
    }
}