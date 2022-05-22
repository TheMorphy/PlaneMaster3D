using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	[Header("UI")]
	[SerializeField]
	GameObject UpgradeUI;
	[SerializeField]
	GameObject upgradeReady, upgradeLocked;


	bool upgradeZoneLocked = false;
			
    void Start()
    {
        anim = GetComponent<Animator>();
		LoadVariables();
	}
    private void OnEnable()
    {
        dz = GetComponent<DroppingZone>();
        SetConditionsToLevel();
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
	}

	void LoadCorrectSpeed()
	{
		speed = speedStandard * Mathf.Pow(1 + 0.05f, speedLevel);
		systemMessageReceiver.SendMessage("OnLevelChanged");
	}


	public void UpgradeStorage()
	{
		storageLevel++;
		PlayerPrefs.SetInt(name + "storageLvl", storageLevel);
		LoadCorrectStorage();
	}
	void LoadCorrectStorage()
	{
		storage = (int)(storageStandard * Mathf.Pow(1 + 0.05f, storageLevel));
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
		level = PlayerPrefs.GetInt(savingKey + "c");
        

        if (level < PlayerPrefs.GetInt(savingKey + "p"))
        {
            readyForUpgrade = true;
            build.SetActive(true);
            GetComponent<DroppingZone>().enabled = true;
        }
        else
        {
            readyForUpgrade = false;
            build.SetActive(false);
            GetComponent<DroppingZone>().enabled = false;
        }
        if (level > 0)
            system.SetActive(true);
        system.transform.GetChild(0).SendMessage("OnLoadLevels", SendMessageOptions.DontRequireReceiver);
        /*
                if (level > 0)
                {
                    GetComponent<Animator>().enabled = false;
                    build.SetActive(false);
                    system.SetActive(true);
                    system.transform.GetChild(0).SendMessage("OnChangeLevel");
                }
                else
                {
                    GetComponent<DroppingZone>().enabled = true;
                    build.SetActive(true);
                    system.SetActive(false);
                }
        */
    }

    public void OnAddedPossibleLevel()
    {
        LoadVariables();
    }
    

    void OnAllConditionsComplete()
    {
        print("OnAllConditionsComplete");
        //Upgrade
        if (level == 0)
            anim.Play("Build");

        level = PlayerPrefs.GetInt(savingKey + "c") + 1;
        PlayerPrefs.SetInt(savingKey + "c", level);
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
