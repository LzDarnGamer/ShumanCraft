using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Onclick : MonoBehaviour
{
    Crafting crafting;

    DisplayInventory display;

    [SerializeField]
    MatrixInventory inventory;

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
        inventory.CraftItem(ItemsIndex.getItem(id));
    }



}
