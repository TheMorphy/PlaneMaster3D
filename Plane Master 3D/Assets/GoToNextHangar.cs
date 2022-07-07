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

	// Start is called before the first frame update
	void Start()
	{
		dz = GetComponent<DroppingZone>();
		anim = GetComponent<Animator>();
	}

	void OnAllConditionsComplete()
	{
		anim.SetBool("GateOpen", true);
		disableOnOpen.SetActive(false);
		disableOnOpen2.SetActive(false);

		LevelLoader.instance.LoadNextLevel();
	}
}
