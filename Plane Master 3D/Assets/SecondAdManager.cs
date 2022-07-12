using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondAdManager : MonoBehaviour
{
	[SerializeField] GameObject[] partsToAddRbAndCollider;
	[SerializeField] GameObject bomb, spawnPoint, failedUI;

	[SerializeField] Animator cameraAnimation;
	[SerializeField] float startDelay;

	public void ExplodeTheRocket()
	{
		
		
		StartCoroutine(WaitToFailLevel());
	}

	void AddRigidbody()
	{
		for (int i = 0; i < partsToAddRbAndCollider.Length; i++)
		{
			GameObject gObject = partsToAddRbAndCollider[i];
			gObject.GetComponent<Rigidbody>().isKinematic = false;
			gObject.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
			gObject.GetComponent<Collider>().enabled = true;
		}
	}

	Bomb SpawnBomb()
	{
		return Instantiate(bomb, spawnPoint.transform).GetComponent<Bomb>();
	}

	IEnumerator WaitToFailLevel()
	{
		yield return new WaitForSeconds(startDelay);
		cameraAnimation.SetTrigger("HasExploded");
		Bomb bomb = SpawnBomb();
		yield return new WaitForSeconds(0.12f);
		AddRigidbody();
		bomb.KnockBack();
		yield return new WaitForSeconds(0.5f - 0.12f);
		failedUI.SetActive(true);
		//yield return new WaitForSeconds(1f);
		//Time.timeScale = 0;
	}
}
