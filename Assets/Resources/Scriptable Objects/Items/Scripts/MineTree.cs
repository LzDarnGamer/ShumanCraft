using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineTree : MineObject {

    AudioSource aud;

    private void Start() {
        aud = GetComponent<AudioSource>();
    }


    protected override void dropItems() {
        foreach (KeyValuePair<int, int> entry in LootTable.TreeLootTable()) {
            for (int i = 0; i < entry.Value; i++) {
                Instantiate(ItemsIndex.getItem(entry.Key).inGameObject, 
                    transform.position + Vector3.up, Quaternion.identity);
            }
        }
    }

    private void OnCollisionEnter(Collision collision) {
        Debug.Log(collision.transform.name);
    }

    protected override void OnTriggerEnter(Collider col) {
        Item to = col.gameObject.GetComponent<Item>();
        if (to != null) {
            float rt = GetRandomNumber(minTimeToRespawn, maxTimeToRespawn);
            aud.PlayOneShot(SoundOnHit, 0.05f);
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

    protected override void RespawnCountDown(float seconds, GameObject g) {
        aud.PlayOneShot(SoundOnDestroy, 0.05f);
        StartCoroutine(waitSeconds(seconds, g));
        g.GetComponent<MeshRenderer>().enabled = false;
        g.GetComponent<MeshCollider>().enabled = false; //Outros colliders tambem
        HitsTaken = 0;
    }

    protected override IEnumerator waitSeconds(float seconds, GameObject g) {
        yield return new WaitForSeconds(seconds);
        g.GetComponent<MeshRenderer>().enabled = true;
        g.GetComponent<MeshCollider>().enabled = true;
        aud.PlayOneShot(SoundOnRespawn, 0.05f);
    }
}

  
