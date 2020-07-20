using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsIndex : MonoBehaviour
{
    private static Dictionary<int, ItemObject> ITEMS;

    public void Awake() {
        ITEMS = new Dictionary<int, ItemObject>();

        FoodObject[] Fooditems = Resources.LoadAll<FoodObject>("Scriptable Objects/Items/Categories/Food");
        ArmourObject[] Armouritems = Resources.LoadAll<ArmourObject>("Scriptable Objects/Items/Categories/Armour");
        ToolObject[] Toolitems = Resources.LoadAll<ToolObject>("Scriptable Objects/Items/Categories/Tool");
        WeaponObject[] Weaponsitems = Resources.LoadAll<WeaponObject>("Scriptable Objects/Items/Categories/Weapons");
        MaterialObject[] Materialitems = Resources.LoadAll<MaterialObject>("Scriptable Objects/Items/Categories/Material");
        PlaceableObject[] Placeableitems = Resources.LoadAll<PlaceableObject>("Scriptable Objects/Items/Categories/Placeables");
        initializeItems(Fooditems);
        initializeItems(Armouritems);
        initializeItems(Toolitems);
        initializeItems(Weaponsitems);
        initializeItems(Materialitems);
        initializeItems(Placeableitems);

    }
    public static ItemObject getItem(int itemId) {
        ITEMS.TryGetValue(itemId, out ItemObject it);
        return it;
    }

    private void initializeItems(ItemObject[] it) {
        for (int i = 0; i < it.Length; i++) {
            ITEMS.Add(it[i].itemID, it[i]);
        }
    }
}
