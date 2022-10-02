using GatherGame.UI;
using GatherGame.Inventory.Behaviour;

namespace GatherGame.Inventory 
{
    public class CloseInventory : CloseUI
    {
        public override void disable()
        {
            InventoryBehaviour inventory = GetComponentInParent<InventoryBehaviour>();
            if (inventory.Equals(InventoryManager.currentBackpack))
            {
                GetComponentInParent<InventoryBehaviour>().closeInventory(false);
                return;
            }

            GetComponentInParent<InventoryBehaviour>().closeInventory();
        }
    } 
}
