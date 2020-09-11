using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crafting : MonoBehaviour
{

    [SerializeField]
    MatrixInventory inventory;

    [SerializeField]
    GameObject contentCraftings;


    public void UpdateCratables() {
        for (int i = 0; i < contentCraftings.transform.childCount; i++) {
            GameObject Recipe = contentCraftings.transform.GetChild(i).GetChild(3).gameObject;
            
            for(int j = 0; j < Recipe.transform.childCount; j++) {
                int amount = int.Parse(Recipe.transform.GetChild(j).GetChild(2).GetComponent<TMPro.TMP_Text>().text);
                string txt = Recipe.transform.GetChild(j).GetChild(3).GetComponent<TMPro.TMP_Text>().text;
                int itemId = int.Parse(txt.Substring(1, txt.Length-1));
                if (!inventory.canCraft(itemId,amount)) {
                    contentCraftings.transform.GetChild(i).GetChild(4).GetComponent<Button>().interactable = false;
                    break;
                }
                contentCraftings.transform.GetChild(i).GetChild(4).GetComponent<Button>().interactable = true;
            }
            
        }
    }


}
