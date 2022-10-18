using GatherGame.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GatherGame.Crafting.Internal
{
    [System.Serializable]
    public class RecipeComponent
    {
        [SerializeField] public ItemScriptable component;
        [SerializeField] public int amount;
    }
}
