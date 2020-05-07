using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineTree : MineObject {

    [SerializeField]
    float minTimeToRespawn;

    [SerializeField]
    float maxTimeToRespawn;

    
    AudioSource aud;


    private void Start() {
        aud = GetComponent<AudioSource>();
    }

    protected override void dropItems() {
        int[] randomLocation = new int[LootTable.TreeLootTable().Count];
        for (int i = 0; i < randomLocation.Length; i++) {
            randomLocation[i] = Random.Range(0, transform.childCount-1);
            for (int j = 0; j < i; j++) {
                if (randomLocation[i] == randomLocation[j]) {
                    i--;
                    break;
                }
            }
        }
        int pos = 0;
        foreach (KeyValuePair<int, int> entry in LootTable.TreeLootTable()) {
            for (int i = 0; i < entry.Value; i++) {
                Instantiate(ItemsIndex.getItem(entry.Key).inGameObject, 
                    transform.GetChild(randomLocation[pos]).transform.position,
                    Quaternion.identity);
            }
            pos++;
        }
    }

    protected override void OnCollisionEnter(Collision col) {
        Item to = col.gameObject.GetComponent<Item>();
        if (to != null) {
            float rt = GetRandomNumber(minTimeToRespawn, maxTimeToRespawn);
            switch (to.item.itemID) {
                case 701:
                    if (CheckIfAliveAfterHit(1)) {
                        dropItems();
                        RespawnCountDown(rt, gameObject);
                }
                    break;
                case 702:
                    if (CheckIfAliveAfterHit(2)) {
                        dropItems();
                        RespawnCountDown(rt, gameObject);
                }
                    break;
                case 703:
                    if (CheckIfAliveAfterHit(3)) {
                        dropItems();
                        RespawnCountDown(rt, gameObject);
                    }
                    break;
                case 704:
                    if (CheckIfAliveAfterHit(4)) {
                        dropItems();
                        RespawnCountDown(rt, gameObject);
                    }
                break;

            }
        }
    }


}

  
