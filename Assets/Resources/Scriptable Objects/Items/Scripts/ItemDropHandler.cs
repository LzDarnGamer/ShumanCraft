using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ItemDropHandler : MonoBehaviour, IDropHandler
{
    [SerializeField]
    DisplayInventory DisplayInv;
    public void OnDrop(PointerEventData eventData) {
        
        if (eventData.pointerDrag != null 
            && eventData.pointerDrag.GetComponent<Image>().sprite.name != "UISprite") {


            
            string PosOld = eventData.pointerDrag.name.Substring(7, eventData.pointerDrag.name.Length-7);
            string Posnew = name.Substring(7, name.Length-7);

            
            DisplayInv.UpdatePostion(int.Parse(PosOld)-1, int.Parse(Posnew)-1);
            

            /*
            Sprite i = GetComponent<Image>().sprite;
            GetComponent<Image>().sprite = eventData.pointerDrag.GetComponent<Image>().sprite;
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            eventData.pointerDrag.GetComponent<Image>().sprite = i;
            */    
    } 
    }
}
