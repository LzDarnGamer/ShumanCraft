using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tool Object", menuName = "Inventory System/Items/Tool")]
public class ToolObject : ItemObject {

    public int durability;
    public int hitPower;

    private void Awake() {
        type = ItemType.Tools;
    }
}
