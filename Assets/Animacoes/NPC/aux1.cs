using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aux1 : MonoBehaviour {

    public GameObject raio;
    public float value, valueClimb;

    private float initClimb;

    void Start() {
        raio.transform.localScale = Vector3.zero;
        initClimb = raio.transform.localPosition.y;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyUp(KeyCode.G)) {
            raio.transform.LeanMoveLocalY(1, .5f);
            LeanTween.scale(raio, new Vector3(value, value, value), .5f);
        } else if (Input.GetKeyUp(KeyCode.H)) {
            LeanTween.scale(raio, Vector3.zero, .5f); ;
            raio.transform.LeanMoveLocalY(initClimb, .5f);
        }
        
    }
}
