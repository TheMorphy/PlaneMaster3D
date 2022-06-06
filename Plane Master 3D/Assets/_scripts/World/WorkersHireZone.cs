using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WorkersHireZone : MonoBehaviour
{
	[SerializeField]
	GameObject standardWorkerField;
	[SerializeField]
	GameObject workerAIPrefab;

	[SerializeField]
	List<WorkerField> workerFields = new List<WorkerField>();

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

	void ReloadUI()
	{
		for(int i = 0; i < workerFields.Count; i++)
		{
			workerFields[i].price = Mathf.CeilToInt(priceStandard + (workerFields[i].workerAI != null ? workerFields[i].workerAI.level - 1 : 0) * priceIncrement);
		}
	}

	void AddNewWorkerField()
	{
		WorkerField fieldToAdd = Instantiate(standardWorkerField, workersMenu).GetComponent<WorkerField>();
		workerFields.Add(fieldToAdd);
		fieldToAdd.hireZone = this;
		ReloadUI();
	}

	void LoadWorkers()
	{
		string savedString = PlayerPrefs.GetString("workers");
		string[] saves = savedString.Split('.');
		for(int i = 0; i < saves.Length; i++)
		{
			WorkerField fieldToLoad = Instantiate(standardWorkerField, workersMenu).GetComponent<WorkerField>();
			int loadedLevel = System.Convert.ToInt32(saves[i]);
			bool bought = loadedLevel > 0;
			if(bought)
			{
				fieldToLoad.workerAI = SpawnNewWorker();
				fieldToLoad.workerAI.level = loadedLevel;

				fieldToLoad.workerAI.agent.speed = speedStandard * Mathf.Pow(1 + speedIncrement, loadedLevel - 1);
				fieldToLoad.workerAI.backpack.stackSize = storageStandard + storageIncrement * (loadedLevel - 1);
			}


			fieldToLoad.SetBought(bought);
		}
	}

	void SaveWorkers()
	{
		string saveString = "";
		for(int i = 0; i < workerFields.Count; i++)
		{
			saveString += ".";
			saveString += (workerFields[i].workerAI != null ? workerFields[i].workerAI.level : 0).ToString();
		}
		PlayerPrefs.SetString("workers", saveString);
	}

	public WorkerAI SpawnNewWorker()
	{
		return Instantiate(workerAIPrefab, workersSpawnPosition).GetComponent<WorkerAI>();
	}




	
}
