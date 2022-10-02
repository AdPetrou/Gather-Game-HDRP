using System.Collections.Generic;
using UnityEngine;
using GatherGame.Utilities;
using GatherGame.Inventory.Scriptables;

namespace GatherGame.Inventory.Behaviour
{
    public class InventoryBehaviour : MonoBehaviour
    {
        #region Variables
        private int width, height;
        private ItemType validItems;
        private Vector2 closedPosition;

        protected bool[][] slots = null;
        public bool[][] Slots
        {
            get
            {
                if (slots == null)
                {
                    slots = new bool[width][];
                    for (int x = 0; x < width; x++)
                    {
                        slots[x] = new bool[height];

                        for (int y = 0; y < height; y++)
                        {
                            slots[x][y] = false;
                        }
                    }
                }

                return slots;
            }

            protected set
            {
                slots = value;
            }
        }

        public bool status { get; private set; }
        protected bool firstOpen;
        public System.Tuple<int, int> getBounds() { return System.Tuple.Create(width, height); }

        public void setStats(InventoryScriptable scriptable)
        {
            width = scriptable.width; height = scriptable.height;
            validItems = scriptable.validItems;
            bool bootstrapSlots = Slots[0][0];
        }
        #endregion

        #region Unity Methods
        public void Awake()
        {
            firstOpen = true;
            slots = null;
            status = gameObject.activeSelf;
        }
        #endregion

        #region Methods
        public bool isFirstOpen() { if (firstOpen) { firstOpen = false; return true; } else return false; }
        public bool addItemToSlot(GameObject item, System.Tuple<int, int> index, Vector2 offset)
        {
            ItemBehaviour data = item.GetComponent<ItemBehaviour>();
            if (data.itemType == validItems || validItems == ItemType.Generic)
            {
                if (checkGrid(index, data.objectSize))
                {
                    //Debug.Log("Add Item to Slot");
                    addToGrid(index, data.objectSize);
                    item.GetComponent<RectTransform>().localPosition = new Vector2(
                        index.Item1 * InventoryManager.offset, index.Item2 * InventoryManager.offset) + offset;
                    return true;
                }
            }

            return false;
        }

        public GameObject spawnItem(ItemClass item)
        {
            GameObject obj = item.createItem(this);
            ItemBehaviour behaviour = obj.GetComponent<ItemBehaviour>();
            addItemToSlot(obj, findEmptySlot(behaviour.objectSize), behaviour.itemOffset);

            return obj;
        }

        public GameObject spawnGhostItem(object item)
        {
            GameObject obj = null;
            if (typeof(HarvestableItem).IsAssignableFrom(item.GetType()))
            {
                // Needed to differentiate between Quality types in Recipe objects
                // This creates a dummy item that will be deleted but acts as a reference to the Quality type
                // The Quality Type will partly be RNG based, requiring this bypass, I also want to use the same function

                obj = spawnItem((HarvestableItem)item);
                obj.name = "Ignore Filter";
            }
            return obj;
        }

        public void modStackableItem(GenericItem item, int itemAmount, GameObject ghostObj)
        {
            List<Transform> itemList;
            if (itemAmount < 0)
                itemList = filterEmptyStacks(getItemsOfType(item));
            else
                itemList = filterFullStacks(getItemsOfType(item));

            bool complete = false;
            while (!complete)
            {
                float result;

                //////////////////////////////////////////////////////////////////////

                // This should never be possible
                if (itemList.Count <= 0 && itemAmount < 0)
                {
                    Debug.LogError("Something went wrong"); return;
                }

                // If no similar items
                if (itemList.Count <= 0 && itemAmount > 0)
                {
                    // If there are no item stacks with space spawn a new one
                    GameObject newObj = Instantiate(ghostObj, ghostObj.transform.parent, true);

                    if (ghostObj == null)
                        newObj = spawnItem(item);
                    else
                    {
                        newObj.name = item.itemName;
                        ((HarvestableItem)item).createQualityIndicator(newObj.transform);
                    }
                    // The Dummy item above is duplicated and assigned the right name
                    // Quality indicator has to be regenerated because for some reason it didn't duplicate properly in Instantiate :(

                    if (stackChange(newObj.gameObject, itemAmount) == 0.1f)
                    {
                        // The new item is set to the dummy items position after it is deleted
                        ItemBehaviour behaviour = ghostObj.GetComponent<ItemBehaviour>();
                        System.Tuple<int, int> pos = behaviour.getPositionIndex(this);
                        Vector2 itemOffset = behaviour.itemOffset;
                        behaviour.destroyItem(this);
                        addItemToSlot(newObj, pos, itemOffset);
                        return;
                    }

                    itemAmount -= newObj.GetComponent<GenericItemBehaviour>().stackMax;
                    continue;
                }

                //////////////////////////////////////////////////////////////////////

                if (ghostObj != null)
                {
                    itemList = filterQualityStacks(ghostObj, itemList);
                    if (itemList.Count <= 0)
                        continue;
                }

                result = stackChange(itemList[0].gameObject, itemAmount);
                if (result == 0.1f)
                { ghostObj.GetComponent<ItemBehaviour>().destroyItem(this); return; }

                // If Subtracting items
                if (itemAmount < 0)
                {
                    itemAmount += (int)result;
                    itemList = filterEmptyStacks(getItemsOfType(item));
                    continue;
                }

                // If Adding items
                itemAmount -= itemList[0].gameObject.GetComponent<GenericItemBehaviour>().stackMax - (int)result;
                itemList = filterFullStacks(getItemsOfType(item));
                continue;

                //////////////////////////////////////////////////////////////////////             
            }
        }

        private float stackChange(GameObject gameObject, int itemAmount)
        {
            GenericItemBehaviour behaviour = gameObject.GetComponent<GenericItemBehaviour>();
            int priorStackSize = behaviour.currentStack;

            if (behaviour.modifyStackSize(itemAmount)) return 0.1f;

            behaviour.isEmpty();
            return priorStackSize;
        }

        #region Grid Functions
        public bool checkGrid(System.Tuple<int, int> objPosition, System.Tuple<int, int> objSize)
        {
            if (outOfBounds(objPosition, objSize))
                return false;

            for (int x = objPosition.Item1; x < objSize.Item1 + objPosition.Item1; x++)
            {
                for (int y = objPosition.Item2; y < objSize.Item2 + objPosition.Item2; y++)
                {
                    //Debug.Log("Check grid: " + x + " " + y);
                    if (Slots[x][y])
                        return false;
                }
            }
            return true;
        }

        public void addToGrid(System.Tuple<int, int> objPosition, System.Tuple<int, int> objSize)
        {
            for (int x = objPosition.Item1; x < objSize.Item1 + objPosition.Item1; x++)
            {
                for (int y = objPosition.Item2; y < objSize.Item2 + objPosition.Item2; y++)
                {
                    //Debug.Log("Add to grid: " + x + " " + y);
                    Slots[x][y] = true;
                }
            }
        }

        public System.Tuple<int, int> findEmptySlot(System.Tuple<int, int> objectSize)
        {
            // This will be used for initially spawning an item
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (!Slots[x][y] && checkGrid(System.Tuple.Create(x, y), objectSize))
                        return System.Tuple.Create(x, y);
                }
            }
            return null;
        }

        public GameObject findItemAtPos(ItemBehaviour itemBehaviour, System.Tuple<int, int> index)
        {
            if (outOfBounds(index, itemBehaviour.objectSize))
                return null;

            List<ItemBehaviour> items = getBehavioursOfType(itemBehaviour);
            if (items.Count < 1)
                return null;

            for (int i = 0; i < items.Count; i++)
            {
                if (index.Equals(items[i].getPositionIndex(this)) &&
                    !itemBehaviour.Equals(items[i]))
                    return items[i].gameObject;
                ;
            }

            return null;
        }

        public void clearGrid(System.Tuple<int, int> objPosition, System.Tuple<int, int> objSize)
        {
            for (int x = objPosition.Item1; x < objSize.Item1 + objPosition.Item1; x++)
            {
                for (int y = objPosition.Item2; y < objSize.Item2 + objPosition.Item2; y++)
                {
                    Slots[x][y] = false;
                }
            }
        }

        public bool outOfBounds(System.Tuple<int, int> objPosition, System.Tuple<int, int> objSize)
        {
            if (objPosition.Item1 < 0 || objPosition.Item2 < 0 ||
            objSize.Item1 + objPosition.Item1 > width || objSize.Item2 + objPosition.Item2 > height)
                return true;

            return false;
        }
        #endregion

        #region Utilities
        public List<Transform> getItemsOfType(ItemClass item)
        {
            Transform itemParent = transform.GetChild(transform.childCount - 1);

            List<Transform> filtereditems = new List<Transform>();
            for (int i = 0; i < itemParent.childCount; i++)
            {
                if (itemParent.GetChild(i).name == item.itemName)
                {
                    filtereditems.Add(itemParent.GetChild(i));
                }
            }

            return filtereditems;
        }
        public List<ItemBehaviour> getBehavioursOfType(ItemClass item)
        {
            Transform itemParent = transform.GetChild(transform.childCount - 1);

            List<ItemBehaviour> filtereditems = new List<ItemBehaviour>();
            for (int i = 0; i < itemParent.childCount; i++)
            {
                if (itemParent.GetChild(i).name == item.itemName)
                {
                    filtereditems.Add(itemParent.GetChild(i).gameObject.
                        GetComponent<ItemBehaviour>());
                }
            }

            return filtereditems;
        }
        public List<ItemBehaviour> getBehavioursOfType(ItemBehaviour itemObj)
        {
            ItemClass item = InventoryManager.Instance.getClassFromObject(itemObj.gameObject);
            Transform itemParent = itemObj.transform.parent;

            List<ItemBehaviour> filtereditems = new List<ItemBehaviour>();
            for (int i = 0; i < itemParent.childCount; i++)
            {
                if (itemParent.GetChild(i).name == item.itemName)
                {
                    filtereditems.Add(itemParent.GetChild(i).gameObject.
                        GetComponent<ItemBehaviour>());
                }
            }

            return filtereditems;
        }

        private List<Transform> filterFullStacks(List<Transform> items)
        {
            List<Transform> filteredItems = new List<Transform>();
            foreach (Transform T in items)
            {
                GenericItemBehaviour behaviour = T.gameObject.GetComponent<GenericItemBehaviour>();
                if (!behaviour.isFull())
                    filteredItems.Add(T);
            }

            return filteredItems;
        }
        private List<Transform> filterEmptyStacks(List<Transform> items)
        {
            List<Transform> filteredItems = new List<Transform>();
            foreach (Transform T in items)
            {
                GenericItemBehaviour behaviour = T.gameObject.GetComponent<GenericItemBehaviour>();
                if (!behaviour.isEmpty())
                    filteredItems.Add(T);
            }

            return filteredItems;
        }
        private List<Transform> filterQualityStacks(GameObject obj, List<Transform> items)
        {
            List<Transform> filteredItems = new List<Transform>();
            foreach (Transform T in items)
            {
                HarvestableItemBehaviour behaviour = T.gameObject.GetComponent<HarvestableItemBehaviour>();
                if (obj.GetComponent<HarvestableItemBehaviour>().quality == behaviour.quality)
                    filteredItems.Add(T);
            }

            return filteredItems;
        }
        #endregion

        #region Object State
        public void closeInventory(bool disable = true)
        {
            if (!status)
                return;

            if (disable)
                gameObject.SetActive(false);

            closedPosition = GetComponent<RectTransform>().position;
            transform.position = new Vector2(2000, 2000);
            status = false;
        }

        public void openInventory()
        {
            if (status)
                return;

            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            transform.position = closedPosition;
            status = true;
        }
        #endregion

        #endregion
    }
}
