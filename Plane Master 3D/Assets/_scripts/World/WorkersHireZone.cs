using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkersHireZone : MonoBehaviour
{
	[SerializeField]
	GameObject worker;







	private void OnTriggerStay(Collider other)
	{
		worker.SendMessage("OnTriggerStay", other);
	}

	private void OnTriggerExit(Collider other)
	{
		worker.SendMessage("OnTriggerExit", other);
	}
}
