using System;
using System.Collections.Generic;
using UnityEngine;
using GatherGame.Utilities;
using UnityEditor.Search;
using System.Collections;
using UnityEngine.EventSystems;

namespace GatherGame.Inventory
{
    public class InventoryBehaviour : EventTrigger
    {
        #region Variables
        private int width, height;
        public int index { get; private set; } 
        // This index is assigned in the Inventory Manager when it is added to the inventory list
        // need this to find the inventory I want in a list that may have shifting indexes

        public List<ItemBehaviour> items { get; private set; }
        // This holds all the items currently in the Inventory
        public bool[,] slotStatus
        {
            get
            {
                RefreshItems();
                bool[,] returnResult = new bool[width, height];

                for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    returnResult[x, y] = false;

                for (int i = 0; i < items.Count; i++)
                    if (items[i].usedSlots != null)
                    foreach (Tuple<int, int> index in items[i].usedSlots.Item2)
                        returnResult[index.Item1, index.Item2] = true;

                return returnResult;
            }
        }
        // This checks all the items in the item list (and removes the items no longer in the inventory)
        // it creates a bool array that checks if the slots are full or not and returns it

        private bool firstOpen = true;
        public bool FirstOpen { get { return firstOpen; } }
        // Used for the Stat exp, will move if possible
        protected Vector2 offset { get { return new Vector2((width - 1) / 2f, (height - 1) / 2f); } }
        // This is the offset needed to transform the center of the gameObject into a grid
        // Multiply by Inventory Scale if needed for Vector

        public void SpawnInventory(InventoryScriptable scriptable)
        {
            width = scriptable.objectSize.Item1; 
            height = scriptable.objectSize.Item2;
            InventoryManager.Instance.inventories.Add(this);
            index = InventoryManager.Instance.GetUniqueID();
        }
        #endregion

        #region Unity Methods
        public void Awake()
        {
            items = new List<ItemBehaviour>();
            firstOpen = true;

            // Need this trigger to set canvas priority for overlapping inventories
            EventTrigger trigger = this;
            Entry entry = new Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            trigger.triggers.Add(entry);
        }
        #endregion

        #region Methods
        public void ToggleInventory()
        {
            if (firstOpen && gameObject.activeSelf)
                firstOpen = false;
            // Gets the parent (canvas) and deactivates that instead of the object to keep UI clean
            GetComponentInParent<Canvas>(true).gameObject.SetActive(!transform.parent.gameObject.activeSelf);
        }

        public bool AddItemToSlot(ItemBehaviour item, Tuple<int, int> position = null, 
            Queue<Tuple<int, int>> sizeOffset = null)
        {
            // Safety check
            if (!item)
                return false;

            // Position and Offset are not null if the object is being dragged and the origin is not the bottom left
            if(position == null)
                item.SetUsedSlots(this, GetEmptyPosition(item));
            else
                item.SetUsedSlots(this, GetGridPositions(position, sizeOffset));

            // If the used slots aren't null then the item will be added to the Inventory
            // If they are null then the Inventory is either full or the position was already filled
            if (item.usedSlots.Item2 != null)
            {
                item.transform.SetParent(transform.Find("Items"));
                items.Add(item); return true;
            }
            else
                return false;
        }

        public bool AddItemToSlot(ItemBehaviour item, List<Tuple<int, int>> position)
        {
            // This isn't the same position as the other function
            // This is to force the position (even if it is full)
            // this could lead to overlapping objects if I'm not careful

            item.SetUsedSlots(this, position);

            if (item.usedSlots.Item2 != null)
            {
                item.transform.SetParent(transform.Find("Items"));
                items.Add(item); return true;
            }
            else
                return false;
        }

        protected List<Tuple<int, int>> GetEmptyPosition(ItemBehaviour item)
        {
            // Gets an empty grid slot and checks if the item fits

            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                if (!slotStatus[x, y])
                {
                    List<Tuple<int, int>> result = 
                        GetGridPositions(Tuple.Create(x, y), item.objectSize);

                    if (result != null)
                        return result;
                }
            }

            return null;
        }

        protected List<Tuple<int, int>> GetGridPositions(Tuple<int, int> position, Tuple<int, int> itemSize)
        {
            // Returns a list of the slots the item can fit in, if it can't then it returns null
            List<Tuple<int, int>> result = new List<Tuple<int, int>>();
            for (int x = 0; x < itemSize.Item1; x++)
            for (int y = 0; y < itemSize.Item2; y++)
            {
                if (slotStatus[position.Item1 + x, position.Item2 + y])
                    return null;

                result.Add(Tuple.Create(position.Item1 + x, position.Item2 + y));
            }

            return result;
        }

        protected List<Tuple<int, int>> GetGridPositions(Tuple<int, int> position, Queue<Tuple<int, int>> itemSize)
        {
            // Similar to the other one except this is used when the item is offset from the bottom left corner
            // The item size is the directions, from the position, that the item overlaps

            // Might not need to copy the queue but it will be reused if this fails
            Queue<Tuple<int, int>> queueCopy = new Queue<Tuple<int, int>>(itemSize);
            List<Tuple<int, int>> result = new List<Tuple<int, int>>();
            while (queueCopy.Count > 0)
            {
                Tuple<int, int> offset = queueCopy.Dequeue();
                // Returns if the item is out of bounds
                if (position.Item1 + offset.Item1 > width - 1 || position.Item2 + offset.Item2 > height - 1
                    || position.Item1 + offset.Item1 < 0 || position.Item2 + offset.Item2 < 0)
                    return null;

                if (slotStatus[position.Item1 + offset.Item1, position.Item2 + offset.Item2])
                    return null;

                result.Add(Tuple.Create(position.Item1 + offset.Item1, position.Item2 + offset.Item2));
            }

            return result;
        }

        public Tuple<int, int> GetPositionFromVector(Vector2 point, bool isWorldPos = false)
        {
            // Transforms a vector into a grid position
            if (isWorldPos)
            {
                Vector2 localPos = transform.InverseTransformPoint(point);
                return Tuple.Create(Mathf.RoundToInt(localPos.x / InventoryManager.Instance.inventoryScale + offset.x),
                     Mathf.RoundToInt(localPos.y / InventoryManager.Instance.inventoryScale + offset.y));
            }
            else
                return Tuple.Create(Mathf.RoundToInt(point.x / InventoryManager.Instance.inventoryScale + offset.x),
                    Mathf.RoundToInt(point.y / InventoryManager.Instance.inventoryScale + offset.y));
        }

        public List<T> GetItemsOfType<T>(string name) where T : ItemBehaviour
        {
            List<T> result = new List<T>();
            foreach(T item in items)
                if(item.name == name) result.Add(item);

            return result;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            InventoryManager.Instance.InventoryToTop(this);
        }

        private void RefreshItems()
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].usedSlots.Item1 == null
                    || items[i].usedSlots.Item1 != this)
                { items.RemoveAt(i);  i--; }
            }
        }
        #endregion
    }
}
