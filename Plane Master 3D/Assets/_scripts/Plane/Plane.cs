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
    public int randomRepair;

    public string repairParts;

    public void RandomRepair()
    {
        randomRepair = Random.Range(1, 4);

        //Debug.Log(randomRepair);

        switch (randomRepair)
        {
            case 1:
                repairParts = "Front";
                break;
            case 2:
                repairParts = "Left";
                break;
            case 3:
                repairParts = "Right";
                break;
        }
    }
}
