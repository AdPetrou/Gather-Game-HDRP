using UnityEngine;
using GatherGame.Inventory;
using GatherGame.Interaction;
using GatherGame.UI;

namespace GatherGame.Managers
{
    public class bootstrapManagers : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            UIManager bootUI = UIManager.Instance;
            InteractionManager bootHarvest = InteractionManager.Instance;
            InventoryManager bootInventory = InventoryManager.Instance;
        }
    }
}
