using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropInvItem : DragAndDrop
{
    #region Variables
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject invUI;

    private PlayerInventory playerInventory;
    private CanvasGroup canvasGroup;

    #endregion Variables

    #region MonoBehaviours
    public override void Awake()
    {
        base.Awake();
        playerInventory = player.GetComponent<PlayerInventory>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    #endregion MonoBehaviours

    #region OnDragEvents
    public override void OnBeginDrag(PointerEventData eventData)
    {
        GameObject otherItem = eventData.pointerDrag;
        
        // Drop item to the lowest in the heirarchy so it appears on top of everything else
        int childCount = invUI.transform.childCount;

        otherItem.transform.SetSiblingIndex(childCount);

        canvasGroup.alpha = 0.8f;
        canvasGroup.blocksRaycasts = false;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        playerInventory.RefreshInventoryVisuals();
    }

    #endregion OnDragEvents

    #region OnDropEvent
    public override void OnDrop(PointerEventData eventData)
    {
        // Determine which InventoryItems need to swap
        GameObject otherItem = eventData.pointerDrag;
        int firstInvIndex = otherItem.GetComponent<IndexValue>().GetIndexValue();
        int secondInvIndex = GetComponent<IndexValue>().GetIndexValue();
        bool isFirstItemInvHand;
        bool isFirstItemInvArmor;
        bool isSecondItemInvHand;
        bool isSecondItemInvArmor;
        bool isCombineSuccessful = false;
        InventoryItem otherInvItem;
        InventoryItem thisInvItem;

        // Other InvItem is in invHandUI
        if (otherItem.GetComponent<IsInvHandItem>())
        {
            isFirstItemInvArmor = false;
            isFirstItemInvHand = true;
            otherInvItem = playerInventory.GetInvHandItemList()[firstInvIndex];
        }
        // Other InvItem is in invArmorUI
        else if (otherItem.GetComponent<IsInvArmorItem>())
        {
            isFirstItemInvHand = false;
            isFirstItemInvArmor = true;
            otherInvItem = playerInventory.GetInvItemArmorList()[firstInvIndex];
        }
        // Other InvItem is in invUI
        else
        {
            isFirstItemInvHand = false;
            isFirstItemInvArmor = false;
            otherInvItem = playerInventory.GetInvItemList()[firstInvIndex];
        }

        // If player is dragging around an empty invItem, don't do anything
        if (otherInvItem.GetItemType() == InventoryItem.ItemType.Empty)
        {
            return;
        }

        // This InvItem is in invHandUI
        if (GetComponent<IsInvHandItem>())
        {
            isSecondItemInvArmor = false;
            isSecondItemInvHand = true;
            thisInvItem = playerInventory.GetInvHandItemList()[secondInvIndex];
        }
        // This InvItem is in invArmorUI
        else if (GetComponent<IsInvArmorItem>())
        {
            isSecondItemInvHand = false;
            isSecondItemInvArmor = true;
            thisInvItem = playerInventory.GetInvItemArmorList()[secondInvIndex];
        }
        // This InvItem is in invUI
        else
        {
            isSecondItemInvHand = false;
            isSecondItemInvArmor = false;
            thisInvItem = playerInventory.GetInvItemList()[secondInvIndex];
        }

        // Check if items are the same
        if (thisInvItem.GetItem() == otherInvItem.GetItem())
        {
            // Attempt to combine this item into the other item's stack instead of swapping
            isCombineSuccessful = playerInventory.CombineStacks(thisInvItem, otherInvItem, firstInvIndex, isFirstItemInvHand);
        }

        // Don't swap items if they successfully combined stacks
        if (isCombineSuccessful)
        {
            return;
        }

        // Both items are in invHandUI
        if (isFirstItemInvHand && isSecondItemInvHand)
        {
            playerInventory.SwapTwoInvHandItems(firstInvIndex, secondInvIndex);
        }
        // Other item is in invUI, this item is in invHandUI
        else if (!isFirstItemInvHand && isSecondItemInvHand)
        {
            // Prevent resources from entering the invHand
            if (playerInventory.GetInvItemList()[firstInvIndex].GetItemType() == InventoryItem.ItemType.Resource)
            {
                return;
            }
            
            // Prevent armor from entering the invHand
            if (playerInventory.GetInvItemList()[firstInvIndex].GetItemType() == InventoryItem.ItemType.Helmet ||
                playerInventory.GetInvItemList()[firstInvIndex].GetItemType() == InventoryItem.ItemType.Chestplate ||
                playerInventory.GetInvItemList()[firstInvIndex].GetItemType() == InventoryItem.ItemType.Leggings)
            {
                return;
            }
            
            playerInventory.SwapInvItemWithHandItem(firstInvIndex, secondInvIndex);
        }
        // Other item is in invHandUI, this item is in invUI
        else if (isFirstItemInvHand && !isSecondItemInvHand)
        {
            // Prevent resources from entering the invHand
            if (playerInventory.GetInvItemList()[secondInvIndex].GetItemType() == InventoryItem.ItemType.Resource)
            {
                return;
            }
            
            // Prevent armor from entering the invHand
            if (playerInventory.GetInvItemList()[secondInvIndex].GetItemType() == InventoryItem.ItemType.Helmet ||
                playerInventory.GetInvItemList()[secondInvIndex].GetItemType() == InventoryItem.ItemType.Chestplate ||
                playerInventory.GetInvItemList()[secondInvIndex].GetItemType() == InventoryItem.ItemType.Leggings)
            {
                return;
            }

            playerInventory.SwapInvItemWithHandItem(secondInvIndex, firstInvIndex);
        }
        // Other item is in invArmorUI, this item is in invHandUI
        else if (isFirstItemInvArmor && isSecondItemInvHand)
        {
            // Armor shouldn't be in the hotbar
        }
        // Other item is in invHandUI, this item is in inveArmorUI
        else if (isFirstItemInvHand && isSecondItemInvArmor)
        {
            // Armor shouldn't be in the hotbar
        }
        // Other item is in invArmorUI, this item is in invUI
        else if (isFirstItemInvArmor && !isSecondItemInvArmor)
        {
            playerInventory.SwapArmorItemWithInvItem(firstInvIndex, secondInvIndex);
        }
        // Other item is in invUI, this item is in invArmorUI
        else if (!isFirstItemInvArmor && isSecondItemInvArmor)
        {
            playerInventory.SwapArmorItemWithInvItem(secondInvIndex, firstInvIndex);
        }
        // Both items are in invArmorUI
        else if (isFirstItemInvArmor && isSecondItemInvArmor)
        {
            // Do nothing because this would mean swapping helmet in helmet slot with chestplate in chestplate slot
        }
        // Both items are in invUI
        else
        {
            playerInventory.SwapTwoInvItems(firstInvIndex, secondInvIndex);
        }
    }

    #endregion OnDropEvent
}