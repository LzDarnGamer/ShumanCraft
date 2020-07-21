using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Onclick : MonoBehaviour
{
    Crafting crafting;

    DisplayInventory display;

    [SerializeField] MatrixInventory inventory;
    private PlayerScript player;

    private Button btn;
    private int id;
    private void Start() {
        btn = GetComponent<Button>();
        id = int.Parse(transform.parent.transform.GetChild(5).name);
        crafting = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>().GetComponent<Crafting>();
        display = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>().GetComponent<DisplayInventory>();
        player = GameObject.FindGameObjectWithTag("MainCanvas").transform.root.GetComponent<PlayerScript>();

        btn.onClick.AddListener(() => { craft(); crafting.UpdateCratables(); display.UpdateDisplay(); });
    }
    
    private void craft() {
        ItemObject _id = ItemsIndex.getItem(id);
        if (_id.itemID >= 800 && _id.itemID <= 850) {
            // Dar disable ao Panel para desaparecer

            /*
             * 1. Get the construction script
             * 2. Set the construction prefab
             * 3. Set the construction mode on
             */
            ConstructionController construct = player.construction;
            construct.SetObject(_id.inGameObject);            
            construct.HandleNewObject();
        }
        inventory.CraftItem(_id);
    }



}
