using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GearMinigameManager : MonoBehaviour
{
	[SerializeField] GameObject congratulationsCanvas;
	[Header("Gears")]
	[SerializeField] List<GameObject> gearHoles = new List<GameObject>();
	[SerializeField] List<GameObject> gears = new List<GameObject>();
	[Space(10)]
	[Header("Gears Starting Position")]
	[SerializeField] List<GameObject> startingPositionHoles = new List<GameObject>();
	[SerializeField] List<GameObject> startingPositionGears = new List<GameObject>();


	[SerializeField] public List<int> randomNumbers = new List<int>();
	int rand;
	int lenght;

	[HideInInspector]
	public int gearCount;

	int maxGearCount;

	bool hasReset;

	[HideInInspector]
	public string gearName;

	private void Start()
	{
		hasReset = true;
		maxGearCount = gears.Count;
		ChangeGearHolePosition();
		ChangeGearStartingPosition();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.X))
		{
			ResetMinigame();
		}

		
	}

	void ChangeGearHolePosition()
	{
		Vector2 holeStartingPosition;
		int randomNumbIndex;

		AssignRandomParts();

		if (gearHoles.Count == startingPositionHoles.Count)
		{
			for (int i = 0; i < startingPositionHoles.Count; i++)
			{
				holeStartingPosition = startingPositionHoles[i].transform.position;
				randomNumbIndex = randomNumbers[i];
				gearHoles[randomNumbIndex].transform.position = holeStartingPosition;
			}
		} else print("The Lenght of both lists must be the same!");
	}

	void ChangeGearStartingPosition()
	{
		Vector2 gearStartingPosition;

		if (gears.Count == startingPositionGears.Count)
		{
			for (int i = 0; i < startingPositionGears.Count; i++)
			{
				gearStartingPosition = startingPositionGears[i].transform.position;
				gears[i].transform.position = gearStartingPosition;
			}
		} else print("The Lenght of both lists must be the same!");
	}

	public void AssignRandomParts()
	{
		randomNumbers = new List<int>(new int[lenght]);

		lenght = gearHoles.Count;
		
		for (int i = 0; i < lenght; i++)
		{
			rand = Random.Range(0, gearHoles.Count);

			while (randomNumbers.Contains(rand))
			{
				rand = Random.Range(0, gearHoles.Count);
			}

			randomNumbers.Add(rand);
		}
	}

	IEnumerator WaitAndStartMinigame()
	{
		if (hasReset)
		{
			ChangeGearHolePosition();
			ChangeGearStartingPosition();
		}
		yield return new WaitForSeconds(0.2f);
		hasReset = false;
	}

	public IEnumerator WaitToMoveGear()
	{
		Debug.Log("Move Gear");
		yield return new WaitForSeconds(0.5f);
		for(int i = 0; i < gears.Count; i++)
		{
			if (gearName == gears[i].transform.name)
			{
				gears[i].GetComponent<GearMoveScript>().isInHole = true;
				gears[i].GetComponent<GearMoveScript>().enabled = false;
				gears[i].transform.position = gearHoles[i].transform.position;
				gears[i].GetComponent<Image>().raycastTarget = false;
				gearCount += 1;
				Debug.Log("Right Hole!");
			}
		}
		if (gearCount >= maxGearCount)
		{
			gameObject.SetActive(false);
		}
	}

	void ResetMinigame()
	{
		ChangeGearStartingPosition();

		randomNumbers.Remove(0);
		randomNumbers.Remove(1);
		randomNumbers.Remove(2);
	}
}
