using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using RuntimeScriptField;

namespace GatherGame.Actors.Stats
{
    public class Stat : ScriptableObject
    {
        [SerializeField][MyBox.ReadOnly]
        private int index;
        [SerializeField]
        private ComponentReference function;

        public int Index
        {
            get
            {
                return index;
            }
            private set
            {
                index = value;
            }
        }

        public void addFunctionToGameobject(GameObject gameObject)
        {
            if (function == null)
                return;

            function.AddTo(gameObject);
        }

        public async Task setIndex()
        {
            try 
            {
                if (index > 0) 
                {
                    return;
                }
                else
                {
                    Stat[] stats = Resources.LoadAll<Stat>("");
                    int highest = 0;
                    foreach (Stat s in stats)
                        if (s.index > highest)
                            highest = s.index;
                    index = highest + 1;

                    EditorUtility.SetDirty(this);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    await Task.Delay(1);
                    return;
                }
            }
            catch
            {
                Stat[] stats = Resources.LoadAll<Stat>("");
                int highest = 0;
                foreach (Stat s in stats)
                    if (s.index > highest)
                        highest = s.index;
                index = highest + 1;

                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                await Task.Delay(1);
                return;
            }
        }
    }
}
