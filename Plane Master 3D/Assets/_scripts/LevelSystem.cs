using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

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

	[Space(10)]
	[Header("Truck System")]
	[SerializeField] GameObject platformSummon;
	[SerializeField] GameObject summonTruckButton;

	TruckManager tmScript;
	PlaneMain pmScript;

	bool truckSystemCalled = false;

	private void Start()
    {
		#region singleton
		instance = this;
		#endregion
		LoadMoney();
		RefreshUI();
    }
	private void Awake()
	{
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

	public void PlayClickSound()
	{
		clickSource.PlayOneShot(clickSound);
	}
	public void PlayReleaseSound()
	{
		clickSource.PlayOneShot(releaseSound);
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

	#region Truck System
	public void TriggerDetected(ChildScript childScript)
	{
		if (tmScript.Truck == null && !truckSystemCalled)
		{
			TruckSystem();
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
			tmScript = FindObjectOfType<TruckManager>();
			summonTruckButton.SetActive(false);
		}
	}
	#endregion

}
