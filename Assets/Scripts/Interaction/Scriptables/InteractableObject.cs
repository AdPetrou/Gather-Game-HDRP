using UnityEngine;
using UnityEditor;
using System.Collections;

namespace GatherGame.Interaction
{
    public abstract class InteractableObject : ScriptableObject
    {
        [MyBox.ReadOnly][MyBox.Layer]
        public int Layer;
        public GameObject prefab;
        [Header("--Experience Data--")]
        public SkillType type = SkillType.None;
        public float expGain;
        [Header("--Harvest Data--")]
        public int harvestTime;
        public int respawnTime;
        public float harvestDistance;

        public abstract GameObject interact(GameObject gameObject);
        public virtual void OnValidate()
        {
            Layer = 6;
            if(prefab.layer != Layer)
                prefab.layer = Layer; // If Actor can't find Object check Layer
        }
    }
}
