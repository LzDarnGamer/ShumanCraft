using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text.RegularExpressions;
using com.ootii.Helpers;

public class PlayerAux : MonoBehaviour {

    [SerializeField] PhotonView PV;
    [SerializeField] private GameObject objectInHand;
    [SerializeField] private GameObject playerHand;

    public void SetObject(GameObject a) { objectInHand = a; }
    public void SetHand(GameObject b) { playerHand = b; }

    public void runDebugger(string a) {
        PV.RPC("RPC_Debugger", RpcTarget.AllBuffered, a);
    }

    public void handleChat(string txt, int id) {
        PV.RPC("RPC_UpdateChat", RpcTarget.AllBuffered, txt, id);
    }

    public void runRPC(string name, int handID, int instatiatedID, float[] position, float[] rotation) {
        PV.RPC("RPC_UpdateObject", RpcTarget.AllBuffered, name, handID, instatiatedID, position, rotation);
    }
    public void deleteObj(int id) {
        gameObject.GetComponent<PhotonView>().RPC("RPC_delete", RpcTarget.AllBuffered, id);
    }

    public void disableMaterialProperties(int id) {
        gameObject.GetComponent<PhotonView>().RPC("RPC_disableMaterialProperties", RpcTarget.AllBuffered, id);
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

    [PunRPC]
    private void RPC_UpdateChat(string textmsg, int id) {
        TMPro.TMP_Text msg = PhotonView.Find(id).gameObject.GetComponent<TMPro.TMP_Text>();

        msg.text = msg.text.Replace((char)10, (char)159);
        string[] aj = msg.text.Split((char)159);
        int a = aj.Length;

        Debug.Log("Numero: " + a);
        msg.text = (a > 6) ? textmsg : msg.text + textmsg;
        msg.text = msg.text.Replace((char)159, (char)10);
        //TMPro.TMP_Text msg = this.gameObject.GetComponent<PlayerScript>().chatMsgTxt;
    }

    [PunRPC]
    private void RPC_Debugger(string ajuda) {
        Debug.Log("DEBUG OF PLAYER DISCONNECTING: \"" + ajuda + "\"");
    }


    [PunRPC]
    private void RPC_delete(int id) {
        GameObject a = PhotonView.Find(id).gameObject;
        Destroy(a);
    }


    [PunRPC]
    private void RPC_disableMaterialProperties(int id) {
        GameObject a = PhotonView.Find(id).gameObject;
        a.GetComponent<MeshCollider>().enabled = false;
        a.GetComponent<Rigidbody>().isKinematic = true;
    }


}