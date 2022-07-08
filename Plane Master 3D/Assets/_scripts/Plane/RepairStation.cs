using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

	//David Changes
	[SerializeField] GameObject[] specificToolToEnable;

	[SerializeField] GameObject[] specificWorkerButtonToEnable;

	[SerializeField]
	GameObject partyEmoji;
	[SerializeField]
	bool minigamesEnabled = false;

	[SerializeField]
	GameObject textBG;
	[SerializeField]
	float timePilotDropsMoney = 1;

	private void Awake()
	{
		dz = GetComponent<DroppingZone>();

		LoadAirCraft();

	}
	private void Start()
	{
		print("start");

		for (int i = 0; i < specificToolToEnable.Length; i++)
		{
			specificWorkerButtonToEnable[i].GetComponent<Button>().interactable = true;
		}

		anim = aircraftHolder.GetComponent<Animator>();
		if(PlayerPrefs.GetInt(name + "r1") == PlayerPrefs.GetInt(name + "r2"))
		{
			//level = -1;
			PlayerPrefs.SetInt(name + "r1", 0);
			PlayerPrefs.SetInt(name + "r2", 1);
			//StartCoroutine(WaitForLevelUp());
			print("FirstLoadRepair");
		}
		try
		{
			if (overrideItemDestination != null)
				for (int i = 0; i < aircrafts.Count; i++)
					for (int b = 0; b < aircrafts[i].Breakables.Count; b++)
						for (int c = 0; c < aircrafts[i].Breakables[b].conditions.Count; c++)
							aircrafts[i].Breakables[b].conditions[c].itemDestination = overrideItemDestination;

		}
		catch
		{

		}

		CheckForProgress();

		anim.SetFloat("kind", currentAircraft.Model.CompareTag("Rocket") ? 1 : 0);



	}


	void CheckForProgress()
	{
		switch (name)
		{
			
			case "Boeing555":
				QuestSystem.instance.AddProgress("Buy second repair zone", 1);
				break;
			case "Boeing701":
				QuestSystem.instance.AddProgress("Buy third repair zone", 1);
				break;
			case "Boeing21121":
				QuestSystem.instance.AddProgress("Buy fourth repair zone", 1);
				break;
			case "Boeing22":
				QuestSystem.instance.AddProgress("Buy fifth repair zone", 1);
				break;
			case "Boeing111":
				QuestSystem.instance.AddProgress("Buy sixth repair zone", 1);
				break;

		}
	}


	void OnAllConditionsComplete()
	{
			if (breakablesToRepair.Count > 0)
			breakablesToRepair[0].SendMessage("OnAllConditionsComplete");
			if(breakablesToRepair.Count > 1)
			breakablesToRepair[1].SendMessage("OnAllConditionsComplete");

		StartCoroutine(WaitForLevelUp());
		CheckForProgress();
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
		if(textBG != null)
		textBG.SetActive(false);


		if (isPlayerInTrigger && minigamesEnabled)
		{
			//start the minigame
			LevelSystem.instance.PlayMinigame(minigameIndex);
			LevelSystem.instance.OnMinigameFinish.AddListener(MinigameDone);

			//wait until its finished
			yield return new WaitUntil(() => minigameDone);

			

			
		}


		switch (name)
		{
			case "Boeing555":
				QuestSystem.instance.AddProgress("Repair plane in second repair zone", 1);
				break;
			case "Boeing701":
				QuestSystem.instance.AddProgress("Repair plane in third repair zone", 1);
				break;


		}


		//Put Money in stash zone
		int rewardLeft = currentAircraft.Profit;
		while(rewardLeft > 0)
		{
			rewardStashZone.AddItem(LevelSystem.SpawnMoneyAtPosition(ref rewardLeft, pilot.transform.position + Vector3.up));
			yield return new WaitForSeconds(timePilotDropsMoney / currentAircraft.Profit);
		}

		pilot.PilotGoToPlanePos();

		partyEmoji.SetActive(true);
		yield return new WaitForSeconds(1f);


		yield return new WaitWhile(() => pilot.isMoving);
		anim.Play("RollOut");
		foreach (BoxCollider collider in currentAircraft.Model.GetComponents<BoxCollider>())
		{
			collider.enabled = false;
		}
		anim.SetFloat("kind", currentAircraft.Model.CompareTag("Rocket") ? 1 : 0);

		
		pilot.gameObject.SetActive(false);
		partyEmoji.SetActive(false);
		LevelUp();
		yield return new WaitForSeconds(1.2f);
		// do the mat stuff here

		//Set Random Material


		int materialIndex = Random.Range(0, materialPool.Count);
		PlayerPrefs.SetInt(name + "mat", materialIndex);
		if(currentAircraft.Model.GetComponent<MeshRenderer>() != null)
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
		//print("!!!!" + level % aircrafts.Count);

		cameraZone.SetCameraIndex(currentAircraft.CameraIndex);

		



		//
		
		int r1 = Random.Range(0, currentAircraft.Breakables.Count);
		int r2 = r1;
		while(r2 == r1 && currentAircraft.Breakables.Count >= 2)
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

		if (specificToolToEnable[0] != null)
		{
			specificToolToEnable[0].SetActive(true);
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
		if (textBG != null)
			textBG.SetActive(true);

	}

	void LoadAirCraft()
	{
		level = PlayerPrefs.GetInt(name + "level");

		currentAircraft = aircrafts[level % aircrafts.Count];
		ActivateAircraftModel(level % aircrafts.Count);
		//GetBreakablesToRepair(PlayerPrefs.GetInt(name + "r1"), PlayerPrefs.GetInt(name + "r2"));

		int r1 = Random.Range(0, currentAircraft.Breakables.Count);
		int r2 = r1;
		while (r2 == r1 && currentAircraft.Breakables.Count >= 2)
		{
			r2 = Random.Range(0, currentAircraft.Breakables.Count);
		}
		GetBreakablesToRepair(r1, r2);

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
