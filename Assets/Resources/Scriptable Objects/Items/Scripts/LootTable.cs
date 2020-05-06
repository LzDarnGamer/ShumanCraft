﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootTable : MonoBehaviour
{

    public static Dictionary<int,int> IronOreLootTable() {
        return new Dictionary<int, int> {
            { ItemsIndex.getItem(4).itemID, 1 }
        };
    }

    public static Dictionary<int, int> CooperOreLootTable() {
        return new Dictionary<int, int> {
            { ItemsIndex.getItem(5).itemID, 1 }
        };
    }

    public static Dictionary<int, int> TreeLootTable() {
        return new Dictionary<int, int> {
            { ItemsIndex.getItem(1).itemID, Random.Range(1,4) },
            { ItemsIndex.getItem(6).itemID, Random.Range(1,3) }
        };
    }

}
