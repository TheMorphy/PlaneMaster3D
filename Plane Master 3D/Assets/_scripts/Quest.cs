using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    [Space(10)]
    public string questName;
    public int progress;
    public int maxProgess;
    
    public bool done;
    
    public List<Reward> rewards = new List<Reward>();
    
}

[System.Serializable]
public class Reward
{
    public enum RewardType { Money }
    public RewardType rewardType;
    public int count;
    
    public void Collect()
    {
        switch(rewardType)
        {
            case RewardType.Money:
                Debug.Log("collected some money: " + count);
                LevelSystem.instance.moneyToGet = count;
                LevelSystem.instance.AddMoney();
                break;
        }
    }
}