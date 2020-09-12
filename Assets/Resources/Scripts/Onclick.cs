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
    private GameObject canvas;

    private Button btn;
    private int id;

    private void Start() {
        btn = GetComponent<Button>();
        id = int.Parse(transform.parent.transform.GetChild(5).name);
        canvas = GameObject.FindGameObjectWithTag("MainCanvas");
        crafting = canvas.GetComponent<Canvas>().GetComponent<Crafting>();
        display = canvas.GetComponent<Canvas>().GetComponent<DisplayInventory>();
        player = canvas.transform.root.GetComponent<PlayerScript>();

        btn.onClick.AddListener(() => { craft(); crafting.UpdateCratables(); display.UpdateDisplay(); });
    }
    
    private void craft() {
        ItemObject _id = ItemsIndex.getItem(id);
        if (_id.itemID >= 800 && _id.itemID <= 850) {
            // Disable the panel
            Debug.Log("Found Info book: " + (canvas.transform.Find("InfoBook").gameObject != null));
            canvas.transform.Find("InfoBook").gameObject.SetActive(false);

            /*
             * 1. Get the construction script
             * 2. Set the construction prefab
             * 3. Set the construction mode on
             */
            ConstructionController construct = player.construction;
            construct.SetObject(_id.inGameObject);
            construct.HandleNewObject();
        } else if (_id.itemID == 900) {
            player.InstantiateBridge();
        } else if (_id.itemID == 901) {
            player.InstantiateBoat();
        } else {
            inventory.CraftItem(_id);
        }
    }



}
