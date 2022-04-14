using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppingZone : MonoBehaviour
{

    [SerializeField]
    public List<UpgradeCondition> conditions = new List<UpgradeCondition>();
    public bool allConditionsComplete;
    [SerializeField]
    public bool showDroppedItems;
    [SerializeField]
    Vector3 limitations, space;
    [SerializeField]
    public List<Item> items = new List<Item>();
    [SerializeField]
    Transform visualStart;

    private void Start()
    {
        StartCoroutine(WaitForComplete());
    }

    IEnumerator WaitForComplete()
    {
        while ( false == false)
        {
            yield return new WaitUntil(() => AllDone());
            allConditionsComplete = true;
            yield return new WaitUntil(() => !AllDone());
            yield return null;
        }
    }

    bool AllDone()
    {
        bool output = true;
        foreach(UpgradeCondition u in conditions)
        {
            if (u.count >= u.countNeeded)
                u.completed = true;
            if(!u.completed)
            {
                output = false;
            }
            
            u.text.text = u.count + "/" + u.countNeeded;
        }
        return output;
    }

    private void Update()
    {
        if(!itemsAllSet() && showDroppedItems && items.Count > 0)
        {
            SortItems();
        }
    }
    bool itemsAllSet()
    {
        bool output = true;
        foreach(Item i in items)
        {
            if(i.transform.position != i.destination)
            {
                output = false;
            }
        }
        return output;
    }

    void SortItems()
    {
        Vector3 nextPos = Vector3.zero;
        int count = 0;
        for(int y = (int)limitations.y; y > 0; y--)
        {
            for (int x = (int)limitations.x; x > 0; x--)
            {
                for (int z = (int)limitations.z; z > 0; z--)
                {
                    Item i = items[count];
                    count++;
                    i.destination = nextPos;
                    i.transform.parent = visualStart;
                    if (i.transform.localPosition != i.destination)
                    {
                        i.transform.localPosition = Vector3.Lerp(i.transform.localPosition, i.destination, 0.1f);
                        i.transform.localRotation = Quaternion.Lerp(i.transform.localRotation, Quaternion.Euler(Vector3.zero), 0.1f);
                    }
                    nextPos.z += space.z;
                }
                nextPos.x += space.x;
                nextPos.z = 0;
                
            }
            nextPos.x = 0;
            nextPos.y += space.y;
        }
    }
}
