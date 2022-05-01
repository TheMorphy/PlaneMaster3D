using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Build : MonoBehaviour
{
    DroppingZone dz;
    Animator anim;
    [SerializeField]
    GameObject build, system;
    public int level;
    public string savingKey;
    [SerializeField]
    List<BuildLevel> levels = new List<BuildLevel>();
    bool readyForUpgrade;
    void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        dz = GetComponent<DroppingZone>();
        LoadVariables();
        SetConditionsToLevel();
    }

    void LoadVariables()
    {
        level = PlayerPrefs.GetInt(savingKey + "c");
        

        if (level < PlayerPrefs.GetInt(savingKey + "p"))
        {
            readyForUpgrade = true;
            build.SetActive(true);
            GetComponent<DroppingZone>().enabled = true;
        }
        else
        {
            readyForUpgrade = false;
            build.SetActive(false);
            GetComponent<DroppingZone>().enabled = false;
        }
        if (level > 0)
            system.SetActive(true);
        system.transform.GetChild(0).SendMessage("OnLoadLevels", SendMessageOptions.DontRequireReceiver);
        /*
                if (level > 0)
                {
                    GetComponent<Animator>().enabled = false;
                    build.SetActive(false);
                    system.SetActive(true);
                    system.transform.GetChild(0).SendMessage("OnChangeLevel");
                }
                else
                {
                    GetComponent<DroppingZone>().enabled = true;
                    build.SetActive(true);
                    system.SetActive(false);
                }
        */
    }

    public void OnAddedPossibleLevel()
    {
        LoadVariables();
    }
    

    void OnAllConditionsComplete()
    {
        print("OnAllConditionsComplete");
        //Upgrade
        if (level == 0)
            anim.Play("Build");

        level = PlayerPrefs.GetInt(savingKey + "c") + 1;
        PlayerPrefs.SetInt(savingKey + "c", level);
        LoadVariables();
        
        SetConditionsToLevel();
        dz.ResetConditions();
    }

    void SetConditionsToLevel()
    {
        dz.conditions = levels[level].conditions;
    }
    public void SwitchToSystem()
    {
        build.SetActive(false);
        system.SetActive(true);
    }
}
