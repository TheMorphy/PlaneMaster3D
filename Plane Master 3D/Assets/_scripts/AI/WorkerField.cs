using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class WorkerField : MonoBehaviour
{
    public WorkerAI workerAI;
    [SerializeField]
    List<GameObject> disableOnBought = new List<GameObject>();
    [SerializeField]
    List<GameObject> enableWhenBought = new List<GameObject>();

    

    bool bought;

    public int price;
    public WorkersHireZone hireZone;

    public void ButtonClick()
	{
        if(bought)
		{
            Upgrade();
		}
        else
		{
            Buy();
		}
	}

    void Buy()
	{
        SetBought(true);
        workerAI = hireZone.SpawnNewWorker();
	}

    void Upgrade()
	{
	}

    public void SetBought(bool isBought)
	{
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
