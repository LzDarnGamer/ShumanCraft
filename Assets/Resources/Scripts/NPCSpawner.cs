using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour {

    public GameObject asass;
    public GameObject spawn;
    public Transform[] spawnPoints;
    public int numberOfInstances = 0;

    public GameObject[] instances;

    private void Start() {

        spawnPoints = spawn.transform.GetComponentsInChildren<Transform>();

        instances = new GameObject[spawnPoints.Length];
        for (int i = 0; i < spawnPoints.Length; ++i) {
            GameObject a = Instantiate(asass, spawnPoints[i].position, Quaternion.identity);
            //PhotonNetwork.Instantiate("", spawnPoints[i].position, Quaternion.identity);
            instances[i] = a;
            numberOfInstances++;
        }
        StartCoroutine(UpdateArray());
    }

    IEnumerator UpdateArray() {
        while (true) {
            for (int i = 0; i < instances.Length; ++i) {
                if (instances[i] == null) {
                    GameObject a = Instantiate(asass, spawnPoints[i].position, Quaternion.identity);
                    //PhotonNetwork.Instantiate("", spawnPoints[i].position, Quaternion.identity);
                    instances[i] = a;
                }
            }

            yield return new WaitForSeconds(10);
        }
    }


}
