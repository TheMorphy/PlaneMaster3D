using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class WorkerField : MonoBehaviour
{
	public WorkerAI workerAI;

	[Space(10)]
	[Header("If is the buy button:")]
    [SerializeField]
    List<GameObject> disableOnBought = new List<GameObject>();
    [SerializeField]
    List<GameObject> enableWhenBought = new List<GameObject>();
    bool bought;
    int price;
	[SerializeField]
	TextMeshProUGUI priceText;
    public WorkersHireZone hireZone;
	[Space(10)]
	[SerializeField] GameObject[] upgradeButtonObjects;

	[Space(10)]
	[Header("If is an upgrade button:")]
	[SerializeField] TextMeshProUGUI capacityInfo, speedInfo;
	[SerializeField] bool isUpgradeButton, isSpeedButton;
	[SerializeField] TextMeshProUGUI levelText;
	public int speedLevel, backpackLevel;
	int maxSpeedLevel, maxBackpackLevel;
	int totalUpgrades;

	[SerializeField] public WorkerField[] workerFieldScripts;

	private void Start()
	{
		if (isUpgradeButton)
		{
			bought = true;
			maxBackpackLevel = 5;
			maxSpeedLevel = 5;
			//ChangeLevelUI();
		}

		//totalUpgrades = upgradeButtonObjects.Length;
		//workerFieldScripts = new WorkerField[totalUpgrades];

		/*for(int i = 0; i < workerFieldScripts.Length; i++)
		{
			workerFieldScripts[i] = upgradeButtonObjects[i].GetComponent<WorkerField>();
		}*/
	}

	//Not Using For Now
	public void SetPrice(int newPrice, int backpackValue, float speedValue)
	{
		price = newPrice;
		workerFieldScripts[0].price = newPrice;
		workerFieldScripts[1].price = newPrice;

		workerFieldScripts[0].priceText.text = price.ToString();
		workerFieldScripts[1].priceText.text = price.ToString();
		priceText.text = "$" + price.ToString();

		totalUpgrades = backpackLevel + speedLevel;

		workerFieldScripts[0].backpackLevel = backpackLevel;
		workerFieldScripts[1].backpackLevel = backpackLevel;
		workerFieldScripts[0].speedLevel = speedLevel;
		workerFieldScripts[1].speedLevel = speedLevel;



		workerFieldScripts[0].levelText.text = "Lvl " + backpackLevel.ToString();
		workerFieldScripts[1].levelText.text = "Lvl " + speedLevel.ToString();
		if(workerAI != null)
		{
			capacityInfo.text = "Backpack: " + backpackValue;

			speedInfo.text = "Speed: " + Mathf.CeilToInt((speedValue / hireZone.speedStandard) * 100) + "%";

		}
		if (backpackLevel >= 5)
		{
			workerFieldScripts[0].GetComponent<Button>().interactable = false;
		}
		if(speedLevel >= 5)
		{
			workerFieldScripts[1].GetComponent<Button>().interactable = false;
		}


		if (hireZone.savingKey == "Box02")
		{
			if (totalUpgrades >= 3)
			{
				switch (hireZone.WorkerFieldIndex(this))
				{
					case 1:
						QuestSystem.instance.AddProgress("Upgrade fifth worker to level 3", 1);

						break;
					case 2:
						QuestSystem.instance.AddProgress("Upgrade sixth worker to level 3", 1);
						break;
				}

			}
			if (totalUpgrades >= 5)
			{
				switch (hireZone.WorkerFieldIndex(this))
				{

					case 0:
						QuestSystem.instance.AddProgress("Upgrade fourth worker to level 5", 1);
						break;
					case 1:
						QuestSystem.instance.AddProgress("Upgrade fifth worker to level 5", 1);
						break;
					case 2:
						QuestSystem.instance.AddProgress("Upgrade sixth worker to level 5", 1);
						break;
				}
			}
		}
		else
		{
			if (totalUpgrades >= 5)
			{
				switch (hireZone.WorkerFieldIndex(this))
				{
					case 0:
						QuestSystem.instance.AddProgress("Upgrade first worker to level 5", 1);
						break;
					case 1:
						QuestSystem.instance.AddProgress("Upgrade second worker to level 5", 1);
						break;
					case 2:
						QuestSystem.instance.AddProgress("Upgrade third worker to level 5", 1);
						break;

				}
			}
		}

	}

	

    public void BuyButtonClick()
	{
		if (!bought /*|| workerAI.level < hireZone.maxLevel*/)
		{
			if (LevelSystem.instance.playerBackpack.TryPay(price, hireZone.transform.position))
			{
				Buy();
				/*if (bought)
				{
					Upgrade();
				}
				else
				{

				}*/
			}
		}
	}

	void ChangeLevelUI()
	{
		if (workerAI != null)
		{
			if (isSpeedButton)
			{
				levelText.text = "Lvl " + speedLevel;
			}
			else
				levelText.text = "Lvl " + backpackLevel;
		}
	}

	// Storage Button
	public void SpeedButtonClick()
	{
		if(speedLevel < maxSpeedLevel)
		{
			if (LevelSystem.instance.playerBackpack.TryPay(price, hireZone.transform.position))
			{
				//workerAI.SpeedUpgrade();
				
				speedLevel++;
				for (int i = 0; i < workerFieldScripts.Length; i++)
				{
					workerFieldScripts[i].speedLevel = speedLevel;

				}
				//ChangeLevelUI();
				if (speedLevel >= maxSpeedLevel)
				{
					gameObject.GetComponent<Button>().interactable = false;
				}
				Upgrade();
			}
		}
	}

	// Storage Button
	public void StorageButtonClick()
	{
		if (backpackLevel < maxBackpackLevel)
		{
			if (LevelSystem.instance.playerBackpack.TryPay(price, hireZone.transform.position))
			{
				//workerAI.StorageUpgrade();
				
				backpackLevel++;
				for(int i = 0; i < workerFieldScripts.Length; i++)
				{
					workerFieldScripts[i].backpackLevel = backpackLevel;
					
				}
				//ChangeLevelUI();
				if (backpackLevel >= maxBackpackLevel)
				{
					gameObject.GetComponent<Button>().interactable = false;
				}
				Upgrade();
				hireZone.SaveWorkers();
			}
		}
	}

	public void Buy()
	{
        SetBought(true);
        workerAI = hireZone.SpawnNewWorker();

		backpackLevel = 1;
		speedLevel = 1;

		for (int i = 0; i < workerFieldScripts.Length; i++)
		{
			workerFieldScripts[i].workerAI = workerAI;
			workerFieldScripts[i].backpackLevel = 1;
			workerFieldScripts[i].speedLevel = 1;
		}

		workerAI.backpack.savingKey = hireZone.savingKey + hireZone.WorkerFieldIndex(this);

		hireZone.SetTask(this);

		workerAI.repairStation = hireZone.savingKey == "Box01" ? LevelSystem.instance.repairStations[hireZone.WorkerFieldIndex(this)] : LevelSystem.instance.repairStations[hireZone.WorkerFieldIndex(this) + 3];
		workerAI.level = 1;
		
		hireZone.SaveWorkers();
		hireZone.ReloadStats();
	}

    void Upgrade()
	{
		//workerAI.level++;
		totalUpgrades = speedLevel + backpackLevel;
		print(workerAI.level);
		hireZone.SaveWorkers();
		hireZone.ReloadStats();
		
		
		
		
	}

    public void SetBought(bool isBought)
	{
		bought = isBought;
		print("Set Bought");
        if(isBought)
		{
            for(int i = 0; i < disableOnBought.Count; i++)
			{
                disableOnBought[i].SetActive(false);
			}
            for(int i = 0; i < enableWhenBought.Count; i++)
			{
                enableWhenBought[i].SetActive(true);
			}
		}
        else
		{
            for (int i = 0; i < disableOnBought.Count; i++)
            {
                disableOnBought[i].SetActive(true);
            }
            for (int i = 0; i < enableWhenBought.Count; i++)
            {
                enableWhenBought[i].SetActive(false);
            }
        }

	}
}
