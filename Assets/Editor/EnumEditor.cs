using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;
using System.Linq;
using System;
using MyBox;

public class EnumEditor : ScriptableWizard
{
    [SerializeField][ReadOnly]
    private string path = "Assets/Scripts/Enums/";
    private static string pathRef;

    [Separator]

    public ScriptableObject objectOfType = null;
    private ScriptableObject previousObject;

    [Dropdown(nameof(namespaceTypes))]
    public string inputClass = null;
    private string previousInputClass;

    [Dropdown(nameof(derivativeClasses))][OverrideLabel("Class Blacklist")]
    public string[] classBlacklist;

    [Separator]

    public bool editName;
    [ReadOnly(nameof(editName), true)]
    public string enumName;

    private Assembly assembly;
    private Type type;
    public const string extension = ".cs";
    public static List<string> namespaceTypes { get; private set; } = new List<string>();
    private static List<string> derivativeClasses = new List<string>();

    private void Awake()
    {
        previousObject = objectOfType;
        previousInputClass = inputClass;
        pathRef = path;
    }

    public void Update()
    {
        if (objectOfType != previousObject && objectOfType != null)
        {
            namespaceTypes.Clear();
            Type currentClass = objectOfType.GetType();
            assembly = currentClass.Assembly;
            while (currentClass != null)
            {
                if (currentClass.Namespace.Contains("GatherGame"))
                    namespaceTypes.Add(currentClass.Namespace + "." + currentClass.Name);

                currentClass = currentClass.BaseType;
            }

            inputClass = namespaceTypes[0];
            previousObject = objectOfType;
        }

        if (inputClass != previousInputClass && inputClass != null && objectOfType != null)
        {
            derivativeClasses = getChildClassList();
            previousInputClass = inputClass;
            enumName = removeNamespace(inputClass) + "Type";
        }
    }

    private string removeNamespace(string input)
    {
        List<char> result = new List<char>();
        foreach (char c in input)
        {
            result.Add(c);
            if (c == '.')
                result.Clear();
        }
        return new string(result.ToArray());
    }
    private string objectNameFromPath(string input)
    {
        List<char> result = new List<char>();
        foreach (char c in input)
        {
            if (c == '.')
                break;

            result.Add(c);

            if (c == '/')
                result.Clear();
        }
        return new string(result.ToArray()).Replace(" ", "");
    }

    private List<string> getChildClassList()
    {
        List<Type> typeList = assembly.GetTypes().Where(t => t.IsSubclassOf(assembly.GetType(inputClass))).ToList();

        List<string> returnValue = new List<string>();
        foreach (Type t in typeList)
            returnValue.Add(t.Namespace + "." + t.Name);

        return returnValue;
    }

    private List<IndexedString> getObjectsFromInput()
    {
        List<IndexedString> result = new List<IndexedString>();

        string[] assetGUIDS = AssetDatabase.FindAssets("t:" + removeNamespace(inputClass));

        List<string[]> blacklistRemoval = new List<string[]>();
        foreach(string t in classBlacklist)
            blacklistRemoval.Add(AssetDatabase.FindAssets("t:" + removeNamespace(t)));

        foreach (string[] guidList in blacklistRemoval)
            for (int i = 0; i < assetGUIDS.Length; i++)
                for (int u = 0; u < guidList.Length; u++)
                {
                    if(assetGUIDS[i] == guidList[u])
                    {
                        assetGUIDS = assetGUIDS.RemoveAt(i);
                        i = 0; u = 0;
                    }
                }

        for (int i = 0; i < assetGUIDS.Length; i++)
            result.Add(new IndexedString(objectNameFromPath(AssetDatabase.GUIDToAssetPath(assetGUIDS[i])), i));

        return result;
    }

    public static Type GetEnumType(string enumName)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var type = assembly.GetType(enumName);
            if (type == null)
                continue;
            if (type.IsEnum)
                return type;
        }
        return null;
    }

    private List<IndexedString> filterForEnumUpdate(List<IndexedString> input)
    {
        Type enumType = GetEnumType(enumName);
        if (enumType == null)
            return input;

        Array enumValues = enumType.GetEnumValues();
        List<IndexedString> filteredResult = new List<IndexedString>();

        for (int i = 0; i < enumValues.Length; i++)
        {
            filteredResult.Add(new IndexedString(enumValues.GetValue(i).ToString(), (int)enumValues.GetValue(i)));
            for (int u = 0; u < input.Count; u++) 
            {
                if (enumValues.GetValue(i).ToString() == input[u].name)
                {
                    input.RemoveAt(u);
                    break;
                }
            } 
        }

        if (input.Count > 0)
        {
            for (int i = 0; i < input.Count; i++)
                filteredResult.Add(new IndexedString(input[i].name, findOpenValue(filteredResult)));
        }

        return filteredResult;
    }

    private int findOpenValue(List<IndexedString> input)
    {
        int highestNumber = 0;
        foreach(IndexedString obj in input)
            if (highestNumber < obj.index)
                highestNumber = obj.index;

        bool reset = false;
        for (int i = 0; i < highestNumber; i++)
        {
            reset = false;
            foreach (IndexedString obj in input)
                if (i == obj.index)
                {
                    reset = true;
                    break;
                }
            if (!reset)
                return i;
        }

        return highestNumber + 1;
    }

    [MenuItem("Tools/Enum Generator")]
    public static void EnumEditorWizard()
    {
        DisplayWizard<EnumEditor>("Generate Enum", "Generate");
    }

    public void OnWizardCreate()
    {
        if (inputClass != null && objectOfType != null)
        {
            WriteToEnum(enumName, filterForEnumUpdate(getObjectsFromInput()));
        }
        else
            Debug.Log("Set the Object and Class to generate");
    }

    public static void WriteToEnum(string name, List<IndexedString> enumType)
    {
        using (StreamWriter file = File.CreateText(pathRef + name + extension))
        {
            file.WriteLine("public enum " + name + " \n{");

            int i = 0;
            foreach (IndexedString index in enumType)
            {
                string lineRep = index.name.Replace(" ", string.Empty);
                if (!string.IsNullOrEmpty(lineRep))
                {
                    file.WriteLine(string.Format("\t{0} = {1},",
                        lineRep, index.index));
                    i++;
                }
            }

            file.WriteLine("\n}");
        }

        AssetDatabase.ImportAsset(pathRef + name + extension);
    }
}

public struct IndexedString
{
    public string name;
    public int index;

    public IndexedString(string _name, int _index)
    {
        name = _name;
        index = _index;
    }
}

//public enum SkillType
//{
//    Gathering = 7,
//    Looting = 8,
//    Mining = 9,
//    None = 10,

//}
