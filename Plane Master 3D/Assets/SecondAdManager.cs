using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondAdManager : MonoBehaviour
{
	[SerializeField] GameObject[] partsToAddRbAndCollider;
	[SerializeField] GameObject bomb, spawnPoint;

	public void ExplodeTheRocket()
	{
		AddRigidbody();
		SpawnBomb();
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
}
