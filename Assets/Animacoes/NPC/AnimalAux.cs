using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalAux : MonoBehaviour {

    [SerializeField] PhotonView PV;

    private void Awake() {
        PV = GetComponent<PhotonView>();
    }

    public void RunRPC (int playerID) {
        PV.RPC("RPC_TakePlayerDamage", RpcTarget.AllBuffered, playerID);
    }

    [PunRPC]
    public void RPC_TakePlayerDamage(int id) {
        PlayerScript player = PhotonView.Find(id).GetComponent<PlayerScript>();

        player.GotBitten(40f);

    }
    
}
