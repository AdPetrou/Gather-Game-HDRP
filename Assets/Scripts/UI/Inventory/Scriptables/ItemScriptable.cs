using UnityEngine;
using UnityEngine.UI;
using GatherGame.Utilities;
using GatherGame.Inventory.Behaviour;
using System;

namespace GatherGame.Inventory
{
    public abstract class ItemScriptable : ScriptableObject
    {
        #region Variables
        [SerializeField] protected string itemName;
        [SerializeField] protected Sprite itemIcon;
        [Range(1, 20)]
        [SerializeField] protected int width, height;
        public Tuple<int, int> objectSize { get { return Tuple.Create(width, height); } }
        [SerializeField] protected Tier tier;

        public virtual ItemScriptable GetItem() { return this; }
        public string GetName() { return itemName; }
        public Sprite GetSprite() { return itemIcon; }
        #endregion

        #region Unity Functions
        protected virtual void Awake()
        {
        }
        #endregion

        #region Create Prefab
        public virtual GameObject CreateItem(InventoryBehaviour inventory)
        {
            GameObject obj = new GameObject(itemName, typeof(Image));                   
            obj.GetComponent<Image>().sprite = itemIcon;

            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.pivot = new Vector2(0.5f, 0.5f); // Pivot is the centre
            rect.sizeDelta = new Vector2(width * InventoryManager.Instance.inventoryScale,
                height * InventoryManager.Instance.inventoryScale);
            //rect.SetParent(inventory.transform.Find("Items"));

            AddBehaviour(obj, inventory);
            return obj;
        }

        protected virtual void AddBehaviour(GameObject obj, InventoryBehaviour inventory)
        {
            ItemBehaviour data = obj.AddComponent<ItemBehaviour>();
            data.SpawnItem(this, inventory);
        }
        #endregion
    }
}