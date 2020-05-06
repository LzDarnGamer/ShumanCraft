using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionController : MonoBehaviour {

    [SerializeField]
    private GameObject[] objects;
    public int objectIndex = 0;

    [SerializeField]
    private GameObject placeableObjectPrefab;

    [SerializeField]
    private KeyCode newObjectHotKey = KeyCode.R;

    private GameObject currentPlaceableObject;

    private float angle = 0f;
    private float angleInc = 5f; 

    void Update() {
        placeableObjectPrefab = objects[objectIndex];
        HandleNewObjectHotKey();

        if (currentPlaceableObject != null) {
            MoveCurrentPlaceableObjectToMouse();
            RotateFromMouseWheel();
            ReleaseIfClicked();
        }

    }

    public void SetHotKey(KeyCode key) {
        this.newObjectHotKey = key;
    }

    public void SetObjects(GameObject[] obj) {
        this.objects = obj;
    }

    public void increaseIndex() {
        objectIndex = (objectIndex + 1) % objects.Length;
    }

    private void ReleaseIfClicked()
    {
        if (Input.GetMouseButtonDown(0))
        {
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo)) {

            if (hitInfo.collider.gameObject.tag.Equals("Terrain")) {
                currentPlaceableObject.transform.position = hitInfo.point;
                currentPlaceableObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            }
        }
    }

    private void HandleNewObjectHotKey()
    {
        if (Input.GetKeyDown(newObjectHotKey))
        {
            if (currentPlaceableObject == null)
            {
                currentPlaceableObject = Instantiate(placeableObjectPrefab);
                SetColliders(false);
            }
            else
            {
                Destroy(currentPlaceableObject);
            }
        }
    }
}
