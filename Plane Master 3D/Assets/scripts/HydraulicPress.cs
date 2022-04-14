using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HydraulicPress : MonoBehaviour
{
    [SerializeField]
    DroppingZone droppingZone;

    [SerializeField] float cogsPerMinute;
    [SerializeField] GameObject outputItem;
    [SerializeField] Item currentMetal;
    

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator HydraulicWorker()
    {
        while(false == false)
        {



            yield return null;
        }
    }

    public void ConvertObject()
    {
        Destroy(currentMetal.gameObject);
    }
}
