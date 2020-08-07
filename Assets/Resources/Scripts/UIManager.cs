using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class UIManager : MonoBehaviour {
    private KeyCode[] keys = {
        KeyCode.E,
        KeyCode.Tab,
        KeyCode.Escape,
        KeyCode.B,
    };

    private KeyCode currentActive = KeyCode.Pipe;

    private bool isMouseActive;
    private bool isEscActive;
    private bool isTabActive;
    private bool isInvEActive;
    private bool isQuestBookActive;

    [SerializeField] GameObject Infobook;
    [SerializeField] GameObject BackgroundPanel;
    [SerializeField] GameObject EscPanel;
    [SerializeField] GameObject InvPanel;
    [SerializeField] GameObject QuestBookPanel;
    [SerializeField] DisplayInventory displayInventory;
    [SerializeField] Crafting crafting;

    void Update() {

        if (Input.anyKey)
            if (Array.Exists(keys, e => Input.GetKeyDown(e))) {
                isMouseActive = true;
                BackgroundPanel.SetActive(true);

                if (Input.GetKey(KeyCode.Escape) && 
                    (currentActive == KeyCode.Pipe || currentActive == KeyCode.Escape )) {
                    currentActive = KeyCode.Escape;
                    isEscActive = !isEscActive;
                    if (isEscActive) {
                        EscPanel.SetActive(true);

                    } else {
                        EscPanel.SetActive(false);
                        BackgroundPanel.SetActive(false);
                        isMouseActive = false;
                        currentActive = KeyCode.Pipe;
                    }
                }
                if (Input.GetKey(KeyCode.E) &&
                    (currentActive == KeyCode.Pipe || currentActive == KeyCode.E)) {
                    currentActive = KeyCode.E;
                    isInvEActive = !isInvEActive;
                    if (isInvEActive) {
                        InvPanel.SetActive(true);
                    } else {
                        InvPanel.SetActive(false);
                        BackgroundPanel.SetActive(false);
                        isMouseActive = false;
                        currentActive = KeyCode.Pipe;
                    }
                }
                if (Input.GetKey(KeyCode.Tab) &&
                    (currentActive == KeyCode.Pipe || currentActive == KeyCode.Tab)) {
                    currentActive = KeyCode.Tab;
                    isTabActive = !isTabActive;
                    if (isTabActive) {
                        Infobook.SetActive(true);
                        crafting.UpdateCratables();
                    } else {
                        Infobook.SetActive(false);
                        BackgroundPanel.SetActive(false);
                        isMouseActive = false;
                        currentActive = KeyCode.Pipe;
                    }
                }
                if (Input.GetKey(KeyCode.B) &&
                    (currentActive == KeyCode.Pipe || currentActive == KeyCode.B)) {
                    currentActive = KeyCode.B;
                    isQuestBookActive = !isQuestBookActive;
                    if (isQuestBookActive) {
                        displayInventory.updateAchivement();
                        QuestBookPanel.SetActive(true);
                    } else {
                        QuestBookPanel.SetActive(false);
                        BackgroundPanel.SetActive(false);
                        isMouseActive = false;
                        currentActive = KeyCode.Pipe;
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

    public void ResumeGame() {
        EscPanel.SetActive(false);
        BackgroundPanel.SetActive(false);
        isMouseActive = false;
        currentActive = KeyCode.Pipe;
    }

    public void QuitGame() {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                    Application.Quit();
        #endif
    }

}
