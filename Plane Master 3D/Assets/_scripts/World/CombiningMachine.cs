using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombiningMachine : MonoBehaviour
{
	
	
	




	[SerializeField]
	ItemType inputType1, inputType2;
	[SerializeField]
	GameObject inputPrefab1, inputPrefab2;
	[SerializeField]
	GameObject outputItemPrefab;
	[SerializeField]
	float speed;


	[Header("Systemic")]
	[SerializeField]
	DroppingZone droppingZone;
	[SerializeField]
	StashZone stashZone;
	[SerializeField]
	Transform combinePosition;
	[SerializeField]
	Transform itemDestination;
	[SerializeField]
	Animator anim;

	bool circuitFinished;



	Coroutine machine;
	// Start is called before the first frame update
	private void OnEnable()
	{
		machine = StartCoroutine(Machine());
	}

	private void OnDisable()
	{
		StopCoroutine(machine);
	}

	// Update is called once per frame
	void Update()
    {
        
    }

	void FinishCircuit()
	{
		circuitFinished = true;
	}

	IEnumerator Machine()
	{
		while(true)
		{
			
			if(!(droppingZone.conditions[0].count > 0 && droppingZone.conditions[1].count > 0 && stashZone.currentStashCount < stashZone.capacity))
				anim.SetBool("Printing", false);
			circuitFinished = false;
			yield return new WaitUntil(() => droppingZone.conditions[0].count > 0 && droppingZone.conditions[1].count > 0 && stashZone.currentStashCount < stashZone.capacity);
			anim.SetBool("Printing", true);
			anim.speed = speed;
			yield return null;
			
			Item item1 = Instantiate(inputPrefab1, itemDestination.position, Quaternion.identity).GetComponent<Item>();
			Item item2 = Instantiate(inputPrefab2, itemDestination.position, Quaternion.identity).GetComponent<Item>();
			Vector3 start1 = item1.transform.position, start2 = item2.transform.position;
			droppingZone.items.RemoveAt(droppingZone.items.FindLastIndex(i => i.itemType == inputType1));
			droppingZone.items.RemoveAt(droppingZone.items.FindLastIndex(i => i.itemType == inputType2));
			droppingZone.conditions[0].count--;
			droppingZone.conditions[1].count--;
			droppingZone.Refresh();

			item1.pickedUp = true;
			item2.pickedUp = true;



			//Lerp items to combinePosition


			float t = 0;
			while(t < 1)
			{
				t += Time.deltaTime * speed * 5;
				item1.transform.position = Vector3.Lerp(start1, combinePosition.position, t);
				item2.transform.position = Vector3.Lerp(start2, combinePosition.position, t);
				yield return null;
			}
			anim.GetComponent<PrinterAnimation>().OnAnimationEvent.AddListener(FinishCircuit);
			yield return new WaitUntil(() => circuitFinished);
			item1.gameObject.SetActive(false);
			item2.gameObject.SetActive(false);
			Destroy(item1.gameObject, 10f);
			Destroy(item2.gameObject, 10f);
			//Item combinedItem = Instantiate(outputItemPrefab, combinePosition.position, Quaternion.identity).GetComponent<Item>();
			if(stashZone.currentStashCount < stashZone.capacity)
			stashZone.AddItem(Instantiate(outputItemPrefab, combinePosition.position, Quaternion.identity).GetComponent<Item>());
		}
	}


	
}
