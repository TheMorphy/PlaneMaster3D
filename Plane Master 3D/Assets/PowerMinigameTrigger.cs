using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerMinigameTrigger : MonoBehaviour
{
	[SerializeField] Image fillImage;
	[SerializeField] GameObject parentObject;
	[SerializeField] GameObject congratulationsUI;

	public float timeRemaining = 10;

	float startTime;

	bool isTriggering;

	private void Start()
	{
		fillImage.color = Color.red;
		startTime = timeRemaining;
	}

	private void Update()
	{
		if (isTriggering)
		{
			if (timeRemaining > 0)
			{
				timeRemaining -= Time.deltaTime;
			}
		}
		else
			timeRemaining = startTime;

		if (timeRemaining <= 0)
		{
			ResetMinigame();
		}
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "redspot")
		{
			fillImage.color = Color.white;
			isTriggering = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		fillImage.color = Color.red;
		isTriggering = false;
	}

	private void ResetMinigame()
	{
		timeRemaining = 0;
		PowerMinigameManager powerManagerScript = parentObject.GetComponent<PowerMinigameManager>();
		//powerManagerScript.Restart();
		fillImage.color = Color.red;
		fillImage.fillAmount = 0;
		parentObject.SetActive(false);
		//congratulationsUI.SetActive(true);
	}
}
