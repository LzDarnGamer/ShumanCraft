using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class MineTree : MineObject {

    AudioSource aud;
    private void Start() {
        ToolNeeded = new int[] { 701,702,703,704 };
        aud = GetComponent<AudioSource>();
    }


    protected override void dropItems() {
        LootTable l = JSONLoader.lootTables["tree"];
        for (int i = 0; i < l.itemID.Length; i++) {
            for (int j = 0; j < UnityEngine.Random.Range(l.minValue[i],l.maxValue[i]); j++) {
                PhotonNetwork.Instantiate(l.pathName[i], transform.position + Vector3.up, Quaternion.identity);
            }
        }
    }


    public override void RaycastHit(Item it, Vector3 hitPos) {

        if (!Array.Exists(ToolNeeded, element => element == it.item.itemID)) return;

        aud.PlayOneShot(SoundOnHit[UnityEngine.Random.Range(0, SoundOnHit.Length - 1)], 0.05f);
        PhotonNetwork.Instantiate("Prefabs\\" + particleEffects.name, hitPos, Quaternion.identity);
        ToolObject tool = (ToolObject)it.item;
        if (CheckIfAliveAfterHit(tool.hitPower)) {
            dropItems();
            disableMesh(gameObject.GetComponent<PhotonView>().ViewID);
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


    [PunRPC]
    private void RPC_disableMesh(int id) {
        GameObject a = PhotonView.Find(id).gameObject;
        float rt = GetRandomNumber(minTimeToRespawn, maxTimeToRespawn);
        RespawnCountDown(rt, a);
    }

    public void disableMesh(int id) {
        gameObject.GetComponent<PhotonView>().RPC("RPC_disableMesh", RpcTarget.AllBuffered, id);
    }
}



  
