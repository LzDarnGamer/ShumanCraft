using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar_Manager : MonoBehaviour
{
    [SerializeField] Sprite defaultIcon;
    [SerializeField] Sprite selectedIcon;

    private KeyCode[] keyCodes = {
         KeyCode.Alpha0,
         KeyCode.Alpha1,
         KeyCode.Alpha2,
         KeyCode.Alpha3,
         KeyCode.Alpha4,
         KeyCode.Alpha5,
         KeyCode.Alpha6,
         KeyCode.Alpha7,
         KeyCode.Alpha8,
         KeyCode.Alpha9,
     };


    [SerializeField]
    GameObject hotbar;

    [SerializeField]
    public int scrollPosition;

    [SerializeField]
    Transform[] hotbarslots;
    
    void Start() {
        Cursor.visible = true;
        hotbarslots = new Transform[hotbar.transform.childCount];
        for (int i = 0; i < hotbar.transform.childCount; i++) {
            hotbarslots[i] = hotbar.transform.GetChild(i);
        }
        
    }

    void Update() {
        for (int i = 0; i < keyCodes.Length; i++) {
            if (Input.GetKeyDown(keyCodes[i])) {
                if (i == 0) {
                    hotbarslots[scrollPosition].transform.GetComponent<Image>().sprite = defaultIcon;
                    hotbarslots[9].transform.GetComponent<Image>().sprite = selectedIcon;
                    scrollPosition = 9;
                } else {
                    hotbarslots[scrollPosition].transform.GetComponent<Image>().sprite = defaultIcon;
                    hotbarslots[i-1].transform.GetComponent<Image>().sprite = selectedIcon;
                    scrollPosition = i-1;
                }
            }
        }


        if (Input.mouseScrollDelta.y >= 1) {
            hotbarslots[scrollPosition].transform.GetComponent<Image>().sprite = defaultIcon;
            scrollPosition++;
            scrollPosition = (scrollPosition % 10);
            hotbarslots[scrollPosition].transform.GetComponent<Image>().sprite = selectedIcon;
        } else if (Input.mouseScrollDelta.y <= -1) {
            hotbarslots[scrollPosition].transform.GetComponent<Image>().sprite = defaultIcon;
            scrollPosition--;
            if (scrollPosition < 0) {
                scrollPosition = 9;
            }
            hotbarslots[scrollPosition].transform.GetComponent<Image>().sprite = selectedIcon;
        }
    }

    public int getIndex() { return scrollPosition; }
}
