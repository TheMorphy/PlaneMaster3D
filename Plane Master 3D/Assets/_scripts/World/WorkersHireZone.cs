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

public class WorkersHireZone : MonoBehaviour
{
	[SerializeField]
	GameObject standardWorkerField;
	[SerializeField]
	GameObject workerAIPrefab;

	[SerializeField]
	List<WorkerField> workerFields = new List<WorkerField>();

	[SerializeField]
	List<WorkerTask> tasks = new List<WorkerTask>();

	[SerializeField]
	float speedStandard;
	[SerializeField]
	float speedIncrement;

	[SerializeField]
	int storageStandard;
	int storageIncrement;

	[SerializeField]
	int priceStandard;
	[SerializeField]
	float priceIncrement;

	[Header("Systemic")]
	[SerializeField]
	Transform workersMenu;
	[SerializeField]
	Transform workersSpawnPosition;
	bool alreadyOpened;
	DroppingZone droppingZone;

	private void Start()
	{
		LoadWorkers();
		droppingZone = GetComponent<DroppingZone>();
		droppingZone.conditions[0].count = 0;

	}

	private void Update()
	{
		if(Input.GetButtonDown("Fire2"))
		{
			AddNewWorkerField();
		}
	}

	public void ReloadStats()
	{
		for(int i = 0; i < workerFields.Count; i++)
		{
			WorkerField current = workerFields[i];
			current.price = Mathf.CeilToInt(priceStandard * Mathf.Pow(1 + priceIncrement ,(workerFields[i].workerAI != null ? workerFields[i].workerAI.level - 1 : 0)));
			if(current.workerAI != null)
			{
				print(Mathf.Repeat(i, tasks.Count - 1));
				current.workerAI.agent.speed = speedStandard + (current.workerAI.level - 1) * speedIncrement;
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
				WorkerField fieldToLoad = Instantiate(standardWorkerField, workersMenu).GetComponent<WorkerField>();
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

					fieldToLoad.workerAI.agent.speed = speedStandard + speedIncrement * (loadedLevel - 1);
					fieldToLoad.workerAI.backpack.stackSize = storageStandard + storageIncrement * (loadedLevel - 1);
				}

				fieldToLoad.hireZone = this;
				fieldToLoad.SetBought(bought);
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
		return Instantiate(workerAIPrefab, workersSpawnPosition).GetComponent<WorkerAI>();
	}


	public void CloseWorkersMenu()
	{
		workersMenu.gameObject.SetActive(false);
	}

	void OnAllConditionsComplete()
	{
		AddNewWorkerField();
		droppingZone.ResetConditions();
		droppingZone.conditions[0].count = 0;
	}

	private void OnTriggerStay(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			if(!other.GetComponent<Player>().isMoving && !alreadyOpened)
			{
				workersMenu.gameObject.SetActive(true);


				alreadyOpened = true;
			}
			else
			{
				alreadyOpened = false;
			}
			
		}
	}



}
