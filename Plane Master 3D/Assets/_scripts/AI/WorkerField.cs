using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class WorkerField : MonoBehaviour
{
    public WorkerAI workerAI;
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
	[Header("If is the buy button:")]
	[SerializeField] GameObject[] upgradeButtonObjects;
	[Space(10)]
	[Header ("If is an upgrade button:")]
	[SerializeField] bool isUpgradeButton;
	[SerializeField] TextMeshProUGUI levelText;
	int speedLevel, backpackLevel;
	int maxSpeedLevel, maxBackpackLevel;

	WorkerField workerFieldScript;

	private void Start()
	{
		if (isUpgradeButton)
		{
			bought = true;
			maxBackpackLevel = 5;
			maxSpeedLevel = 5;
		}

		for (int i = 0; i < upgradeButtonObjects.Length; i++)
		{
			workerFieldScript = upgradeButtonObjects[i].GetComponent<WorkerField>();
		}
	}

	public void SetPrice(int newPrice)
	{
		price = newPrice;
		priceText.text = "$" + price.ToString();
		if(workerAI != null)
		{
			levelText.text = "Lvl " + workerAI.level.ToString();
			if (workerAI.level >= 5)
			{
				GetComponent<Button>().interactable = false;
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

	// Storage Button
	void StorageButtonClick()
	{
		if(speedLevel < maxSpeedLevel)
		{
			if (LevelSystem.instance.playerBackpack.TryPay(price, hireZone.transform.position))
			{
				Upgrade();
			}
		}
	}

	// Storage Button
	void SpeedButtonClick()
	{
		if (backpackLevel < maxBackpackLevel)
		{
			if (LevelSystem.instance.playerBackpack.TryPay(price, hireZone.transform.position))
			{
				Upgrade();
			}
		}
	}

	void Buy()
	{
        SetBought(true);
        workerAI = hireZone.SpawnNewWorker();
		workerFieldScript.workerAI = workerAI;

		workerAI.backpack.savingKey = hireZone.savingKey + hireZone.WorkerFieldIndex(this);

		hireZone.SetTask(this);

		workerAI.repairStation = hireZone.savingKey == "Box01" ? LevelSystem.instance.repairStations[hireZone.WorkerFieldIndex(this)] : LevelSystem.instance.repairStations[hireZone.WorkerFieldIndex(this) + 3];
		workerAI.level = 1;
		hireZone.SaveWorkers();
		//hireZone.ReloadStats();
	}

    void Upgrade()
	{
		workerAI.level++;
		print(workerAI.level);
		hireZone.SaveWorkers();
		//hireZone.ReloadStats();
		
		
		if(hireZone.savingKey == "Box02")
		{
			if (workerAI.level >= 3)
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
			if (workerAI.level >= 5)
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
			if (workerAI.level >= 5)
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
