using System;
using UnityEngine;
using UnityEngine.EventSystems;
using GatherGame.UI;
using System.Collections.Generic;

namespace GatherGame.Inventory
{
    public class ItemBehaviour : DraggableUI
    {
        #region Variables
        public Tuple<InventoryBehaviour, List<Tuple<int, int>>> usedSlots { get; private set; }
        /* The usedSlots take the inventory and the position of all the slots the item inhibits, as a tuple.
        The list of slots is used in the inventory to check if a slot is filled when adding or moving other items. */
        public void SetUsedSlots(InventoryBehaviour inventory, List<Tuple<int, int>> usedSlots)        
        { this.usedSlots = Tuple.Create(inventory, usedSlots); }
        // SetUsedSlots for encapsulaion so the data doesn't accidentally get edited.
        protected Tuple<InventoryBehaviour, List<Tuple<int, int>>> previousPos;
        // previousPos is set when the object is picked up and used if the new position is invalid.

        private int width, height;
        public Tuple<int, int> objectSize { get { return Tuple.Create(width, height); } }
        // the tuple is mostly for convinicence

        protected RectTransform rectTransform;
        protected Vector2 itemOffset { get { return new Vector2((width - 1) / 2f, (height - 1) / 2f); } }
        // (width - 1) / 2 && (height - 1) / 2, is the offset from the middle of the object

        public virtual void SpawnItem(ItemScriptable scriptable, InventoryBehaviour inventory)
        {
            width = scriptable.objectSize.Item1; height = scriptable.objectSize.Item2;
            inventory.AddItemToSlot(this);
            rectTransform.localPosition = GetPosition();
        }
        #endregion

        #region Unity Methods
        public override void Awake()
        {
            rectTransform = gameObject.GetComponent<RectTransform>();
            base.Awake();
        }
        #endregion

        #region Event System
        public override void OnPointerDown(PointerEventData eventData)
        {
            previousPos = usedSlots;
            usedSlots = Tuple.Create<InventoryBehaviour, List<Tuple<int, int>>>(null, null);
            transform.SetParent(InventoryManager.Instance.itemCanvas.transform, true);
            transform.SetAsLastSibling();   
            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            InventoryBehaviour inventory = InventoryManager.Instance.currentInventory;
            bool success = AddToInventory(inventory, eventData);
            if (!success)
            {
                inventory = InventoryManager.Instance.GetClosestInventory(Input.mousePosition);
                success = AddToInventory(inventory, eventData);
                if (!success)
                    previousPos.Item1.AddItemToSlot(this, previousPos.Item2);
            }
            rectTransform.localPosition = GetPosition();
        }

        protected virtual bool AddToInventory(InventoryBehaviour inventory, PointerEventData eventData)
        {
            var itemOffset = GetItemOrigin(eventData);
            var origin = inventory.GetPositionFromVector(Input.mousePosition, true);

            return inventory.AddItemToSlot(this, origin, itemOffset);
        }

        private Queue<Tuple<int, int>> GetItemOrigin(PointerEventData eventData = null)
        {
            Vector2 origin;
            if (eventData != null)
                origin = offset / InventoryManager.Instance.inventoryScale + itemOffset;
            else
                origin = Vector2.one;

            Queue<Tuple<int, int>> returnResult = new Queue<Tuple<int, int>>();
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    //Debug.Log((x - Mathf.FloorToInt(origin.x)) + " " + (y - Mathf.FloorToInt(origin.y)));
                    returnResult.Enqueue(Tuple.Create(x - Mathf.RoundToInt(origin.x), y - Mathf.RoundToInt(origin.y)));
                }

            return returnResult;
        }
        #endregion

        #region Methods

        public Vector2 GetPosition()
        {
            if (usedSlots.Item2 == null || usedSlots.Item2.Count <= 0)
                return Vector2.zero;

            int xCorner = usedSlots.Item2[0].Item1, yCorner = usedSlots.Item2[0].Item2;
            foreach (var slot in usedSlots.Item2)
            {
                if (slot.Item1 < xCorner)
                    xCorner = slot.Item1;
                if (slot.Item2 < yCorner)
                    yCorner = slot.Item2;
            }

            return (new Vector2(xCorner, yCorner) + itemOffset)
                * InventoryManager.Instance.inventoryScale;
            #endregion
        }
    }
}
