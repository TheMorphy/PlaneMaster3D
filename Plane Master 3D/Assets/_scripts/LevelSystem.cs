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

	#region public
	[Space]
	[Header("Money System")]
	[SerializeField] GameObject moneyUI;
	[SerializeField] GameObject moneyUIFinal;
	[SerializeField] public int moneyToGet;
    [SerializeField] TextMeshProUGUI moneyNumber;
	[Space]
	[Header("Truck System")]
	[SerializeField] GameObject platformSummon;
	[SerializeField] GameObject summonTruckButton;
	#endregion

	#region private
	int money;
	TruckManager tmScript;
	PlaneMain pmScript;
	#endregion

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

	private void Start()
    {
		#region singleton
		instance = this;
		#endregion
		moneyNumber = moneyUI.GetComponent<TextMeshProUGUI>();
        money = PlayerPrefs.GetInt("money");
	}

	public void AddMoney()
    {
        money += moneyToGet;
    }

    public void SaveMoney()
    {
        PlayerPrefs.SetInt("Money", money);
    }

    public void UpdateMoney()
    {
        moneyNumber.text = money.ToString();
    }

	#region Truck System
	public void TriggerDetected(ChildScript childScript)
	{
		if(tmScript.Truck == null)
		{
			summonTruckButton.SetActive(true);
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
		if (pmScript.allIsRepaired && pmScript != null)
		{
			tmScript.ResetTruckManager();
			pmScript = null;
			Debug.Log("Reset Truck Manager");
		}
	}

	public void TruckSystem()
	{
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
