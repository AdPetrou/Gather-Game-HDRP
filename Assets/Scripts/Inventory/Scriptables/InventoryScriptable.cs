using System;
using UnityEngine;
using UnityEngine.UI;

namespace GatherGame.Inventory
{
    [CreateAssetMenu(fileName = "Basic Inventory", menuName = "Inventories")]
    public class InventoryScriptable : ScriptableObject
    {
        protected struct ObjectData
        { 
            // Much like CreateObject, this doesn't help that much but it simplifies it a bit
            public Vector2 anchor { get; }
            public Vector2 pivot { get; }
            public Vector2 size { get; }
            public Vector2 position { get; }

            public ObjectData(Vector2 size, Vector2? position = null,
                Vector2? anchor = null, Vector2? pivot = null)
            {
                this.size = size;

                if (anchor == null)
                    this.anchor = new Vector2(0.5f, 0.5f);
                else
                    this.anchor = (Vector2)anchor;

                if (pivot == null)
                    this.pivot = new Vector2(0.5f, 0.5f);
                else
                    this.pivot = (Vector2)pivot;

                if (position == null)
                    this.position = new Vector2(0f, 0f);
                else
                    this.position = (Vector2)position;
            }

            public RectTransform SetRect(RectTransform rect)
            {
                rect.anchoredPosition = anchor; rect.pivot = pivot;
                rect.sizeDelta = size; rect.localPosition = position;
                return rect;
            }
        }

        #region Variables
        [MyBox.Separator][Header("--Data--")]
        [SerializeField] protected string inventoryName;
        [Tooltip("(in slots)")][Range(1, 30)]
        [SerializeField] protected int width, height;
        public Tuple<int, int> objectSize { get { return Tuple.Create(width, height); } }

        [MyBox.Separator][Header("--Sprites--")]
        [SerializeField] protected Sprite inventorySprite;
        [SerializeField] protected Sprite slotSprite;
        [SerializeField] protected Sprite closeSprite;
        protected Vector2 offset { get { return new Vector2((width - 1) / 2f, (height - 1) / 2f); } }
        // (width - 1) / 2 && (height - 1) / 2, is the offset from the middle of the object
        #endregion

        #region Create Prefab
        public GameObject CreateInventory(Transform parent)
        {
            #region Canvas
            GameObject canvas =
            CreateObject
            (
                inventoryName,
                new System.Type[]
                {
                    typeof(Canvas),
                    typeof(CanvasScaler),
                    typeof(GraphicRaycaster),
                },
                new ObjectData
                (
                    new Vector2(width * InventoryManager.Instance.inventoryScale, // Size
                    height * InventoryManager.Instance.inventoryScale),
                    new Vector2(Screen.width / 2f, Screen.height / 2f) // Position
                ),
                parent
            );
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            #endregion
            #region Inventory
            GameObject inventory =
            CreateObject
            (
                inventoryName,
                new System.Type[]
                {
                    typeof(Image),
                    typeof(InventoryBehaviour),
                },
                new ObjectData
                (
                    new Vector2(width, height) // Size
                    * InventoryManager.Instance.inventoryScale,
                    new Vector2(0, 0) // Position
                ),
                canvas.transform, inventorySprite
            );
            inventory.GetComponent<InventoryBehaviour>().SpawnInventory(this);
            #endregion
            #region Slots
            GameObject slotParent = new GameObject("Slots");
            slotParent.transform.SetParent(inventory.transform, false);
            slotParent.transform.localPosition = -offset * InventoryManager.Instance.inventoryScale; 
            // Slot parents position is set to the middle of the bottom left slot

            GameObject[,] slots = new GameObject[width, height];
            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                slots[x, y] = CreateObject
                (
                    "Slot", typeof(Image),
                    new ObjectData
                    (
                        Vector2.one *  // Size
                        InventoryManager.Instance.inventoryScale,
                        new Vector2(x , y) // Position
                        * InventoryManager.Instance.inventoryScale
                    ),
                    slotParent.transform, slotSprite
                );
            }
            #endregion
            #region Item Header
            GameObject items = new GameObject("Items"); // Just an empty object to parent any added items
            items.transform.parent = inventory.transform;
            items.transform.localPosition = slotParent.transform.localPosition;
            #endregion
            #region Drag Bar
            float barOffset = 4f;

            GameObject dragBar = CreateObject
            (
                "Drag Bar",
                new System.Type[]
                {
                    typeof(UI.DraggableUI),
                    typeof(Image)
                },
                new ObjectData
                (
                    new Vector2(width * InventoryManager.Instance.inventoryScale, // Size
                    InventoryManager.Instance.inventoryScale / barOffset),
                    // Divide by 10 so the bar is a tenth of the size of the Scale
                    new Vector2(inventory.transform.localPosition.x, inventory.transform.localPosition.y // Position
                    + (height * InventoryManager.Instance.inventoryScale) / 2f),
                    new Vector2(0.5f, 1f) // Anchor
                ),
                inventory.transform, inventorySprite
            );
            dragBar.GetComponent<UI.DraggableUI>().SetObject(inventory);
            dragBar.transform.SetAsFirstSibling();
            #endregion
            #region Close Bar
            CreateObject
            (
                "Close",
                new System.Type[]
                {
                    typeof(UI.CloseUI),
                    typeof(Image)
                },
                new ObjectData
                (
                    new Vector2(InventoryManager.Instance.inventoryScale / barOffset, // Size
                    InventoryManager.Instance.inventoryScale / barOffset),
                    // Divide by 10 so the bar is a tenth of the size of the Scale
                    new Vector2(dragBar.transform.localPosition.x + ((width - 1f / barOffset) * 
                    InventoryManager.Instance.inventoryScale) / 2f, 0), // Position
                    new Vector2(1, 1) // Anchor
                ),
                dragBar.transform, closeSprite
            ).GetComponent<UI.CloseUI>().SetObject(canvas);
            #endregion
            return inventory;          
        }

        protected GameObject CreateObject(string name, System.Type[] components, 
            ObjectData data, Transform parent, Sprite sprite = null)
        {
            // This doesn't really make the code cleaner, I thought it would
            // It helps a little bit I guess

            GameObject gameObject = new GameObject(name, components);
            data.SetRect(gameObject.GetComponent<RectTransform>()).SetParent(parent, false);

            foreach(System.Type component in components)
                if(component == typeof(Image) && sprite != null) 
                { gameObject.GetComponent<Image>().sprite = sprite; }

            return gameObject;
        }

        protected GameObject CreateObject(string name, System.Type components, 
            ObjectData data, Transform parent, Sprite sprite = null)
        {
            // Kind of unecessary

            GameObject gameObject = new GameObject(name, components);
            data.SetRect(gameObject.GetComponent<RectTransform>()).SetParent(parent, false);

            if (components == typeof(Image) && sprite != null) 
            { gameObject.GetComponent<Image>().sprite = sprite; }

            return gameObject;
        }
        #endregion
    }
}