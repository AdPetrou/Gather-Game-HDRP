using UnityEngine;
using UnityEditor;
using System.Collections;

namespace GatherGame.Interaction
{
    public abstract class InteractableObject : ScriptableObject
    {
        [MyBox.ReadOnly][MyBox.Layer]
        [MyBox.Separator][Header("--Object Data--")]
        [SerializeField] private int layer;
        public int Layer { get { return layer; } }
        [SerializeField] protected GameObject prefab;
        [MyBox.Separator][Header("--Experience Data--")]
        [SerializeField] protected SkillType type = SkillType.None;
        [SerializeField] protected float expGain;
        [MyBox.Separator][Header("--Interact Data--")]
        [SerializeField] protected int interactTime;
        [SerializeField] protected int respawnTime;
        [SerializeField] protected float interactDistance;

        public abstract GameObject Interact(GameObject interactable, Actors.ActorBehaviour actor);
        public float GetDistance() { return interactDistance; }
        public int GetRespawnTime() { return respawnTime; }
        public int GetInteractTime() { return interactTime; }
        public string GetTag() { return prefab.tag; }
        public virtual void OnValidate()
        {
            layer = 6;
            if(prefab.layer != layer)
                prefab.layer = layer; // If Actor Raycast can't find Object, check Layer
        }
    }
}
