using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class changePanelUI : MonoBehaviour {


    [SerializeField]
    public GameObject[] panels;


    public void changePanel(int panelNum) {
        for (int i = 0; i < panels.Length; i++) {
            panels[i].SetActive(false);
        }
        panels[panelNum].SetActive(true);
    }
    
}
