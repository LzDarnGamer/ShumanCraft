using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDeattach : MonoBehaviour
{
    private void Awake() {
        this.gameObject.transform.parent = null;
    }
}
