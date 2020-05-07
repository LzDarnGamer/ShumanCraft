using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MineObject : MonoBehaviour{
    [SerializeField]
    int HitsNeeded;

    [SerializeField]
    int HitsTaken;

    [SerializeField]
    protected AudioClip SoundOnHit;

    [SerializeField]
    protected AudioClip OnRespawn;

    [SerializeField]
    protected AudioClip OnDestroy;


    protected abstract void OnCollisionEnter(Collision collision);

    protected abstract void dropItems();
    protected float GetRandomNumber(float min, float max) {
        return Random.Range(min, max);
    }

    protected void RespawnCountDown(float seconds, GameObject g) {
        g.GetComponent<AudioSource>().PlayOneShot(OnDestroy);
        StartCoroutine(waitSeconds(seconds, g));
        g.GetComponent<MeshRenderer>().enabled = false;
        g.GetComponent<MeshCollider>().enabled = false; //Outros colliders tambem
        HitsTaken = 0;
    }

    protected IEnumerator waitSeconds(float seconds, GameObject g) {
        yield return new WaitForSeconds(seconds);
        g.GetComponent<MeshRenderer>().enabled = true;
        g.GetComponent<MeshCollider>().enabled = true;
        g.GetComponent<AudioSource>().PlayOneShot(OnRespawn);
    }

    protected bool CheckIfAliveAfterHit(int amount) {
        HitsTaken += amount;
        return HitsTaken >= HitsNeeded;
    }
}
