using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GatherGame.UI
{
    public class DestroyItem : MonoBehaviour
    {
        private Inventory.Behaviour.ItemBehaviour item;

        public void disableThis()
        {
            gameObject.SetActive(false);
        }
        public void destroyItem()
        {
            item.destroyItem(item.transform.GetComponentInParent<Inventory.Behaviour.InventoryBehaviour>(true));
            disableThis();
        }
        public void setItem(Inventory.Behaviour.ItemBehaviour _item)
        {
            item = _item;
        }
        public void setItem(GameObject _item)
        {
            item = _item.GetComponent<Inventory.Behaviour.ItemBehaviour>();
        }
    }
}