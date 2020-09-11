using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Food Object", menuName = "Inventory System/Items/Food")]
public class FoodObject : ItemObject
{
    public float hungerRestore;
    public float thirstRestore;
    public float healthRestore;

    private void Awake() {
        type = ItemType.Foods;
    }
}
