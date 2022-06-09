using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeScript : MonoBehaviour
{
	[SerializeField] bool isPlaced = false;

	[SerializeField] float[] correctRotations;

	int possibleRotations = 1;

	float[] rotations = { 0, 90, 180, 270 };

	AlignPipesManager pipesManagerScript;

	private void Awake()
	{
		pipesManagerScript = FindObjectOfType<AlignPipesManager>();
	}

	private void Start()
	{
		possibleRotations = correctRotations.Length;

		ChangePipesRotationRandomly();

		if (possibleRotations > 1)
		{
			if (transform.eulerAngles.z == correctRotations[0] || transform.eulerAngles.z == correctRotations[1])
			{
				isPlaced = true;
				pipesManagerScript.checkForCorrectMove();
			}
		}
		else
		{
			if (transform.eulerAngles.z == correctRotations[0])
			{
				isPlaced = true;
				pipesManagerScript.checkForCorrectMove();
			}
		}
	}

	public void ChangePipesRotationRandomly()
	{
		int rand = Random.Range(0, rotations.Length);

		transform.eulerAngles = new Vector3(0, 0, rotations[rand]);
	}

	void CheckPipeRotation()
	{
		if (possibleRotations > 1)
		{
			if (transform.eulerAngles.z == correctRotations[0] || transform.eulerAngles.z == correctRotations[1])
			{
				isPlaced = true;
				pipesManagerScript.checkForCorrectMove();
			}
			else if (isPlaced)
			{
				isPlaced = false;
				pipesManagerScript.checkForWrongMove();
			}
		}
		else
		{
			if (transform.eulerAngles.z == correctRotations[0])
			{
				isPlaced = true;
				pipesManagerScript.checkForCorrectMove();
			}
			else if (isPlaced)
			{
				isPlaced = false;
				pipesManagerScript.checkForWrongMove();
			}
		}
	}

	public void OnMouseDown()
	{
		transform.Rotate(new Vector3(0, 0, 90));

		CheckPipeRotation();
	}
}
