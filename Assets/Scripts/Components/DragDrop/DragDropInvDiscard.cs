using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropInvDiscard : DragAndDrop
{
    #region Variables
    [SerializeField] private GameObject player;

    private PlayerInventory playerInventory;
    private List<InventoryItem> inventoryItems;
    private List<InventoryItem> inventoryHandItems;
    private List<InventoryItem> inventoryArmorItems;

    #endregion Variables

    #region MonoBehaviours
    public override void Awake()
    {
        base.Awake();
        playerInventory = player.GetComponent<PlayerInventory>();
        inventoryItems = playerInventory.GetInvItemList();
        inventoryHandItems = playerInventory.GetInvHandItemList();
        inventoryArmorItems = playerInventory.GetInvItemArmorList();
    }

    #endregion MonoBehaviours

    #region OnDropEvents
    public override void OnDrop(PointerEventData eventData)
    {
        // Get index of InvItem to discard aka the object being dragged by the mouse
        GameObject otherItem = eventData.pointerDrag;
        int index = otherItem.GetComponent<IndexValue>().GetIndexValue();

        // Determine if InvItem comes from the inventory, the hotbar, or an armor slot
        bool isInvHandItem;
        bool isInvArmorItem;

        if (otherItem.GetComponent<IsInvHandItem>())
        {
            isInvHandItem = true;
            isInvArmorItem = false;
        }
        else if (otherItem.GetComponent<IsInvArmorItem>())
        {
            isInvHandItem = false;
            isInvArmorItem = true;
        }
        else
        {
            isInvHandItem = false;
            isInvArmorItem = false;
        }

        // Get InvItem from playerInventory using index
        InventoryItem invItem;

        if (isInvHandItem)
        {
            invItem = inventoryHandItems[index];
        }
        else if (isInvArmorItem)
        {
            invItem = inventoryArmorItems[index];

            // Remove effect from player since the discarded item is armor straight from an invArmorItem slot
            invItem.SecondaryAction(playerInventory.GetComponent<PowerupManager>());
        }
        else
        {
            invItem = inventoryItems[index];
        }

        // Get InvItem Prefab from instance of InvItemManager using InvItem
        GameObject itemToDiscard = InvItemManager.instance.GetPrefabForInvItem(invItem);

        // Set item count of this prefab's InventoryItem component to match the item count from the InvItem
        // Everything else will already match
        itemToDiscard.GetComponent<InventoryItem>().SetItemCount(invItem.GetItemCount());

        CheckForValidSpawnPoint(itemToDiscard, index, isInvHandItem, isInvArmorItem, 0, 0);
    }

    #endregion OnDropEvents

    #region HelperFunctions
    public Bounds GetMaxBounds(GameObject parentObj)
    {
        Bounds totalColliderSize = new Bounds(parentObj.transform.position, Vector3.zero);

        foreach (var child in parentObj.GetComponentsInChildren<Collider>())
        {
            totalColliderSize.Encapsulate(child.bounds);
        }

        return totalColliderSize;
    }

    public void CheckForValidSpawnPoint(GameObject itemToDiscard, int index, bool isInvHandItem, bool isInvArmorItem, float prevHeight, float combinedHeight)
    {
        // Don't want to spawn/remove an empty item
        if (itemToDiscard.GetComponent<InventoryItem>().GetItem() == InventoryItem.Item.None)
        {
            return;
        }

        float sphereRadius = 0.1f;

        Vector3 spawnLocation = player.gameObject.transform.position +
                (player.gameObject.transform.forward * player.GetComponent<PlayerController>().GetPickupMinDropDistance());
        spawnLocation.y = combinedHeight;

        // Use Physics.OverlapShere to check if there is an inventory item in the way
        Collider[] hitColliders = Physics.OverlapSphere(spawnLocation, sphereRadius);
        Collider obstacle = new Collider();

        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.GetComponent<InventoryItem>())
            {
                if (obstacle == null)
                {
                    obstacle = collider;
                }
                // Get the collider of the heighest object
                else if (collider.transform.position.y > obstacle.transform.position.y)
                {
                    obstacle = collider;
                }
            }
        }

        if (obstacle != null)
        {
            // Take into account height of object
            var maxBounds = GetMaxBounds(obstacle.gameObject);
            prevHeight = maxBounds.size.y;
            spawnLocation.y += maxBounds.size.y;

            CheckForValidSpawnPoint(itemToDiscard, index, isInvHandItem, isInvArmorItem, prevHeight, spawnLocation.y);
        }
        else
        {
            spawnLocation.y = combinedHeight - prevHeight;

            // Spawn itemToDiscard in front of the player at valid location
            Instantiate(itemToDiscard, spawnLocation, player.gameObject.transform.rotation);

            // Remove InvItem from playerInventory's list of InvItems using index
            playerInventory.RemoveFromInventory(index, isInvHandItem, isInvArmorItem);

            playerInventory.RefreshInventoryVisuals();
        }
    }

    #endregion HelperFunctions
}