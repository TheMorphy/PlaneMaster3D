using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GoToNextHangar : MonoBehaviour
{
	DroppingZone dz;
	[SerializeField]
	Animator anim;
	[SerializeField]
	GameObject disableOnOpen;
	[SerializeField]
	GameObject disableOnOpen2;

	[SerializeField] bool resetActiveScene;
	[SerializeField] bool tutorialEnd;
	[SerializeField] GameObject tutorialEndWindow;

	// Start is called before the first frame update
	void Start()
	{
		dz = GetComponent<DroppingZone>();
		anim = GetComponent<Animator>();
	}

	void OnAllConditionsComplete()
	{
		if (tutorialEnd)
		{
			tutorialEndWindow.SetActive(true);
			return;
		}

		if (!resetActiveScene)
		{
			anim.SetBool("GateOpen", true);

			if (disableOnOpen != null && disableOnOpen2 != null)
			{
				disableOnOpen.SetActive(false);
				disableOnOpen2.SetActive(false);
			}
			LevelLoader.instance.LoadNextLevel();
		}
		else
		{
			anim.SetBool("GateOpen", true);

			if (disableOnOpen != null && disableOnOpen2 != null)
			{
				disableOnOpen.SetActive(false);
				disableOnOpen2.SetActive(false);
			}
			LevelLoader.instance.ResetActiveLevel();
		}
	}
}
