using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchivementLog : MonoBehaviour {
    [SerializeField] AudioClip achivementDone;
    [SerializeField] GameObject achivementdonePanel;

    [SerializeField] AchivementList achivementList;
    public int ACHIVEMENT_LEVEL { get; set; }

    private Dictionary<Achivement, int> AchivementProgress;
    public void Start() {
        ACHIVEMENT_LEVEL = 0;
        AchivementProgress = new Dictionary<Achivement, int>();

        for (int i = 0; i < achivementList.AchivementsList.Count; i++) {
            AchivementProgress.Add(achivementList.AchivementsList[i], 0);
        }

    }

    public void advanceAchivement(ItemObject it) {
        foreach (var item in AchivementProgress) {
            if (!item.Key.isDone && item.Key.requirement[0] == it.itemID) {
                AchivementProgress[item.Key] = item.Value + 1;
                if(item.Value >= item.Key.requirement[1]) {
                    AchivementProgress[item.Key] = item.Key.requirement[1];
                    ACHIVEMENT_LEVEL++;
                    item.Key.isDone = true;
                }
            }
        }
    }
}
