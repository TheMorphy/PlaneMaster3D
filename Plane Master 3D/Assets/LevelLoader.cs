using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
	[SerializeField] Animator transition;

	[SerializeField] public float transitionTime = 1f;

	#region singleton
	public static LevelLoader instance;
	#endregion

	private void Start()
	{
		Debug.Log("start");

		if (instance == null)
		{
			instance = this;
			//if (SceneManager.GetActiveScene().buildIndex != PlayerPrefs.GetInt("currentLevel"))
			//{
				//SceneManager.LoadScene(PlayerPrefs.GetInt("currentLevel"));
			//}	
		}
		else
			Destroy(gameObject);

	}

	public void LoadNextLevel()
	{
		Debug.Log("Load Next Level");
		StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
	}

	IEnumerator LoadLevel(int levelIndex)
	{
		transition.SetTrigger("Start");

		PlayerPrefs.DeleteAll();

		yield return new WaitForSeconds(transitionTime);

		PlayerPrefs.SetInt("currentLevel", SceneManager.GetActiveScene().buildIndex + 1);

		SceneManager.LoadScene(levelIndex);
	}
}
