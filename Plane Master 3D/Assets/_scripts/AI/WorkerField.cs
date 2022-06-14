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
	TextMeshProUGUI priceText, levelText;
    public WorkersHireZone hireZone;

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

    public void ButtonClick()
	{
		if (!bought || workerAI.level < hireZone.maxLevel)
		{
			if (LevelSystem.instance.playerBackpack.TryPay(price, hireZone.transform.position))
			{
				if (bought)
				{
					Upgrade();
				}
				else
				{
					Buy();
				}
			}
		}
	}

    void Buy()
	{
        SetBought(true);
        workerAI = hireZone.SpawnNewWorker();
		workerAI.level = 1;
		hireZone.SaveWorkers();
		hireZone.ReloadStats();
	}

    void Upgrade()
	{
		
		
			workerAI.level++;
			hireZone.SaveWorkers();
			hireZone.ReloadStats();
		
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
                case 3:
                    QuestSystem.instance.AddProgress("Upgrade fourth worker to level 5", 1);
                    break;
                case 4:
					QuestSystem.instance.AddProgress("Upgrade fifth worker to level 5", 1);
					break;
                case 5:
                    QuestSystem.instance.AddProgress("Upgrade sixth worker to level 5", 1);
                    break;
            }
        }
		else if(workerAI.level >= 3)
		{
			switch (hireZone.WorkerFieldIndex(this))
			{
				case 4:
					QuestSystem.instance.AddProgress("Upgrade fifth worker to level 3", 1);

					break;
				case 5:
					QuestSystem.instance.AddProgress("Upgrade sixth worker to level 3", 1);
					break;
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
