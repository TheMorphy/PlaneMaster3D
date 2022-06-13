using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

[System.Serializable]
public class UpgradeVariables
{
	[HideInInspector]
	public string savingKeyPrefix = "";
	[SerializeField]
	string savingKey;
	[Space(10)]

	public int level;
	[Space(6)]
	public float standard;
	public float increment;
	public bool incrementPercentage;
	[Header("Money")]
	public int costCurrent;
	public int costStandard;
	public float costIncrement;
	public bool costIncrementPercentage;

	[Header("UI Elements")]
	[SerializeField]
	TextMeshProUGUI textLevel;
	[SerializeField]
	TextMeshProUGUI textCost;
	public void refreshUI()
	{
		if(textLevel != null)
			textLevel.text = "LVL " + level.ToString();
		if (textCost != null)
			textCost.text = costCurrent.ToString();
	}

	public float Upgrade()
	{
		level++;
		costCurrent = costIncrementPercentage ? (int)(costStandard * Mathf.Pow(1 + costIncrement, level)) : (int)(costStandard + costIncrement * level);

		refreshUI();
		Save();
		return incrementPercentage ? standard * Mathf.Pow(1 + increment, level) : standard + increment * level;
	}
	public int UpgradeInt()
	{
		level++;
		costCurrent = costIncrementPercentage ? (int)(costStandard * Mathf.Pow(1 + costIncrement, level)) : (int)(costStandard + costIncrement * level);

		refreshUI();
		Save();
		return incrementPercentage ? (int)(standard * Mathf.Pow(1 + increment, level)) : (int)(standard + increment * level);
	}

	public float Load()
	{
		level = PlayerPrefs.GetInt(savingKeyPrefix + savingKey);
		costCurrent = costIncrementPercentage ? (int)(costStandard * Mathf.Pow(1 + costIncrement, level)) : (int)(costStandard + costIncrement * level);

		refreshUI();
		return incrementPercentage ? standard *Mathf.Pow(1 + increment, level) : standard + increment * level;
	}
	public int LoadInt()
	{
		level = PlayerPrefs.GetInt(savingKeyPrefix + savingKey);
		costCurrent = costIncrementPercentage ? (int)(costStandard * Mathf.Pow(1 + costIncrement, level)) : (int)(costStandard + costIncrement * level);
		refreshUI();
		return incrementPercentage ? (int)(standard * Mathf.Pow(1 + increment, level)) : (int)(standard + increment * level);
	}

	void Save()
	{
		PlayerPrefs.SetInt(savingKeyPrefix + savingKey, level);
	}
	
}

public class WorkerAI : MonoBehaviour
{
	public int level;
	#region other vars
	[SerializeField]
	string savingKey;
	[Space(10)]

	[SerializeField]
    public Backpack backpack;
    [SerializeField]
    List<WorkerLevel> levels = new List<WorkerLevel>();
    [SerializeField]
    WorkerLevel generationParameters;
    [SerializeField]
    WorkerLevel currentLevel;
    public enum TaskType { Courier , Scientist , Pilot}
    [SerializeField]
    TaskType task;
    [SerializeField]
    public Transform getItemPos, itemDestinationPos;
    [SerializeField]
    public ItemType itemToCarry;

    [SerializeField]
    float movementSpeed = 2, animSmooth, stayTime;
    float stayTimer;
    [SerializeField]
    Animator visual3D;

    public NavMeshAgent agent;
    public bool isMoving;
    [SerializeField]
    ResearchSystem researchSystem;

	[SerializeField]
	public int hireCost;
	[SerializeField]
	TextMeshProUGUI hireCostText;
	//[HideInInspector]
	public bool isHired;
#endregion
	[Header("Upgrades")]
	[SerializeField]
	public UpgradeVariables speed;

	[SerializeField]
	public UpgradeVariables storage;


	public bool stopped;

	bool readyToOpenUI;
	[SerializeField]
	Outline workersOutline;
	Coroutine outline;

    void OnEnable()
    {
		
        agent = GetComponent<NavMeshAgent>();
        switch (task)
        {
            case TaskType.Courier:
                StartCoroutine(CourierWorker());
				QuestSystem.instance.AddProgress("Hire a worker", 1);
				break;
            case TaskType.Scientist:
                StartCoroutine(ScientistWorker());
				QuestSystem.instance.AddProgress("Hire a scientist", 1);
				break;
        }
		//speed.savingKeyPrefix = savingKey;
		//storage.savingKeyPrefix = savingKey;

		//agent.speed = speed.Load();
		//backpack.stackSize = storage.LoadInt();

		//isHired = PlayerPrefs.GetInt(savingKey) == 1 ? true : false;
		//stopped = !isHired;

	}


	
	public void SpeedUpgrade()
	{
		if(LevelSystem.instance.playerBackpack.TryPay(speed.costCurrent, transform.position))
		{
			agent.speed = speed.Upgrade();
		}
	}

	public void StorageUpgrade()
	{
		if (LevelSystem.instance.playerBackpack.TryPay(storage.costCurrent, transform.position))
		{
			backpack.stackSize = storage.UpgradeInt();
		}
	}

	public void Hire()
	{
		if(LevelSystem.instance.playerBackpack.TryPay(hireCost, transform.position))
		{
			PlayerPrefs.SetInt(savingKey, 1);
			//LevelSystem.instance.CloseHiredUI();
			isHired = true;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		
	}
	#region triggerstuff
	/*
	private void OnTriggerStay(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			if (LevelSystem.instance.currentWorker == null)
			{
				if (!other.GetComponent<Player>().isMoving)
				{
					if(readyToOpenUI)
					{
						//Open UpgradeUI
						LevelSystem.instance.currentWorker = this;
						LevelSystem.instance.OpenWorkersUpgradeUI();
						hireCostText.text = hireCost.ToString();
						readyToOpenUI = false;
						if (outline != null)
							StopCoroutine(outline);
						outline = StartCoroutine(DeactivateOutline());
					}
					
				}
				else
				{
					readyToOpenUI = true;
					if (outline != null)
						StopCoroutine(outline);
					outline = StartCoroutine(ActivateOutline());
				}

			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			if (outline != null)
				StopCoroutine(outline);
			outline = StartCoroutine(DeactivateOutline());
		}
	}
	*/
	#endregion

	IEnumerator ActivateOutline()
	{
		float t = 0;
		workersOutline.enabled = true;
		while (t < 1)
		{
			t += Time.deltaTime * 3;
			workersOutline.OutlineColor = new Color(workersOutline.OutlineColor.r, workersOutline.OutlineColor.g, workersOutline.OutlineColor.b, Mathf.Lerp(workersOutline.OutlineColor.a, 0.43f, t));
			yield return null;
		}
	}

	IEnumerator DeactivateOutline()
	{
		float t = 0;
		while (t < 1)
		{
			t += Time.deltaTime * 3;
			workersOutline.OutlineColor = new Color(workersOutline.OutlineColor.r, workersOutline.OutlineColor.g, workersOutline.OutlineColor.b, Mathf.Lerp(workersOutline.OutlineColor.a, 0, t));
			yield return null;
		}
		workersOutline.enabled = false;
	}

	public void setLevel(int level)
    {
        if(levels.Count <= level)
        {
            WorkerLevel generatedLevel = new WorkerLevel();
            generatedLevel.backpackSize = levels[level - 1].backpackSize + generationParameters.backpackSize;
            generatedLevel.speed = levels[level - 1].speed + generationParameters.speed;
            levels.Add(generatedLevel);
        }
      //  backpack.backpackSize = levels[level].backpackSize;
      //  agent.speed = levels[level].speed;
        //currentLevel.backpackSize = levels[level].backpackSize;
        //currentLevel.speed = levels[level].speed;
    }

    void Update()
    {
		agent.isStopped = stopped;
        

        visual3D.SetFloat("Speed", isMoving && !stopped ? agent.speed : 0, animSmooth, Time.deltaTime);

    }

    private void FixedUpdate()
    {
        isMoving = agent.remainingDistance > agent.stoppingDistance;

    }

    IEnumerator ScientistWorker()
    {
        while(true)
        {
            if(!isMoving)
            {
                researchSystem.scientistAvailable = true;
            }
            else
            {
                researchSystem.scientistAvailable = false;

            }
            agent.SetDestination(itemDestinationPos.position);
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator CourierWorker()
    {
		yield return new WaitWhile(() => getItemPos == null);
        while(true)
        {
            if (isMoving)
            {
                stayTimer = stayTime;
            }
            else
            {
                stayTimer -= 0.5f;
            }
            if (backpack.itemStacks.Count > 0 && stayTimer <= 0)
            {
                agent.SetDestination(itemDestinationPos.position);
            }
            else if (backpack.itemStacks.Count == 0 && stayTimer <= 0)
            {
                agent.SetDestination(getItemPos.position);
				yield return new WaitUntil(() => backpack.itemStacks.Count > 0 && backpack.itemStacks[0].items.Count == backpack.stackSize);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

	public void PilotGoToStandPos()
	{
		agent.SetDestination(getItemPos.position);
	}

	public void PilotGoToPlanePos()
	{
		agent.SetDestination(itemDestinationPos.position);
	}

}
