using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
class WorkerTask
{
	public Transform getPos, bringPos;
	public ItemType itemToCarry;
}

[System.Serializable]
class WorkerStats
{
	public int costHire, costStandard, costIncrement;


}

public class WorkersHireZone : MonoBehaviour
{
	public string savingKey;
	[SerializeField]
	GameObject standardWorkerField;
	[SerializeField]
	GameObject workerAIPrefab;

	[SerializeField]
	List<WorkerField> workerFields = new List<WorkerField>();
	[SerializeField]
	public int maxLevel = 5;

	[SerializeField]
	List<WorkerTask> tasks = new List<WorkerTask>();

	[SerializeField]
	public float speedStandard;
	[SerializeField]
	float speedIncrement;

	[SerializeField]
	int storageStandard;
	[SerializeField]
	int storageIncrement;

	/*[SerializeField]
	int priceStandard;
	[SerializeField]
	float priceIncrement;*/
	[SerializeField]
	List<WorkerStats> workerStats = new List<WorkerStats>();

	[Header("Systemic")]
	[SerializeField]
	Transform workersMenu;
	[SerializeField]
	GameObject workersCanvas;
	[SerializeField]
	Transform workersSpawnPosition;
	bool alreadyOpened;
	DroppingZone droppingZone;
	[SerializeField]
	RepairStationOrganizer repairStationOrganizer;
	[SerializeField]
	Animator doorAnim;
	[SerializeField]
	float workerOpenGateDistance;

	public int WorkerFieldIndex(WorkerField workerField)
	{
		return workerFields.FindIndex(w => w == workerField);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, workerOpenGateDistance);
	}

	private void Awake()
	{
		//LoadWorkers();


	}
	public void SetTask(WorkerField workerField)
	{
		int fieldIndex = WorkerFieldIndex(workerField);

		workerField.workerAI.itemToCarry = tasks[fieldIndex].itemToCarry;
		workerField.workerAI.getItemPos = tasks[fieldIndex].getPos;
		workerField.workerAI.itemDestinationPos = tasks[fieldIndex].bringPos;
		
	}
	private void Start()
	{
		LoadWorkers();
		ReloadStats();
		droppingZone = GetComponent<DroppingZone>();
		droppingZone.conditions[0].count = 0;

		if (repairStationOrganizer.level <= workerFields.Count)
		{
			droppingZone.conditions[0].count = 100;
			repairStationOrganizer.OnLevelUp.AddListener(Onlvlup);
			
		}

		StartCoroutine(CheckDoor());
			
	}

	IEnumerator CheckDoor()
	{
		yield return new WaitUntil(() => IsWorkerInRange());
		doorAnim.SetBool("open", true);
		yield return new WaitUntil(() => !IsWorkerInRange());
		doorAnim.SetBool("open", false);
	}

	public void PressAd03HireButton()
	{
		StartCoroutine(CheckDoor());
		WorkerAI ai = Instantiate(workerAIPrefab, workersSpawnPosition).GetComponent<WorkerAI>();
		WorkerField placebo = new WorkerField();
		placebo.workerAI = ai;
		workerFields.Add(placebo);
		ai.task = WorkerAI.TaskType.Follower;
	}
	bool IsWorkerInRange()
	{
		for(int i = 0; i < workerFields.Count; i++)
		{
			if(workerFields[i].workerAI != null)
			{
				if(Vector3.Distance(workerFields[i].workerAI.transform.position, transform.position) < workerOpenGateDistance)
				{
					return true;
				}
			}
			
		}
		return false;
	}

	public void ReloadStats()
	{
		for(int i = 0; i < workerFields.Count; i++)
		{
			WorkerField current = workerFields[i];
			//current.SetPrice(Mathf.CeilToInt(current.workerAI == null ? workerStats[i].costHire : workerStats[i].costStandard + ((current.workerAI.level - 1) * workerStats[i].costIncrement)));
			current.SetBought(current.workerAI != null);
			if(current.workerAI != null)
			{
				current.workerAI.agent.speed = speedStandard * Mathf.Pow(1 + speedIncrement, current.speedLevel - 1);
				current.workerAI.backpack.stackSize = storageStandard + (current.backpackLevel - 1) * storageIncrement;
				//current.workerAI.itemToCarry = tasks[(int)Mathf.Repeat(i, tasks.Count - 1)].itemToCarry;
				//current.workerAI.itemDestinationPos = tasks[(int)Mathf.Repeat(i, tasks.Count - 1)].bringPos;
				//current.workerAI.getItemPos = tasks[(int)Mathf.Repeat(i, tasks.Count - 1)].getPos;

			}
			current.SetPrice(Mathf.CeilToInt(current.workerAI == null ? workerStats[i].costHire : workerStats[i].costStandard + ((current.backpackLevel - 1) * workerStats[i].costIncrement)), storageStandard + (current.backpackLevel - 1) * storageIncrement, speedStandard * Mathf.Pow(1 + speedIncrement, current.speedLevel - 1));

		}
	}

	void AddNewWorkerField()
	{
		WorkerField fieldToAdd = Instantiate(standardWorkerField, workersMenu).GetComponent<WorkerField>();
		fieldToAdd.hireZone = this;
		workerFields.Add(fieldToAdd);

		//ReloadStats();
	}

	void LoadWorkers()
	{
		
		string savedString = PlayerPrefs.GetString(savingKey + "workers");
		string[] saves = savedString.Split('.');
		for(int i = 0; i < saves.Length; i++)
		{
			
			if(saves[i] != "")
			{
				print("saves: " + saves[i]);
					WorkerField fieldToLoad;
				if (i < workerFields.Count)
					fieldToLoad = workerFields[i];  //Instantiate(standardWorkerField, workersMenu).GetComponent<WorkerField>();
				else
					break;
				string[] levels = saves[i].Split('|');
				int loadedStorageLevel = System.Convert.ToInt32(levels[0]);
				int loadedSpeedLevel = System.Convert.ToInt32(levels[1]);
				//int loadedLevel = System.Convert.ToInt32(saves[i]);
				bool bought = Mathf.Max(loadedSpeedLevel, loadedStorageLevel) > 0;
				print("bought" + bought);
				if (bought)
				{
					WorkerAI loadedWorker = SpawnNewWorker();
					fieldToLoad.workerAI = loadedWorker;
					fieldToLoad.workerFieldScripts[0].workerAI = loadedWorker;
					fieldToLoad.workerFieldScripts[1].workerAI = loadedWorker;
					fieldToLoad.workerAI.backpack.savingKey = savingKey + i;
					fieldToLoad.workerAI.repairStation = savingKey == "Box01" ? LevelSystem.instance.repairStations[i] : LevelSystem.instance.repairStations[i + 3];
					/*
					fieldToLoad.workerAI.itemToCarry = tasks[(int)Mathf.Repeat(i, tasks.Count)].itemToCarry;
					fieldToLoad.workerAI.itemDestinationPos = tasks[(int)Mathf.Repeat(i, tasks.Count)].bringPos;
					fieldToLoad.workerAI.getItemPos = tasks[(int)Mathf.Repeat(i, tasks.Count)].getPos; */
					SetTask(fieldToLoad);
					fieldToLoad.backpackLevel = loadedStorageLevel;
					fieldToLoad.speedLevel = loadedSpeedLevel;
					fieldToLoad.workerFieldScripts[0].backpackLevel = loadedStorageLevel;

					fieldToLoad.workerFieldScripts[1].speedLevel = loadedSpeedLevel;
					

					fieldToLoad.workerAI.agent.speed = speedStandard * Mathf.Pow(1 + speedIncrement, loadedSpeedLevel - 1);
					fieldToLoad.workerAI.backpack.stackSize = storageStandard + storageIncrement * (loadedStorageLevel - 1);
				}

				fieldToLoad.hireZone = this;

				fieldToLoad.SetBought(bought);
				fieldToLoad.SetPrice(Mathf.CeilToInt(fieldToLoad.workerAI == null ? workerStats[i].costHire : workerStats[i].costStandard + ((fieldToLoad.workerAI.level - 1) * workerStats[i].costIncrement)), storageStandard + storageIncrement * (loadedStorageLevel - 1), speedStandard * Mathf.Pow(1 + speedIncrement, loadedSpeedLevel - 1));
				//workerFields.Add(fieldToLoad);
      
			}

		}
	}

	public void SaveWorkers()
	{
		string saveString = "";
		for(int i = 0; i < workerFields.Count; i++)
		{
			
			saveString += (workerFields[i].workerAI != null ? workerFields[i].workerFieldScripts[0].backpackLevel : 0).ToString() + "|";
			saveString += (workerFields[i].workerAI != null ? workerFields[i].workerFieldScripts[1].speedLevel : 0).ToString() + ".";
		}
		PlayerPrefs.SetString(savingKey + "workers", saveString);
		print(saveString);
	}

	public WorkerAI SpawnNewWorker()
	{ 
		StartCoroutine(CheckDoor());
		return Instantiate(workerAIPrefab, workersSpawnPosition).GetComponent<WorkerAI>();
		
	}


	public void CloseWorkersMenu()
	{
		workersCanvas.SetActive(false);
	}

	void OnAllConditionsComplete()
	{
		
		if (repairStationOrganizer.level > workerFields.Count)
		{
			AddNewWorkerField();
			droppingZone.ResetConditions();
		}
		
		repairStationOrganizer.OnLevelUp.AddListener(Onlvlup);
	}

	void Onlvlup()
	{
		
		if(droppingZone.conditions[0].count == droppingZone.conditions[0].countNeeded)
		{
			print("onLevelup");
			droppingZone.ResetConditions();
		}
	}

	private void Stay(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			if(!other.GetComponent<Player>().isMoving && !alreadyOpened)
			{
				workersCanvas.SetActive(true);


				alreadyOpened = true;
			}
			else if(other.GetComponent<Player>().isMoving)
			{
				alreadyOpened = false;
			}
			
		}
	}



}
