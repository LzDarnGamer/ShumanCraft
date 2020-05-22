using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchivementList : MonoBehaviour
{
    public List<Achivement> AchivementsList { get; private set; }
    void Awake() {
        AchivementsList = new List<Achivement> {
            new Achivement(
                "Collect Wood",
                "Break trees to get wood",
                new int[] { ItemsIndex.getItem(1).itemID, 1 },
                ItemsIndex.getItem(1).icon
                ),
            new Achivement(
                "Collect Leafs",
                "Break trees to get Leafs",
                new int[] { ItemsIndex.getItem(6).itemID, 15 },
                ItemsIndex.getItem(6).icon
                ),
            new Achivement(
              "Craft rope",
              "Rope is going to be in a lot of tools and weapons craftings, use your leafs to craft",
              new int[] { ItemsIndex.getItem(7).itemID, 10 },
              ItemsIndex.getItem(7).icon
              )
        };
    }
}
