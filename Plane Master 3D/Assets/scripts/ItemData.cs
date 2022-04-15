using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Storage Item Data")]
public class ItemData : ScriptableObject
{
    public string id;
    //public Sprite icon;
    public GameObject prefab;
}
