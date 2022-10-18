using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GatherGame.Crafting;
using GatherGame.Inventory;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using GatherGame.Crafting.Internal;
using System.Reflection;
using static Codice.Client.BaseCommands.StatusChangeInfo;

[CustomEditor(typeof(RecipeScriptable))]
public class RecipeEditor : Editor
{
    public VisualTreeAsset xmlTree;

    public override VisualElement CreateInspectorGUI()
    {
        RecipeScriptable instance = target as RecipeScriptable;
        VisualElement container = new VisualElement();
        xmlTree.CloneTree(container);
        //container.StretchToParentSize();

        var componenetField = container.Q("Items");
        var propertyForSize = serializedObject.FindProperty(nameof(RecipeScriptable.components) + ".Array");
        propertyForSize.Next(true); // Expand to obtain array size
        container.TrackPropertyValue(propertyForSize, prop => SetupList(componenetField));
        SetupList(componenetField);

        var resultField = container.Q("Result");
        var inspectField = new PropertyField(serializedObject.FindProperty("result"));
        resultField.Add(inspectField);

        container.Q<Button>("AddComponent").RegisterCallback<ClickEvent>(AddComponent);
        container.Q<Button>("RemoveComponent").RegisterCallback<ClickEvent>(RemoveComponent);

        return container;
    }

    private void SetupList(VisualElement container)
    {
        var property = serializedObject.FindProperty(nameof(RecipeScriptable.components) + ".Array");
        var endProperty = property.GetEndProperty();
        property.NextVisible(true); // Expand the first child.

        var childIndex = 0;

        do
        {
            // Stop if you reach the end of the array
            if (SerializedProperty.EqualContents(property, endProperty))
                break;

            // Skip the array size property
            if (property.propertyType == SerializedPropertyType.ArraySize)
                continue;

            VisualElement newField; PropertyField inspector;
            if (childIndex < container.childCount)
            {
                newField = container[childIndex];
                inspector = (PropertyField)newField[0];
            }
            else
            {
                newField = new VisualElement();
                inspector = new PropertyField(property);
                newField.Add(inspector); container.Add(newField);
            }

            newField.style.position = Position.Absolute;
            int xOffset = Mathf.FloorToInt(250 * (childIndex % 3));
            int yOffset = 300 * (Mathf.FloorToInt(childIndex / 3));

            newField.style.left = xOffset; newField.style.top = yOffset;
            newField.style.width = 250; newField.style.height = 300;

            inspector.BindProperty(property);
            childIndex++;

        } while (property.NextVisible(false)); // Never expand children

        while (childIndex < container.childCount)
        {
            container.RemoveAt(container.childCount - 1);
        }
    }

    void AddComponent(ClickEvent @event)
    {
        var property = serializedObject.FindProperty(nameof(RecipeScriptable.components));
        property.arraySize += 1; serializedObject.ApplyModifiedProperties();
    }

    void RemoveComponent(ClickEvent @event)
    {
        var property = serializedObject.FindProperty(nameof(RecipeScriptable.components));
        property.arraySize -= 1; serializedObject.ApplyModifiedProperties();
    }
}

