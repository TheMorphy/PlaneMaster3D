using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchSystem : MonoBehaviour
{
    DroppingZone dz;
    [SerializeField]
    public List<Research> researches = new List<Research>();
    [SerializeField]
    int level;
    [SerializeField]
    GameObject Gui;
    public bool scientistAvailable;
    // Start is called before the first frame update
    void Start()
    {
        dz = GetComponent<DroppingZone>();
        dz.enabled = false;
        Gui.SetActive(false);
        StartCoroutine(ResearchHandler());
        LoadResearches();
    }

    void LoadResearches()
    {
        level = PlayerPrefs.GetInt("researchLevel");
        for(int i = 0; i < level; i++)
        {
            researches[i].completed = true;
            researches[i].buildToUnlock.gameObject.SetActive(true);
            researches[i].icon.gameObject.SetActive(false);
        }
        dz.conditions = researches[level].conditions;
        researches[level].icon.gameObject.SetActive(true);
    }

    void OnAllConditionsComplete()
    {
        researches[level].buildToUnlock.gameObject.SetActive(true);
        researches[level].completed = true;
        PlayerPrefs.SetInt(researches[level].buildToUnlock.savingKey + "p", PlayerPrefs.GetInt(researches[level].buildToUnlock.savingKey + "p") + 1);
        researches[level].buildToUnlock.OnAddedPossibleLevel();


        level += 1;
        PlayerPrefs.SetInt("researchLevel", level);
        LoadResearches();
    }

    IEnumerator ResearchHandler()
    {
        while(true)
        {
            yield return new WaitUntil(() => scientistAvailable);
            dz.enabled = true;
            Gui.SetActive(true);
            yield return new WaitUntil(() => !scientistAvailable);
            dz.enabled = false;
            Gui.SetActive(false);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
