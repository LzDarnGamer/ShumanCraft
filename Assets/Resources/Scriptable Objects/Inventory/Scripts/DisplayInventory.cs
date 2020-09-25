using UnityEngine;
using UnityEngine.UI;
using System;

public class DisplayInventory : MonoBehaviour {
    [SerializeField] MatrixInventory AbstractInventory;
    [SerializeField] GameObject inventorySlots;
    [SerializeField] GameObject hotbarSlots;
    [SerializeField] GameObject amountPanel;


    [SerializeField] GameObject achivementChapter;
    [SerializeField] AchivementLog achivementLog;

    private FBConnector[][] RealInventory;

    void Start() {

        RealInventory = new FBConnector[2][];
        RealInventory[0] = new FBConnector[10];
        RealInventory[1] = new FBConnector[20];



        for (int i = 0; i < AbstractInventory.getHotbar().Length; i++) {
            RealInventory[0][i] = new FBConnector(hotbarSlots.transform.GetChild(i).gameObject, 
                AbstractInventory.getHotbar()[i]);
        }
        for (int i = 0; i < AbstractInventory.getInventory().Length; i++) {
            RealInventory[1][i] = new FBConnector(inventorySlots.transform.GetChild(i).gameObject,
                AbstractInventory.getInventory()[i]);
        }


        inventorySlots.transform.parent.gameObject.SetActive(false);
        UpdateDisplay();
    }
    
    public bool PickupItem(ItemObject it, int amount) {
        if(AbstractInventory.AddItem(it, amount)) {
            UpdateDisplay();
            return true;
        }
        return false;
    }


    public void UpdateDisplay() {
        SyncronizeInventories();
        ReloadTextures();
    }

    private void LoadAmount(int position, InventorySlot[] type, int inv) {
        int amount = type[position].amount;
        GameObject amountpan = Instantiate(amountPanel);
        amountpan.transform.GetChild(0).gameObject.GetComponent<TMPro.TMP_Text>().text = amount.ToString();
        amountpan.transform.SetParent(RealInventory[inv][position].InGameSlot.transform.GetChild(0).transform, false);
    }

    private void updateAmount(int position, InventorySlot[] type, GameObject panel) {
        int amount = type[position].amount;
        panel.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TMPro.TMP_Text>().text = amount.ToString();
    }

    public void UpdatePostion(int PosOld, int PosNew) {
        if (PosOld < 10 && PosNew < 10) {
            InventorySlot iv = new InventorySlot(AbstractInventory.getHotbar()[PosOld].item, AbstractInventory.getHotbar()[PosOld].amount);
            AbstractInventory.getHotbar()[PosOld] = AbstractInventory.getHotbar()[PosNew];
            AbstractInventory.getHotbar()[PosNew] = iv;

        } else if(PosOld < 10 && PosNew >= 10) {
            InventorySlot iv = new InventorySlot(AbstractInventory.getHotbar()[PosOld].item, AbstractInventory.getHotbar()[PosOld].amount);
            AbstractInventory.getHotbar()[PosOld] = AbstractInventory.getInventory()[PosNew-10];
            AbstractInventory.getInventory()[PosNew-10] = iv;

        } else if (PosOld >= 10 && PosNew >= 10) {
            InventorySlot iv = new InventorySlot(AbstractInventory.getInventory()[PosOld-10].item, AbstractInventory.getInventory()[PosOld-10].amount);
            AbstractInventory.getInventory()[PosOld-10] = AbstractInventory.getInventory()[PosNew-10];
            AbstractInventory.getInventory()[PosNew-10] = iv;

        } else if (PosOld >= 10 && PosNew < 10) {
            InventorySlot iv = new InventorySlot(AbstractInventory.getInventory()[PosOld-10].item, AbstractInventory.getInventory()[PosOld-10].amount);
            AbstractInventory.getInventory()[PosOld-10] = AbstractInventory.getHotbar()[PosNew];
            AbstractInventory.getHotbar()[PosNew] = iv;
        }

        UpdateDisplay();

    }

    private void SyncronizeInventories() {
        for (int i = 0; i < AbstractInventory.getHotbar().Length; i++) {
            RealInventory[0][i].ArraySlot = AbstractInventory.getHotbar()[i];
        }
        for (int i = 0; i < AbstractInventory.getInventory().Length; i++) {
            RealInventory[1][i].ArraySlot = AbstractInventory.getInventory()[i];

        }
    }

    private void ReloadTextures() {
        for (int i = 0; i < AbstractInventory.getHotbar().Length; i++) {
            if (RealInventory[0][i].ArraySlot.item != null) {
                RealInventory[0][i].InGameSlot.transform.GetChild(0).GetComponent<Image>().sprite
                    = RealInventory[0][i].ArraySlot.item.icon;
                if(RealInventory[0][i].InGameSlot.transform.GetChild(0).childCount == 0) {
                    LoadAmount(i, AbstractInventory.getHotbar(), 0);
                } else {
                    updateAmount(i, AbstractInventory.getHotbar(), RealInventory[0][i].InGameSlot.transform.GetChild(0).gameObject);
                }

            } else {
                RealInventory[0][i].InGameSlot.transform.GetChild(0).GetComponent<Image>().sprite
                = null;
                if (RealInventory[0][i].InGameSlot.transform.GetChild(0).childCount > 0) {
                    Destroy(RealInventory[0][i].InGameSlot.transform.GetChild(0).GetChild(0).gameObject);
                }
            }
        }

        for (int i = 0; i < AbstractInventory.getInventory().Length; i++) {
            if (RealInventory[1][i].ArraySlot.item != null) {
                RealInventory[1][i].InGameSlot.transform.GetChild(0).GetComponent<Image>().sprite
                    = RealInventory[1][i].ArraySlot.item.icon;
                if (RealInventory[1][i].InGameSlot.transform.GetChild(0).childCount == 0) {
                    LoadAmount(i, AbstractInventory.getInventory(), 1);
                } else {
                    updateAmount(i, AbstractInventory.getInventory(), RealInventory[1][i].InGameSlot.transform.GetChild(0).gameObject);
                }
            } else {
                RealInventory[1][i].InGameSlot.transform.GetChild(0).GetComponent<Image>().sprite
                = null;
                if (RealInventory[1][i].InGameSlot.transform.GetChild(0).childCount > 0) {
                    Destroy(RealInventory[1][i].InGameSlot.transform.GetChild(0).GetChild(0).gameObject);
                }
            }
        }

    }


    
    public void updateAchivement() {
        if (achivementChapter.activeSelf) {
            Debug.Log("Update");
            Achivement[] keys = achivementLog.keys;
            int[] values = achivementLog.values;
            int count = 0;
            for (int i = 1; i < achivementChapter.transform.childCount; i++) {
                Transform page = achivementChapter.transform.GetChild(i);
                for (int j = 0; j < page.transform.childCount; j++) {
                    string text = updateText(values[count], keys[count].requirement[1]);
                    page.transform.GetChild(j).GetChild(3).GetComponent<TMPro.TMP_Text>().text = text;
                    count++;
                }
            }
        }
    }


    private string updateText(int amount, int max) {
        return "Progress: "+amount+ "/" + max;
    }




    private void OnApplicationQuit() {
        //Array.Clear(AbstractInventory.getHotbar(), 0, AbstractInventory.getHotbar().Length);
        //Array.Clear(AbstractInventory.getInventory(), 0, AbstractInventory.getInventory().Length);
    }

}

