using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchivementList : MonoBehaviour
{

    public List<Achivement> AchivementsList = new List<Achivement>();
    private void Start() {
        AchivementsList.Add(
            new Achivement(
                "Collect Wood",
                "Break trees to get wood",
                new Dictionary<int, int>(){ { ItemsIndex.getItem(1).itemID, 20 } },
                ItemsIndex.getItem(1).icon
                ));
        AchivementsList.Add(
            new Achivement(
                "Collect Leafs",
                "Break trees to get Leafs",
                new Dictionary<int, int>(){ { ItemsIndex.getItem(6).itemID, 15 } },
                ItemsIndex.getItem(6).icon
                ));
        AchivementsList.Add(
          new Achivement(
              "Craft rope",
              "Rope is going to be in a lot of tools and weapons craftings, use your leafs to craft",
              new Dictionary<int, int>() { { ItemsIndex.getItem(7).itemID, 10 } },
              ItemsIndex.getItem(7).icon
              ));
    }
}
