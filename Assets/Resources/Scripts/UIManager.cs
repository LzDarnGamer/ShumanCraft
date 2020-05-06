using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class UIManager : MonoBehaviour {
    private KeyCode[] keys = {
        KeyCode.E,
        KeyCode.Tab,
        KeyCode.Escape
    };
    private bool isMouseActive;
    private bool isEscActive;
    private bool isQuestActive;
    private bool isInvEActive;

    [SerializeField]
    GameObject QuestPanel;

    [SerializeField]
    GameObject BackgroundPanel;

    [SerializeField]
    GameObject EscPanel;

    [SerializeField]
    GameObject InvPanel;

    [SerializeField]
    Crafting crafting;


    void Update() {

        if (Input.anyKey)
            if (Array.Exists(keys, e => Input.GetKeyDown(e))) {
                isMouseActive = true;
                BackgroundPanel.SetActive(true);

                if (Input.GetKey(KeyCode.Escape)) {
                    isEscActive = !isEscActive;
                    if (isEscActive) {
                        EscPanel.SetActive(true);

                    } else {
                        EscPanel.SetActive(false);
                        BackgroundPanel.SetActive(false);
                        isMouseActive = false;
                    }
                }
                if (Input.GetKey(KeyCode.E)) {
                    isInvEActive = !isInvEActive;
                    if (isInvEActive) {
                        InvPanel.SetActive(true);
                        
                    } else {
                        InvPanel.SetActive(false);
                        BackgroundPanel.SetActive(false);
                        isMouseActive = false;
                    }
                }
                if (Input.GetKey(KeyCode.Tab)) {
                    isQuestActive = !isQuestActive;
                    if (isQuestActive) {
                        QuestPanel.SetActive(true);
                        crafting.UpdateCratables();
                    } else {
                        QuestPanel.SetActive(false);
                        BackgroundPanel.SetActive(false);
                        isMouseActive = false;
                    }
                }
            }
    } 
    private void LateUpdate() {
        if (isMouseActive) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
