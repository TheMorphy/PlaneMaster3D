using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildScript : MonoBehaviour
{
	private void OnTriggerStay(Collider other)
	{
		if (other.tag == "Player")
		{
			transform.parent.GetComponent<LevelSystem>().TriggerDetected(this);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			transform.parent.GetComponent<LevelSystem>().TriggerExit(this);
		}
	}
}
