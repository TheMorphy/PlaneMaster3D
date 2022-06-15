using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public enum ItemType { Money, Iron, Cog , Copper , CopperCog, Circuit};

[System.Serializable]
public class ItemInfo
{
    public ItemType itemType;
    public TextMeshPro itemText;
    [HideInInspector]
    public int count, countNeeded;
	
}
public class DroppingZone : MonoBehaviour
{
	[SerializeField] bool isPlanePlatform;
    [SerializeField]
    public List<UpgradeCondition> conditions = new List<UpgradeCondition>();
    public bool allConditionsComplete;
    [SerializeField]
    public bool showDroppedItems, displayTextAsCountDown;
    [SerializeField]
    Vector3 limitations, space;
    [SerializeField]
    public List<Item> items = new List<Item>();
    [SerializeField]
    Transform visualStart;
    [SerializeField]
    bool debug = false;
	[SerializeField] public List<int> conditionNumbers = new List<int>();

	Vector3[] positions = new Vector3[10];
    List<int> emptySlots = new List<int>();

	PlaneMain planeScript;
	[SerializeField]
	SpriteRenderer progressbar;
	private float progressBarStartSize;

    [SerializeField]
    private List<ItemInfo> itemInfos = new List<ItemInfo>();

	private void Awake()
	{
		if (progressbar != null)
			progressBarStartSize = progressbar.size.y;
	}
	private void Start()
    {
		
        if(showDroppedItems)
        {
            
            Array.Resize(ref positions, conditions[0].countNeeded);
            GenerateSortingSystem();
        }
        else
        {
            LoadConditions();
        }
		//StartCoroutine(WaitForComplete());
		//CheckForDone();
		RefreshProgressbar(0, 1);
    }

    void LoadConditions()
    {
        foreach (UpgradeCondition c in conditions)
        {
            c.count = PlayerPrefs.GetInt(transform.position.magnitude + c.name + "count");
        }
    }

    public void SaveConditions()
    {
        foreach (UpgradeCondition c in conditions)
        {
            PlayerPrefs.SetInt(transform.position.magnitude + c.name + "count", c.count);
        }
        //CheckForDone();
    }
    private void Update()
    {
        if(debug)
        {
            GenerateSortingSystem();
        }

		
	}

	public void GetEachCondition()
	{
		if (isPlanePlatform)
		{
			planeScript = FindObjectOfType<PlaneMain>();

			int Lenght = planeScript.Lenght;

			conditionNumbers = new List<int>(new int[Lenght]);

			for (int i = 0; i < Lenght; i++)
			{
				int cNumb = planeScript.randomNumbers[i];
				conditionNumbers[i] = cNumb;
				print(cNumb);
				DroppingZone dz = planeScript.breakables[cNumb].GetComponent<DroppingZone>();
				dz.enabled = false;
				conditions.Add(dz.conditions[i]);
			}
		}
	}

	public void UpdateConditions()
	{
		if(planeScript != null && !planeScript.allIsRepaired)
		{
			if (conditions[0].completed == true)
			{
				// Once first condition is completed change everything to the next condition

				int numb = conditionNumbers[1];

				DroppingZone dz = planeScript.breakables[numb].GetComponent<DroppingZone>();
				conditions = dz.conditions;
			}
		}
	}

	public void ResetConditions()
    {
        foreach(UpgradeCondition c in conditions)
        {
            c.count = 0;
			c.completed = false;
        }
        SaveConditions();
		CheckForDone();
		
    }



    IEnumerator WaitForComplete()
    {
        yield return new WaitUntil(() => !AllDone() || !allConditionsComplete);
        while (false == false)
        {
            yield return new WaitUntil(() => AllDone());
            allConditionsComplete = true;
            //SendMessage("OnAllConditionsComplete", SendMessageOptions.DontRequireReceiver);
            yield return new WaitUntil(() => !AllDone());
            allConditionsComplete = false;
            yield return null;
        }
    }

    public void Refresh()
	{
        CheckForDone();
	}

    void CheckForDone()
	{
        foreach(ItemInfo i in itemInfos)
		{
            i.count = 0;
            i.countNeeded = 0;
		}

        bool allDone = true;
        for(int c = 0; c < conditions.Count; c++)
        {
            UpgradeCondition u = conditions[c];
            
            for(int i = 0; i < itemInfos.Count; i++)
			{
                
                if(u.itemType == itemInfos[i].itemType)
				{
                    itemInfos[i].count += u.count;
                    itemInfos[i].countNeeded += u.countNeeded;
                    break;
				}
			}




            
            if (u.count >= u.countNeeded)
			{
                if (!u.completed)
                {
                    u.completed = true;
                    SendMessage("OnConditionComplete", SendMessageOptions.DontRequireReceiver);
                }
            }
            else
                u.completed = false;

            if (u.completed == false)
                allDone = false;

            if (displayTextAsCountDown)
            {
                u.text.text = (u.countNeeded - u.count).ToString();
                if (u.completed)
                {
                    u.text.gameObject.SetActive(false);
                }
                else if (!u.text.gameObject.activeSelf)
                    u.text.gameObject.SetActive(true);

            }
            else if (u.text != null && isActiveAndEnabled)
            {
                u.text.text = u.count + "/" + u.countNeeded;
            }
        }
        //Set the text
        for(int i = 0; i < itemInfos.Count; i++)
		{
            if(itemInfos[i].itemText != null)
            itemInfos[i].itemText.text = itemInfos[i].count + "/" + itemInfos[i].countNeeded;
		}
		//Set the progress bar
		if(progressbar != null)
		{
			float count = 0;
			int countNeeded = 0;
			for (int i = 0; i < itemInfos.Count; i++)
			{
				count += itemInfos[i].count;
				countNeeded += itemInfos[i].countNeeded;
			}

			RefreshProgressbar(count, countNeeded);		
		}
		

		allConditionsComplete = allDone;
        if (allDone && this.isActiveAndEnabled)
            SendMessage("OnAllConditionsComplete", SendMessageOptions.DontRequireReceiver);
        
    }

	void RefreshProgressbar(float count, float countNeeded)
	{
		float progress = count / countNeeded;
		progressbar.size = new Vector2(progressbar.size.x, count == 0 || countNeeded == 0 ? 0 : progressBarStartSize * progress);

	}

	//This one is outdated 
	bool AllDone()
    {
		if (debug)
			print("droppingZone AllDone()");
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
            if(displayTextAsCountDown)
            {
                u.text.text = (u.countNeeded - u.count).ToString();
                if(u.completed)
                {
                    u.text.gameObject.SetActive(false);
                }
                else if(!u.text.gameObject.activeSelf)
                    u.text.gameObject.SetActive(true);

            }
            else if(u.text != null && isActiveAndEnabled)
            {
                u.text.text = u.count + "/" + u.countNeeded;
            }
        }
        return output;
    }

    

    public void AddItem(Item item, bool translateItem = true)
    {
        SaveConditions();
        items.Add(item);
        if (translateItem)
        {
            item.transform.parent = visualStart;

            SetItemDestination(items.Count - 1);

        }
        CheckForDone();
        SendMessage("OnAddItem", SendMessageOptions.DontRequireReceiver);
    }
    void SetItemDestination(int item)
    {
        items[item].destination = positions.Length > item ? positions[item] : positions[0];
        Coroutine c = StartCoroutine(LerpItemToDestination(item));
    }
    IEnumerator LerpItemToDestination(int i)
    {
        Item itemToLerp = items[i];
        Vector3 des = itemToLerp.destination;
            while (itemToLerp.transform.localPosition != itemToLerp.destination && itemToLerp.destination == des)
            {
                itemToLerp.transform.localPosition = Vector3.Lerp(itemToLerp.transform.localPosition, itemToLerp.destination, 0.1f);
                itemToLerp.transform.localRotation = Quaternion.Lerp(itemToLerp.transform.localRotation, Quaternion.Euler(Vector3.zero), 0.1f);
                yield return new WaitForEndOfFrame();
            if(itemToLerp == null)
            {
                yield break;
            }
                
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
