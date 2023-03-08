using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : Inventory
{
    #region Variables
    [SerializeField] private InventoryUI inventoryUI;

    private List<GameObject> invSlotsUI;
    private List<GameObject> invItemsUI;
    private List<GameObject> invItemCountersUI;
    private List<GameObject> invHandSlotsUI;
    private List<GameObject> invHandItemsUI;
    private List<GameObject> invHandItemCountersUI;
    private List<GameObject> invArmorSlotsUI;
    private List<GameObject> invArmorItemsUI;
    private List<GameObject> invArmorItemCountersUI;

    private int selectedInvHandSlot = 0;

    #endregion Variables

    #region MonoBehaviours
    public override void Awake()
    {
        base.Awake();

        invSlotsUI = inventoryUI.GetInvSlotsUI();
        invItemsUI = inventoryUI.GetInvItemsUI();
        invItemCountersUI = inventoryUI.GetInvItemCountersUI();
        invHandSlotsUI = inventoryUI.GetInvHandSlotsUI();
        invHandItemsUI = inventoryUI.GetInvHandItemsUI();
        invHandItemCountersUI = inventoryUI.GetInvHandItemCountersUI();
        invArmorSlotsUI = inventoryUI.GetInvArmorSlotsUI();
        invArmorItemsUI = inventoryUI.GetInvArmorItemsUI();
        invArmorItemCountersUI = inventoryUI.GetInvArmorItemCountersUI();

        RefreshInventoryVisuals();
    }

    #endregion MonoBehaviours

    #region GetSet
    public InventoryUI GetInventoryUI()
    {
        return inventoryUI;
    }

    public int GetSelectedInvHandSlot()
    {
        return selectedInvHandSlot;
    }

    public void SetSelectedInvHandSlot(int slotIndex)
    {
        selectedInvHandSlot = slotIndex;
    }

    #endregion GetSet

    #region RefreshInventoryVisuals
    public void RefreshInventoryVisuals()
    {
        // Move UI inventory items to the correct location
        for (int i = 0; i < invItemList.Count; i++)
        {
            invItemsUI[i].transform.position = invSlotsUI[i].transform.position;
        }

        for (int i = 0; i < invHandItemList.Count; i++)
        {
            invHandItemsUI[i].transform.position = invHandSlotsUI[i].transform.position;
        }

        for (int i = 0; i < invItemArmorList.Count; i++)
        {
            invArmorItemsUI[i].transform.position = invArmorSlotsUI[i].transform.position;
        }

        // Set correct sprites and text for stack size and adjust IndexValue
        for (int i = 0; i < invItemList.Count; i++)
        {
            if (invItemList[i].GetItemSprite() == null)
            {
                invItemsUI[i].GetComponent<RawImage>().color = new Color(255, 255, 255, 0);
                invItemsUI[i].GetComponent<RawImage>().texture = null;
            }
            else
            {
                invItemsUI[i].GetComponent<RawImage>().color = new Color(255, 255, 255, 255);
                invItemsUI[i].GetComponent<RawImage>().texture = invItemList[i].GetItemSprite().texture;
            }

            if (invItemList[i].GetItemCount() > 1)
            {
                invItemCountersUI[i].GetComponent<Text>().text = invItemList[i].GetItemCount().ToString();
            }
            else
            {
                invItemCountersUI[i].GetComponent<Text>().text = "";
            }

            invItemsUI[i].GetComponent<IndexValue>().SetIndexValue(i);
        }

        for (int i = 0; i < invHandItemList.Count; i++)
        {
            if (invHandItemList[i].GetItemSprite() == null)
            {
                invHandItemsUI[i].GetComponent<RawImage>().color = new Color(255, 255, 255, 0);
                invHandItemsUI[i].GetComponent<RawImage>().texture = null;
            }
            else
            {
                invHandItemsUI[i].GetComponent<RawImage>().color = new Color(255, 255, 255, 255);
                invHandItemsUI[i].GetComponent<RawImage>().texture = invHandItemList[i].GetItemSprite().texture;
            }

            if (invHandItemList[i].GetItemCount() > 1)
            {
                invHandItemCountersUI[i].GetComponent<Text>().text = invHandItemList[i].GetItemCount().ToString();
            }
            else
            {
                invHandItemCountersUI[i].GetComponent<Text>().text = "";
            }

            invHandItemsUI[i].GetComponent<IndexValue>().SetIndexValue(i);
        }

        for (int i = 0; i < invItemArmorList.Count; i++)
        {
            if (invItemArmorList[i].GetItemSprite() == null)
            {
                invArmorItemsUI[i].GetComponent<RawImage>().color = new Color(255, 255, 255, 0);
                invArmorItemsUI[i].GetComponent<RawImage>().texture = null;
            }
            else
            {
                invArmorItemsUI[i].GetComponent<RawImage>().color = new Color(255, 255, 255, 255);
                invArmorItemsUI[i].GetComponent<RawImage>().texture = invItemArmorList[i].GetItemSprite().texture;
            }

            if (invItemArmorList[i].GetItemCount() > 1)
            {
                invArmorItemCountersUI[i].GetComponent<Text>().text = invItemArmorList[i].GetItemCount().ToString();
            }
            else
            {
                invArmorItemCountersUI[i].GetComponent<Text>().text = "";
            }

            invArmorItemsUI[i].GetComponent<IndexValue>().SetIndexValue(i);
        }
    }

    #endregion RefreshInventoryVisuals

    #region SwapInvItems
    // Called whenever the player adjusts Inventory through the UI
    public void SwapTwoInvItems(int first, int second)
    {
        // Swap Inventory Items in Inventory
        InventoryItem firstInvItem = invItemList[first];
        InventoryItem secondInvItem = invItemList[second];
        
        invItemList[first] = secondInvItem;
        invItemList[second] = firstInvItem;
        
        RefreshInventoryVisuals();
    }

    public void SwapTwoInvHandItems(int first, int second)
    {
        InventoryItem firstInvHandItem = invHandItemList[first];
        InventoryItem secondInvHandItem = invHandItemList[second];

        invHandItemList[first] = secondInvHandItem;
        invHandItemList[second] = firstInvHandItem;

        RefreshInventoryVisuals();
    }

    public void SwapInvItemWithHandItem(int firstFromInv, int secondFromHandInv)
    {
        InventoryItem invItem = invItemList[firstFromInv];
        InventoryItem invHandItem = invHandItemList[secondFromHandInv];

        invItemList[firstFromInv] = invHandItem;
        invHandItemList[secondFromHandInv] = invItem;

        RefreshInventoryVisuals();
    }

    // Index of 0 = helmet, Index of 1 = chestplate, Index of 2 = leggings
    public void SwapArmorItemWithInvItem(int firstFromArmorInv, int secondFromInv)
    {
        // The invItem in this case is the armor being equipped
        InventoryItem invArmorItem = invItemArmorList[firstFromArmorInv];
        InventoryItem invItem = invItemList[secondFromInv];

        bool isInvArmorItemEmpty = false;
        bool isInvItemEmpty = false;

        if (invArmorItem.GetItemType() == InventoryItem.ItemType.Empty)
        {
            isInvArmorItemEmpty = true;
        }
        if (invItem.GetItemType() == InventoryItem.ItemType.Empty)
        {
            isInvItemEmpty = true;
        }

        if (isInvArmorItemEmpty && isInvItemEmpty)
        {
            return;
        }
        // Attempting to equip armor in empty armor slot
        else if (isInvArmorItemEmpty && !isInvItemEmpty)
        {
            // Only equip armor if the type matches the kind of armor the slot is meant to store
            if (invItem.GetItemType() == InventoryItem.ItemType.Helmet && firstFromArmorInv == 0)
            {
                invItemArmorList[firstFromArmorInv] = invItem;
                invItemList[secondFromInv] = invArmorItem;

                invItem.PrimaryAction(GetComponent<PowerupManager>());
            }
            else if (invItem.GetItemType() == InventoryItem.ItemType.Chestplate && firstFromArmorInv == 1)
            {
                invItemArmorList[firstFromArmorInv] = invItem;
                invItemList[secondFromInv] = invArmorItem;

                invItem.PrimaryAction(GetComponent<PowerupManager>());
            }
            else if (invItem.GetItemType() == InventoryItem.ItemType.Leggings && firstFromArmorInv == 2)
            {
                invItemArmorList[firstFromArmorInv] = invItem;
                invItemList[secondFromInv] = invArmorItem;

                invItem.PrimaryAction(GetComponent<PowerupManager>());
            }
        }
        // Attempting to remove armor and place in an empty slot
        else if (!isInvArmorItemEmpty && isInvItemEmpty)
        {
            invItemArmorList[firstFromArmorInv] = invItem;
            invItemList[secondFromInv] = invArmorItem;

            invArmorItem.SecondaryAction(GetComponent<PowerupManager>());
        }
        // Attempting to remove armor and add something else to the armor slot
        else
        {
            // Replace the helmet with a different helmet
            if (invItem.GetItemType() == InventoryItem.ItemType.Helmet && firstFromArmorInv == 0)
            {
                invItemArmorList[firstFromArmorInv] = invItem;
                invItemList[secondFromInv] = invArmorItem;

                invArmorItem.SecondaryAction(GetComponent<PowerupManager>());
                invItem.PrimaryAction(GetComponent<PowerupManager>());
            }
            // Replace chestplate with different chestplate
            else if (invItem.GetItemType() == InventoryItem.ItemType.Chestplate && firstFromArmorInv == 1)
            {
                invItemArmorList[firstFromArmorInv] = invItem;
                invItemList[secondFromInv] = invArmorItem;

                invArmorItem.SecondaryAction(GetComponent<PowerupManager>());
                invItem.PrimaryAction(GetComponent<PowerupManager>());
            }
            // Replace leggings with different leggings
            else if (invItem.GetItemType() == InventoryItem.ItemType.Leggings && firstFromArmorInv == 2)
            {
                invItemArmorList[firstFromArmorInv] = invItem;
                invItemList[secondFromInv] = invArmorItem;

                invArmorItem.SecondaryAction(GetComponent<PowerupManager>());
                invItem.PrimaryAction(GetComponent<PowerupManager>());
            }
        }

        RefreshInventoryVisuals();
    }

    #endregion SwapInvItems

    #region CombineStacks
    // These will always be of the same item
    public bool CombineStacks(InventoryItem firstInvItem, InventoryItem secondInvItem, int secondInvItemIndex, bool isSecondInvHandItem)
    {
        int firstInvItemCount = firstInvItem.GetItemCount();
        int secondInvItemCount = secondInvItem.GetItemCount();
        int maxItemCount = firstInvItem.GetMaxStackSize();
        int newAmount = firstInvItemCount + secondInvItemCount;

        // Can't combine stacks if 1 or both are already at capacity
        if (firstInvItemCount == maxItemCount || secondInvItemCount == maxItemCount)
        {
            return false;
        }
        // Adding this will exceed max stack size, will give any remainder back to second inv item
        else if (newAmount > maxItemCount)
        {
            secondInvItemCount = newAmount - maxItemCount;

            firstInvItem.SetItemCount(maxItemCount);
            secondInvItem.SetItemCount(secondInvItemCount);
            return true;
        }
        // Stacks can combine fully into 1 stack
        else
        {
            firstInvItem.SetItemCount(newAmount);

            // Remove second item from the player's inventory
            RemoveFromInventory(secondInvItemIndex, isSecondInvHandItem, false);

            return true;
        }
    }

    #endregion CombineStacks

    #region SelectHotbarSlot
    public void MoveSelectedInvHandSlotRight()
    {
        selectedInvHandSlot++;

        if (selectedInvHandSlot > invHandSlotsUI.Count - 1)
        {
            selectedInvHandSlot = 0;
        }
    }

    public void MoveSelectedInvHandSlotLeft()
    {
        selectedInvHandSlot--;

        if (selectedInvHandSlot < 0)
        {
            selectedInvHandSlot = invHandSlotsUI.Count - 1;
        }
    }

    #endregion SelectHotbarSlot
}