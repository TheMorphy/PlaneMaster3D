using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Drill : MonoBehaviour
{

    [SerializeField] int timeToDrop, maxDrop, currentDrop;

    [SerializeField] TextMeshPro dropText;

    //[SerializeField] GameObject itemDrop;

    //int currentDrop;

    public int CurrentDrop { get => currentDrop; set => currentDrop = value; }

    void Start()
    {
        StartCoroutine(WaitToDrop());
    }

    private void Update()
    {
        dropText.text = currentDrop.ToString() + "/" + maxDrop.ToString();
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
                //dropText.text = currentDrop.ToString() + "/" + maxDrop.ToString();
            }
            print(CurrentDrop);
        }
    }
}
