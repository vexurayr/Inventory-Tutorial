using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    public enum Item
    {
        None,
        Wood,
        Stone,
        Water,
        Berry,
        Bandage,
        Stamina_Boost,
        Cloth_Bandana,
        Wood_Chestplate,
        Wood_Leggings
    }

    public enum ItemType
    {
        Empty,
        Resource,
        Consumable,
        Equipment,
        Tool,
        Weapon,
        Helmet,
        Chestplate,
        Leggings
    }

    [SerializeField] protected Item item;
    [SerializeField] protected ItemType itemType;
    [SerializeField] protected Sprite itemSprite;

    // Used to manage same objects in inventory
    [SerializeField] protected int itemCount;
    [SerializeField] protected int maxStackSize;

    public virtual bool PrimaryAction(PowerupManager powerupManager)
    { 
        return false;
    }

    public virtual void PrimaryAction()
    { }

    public virtual bool SecondaryAction(PowerupManager powerupManager)
    {
        return false;
    }

    public virtual void SecondaryAction()
    { }

    public void PickItemUp(Inventory targetInv)
    {
        targetInv.AddToInventory(this.gameObject);
    }

    public Item GetItem()
    {
        return item;
    }

    public void SetItem(Item newItem)
    {
        item = newItem;
    }

    public ItemType GetItemType()
    {
        return itemType;
    }

    public void SetItemType(ItemType newType)
    {
        itemType = newType;
    }

    public int GetItemCount()
    {
        return itemCount;
    }

    public void SetItemCount(int num)
    {
        itemCount = num;
    }

    public int GetMaxStackSize()
    {
        return maxStackSize;
    }

    public void SetMaxStackSize(int num)
    {
        maxStackSize = num;
    }

    public Sprite GetItemSprite()
    {
        return itemSprite;
    }

    public void SetItemSprite(Sprite newSprite)
    {
        itemSprite = newSprite;
    }
}