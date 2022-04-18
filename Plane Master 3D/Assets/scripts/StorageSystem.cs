using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageSystem : MonoBehaviour
{
    public static StorageSystem current;
    private Dictionary<ItemData, StorageItem> m_itemDictionary;
    public List<StorageItem> inventory;

    private int currentInventorySize, maxInventorySize;

    private void Awake()
    {
        current = this;
        inventory = new List<StorageItem>();
        m_itemDictionary = new Dictionary<ItemData, StorageItem>();
    }

    public StorageItem Get(ItemData referenceData)
    {
        if (m_itemDictionary.TryGetValue(referenceData, out StorageItem value))
        {
            return value;
        }
        return null;
    }

    public void Add(ItemData referenceData)
    {

        // If the item already exists it is added to the stack
        if (m_itemDictionary.TryGetValue(referenceData, out StorageItem value))
        {
            value.AddToStack();
        }
        else //If it doesn't exist it is created a new instance of the item and its added to the storage
        {
            StorageItem newItem = new StorageItem(referenceData);
            inventory.Add(newItem);
            m_itemDictionary.Add(referenceData, newItem);

            if (currentInventorySize >= maxInventorySize)
            {
                currentInventorySize = maxInventorySize;
            }
        }
    }

    public void Remove(ItemData referenceData)
    {

        // If there is more than one item it removes from the stack
        if (m_itemDictionary.TryGetValue(referenceData, out StorageItem value))
        {
            value.RemoveFromStack();

            // If there is no stack size it removes the stack completely
            if (value.stackSize == 0)
            {
                inventory.Remove(value);
                m_itemDictionary.Remove(referenceData);
            }
        }
    }
}
