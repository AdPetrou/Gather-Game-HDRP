using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GatherGame.Actors.Player;
using GatherGame.Actors.Stats;
using GatherGame.Inventory;
using GatherGame.Inventory.Behaviour;

namespace GatherGame.UI
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField]
        private HarvestUI harvestUI;
        [SerializeField]
        private DestroyItem destroyItemPopup;
        [SerializeField]
        private Transform persistantStatsParent;
        private Transform previousStatParent;

        private PlayerBehaviour behaviour;

        // Start is called before the first frame update
        void Start()
        {
            behaviour = gameObject.GetComponent<PlayerBehaviour>();
            harvestUI.gameObject.SetActive(false);
            destroyItemPopup.gameObject.SetActive(false);
            Invoke("setPersistantStats", 0.1f);
        }

        // Update is called once per frame
        void Update()
        {
            if (PlayerVariables.controls.UI.Inventory.triggered)
                inventoryPressed();

            if (PlayerVariables.controls.Player.Movement.triggered)
                closeInventories();

            if (PlayerVariables.controls.UI.Stats.triggered)
                playerStatsPressed();

            if (behaviour.interactableObject)
            {
                enableHarvestUI(behaviour.interactableObject.transform.position + Vector3.up);
                if (behaviour.CurrentState.StateRef == StateType.Gather)
                    runHarvestUI(Interaction.InteractionManager.Instance.GetObjectFromTag(behaviour.interactableObject.tag).harvestTime);
            }
            else
                disableHarvestUI();
        }

        public void enableHarvestUI(Vector3 position)
        {
            harvestUI.enable(position);
        }
        public void disableHarvestUI()
        {
            harvestUI.disable();
        }
        public void runHarvestUI(int time)
        {
            StartCoroutine(harvestUI.fillUI(time));
        }

        public void enableDropItemPopup(ItemBehaviour item)
        {
            UIManager.Instance.setActivePopup(destroyItemPopup.gameObject);
            destroyItemPopup.setItem(item);
        }

        public void inventoryPressed()
        {
            StartCoroutine(PlayerBehaviour.disableMovement(0.5f));
            if (!InventoryManager.currentBackpack.status)
            {
                InventoryManager.currentBackpack.openInventory();
                return;
            }

            InventoryManager.currentBackpack.closeInventory(false);
        }

        public void playerStatsPressed()
        {
            StatBehaviour playerStats = gameObject.GetComponent<StatBehaviour>();
            if (playerStats.activeCall())
            {
                foreach (ResourceStatBehaviour r in playerStats.resourceStats)
                {
                    r.thisObject.SetParent(previousStatParent);
                    r.thisObject.gameObject.SetActive(true);
                }
            }
            else
                setPersistantStats();
        }
        public void setPersistantStats()
        {          
            StatBehaviour playerStats = gameObject.GetComponent<StatBehaviour>();
            foreach (ResourceStatBehaviour r in playerStats.resourceStats)
            {
                previousStatParent = r.thisObject.parent;
                r.thisObject.SetParent(persistantStatsParent);
                r.thisObject.gameObject.SetActive(true);
            }
        }

        public void closeInventories()
        {
            Inventory.Behaviour.InventoryBehaviour[] inventories = FindObjectsOfType<Inventory.Behaviour.InventoryBehaviour>();
            if (inventories.Length < 2 && !InventoryManager.currentBackpack.status)
                return;

            for (int i = 0; i < inventories.Length; i++)
            {
                if (inventories[i] == InventoryManager.currentBackpack
                    && InventoryManager.currentBackpack.status)
                    inventories[i].closeInventory(false);
                else
                    inventories[i].closeInventory();
            }
        }
    }
}