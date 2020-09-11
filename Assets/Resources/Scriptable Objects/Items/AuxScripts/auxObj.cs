using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class auxObj : MonoBehaviour {
    bool hit = false;

    public Vector3 t = new Vector3(-0.2f, 0.4f, 0.1f);
    public Quaternion t1 = new Quaternion(-167.605f, 165.34f, -78.35199f, 0);
    public Vector3 t2 = new Vector3(0.3f, 0.3f, 0.3f);

    private void Update() {
        transform.localPosition = t;
        transform.localScale = t2;
        transform.localRotation = t1;
    }
}
