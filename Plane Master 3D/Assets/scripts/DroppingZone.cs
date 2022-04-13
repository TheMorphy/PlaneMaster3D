using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppingZone : MonoBehaviour
{

    [SerializeField]
    public List<UpgradeCondition> conditions = new List<UpgradeCondition>();
    public bool allConditionsComplete;

    private void Start()
    {
        StartCoroutine(WaitForComplete());
    }

    IEnumerator WaitForComplete()
    {
        while ( false == false)
        {
            yield return new WaitUntil(() => AllDone());
            allConditionsComplete = true;
            yield return new WaitUntil(() => !AllDone());
            yield return null;
        }
    }

    bool AllDone()
    {
        bool output = true;
        foreach(UpgradeCondition u in conditions)
        {
            if (u.count >= u.countNeeded)
                u.completed = true;
            if(!u.completed)
            {
                output = false;
                
            }
            u.text.text = u.count + "/" + u.countNeeded;
        }
        return output;
    }



}
