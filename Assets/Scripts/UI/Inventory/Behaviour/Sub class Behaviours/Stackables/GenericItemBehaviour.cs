using UnityEngine;
using TMPro;

namespace GatherGame.Inventory.Behaviour
{
    public class GenericItemBehaviour : ItemBehaviour
    {
        #region Variables
        public int stackMax;
        public int currentStack { get; protected set; }

        public override void setStats(Scriptables.ItemClass scriptable)
        {
            stackMax = ((Scriptables.GenericItem)scriptable).stackMax;
            base.setStats(scriptable);
        }
        #endregion

        #region Methods
        public bool modifyStackSize(int mod)
        {
            int incremental = mod / Mathf.Abs(mod);
            for (int i = 0; i < Mathf.Abs(mod); i++)
            {
                if (currentStack + incremental < 0 || currentStack + incremental > stackMax)
                { updateText(); return false; }

                currentStack += incremental;
            }

            updateText();
            return true;
        }
        public override void returnItem()
        {
            InventoryBehaviour inventory = InventoryManager.selectedInventory;
            if (inventory.outOfBounds(getPositionIndex(inventory), objectSize))
                inventory = InventoryManager.Instance.findClosestInventory(this);

            GameObject mergeObject = inventory.findItemAtPos(this, getPositionIndex(inventory));

            if (!compareObjects(mergeObject))
            { base.returnItem(); return; }

            GenericItemBehaviour mergeBehaviour = mergeObject.GetComponent<GenericItemBehaviour>();

            if (mergeBehaviour.isFull())
            { base.returnItem(); return; }

            int stackSpace = stackMax - mergeBehaviour.currentStack;

            if (currentStack > stackSpace)
            {
                mergeBehaviour.modifyStackSize(stackSpace);
                modifyStackSize(-stackSpace);
                base.returnItem();
                return;
            }

            mergeBehaviour.modifyStackSize(currentStack);
            modifyStackSize(-currentStack);
            destroyItem(inventory, true);
        }

        #region Utilities
        public virtual bool compareObjects(GameObject obj)
        {
            if (obj == null || obj.name != gameObject.name)
                return false;

            return true;
        }

        public bool isFull()
        {
            if (currentStack == stackMax)
                return true;

            return false;
        }
        public bool isEmpty()
        {
            if (currentStack == 0)
            { destroyItem(transform.GetComponentInParent<InventoryBehaviour>(true)); return true; }

            return false;
        }

        public void updateText()
        {
            transform.GetChild(transform.childCount - 1).gameObject.GetComponent<TextMeshProUGUI>().text
                = currentStack.ToString();
        }
        #endregion
        #endregion
    }
}
