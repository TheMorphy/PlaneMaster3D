using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate3 : MonoBehaviour
{
	DroppingZone dz;
	[SerializeField]
	Animator anim;
	[SerializeField]
	string savingKey;
	[SerializeField]
	GameObject disableOnOpen;
	[SerializeField]
	GameObject disableOnOpen2;

	// Start is called before the first frame update
	void Start()
	{
		dz = GetComponent<DroppingZone>();
		anim = GetComponent<Animator>();
		//LoadState();
	}

	void OnAllConditionsComplete()
	{
		anim.SetBool("GateOpen", true);
		disableOnOpen.SetActive(false);
		disableOnOpen2.SetActive(false);

		LevelSystem.instance.EnterNextLevel();
	}

	void LoadState()
	{
		if (PlayerPrefs.GetInt(savingKey) == 1)
		{
			anim.SetBool("GateOpen", true);
			disableOnOpen.SetActive(false);
			disableOnOpen2.SetActive(false);
			if (savingKey == "eastgate")
			{
				QuestSystem.instance.AddProgress("Open gate", 1);
			}
		}
		else
		{
			anim.SetBool("GateOpen", false);
			disableOnOpen.SetActive(true);
			disableOnOpen2.SetActive(true);
		}
	}
}
