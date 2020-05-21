using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    private CanvasGroup canvasGroup;

    private Canvas canvas;

    private Transform actualParent;

    GameObject g;
    private bool isDragable = true;
    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = FindObjectOfType<Canvas>();
        actualParent =  transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (transform.gameObject.GetComponent<Image>().sprite != null)
            if(!transform.gameObject.GetComponent<Image>().sprite.name.Equals("UISprite")) {
                canvasGroup.blocksRaycasts = false;
                g = Instantiate(Resources.Load<GameObject>("Scriptable Objects/Inventory/Prefabs/invSlotImg"));
                g.GetComponent<Image>().sprite = transform.GetComponent<Image>().sprite;
                g.transform.SetParent(canvas.transform);
        }
    }   

    public void OnDrag(PointerEventData eventData) {
        if (transform.gameObject.GetComponent<Image>().sprite != null)
            if(!transform.gameObject.GetComponent<Image>().sprite.name.Equals("UISprite")) {
            g.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        Destroy(g);
        canvasGroup.blocksRaycasts = true;
        GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
       
        
    }


}
