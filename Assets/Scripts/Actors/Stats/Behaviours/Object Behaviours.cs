using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GatherGame.Utilities;

namespace GatherGame.Actors.Stats
{
    #region Resource Behaviour
    public class ResourceStatBehaviour : IntStatBehaviour
    {
        public int maxValue { get; protected set; }
        public Image fill { get; protected set; }
        public TMP_Text maxText { get; protected set; }

        public ResourceStatBehaviour(StatType _type, int _maxValue, Transform template)
            : base(_type, _maxValue, template)
        {
            maxValue = _maxValue;
            fill = null;
            maxText = null;
        }

        protected async override Task updateUI(float value)
        {
            text.text = Mathf.CeilToInt(value).ToString();
            await lerpUI(value / maxValue);
            maxText.text = Mathf.CeilToInt(maxValue).ToString();
            return;
        }

        public async Task lerpUI(float amount)
        {
            float t = 0;
            while (fill.fillAmount != amount)
            {
                t += Time.deltaTime;
                fill.fillAmount = Mathf.Lerp(fill.fillAmount, amount, t / 100f);

                if (fill.fillAmount > amount - 0.005f && fill.fillAmount < amount)
                    fill.fillAmount = amount;
                if (fill.fillAmount < amount + 0.005f && fill.fillAmount > amount)
                    fill.fillAmount = amount;

                await Task.Delay((int)(Time.deltaTime * 1000));
            }
            return;
        }

        public override void setUI(int index, Transform parent, object color)
        {
            thisObject = GameObject.Instantiate(thisObject.gameObject, parent).transform;
            LayoutGroup layout = parent.GetComponent<LayoutGroup>();
            thisObject.GetComponent<RectTransform>().sizeDelta = new Vector2(layout.preferredWidth, layout.preferredHeight);
            thisObject.localPosition = Vector2.zero;

            fill = thisObject.Find("Fill").GetComponent<Image>();
            fill.color = (Color)color;
            Transform textTrans = thisObject.Find("Text");
            text = textTrans.Find("Current").GetComponent<TMP_Text>();
            maxText = textTrans.Find("Max").GetComponent<TMP_Text>();

            runUpdateUI(maxValue);
        }
    }
    #endregion

    #region Int Behaviour
    public class IntStatBehaviour
    {
        protected bool updateRunning = false;

        public Queue<float> updateQueue { get; private set; } = new Queue<float>();
        public StatType type { get; protected set; }
        public float value { get; protected set; }
        public TMP_Text text { get; protected set; }
        public Transform thisObject { get; protected set; }

        public virtual async void runUpdateUI(float _value)
        {
            value = _value;
            updateQueue.Enqueue(value);
            if (!updateRunning)
            {
                updateRunning = true;
                while (updateQueue.Count > 0)
                {
                    float valueData = updateQueue.Dequeue();
                    await updateUI(valueData);
                }
                updateRunning = false;
            }
        }

        protected virtual Task updateUI(float value)
        {
            text.text = type.ToString() + ": " + Mathf.CeilToInt(value).ToString();
            return Task.CompletedTask;
        }

        public virtual void setUI(int index, Transform parent, object sprite)
        {
            thisObject = GameObject.Instantiate(thisObject.gameObject, parent).transform;
            LayoutGroup layout = parent.GetComponent<LayoutGroup>();
            thisObject.GetComponent<RectTransform>().sizeDelta = new Vector2(layout.preferredWidth, layout.preferredHeight);
            thisObject.localPosition = Vector2.zero;

            text = thisObject.Find("Text").GetComponent<TMP_Text>();
            thisObject.Find("Sprite").GetComponent<Image>().sprite = (Sprite)sprite;
            runUpdateUI(value);
        }

        public IntStatBehaviour(StatType _type, float _value, Transform template)
        {
            type = _type;
            value = _value;
            text = null;

            thisObject = template;
        }
    }
    #endregion

    #region Skill Behaviour
    public class SkillBehaviour
    {
        private int maxLevel, level;
        public int Level
        {
            get { return level; }
            private set { level = value; }
        }

        private float currentExp, expNeeded, expMulti, expBase, minExp;
        private Image fill = null;
        private TMP_Text text = null;
        private bool updateRunning = false;
        private bool hasLevelUp = false;

        public Queue<System.Tuple<float, float>> updateQueue { get; private set; } = new Queue<System.Tuple<float, float>>();
        public SkillType type { get; private set; }
        public Transform thisObject { get; private set; }

        public async void runUpdateUI()
        {
            updateQueue.Enqueue(System.Tuple.Create(currentExp, expNeeded));
            if (!updateRunning)
            {
                updateRunning = true;
                while (updateQueue.Count > 0)
                {
                    System.Tuple<float, float> expData = updateQueue.Dequeue();

                    float value = expData.Item1 / expData.Item2;
                    if (level > 0)
                        value = (expData.Item1 - minExp) / (expData.Item2 - minExp);

                    await updateUI(value);
                }
                updateRunning = false;
            }
        }

        protected async virtual Task updateUI(float value)
        {
            if (fill.fillAmount == value && !hasLevelUp)
                return;

            if (fill.fillAmount > value || hasLevelUp)
            {
                await lerpUI(1);
                fill.fillAmount = 0;
                text.text = type.ToString() + "- " + level.ToString();
            }

            _ = lerpUI(value);
            hasLevelUp = false;
            return;
        }

        public async Task lerpUI(float amount)
        {
            float t = 0;
            while(fill.fillAmount < amount)
            {
                t += Time.deltaTime;
                fill.fillAmount = Mathf.Lerp(fill.fillAmount, amount, t / 100f);

                if (fill.fillAmount > amount - 0.005f && fill.fillAmount < amount)
                    fill.fillAmount = amount;
                if (fill.fillAmount < amount + 0.005f && fill.fillAmount > amount)
                    fill.fillAmount = amount;

                await Task.Delay((int)(Time.deltaTime * 1000));
            }
            return;
        }

        public virtual void setUI(int index, Transform parent, object extra = null)
        {
            thisObject = GameObject.Instantiate(thisObject.gameObject, parent).transform;
            LayoutGroup layout = parent.GetComponent<LayoutGroup>();
            thisObject.GetComponent<RectTransform>().sizeDelta = new Vector2(layout.preferredWidth, layout.preferredHeight);
            thisObject.localPosition = Vector2.zero;

            text = thisObject.Find("Text").GetComponent<TMP_Text>();
            text.text = type.ToString() + "- " + level.ToString();
            fill = thisObject.Find("Fill").GetComponent<Image>();
            runUpdateUI();
        }

        public SkillBehaviour(Skill skill, Transform transform)
        {
            maxLevel = skill.maxLevel;
            expNeeded = skill.expBase;
            expBase = skill.expBase;
            expMulti = skill.expMulti / 100;

            thisObject = transform;

            currentExp = 0;
            minExp = 0;
            Level = 0;

            List<int> index = new List<int>();
            type = (SkillType)System.Enum.Parse(typeof(SkillType), skill.name);
        }

        public async Task addExp(float amount)
        {
            currentExp += amount;
            while (currentExp >= expNeeded)
                await levelUp();

            return;
        }

        private async Task levelUp()
        {
            if (level + 1 > maxLevel)
            { Debug.Log("Max Level"); return; }

            hasLevelUp = true;
            Level++;
            minExp = expNeeded;
            expNeeded += expBase + expNeeded * expMulti;
            await Task.Delay((int)(Time.deltaTime * 1000));
            return;
        }
    }
    #endregion
}
