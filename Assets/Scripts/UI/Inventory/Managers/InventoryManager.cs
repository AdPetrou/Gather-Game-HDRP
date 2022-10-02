using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GatherGame.Utilities;
using GatherGame.Inventory.Behaviour;
using GatherGame.Inventory.Scriptables;

public enum ItemType
{
    Generic = -1,
    Harvesting = 0,
    Mining = 1,
}

namespace GatherGame.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        #region Static Variables
        public static List<InventoryBehaviour> inventories;
        public static float offset = 50;
        public static ItemBehaviour currentItem;
        public static InventoryBehaviour currentBackpack;
        public static InventoryBehaviour selectedInventory;
        #endregion
        #region Variables
        private InventoryScriptable[] backpackScriptables;
        #endregion

        #region Unity Functions
        // Start is called before the first frame update
        void Start()
        {
            currentItem = null; currentBackpack = null;
            inventories = new List<InventoryBehaviour>();
            backpackScriptables = Resources.LoadAll<InventoryScriptable>("UI/Inventory/Backpacks").ToArray();

            currentBackpack = backpackScriptables[0].createInventory().GetComponent<InventoryBehaviour>();
            currentBackpack.transform.SetParent(GameObject.Find("Persistant").transform);
            currentBackpack.transform.localPosition = new Vector2(0, 0);
            currentBackpack.closeInventory(false);

            selectedInventory = currentBackpack;
        }

        // Update is called once per frame
        void Update()
        {
            if (currentItem == null)
                return;
        }
        #endregion

        #region Methods


        #region Utilities
        public ItemClass getClassFromObject(GameObject obj)
        {
            foreach (ItemClass C in Resources.LoadAll<ItemClass>(""))
                if (C.itemName == obj.name)
                    return C;

            return null;
        }

        public InventoryBehaviour findClosestInventory(ItemBehaviour item)
        {
            InventoryBehaviour[] inventories = FindObjectsOfType<InventoryBehaviour>();
            InventoryBehaviour currentClosest = null;
            foreach(InventoryBehaviour i in inventories)
            {
                if (currentClosest == null)
                    currentClosest = i;

                if (Vector2.Distance(item.transform.position, i.transform.position)
                    < Vector2.Distance(item.transform.position, currentClosest.transform.position))
                    currentClosest = i;
            }

            return currentClosest;
        }

        public InventoryBehaviour getInventory(int index)
        {
            if (index > inventories.Count)
                return null;

            return inventories[index];
        }
        #endregion
        #endregion
    }
}
