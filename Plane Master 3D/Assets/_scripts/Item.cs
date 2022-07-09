using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public ItemType itemType;
    public float height, width = 0.23f;
    //[HideInInspector]
    public bool pickedUp;
    //[HideInInspector]
    public Vector3 destination;
	[HideInInspector] public Coroutine lerpCoroutine;
    public int amount = 1;

    public Item(Item item)
	{
        itemName = item.itemName;
        itemType = item.itemType;
        height = item.height;
        pickedUp = item.pickedUp;
        destination = item.destination;
        lerpCoroutine = item.lerpCoroutine;
        amount = item.amount;
	}
}
