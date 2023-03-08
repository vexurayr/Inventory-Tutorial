using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Variables
    [SerializeField] protected int invSlots;
    [SerializeField] protected int invHandSlots;
    [SerializeField] protected InventoryItem emptyInvItem;

    protected List<InventoryItem> invItemList;
    protected List<InventoryItem> invHandItemList;
    protected List<InventoryItem> invItemArmorList;

    #endregion Variables

    #region MonoBehaviours
    public virtual void Awake()
    {
        invItemList = new List<InventoryItem>();
        invHandItemList = new List<InventoryItem>();
        invItemArmorList = new List<InventoryItem>();

        InitializeInventory();
    }

    #endregion MonoBehaviours

    #region GetSet
    public virtual int GetInvSlotCount()
    {
        return invSlots;
    }

    public virtual int GetInvHandSlotCount()
    {
        return invHandSlots;
    }

    public virtual List<InventoryItem> GetInvItemList()
    {
        return invItemList;
    }

    public virtual List<InventoryItem> GetInvHandItemList()
    {
        return invHandItemList;
    }

    public virtual List<InventoryItem> GetInvItemArmorList()
    {
        return invItemArmorList;
    }

    #endregion GetSet

    #region AddItemToInventory
    public virtual void AddToInventory(InventoryItem newItem)
    {
        if (newItem == null)
        {
            return;
        }

        if (newItem.GetItemType() == InventoryItem.ItemType.Resource || newItem.GetItemType() == InventoryItem.ItemType.Consumable)
        {
            // Check if incoming item can be added to an existing stack
            for (int i = 0; i < invItemList.Count; i++)
            {
                // Check if resource/consumable in this inventory slot matches the incoming item's
                if (invItemList[i].GetItem() == newItem.GetItem())
                {
                    int invMaxSize = invItemList[i].GetMaxStackSize();
                    int invCurrenSize = invItemList[i].GetItemCount();
                    int amountToAdd = newItem.GetItemCount();
                    int newAmount = invCurrenSize + amountToAdd;

                    // Skip slots that are already at capacity
                    if (invCurrenSize == invMaxSize)
                    {}
                    // Adding this will exceed max stack size, will add remainder to different stack or slot
                    else if (newAmount > invMaxSize)
                    {
                        int remainder = newAmount - invMaxSize;
                        invItemList[i].SetItemCount(invMaxSize);
                        newItem.SetItemCount(remainder);
                    }
                    // Whole stack can fit in this slot
                    else
                    {
                        invItemList[i].SetItemCount(newAmount);
                        return;
                    }
                }
            }
        }

        // Place object in first empty inventory slot
        for (int i = 0; i < invItemList.Count; i++)
        {
            if (invItemList[i].GetItemType() == InventoryItem.ItemType.Empty)
            {
                invItemList[i] = newItem;
                // Break out of the loop
                i = invItemList.Count;
            }
        }
    }

    public virtual void AddToInventory(GameObject newObj)
    {
        InventoryItem newItem = newObj.GetComponent<InventoryItem>();
        AddToInventory(newItem);
    }

    #endregion AddItemToInventory

    #region RemoveItemFromInventory
    public virtual void RemoveFromInventory(int itemAtIndexToRemove, bool isInvHandItem, bool isInvArmorItem)
    {
        if (itemAtIndexToRemove < 0 || itemAtIndexToRemove > invItemList.Count)
        {
            return;
        }

        if (isInvHandItem)
        {
            invHandItemList[itemAtIndexToRemove] = emptyInvItem;
        }
        else if (isInvArmorItem)
        {
            invItemArmorList[itemAtIndexToRemove] = emptyInvItem;
        }
        else
        {
            invItemList[itemAtIndexToRemove] = emptyInvItem;
        }
    }

    #endregion RemoveItemFromInventory

    #region HelperFunctions
    public virtual void InitializeInventory()
    {
        invItemList.Clear();
        invHandItemList.Clear();
        invItemArmorList.Clear();

        for (int i = 0; i < invSlots; i++)
        {
            invItemList.Add(emptyInvItem);
        }

        for (int i = 0; i < invHandSlots; i++)
        {
            invHandItemList.Add(emptyInvItem);
        }

        for (int i = 0; i < 3; i++)
        {
            invItemArmorList.Add(emptyInvItem);
        }
    }

    // Only checks invItemList because items will never be placed directly into the hotbar
    public bool IsInventoryFull()
    {
        foreach (InventoryItem item in invItemList)
        {
            if (item.GetItem() == InventoryItem.Item.None)
            {
                return false;
            }
        }

        return true;
    }

    public InventoryItem HasSameItemOfNonMaxStackSize(InventoryItem itemToCheck)
    {
        foreach (InventoryItem item in invItemList)
        {
            // Items match and the inventory item has room for more
            if (item.GetItem() == itemToCheck.GetItem() && item.GetItemCount() != item.GetMaxStackSize())
            {
                return item;
            }
        }

        return emptyInvItem;
    }

    public virtual void DisplayInventoryDebug()
    {
        string invItems = "";
        string invHandItems = "";

        foreach (InventoryItem item in invItemList)
        {
            invItems = invItems + ", " + item.GetItem();
        }

        foreach (InventoryItem item in invHandItemList)
        {
            invHandItems = invHandItems + ", " + item.GetItem();
        }

        Debug.Log("Inventory: " + invItems + "\nHotbar: " + invHandItems);
    }

    #endregion HelperFunctions
}