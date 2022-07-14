using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class LoadingScene : MonoBehaviour
{
	[SerializeField]
	Image progressBar;
	[SerializeField]
	TextMeshProUGUI percentage;
    // Start is called before the first frame update
    void Start()
    {
		LoadCorrectLevel();

	}

    void LoadCorrectLevel()
	{
		StartCoroutine(LoadCorrectScene());
	}

	IEnumerator LoadCorrectScene()
	{
		int sceneIndex = PlayerPrefs.GetInt("currentLevel");
		if (sceneIndex == 0)
			sceneIndex = 1;
		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

		while(!operation.isDone)
		{
			if(progressBar != null)
				progressBar.fillAmount = operation.progress * 1.1f;
			percentage.text = (Mathf.RoundToInt(operation.progress * 100)).ToString() + "%";
			
			Debug.Log(operation.progress);
			yield return null;
		}

	}
}
