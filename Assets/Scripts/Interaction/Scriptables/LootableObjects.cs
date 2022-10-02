using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GatherGame.Inventory.Scriptables;
using GatherGame.Utilities;
using GatherGame.Inventory.Behaviour;

namespace GatherGame.Interaction
{
    [CreateAssetMenu(fileName = "Lootable Object", menuName = "Interactable Objects/Lootable Object")]
    public class LootableObjects : InteractableObject
    {
        #region Variables
        [Header("--Inventory & Loot Data--")]
        public InventoryScriptable inventory;
        public LootTable lootTable;
        public int minimumPasses, maximumPasses; // The amount of items the object can generate
        #endregion

        #region Methods
        public int getRandomRange()
        {
            return Random.Range(minimumPasses, maximumPasses);
        }
        
        public override GameObject interact(GameObject gameObject)
        {
            InventoryBehaviour behaviour = gameObject.GetComponentInChildren<InventoryBehaviour>();

            if (behaviour == null)
            {
                if (hasBlocker(gameObject))
                    createLoot(gameObject.transform, true);   
                else
                    createLoot(gameObject.transform, false);

                Inventory.InventoryManager.inventories.Add(behaviour);
                Inventory.InventoryManager.currentBackpack.openInventory();
            }
            else
            {
                behaviour.openInventory();
                Inventory.InventoryManager.currentBackpack.openInventory();
            }

            return gameObject;
        }

        public bool hasBlocker(GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out Blocker blocker))
                return true;

            return false;
        }
        public bool activeInventory(GameObject gameObject)
        {
            if (gameObject.GetComponentInChildren<InventoryBehaviour>() == null)
                return false;

            return true;
        }

        #region Create Prefab
        public GameObject createLoot(Transform parent, bool persistant)
        {
            GameObject inventoryGO = inventory.createInventory();

            InventoryBehaviour behaviour = 
                inventoryGO.GetComponent<InventoryBehaviour>();

            inventoryGO.transform.SetParent(parent);
            inventoryGO.transform.localPosition = new Vector2((-Screen.width / 2) / 4, 0);

            if (persistant)
                return inventoryGO;

            lootLoop(behaviour);
            return inventoryGO;
        }

        private void lootLoop(InventoryBehaviour behaviour)
        {
            for (int i = 0; i < getRandomRange(); i++)
            {
                object item = lootTable.getRandomItem();
                if (typeof(GenericItem).IsAssignableFrom(item.GetType()))
                {
                    GameObject obj = behaviour.spawnGhostItem(item);

                    if (typeof(HarvestableItem).IsAssignableFrom(item.GetType()))
                    {
                        HarvestableItemBehaviour itemBehaviour = obj.GetComponent<HarvestableItemBehaviour>();
                        ((HarvestableItem)item).setRandomQuality(itemBehaviour, Quality.Average);
                        ((HarvestableItem)item).createQualityIndicator(obj.transform);
                    }

                    behaviour.modStackableItem((GenericItem)item, 1, obj);
                }
                else
                    behaviour.spawnItem((ItemClass)item);
            }
        }

        public void createPersistantGameObject()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Instantiate(prefab, new Vector3(Mathf.FloorToInt(hit.point.x), Mathf.FloorToInt(hit.point.y), Mathf.FloorToInt(hit.point.z)) 
                    + Vector3.up * 1.5f, prefab.transform.rotation, GameObject.Find("PersistantLootables").transform).AddComponent<Blocker>();               
            }             
        }
        #endregion

        #endregion
    }
}
