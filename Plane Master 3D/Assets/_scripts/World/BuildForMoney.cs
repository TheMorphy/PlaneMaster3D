using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildForMoney : MonoBehaviour
{
	DroppingZone dz;


	[SerializeField]
	GameObject buyArea;
	[SerializeField]
	GameObject interactableToUnlock;

	[SerializeField]
	GameObject messageReceiver;
	[SerializeField]
	string savingKey;

	[SerializeField]
	bool activeStart = false;


	private void Start()
	{
		dz = GetComponent<DroppingZone>();
		LoadProgress();
	}

	public void ActivateBuyZone()
	{
		this.gameObject.SetActive(true);
		buyArea.SetActive(true);
		
		PlayerPrefs.SetInt(savingKey, Mathf.Max(PlayerPrefs.GetInt(savingKey), 1));

		LoadProgress();
	}

	void OnAllConditionsComplete()
	{
		PlayerPrefs.SetInt(savingKey, 2);
		if(messageReceiver != null)
		messageReceiver.SendMessage("OnBoughtStation");
		LoadProgress();
	}

	

	public void SetAsBought()
	{
		PlayerPrefs.SetInt(savingKey, 2);
		LoadProgress();
	}

	void LoadProgress()
	{
		
		int progress = PlayerPrefs.GetInt(savingKey);
		switch (progress)
		{
			case 0:
				//This object is not even unlocked
				gameObject.SetActive(false);
				break;
			case 1:
				//This object is unlocked
				gameObject.SetActive(true);
				buyArea.SetActive(true);
				interactableToUnlock.SetActive(false);
				break;
			case 2:
				//Interactable bought
				gameObject.SetActive(true);
				buyArea.SetActive(false);
				interactableToUnlock.SetActive(true);
				break;
		}
		if (activeStart)
			gameObject.SetActive(true);
	}
}
