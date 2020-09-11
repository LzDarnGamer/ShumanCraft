using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBConnector {
    public GameObject InGameSlot;
    public InventorySlot ArraySlot;

    public FBConnector(GameObject InGameSlot, InventorySlot ArraySlot) {
        this.InGameSlot = InGameSlot;
        this.ArraySlot = ArraySlot;
    }
}
