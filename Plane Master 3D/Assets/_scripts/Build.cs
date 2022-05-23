using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Build : MonoBehaviour
{
    DroppingZone dz;
    Animator anim;
    [SerializeField]
    GameObject build, system;
    public int level;
    public string savingKey;
    [SerializeField]
    List<BuildLevel> levels = new List<BuildLevel>();
    bool readyForUpgrade;
	[SerializeField]
	AudioSource soundSource;
	[SerializeField]
	AudioClip buildSound;


	[SerializeField]
	GameObject systemMessageReceiver;


	[SerializeField]
	public int storage;
	[SerializeField]
	public float speed;


	[Header("Levels")]
	[SerializeField]
	int storageLevel;
	[SerializeField]
	int maxStorageLevel;
	[SerializeField]
	int storageStandard;

	[Space(10)]

	[SerializeField]
	int speedLevel;
	[SerializeField]
	int maxSpeedLevel;
	[SerializeField]
	float speedStandard;

    [Space(10)]
    [SerializeField]
    int costStart;
    [SerializeField]
    int costIncrement;

    [SerializeField]
    int speedUpgradePrize, storageUpgradePrize;

	[Header("UI")]
	[SerializeField]
	GameObject UpgradeUI;
	[SerializeField]
	GameObject upgradeReady, upgradeLocked;
    [SerializeField]
    TextMeshProUGUI currentSpeedInfo, currentStorageInfo;
    [SerializeField]
    TextMeshProUGUI currentSpeedLevelInfo, currentStorageLevelInfo;
    [SerializeField]
    TextMeshProUGUI storageUpgradeCost, speedUpgradeCost;


	bool upgradeZoneLocked = false;
			
    void Start()
    {
        anim = GetComponent<Animator>();
		LoadVariables();

		speedLevel = PlayerPrefs.GetInt(name + "speedLvl");
		storageLevel = PlayerPrefs.GetInt(name + "storageLvl");

		LoadCorrectSpeed();
		LoadCorrectStorage();

		speedUpgradePrize = costStart + costIncrement * speedLevel;
        storageUpgradePrize = costStart + costIncrement * storageLevel;

        UpdateUpgradeUI();
		

	}
	private void OnEnable()
    {
        dz = GetComponent<DroppingZone>();
        SetConditionsToLevel();
    }

    void UpdateUpgradeUI()
	{
        currentSpeedLevelInfo.text = "LVL " + speedLevel;
        currentStorageLevelInfo.text = "LVL " + storageLevel;

        currentSpeedInfo.text = "Speed " + (speed * 60).ToString("0.00") + $"/min (+{ (speed * 0.05f).ToString("0.00")})";
        currentStorageInfo.text = "Capacity " + storage.ToString() + $"(+{ Mathf.CeilToInt(storage * 0.05f)})";

        storageUpgradeCost.text = storageUpgradePrize.ToString();
        speedUpgradeCost.text = speedUpgradePrize.ToString();
	}

    void CheckForOutOfLevels()
	{
		if(level < 2)
		{
			CloseUpgradeUI();
			upgradeZoneLocked = true;
			upgradeReady.SetActive(false);
			upgradeLocked.SetActive(false);
		}
		else
		{
			if (speedLevel + storageLevel >= PlayerPrefs.GetInt(savingKey + "p"))
			{
				CloseUpgradeUI();
				upgradeZoneLocked = true;
				upgradeReady.SetActive(false);
				upgradeLocked.SetActive(true);
			}
			else
			{
				upgradeZoneLocked = false;
				upgradeReady.SetActive(true);
				upgradeLocked.SetActive(false);
			}
		}
        
	}

	public void CloseUpgradeUI()
	{
		UpgradeUI.SetActive(false);
	}




	public void UpgradeSpeed()
	{
		speedLevel++;
		PlayerPrefs.SetInt(name + "speedLvl", speedLevel);
		LoadCorrectSpeed();
        UpdateUpgradeUI();
        CheckForOutOfLevels();
    }

    void LoadCorrectSpeed()
	{
		speed = speedStandard * Mathf.Pow(1 + 0.05f, speedLevel);
        speedUpgradePrize = costStart + costIncrement * speedLevel;

        systemMessageReceiver.SendMessage("OnLevelChanged");
	}


	public void UpgradeStorage()
	{
		storageLevel++;
		PlayerPrefs.SetInt(name + "storageLvl", storageLevel);
		LoadCorrectStorage();
        UpdateUpgradeUI();
        CheckForOutOfLevels();
    }
	void LoadCorrectStorage()
	{
		storage = (int)(storageStandard * Mathf.Pow(1 + 0.05f, storageLevel));
        storageUpgradePrize = costStart + costIncrement * storageLevel;

        systemMessageReceiver.SendMessage("OnLevelChanged");
	}

	void OnUpgradeZoneTriggered()
	{
		if(!upgradeZoneLocked)
		{
			//Open Upgrade UI
			UpgradeUI.SetActive(true);
		}
	}

    void LoadVariables()
    {
        
		//print("load vars");
		//print("load vars 2 ");
		level = PlayerPrefs.GetInt(savingKey + "p");
        

        if (level < PlayerPrefs.GetInt(savingKey + "p"))
        {
            //readyForUpgrade = true;
            //build.SetActive(true);
            GetComponent<DroppingZone>().enabled = true;
        }
        else
        {
            //readyForUpgrade = false;
            build.SetActive(false);
            GetComponent<DroppingZone>().enabled = false;
        }
   //     if (level > 1)
        //    system.SetActive(true);
        system.transform.GetChild(0).SendMessage("OnLoadLevels", SendMessageOptions.DontRequireReceiver);
        
                if (level > 1)
                {
                    GetComponent<Animator>().enabled = false;
                    build.SetActive(false);
                    system.SetActive(true);
                    //system.transform.GetChild(0).SendMessage("OnChangeLevel");
                }
                else
                {
                    GetComponent<DroppingZone>().enabled = true;
                    build.SetActive(true);
                    system.SetActive(false);
                }

		CheckForOutOfLevels();
        
    }

    public void OnAddedPossibleLevel()
    {
        LoadVariables();
		print("level added");
    }
    

    void OnAllConditionsComplete()
    {
        print("OnAllConditionsComplete");
        //Upgrade
        if (level == 0)
            anim.Play("Build");

        level = PlayerPrefs.GetInt(savingKey + "p") + 1;
        PlayerPrefs.SetInt(savingKey + "p", level);
        LoadVariables();
        
        SetConditionsToLevel();
        dz.ResetConditions();
    }

    void SetConditionsToLevel()
    {
        dz.conditions = levels[level].conditions;
    }
    public void SwitchToSystem() 
    {
        build.SetActive(false);
        system.SetActive(true);
		if (soundSource != null)
			soundSource.PlayOneShot(buildSound);
    }
}
