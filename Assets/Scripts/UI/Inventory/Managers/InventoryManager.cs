using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GatherGame.Utilities;

namespace GatherGame.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        #region Variables
        public int inventoryScale { get; private set; } = 50;
        public List<InventoryBehaviour> inventories { get; } = new List<InventoryBehaviour>();
        public InventoryBehaviour currentInventory { get; private set; }
        public GameObject itemCanvas { get; private set; }
        #endregion

        #region Unity Functions
        // Start is called before the first frame update
        void Start()
        {
            itemCanvas = GameObject.Find("Item Canvas");
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        #endregion

        #region Methods
        public int GetUniqueID()
        {
            int i = 0;
            foreach (InventoryBehaviour inventory in inventories)
                if (inventory.index > i)
                    i = inventory.index + 1;
            return i;
        }

        public void InventoryToTop(InventoryBehaviour selectedInventory)
        {
            foreach (InventoryBehaviour inventory in inventories)
                if(inventory.gameObject.activeSelf)
                    inventory.transform.parent.GetComponent<Canvas>().sortingOrder = 0;

            selectedInventory.transform.parent.GetComponent<Canvas>().sortingOrder = 1;
            currentInventory = selectedInventory;
            return;
        }

        public InventoryBehaviour GetClosestInventory(Vector2 position)
        {
            InventoryBehaviour returnValue = currentInventory;
            foreach(InventoryBehaviour inventory in inventories)
                if (inventory.gameObject.activeSelf)
                {
                    if(Vector2.Distance(position, inventory.transform.position) < 
                        Vector2.Distance(position, returnValue.transform.position))
                        returnValue = inventory;
                }

            return returnValue;
        }
        #endregion
    }
}
