using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    [SerializeField]
    bool debug = false;

    Vector3[] positions = new Vector3[10];
    List<int> emptySlots = new List<int>();

    private void Start()
    {
        if(showDroppedItems)
        {
            
            Array.Resize(ref positions, conditions[0].countNeeded);
            GenerateSortingSystem();
        }
        StartCoroutine(WaitForComplete());
    }
    private void Update()
    {
        if(debug)
        {
            GenerateSortingSystem();
        }
    }

    IEnumerator WaitForComplete()
    {
        while (false == false)
        {
            yield return new WaitUntil(() => AllDone());
            allConditionsComplete = true;
            SendMessage("OnAllConditionsComplete", SendMessageOptions.DontRequireReceiver);
            yield return new WaitUntil(() => !AllDone());
            allConditionsComplete = false;
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
            else
                u.completed = false;
            if(!u.completed)
            {
                output = false;
            }
            
            u.text.text = u.count + "/" + u.countNeeded;
        }
        return output;
    }

    public void AddItem(Item item, bool translateItem = true)
    {
        print("adding ITem");
        items.Add(item);
        if (translateItem)
        {
            item.transform.parent = visualStart;

            SetItemDestination(items.Count - 1);

        }
        SendMessage("OnAddItem", SendMessageOptions.DontRequireReceiver);
    }
    void SetItemDestination(int item)
    {
        items[item].destination = positions[item];
        Coroutine c = StartCoroutine(LerpItemToDestination(item));
    }
    IEnumerator LerpItemToDestination(int i)
    {
        while (items[i].transform.localPosition != items[i].destination)
        {
            items[i].transform.localPosition = Vector3.Lerp(items[i].transform.localPosition, items[i].destination, 0.1f);
            items[i].transform.localRotation = Quaternion.Lerp(items[i].transform.localRotation, Quaternion.Euler(Vector3.zero), 0.1f);
            yield return new WaitForEndOfFrame();
        }
    }
    void GenerateSortingSystem()
    {
        Vector3 nextPos = Vector3.zero;
        int count = 0;
        for (int y = (int)limitations.y; y > 0; y--)
        {
            for (int x = (int)limitations.x; x > 0; x--)
            {
                for (int z = (int)limitations.z; z > 0; z--)
                {
                    positions[count] = nextPos;
                    nextPos.z += space.z;
                    count++;
                    if (count >= conditions[0].countNeeded)
                    {
                        return;
                    }
                }
                nextPos.x += space.x;
                nextPos.z = 0;
            }
            nextPos.x = 0;
            nextPos.y += space.y;
        }
    }


    void SortItems()
    {
        print("sorting");
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
                    if (count >= conditions[0].countNeeded)
                        return;
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
