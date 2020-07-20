using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Onclick : MonoBehaviour
{
    Crafting crafting;

    DisplayInventory display;

    [SerializeField] MatrixInventory inventory;

    private Button btn;
    private int id;
    private void Start() {
        btn = GetComponent<Button>();
        id = int.Parse(transform.parent.transform.GetChild(5).name);
        crafting = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>().GetComponent<Crafting>();
        display = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>().GetComponent<DisplayInventory>();

        btn.onClick.AddListener(() => { craft(); crafting.UpdateCratables(); display.UpdateDisplay(); });
    }
    
    private void craft() {
        ItemObject _id = ItemsIndex.getItem(id);
        inventory.CraftItem(_id);
        if (!(_id.itemID >= 800 && _id.itemID <= 850)) {
            //Aceder ao player
            Debug.Log((transform.root.gameObject.name) + ": IS PLAYER OR NOT");
        }
    }



}
