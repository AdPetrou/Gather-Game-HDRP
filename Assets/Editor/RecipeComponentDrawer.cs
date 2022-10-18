using UnityEditor;
using GatherGame.Crafting.Internal;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using GatherGame.Inventory;
using System.ComponentModel;
using UnityEngine;

[CustomPropertyDrawer(typeof(RecipeComponent))]
public class RecipeComponentDrawer : PropertyDrawer
{
    public VisualTreeAsset xmlTree
    {
        get
        {
            return AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
                ("Assets/Editor/UXML/RecipeComponentUI.uxml");
        }
    }

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var obj = fieldInfo.GetValue(property.serializedObject.targetObject) as RecipeComponent;

        var container = new VisualElement();
        xmlTree.CloneTree(container);

        ObjectField field = container.Query<ObjectField>().First();
        field.bindingPath = "component"; field.RegisterCallback<ChangeEvent<Object>, VisualElement>(OnChange, container);
        field.objectType = typeof(ItemScriptable);

        if(obj != null && obj.component != null)
            Update(obj.component, container);

        return container;
    }
    
    private void OnChange(ChangeEvent<Object> evt, VisualElement container)
    {
        if (evt.newValue == null || evt.newValue == evt.previousValue)
            return;

        ItemScriptable item = evt.newValue as ItemScriptable;

        PropertyField sprite = container.Query<PropertyField>();
        sprite.style.backgroundImage = new StyleBackground(item.GetSprite());


        if (item.GetType() != typeof(GenericItemScriptable)
            && !item.GetType().IsSubclassOf(typeof(GenericItemScriptable)))
        {
            var field = container.Query<IntegerField>().First();
            field.SetEnabled(false); field.visible = false;
        }
        else
        {
            var field = container.Query<IntegerField>().First();
            field.SetEnabled(true); field.bindingPath = "amount";
            field.visible = true;
        }
    }

    private void Update(ItemScriptable evt, VisualElement container)
    {
        if (evt == null)
            return;


        PropertyField sprite = container.Query<PropertyField>();
        sprite.style.backgroundImage = new StyleBackground(evt.GetSprite());


        if (evt.GetType() != typeof(GenericItemScriptable)
            && !evt.GetType().IsSubclassOf(typeof(GenericItemScriptable)))
        {
            var field = container.Query<IntegerField>().First();
            field.SetEnabled(false); field.visible = false;
        }
        else
        {
            var field = container.Query<IntegerField>().First();
            field.SetEnabled(true); field.bindingPath = "amount";
            field.visible = true;
        }
    }
}

//[CustomPropertyDrawer(typeof(RecipeComponent))]
//public class RecipeComponentUIE : PropertyDrawer
//{

//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        RecipeComponent obj = fieldInfo.GetValue(property.serializedObject.targetObject) as RecipeComponent;
//        if(obj == null) return;

//        EditorGUI.BeginProperty(position, label, property);
//        var itemField = property.FindPropertyRelative("component");
//        EditorGUI.PropertyField(new Rect(position.x, position.y, position.width / 2, 25), itemField, GUIContent.none);

//        if(obj.component == null) return;

//        Rect imageRect = new Rect(position.x, position.y + 35, 75, 75);
//        GUI.DrawTexture(imageRect, obj.sprite.texture, ScaleMode.ScaleToFit, true);

//        if (obj.type == typeof(GenericItemScriptable) || obj.type.IsSubclassOf(typeof(GenericItemScriptable)))
//        {
//            var amountField = property.FindPropertyRelative("amount");
//            EditorGUI.PropertyField(new Rect( position.x + ( 3 * position.width / 8), imageRect.y + 35, position.width / 8, 25), 
//                amountField, GUIContent.none);
//        }
//        EditorGUI.EndProperty();
//    }
//    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//    {
//        return base.GetPropertyHeight(property, label) + 100;
//    }
//}
