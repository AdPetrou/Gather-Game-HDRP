using UnityEngine;

namespace GatherGame.Inventory.Scriptables
{
    [CreateAssetMenu(fileName = "Test Item", menuName = "Items/Test Item")]
    public class TestItem : ItemClass
    {
        protected override void Awake()
        {
            base.Awake();
        }
    }
}