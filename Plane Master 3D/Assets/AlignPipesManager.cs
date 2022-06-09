using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignPipesManager : MonoBehaviour
{
	[SerializeField] GameObject pipesHolder;
	[Space(10)]
	[SerializeField] GameObject[] pipes;

	[SerializeField] GameObject congratulationsGameobject;

	int totalPipes = 0;
	int correctPipes = 0;

	private void Start()
	{
		totalPipes = pipesHolder.transform.childCount;

		pipes = new GameObject[totalPipes];

		for (int i = 0; i < pipes.Length; i++)
		{
			pipes[i] = pipesHolder.transform.GetChild(i).gameObject;
		}
	}

	public void checkForCorrectMove()
	{
		correctPipes += 1;

		if (correctPipes == totalPipes)
		{
			Debug.Log("You Win!");
			RestartMinigame();
			//congratulationsGameobject.SetActive(true);
		}
	}

	public void checkForWrongMove()
	{
		correctPipes -= 1;
	}

	private void RestartMinigame()
	{
		for (int i = 0; i < pipes.Length; i++)
		{
			pipes[i] = pipesHolder.transform.GetChild(i).gameObject;
			pipes[i].GetComponent<PipeScript>().ChangePipesRotationRandomly();
		}

		correctPipes = 0;

		gameObject.SetActive(false);
	}
}
