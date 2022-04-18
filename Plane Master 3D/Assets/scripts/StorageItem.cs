using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class StorageItem
{
    public ItemData data;
    public int stackSize;

    public StorageItem(ItemData source)
    {
        data = source;
        AddToStack();
    }

    public void AddToStack()
    {
        stackSize++;
    }

    public void RemoveFromStack()
    {
        stackSize--;
    }
}
