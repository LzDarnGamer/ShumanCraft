using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ConstructionController : MonoBehaviour {

    [SerializeField] private GameObject objects;

    [SerializeField] private GameObject placeableObjectPrefab;

    [SerializeField] private KeyCode newObjectHotKey = KeyCode.R;

    private bool isOnline = true;

    private Camera cam = null;

    private GameObject currentPlaceableObject;

    private float angle = 0f;
    private float angleInc = 2.2f; 

    void Update() {
        if (cam != null) {
            placeableObjectPrefab = objects;
            HandleNewObjectHotKey();

            if (currentPlaceableObject != null) {
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
        }
    }

    private void SetColliders(bool active) {
        foreach (Collider c in currentPlaceableObject.GetComponents<Collider>()) {
            c.enabled = active;
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
            if (hitInfo.collider.gameObject.tag.Equals("Terrain")) {
                Debug.Log("CONSTRUCTION ~ Hit Info: " + hitInfo.point);
                currentPlaceableObject.transform.position = hitInfo.point;
                currentPlaceableObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            } else Debug.Log("CONSTRUCTION ~ NO TERRAIN");
        } else {
            Debug.Log("CONSTRUCTION ~ NO RAYCAST");
        }
    }

    private void HandleNewObjectHotKey() {
        if (Input.GetKeyDown(newObjectHotKey)) {
            if (currentPlaceableObject == null) {

                currentPlaceableObject = !isOnline ? Instantiate(placeableObjectPrefab) :
                    PhotonNetwork.Instantiate(Path.Combine("Scriptable Objects\\Items\\Prefabs\\Placeables", placeableObjectPrefab.name), transform.position, Quaternion.identity, 0);
                
                SetColliders(false);
            } else {
                Destroy(currentPlaceableObject);
            }
        }
    }
}
