using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlataformAdd : MonoBehaviour
{
    //[SerializeField] ItemData referenceItem;
    [SerializeField] int timeToDrop, maxDrop, currentDrop;
    [SerializeField] TextMeshPro dropText;
    [SerializeField] Vector3 limitations, space;
    [SerializeField] public List<Item> items = new List<Item>();
    [SerializeField] Transform visualStart, spawn;

    [SerializeField] GameObject dropPrefab;

    //private StorageSystem storageSystem;
    //private int i;

    private void Start()
    {
        //storageSystem = FindObjectOfType<StorageSystem>();

        StartCoroutine(WaitToDrop());
    }

    private void Update()
    {
        if (items.Count > 0)
        {
            SortItems();
        }
    }

    /*public void OnHandleHoverPlatform()
    {
        for (i = 0; i < currentDrop; i++)
        {
            storageSystem.Add(referenceItem);
            currentDrop -= 1;
            dropText.text = currentDrop.ToString() + "/" + maxDrop.ToString();
        }
    }*/
    IEnumerator WaitToDrop()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeToDrop);
            if (currentDrop < maxDrop)
            {
                currentDrop += 1;
                dropText.text = currentDrop.ToString() + "/" + maxDrop.ToString();
                Instantiate(dropPrefab, spawn);
                items.Add(dropPrefab.GetComponent<Item>());
            }
            //print(currentDrop);
        }
    }

    void SortItems()
    {
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

    private void OnDrawGizmos()
    {
        Vector3 cubeSpace = new Vector3(space.x, space.y, space.z);

        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position, cubeSpace);
    }
}
