using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public ItemType itemType;
    public float height;
    [HideInInspector]
    public bool pickedUp;
    [HideInInspector]
    public Vector3 destination;
	[HideInInspector] public Coroutine lerpCoroutine;
}
