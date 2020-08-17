using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NPCSpawner : MonoBehaviour {

    //public GameObject NPCToSpawn;
    public string area;
    [SerializeField] private string[] animal;

    public GameObject spawn;
    private Transform[] spawnPoints;
    private int numberOfInstances = 0;

    public GameObject[] instances;

    private void Start() {
        switch (area) {
            case "Forest":
                animal = new string[] {"Sheep", "Wolf Black", "Cat"};
                break;
            case "Ice":
                animal = new string[] { "Wolf White", "Cat" };
                break;
            case "Desert":
                animal = new string[] { "Deer" };
                break;
            case "Savana":
                animal = new string[] { "Zebra", "Deer" };
                break;
            case "Swamp":
                animal = new string[] { "Wolf White", "Wolf Black", "Horse" };
                break;
            default:
                Debug.Log("ERROR: Area name not found!");
                break;
        }
        spawnPoints = spawn.transform.GetComponentsInChildren<Transform>();

        instances = new GameObject[spawnPoints.Length];
        for (int i = 0; i < spawnPoints.Length; ++i) {
            //GameObject a = Instantiate(NPCToSpawn, spawnPoints[i].position, Quaternion.identity);
            int aux = Random.Range(0, animal.Length);
            GameObject a = PhotonNetwork.Instantiate(Path.Combine("Animals", animal[aux]),
                                            spawnPoints[i].position,
                                            Quaternion.identity, 0);
            instances[i] = a;
            numberOfInstances++;
        }
        StartCoroutine(UpdateArray());
    }

    IEnumerator UpdateArray() {
        while (true) {
            for (int i = 0; i < instances.Length; ++i) {
                if (instances[i] == null) {
                    //GameObject a = Instantiate(NPCToSpawn, spawnPoints[i].position, Quaternion.identity);
                    int aux = Random.Range(0, animal.Length);
                    Debug.Log("RANDOM AUX: " + aux);
                    GameObject a = PhotonNetwork.Instantiate(Path.Combine("Animals", animal[aux]),
                                            spawnPoints[i].position,
                                            Quaternion.identity, 0);
                    instances[i] = a;
                }
            }

            yield return new WaitForSeconds(10);
        }
    }


}
