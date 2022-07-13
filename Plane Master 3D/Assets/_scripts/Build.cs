using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
	[SerializeField]
	int storageIncrement;

	[SerializeField]
	List<int> levelCost = new List<int>();

	[Space(10)]

	[SerializeField]
	int speedLevel;
	[SerializeField]
	int maxSpeedLevel;
	[SerializeField]
	float speedStandard;
	[SerializeField]
	float speedIncrement;

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
	[SerializeField] Button storageButton, speedButton;

	Backpack playerBackpack;
	bool upgradeZoneLocked = false;
	bool UIActivated;
			
    void Start()
    {
        anim = GetComponent<Animator>();
		

		speedLevel = PlayerPrefs.GetInt(savingKey + "speedLvl");
		storageLevel = PlayerPrefs.GetInt(savingKey + "storageLvl");
		if (speedLevel < 1)
			speedLevel = 1;
		if (storageLevel < 1)
			storageLevel = 1;

		LoadCorrectSpeed();
		LoadCorrectStorage();

		speedUpgradePrize = costStart + costIncrement * speedLevel;
		storageUpgradePrize = costStart + costIncrement * storageLevel;

		speedUpgradePrize = levelCost[Mathf.Clamp(speedLevel, 0, levelCost.Count - 1)];
		storageUpgradePrize = levelCost[Mathf.Clamp(storageLevel, 0, levelCost.Count - 1)];

        UpdateUpgradeUI();

		if(savingKey == "Press02")
		{
			QuestSystem.instance.AddProgress("Build wire press", 1);
		}
		CheckForOutOfLevels();

		LoadVariables();
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

		if (storageLevel >= levelCost.Count)
		{
			storageButton.interactable = false;
		}

		if (speedLevel >= levelCost.Count)
		{
			speedButton.interactable = false;
		}
	}

    void CheckForOutOfLevels()
	{
		if(savingKey == "Drill")
		{
			
			if (storageLevel + speedLevel >= 2)
				QuestSystem.instance.AddProgress("Upgrade iron drill", 1);
			if (storageLevel + speedLevel >= 3)
				QuestSystem.instance.AddProgress("Upgrade iron drill to level 3", 1);
			if (storageLevel + speedLevel >= 4)
				QuestSystem.instance.AddProgress("Upgrade iron drill to level 4", 1);
			if (storageLevel + speedLevel >= 5)
				QuestSystem.instance.AddProgress("Upgrade iron drill to level 5", 1);
			if (storageLevel + speedLevel >= 6)
				QuestSystem.instance.AddProgress("Upgrade iron drill to level 6", 1);

		}
		else if(savingKey =="Drill02")
		{
			if (storageLevel + speedLevel >= 2)
				QuestSystem.instance.AddProgress("Upgrade copper drill", 1);
			if (storageLevel + speedLevel >= 3)
				QuestSystem.instance.AddProgress("Upgrade copper drill to level 3", 1);
			if (storageLevel + speedLevel >= 4)
				QuestSystem.instance.AddProgress("Upgrade copper drill to level 4", 1);
			if (storageLevel + speedLevel >= 5)
				QuestSystem.instance.AddProgress("Upgrade copper drill to level 5", 1);
			if (storageLevel + speedLevel >= 6)
				QuestSystem.instance.AddProgress("Upgrade copper drill to level 6", 1);

		}
		else if(savingKey == "Press")
		{
			if (storageLevel + speedLevel >= 2)
				QuestSystem.instance.AddProgress("Upgrade gear press", 1);
			if (storageLevel + speedLevel >= 3)
				QuestSystem.instance.AddProgress("Upgrade gear press to level 3", 1);
			if (storageLevel + speedLevel >= 4)
				QuestSystem.instance.AddProgress("Upgrade gear press to level 4", 1);
			if (storageLevel + speedLevel >= 5)
				QuestSystem.instance.AddProgress("Upgrade gear press to level 5", 1);

		}
		else if(savingKey == "Press02")
		{
			if (storageLevel + speedLevel >= 2)
				QuestSystem.instance.AddProgress("Upgrade copper wire press", 1);
			if (storageLevel + speedLevel >= 3)
				QuestSystem.instance.AddProgress("Upgrade gear press to level 3", 1);
			if (storageLevel + speedLevel >= 4)
				QuestSystem.instance.AddProgress("Upgrade gear press to level 4", 1);

		}


		
		


	}

	public void CloseUpgradeUI()
	{
		UpgradeUI.SetActive(false);
	}




	public void UpgradeSpeed()
	{
		if (speedLevel < levelCost.Count)
		{
			if (LevelSystem.instance.playerBackpack.TryPay(speedUpgradePrize, transform.position))
			{
				speedLevel++;
				PlayerPrefs.SetInt(savingKey + "speedLvl", speedLevel);
				LoadCorrectSpeed();
				UpdateUpgradeUI();
				CheckForOutOfLevels();



			}
		}
		
		
    }

    void LoadCorrectSpeed()
	{
		speed = speedStandard + (speedLevel - 1) * speedIncrement;
		speedUpgradePrize = costStart + costIncrement * (speedLevel - 1);
		
		speedUpgradePrize = levelCost[Mathf.Clamp(speedLevel, 0, levelCost.Count - 1)];

        systemMessageReceiver.SendMessage("OnLevelChanged", SendMessageOptions.DontRequireReceiver);
	}


	public void UpgradeStorage()
	{
		if(storageLevel < levelCost.Count)
		{
			if (LevelSystem.instance.playerBackpack.TryPay(storageUpgradePrize, transform.position))
			{
				storageLevel++;
				PlayerPrefs.SetInt(savingKey + "storageLvl", storageLevel);
				LoadCorrectStorage();
				UpdateUpgradeUI();
				CheckForOutOfLevels();
				QuestSystem.instance.AddProgress("Upgrade iron drill", 1);
			}
		}
		
    }
	void LoadCorrectStorage()
	{
		storage = Mathf.CeilToInt(storageStandard + (storageLevel -1) * storageIncrement);
		storageUpgradePrize = costStart + costIncrement * storageLevel;
		storageUpgradePrize = levelCost[Mathf.Clamp(storageLevel, 0, levelCost.Count - 1)];

		systemMessageReceiver.SendMessage("OnLevelChanged", SendMessageOptions.DontRequireReceiver);
	}

	void OnUpgradeZoneTriggered()
	{
		if(!upgradeZoneLocked)
		{
			UIActivated = false;
		}
	}

	void OnUpgradeZoneStay(Collider collider)
	{

		if(!collider.GetComponent<Player>().isMoving && !UIActivated)
		{
			UpgradeUI.SetActive(true);
			UIActivated = true;
		}
	}

    void LoadVariables()
    {
        
		//print("load vars");
		//print("load vars 2 ");
		level = PlayerPrefs.GetInt(savingKey + "p");
        

        
   //     if (level > 1)
        //    system.SetActive(true);
        system.transform.GetChild(0).SendMessage("OnLoadLevels", SendMessageOptions.DontRequireReceiver);
        
                if (level >= 1)
                {
                    GetComponent<Animator>().enabled = false;
					GetComponent<DroppingZone>().enabled = false;
					build.SetActive(false);
                    system.SetActive(true);
				if(upgradeReady != null)
					upgradeReady.SetActive(true);
				system.transform.GetChild(0).SendMessage("OnChangeLevel");
				}
                else
                {
                    GetComponent<DroppingZone>().enabled = true;
                    build.SetActive(true);
					//upgradeReady.SetActive(false);
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
		dz.enabled = false;
    }

    void SetConditionsToLevel()
    {
        //dz.conditions = levels[level].conditions;
    }
    public void SwitchToSystem() 
    {
        build.SetActive(false);
        system.SetActive(true);
		if (soundSource != null)
			soundSource.PlayOneShot(buildSound);
		upgradeReady.SetActive(true);
	}
}
