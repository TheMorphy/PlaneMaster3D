using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Breakable
{
    public Transform obj, normalPosition, brokenPosition;
    [Range(0, 100)]
    public float neededRepairPercent;
    [Range(0, 1)]
    public float health;
    public Coroutine alignCoroutine;

}
