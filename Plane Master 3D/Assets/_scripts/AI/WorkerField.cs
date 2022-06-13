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

	public void SetPrice(int newPrice)
	{
		price = newPrice;
		priceText.text = "$" + price.ToString();
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
