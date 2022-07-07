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
		instance = this;
	}

	/*private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			LoadNextLevel();
		}
	}*/

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

		SceneManager.LoadScene(levelIndex);
	}
}
