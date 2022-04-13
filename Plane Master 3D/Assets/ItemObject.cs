using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public ItemData referenceItem;
    private StorageSystem storageSystem;

    private void Start()
    {
        storageSystem = FindObjectOfType<StorageSystem>();
    }

    public void OnHandlePickupItem()
    {
        storageSystem.Add(referenceItem);
        Destroy(gameObject);
    }
}
