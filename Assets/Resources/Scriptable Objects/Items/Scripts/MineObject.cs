using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MineObject : MonoBehaviour{
    [SerializeField]
    int HitsNeeded;

    [SerializeField]
    int HitsTaken;

    [SerializeField]
    private AudioClip SoundOnHit;

    [SerializeField]
    private AudioClip OnRespawn;

    protected abstract void OnCollisionEnter(Collision collision);

    protected abstract void dropItems();
    protected float GetRandomNumber(float min, float max) {
        return Random.Range(min, max);
    }

    protected void RespawnCountDown(float seconds, GameObject g) {
        StartCoroutine(waitSeconds(seconds, g));
        g.GetComponent<MeshRenderer>().enabled = false;
        g.GetComponent<MeshCollider>().enabled = false;
        HitsTaken = 0;
        Debug.Log("Respawn");
    }

    protected IEnumerator waitSeconds(float seconds, GameObject g) {
        yield return new WaitForSeconds(seconds);
        g.GetComponent<MeshRenderer>().enabled = true;
        g.GetComponent<MeshCollider>().enabled = true;
    }

    protected bool CheckIfAliveAfterHit(int amount) {
        HitsTaken += amount;
        return HitsTaken >= HitsNeeded;
    }
}
