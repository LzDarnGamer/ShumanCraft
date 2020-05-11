using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchivementList : MonoBehaviour
{
    [SerializeField]
    Sprite ach0;
    [SerializeField]
    Sprite ach1;
    [SerializeField]
    Sprite ach2;
    [SerializeField]
    Sprite ach3;
    [SerializeField]
    Sprite ach4;
    [SerializeField]
    Sprite ach5;
    [SerializeField]
    Sprite ach6;
    [SerializeField]
    Sprite ach7;
    [SerializeField]
    Sprite ach8;
    [SerializeField]
    Sprite ach9;
    [SerializeField]
    Sprite ach10;


    public static List<Achivement> AchivementsList = new List<Achivement>();
    private void Start() {
        AchivementsList.Add(
            new Achivement(
                "Collect Wood",
                "Break trees to get wood",
                new Dictionary<int, int>(){ { ItemsIndex.getItem(1).itemID, 20 } },
                ach0
                ));
        AchivementsList.Add(
      new Achivement(
          "Collect Wood",
          "Break trees to get wood",
          new Dictionary<int, int>() { { ItemsIndex.getItem(1).itemID, 20 } },
          ach0
          ));
        AchivementsList.Add(
      new Achivement(
          "Collect Wood",
          "Break trees to get wood",
          new Dictionary<int, int>() { { ItemsIndex.getItem(1).itemID, 20 } },
          ach0
          ));
        AchivementsList.Add(
      new Achivement(
          "Collect Wood",
          "Break trees to get wood",
          new Dictionary<int, int>() { { ItemsIndex.getItem(1).itemID, 20 } },
          ach0
          ));
    }
}
