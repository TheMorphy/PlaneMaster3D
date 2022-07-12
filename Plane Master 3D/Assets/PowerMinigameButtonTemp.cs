using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerMinigameButtonTemp : MonoBehaviour
{
	[SerializeField]
	GameObject anim, anim2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void PressButton()
	{
		anim.SetActive(true);
		anim2.GetComponent<Animator>().speed = 0;
	}
}
