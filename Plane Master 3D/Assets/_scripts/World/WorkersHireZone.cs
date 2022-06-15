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
	float speedStandard;
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
		
		

	}
	private void Start()
	{
		LoadWorkers();
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
			current.SetPrice(Mathf.CeilToInt(current.workerAI == null ? workerStats[i].costHire : workerStats[i].costStandard + ((current.workerAI.level - 1) * workerStats[i].costIncrement)));
			current.SetBought(current.workerAI != null);
			if(current.workerAI != null)
			{
				current.workerAI.agent.speed = speedStandard * Mathf.Pow(1 + speedIncrement, current.workerAI.level - 1);
				current.workerAI.backpack.stackSize = storageStandard + (current.workerAI.level - 1) * storageIncrement;
				current.workerAI.itemToCarry = tasks[(int)Mathf.Repeat(i, tasks.Count - 1)].itemToCarry;
				current.workerAI.itemDestinationPos = tasks[(int)Mathf.Repeat(i, tasks.Count - 1)].bringPos;
				current.workerAI.getItemPos = tasks[(int)Mathf.Repeat(i, tasks.Count - 1)].getPos;

			}

		}
	}

	void AddNewWorkerField()
	{
		WorkerField fieldToAdd = Instantiate(standardWorkerField, workersMenu).GetComponent<WorkerField>();
		fieldToAdd.hireZone = this;
		workerFields.Add(fieldToAdd);

		ReloadStats();
		
	}

	void LoadWorkers()
	{
		string savedString = PlayerPrefs.GetString("workers");
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
				int loadedLevel = System.Convert.ToInt32(saves[i]);
				bool bought = loadedLevel > 0;
				print("bought" + bought);
				if (bought)
				{
					fieldToLoad.workerAI = SpawnNewWorker();
					fieldToLoad.workerAI.itemToCarry = tasks[(int)Mathf.Repeat(i, tasks.Count - 1)].itemToCarry;
					fieldToLoad.workerAI.itemDestinationPos = tasks[(int)Mathf.Repeat(i, tasks.Count - 1)].bringPos;
					fieldToLoad.workerAI.getItemPos = tasks[(int)Mathf.Repeat(i, tasks.Count - 1)].getPos;
					fieldToLoad.workerAI.level = loadedLevel;

					fieldToLoad.workerAI.agent.speed = speedStandard * Mathf.Pow(1 + speedIncrement, fieldToLoad.workerAI.level - 1);
					fieldToLoad.workerAI.backpack.stackSize = storageStandard + storageIncrement * (loadedLevel - 1);
				}

				fieldToLoad.hireZone = this;
				fieldToLoad.SetBought(bought);
				fieldToLoad.SetPrice(Mathf.CeilToInt(fieldToLoad.workerAI == null ? workerStats[i].costHire : workerStats[i].costStandard + ((fieldToLoad.workerAI.level - 1) * workerStats[i].costIncrement)));
				//workerFields.Add(fieldToLoad);
      
			}

		}
	}

	public void SaveWorkers()
	{
		string saveString = "";
		for(int i = 0; i < workerFields.Count; i++)
		{
			saveString += ".";
			saveString += (workerFields[i].workerAI != null ? workerFields[i].workerAI.level : 0).ToString();
		}
		PlayerPrefs.SetString("workers", saveString);
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
