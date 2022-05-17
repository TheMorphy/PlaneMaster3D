using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GoToNextHangar : MonoBehaviour
{

	[SerializeField] GameObject button;

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.layer == 8)
		{
			button.SetActive(true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 8)
		{
			button.SetActive(false);
		}
	}

	public void TransitionToNextHangar()
	{
		//SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		LevelSystem.instance.EnterNextLevel();
	}
}
