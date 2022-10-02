using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using GatherGame.Actors.Stats;

namespace GatherGame
{
    //public static class EnumEditorOld
    //{
    //    const string extension = ".cs";

    //    public async static void updateEnum()
    //    {
    //        Stat[] loaded = Resources.LoadAll<Stat>("");
    //        List<IndexedString> skillEnum = new();
    //        List<IndexedString> statEnum = new();

    //        foreach (Stat stat in loaded)
    //        {
    //            await stat.setIndex();
    //            switch (stat)
    //            {                   
    //                case Skill s:
    //                    skillEnum.Add(new IndexedString(s.name, s.Index));
    //                    break;

    //                default:                       
    //                    statEnum.Add(new IndexedString(stat.name, stat.Index));
    //                    break;
    //            }
    //        }

    //        WriteToEnum("StatType", statEnum);
    //        WriteToEnum("SkillType", skillEnum);
    //    }

    //    public static void WriteToEnum(string name, List<IndexedString> enumType)
    //    {
    //        string path = "Assets/Scripts/Enums/";

    //        using (StreamWriter file = File.CreateText(path + name + extension))
    //        {
    //            file.WriteLine("public enum " + name + " \n{");

    //            int i = 0;
    //            foreach (IndexedString stat in enumType)
    //            {
    //                string lineRep = stat.name.ToString().Replace(" ", string.Empty);
    //                if (!string.IsNullOrEmpty(lineRep))
    //                {
    //                    file.WriteLine(string.Format("\t{0} = {1},",
    //                        lineRep, stat.index));
    //                    i++;
    //                }
    //            }

    //            file.WriteLine("\n}");
    //        }

    //        AssetDatabase.ImportAsset(path + name + extension);
    //    }
    //} 
}
