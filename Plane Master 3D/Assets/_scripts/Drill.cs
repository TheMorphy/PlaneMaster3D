using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drill : MonoBehaviour
{

    [SerializeField] int timeToDrop, maxDrop;
    [SerializeField]
    StashZone stashZone;
    [SerializeField]
    GameObject ironItemPrefab;
    [SerializeField]
    List<Transform> drillIronStickSpots = new List<Transform>();
    Item itemToAdd;
    [SerializeField]
    Transform animParent;

    //[SerializeField] GameObject itemDrop;

    int currentDrop;

    public int CurrentDrop { get => currentDrop; set => currentDrop = value; }

    void Start()
    {
       // StartCoroutine(WaitToDrop());
    }
    private void OnEnable()
    {
        QuestSystem.instance.AddProgress("Build a drill", 1);
    }
    IEnumerator WaitToDrop()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeToDrop);
            if(stashZone.currentStashCount < stashZone.capacity)
            {
                stashZone.AddItem(Instantiate(ironItemPrefab, RandomSpawnPos().position, Quaternion.identity).GetComponent<Item>());
            }
        }
    }
    public void CatchIron()
    {
        if (stashZone.currentStashCount < stashZone.capacity)
        {
            Transform rnd = RandomSpawnPos();
            itemToAdd = Instantiate(ironItemPrefab, rnd.position, rnd.rotation, animParent).GetComponent<Item>();
        }
            
    }

    public void ReleaseIron()
    {
        if (stashZone.currentStashCount < stashZone.capacity)
        {
            stashZone.AddItem(itemToAdd);
        }
    }

    Transform RandomSpawnPos()
    {
        int i = Random.Range(0, drillIronStickSpots.Count - 1);
        return drillIronStickSpots[i];
    }
}
