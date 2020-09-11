using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ConstructionController : MonoBehaviour {

    [SerializeField] private GameObject placeableObjectPrefab;

    [SerializeField] private KeyCode newObjectHotKey = KeyCode.R;

    [SerializeField] private bool isUsing = false;

    private bool isOnline = true;

    private Camera cam = null;

    private GameObject currentPlaceableObject;

    private float angle = 0f;
    private float angleInc = 2.2f;

    private ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();

    void Update() {
        if (cam != null) {
            //HandleNewObjectHotKey();

            if (isUsing && currentPlaceableObject != null) {
                MoveCurrentPlaceableObjectToMouse();
                RotateFromMouseWheel();
                ReleaseIfClicked();
            }
        }
    }

    public void setOnline(bool y) { isOnline = y; }

    public void setCamera(Camera cam) { this.cam = cam; }

    private void ReleaseIfClicked() {
        if (Input.GetMouseButtonDown(0)) {
            SetColliders(true);
            currentPlaceableObject = null;
            isUsing = false;
        }
    }

    private void SetColliders(bool active) {
        foreach (Collider c in currentPlaceableObject.GetComponents<Collider>()) {
            c.enabled = active;
        }
        if (active) {
            hash = PhotonNetwork.LocalPlayer.CustomProperties;
            // Save position
            if (hash.ContainsKey("iposX")) { hash["iposX"] = currentPlaceableObject.transform.position.x; } else { hash.Add("iposX", currentPlaceableObject.transform.position.x); }
            if (hash.ContainsKey("iposY")) { hash["iposY"] = currentPlaceableObject.transform.position.y; } else { hash.Add("iposY", currentPlaceableObject.transform.position.y); }
            if (hash.ContainsKey("iposZ")) { hash["iposZ"] = currentPlaceableObject.transform.position.z; } else { hash.Add("iposZ", currentPlaceableObject.transform.position.z); }
            // Save name
            if (hash.ContainsKey("iName")) { hash["iName"] = placeableObjectPrefab.name; } else { hash.Add("iName", placeableObjectPrefab.name); }
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    private void RotateFromMouseWheel() {
        if (Input.GetKey(KeyCode.K)) angle += angleInc;
        if (Input.GetKey(KeyCode.L)) angle -= angleInc;
        currentPlaceableObject.transform.Rotate(Vector3.up, angle);
    }

    private void MoveCurrentPlaceableObjectToMouse()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        Debug.DrawRay(cam.transform.position, ray.direction, Color.green);
        if (Physics.Raycast(ray, out hitInfo)) {
            if (hitInfo.collider.gameObject.tag.Equals("Sand")) {
                //Debug.Log("CONSTRUCTION ~ Hit Info: " + hitInfo.point);
                currentPlaceableObject.transform.position = hitInfo.point;
                currentPlaceableObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            } else Debug.Log("CONSTRUCTION ~ NO TERRAIN");
        } else {
            //Debug.Log("CONSTRUCTION ~ NO RAYCAST");
        }
    }

    /*
     * Set Object to place in the map
     */
    public void SetObject(GameObject obj) {
        placeableObjectPrefab = obj;
    }

    /*
     * Set Construction Mode On
     */
    public void HandleNewObject() {
        //if (Input.GetKeyDown(newObjectHotKey)) {
            //if (currentPlaceableObject == null) {

            currentPlaceableObject = !isOnline ? Instantiate(placeableObjectPrefab) :
                PhotonNetwork.Instantiate(Path.Combine("Scriptable Objects\\Items\\Prefabs\\Placeables", placeableObjectPrefab.name), transform.position, Quaternion.identity, 0);
                
            SetColliders(false);
            isUsing = true;
            //} else {
            //    Destroy(currentPlaceableObject);
            //}
        //}
    }
}
