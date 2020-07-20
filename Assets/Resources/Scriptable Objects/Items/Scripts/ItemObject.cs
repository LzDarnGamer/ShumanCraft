using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType {
    Foods,
    Tools,
    Armour,
    Materials,
    Weapons,
    Placeables
}

public abstract class ItemObject : ScriptableObject {
    public Sprite icon;

    public ItemType type;
    
    public int itemID;

    public bool isStackable;
    public int maxStackSize;

    public MaterialObject[] recipeItems;

    public int[] recipeAmount;

    public GameObject inGameObject;

    [TextArea(15,20)]
    public string description;
    
}
