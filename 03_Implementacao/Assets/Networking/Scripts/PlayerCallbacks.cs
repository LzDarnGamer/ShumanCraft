using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class PlayerCallbacks : MonoBehaviourPunCallbacks {

    [SerializeField] PhotonView PV;

    override
    public void OnLeftRoom() {
        if (PV.IsMine) {
            string gamerTag = PhotonNetwork.NickName;
            PV.RPC("RPC_LeaveRoom", RpcTarget.AllBuffered, gamerTag);
        }
    }

    [PunRPC]
    public void RPC_LeaveRoom (string gamerTag) {
        Debug.Log("Player " + gamerTag + " just left the room");
    }
}
