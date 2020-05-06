using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCheck : MonoBehaviour
{
    [SerializeField]
    MatrixInventory inventory;

    [SerializeField]
    Hotbar_Manager hotbar_Manager;

    [SerializeField]
    PlayerScript playerScript;
    public bool Eat() {
        if (inventory.getHotbar()[hotbar_Manager.scrollPosition].item != null && typeof(FoodObject).IsInstanceOfType
            (inventory.getHotbar()[hotbar_Manager.scrollPosition].item)) {
            FoodObject Foodobj = (FoodObject)inventory.getHotbar()[hotbar_Manager.scrollPosition].item;
            playerScript.addHealth(Foodobj.healthRestore);
            playerScript.addHunger(Foodobj.hungerRestore);
            playerScript.addThirst(Foodobj.thirstRestore);
            inventory.getHotbar()[hotbar_Manager.scrollPosition].RemoveAmount(1);
            return true;
        }
        return false;
    }

}
