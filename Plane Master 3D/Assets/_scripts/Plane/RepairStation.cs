using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairStation : MonoBehaviour
{
	[SerializeField]
	new string name;


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
	bool debug = false;
	private void Start()
	{
		dz = GetComponent<DroppingZone>();
		anim = aircraftHolder.GetComponent<Animator>();
		if(PlayerPrefs.GetInt(name + "r1") == PlayerPrefs.GetInt(name + "r2"))
		{
			level = -1;
			LevelUp();
			print("FirstLoadRepair");
		}
		else
		LoadAirCraft();
		if(overrideItemDestination != null)
			for(int i = 0; i < aircrafts.Count; i++)
				for(int b = 0; b < aircrafts[i].Breakables.Count; b++)
					for (int c = 0; c < aircrafts[i].Breakables[b].conditions.Count; c++)
						aircrafts[i].Breakables[b].conditions[c].itemDestination = overrideItemDestination;
				
			
	}

	void OnAllConditionsComplete()
	{
		breakablesToRepair[0].SendMessage("OnAllConditionsComplete");
		breakablesToRepair.RemoveAt(0);
		if(breakablesToRepair.Count > 0)
		{
			// 1 of 2 completed
			dz.conditions = breakablesToRepair[0].conditions;
			PlayerPrefs.SetInt(name + "firstDone", 1);

		}
		else
		{
			StartCoroutine(WaitForLevelUp());
		}
	}

	IEnumerator WaitForLevelUp()
	{
		pilot.PilotGoToPlanePos();
		//Put Money in stash zone
		for(int i = 0; i <= currentAircraft.Profit; i++)
		{
			rewardStashZone.AddItem(Instantiate(moneyPrefab, rewardStashZone.transform.position, Quaternion.identity).GetComponent<Item>());
			yield return new WaitForSeconds(0.04f);
		}
		
		yield return new WaitWhile(() => pilot.isMoving);
		anim.Play("RollOut");
		pilot.gameObject.SetActive(false);
		LevelUp();
		yield return new WaitForSeconds(1.2f);
		ActivateAircraftModel(level % aircrafts.Count);
		StartCoroutine(BringBreakablesToBrokenPos());
	}

	void LevelUp()
	{
		
		level += 1;
		PlayerPrefs.SetInt(name + "level", level);
		PlayerPrefs.SetInt(name + "firstDone", 0);
		dz.enabled = false;
		currentAircraft = aircrafts[level % aircrafts.Count];
		print("!!!!" + level % aircrafts.Count);
		
		
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
		dz.conditions = breakablesToRepair[0].conditions;

	}

	void GetBreakablesToRepair(int r1, int r2)
	{
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

	IEnumerator BringBreakablesToBrokenPos(bool lerpOnlyOnePart = false)
	{
		yield return new WaitForSeconds(2.5f);

		float t = 0;
		Vector3 firstStartPos = breakablesToRepair[0].transform.position;
		Quaternion firstStartRot = breakablesToRepair[0].transform.rotation;
		Vector3 secondStartPos = Vector3.zero;
		Quaternion secondStartRot = Quaternion.identity;
		if (!lerpOnlyOnePart)
		{
			 secondStartPos = breakablesToRepair[1].transform.position;
			 secondStartRot = breakablesToRepair[1].transform.rotation;
		}
		

		
		while (t < 1 || debug)
		{
			t += Time.deltaTime;
			Vector3 firstPos = Vector3.Lerp(firstStartPos, brokenUpperPos.position, partLerpCurve.Evaluate(t));
			Quaternion firstRot = Quaternion.Lerp(firstStartRot, brokenUpperPos.rotation, partLerpCurve.Evaluate(t));
			
			Vector3 secondPos = Vector3.Lerp(secondStartPos, brokenLowerPos.position, partLerpCurve.Evaluate(t));
			Quaternion secondRot = Quaternion.Lerp(secondStartRot, brokenLowerPos.rotation, partLerpCurve.Evaluate(t));

			
			breakablesToRepair[0].transform.SetPositionAndRotation(firstPos, firstRot);
			if (!lerpOnlyOnePart)
			{
				breakablesToRepair[1].transform.SetPositionAndRotation(secondPos, secondRot);

			}
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
		if(PlayerPrefs.GetInt(name + "firstDone") == 1)
		{
			print("Use one");
			breakablesToRepair.RemoveAt(0);
			StartCoroutine(BringBreakablesToBrokenPos(true));

		}
		else
		{
			print("Use Both");
			StartCoroutine(BringBreakablesToBrokenPos());

		}

		dz.conditions = breakablesToRepair[0].conditions;
	}

	void SaveAircraft()
	{
		PlayerPrefs.SetInt(name + "level", level);

	}

	






}