using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefuelMinigameMultipleManager : MonoBehaviour
{
	[SerializeField] List<GameObject> multipleRefuels = new List<GameObject>();

	[SerializeField] int numberOfRefills = 0;

	int maxRefill;
	bool allIsRefilled;

	LongClickButton buttonScript;

	private void Start()
	{
		maxRefill = multipleRefuels.Count;
		print(maxRefill);
	}

	private void Update()
	{
		if (numberOfRefills >= maxRefill)
		{
			numberOfRefills = 0;
			allIsRefilled = true;
		}

		// Reset the minigame
		if (allIsRefilled)
		{
			for (int i = 0; i < multipleRefuels.Count; i++)
			{
				buttonScript = multipleRefuels[i].GetComponentInChildren<LongClickButton>();
				buttonScript.ResetMultipleRefills();
				gameObject.SetActive(false);
			}
			allIsRefilled = false;
			buttonScript = null;
		}
	}

	public void GetEachCompletedFill()
	{
		if (numberOfRefills < maxRefill)
		{
			numberOfRefills += 1;
		}
	}
}
