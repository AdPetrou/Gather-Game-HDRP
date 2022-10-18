using UnityEngine;
using TMPro;
using System.Collections.Generic;
using static UnityEditor.Progress;

namespace GatherGame.Inventory
{
    public class GenericItemBehaviour : ItemBehaviour
    {
        #region Variables
        public int stackMax;
        public int currentStack { get; protected set; }
        public bool isFull
        {
            get
            {
                if (currentStack == stackMax)
                    return true;

                return false;
            }
        }

        public override void SpawnItem(ItemScriptable scriptable, InventoryBehaviour inventory)
        {
            stackMax = ((GenericItemScriptable)scriptable).stackMax;
            List<GenericItemBehaviour> items = inventory.GetItemsOfType<GenericItemBehaviour>(scriptable.GetName());
            if (items.Count > 0)
            {
                GenericItemBehaviour item = GetEmptyItemFromList(items);
                if (item) { item.ModifyStackSize(1); return; }
            }
            base.SpawnItem(scriptable, inventory);
            ModifyStackSize(1);
        }
        #endregion

        #region Methods
        protected virtual T GetEmptyItemFromList<T>(List<T> itemList) where T : GenericItemBehaviour
        {
            for (int i = 0; i < itemList.Count; i++)
                if (itemList[i].currentStack != stackMax)
                    return itemList[i];

            return null;
        }

        public bool ModifyStackSize(int mod)
        {
            int incremental = mod / Mathf.Abs(mod);
            for (int i = 0; i < Mathf.Abs(mod); i++)
            {
                //Debug.Log(currentStack);
                if (currentStack + incremental <= 0 || currentStack + incremental > stackMax)
                { UpdateText(); return false; }

                currentStack += incremental;
            }

            UpdateText();
            return true;
        }
        

        #region Utilities
        public virtual bool compareObjects(GameObject obj)
        {
            if (obj == null || obj.name != gameObject.name)
                return false;

            return true;
        }

        public void UpdateText()
        {
            transform.GetComponentInChildren<TextMeshProUGUI>().text
                = currentStack.ToString();
        }
        #endregion
        #endregion
    }
}
