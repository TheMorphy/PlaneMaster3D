using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drill : MonoBehaviour
{


	[SerializeField] float drillSpeed;
    [SerializeField]
    StashZone stashZone;
    [SerializeField]
    GameObject ironItemPrefab;
    [SerializeField]
    List<Transform> drillIronStickSpots = new List<Transform>();
    Item itemToAdd;
    [SerializeField]
    Transform animParent;
    [SerializeField] Build drillBuild;
	//[SerializeField]
	//AudioSource soundSource;

    //[SerializeField] GameObject itemDrop;

    int currentDrop;

    public int CurrentDrop { get => currentDrop; set => currentDrop = value; }




	void OnLevelChanged()
	{
		drillSpeed = drillBuild.speed;
		Animator anim = GetComponent<Animator>();
		anim.speed = drillSpeed;
		stashZone.capacity = drillBuild.storage;
		stashZone.SendMessage("GenerateSortingSystem");
	}


	void OnLoadLevels()
	{

        if (PlayerPrefs.GetInt(drillBuild.savingKey + "p") == 2)
            QuestSystem.instance.AddProgress("Research drill upgrade", 1);

        if (drillBuild.level > 1)
            QuestSystem.instance.AddProgress("Upgrade drill", 1);
	}

    private void Start()
    {

		

		string drillName = drillBuild.savingKey;
		if (drillName == "Drill")
		{
			QuestSystem.instance.AddProgress("Build Iron Drill", 1);
		}
		else if (drillName == "Drill02")
		{
			QuestSystem.instance.AddProgress("Build copper drill", 1);
		}
	}
    IEnumerator WaitToDrop()
    {
        while (true)
        {
            yield return new WaitForSeconds(drillSpeed);
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
