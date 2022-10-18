using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GatherGame.Inventory;
using GatherGame.Crafting.Internal;

namespace GatherGame.Crafting 
{
    [CreateAssetMenu(fileName = "Recipe", menuName = "Recipes")]
    public class RecipeScriptable : ScriptableObject
    {
        [SerializeField] public List<RecipeComponent> components = new List<RecipeComponent>();
        [SerializeField] public RecipeComponent result = new RecipeComponent();

    }
}
