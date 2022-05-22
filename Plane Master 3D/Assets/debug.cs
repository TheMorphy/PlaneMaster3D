using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debug : MonoBehaviour
{
	[SerializeField] int amount = 10;
	[SerializeField] float time = 1;
	[SerializeField] List<Item> itemList = new List<Item>();

	private void OnTriggerEnter(Collider other)
	{
		int moneyToAdd = amount;
		while(moneyToAdd > 0)
		{
			LevelSystem.SpawnMoneyAtPosition(ref moneyToAdd, transform.position);
		}
		print("Debug");
	}	
}
