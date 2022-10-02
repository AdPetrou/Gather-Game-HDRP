using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GatherGame.Actors.Stats
{
    [CreateAssetMenu(fileName = "Resource", menuName = "Stats/Types/Resource")]
    public class ResourceStat : Stat
    {
        public Color color;
    }

    [System.Serializable]
    public class ResourceTemplate
    {
        public ResourceStat stat;
        public int maxValue;
    }
}
