using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GearMoveScript : MonoBehaviour
{

	[SerializeField] private Canvas canvas;

	GearMinigameManager gearMinigameScript;

	[HideInInspector]
	public bool isInHole;

	private void Start()
	{
		gearMinigameScript = FindObjectOfType<GearMinigameManager>();
	}

	public void DragHandler(BaseEventData data)
	{
		if (!isInHole)
		{
			PointerEventData pointerData = (PointerEventData)data;

			Vector2 position;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(
				(RectTransform)canvas.transform,
				pointerData.position,
				canvas.worldCamera,
				out position);

			transform.position = canvas.transform.TransformPoint(position);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.tag == "Gear1")
		{
			string gameobjectName = gameObject.transform.name;

			if (gameobjectName == "Gear1")
			{
				gearMinigameScript.gearName = gameobjectName;
				StartCoroutine(gearMinigameScript.WaitToMoveGear());
			}
			else
				Debug.Log("Wrong Hole!");
		}

		if (collision.tag == "Gear2")
		{
			string gameobjectName = gameObject.transform.name;

			if (gameobjectName == "Gear2")
			{
				gearMinigameScript.gearName = gameobjectName;
				StartCoroutine(gearMinigameScript.WaitToMoveGear());
			}
			else
				Debug.Log("Wrong Hole!");
		}

		if (collision.tag == "Gear3")
		{
			string gameobjectName = gameObject.transform.name;

			if (gameobjectName == "Gear3")
			{
				gearMinigameScript.gearName = gameobjectName;
				StartCoroutine(gearMinigameScript.WaitToMoveGear());
			}
			else
				Debug.Log("Wrong Hole!");
		}
	}
}
