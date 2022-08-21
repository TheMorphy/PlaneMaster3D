using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingLights : MonoBehaviour
{
	[SerializeField]
	List<MeshRenderer> lights = new List<MeshRenderer>();
	[SerializeField]
	Color red, green, off;
	Coroutine blinking;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void TurnGreen()
	{
		if (blinking != null)
			StopCoroutine(blinking);
		for(int i = 0; i < lights.Count; i++)
		{
			lights[i].material.color = green;
		}
	}

	public void TurnRed()
	{
		if (blinking != null)
			StopCoroutine(blinking);
		for (int i = 0; i < lights.Count; i++)
		{
			lights[i].material.color = red;
		}

	}

	public void Blink()
	{
		blinking = StartCoroutine(Blinking());
	}

	IEnumerator Blinking()
	{
		float t = 0;
		while(true)
		{
			for (int i = 0; i < lights.Count; i++)
			{
				lights[i].material.color = off;
			}

			yield return new WaitForSeconds(0.15f);
			for (int i = 0; i < lights.Count; i++)
			{
				lights[i].material.color = red;
			}
			yield return new WaitForSeconds(0.3f);
		}
	}
}
