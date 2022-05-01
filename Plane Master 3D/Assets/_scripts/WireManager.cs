using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WireManager : MonoBehaviour
{

    [SerializeField] Image[] wireGroup;
    [SerializeField] string[] wireColor;
    [SerializeField] List<int> usedValues = new List<int>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            RandomizeWires();
        }
    }

    void RandomizeWires()
    {
        for (int i = 0; i < wireColor[i].Length; i++)
        {
            UniqueRandomInt(0, 4);

            wireGroup[i].gameObject.name = wireColor[i];

            //Give Color
            if (wireGroup[i].gameObject.name == "red")
            {
                wireGroup[i].color = Color.red;
            }
            else if (wireGroup[i].gameObject.name == "blue")
            {
                wireGroup[i].color = Color.blue;
            }
            else if (wireGroup[i].gameObject.name == "purple")
            {
                wireGroup[i].color = Color.magenta;
            }
            else if (wireGroup[i].gameObject.name == "yellow")
            {
                wireGroup[i].color = Color.yellow;
            }
        }
    }
    public int UniqueRandomInt(int min, int max)
    {
        int val = Random.Range(min, max);
        usedValues.Add(val);
        while (usedValues.Contains(val))
        {
            val = Random.Range(min, max);
        }
        return val;
    }

}
