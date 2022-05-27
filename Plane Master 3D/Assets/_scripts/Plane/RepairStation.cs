using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairStation : MonoBehaviour
{
	[SerializeField]
	new string name;

	[SerializeField]
	int minigameIndex;

	[SerializeField]
	Aircraft currentAircraft;

	List<Breakable> breakablesToRepair = new List<Breakable>();

	[SerializeField]
	Transform brokenLowerPos, brokenUpperPos;


	[SerializeField]
	int level;

	[SerializeField]
	Transform aircraftHolder;

	[SerializeField]
	List<Aircraft> aircrafts = new List<Aircraft>();


	[SerializeField]
	AnimationCurve partLerpCurve;
	DroppingZone dz;
	Animator anim;
	[SerializeField]
	Transform overrideItemDestination;

	[SerializeField]
	WorkerAI pilot;
	[SerializeField]
	StashZone rewardStashZone;
	[SerializeField]
	GameObject moneyPrefab;
	[SerializeField]
	List<Material> materialPool;
	[SerializeField]
	CameraZone cameraZone;

	bool minigameDone = false;
	[SerializeField]
	bool debug = false;
	bool isPlayerInTrigger;
	private void Start()
	{
		print("start");
		dz = GetComponent<DroppingZone>();
		anim = aircraftHolder.GetComponent<Animator>();
		if(PlayerPrefs.GetInt(name + "r1") == PlayerPrefs.GetInt(name + "r2"))
		{
			//level = -1;
			PlayerPrefs.SetInt(name + "r1", 0);
			PlayerPrefs.SetInt(name + "r2", 1);
			//StartCoroutine(WaitForLevelUp());
			print("FirstLoadRepair");
		}
		
		LoadAirCraft();
		if(overrideItemDestination != null)
			for(int i = 0; i < aircrafts.Count; i++)
				for(int b = 0; b < aircrafts[i].Breakables.Count; b++)
					for (int c = 0; c < aircrafts[i].Breakables[b].conditions.Count; c++)
						aircrafts[i].Breakables[b].conditions[c].itemDestination = overrideItemDestination;
				
			
	}

	private void OnEnable()
	{
		
	}



	void OnAllConditionsComplete()
	{
		
			breakablesToRepair[0].SendMessage("OnAllConditionsComplete");
		
			breakablesToRepair[1].SendMessage("OnAllConditionsComplete");

		StartCoroutine(WaitForLevelUp());
	}

	bool AllConditionsTrue(List <UpgradeCondition> c)
	{
		bool output = true;
		for(int i = 0; i < c.Count; i++)
		{
			if(!c[i].completed)
			{
				output = false;
				return output;
			}
		}

		return output;

	}

	void MinigameDone()
	{
		minigameDone = true;
	}

	IEnumerator WaitForLevelUp()
	{
		
		
		if(isPlayerInTrigger)
		{
			//start the minigame
			LevelSystem.instance.PlayMinigame(minigameIndex);
			LevelSystem.instance.OnMinigameFinish.AddListener(MinigameDone);

			//wait until its finished
			yield return new WaitUntil(() => minigameDone);
		}
		
			

		
		
		//Put Money in stash zone
		int rewardLeft = currentAircraft.Profit;
		while(rewardLeft > 0)
		{
			rewardStashZone.AddItem(LevelSystem.SpawnMoneyAtPosition(ref rewardLeft, pilot.transform.position + Vector3.up));
			yield return new WaitForSeconds(0.04f);
		}

		pilot.PilotGoToPlanePos();
		yield return new WaitForSeconds(3);


		yield return new WaitWhile(() => pilot.isMoving);
		anim.Play("RollOut");
		pilot.gameObject.SetActive(false);
		LevelUp();
		yield return new WaitForSeconds(1.2f);
		// do the mat stuff here

		//Set Random Material


		int materialIndex = Random.Range(0, materialPool.Count);
		PlayerPrefs.SetInt(name + "mat", materialIndex);
		currentAircraft.Model.GetComponent<MeshRenderer>().material = materialPool[materialIndex];
		for (int i = 0; i < currentAircraft.Breakables.Count; i++)
		{
			if (currentAircraft.Breakables[i].GetComponent<MeshRenderer>() != null)
				currentAircraft.Breakables[i].GetComponent<MeshRenderer>().material = materialPool[materialIndex];
			else if (currentAircraft.Breakables[i].GetComponentInChildren<MeshRenderer>() != null)
			{
				currentAircraft.Breakables[i].GetComponentInChildren<MeshRenderer>().material = materialPool[materialIndex];
			}
		}

		ActivateAircraftModel(level % aircrafts.Count);
		
		

		StartCoroutine(BringBreakablesToBrokenPos());
		minigameDone = false;
	}


	void LoadMaterials()
	{
		int materialIndex = PlayerPrefs.GetInt(name + "mat");
		currentAircraft.Model.GetComponent<MeshRenderer>().material = materialPool[materialIndex];
		for (int i = 0; i < currentAircraft.Breakables.Count; i++)
		{
			currentAircraft.Breakables[i].GetComponent<MeshRenderer>().material = materialPool[materialIndex];
		}
	}

	void LevelUp()
	{
		
		level += 1;
		QuestSystem.instance.AddProgress("Repair a plane", 1);
		LevelSystem.instance.EnableWorkerHouse();
		PlayerPrefs.SetInt(name + "level", level);
		dz.enabled = false;
		currentAircraft = aircrafts[level % aircrafts.Count];
		print("!!!!" + level % aircrafts.Count);

		cameraZone.SetCameraIndex(currentAircraft.CameraIndex);

		



		//
		
		int r1 = Random.Range(0, currentAircraft.Breakables.Count);
		int r2 = r1;
		while(r2 == r1 && currentAircraft.Breakables.Count > 2)
		{
			r2 = Random.Range(0, currentAircraft.Breakables.Count);
		}
		GetBreakablesToRepair(r1, r2);
		

		//clear out the counts of the conditions
		for(int i = 0; i < breakablesToRepair.Count; i++)
		{
			for(int c = 0; c < breakablesToRepair[i].conditions.Count; c++)
			{
				breakablesToRepair[i].conditions[c].count = 0;
			}
		}

		PlayerPrefs.SetInt(name + "r1", r1);
		PlayerPrefs.SetInt(name + "r2", r2);
		List<UpgradeCondition> newConditions = new List<UpgradeCondition>();
		foreach (UpgradeCondition u in breakablesToRepair[0].conditions)
		{
			newConditions.Add(u);
		}
		foreach (UpgradeCondition u in breakablesToRepair[1].conditions)
		{
			newConditions.Add(u);
		}
		dz.conditions = newConditions;
		dz.Refresh();

	}

	void GetBreakablesToRepair(int r1, int r2)
	{
		breakablesToRepair.Clear();


		for (int i = currentAircraft.Breakables.Count - 1; i >= 0; i--)
		{
			if (i == r1 || i == r2)
			{
				breakablesToRepair.Add(currentAircraft.Breakables[i]);
			}
		}
	}


	void ActivateAircraftModel(int index)
	{
		for(int i = 0; i < aircrafts.Count; i++)
		{
			if(i == index)
			{
				aircrafts[i].Model.SetActive(true);
			}
			else
			{
				aircrafts[i].Model.SetActive(false);
			}
		}
	}

	IEnumerator BringBreakablesToBrokenPos()
	{
		yield return new WaitForSeconds(2.5f);

		float t = 0;
		Vector3 firstStartPos = breakablesToRepair[0].transform.position;
		Quaternion firstStartRot = breakablesToRepair[0].transform.rotation;
		Vector3 secondStartPos = Vector3.zero;
		Quaternion secondStartRot = Quaternion.identity;
		
			 secondStartPos = breakablesToRepair[1].transform.position;
			 secondStartRot = breakablesToRepair[1].transform.rotation;
		
		

		
		while (t < 1)
		{
			t += Time.deltaTime;
			Vector3 firstPos = Vector3.Lerp(firstStartPos, brokenUpperPos.position, partLerpCurve.Evaluate(t));
			Quaternion firstRot = Quaternion.Lerp(firstStartRot, Quaternion.Euler(brokenUpperPos.rotation.eulerAngles + breakablesToRepair[0].PaletteRotation), partLerpCurve.Evaluate(t));
			
			

			
			breakablesToRepair[0].transform.SetPositionAndRotation(firstPos, firstRot);
			
			Vector3 secondPos = Vector3.Lerp(secondStartPos, brokenLowerPos.position, partLerpCurve.Evaluate(t));
			Quaternion secondRot = Quaternion.Lerp(secondStartRot, Quaternion.Euler(brokenLowerPos.rotation.eulerAngles + breakablesToRepair[1].PaletteRotation), partLerpCurve.Evaluate(t));
			breakablesToRepair[1].transform.SetPositionAndRotation(secondPos, secondRot);

			
			yield return null;
		}
		dz.enabled = true;
		pilot.gameObject.SetActive(true);
		pilot.PilotGoToStandPos();
	}

	void LoadAirCraft()
	{
		level = PlayerPrefs.GetInt(name + "level");

		currentAircraft = aircrafts[level % aircrafts.Count];
		ActivateAircraftModel(level % aircrafts.Count);
		GetBreakablesToRepair(PlayerPrefs.GetInt(name + "r1"), PlayerPrefs.GetInt(name + "r2"));
		
		StartCoroutine(BringBreakablesToBrokenPos());
		List<UpgradeCondition> newConditions = new List<UpgradeCondition>();
		foreach(UpgradeCondition u in breakablesToRepair[0].conditions)
		{
			newConditions.Add(u);
		}
		foreach(UpgradeCondition u in breakablesToRepair[1].conditions)
		{
			newConditions.Add(u);
		}
		dz.conditions = newConditions;
		dz.Refresh();
		cameraZone.SetCameraIndex(currentAircraft.CameraIndex);

	}

	void SaveAircraft()
	{
		PlayerPrefs.SetInt(name + "level", level);

	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			isPlayerInTrigger = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			isPlayerInTrigger = false;
		}
	}








}
