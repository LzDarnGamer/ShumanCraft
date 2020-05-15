using UnityEngine;
using UnityEngine.UI;
using System;

public class DisplayInventory : MonoBehaviour {
    [SerializeField] MatrixInventory AbstractInventory;
    [SerializeField] GameObject inventorySlots;
    [SerializeField] GameObject hotbarSlots;
    [SerializeField] GameObject amountPanel;

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

        for (int i = 0; i < AbstractInventory.getHotbar().Length; i++) {
            if (RealInventory[0][i].ArraySlot.item != null) {
                RealInventory[0][i].InGameSlot.transform.GetChild(0).GetComponent<Image>().sprite
                    = AbstractInventory.getHotbar()[i].item.icon;
                LoadAmount(i, AbstractInventory.getHotbar(), 0);
            }
        }

        for (int i = 0; i < AbstractInventory.getInventory().Length; i++) {
            if (RealInventory[1][i].ArraySlot.item != null) {
                RealInventory[1][i].InGameSlot.transform.GetChild(0).GetComponent<Image>().sprite
                    = AbstractInventory.getInventory()[i].item.icon;
                LoadAmount(i, AbstractInventory.getInventory(), 1);
            }
        }


 
    }

    private void LoadAmount(int position, InventorySlot[] type, int inv) {
        int amount = type[position].amount;
        GameObject amountpan = Instantiate(amountPanel);
        amountpan.transform.GetChild(0).gameObject.GetComponent<TMPro.TMP_Text>().text = amount.ToString();
        amountpan.transform.SetParent(RealInventory[inv][position].InGameSlot.transform.GetChild(0).transform, false);
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
            if (RealInventory[0][i].ArraySlot.item != null)
                RealInventory[0][i].InGameSlot.transform.GetChild(0).GetComponent<Image>().sprite
                    = RealInventory[0][i].ArraySlot.item.icon;
            else
                RealInventory[0][i].InGameSlot.transform.GetChild(0).GetComponent<Image>().sprite
                = Resources.Load<Sprite>("Resources/unity_buildin_extra/UISprite");
        }

        for (int i = 0; i < AbstractInventory.getInventory().Length; i++) {
            if (RealInventory[1][i].ArraySlot.item != null)
                RealInventory[1][i].InGameSlot.transform.GetChild(0).GetComponent<Image>().sprite
                    = RealInventory[1][i].ArraySlot.item.icon;
            else
                RealInventory[1][i].InGameSlot.transform.GetChild(0).GetComponent<Image>().sprite
                = Resources.Load<Sprite>("Resources/unity_buildin_extra/UISprite");
        }
    }

    public void updateAchivement() {

    }






    private void OnApplicationQuit() {
        //Array.Clear(AbstractInventory.getHotbar(), 0, AbstractInventory.getHotbar().Length);
        //Array.Clear(AbstractInventory.getInventory(), 0, AbstractInventory.getInventory().Length);
    }

}

