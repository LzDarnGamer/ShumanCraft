using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundScipt : MonoBehaviour
{
    [Header("Footstep Grass")]
    [SerializeField] AudioClip[] grass;

    [Header("Footstep Dirt")]
    [SerializeField] AudioClip[] dirt;

    [Header("Footstep Sand")]
    [SerializeField] AudioClip[] sand;

    [Header("Tired")] [SerializeField] AudioClip tired;
    private bool tiredBool;

    [Header("Audio Player")]
    [SerializeField] AudioSource audioPlayer;

    public void playFootStep() {
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 2)) {
            string floortag = hit.collider.tag;
            if (floortag == "Dirt") {
                int r = Random.Range(0, dirt.Length-1);
                audioPlayer.PlayOneShot(dirt[r], 0.1f);
            } else if (floortag == "Sand") {
                int r = Random.Range(0, sand.Length - 1);
                audioPlayer.PlayOneShot(sand[r], 0.1f);
            } else if (floortag == "Grass") {
                int r = Random.Range(0, grass.Length - 1);
                audioPlayer.PlayOneShot(grass[r], 0.1f);
            }
        }
    }

    public IEnumerator PlayTired() {
        if (!tiredBool) {
            tiredBool = true;
            audioPlayer.PlayOneShot(tired);
            yield return new WaitForSeconds(2f);
            tiredBool = false;
        } else {
            yield return null;
        }
    }

    

}
