using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalTrigger : MonoBehaviour
{

	[SerializeField]
	string messageToSend;
	[SerializeField]
	string stayMessage;
	[SerializeField]
	GameObject messageReceiver;
	[SerializeField]
	float outlineAlpha = 0.43f;
	[SerializeField]
	List<Outline> outlines;
	[SerializeField]
	AnimationCurve outlineFadeInCurve;
	Coroutine outlineCoroutine = null;

	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			messageReceiver.SendMessage(messageToSend, other, SendMessageOptions.DontRequireReceiver);
			if (outlineCoroutine != null)
				StopCoroutine(outlineCoroutine);
			outlineCoroutine = StartCoroutine(ActivateOutlines());
		}
		

	}

	void OnTriggerStay(Collider other)
	{
		if (other.CompareTag("Player"))
			messageReceiver.SendMessage(stayMessage, other, SendMessageOptions.DontRequireReceiver);
	}
	void OnTriggerExit(Collider other)
	{
		if (outlineCoroutine != null)
			StopCoroutine(outlineCoroutine);
		outlineCoroutine = StartCoroutine(DeactivateOutlines());
	}

	IEnumerator ActivateOutlines()
	{
		float t = 0;
		while(t < 1)
		{
			t += Time.deltaTime;
			for(int i = 0; i < outlines.Count; i++)
			{
				outlines[i].OutlineColor = new Color(outlines[i].OutlineColor.r, outlines[i].OutlineColor.g, outlines[i].OutlineColor.b, Mathf.Lerp(outlines[i].OutlineColor.a, outlineAlpha, outlineFadeInCurve.Evaluate(t)));
				yield return null;
			}
		}
	}

	IEnumerator DeactivateOutlines()
	{
		float t = 0;
		while (t < 1)
		{
			t += Time.deltaTime;
			for (int i = 0; i < outlines.Count; i++)
			{
				outlines[i].OutlineColor = new Color(outlines[i].OutlineColor.r, outlines[i].OutlineColor.g, outlines[i].OutlineColor.b, Mathf.Lerp(outlines[i].OutlineColor.a, 0, outlineFadeInCurve.Evaluate(t)));
				yield return null;
			}
		}
	}

}
