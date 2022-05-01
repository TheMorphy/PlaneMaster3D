using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class Research
{
    public string name;
    public SpriteRenderer icon;
    public Build buildToUnlock;
    public bool completed;
    public List<UpgradeCondition> conditions = new List<UpgradeCondition>();
}
