using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class PlayerAux : MonoBehaviour {

    [SerializeField] PhotonView PV;
    [SerializeField] private GameObject objectInHand;
    [SerializeField] private GameObject playerHand;
    [SerializeField] private List<ExitGames.Client.Photon.Hashtable> playersInfo;

    public void SetObject(GameObject a) { objectInHand = a; }
    public void SetHand(GameObject b) { playerHand = b; }

    public void runSaveRPC (int id) {
        PV.RPC("RPC_SaveInfo", RpcTarget.AllBuffered, id);
    }

    [PunRPC]
    private void RPC_SaveInfo (int id) {
        PhotonView obj = PhotonView.Find(id);
        
        float hp = Convert.ToInt32(obj.Owner.CustomProperties["Health"]);
        float hunger = Convert.ToInt32(obj.Owner.CustomProperties["Hunger"]);

        Debug.Log("RPC_SaveInfo HEALTH " + hp);
        Debug.Log("RPC_SaveInfo HUNGER " + hunger);
    }

    public void runRPC(string name, int handID, int instatiatedID, float[] position, float[] rotation) {
        PV.RPC("RPC_UpdateObject", RpcTarget.AllBuffered, name, handID, instatiatedID, position, rotation);
    } 

    [PunRPC]
    private void RPC_UpdateObject(string name, int handID, int instatiatedID, float[] position, float[] rotation) {

        GameObject _hand = PhotonView.Find(handID).gameObject;
        GameObject _instantiated = PhotonView.Find(instatiatedID).gameObject;

        _instantiated.transform.SetParent(_hand.transform);
        bool a = _hand != null;
        Debug.Log("PV: " + PV.ViewID + " PlayerHand:" + a.ToString());

        Vector3 _pos = new Vector3(position[0], position[1], position[2]);
        Quaternion _rot = new Quaternion(rotation[0], rotation[1], rotation[2], rotation[3]);

        _instantiated.transform.localPosition = _pos;
        _instantiated.transform.localRotation = _rot;
                
        /*
        Vector3 scal    = objectInHand.transform.localScale;
           
        i.transform.SetParent(playerHand.transform, true);

        i.transform.localPosition = pos;
        i.transform.localRotation = rot;
        i.transform.localScale = scal;
        */
    }
}