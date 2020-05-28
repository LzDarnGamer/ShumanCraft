using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MineObject : MonoBehaviour{
    [SerializeField] protected GameObject particleEffects;
    [SerializeField] protected int HitsNeeded;
    [SerializeField] protected int HitsTaken;
    [SerializeField] protected float minTimeToRespawn;
    [SerializeField] protected float maxTimeToRespawn;
    [SerializeField] protected AudioClip SoundOnHit;
    [SerializeField] protected AudioClip SoundOnRespawn;
    [SerializeField] protected AudioClip SoundOnDestroy;


    protected abstract void OnTriggerEnter(Collider collision);

    protected abstract void dropItems();
    protected float GetRandomNumber(float min, float max) {
        return Random.Range(min, max);
    }

    protected abstract void RespawnCountDown(float seconds, GameObject g);

    protected abstract IEnumerator waitSeconds(float seconds, GameObject g);

    protected bool CheckIfAliveAfterHit(int amount) {
        HitsTaken += amount;
        return HitsTaken >= HitsNeeded;
    }
}
