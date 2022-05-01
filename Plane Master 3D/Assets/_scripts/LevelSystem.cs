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
	[SerializeField] GameObject moneyUI, moneyUIFinal;
    [SerializeField] public int moneyToGet;
    [SerializeField] TextMeshProUGUI moneyNumber;
    #endregion

    #region private
    int money;
    #endregion

    private void Start()
    {
		#region singleton
		instance = this;
		#endregion
		moneyNumber = moneyUI.GetComponent<TextMeshProUGUI>();
        money = PlayerPrefs.GetInt("money");
		UpdateMoney();
    }

    public void AddMoney()
    {
        money += moneyToGet;
		moneyToGet = 0;
		SaveMoney();
		UpdateMoney();
    }

    public void SaveMoney()
    {
        PlayerPrefs.SetInt("Money", money);
    }

    public void UpdateMoney()
    {
        moneyNumber.text = money.ToString();
    }
}
