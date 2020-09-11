using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Armour Object", menuName = "Inventory System/Items/Armour")]
public class ArmourObject : ItemObject
{
    public int armourPoints;

    private void Awake() {
        type = ItemType.Armour;
    }
}
