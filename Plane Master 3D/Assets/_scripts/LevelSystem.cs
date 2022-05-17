using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Cinemachine;
using UnityEngine.Events;

public class LevelSystem : MonoBehaviour
{
	#region singleton
	public static LevelSystem instance;
	#endregion

   // [SerializeField] public int moneyToGet;
    [SerializeField] TextMeshProUGUI moneyNumber;
	[SerializeField] List<GameObject> moneyPrefabsM10;
	//explanation:
	//[0] = 1 money;
	//[1] = 10 money;
	//[2] = 100 money;
	//...

	[SerializeField] GameObject moneyPrefab;
	[SerializeField] Transform moneyLerpPos;

	[SerializeField]
	Transform moneyParent;

	[SerializeField]
	[Range(0.01f, .7f)]
	float minSmooth = 0.1f, maxSmooth = 0.15f;

	[SerializeField]
	List<AudioClip> moneyCollectSounds;
	[SerializeField]
	AudioSource moneyCollectSource;

	[Space(10)]

	[SerializeField]
	AudioClip clickSound, releaseSound;
	[SerializeField]
	AudioSource clickSource;

    int money;
	int displayMoney;

	public GameObject player;

	[SerializeField] Build workerHouse;

	
	
	

	[Header("Cameras")]
	[SerializeField]
	List<CinemachineVirtualCamera> cameras; // 0 = TopDownCam, 1 = HangarCam, ...


	[Space(10)]
	[Header("Truck System")]
	[SerializeField] GameObject platformSummon;
	[SerializeField] GameObject summonTruckButton;

	TruckManager tmScript;
	PlaneMain pmScript;
	[Header("Minigames")]
	[SerializeField] List<GameObject> minigameObjects = new List<GameObject>();
	/// <summary>
	/// 0 = align
	/// 1 = sliders
	/// 2 = refuel
	/// </summary>

	
	public UnityEvent OnMinigameFinish;
	bool truckSystemCalled = false;

	public void EnterNextLevel()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void EnableWorkerHouse()
	{
		if(workerHouse != null)
		{
			PlayerPrefs.SetInt(workerHouse.savingKey + "p", PlayerPrefs.GetInt(workerHouse.savingKey + "p") == 0 ? 1 : PlayerPrefs.GetInt(workerHouse.savingKey + "p"));
			workerHouse.SendMessage("OnAddedPossibleLevel");
		}
	}

	public void ChangeCamera(int cameraIndex)
	{
		for(int i = 0; i < cameras.Count; i++)
		{
			if (i == cameraIndex)
				cameras[i].Priority = 1;
			else
				cameras[i].Priority = 0;
		}
	}

	private void Start()
    {
		if (OnMinigameFinish == null)
			OnMinigameFinish = new UnityEvent();

		LoadMoney();
		RefreshUI();
		ChangeCamera(0);
		//TruckSystem();
	}
	private void Awake()
	{
		#region singleton
		instance = this;
		#endregion
		#region Get The Managers
		tmScript = FindObjectOfType<TruckManager>();
		if (tmScript == null)
		{
			Debug.Log("There is no Truck manager in the scene");
		}
		#endregion
	}

	//This is the most important function of this script
	public void AddMoney(int moneyToAdd, Vector3 moneySpawnPosition)
    {
        money += moneyToAdd;
		SaveMoney();
		RefreshUI();
			for(int x = moneyPrefabsM10.Count - 1; x >= 0; x--)
			{
				int countToInstantiate = (int)(moneyToAdd / Mathf.Pow(10, x));
				for(int p = 0; p < countToInstantiate; p++)
				{
					StartCoroutine(LerpMoney(moneySpawnPosition, moneyPrefabsM10[x], (int)Mathf.Pow(10, x)));
				}
				moneyToAdd -= countToInstantiate * (int)Mathf.Pow(10, x);
				print(moneyToAdd);
			}
	}

	IEnumerator LerpMoney(Vector3 moneySpawnPosition, GameObject prefab, int moneyValue)
	{
		Transform moneyToLerp = Instantiate(prefab, moneySpawnPosition, Quaternion.identity, moneyParent).transform;
		float smooth = Random.Range(minSmooth, maxSmooth);
		do
		{
			moneyToLerp.position = Vector3.Lerp(moneyToLerp.position, moneyLerpPos.position, smooth);
			yield return null;
		} while (Vector3.Distance(moneyToLerp.transform.position, moneyLerpPos.position) > 0.1f);
		Destroy(moneyToLerp.gameObject);
		//Play money_collect sound
		moneyCollectSource.PlayOneShot(moneyCollectSounds[Random.Range(0, moneyCollectSounds.Count)]);
		displayMoney += moneyValue;
		RefreshUI();
	}

	public void PlayClickSound(Button button)
	{
		clickSource.PlayOneShot(clickSound);
		
	}

	public void ResetGame()
	{
		PlayerPrefs.DeleteAll();
		SceneManager.LoadScene(0);
		Time.timeScale = 1;
	}
	
	void SaveMoney()
    {
        PlayerPrefs.SetInt("Money", money);
    }

	void LoadMoney()
	{
		money = PlayerPrefs.GetInt("Money");
		displayMoney = money;
	}

	void RefreshUI()
    {
        moneyNumber.text = displayMoney.ToString();
    }

	public void ControlPlayer(Vector3 destination, bool destroyWhenReached)
	{
		player.GetComponentInChildren<PlayerAI>().ActivatePlayerAI(destination, destroyWhenReached);
	}


	public void PlayMinigame(int specification = -1)
	{
		if(specification < 0)
		{
			specification = Random.Range(0, minigameObjects.Count);
		}
		minigameObjects[specification].SetActive(true);
		StartCoroutine(WaitForMinigameDone(minigameObjects[specification]));
	}
	IEnumerator WaitForMinigameDone(GameObject minigame)
	{
		//player.GetComponentInChildren<PlayerAI>().PausePlayerAI();
		yield return new WaitWhile(() => minigame.activeSelf);
		//OnMinigameDone
		OnMinigameFinish.Invoke();
		//player.GetComponentInChildren<PlayerAI>().ResumePlayerAI();
	}

	#region Truck System
	public void TriggerDetected(ChildScript childScript)
	{
		if (tmScript.Truck == null && !truckSystemCalled)
		{
			//TruckSystem();
		}
	}
	public void TriggerExit(ChildScript childScript)
	{
		if (tmScript.Truck == null)
		{
			summonTruckButton.SetActive(false);
		}
	}

	private void Update()
	{
		//if(pmScript != null)
		if (pmScript != null && pmScript.allIsRepaired)
		{
			tmScript.ResetTruckManager();
			pmScript = null;
			Debug.Log("Reset Truck Manager");
		}
	}

	public void TruckSystem()
	{
		truckSystemCalled = true;
		if (tmScript.Truck == null)
		{
			tmScript.SummonTruck();
			pmScript = FindObjectOfType<PlaneMain>();
			
			tmScript = FindObjectOfType<TruckManager>();  //Why? xD
			summonTruckButton.SetActive(false);
		}
	}
	#endregion

}
