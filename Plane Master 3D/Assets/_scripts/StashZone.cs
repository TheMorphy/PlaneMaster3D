using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StashZone : MonoBehaviour
{
    public int currentStashCount;
    [SerializeField]
    public int capacity;

    [SerializeField]
    List<Item> items = new List<Item>();
    [SerializeField]
    Vector3 limitations, space;
    [SerializeField]
    Transform visualStart;
    [SerializeField]
    bool debug;

    [SerializeField]
    Vector3[] positions = new Vector3[10];
    List<int> emptySlots = new List<int>();

    void Start()
    {
        
        for(int i = 0; i < capacity; i++)
        {
            //emptySlots.Add(i);
        }
        //StartCoroutine(WaitForItemPickup());
        GenerateSortingSystem();
    }


    private void Update()
    {
        if(debug)
        {
            GenerateSortingSystem();
            SortItems();
        }
        //SortItems();
        int x = 0;
        foreach(Item i in items)
        {
            if (i != null)
                x++;
        }
        currentStashCount = x;
    }
    public void AddItem(Item item)
    {
        if(items.Count < capacity)
        {
            items.Add(item);
            item.transform.parent = visualStart;
            SetItemDestination(items.Count - 1);
        }
        else if(ClosestNullItem() >= 0)
        {
            item.transform.parent = visualStart;
            int i = ClosestNullItem();
            items[i] = item;
            SetItemDestination(i);
        }
        
        
        //emptySlots.RemoveAt(items.Count - 1);
        
    }
    int ClosestNullItem()
    {
        for(int i = 0; i < items.Count; i++)
        {
            if(items[i] == null)
            {
                return i;
            }
        }
        return -1;
    }

    IEnumerator WaitForItemPickup()
    {
        while(true)
        {
            yield return new WaitUntil(() => ItemPickedUp());
            for (int i = 0; i < items.Count; i++)
            {
                if(items[i] != null)
                {
                    if (items[i].pickedUp)
                    {
                        items[i] = null;
                    }
                }
            }
            yield return null;
        }
    }

	void RemoveItem()
	{
		for(int i = 0; i < items.Count; i++)
		{
			if(items[i] != null && items[i].pickedUp)
			{
				items[i] = null;
			}
		}
	}

    bool ItemPickedUp()
    {
        foreach(Item i in items)
        {
            if(i != null)
            {
                if (i.pickedUp)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void SetItemDestination(int item)
    {
        items[item].destination = positions[item];
        StartCoroutine(LerpItemToDestination(items[item]));
    }

    IEnumerator LerpItemToDestination(Item item)
    {
        Item i = item;
        while(i.transform.localPosition != i.destination && !i.pickedUp)
        {
            i.transform.localPosition = Vector3.Lerp(i.transform.localPosition, i.destination, 0.1f);
            i.transform.localRotation = Quaternion.Lerp(i.transform.localRotation, Quaternion.Euler(Vector3.zero), 0.1f);
            yield return new WaitForEndOfFrame();
            if (i == null)
                yield break;
        }
    }

    void GenerateSortingSystem()
    {
        Array.Resize(ref positions, capacity);
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
                    if (count >= capacity)
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
        print("sort");
        Vector3 nextPos = Vector3.zero;
        int count = 0;
        for (int y = (int)limitations.y; y > 0; y--)
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
