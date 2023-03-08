using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvItemManager : MonoBehaviour
{
    public static InvItemManager instance { get; private set; }

    [SerializeField] private List<GameObject> invItemPrefabs;

    private void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject GetPrefabForInvItem(InventoryItem invItem)
    {
        foreach (GameObject obj in invItemPrefabs)
        {
            if (invItem.GetItem() == obj.GetComponent<InventoryItem>().GetItem())
            {
                return obj;
            }
        }

        return null;
    }
}