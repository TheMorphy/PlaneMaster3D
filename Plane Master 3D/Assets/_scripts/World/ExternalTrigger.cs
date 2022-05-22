using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalTrigger : MonoBehaviour
{

	[SerializeField]
	string messageToSend;
	[SerializeField]
	GameObject messageReceiver;

	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		messageReceiver.SendMessage(messageToSend, SendMessageOptions.DontRequireReceiver);
	}

}
