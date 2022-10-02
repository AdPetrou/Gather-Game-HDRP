using UnityEngine;
using TMPro;
using GatherGame.Inventory.Behaviour;

namespace GatherGame.Inventory.Scriptables
{
    [CreateAssetMenu(fileName = "Generic Item", menuName = "Items/Generic Item")]
    public class GenericItem : ItemClass
    {
        public int stackMax;

        #region Unity Functions
        protected override void Awake()
        {
            base.Awake();
        }
        #endregion

        #region Create Prefab
        public override GameObject createItem(InventoryBehaviour inventory)
        {
            GameObject obj = base.createItem(inventory);

            #region Text Object
            GameObject textObj = new GameObject();
            textObj.transform.SetParent(obj.transform);
            textObj.transform.SetAsLastSibling();
            textObj.name = "Text";
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.alignment = TextAlignmentOptions.BottomRight;
            text.fontSize = InventoryManager.offset / 2;
            text.font = Resources.Load<TMP_FontAsset>("");
            text.text = "0";

            textObj.GetComponent<RectTransform>().sizeDelta = new Vector2
            (InventoryManager.offset * width,
            (InventoryManager.offset * height));
            #endregion

            return obj;
        }

        #region Behaviour
        protected override void addBehaviour(GameObject obj)
        {
            GenericItemBehaviour data = obj.AddComponent<GenericItemBehaviour>();
            data.setStats(this);
        }
        #endregion
        #endregion
    }
}
