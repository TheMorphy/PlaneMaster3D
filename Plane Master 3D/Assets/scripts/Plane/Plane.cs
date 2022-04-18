using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Plane", menuName = "Plane")]
public class Plane : ScriptableObject
{
    [Space]
    [Header("Plane Repair Data")]
    public string planeName;
    public int requiredCogs;
    public int requiredIron;

    [Space]
    [Header("Plane Parts")]
    public GameObject mainPart;
    public GameObject[] planeParts;

}
