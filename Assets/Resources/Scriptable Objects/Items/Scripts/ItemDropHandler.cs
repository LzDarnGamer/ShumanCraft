using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ItemDropHandler : MonoBehaviour, IDropHandler
{
    private DisplayInventory DisplayInv;


    private void Start() {
        DisplayInv = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<DisplayInventory>();
    }
    public void OnDrop(PointerEventData eventData) {
        
        if (eventData.pointerDrag != null 
            && eventData.pointerDrag.GetComponent<Image>().sprite.name != "UISprite") {


            
            string PosOld = eventData.pointerDrag.name.Substring(7, eventData.pointerDrag.name.Length-7);
            string Posnew = name.Substring(7, name.Length-7);

            
            DisplayInv.UpdatePostion(int.Parse(PosOld)-1, int.Parse(Posnew)-1);
            

    } 
    }
}
