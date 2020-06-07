using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class QuestbookGenerator : MonoBehaviour
{
    [Header("Infobook")]
    [SerializeField] GameObject foodExamplePanel;
    [SerializeField] GameObject armourExamplePanel;
    [SerializeField] GameObject toolExamplePanel;
    [SerializeField] GameObject weaponsExamplePanel;
    [SerializeField] GameObject materialExamplePanel;
    [SerializeField] GameObject craftingExamplePanel;

    [Header("QuestBook")]
    [SerializeField] Transform AchivementChapter;
    [SerializeField] GameObject AchivementPage;

    [Header("Script")]
    [SerializeField] AchivementList achivementList;

    private Transform contentFood;
    private Transform contentArmour;
    private Transform contentTool;
    private Transform contentWeapon;
    private Transform contentMaterial;
    private Transform contentCraftings;

    void Start() {
        achivementList = GameObject.Find("GameSetup").GetComponent<AchivementList>();

        FoodObject[] Fooditems = Resources.LoadAll<FoodObject>("Scriptable Objects/Items/Categories/Food");
        ArmourObject[] Armouritems = Resources.LoadAll<ArmourObject>("Scriptable Objects/Items/Categories/Armour");
        ToolObject[] Toolitems = Resources.LoadAll<ToolObject>("Scriptable Objects/Items/Categories/Tool");
        WeaponObject[] Weaponsitems = Resources.LoadAll<WeaponObject>("Scriptable Objects/Items/Categories/Weapons");
        MaterialObject[] Materialitems = Resources.LoadAll<MaterialObject>("Scriptable Objects/Items/Categories/Material");

        contentFood = GameObject.Find("Content - Food").transform;
        contentArmour = GameObject.Find("Content - Armour").transform;
        contentTool = GameObject.Find("Content - Tool").transform;
        contentWeapon = GameObject.Find("Content - Weapon").transform;
        contentMaterial = GameObject.Find("Content - Material").transform;
        contentCraftings = GameObject.Find("Content - Craftings").transform;

        foreach (var food in Fooditems) {
            string name = food.name;
            string itemID = food.itemID.ToString();
            string hungerRestore = food.hungerRestore.ToString();
            string healthRestore = food.healthRestore.ToString();
            string waterRestore = food.thirstRestore.ToString();
            string description = food.description;
            Sprite icon = food.icon;
            
            fillFoodInfo(foodExamplePanel, name, itemID, hungerRestore, healthRestore, waterRestore, description, icon);
        }

        foreach (var material in Materialitems) {
            string name = material.name;
            string itemID = material.itemID.ToString();
            string description = material.description;
            Sprite icon = material.icon;

            if (material.recipeItems != null && material.recipeItems.Length > 0) {
                fillCreaftingInfo(craftingExamplePanel, name, description, icon, 
                    material.recipeItems, material.recipeAmount, material.itemID);
            }
            fillMaterialInfo(materialExamplePanel, name, itemID, description, icon);
        }


        foreach (var tool in Toolitems) {
            string name = tool.name;
            string itemID = tool.itemID.ToString();
            string description = tool.description;
            Sprite icon = tool.icon;
            string durability = tool.durability.ToString();
            if (tool.recipeItems != null && tool.recipeItems.Length > 0) {
                fillCreaftingInfo(craftingExamplePanel, name, description, 
                    icon, tool.recipeItems, tool.recipeAmount, tool.itemID);
            }
            fillToolInfo(toolExamplePanel, name, itemID, durability, description, icon);
        }

        foreach (var weapon in Weaponsitems) {
            string name = weapon.name;
            string itemID = weapon.itemID.ToString();
            string description = weapon.description;
            Sprite icon = weapon.icon;
            string damage = weapon.damagePoints.ToString();
            string durability = weapon.durability.ToString();

            if (weapon.recipeItems != null && weapon.recipeItems.Length > 0) {
                fillCreaftingInfo(craftingExamplePanel, name, description, icon,
                    weapon.recipeItems, weapon.recipeAmount, weapon.itemID);
            }
            fillWeaponInfo(weaponsExamplePanel, name, itemID, durability, damage, description, icon);
        }

        List<Achivement> list = AchivementList.achivementsListNoHidden();
        for (int i = 0; i < list.Count; i+=2) {
            if (!list[i].isHidden) {
                if (i == list.Count - 1) {
                    fillAchivements(AchivementPage, list[i], list[i], i == list.Count - 1);
                } else {
                    fillAchivements(AchivementPage, list[i], list[i + 1], i == list.Count - 1);
                }
            }
        }
        AchivementChapter.parent.parent.gameObject.SetActive(false);

        contentFood.parent.gameObject.SetActive(false);
        contentArmour.parent.gameObject.SetActive(false);
        contentTool.parent.gameObject.SetActive(false);
        contentWeapon.parent.gameObject.SetActive(false);
        contentMaterial.parent.gameObject.SetActive(false);
        contentCraftings.parent.gameObject.SetActive(false);

        GameObject.Find("InfoBook").gameObject.SetActive(false);
    }



    private void fillFoodInfo(GameObject obj, string name, string itemID, string hungerRestore
        , string healthRestore, string waterRestore, string description, Sprite icon) {
        GameObject newObj = Instantiate(obj);


        newObj.transform.GetChild(0).gameObject.GetComponent<TMPro.TMP_Text>().text = description;
        newObj.transform.GetChild(1).gameObject.GetComponent<TMPro.TMP_Text>().text = name;
        newObj.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = icon;
        newObj.transform.GetChild(3).GetChild(0).gameObject.GetComponent<TMPro.TMP_Text>().text = hungerRestore;
        newObj.transform.GetChild(4).GetChild(0).gameObject.GetComponent<TMPro.TMP_Text>().text = waterRestore;
        newObj.transform.GetChild(5).GetChild(0).gameObject.GetComponent<TMPro.TMP_Text>().text = healthRestore;

        newObj.transform.SetParent(contentFood, false);
    }

    private void fillMaterialInfo(GameObject obj, string name, string itemID, string description, Sprite icon) {
        GameObject newObj = Instantiate(obj);


        newObj.transform.GetChild(0).gameObject.GetComponent<TMPro.TMP_Text>().text = description;
        newObj.transform.GetChild(1).gameObject.GetComponent<TMPro.TMP_Text>().text = name;
        newObj.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = icon;

        newObj.transform.SetParent(contentMaterial, false);
    }

    private void fillToolInfo(GameObject obj, string name, string itemID, string durability,string description, Sprite icon) {
        GameObject newObj = Instantiate(obj);


        newObj.transform.GetChild(0).gameObject.GetComponent<TMPro.TMP_Text>().text = description;
        newObj.transform.GetChild(1).gameObject.GetComponent<TMPro.TMP_Text>().text = name;
        newObj.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = icon;
        newObj.transform.GetChild(5).gameObject.GetComponent<TMPro.TMP_Text>().text = durability;

        newObj.transform.SetParent(contentTool, false);
    }
    private void fillWeaponInfo(GameObject obj, string name, string itemID, string durability, string damage, string description, Sprite icon) {
        GameObject newObj = Instantiate(obj);

        newObj.transform.GetChild(0).gameObject.GetComponent<TMPro.TMP_Text>().text = description;
        newObj.transform.GetChild(1).gameObject.GetComponent<TMPro.TMP_Text>().text = name;
        newObj.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = icon;
        newObj.transform.GetChild(3).gameObject.GetComponent<TMPro.TMP_Text>().text = durability;
        newObj.transform.GetChild(4).gameObject.GetComponent<TMPro.TMP_Text>().text = damage;

        newObj.transform.SetParent(contentWeapon, false);
    }

    private void fillCreaftingInfo(GameObject obj, string name, string description, 
        Sprite icon, ItemObject[] recipeItem, int[] amountItem, int itemID) {
        GameObject newObj = Instantiate(obj);

        newObj.transform.GetChild(0).gameObject.GetComponent<TMPro.TMP_Text>().text = description;
        newObj.transform.GetChild(1).gameObject.GetComponent<TMPro.TMP_Text>().text = name;
        newObj.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = icon;

        GameObject Recipe = newObj.transform.GetChild(3).gameObject;
        GameObject Item = Recipe.transform.GetChild(0).gameObject;
        for (int i = 0; i < recipeItem.Length; i++) {
            GameObject item = Instantiate(Item);
            item.transform.GetChild(0).gameObject.GetComponent<TMPro.TMP_Text>().text = recipeItem[i].name;
            item.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = recipeItem[i].icon;
            item.transform.GetChild(2).gameObject.GetComponent<TMPro.TMP_Text>().text = amountItem[i].ToString();
            item.transform.GetChild(3).gameObject.GetComponent<TMPro.TMP_Text>().text = "#"+ recipeItem[i].itemID.ToString();
            item.transform.SetParent(Recipe.transform, false);
        }
        newObj.transform.GetChild(4).gameObject.GetComponent<Button>().interactable = false;
        newObj.transform.GetChild(5).name = itemID.ToString();
        Destroy(Item);
        newObj.transform.SetParent(contentCraftings, false);
    }


    private void fillAchivements(GameObject obj, Achivement ach, Achivement ach1, bool isLast) {
        
        GameObject newObj = Instantiate(obj);
        GameObject leftPage = setPage(newObj.transform.GetChild(0).gameObject, ach);
        GameObject rightPage = setPage(newObj.transform.GetChild(1).gameObject, ach1);

        if (isLast) {
            Destroy(rightPage);
        }
        newObj.transform.SetParent(AchivementChapter, false);
        newObj.SetActive(false);
    }

    private GameObject setPage(GameObject obj, Achivement ach) {
        obj.transform.GetChild(0).GetComponent<Image>().sprite = ach.icon;
        obj.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text = ach.achName;
        obj.transform.GetChild(2).GetComponent<TMPro.TMP_Text>().text = ach.description;
        obj.transform.GetChild(3).GetComponent<TMPro.TMP_Text>().text = "Progress: 0/"  + ach.requirement[1];
        return obj;
    }

}
