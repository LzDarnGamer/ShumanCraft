using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHit : MonoBehaviour {

    public PhotonView PV;
    public GameObject particle;
    public Transform whereTo;

    void Awake() {
        PV = GetComponent<PhotonView>();
    }

    public GameObject ActivateParticles() {
        return PhotonView.Instantiate(particle, whereTo.position, Quaternion.identity);
    }

}
