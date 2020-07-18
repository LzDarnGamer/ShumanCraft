using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineTree : MineObject {

    AudioSource aud;
    private void Start() {
        aud = GetComponent<AudioSource>();
    }


    protected override void dropItems() {
        LootTable l = JSONLoader.lootTables["tree"];
        for (int i = 0; i < l.itemID.Length; i++) {
            for (int j = 0; j < Random.Range(l.minValue[i],l.maxValue[i]); j++) {
                Instantiate(ItemsIndex.getItem(l.itemID[i]).inGameObject, transform.position + Vector3.up, Quaternion.identity);
            }
        }
    }


    public override void RaycastHit(Item it, Vector3 hitPos) {
        aud.PlayOneShot(SoundOnHit, 0.05f);
        Instantiate(particleEffects, hitPos, Quaternion.identity);
        ToolObject tool = (ToolObject)it.item;
        if (CheckIfAliveAfterHit(tool.hitPower)) {
            float rt = GetRandomNumber(minTimeToRespawn, maxTimeToRespawn);
            dropItems();
            RespawnCountDown(rt, gameObject);
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



  
