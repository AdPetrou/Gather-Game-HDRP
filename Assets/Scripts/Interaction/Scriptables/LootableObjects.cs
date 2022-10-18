using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GatherGame.Inventory;

namespace GatherGame.Interaction
{
    [CreateAssetMenu(fileName = "Lootable Object", menuName = "Interactable Objects/Lootable Object")]
    public class LootableObject : InteractableObject
    {
        #region Variables
        [MyBox.Separator][Header("--Inventory & Loot Data--")]
        [SerializeField]private InventoryScriptable inventory;
        [SerializeField]private LootTable lootTable;
        [Range(0, 25)]
        [SerializeField]private int minimumPasses; // The amount of items the object can generate
        [Range(0, 50)]
        [SerializeField]private int maximumPasses;
        #endregion

        #region Methods
        public int getRandomRange()
        {
            return Random.Range(minimumPasses, maximumPasses);
        }
        
        public override GameObject interact(GameObject gameObject, Actors.ActorBehaviour actor)
        {
            InventoryBehaviour behaviour = gameObject.GetComponentInChildren<InventoryBehaviour>(true);

            if (behaviour == null)
            {
                behaviour = createLoot(gameObject.transform, hasBlocker(gameObject));

                InventoryManager.Instance.inventories.Add(behaviour);
                actor.GetComponentInChildren<InventoryBehaviour>(true).ToggleInventory();
            }
            else
            {
                actor.GetComponentInChildren<InventoryBehaviour>(true).ToggleInventory();
                behaviour.ToggleInventory();
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
        public InventoryBehaviour createLoot(Transform parent, bool persistant)
        {
            GameObject inventoryGO = inventory.CreateInventory(parent);

            InventoryBehaviour behaviour = 
                inventoryGO.GetComponent<InventoryBehaviour>();
            inventoryGO.transform.localPosition = new Vector2(200, 0);

            if (persistant)
                return behaviour;

            lootLoop(behaviour);
            return behaviour;
        }

        private void lootLoop(InventoryBehaviour behaviour)
        {
            for (int i = 0; i < getRandomRange(); i++)
            {
                ItemScriptable item = lootTable.getRandomItem();
                item.CreateItem(behaviour);
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
