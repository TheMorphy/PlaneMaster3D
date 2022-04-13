using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformAdd : MonoBehaviour
{
    public ItemData referenceItem;
    private StorageSystem storageSystem;

    [SerializeField] Drill drill;

    private int i;

    private void Start()
    {
        storageSystem = FindObjectOfType<StorageSystem>();
        drill = FindObjectOfType<Drill>();
    }

    public void OnHandleHoverPlatform()
    {
        for (i = 0; i < drill.CurrentDrop; i++)
        {
            storageSystem.Add(referenceItem);
            drill.CurrentDrop -= 1;
        }
    }
}
