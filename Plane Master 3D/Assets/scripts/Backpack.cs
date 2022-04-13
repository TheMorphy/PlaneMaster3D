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
    float stackOffset, pickupRadius;
    [SerializeField]
    List<Item> items = new List<Item>();
    [SerializeField]
    Transform itemParent;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ItemHandler());
    }

    // Update is called once per frame
    void Update()
    {
        

        //Check For Item to pick up
        foreach(Collider c in Physics.OverlapSphere(transform.position, pickupRadius))
        {
            if(c.GetComponent<Item>() != null && items.Count < 50)
            {
                Item i = c.GetComponent<Item>();

                if(!i.pickedUp)
                {
                    i.pickedUp = true;
                    items.Add(i);
                    c.isTrigger = true;
                }
            }
        }
        //print(Mathf.CeilToInt(1 / 10));
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
            print(Mathf.CeilToInt(iterations));
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
