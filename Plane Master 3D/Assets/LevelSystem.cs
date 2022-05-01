using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSystem : MonoBehaviour
{
    public static LevelSystem instance;

    #region public
    [SerializeField] GameObject levelUI, moneyUI, moneyUIFinal;
    [SerializeField] public int numberOfLevels, moneyToGet;
    [SerializeField] TextMeshProUGUI moneyNumber;
    #endregion

    #region private
    int money;
    #endregion

    private void Start()
    {
        if(LevelSystem.instance != null)
        {
            Debug.LogError("Multiple LevelSystems!!!");
        }
        else
        {
            instance = this;
        }


        moneyNumber = moneyUI.GetComponent<TextMeshProUGUI>();
        money = PlayerPrefs.GetInt("Money");
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
        moneyNumber.text = money.ToString() + "$";
    }
}
