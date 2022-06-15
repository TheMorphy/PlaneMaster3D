using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    DroppingZone dz;
	[SerializeField]
    Animator anim;
    [SerializeField]
    string savingKey;
    [SerializeField]
    GameObject disableOnOpen;

    // Start is called before the first frame update
    void Start()
    {
        dz = GetComponent<DroppingZone>();
        anim = GetComponent<Animator>();
		LoadState();
    }

    void OnAllConditionsComplete()
	{
        PlayerPrefs.SetInt(savingKey, 1);
        anim.SetBool("GateOpen", true);
        disableOnOpen.SetActive(false);

		if(savingKey == "eastgate2")
		{
			QuestSystem.instance.AddProgress("Open fake gate", 1);
		}
		if (savingKey == "eastgate")
		{
			QuestSystem.instance.AddProgress("Open gate", 1);
		}

	}

    void LoadState()
	{
        if(PlayerPrefs.GetInt(savingKey) == 1)
		{
            anim.SetBool("GateOpen", true);
            disableOnOpen.SetActive(false);
			if (savingKey == "eastgate")
			{
				QuestSystem.instance.AddProgress("Open gate", 1);
			}
		}
        else
		{
			anim.SetBool("GateOpen", false);
			disableOnOpen.SetActive(true);
		}
	}
}
