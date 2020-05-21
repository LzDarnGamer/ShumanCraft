using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class AchivementLog : MonoBehaviour {
    [SerializeField] AudioClip achivementDone;
    [SerializeField] GameObject achivementdonePanel;


    public int ACHIVEMENT_LEVEL { get; set; }

    private AchivementList achivementList;
    private OrderedDictionary AchivementProgress;

    public Achivement[] keys { get;  private set; }
    public int[] values  { get;  private set; }
public void Start() {
        achivementList = GameObject.Find("GameSetup").GetComponent<AchivementList>();
        ACHIVEMENT_LEVEL = 0;
        AchivementProgress = new OrderedDictionary();

        for (int i = 0; i < achivementList.AchivementsList.Count; i++) {
            AchivementProgress.Add(achivementList.AchivementsList[i], 0);
        }

        ICollection k = AchivementProgress.Keys;
        ICollection v = AchivementProgress.Values;

        keys = new Achivement[AchivementProgress.Count];
        values = new int[AchivementProgress.Count];
        k.CopyTo(keys, 0);
        v.CopyTo(values, 0);
    }

    public void advanceAchivement(ItemObject it) {
        for (int i = 0; i < AchivementProgress.Count; i++) {
            if (!keys[i].isDone && keys[i].requirement[0] == it.itemID) {
                values[i] += 1;
                Debug.Log(it.name + " " + values[i]);
                if(values[i] >= keys[i].requirement[1]) {
                    values[i] = keys[i].requirement[1];
                    ACHIVEMENT_LEVEL++;
                    keys[i].isDone = true;
                }
            }
        }
    }
}
