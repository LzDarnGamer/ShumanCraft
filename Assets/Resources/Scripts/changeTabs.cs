using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeTabs : MonoBehaviour
{
    [SerializeField]
    public GameObject[] tabs;

    public void changePanel(int panelNum) {
        for (int i = 0; i < tabs.Length; i++) {
            tabs[i].SetActive(false);
        }
        tabs[panelNum].SetActive(true);
    }
}
