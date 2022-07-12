using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondAdManager : MonoBehaviour
{
	[SerializeField] GameObject[] partsToAddRbAndCollider;
	[SerializeField] GameObject bomb, spawnPoint, failedUI;

	[SerializeField] Animator cameraAnimation;

	public void ExplodeTheRocket()
	{
		AddRigidbody();
		SpawnBomb();
		StartCoroutine(WaitToFailLevel());
	}

	void AddRigidbody()
	{
		for (int i = 0; i < partsToAddRbAndCollider.Length; i++)
		{
			GameObject gObject = partsToAddRbAndCollider[i];
			gObject.AddComponent<Rigidbody>();
			gObject.AddComponent<BoxCollider>();
		}
	}

	void SpawnBomb()
	{
		Instantiate(bomb, spawnPoint.transform);
	}

	IEnumerator WaitToFailLevel()
	{
		cameraAnimation.SetTrigger("HasExploded");
		yield return new WaitForSeconds(0.5f);
		failedUI.SetActive(true);
		//yield return new WaitForSeconds(1f);
		//Time.timeScale = 0;
	}
}
