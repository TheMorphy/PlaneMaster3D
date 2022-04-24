using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{
    public string questName;
    public int progress;
    public int maxProgess;
    public bool done;

    public bool Condition()
    {
        switch(questName)
        {
            case "Collect Iron":
                return CollectIron();
                break;
            default:
                Debug.LogError("Quest name doesn't matches a quest. Look into the public bool Condition inside Quest.cs");
                return false;
                break;
        }
    }

    bool CollectIron()
    {
        return true;
    }
}
