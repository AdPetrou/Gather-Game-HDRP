using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GatherGame.Actors.Stats
{
    [CreateAssetMenu(fileName = "Stat Sheet", menuName = "Stats/Stat Sheet")]
    public class StatSheet : ScriptableObject
    {
        [Header("--Resource Stats--")]
        public ResourceTemplate[] resourceStats;
        [Header("--Misc. Stats--")]
        public IntTemplate[] intStats;

        public Skill[] harvestSkills;

        public List<string> returnStatNames()
        {
            List<string> returnList = new List<string>();

            foreach (ResourceTemplate r in resourceStats)
                returnList.Add(r.stat.name);

            foreach (IntTemplate i in intStats)
                returnList.Add(i.stat.name);

            return returnList;
        }

    }
}
