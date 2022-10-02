using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GatherGame.Actors.Stats 
{
    [CreateAssetMenu(fileName = "Object manager", menuName = "Stats/Utilities/Object manager")]
    public class StatGameobjects : ScriptableObject
    {
        [Header("--Display Sheet--")]
        public GameObject statSheet;
        [Header("--Stats--")]
        public GameObject resourceObject;
        public GameObject intObject;
        public GameObject skillObject;
    } 
}
