using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class MatrixInventory : ScriptableObject
{
    [SerializeField]
    private InventorySlot[] inventory;

    [SerializeField]
    private InventorySlot[] hotbar;

    public InventorySlot[] getInventory() {
        return inventory;
    }

    public InventorySlot[] getHotbar() {
        return hotbar;
    }

    public bool AddItem(ItemObject newItem, int amount) {
        return CheckHotbarInventory(newItem, amount);
    }

    private bool CheckHotbarInventory(ItemObject newItem, int amount) {
        int emptyPos = -1;
        bool onetimeHot = false;
        bool onetimeInv = false;
        for (int i = 0; i < hotbar.Length; i++) {
            if (hotbar[i].item == null && !onetimeHot) {
                emptyPos = i;
                onetimeHot = !onetimeHot;
            }
            if (hotbar[i].item != null
                && newItem.itemID == hotbar[i].item.itemID
                && hotbar[i].amount + amount <= hotbar[i].item.maxStackSize
                && newItem.isStackable) {
                hotbar[i].AddAmount(amount);
                return true;
            }
        }

        for (int i = 0; i < inventory.Length; i++) {
            if (inventory[i].item == null && !onetimeInv && !onetimeHot) {
                emptyPos = i;
                onetimeInv = !onetimeInv;
            }
            if (inventory[i].item != null
                && newItem.itemID == inventory[i].item.itemID
                && inventory[i].amount + amount <= inventory[i].item.maxStackSize
                && newItem.isStackable) {
                inventory[i].AddAmount(amount);
                return true;
            }
        }
        if (emptyPos == -1) {
            return false;
        }
        if (onetimeHot) {
            hotbar[emptyPos] = new InventorySlot(newItem, amount);
        }else if(onetimeInv) {
            inventory[emptyPos] = new InventorySlot(newItem, amount);
        }
        
        return true;
    }

    public bool canCraft(int itemID, int amount) {
        for (int i = 0; i < hotbar.Length; i++) {
            if (hotbar[i].item != null) {
                if (hotbar[i].item.itemID == itemID && hotbar[i].amount >= amount) {
                    return true;
                }
            }
        }
        for (int i = 0; i < inventory.Length; i++) {
            if (inventory[i].item != null) {
                if (inventory[i].item.itemID == itemID && inventory[i].amount >= amount) {
                    return true;
                }
            }
        }
        return false;
    }

    /*
     * Tira os items que necessarios para o crafting
     * e adiciona o item que e necessario craftear
     */
    public void CraftItem(ItemObject item) {
        bool hasFound = false;
        bool alreadyAdded = false;
        for (int i = 0; i < item.recipeItems.Length; i++) {
            // Pesquisa na hotbar
            for (int j = 0; j < hotbar.Length; j++) {
                if (hotbar[j].item != null) {
                    if (hotbar[j].item.itemID == item.recipeItems[i].itemID && hotbar[j].amount >= item.recipeAmount[i]) {
                        hotbar[j].RemoveAmount(item.recipeAmount[i]);
                        if (!(item.itemID >= 800 && item.itemID <= 850) && !alreadyAdded) {
                            alreadyAdded = true;
                            AddItem(item, 1);
                        }
                        hasFound = !hasFound;
                        break;
                    }
                }
            }
            if (hasFound) {
                hasFound = !hasFound;
                continue;
            }

            // Pesquisa no inventario
            for (int j = 0; j < inventory.Length; j++) {
                if (inventory[j].item != null) {
                    if (inventory[j].item.itemID == item.recipeItems[i].itemID &&
                        inventory[j].amount >= item.recipeAmount[i]) {
                        inventory[j].RemoveAmount(item.recipeAmount[i]);
                        if (!(item.itemID >= 800 && item.itemID <= 850) && !alreadyAdded) {
                            alreadyAdded = true;
                            AddItem(item, 1);
                        }
                    }
                }
            }
        }
    }
}

[System.Serializable]
public class InventorySlot {
    public ItemObject item;
    public int amount;

    public InventorySlot(ItemObject item, int amount) {
        this.item = item;
        this.amount = amount;
    }

    public void AddAmount(int amount) {
        this.amount += amount;
    }

    public void RemoveAmount(int amount) {
        this.amount -= amount;
        if(this.amount == 0) {
            RemoveItem();
        }
    }

    private void RemoveItem() {
        item = null;
        amount = 0;
    }
}
