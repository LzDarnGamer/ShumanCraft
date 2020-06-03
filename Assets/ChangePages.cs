using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePages : MonoBehaviour
{
    [SerializeField] GameObject MainPanel;
    [SerializeField] SoundScipt soundScipt;


    private GameObject currentPanel;
    private int currentPageNumber;
    public void onClick(int dir) {
        for (int i = 0; i < MainPanel.transform.childCount; i++) {
            if (MainPanel.transform.GetChild(i).gameObject.activeInHierarchy) {
                currentPanel = MainPanel.transform.GetChild(i).gameObject;
            }
        }

        for (int i = 0; i < currentPanel.transform.childCount; i++) {
            if (currentPanel.transform.GetChild(i).gameObject.activeInHierarchy) {
                currentPageNumber = i;
            }
        }

        
        int tempNumber = currentPageNumber + dir;
        if (tempNumber > -1 && tempNumber < currentPanel.transform.childCount) {
            currentPanel.transform.GetChild(currentPageNumber).gameObject.SetActive(false);
            currentPanel.transform.GetChild(currentPageNumber+=dir).gameObject.SetActive(true);
            soundScipt.PlayTurnPage();
        }
    }
}
