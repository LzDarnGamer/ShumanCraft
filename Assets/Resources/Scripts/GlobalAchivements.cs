using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GlobalAchivements : MonoBehaviour
{
    [SerializeField]
    AudioClip achivementDone;

    [SerializeField]
    GameObject achivementPanel;

    Dictionary<string, int> achivements;
    void Start(){
        achivements = new Dictionary<string, int>();
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
