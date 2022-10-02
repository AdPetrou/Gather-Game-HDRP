using UnityEngine;
using UnityEngine.EventSystems;
using GatherGame.Utilities;
using GatherGame.UI;

namespace GatherGame.Inventory.Behaviour
{
    public class ItemBehaviour : DraggableUI
    {
        #region Variables
        public ItemType itemType;
        public System.Tuple<int, int> objectSize { get; protected set; }
        protected System.Tuple<int, int> previousPos;
        public Vector2 itemOffset;
        public RectTransform rectTransform { get; protected set; }

        public virtual void setStats(Scriptables.ItemClass scriptable)
        {
            itemType = ItemType.Generic;
            objectSize = System.Tuple.Create(scriptable.width, scriptable.height);
            itemOffset = scriptable.itemOffset;
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
            InventoryManager.currentItem = this;
            InventoryManager.selectedInventory = transform.GetComponentInParent<InventoryBehaviour>(true);
            previousPos = getPositionIndex(InventoryManager.selectedInventory);
            setPriority();
            base.OnPointerDown(eventData);

            // Previous Position calculated so it knows what to snap back to should it fail
        }
        private void setPriority()
        {
            transform.SetAsLastSibling();
            transform.parent.parent.SetAsLastSibling();
            transform.parent.parent.parent.SetAsLastSibling();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (InventoryManager.currentItem == null || InventoryManager.currentBackpack == null)
                return;

            InventoryBehaviour inventory = getInventoryToAdd();
            System.Tuple<int, int> newPos = getPositionIndex(inventory);

            if (inventory != InventoryManager.selectedInventory || !previousPos.Equals(newPos))
                addItem(newPos, inventory);
            else
                returnItem();

            InventoryManager.currentItem = null;
        }
        #endregion

        #region Methods
        private InventoryBehaviour getInventoryToAdd()
        {
            System.Tuple<int, int> newPos = getPositionIndex(InventoryManager.selectedInventory);

            if (InventoryManager.selectedInventory.outOfBounds(newPos, objectSize))
                return InventoryManager.Instance.findClosestInventory(this);

            return InventoryManager.selectedInventory;
        }
        public virtual void addItem(System.Tuple<int, int> newPos, InventoryBehaviour newInv)
        {           
            InventoryManager.selectedInventory.clearGrid(previousPos, objectSize);

            transform.SetParent(newInv.transform.GetChild(newInv.transform.childCount - 1));
            transform.SetAsLastSibling();

            if (newInv.addItemToSlot(gameObject, newPos, itemOffset)) return;

            if (newInv.outOfBounds(getPositionIndex(newInv), objectSize))
                UIManager.playerUI.enableDropItemPopup(this);

            returnItem();
        }

        public virtual void returnItem()
        {
            InventoryManager.selectedInventory.clearGrid(previousPos, objectSize);
            InventoryManager.selectedInventory.addItemToSlot(gameObject, previousPos, itemOffset);
        }

        #region Utilities
        public Vector3 getCornerPosition()
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetLocalCorners(corners);
            return corners[0] + new Vector3(InventoryManager.offset / 2, InventoryManager.offset / 2);

            // Gets the local corner of the object so the grid width and height can be calculated properly when adding an item to the backpack,
            // The extra Vector has to be added to allow some room between the slots and remove this effect for small items,
            // Fuck me this took ages I'm really dumb and shouldn't be making games lol.
        }

        public System.Tuple<int, int> getPositionIndex(InventoryBehaviour inventory)
        {
            Vector3 pos = transform.localPosition;
            Transform itemChild = inventory.transform.GetChild(inventory.transform.childCount - 1);
            if (itemChild != transform.parent)
            {
                Transform currentTransform = transform;
                transform.SetParent(itemChild);
                pos = transform.localPosition;
                transform.SetParent(currentTransform);
            }

            //Debug.Log(roundPositionToOffset(pos + getCornerPosition()).a + " " 
              //  + roundPositionToOffset(pos + getCornerPosition()).b);
            return roundPositionToOffset(pos + getCornerPosition());
        }

        public System.Tuple<int, int> roundPositionToOffset(Vector3 position)
        {
            float offset = InventoryManager.offset;
            //Rounds to the nearest int for the bool array
            return new System.Tuple<int, int>
            (Mathf.RoundToInt(position.x / offset),
            Mathf.RoundToInt(position.y / offset));
        }

        public void destroyItem(InventoryBehaviour inventory, bool atPrevious = false)
        {
            if (atPrevious)
                inventory.clearGrid(previousPos, objectSize);
            else
                inventory.clearGrid(getPositionIndex(inventory), objectSize);

            Destroy(gameObject);
        }
        #endregion
        #endregion
    }
}
