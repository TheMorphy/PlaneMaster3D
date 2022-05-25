using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using Cinemachine;

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

    [Header("Objective Camera")]
    [SerializeField]
    CinemachineVirtualCamera objectiveCamera;
    Coroutine objectiveCamCoroutine;

    public void GoToObjectiveCamera()
	{
        if(currentQuest.lookAtTransform != null)
		{
            objectiveCamera.Follow = currentQuest.lookAtTransform;
            if (objectiveCamCoroutine != null)
            {
                StopCoroutine(objectiveCamCoroutine);
            }
            objectiveCamCoroutine = StartCoroutine(ObjectiveCam());
        }
    }

    IEnumerator ObjectiveCam()
	{
        LevelSystem.instance.ChangeCamera(4, 10);
        Coroutine breakCoroutine = StartCoroutine(WaitForBreakCam());
        yield return new WaitForSeconds(3.3f);
        StopCoroutine(breakCoroutine);
        LevelSystem.instance.ChangeCamera(4, -1);


    }

    IEnumerator WaitForBreakCam()
	{
        yield return new WaitUntil(() => Input.touchCount > 0 || Input.anyKeyDown);
        StopCoroutine(objectiveCamCoroutine);
        LevelSystem.instance.ChangeCamera(4, -1);
    }

    // Start is called before the first frame update
    void Start()
    {
        
        progressBarStartWidth = progressBar.sizeDelta.x;
        LoadQuests();
    }

	private void Awake()
	{
		instance = this;
		//print("awake");
		//if (PlayerPrefs.GetInt("Drill" + "p") < 1)
		//	PlayerPrefs.SetInt("Drill" + "p", 1);
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
                    //rewardButton.gameObject.SetActive(true);
					if(currentQuest != null)
                    CollectReward();
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
        Quest q = FindQuestWithName(questName, false);
        if(q == null)
        {
//            Debug.Log("Cant add a Progress to a quest that doesnt Exists! I am really sorry...");
            return;
        }
        if(!q.done)
        {
            q.progress += addedProgress;
            SaveSpecificProgress(questName);
            CheckForDone(null, q);
        }

        if(q == currentQuest)
        {
            UpdateQuestUI();
        }
    }

    Quest FindQuestWithName(string questName, bool returnWhenDone = true)
    {
		Quest q = null;


		//First checks if it should return also when its already done
		if(returnWhenDone)
		{
			for (int i = 0; i < quests.Count; i++)
			{
				if (quests[i].questName == questName)
				{

					return quests[i];
				}
			}
		}
		else 
		{
			for (int i = 0; i < quests.Count; i++)
			{
				if (quests[i].questName == questName)
				{
					if(!quests[i].done)
					{
						return quests[i];
					}
					else
					{
						q = quests[i];
					}
					
				}
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

    public void CheckForDone(string questName = null, Quest quest = null)
    {
		Quest q;
		if (quest == null)
		{
			q = FindQuestWithName(questName);
		}
		else
		{
			q = quest;
		}
		
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
				//rewardButton.gameObject.SetActive(true);
				if (currentQuest != null)
                CollectReward();
            }
        }
    }

    public void CollectReward()
    {
        rewardButton.gameObject.SetActive(false);
		
        foreach(Reward r in currentQuest.rewards)
        {
            r.Collect(rewardButton.transform.position);
        }
        PlayerPrefs.SetInt(currentQuest.questName + "reward", 1);
        questLevel++;
        currentQuest = quests[questLevel];
		CheckForDone(null, currentQuest);
        UpdateQuestUI();
    }
}
