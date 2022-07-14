using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
	[SerializeField] Animator transition;

	[SerializeField] public float transitionTime = 1f;

	[SerializeField] bool isFirstScene;

	#region singleton
	public static LevelLoader instance;
	#endregion

	private void Start()
	{

		if (instance == null)
		{
			instance = this;


			/*	if (isFirstScene)
				{
					if (SceneManager.GetActiveScene().buildIndex != PlayerPrefs.GetInt("currentLevel") && PlayerPrefs.GetInt("currentLevel") > 0)
					{
						SceneManager.LoadScene(PlayerPrefs.GetInt("currentLevel"));
					}
				}*/
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
