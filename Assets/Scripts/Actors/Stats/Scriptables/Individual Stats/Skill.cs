using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GatherGame.Actors.Stats
{
    [CreateAssetMenu(fileName = "Skill", menuName = "Stats/Types/Skill")]
    public class Skill : Stat
    {
        public int maxLevel;
        [Tooltip("The starting exp amount")]
        public float expBase;
        [Tooltip("The % that the exp will increase by on level up")]
        public float expMulti;        
    }
}
