using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Interact : MonoBehaviour {

    private bool isEsc = false;

    [SerializeField]
    DisplayInventory display;
    [SerializeField]
    Hotbar_Manager hotbar_Manager;
    [SerializeField]
    MatrixInventory inventory;
    [SerializeField]
    PlayerScript playerScript;

    void Update() {
        
        if (Input.anyKeyDown) {
            if (Input.GetKeyDown(KeyCode.Mouse1)) {
                if (Eat()) {
                    display.UpdateDisplay();
                }else if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)), out RaycastHit ray
                    , 4)) {
                    GameObject hitObj = ray.transform.gameObject;
                    if (hitObj.CompareTag("Pickupable")) {
                        if (display.PickupItem(ray.transform.GetComponent<Item>().item, 1))
                            Destroy(hitObj);
                    }

                }
            }

        }
    }

    public bool Eat() {
      if (inventory.getHotbar()[hotbar_Manager.scrollPosition].item != null && typeof(FoodObject).IsInstanceOfType
            (inventory.getHotbar()[hotbar_Manager.scrollPosition].item)) {
            FoodObject Foodobj = (FoodObject)inventory.getHotbar()[hotbar_Manager.scrollPosition].item;
            playerScript.addHealth(Foodobj.healthRestore);
            playerScript.addHunger(Foodobj.hungerRestore);
            playerScript.addThirst(Foodobj.thirstRestore);
            inventory.getHotbar()[hotbar_Manager.scrollPosition].RemoveAmount(1);
            Debug.Log("HERE");
            return true;
        }
        return false;
    }

}

