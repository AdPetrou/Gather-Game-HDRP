using GatherGame.Actors.Stats;
using GatherGame.Inventory.Behaviour;
using UnityEngine;
using UnityEngine.UI;

namespace GatherGame.Inventory.Scriptables
{
    [CreateAssetMenu(fileName = "Harvest Item", menuName = "Items/Harvest Item")]
    public class HarvestableItemScriptable : RecipeItemScriptable
    {       
        public Sprite qualityIndicator;
        public SkillType skillType;

        #region Create Prefab
        public override GameObject CreateItem(InventoryBehaviour inventory)
        {
            GameObject obj = base.CreateItem(inventory);
            return obj;
        }

        public void createQualityIndicator(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                if (parent.GetChild(i).name == "Quality")
                    Destroy(parent.GetChild(i).gameObject);

            GameObject quality = new GameObject("Quality", typeof(RectTransform));
            #region Set Componenets
            quality.transform.SetParent(parent);
            quality.transform.SetAsFirstSibling();
            RectTransform rect = quality.GetComponent<RectTransform>();
            rect.pivot = new Vector2(0.5f, 0.5f); // Pivot is the centre
            rect.sizeDelta = new Vector2(width * InventoryManager.Instance.inventoryScale,
                height * InventoryManager.Instance.inventoryScale);
            rect.position = parent.position;
            #endregion

            populateQuality((int)parent.gameObject.GetComponent<HarvestableItemBehaviour>().quality / (int)Quality.Average, quality.transform);
        }

        protected void populateQuality(int amount, Transform parent)
        {
            float indicatorSize = 5f;
            for (int i = 0; i < amount; i++)
            {
                GameObject indicator = new GameObject("Indicator", typeof(Image));
                #region Set Componenets
                indicator.transform.SetParent(parent.transform);
                indicator.GetComponent<Image>().sprite = qualityIndicator;
                RectTransform indicatorRect = indicator.GetComponent<RectTransform>();
                indicatorRect.sizeDelta = new Vector2((InventoryManager.Instance.inventoryScale / indicatorSize), 
                    (InventoryManager.Instance.inventoryScale / indicatorSize));
                indicatorRect.pivot = new Vector2(0, 1);
                indicatorRect.localPosition = new Vector2(-((InventoryManager.Instance.inventoryScale / 2f) * width) + indicatorSize,
                    ((InventoryManager.Instance.inventoryScale / 2f) * height) - (InventoryManager.Instance.inventoryScale / indicatorSize) * i);

                // The local Position is based from the centre point of the item
                // Therefore the position of the corners will be half the Size of the object
                // This gets the top left corner, x is offset by the size for better visuals
                // Y is offset by the size of the indicator * i, i is the quality of the object
                #endregion
            }
        }

        #region Behaviour
        protected override void AddBehaviour(GameObject obj, InventoryBehaviour inventory)
        {
            HarvestableItemBehaviour data = obj.AddComponent<HarvestableItemBehaviour>();
            //data.SpawnItem(this, inventory);
        }

        public void setQualityFromSkill(HarvestableItemBehaviour data)
        {
            if (skillType == SkillType.None)
                return;

            StatBehaviour player = GameObject.FindGameObjectWithTag("Player").GetComponent<StatBehaviour>();
            SkillBehaviour skillBehaviour = null;

            foreach (SkillBehaviour sB in player.skills)
            {
                if (sB.type == skillType)
                { skillBehaviour = sB; break; }
            }
            if (skillBehaviour == null)
                return;

            Quality quality = Quality.Poor;
            foreach (int val in System.Enum.GetValues(typeof(Quality)))
            {
                if (skillBehaviour.Level >= (((int)tier) * ((int)Quality.Average)) + val)
                    quality = (Quality)val;
            }
            data.quality = quality;
        }

        public void setRandomQuality(HarvestableItemBehaviour data, Quality max)
        {
            data.quality = (Quality)(Random.Range((int)Quality.Poor, (int)max + (int)Quality.Average) / (int)Quality.Average * (int)Quality.Average);
        }
        #endregion
        #endregion
    }
}
