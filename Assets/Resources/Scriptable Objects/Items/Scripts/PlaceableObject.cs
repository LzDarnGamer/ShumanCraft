using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Placeable Object", menuName = "Inventory System/Items/Placeables")]
public class PlaceableObject : ItemObject {

    private void Awake() {
        type = ItemType.Placeables;
    }

}
