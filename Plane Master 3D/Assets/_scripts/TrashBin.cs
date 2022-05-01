using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBin : MonoBehaviour
{
    public ItemData referenceItem;
    private StorageSystem storageSystem;

    private void Start()
    {
        storageSystem = FindObjectOfType<StorageSystem>();
    }

    public void OnHandleTrashBin()
    {
        storageSystem.Remove(referenceItem);
    }
}
