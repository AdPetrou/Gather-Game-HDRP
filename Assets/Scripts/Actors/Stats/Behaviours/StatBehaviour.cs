using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// ---------------------------------------------------------------------------------------------------------------------------
/// Reflection code I used to iterate and sort the variables in this class, no longer need but good reference
/// ---------------------------------------------------------------------------------------------------------------------------
/// Transform resourceStats = statsSheet.Find("Resource Stats");
///foreach (var attribute in GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
///{
///    object o = attribute.GetValue(this);
///    switch (o)
///    {
///        case ResourceStat r:
///            updateResourceStat(r, resourceStats);
///            break;
///    }
///}
///---------------------------------------------------------------------------------------------------------------------------
/// </summary>


namespace GatherGame.Actors.Stats
{
    public class StatBehaviour : MonoBehaviour
    {
        [SerializeField]
        private StatSheet statsScriptable;
        private Transform statSheet = null;

        public ResourceStatBehaviour[] resourceStats { get; private set; }
        public IntStatBehaviour[] intStats { get; private set; }
        public SkillBehaviour[] skills { get; private set; }

        public void Start()
        {
            setObject();
        }
        public void Update()
        {
            // Add a lot of exp (temporary)
            if (Input.GetKeyDown(KeyCode.RightShift))
                foreach (SkillBehaviour s in skills)
                    addExp(s.type, 2000000000000000000);
        }

        public StatSheet returnStatScriptable() { return statsScriptable; }
        public void setObject()
        {
            StatGameobjects gameobjects = Resources.LoadAll<StatGameobjects>("")[0];
            statSheet = gameobjects.statSheet.transform;
            // Default Values are full
            List<ResourceStatBehaviour> tempResources = new();
            foreach (ResourceTemplate r in statsScriptable.resourceStats)
            {
                tempResources.Add(new ResourceStatBehaviour(
                    (StatType)System.Enum.Parse(typeof(StatType), r.stat.name.Replace(" ", string.Empty)),
                    r.maxValue, gameobjects.resourceObject.transform));
                r.stat.addFunctionToGameobject(gameObject);
            }

            resourceStats = tempResources.ToArray();

            List<IntStatBehaviour> tempInt = new();
            foreach (IntTemplate r in statsScriptable.intStats)
            {
                tempInt.Add(new IntStatBehaviour(
                    (StatType)System.Enum.Parse(typeof(StatType), r.stat.name.Replace(" ", string.Empty)),
                    r.value, gameobjects.intObject.transform));
            }

            intStats = tempInt.ToArray();

            List<SkillBehaviour> tempSkills = new();
            foreach (Skill s in statsScriptable.harvestSkills)
                tempSkills.Add(new SkillBehaviour(s, gameobjects.skillObject.transform));

            skills = tempSkills.ToArray();

            createStats();
            activeCall();
        }

        public async void addExp(SkillType type, float amount)
        {
            foreach (SkillBehaviour skill in skills)
                if (skill.type.Equals(type))
                {
                    await skill.addExp(amount);
                    skill.runUpdateUI();
                    return;
                }
        }

        public bool activeCall()
        {
            GameObject gO = statSheet.parent.gameObject;
            gO.SetActive(!gO.activeSelf);
            if (gO.activeSelf)
                updateStatSheet();

            return gO.activeSelf;
        }

        public void createStats()
        {
            statSheet = Instantiate(statSheet.gameObject, GameObject.Find("Profiles").transform, false).transform.GetChild(0);
            statSheet.parent.position = new Vector2(Screen.width - 400, Screen.height / 1.25f);

            if (skills.Length <= 0)
            {
                Destroy(statSheet.transform.Find("Skills").gameObject);
                RectTransform rect = statSheet.transform.parent.GetComponent<RectTransform>();
                rect.offsetMin = new Vector2(rect.offsetMin.x, 133);
            }

            Transform parent = statSheet.Find("Resource Stats");
            for(int i = 0; i < resourceStats.Length; i++)
                resourceStats[i].setUI(i, parent, ((ResourceStat)returnScritable(resourceStats[i])).color);

            parent = statSheet.Find("Misc Stats");
            for (int i = 0; i < intStats.Length; i++)
                intStats[i].setUI(i, parent, ((IntStat)returnScritable(intStats[i])).sprite);

            parent = statSheet.Find("Skills");
            for (int i = 0; i < skills.Length; i++)
                skills[i].setUI(i, parent);
        }

        public object returnScritable(object obj)
        {
            switch (obj)
            {
                case ResourceStatBehaviour resource:
                    foreach (ResourceTemplate r in statsScriptable.resourceStats)
                    {
                        //if (r.stat.name.Replace(" ", string.Empty) == resource.type.ToString())
                        //return r.stat;
                        if (r.stat.Index == (int)resource.type)
                            return r.stat;
                    }
                    break;

                case IntStatBehaviour stat:
                    foreach (IntTemplate i in statsScriptable.intStats)
                    {
                        //if (i.stat.name.Replace(" ", string.Empty) == stat.type.ToString())
                            //return i.stat;
                        if (i.stat.Index == (int)stat.type)
                            return i.stat;
                    }
                    break;

                case SkillBehaviour skill:
                    foreach (Skill s in statsScriptable.harvestSkills)
                    {
                        //if (s.name.Replace(" ", string.Empty) == skill.type.ToString())
                        //return s;
                        if (s.Index == (int)skill.type)
                            return s;
                    }
                    break;
            }
            return null;
        } 

        public void updateStatSheet()
        {
            updateAllResourceStatUI();
            updateAllIntStatUI();
            updateAllSkillUI();
        }

        public void updateAllResourceStatUI()
        {
            foreach (ResourceStatBehaviour i in resourceStats)
                i.runUpdateUI(i.value);
        }
        public void updateAllIntStatUI()
        {
            foreach (IntStatBehaviour i in intStats)
                i.runUpdateUI(i.value);
        }
        public void updateAllSkillUI()
        {
            foreach (SkillBehaviour i in skills)
                i.runUpdateUI();
        }
    }   
}
