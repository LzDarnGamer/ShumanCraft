using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class MineFence : MineObject {

    AudioSource aud;
    private void Start() {
        ToolNeeded = new int[] { 707 };
        aud = GetComponent<AudioSource>();
    }


    protected override void dropItems() {

    }


    public override void RaycastHit(Item it, Vector3 hitPos) {

        if (!Array.Exists(ToolNeeded, element => element == it.item.itemID)) return;
        aud.PlayOneShot(SoundOnHit[UnityEngine.Random.Range(0, SoundOnHit.Length - 1)], 0.05f);
        PhotonNetwork.Instantiate("Prefabs\\" + particleEffects.name, hitPos, Quaternion.identity);
        ToolObject tool = (ToolObject)it.item;

        if (CheckIfAliveAfterHit(tool.hitPower)) {
            //float rt = GetRandomNumber(minTimeToRespawn, maxTimeToRespawn);
            //dropItems();
            //RespawnCountDown(rt, gameObject);
            deleteObj(gameObject.GetComponent<PhotonView>().ViewID);
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
    private void RPC_delete(int id) {
        GameObject a = PhotonView.Find(id).gameObject;
        Destroy(a);
    }
    public void deleteObj(int id) {
        gameObject.GetComponent<PhotonView>().RPC("RPC_delete", RpcTarget.AllBuffered, id);
    }
}




