using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class QuestSystem : MonoBehaviour
{
    [HideInInspector]
    public static QuestSystem instance;


    [SerializeField]
    List<Quest> quests = new List<Quest>();
    Quest currentQuest;


    [SerializeField]
    int questLevel;

    [Space(50f)]
    [SerializeField]
    RectTransform progressBar;
    float progressBarStartWidth;
    [SerializeField]
    TextMeshProUGUI progressText, questName;
    [SerializeField] Button rewardButton;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        progressBarStartWidth = progressBar.sizeDelta.x;
        LoadQuests();
    }

    void LoadQuests()
    {
        for(int i = 0; i < quests.Count; i++)
        {
            quests[i].progress = PlayerPrefs.GetInt(quests[i].questName);
            if(quests[i].progress >= quests[i].maxProgess)
            {
                quests[i].done = true;
                
                int value = PlayerPrefs.GetInt(quests[i].questName + "reward");
                if(value == 0)
                {
                    rewardButton.gameObject.SetActive(true);
                    break;
                }
                questLevel = i + 1;
            }
        }
        currentQuest = quests[questLevel];
        UpdateQuestUI();
    }

    void UpdateQuestUI()
    {
        questName.text = currentQuest.questName;
        progressText.text = currentQuest.progress + "/" + currentQuest.maxProgess;
        progressBar.sizeDelta = new Vector2(progressBarStartWidth * ((float)currentQuest.progress / currentQuest.maxProgess) , progressBar.sizeDelta.y);
    }


    public void AddProgress(string questName, int addedProgress)
    {
        Quest q = FindQuestWithName(questName);
        if(q == null)
        {
            Debug.LogError("Cant add a Progress to a quest that doesnt Exists! I am really sorry...");
            return;
        }
        if(!q.done)
        {
            q.progress += addedProgress;
            SaveSpecificProgress(questName);
            CheckForDone(questName);
        }

        if(q == currentQuest)
        {
            UpdateQuestUI();
        }
    }

    Quest FindQuestWithName(string questName)
    {
        for(int i = 0; i < quests.Count; i++)
        {
            if(quests[i].questName == questName)
            {
                return quests[i];
            }
        }
        return null;
    }

    void SaveSpecificProgress(string questName)
    {
        Quest q = quests.Find(x => x.questName == questName);
        if (q == null)
        {
            Debug.LogError("Cant Save a Progress that doesn't exists! I am sorry...");
            return;
        }
        PlayerPrefs.SetInt(q.questName, q.progress);
    }

    public void CheckForDone(string questName)
    {
        Quest q = quests.Find(x => x.questName == questName);
        if (q == null)
        {
            Debug.LogError("Cant check nothing! sorry...");
            return;
        }
        if(q.progress >= q.maxProgess)
        {
            q.done = true;
            if (q == currentQuest)
            {
                rewardButton.gameObject.SetActive(true);
            }
        }
    }

    public void CollectReward()
    {
        rewardButton.gameObject.SetActive(false);
        PlayerPrefs.SetInt(currentQuest.questName + "reward", 1);
        questLevel++;
        currentQuest = quests[questLevel];
        UpdateQuestUI();
    }
}
