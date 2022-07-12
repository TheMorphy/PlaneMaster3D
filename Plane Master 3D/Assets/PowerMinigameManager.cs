using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerMinigameManager : MonoBehaviour
{
	[SerializeField] GameObject redSpotHolder;
	[SerializeField] GameObject[] redSpots;
	[Space (10)]
	[SerializeField] Transform handle;
	[SerializeField] Image fill;
	[SerializeField] GameObject middleButton;
	//[SerializeField] Text valTxt;
	Vector3 mousePos;
	GameObject redSpotToReset;

	int totalSpots = 0;

	/*public void OnHandleDrag()
	{

		Debug.Log("Drag");

		mousePos = Input.mousePosition;
		Vector2 dir = mousePos - handle.position;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		angle = (angle <= 0) ? (360 + angle) : angle;
		if (angle <= 225 || angle >= 315)
		{
			Quaternion r = Quaternion.AngleAxis(angle + 135f, Vector3.forward);
			handle.rotation = r;
			angle = ((angle >= 315) ? (angle - 360) : angle) + 45;
			fill.fillAmount = 0.75f - (angle / 360f);
			//valTxt.text = Mathf.Round((fill.fillAmount * 100) / 0.75f).ToString();
		}
	}

	private void Start()
	{
		totalSpots = redSpotHolder.transform.childCount;

		redSpots = new GameObject[totalSpots];

		for (int i = 0; i < redSpots.Length; i++)
		{
			redSpots[i] = redSpotHolder.transform.GetChild(i).gameObject;
		}

		int rand = Random.Range(0, totalSpots);

		redSpotToReset = redSpots[rand];

		redSpotToReset.gameObject.SetActive(true);
	}

	public void Restart()
	{
		redSpotToReset.SetActive(false);

		for (int i = 0; i < redSpots.Length; i++)
		{
			redSpots[i] = redSpotHolder.transform.GetChild(i).gameObject;
		}

		int rand = Random.Range(0, totalSpots);

		redSpotToReset = redSpots[rand];

		redSpotToReset.gameObject.SetActive(true);

		Vector3 rot = new Vector3(0, 0, 0);

		middleButton.transform.eulerAngles = rot;
	}*/

	public void ButtonClick()
	{
		StartCoroutine(WaitForDisable());
	}

	IEnumerator WaitForDisable()
	{
		yield return new WaitForSeconds(1.1f);
		gameObject.SetActive(false);

	}
}
