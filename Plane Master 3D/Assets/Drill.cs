using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drill : MonoBehaviour
{

    [SerializeField] int timeToDrop, maxDrop;

    //[SerializeField] GameObject itemDrop;

    int currentDrop;

    public int CurrentDrop { get => currentDrop; set => currentDrop = value; }

    void Start()
    {
        StartCoroutine(WaitToDrop());
    }

    IEnumerator WaitToDrop()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeToDrop);
            //Instantiate(itemDrop);
            if (CurrentDrop < maxDrop)
            {
                CurrentDrop += 1;
            }
            else CurrentDrop += 0;
            print(CurrentDrop);
        }
    }
}
