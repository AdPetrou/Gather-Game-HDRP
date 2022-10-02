using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GatherGame.UI
{
    public class HarvestUI : MonoBehaviour
    {
        public void enable(Vector3 position)
        {
            gameObject.SetActive(true);
            transform.position = position;
            transform.LookAt(Camera.main.transform);
        }
        public void disable()
        {
            gameObject.SetActive(false);
        }

        public IEnumerator fillUI(int harvestTime)
        {
            Image fill = transform.GetChild(0).GetComponent<Image>();
            float t = 0f;
            while (t < harvestTime)
            {
                t += Time.deltaTime;
                //fill.fillAmount += t / (harvestTime / (Time.deltaTime / (harvestTime / 2f)));  No idea why this formula works but it does
                //fill.fillAmount += (2 * Time.deltaTime * t) / Mathf.Pow(harvestTime, 2); // Simplified formula
                fill.fillAmount = t / harvestTime;
                yield return null;
            }

            emptyHarvest();
            yield break;
        }

        public void emptyHarvest()
        {
            transform.GetChild(0).GetComponent<Image>().fillAmount = 0f;
        }
    }
}

