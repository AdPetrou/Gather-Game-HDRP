using UnityEngine;
using UnityEngine.UI;
using GatherGame.Inventory.Behaviour;

namespace GatherGame.Inventory.Scriptables
{
    [CreateAssetMenu(fileName = "Basic Inventory", menuName = "Inventories")]
    public class InventoryScriptable : ScriptableObject
    {
        #region Variables
        public string inventoryName;
        public int width, height;
        public float stackModifier;
        public Sprite inventorySprite, 
            slotSprite, closeSprite;
        public ItemType validItems;
        #endregion

        #region Unity Functions
        protected virtual void Awake()
        {

        }
        #endregion

        #region Create Prefab
        public GameObject createInventory()
        {
            //Main Object
            GameObject obj = new GameObject();
            obj.name = inventoryName;           

            InventoryBehaviour behaviour = obj.AddComponent<InventoryBehaviour>(); behaviour.setStats(this);
            Image image = obj.AddComponent<Image>(); image.sprite = inventorySprite;

            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            float offset = InventoryManager.offset;
            rect.sizeDelta = new Vector2(width * offset, height * offset);
            rect.position += new Vector3((offset / 2) - (offset / 10), offset / 2 - (offset / 10));

            populateInventory(obj);
            return obj;
        }

        public void populateInventory(GameObject parentObj)
        {
            float offset = InventoryManager.offset;

            #region Header Object
            GameObject behaviourObj = new GameObject();
            RectTransform behaviourRect = behaviourObj.AddComponent<RectTransform>();

            behaviourObj.name = "Header";
            behaviourObj.transform.SetParent(parentObj.transform);
            UI.DraggableParent behaviour = behaviourObj.AddComponent<UI.DraggableParent>();
            behaviour.height = -new Vector3(0, ((height * offset) / 2) + offset / 5f);
            // This height is for the Header bar at the top that makes the Object movable,
            // I'll probably add the name to it later.
            Image image = behaviourObj.AddComponent<Image>();
            image.sprite = inventorySprite;

            behaviourRect.anchoredPosition = new Vector2(0.5f, 1f);
            behaviourRect.pivot = new Vector2(0.5f, 0.5f);
            behaviourRect.localPosition = new Vector2(0, ((height * offset) / 2) + offset / 5f);
            behaviourRect.sizeDelta = new Vector2(width * offset, offset / 2.5f);
            #endregion

            #region Close Object
            GameObject closeObj = new GameObject();
            RectTransform closeRect = closeObj.AddComponent<RectTransform>();

            closeObj.name = "Closer";
            closeObj.transform.SetParent(parentObj.transform);
            closeObj.AddComponent<CloseInventory>();
            Image closeImage = closeObj.AddComponent<Image>();
            closeImage.sprite = closeSprite;

            closeRect.anchoredPosition = new Vector2(1f, 1f);
            closeRect.pivot = new Vector2(0.5f, 0.5f);
            closeRect.localPosition = new Vector2(((width * offset) / 2) - offset / 5f, ((height * offset) / 2) + offset / 5f);
            closeRect.sizeDelta = new Vector2(offset / 2.5f, offset / 2.5f);
            #endregion

            #region Slots Object
            GameObject slotsObj = new GameObject();
            RectTransform slotsRect = slotsObj.AddComponent<RectTransform>();

            slotsObj.name = "Slots";
            slotsObj.transform.SetParent(parentObj.transform);

            slotsRect.anchoredPosition = new Vector2(0.5f, 0.5f);
            slotsRect.pivot = new Vector2(0.5f, 0.5f);
            slotsRect.localPosition = new Vector2((-(width * offset) / 2) + (offset / 2), (-(height * offset) / 2) + (offset / 2));
            addSlots(slotsObj, offset);
            #endregion

            #region Items Object
            GameObject itemsObj = new GameObject();
            RectTransform itemsRect = itemsObj.AddComponent<RectTransform>();

            itemsObj.name = "Items";
            itemsObj.transform.SetParent(parentObj.transform);

            itemsRect.anchoredPosition = new Vector2(0.5f, 0.5f);
            itemsRect.pivot = new Vector2(0.5f, 0.5f);
            itemsRect.localPosition = new Vector2((-(width * offset) / 2) + (offset / 2), (-(height * offset) / 2) + (offset / 2));
            #endregion
        }

        public void addSlots(GameObject parentObj, float offset)
        {
            // Probably unecessary and should be baked into the design but when in rome,
            // I'll change if this causes performance issues but probably not.

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    #region Slot Object
                    GameObject obj = new GameObject();
                    obj.name = "Slot";
                    obj.transform.SetParent(parentObj.transform);

                    obj.AddComponent<Image>();
                    obj.GetComponent<Image>().sprite = slotSprite;

                    RectTransform rect = obj.GetComponent<RectTransform>();
                    rect.anchoredPosition = new Vector2(0.5f, 0.5f);
                    rect.pivot = new Vector2(0.5f, 0.5f);
                    rect.sizeDelta = new Vector2(offset, offset);
                    rect.localPosition = new Vector2(x * offset, y * offset);
                    #endregion
                }
            }
        }
        #endregion
    }
}