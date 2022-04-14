using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backpack : MonoBehaviour
{
    [SerializeField]
    int backpackSize = 50;
    [SerializeField]
    int stackSize = 10;
    [SerializeField]
    float stackOffset, pickupRadius, itemDropInterval;
    [SerializeField]
    List<Item> items = new List<Item>();
    [SerializeField]
    Transform itemParent;
    [SerializeField]
    Player player;

    //private variables
    float dropTime;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ItemHandler());
    }

    // Update is called once per frame
    void Update()
    {
        
            dropTime -= Time.deltaTime;
        //Check For Item to pick up
        foreach (Collider c in Physics.OverlapSphere(transform.position, pickupRadius))
        {
            if(c.GetComponent<Item>() != null && items.Count < backpackSize)
            {
                Item i = c.GetComponent<Item>();

                if(!i.pickedUp)
                {
                    i.pickedUp = true;
                    items.Add(i);
                    c.isTrigger = true;
                }
            }

            
            if(c.gameObject.layer == 7 && !player.isMoving && dropTime <= 0)
            {
                print("stage1");
                DroppingZone droppingZone = c.GetComponent<DroppingZone>();
                foreach(UpgradeCondition u in droppingZone.conditions)
                { 
                    if(!u.completed && dropTime <= 0)
                    {
                        print("stage2");
                        if (CheckItems(u.name) >= 0)
                        {
                            print("stage3");
                            int itemToDrop = CheckItems(u.name);
                            Transform itemTransform = items[itemToDrop].transform;
                            droppingZone.items.Add(items[itemToDrop]);
                            items.RemoveAt(itemToDrop);
                            
                            if(!droppingZone.showDroppedItems)
                            {
                                itemTransform.parent = u.itemDestination;
                                StartCoroutine(LerpItemToDestination(itemTransform));
                            }
                            
                            u.count++;

                            dropTime = itemDropInterval;
                        }
                    }
                }
            }
        }
    }

    IEnumerator LerpItemToDestination(Transform item, bool destroy = true)
    {
        while(item.transform.localPosition != Vector3.zero)
        {
            print("lerping");
            item.transform.localPosition = Vector3.Lerp(item.transform.localPosition, Vector3.zero, 0.1f);
            yield return null;
        }
        if(destroy)
        Destroy(item.gameObject);
        
    }
    int CheckItems(string itemName)
    {
        int output = -1;
        for(int i = 0; i < items.Count; i++)
        {
            if(items[i].itemName == itemName)
            {
                output = i;
            }
        }
        return output;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }

    IEnumerator ItemHandler()
    {
        while(false == false)
        {
            float nextyOffset = 0;
            float iterations = (float)items.Count / stackSize;
            for (int i = 0; i < Mathf.CeilToInt(iterations); i++)
            {
                float nextItemOffset = 0;
                for (int a = i * stackSize; a < Mathf.Min(items.Count, stackSize * (i + 1)); a++)
                {
                    Vector3 destinatedPos = Vector3.up * nextItemOffset - Vector3.forward * nextyOffset;
                    if (items[a].transform.localPosition != destinatedPos)
                    {
                        items[a].transform.parent = itemParent;

                        items[a].transform.localPosition = Vector3.Lerp(items[a].transform.localPosition, destinatedPos, 0.1f);

                        items[a].transform.localRotation = Quaternion.Lerp(items[a].transform.localRotation, Quaternion.Euler(Vector3.zero), 0.1f);
                    }
                    nextItemOffset += items[a].height;
                }
                nextyOffset += stackOffset;
            }
            yield return null;
        }
    }

    
}
