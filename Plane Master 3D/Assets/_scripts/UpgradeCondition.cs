using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class UpgradeCondition
{
    public string name;
    public ItemType itemType;
    public int count = 0;
    public int countNeeded = 10;
    public bool completed = false;
    public Transform itemDestination;
    public TextMeshPro text;
    public int level;

}
