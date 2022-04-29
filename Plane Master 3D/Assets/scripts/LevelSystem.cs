using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSystem : MonoBehaviour
{
    #region public
    [SerializeField] GameObject levelUI, moneyUI, moneyUIFinal;
    [SerializeField] int moneyToGet;
    [SerializeField] TextMeshProUGUI moneyNumber;
    #endregion

    #region private
    int money;
    #endregion

    private void Start()
    {
        moneyNumber = moneyUI.GetComponent<TextMeshProUGUI>();
        money = PlayerPrefs.GetInt("money");
    }

    void AddMoney()
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
}
