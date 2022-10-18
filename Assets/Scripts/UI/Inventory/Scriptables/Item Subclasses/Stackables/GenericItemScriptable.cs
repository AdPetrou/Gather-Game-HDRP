using UnityEngine;
using TMPro;
using GatherGame.Inventory.Behaviour;

namespace GatherGame.Inventory
{
    [CreateAssetMenu(fileName = "Generic Item", menuName = "Items/Generic Item")]
    public class GenericItemScriptable : ItemScriptable
    {
        [Range(1, 100)]
        public int stackMax;

        #region Unity Functions
        protected override void Awake()
        {
            base.Awake();
        }
        #endregion

        #region Create Prefab
        public override GameObject CreateItem(InventoryBehaviour inventory)
        {
            GameObject obj = base.CreateItem(inventory);

            #region Text Object
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(obj.transform);
            textObj.transform.localPosition = Vector2.zero;
            textObj.transform.SetAsLastSibling();

            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.alignment = TextAlignmentOptions.BottomRight;
            text.fontSize = InventoryManager.Instance.inventoryScale / 2;
            text.font = Resources.Load<TMP_FontAsset>("");
            text.text = "0";

            textObj.GetComponent<RectTransform>().sizeDelta = new Vector2
            (InventoryManager.Instance.inventoryScale * width,
            (InventoryManager.Instance.inventoryScale * height));
            #endregion

            obj.GetComponent<GenericItemBehaviour>().SpawnItem(this, inventory);
            return obj;
        }

        #region Behaviour
        protected override void AddBehaviour(GameObject obj, InventoryBehaviour inventory)
        {
            GenericItemBehaviour data = obj.AddComponent<GenericItemBehaviour>();
            //data.SpawnItem(this, inventory);
        }
        #endregion
        #endregion
    }
}
