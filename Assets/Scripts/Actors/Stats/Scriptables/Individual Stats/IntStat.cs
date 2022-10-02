using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GatherGame.Actors.Stats
{
    [CreateAssetMenu(fileName = "Int Stat", menuName = "Stats/Types/Int")]
    public class IntStat : Stat
    {
        public Sprite sprite;
    }

    [System.Serializable]
    public class IntTemplate
    {
        public IntStat stat;
        public int value;
    }
}
