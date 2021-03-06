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
	[SerializeField] public List<GameObject> moneyPrefabsM10;
	//explanation:
	//[0] = 1 money;
	//[1] = 10 money;
	//[2] = 100 money;
	//...

	[SerializeField] public GameObject moneyPrefab;
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

	public Backpack playerBackpack;
	public Transform workersHouse;

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

	[SerializeField]
	public List<Item> deb = new List<Item>();

	//[HideInInspector]
	public WorkerAI currentWorker;
	[SerializeField]
	GameObject workersUpgradeUI;
	[SerializeField]
	GameObject hireUI, upgradeUI;

	[SerializeField]
	public List<GameObject> itemPrefabs = new List<GameObject>();

	public Drill ironDrill;
	public HydraulicPress cogPress;
	public Drill copperDrill;
	public HydraulicPress copperCogPress;
	public CombiningMachine combineMashine;
	[Space(10)]
	public List<RepairStation> repairStations;
	public List<Transform> itemGrabSpots;
	public List<StashZone> stashZones = new List<StashZone>();

	[HideInInspector]
	public bool hasTransitioned;


	public void CloseWorkersUpgradeUI()
	{
		workersUpgradeUI.SetActive(false);
		if(currentWorker.isHired)
			currentWorker.stopped = false;
		currentWorker = null;
	}
	public void OpenWorkersUpgradeUI()
	{
		workersUpgradeUI.SetActive(true);
		currentWorker.stopped = true;
		if(currentWorker.isHired)
		{
			print("worker is hired");
			upgradeUI.SetActive(true);
			hireUI.SetActive(false);
		}
		else
		{
			print("worker is NOT hired");
			upgradeUI.SetActive(false);
			hireUI.SetActive(true);
		}
	}
	/*
	public void TriggerCurrentWorkersSpeedUpgrade()
	{
		currentWorker.SpeedUpgrade();
	}

	public void TriggerCurrentWorkersStorageUpgrade()
	{
		currentWorker.StorageUpgrade();
	}

	public void CloseHiredUI()
	{
		upgradeUI.SetActive(true);
		hireUI.SetActive(false);
	}
	public void TriggerCurrentWorkersHire()
	{
		currentWorker.Hire();
	}
	*/

	public void EnterNextLevel()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public static Item SpawnMoneyAtPosition(ref int amount, Vector3 spawnPosition)
	{
		for(int x = LevelSystem.instance.moneyPrefabsM10.Count -1; x >= 0; x--)
		{
			if((int)(amount / Mathf.Pow(10, x)) > 0)
			{
				amount -= (int)Mathf.Pow(10, x);
				return Instantiate(LevelSystem.instance.moneyPrefabsM10[x], spawnPosition, Quaternion.identity).GetComponent<Item>();
			}
		}
		return null;
	}

	public static List<Item> SpawnMoneyOverTime(int amount, Transform spawnPos, float time = 0)
	{
		List<Item> itemList = new List<Item>();


		for (int x = LevelSystem.instance.moneyPrefabsM10.Count - 1; x >= 0; x--)
		{
			int countToInstantiate = (int)(amount / Mathf.Pow(10, x));
			for (int p = 0; p < countToInstantiate; p++)
			{
				itemList.Add(new Item(LevelSystem.instance.moneyPrefabsM10[x].GetComponent<Item>())	);
				
				Debug.Log("added item to list");
			}
			amount -= countToInstantiate * (int)Mathf.Pow(10, x);
			print(".::III" + amount);
		}


		LevelSystem.instance.deb = itemList;

		Coroutine spawnCoroutine = LevelSystem.instance.StartCoroutine(SpawnMoneyOverTimeCoroutine(itemList, amount, time, spawnPos));
		Debug.Log($"started the coroutine. the item list has now {itemList.Count} items in it");
		return itemList;
	}

	public static IEnumerator SpawnMoneyOverTimeCoroutine(List<Item> itemList, int amount, float time, Transform spawnPos)
	{
		
		List<GameObject> objList = new List<GameObject>();
		//Calculate the correct money prefabs

		
		

		//Instantiate the money
		for (int i = 0; i < objList.Count; i++)
		{
			Item spawnedItem = Instantiate(itemList[i].gameObject, spawnPos.position, spawnPos.rotation).GetComponent<Item>();
			spawnedItem = itemList[i];
			yield return new WaitForSeconds(time / amount);
		}






		

		





		//Item item = new Item();
		//print(time);
		//ield return new WaitForEndOfFrame();
		//yield return item;
	}

	static void dasd(bool l = true)
	{
		
	}

	public void EnableWorkerHouse()
	{
		if(workerHouse != null)
		{
			PlayerPrefs.SetInt(workerHouse.savingKey + "p", PlayerPrefs.GetInt(workerHouse.savingKey + "p") == 0 ? 1 : PlayerPrefs.GetInt(workerHouse.savingKey + "p"));
			workerHouse.SendMessage("OnAddedPossibleLevel", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void ChangeCamera(int cameraIndex, int priority)
	{
		cameras[cameraIndex].Priority = priority;
	}

	private void Start()
    {
		if (OnMinigameFinish == null)
			OnMinigameFinish = new UnityEvent();

		LoadMoney();
		RefreshUI();
		ChangeCamera(0, 1);
		//TruckSystem();
	}
	private void Awake()
	{
		Application.targetFrameRate = 300;

		hasTransitioned = false;

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
		Debug.Log("Test");
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
		if(Input.GetKeyDown(KeyCode.R))
		{
			ResetGame();
		}
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
