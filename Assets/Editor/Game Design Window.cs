using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;
using System.ComponentModel;
using System.Linq;

public class GameDesignWindow : EditorWindow
{
    public readonly List<Type> types = new List<Type>()
    {
        typeof(GatherGame.Interaction.InteractableObject),
        typeof(GatherGame.Inventory.ItemScriptable),
        typeof(GatherGame.Inventory.InventoryScriptable),        
        typeof(GatherGame.Crafting.RecipeScriptable),
    };

    float windowSize { get { return position.width; } }

    public List<string> typeNames
    {
        get
        {
            var names = new List<string>();
            foreach (var type in types)
            {
                List<char> name = new List<char>();
                for (int i = type.Name.Length - 1; i >= 0; i--)
                {
                    if (type.Name[i] == '.')
                    {
                        name.Insert(0, '•');
                        break;
                    }

                    if (Char.IsUpper(type.Name[i]))
                    {
                        name.Insert(0, type.Name[i]);
                        name.Insert(0, ' ');
                    }
                    else
                        name.Insert(0, type.Name[i]);
                }
                names.Add(new string(name.ToArray()));
            }

            return names;
        }
    }

    private VisualElement typeListElement;
    private VisualElement objectListElement;
    private VisualElement objectWindowElement;

    public void CreateGUI()
    {
        var splitView = AddSplitWindow(0, windowSize * 0.15f);
        typeListElement = splitView[0];

        var typeList = new ListView();
        typeList.makeItem = () => new Label(); typeList.bindItem = (item, index) 
            => { (item as Label).text = typeNames[index]; };

        typeList.itemsSource = types;
        typeList.onSelectionChange += OnTypeListChange;

        AddHeader(typeListElement, "Type list");
        typeListElement.Add(typeList);

        rootVisualElement.Add(splitView);
    }

    private void OnTypeListChange(IEnumerable<object> selectedItem)
    {
        if (selectedItem == null)
            return;

        Type T = selectedItem.First() as Type;

        if (T == null)
        { Debug.Log(name); Debug.LogError("something went wrong"); }

        var GUIDS = AssetDatabase.FindAssets("t:" + T.Name);
        var allObjects = new List<UnityEngine.Object>();
        foreach (var guid in GUIDS)
            allObjects.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), T));

        var list = new ListView();
        list.makeItem = () => new Label(); list.bindItem = (item, index)
            => { (item as Label).text = "• " + allObjects[index].name; };
        list.itemsSource = allObjects;
        list.onSelectionChange += OnDynamicListChange;

        var splitView = AddSplitWindow(0, windowSize * 0.1f);
        rootVisualElement[0].RemoveAt(1); rootVisualElement[0].Insert(1, splitView);
        objectListElement = splitView[0];

        AddHeader(objectListElement, "Object list");
        objectListElement.Add(list);
    }

    private void AddHeader(VisualElement parent, string header)
    {
        parent.Add(new Label("------------"));
        parent.Add(new Label(header));
        parent.Add(new Label("------------"));
        parent.Add(new Label(""));
    }

    private void OnDynamicListChange(IEnumerable<object> selectedItem)
    {
        var element = new InspectorElement(selectedItem.First() as ScriptableObject);
        rootVisualElement[0][1][1].Clear(); rootVisualElement[0][1][1].Add(element);
        element.StretchToParentSize();
    }

    private VisualElement AddSplitWindow(int index, float size)
    {
        var splitView = new TwoPaneSplitView(index, size,
    TwoPaneSplitViewOrientation.Horizontal);

        var leftPane = new VisualElement();
        var rightPane = new VisualElement();
        splitView.Add(leftPane);
        splitView.Add(rightPane);

        return splitView;
    }

    [MenuItem("Tools/Design Window")]
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor
        EditorWindow wnd = GetWindow<GameDesignWindow>();
        wnd.titleContent = new GUIContent("Design Window");
    }
}
