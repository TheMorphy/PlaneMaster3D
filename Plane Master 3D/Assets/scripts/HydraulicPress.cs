using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HydraulicPress : MonoBehaviour
{
    [SerializeField]
    DroppingZone droppingZone;
    [SerializeField]
    StashZone stashZone;
    [SerializeField] Transform cogsSpawnPos;

    [SerializeField] float cogsPerMinute;
    [SerializeField] GameObject outputItem;
    [SerializeField] Item currentMetal;
    Animator anim;
    

    // Update is called once per frame
    void Start()
    {
        StartCoroutine(HydraulicWorker());
        anim = GetComponent<Animator>();
    }

    IEnumerator HydraulicWorker()
    {
        while(false == false)
        {
            yield return new WaitUntil(() => droppingZone.items.Count > 0 && currentMetal == null && stashZone.currentStashCount < stashZone.capacity);
            currentMetal = droppingZone.items[droppingZone.items.Count - 1];
            droppingZone.items.RemoveAt(droppingZone.items.Count - 1);
            droppingZone.conditions[0].count -= 1;
            anim.speed = cogsPerMinute / 60;
            anim.SetBool("isProcessing", true);
            while(currentMetal != null)
            {
                currentMetal.transform.position = Vector3.Lerp(currentMetal.transform.position, cogsSpawnPos.position, 0.05f);
                yield return null;
            }
            yield return null;
        }
    }

    public void ConvertObject()
    {
        
        
        if(stashZone.currentStashCount < stashZone.capacity)
        {
            Destroy(currentMetal.gameObject);
            stashZone.AddItem(Instantiate(outputItem, cogsSpawnPos.position, Quaternion.identity).GetComponent<Item>());
        }
        if(droppingZone.items.Count == 0 || stashZone.currentStashCount >= stashZone.capacity - 1)
        {
            anim.SetBool("isProcessing", false);
        }
    }
}