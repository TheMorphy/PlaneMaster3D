using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlaneDropZone : MonoBehaviour
{
    [SerializeField]
    public List<UpgradeCondition> conditions = new List<UpgradeCondition>();
    public bool allConditionsComplete;
    [SerializeField]
    public bool showDroppedItems, displayTextAsCountDown;
    [SerializeField]
    Vector3 limitations, space;
    [SerializeField]
    public List<Item> items = new List<Item>();
    [SerializeField]
    Transform visualStart;
    [SerializeField]
    bool debug = false;

    Vector3[] positions = new Vector3[10];
    List<int> emptySlots = new List<int>();

    PlaneMain planeScript;

    private void Start()
    {
        if (showDroppedItems)
        {
            Array.Resize(ref positions, conditions[0].countNeeded);
            GenerateSortingSystem();
        }
        else
        {
            LoadConditions();
        }
        StartCoroutine(WaitForComplete());
    }

    void LoadConditions()
    {
        foreach (UpgradeCondition c in conditions)
        {
            c.count = PlayerPrefs.GetInt(gameObject.name + c.name + "count");
        }
    }

    public void SaveConditions()
    {
        foreach (UpgradeCondition c in conditions)
        {
            PlayerPrefs.SetInt(gameObject.name + c.name + "count", c.count);
        }
    }
    private void Update()
    {
        if (debug)
        {
            GenerateSortingSystem();
        }

        planeScript = FindObjectOfType<PlaneMain>();
    }
    public void ResetConditions()
    {
        foreach (UpgradeCondition c in conditions)
        {
            c.count = 0;
        }
        SaveConditions();
    }

    IEnumerator WaitForComplete()
    {
        yield return new WaitUntil(() => !AllDone());
        while (false == false)
        {
            yield return new WaitUntil(() => AllDone());
            allConditionsComplete = true;
            SendMessage("OnAllConditionsComplete", SendMessageOptions.DontRequireReceiver);
            yield return new WaitUntil(() => !AllDone());
            allConditionsComplete = false;
            yield return null;
        }
    }

    bool AllDone()
    {
        bool output = true;
        foreach (UpgradeCondition u in conditions)
        {
            if (u.count >= u.countNeeded)
                u.completed = true;
            else
                u.completed = false;
            if (!u.completed)
            {
                output = false;
            }
            if (displayTextAsCountDown)
            {
                u.text.text = (u.countNeeded - u.count).ToString();
                if (u.completed)
                {
                    u.text.gameObject.SetActive(false);
                }
                else if (!u.text.gameObject.activeSelf)
                    u.text.gameObject.SetActive(true);

            }
            else if (u.text != null)
            {
                u.text.text = u.count + "/" + u.countNeeded;
            }
        }
        return output;
    }

    public void AddItem(Item item, bool translateItem = true)
    {
        SaveConditions();
        items.Add(item);
        if (translateItem)
        {
            item.transform.parent = visualStart;

            SetItemDestination(items.Count - 1);

        }
        SendMessage("OnAddItem", SendMessageOptions.DontRequireReceiver);
    }

    void SetItemDestination(int item)
    {
        items[item].destination = positions[item];
        Coroutine c = StartCoroutine(LerpItemToDestination(item));
    }

    IEnumerator LerpItemToDestination(int i)
    {
        Item itemToLerp = items[i];
        Vector3 des = itemToLerp.destination;
        while (itemToLerp.transform.localPosition != itemToLerp.destination && itemToLerp.destination == des)
        {
            itemToLerp.transform.localPosition = Vector3.Lerp(itemToLerp.transform.localPosition, itemToLerp.destination, 0.1f);
            itemToLerp.transform.localRotation = Quaternion.Lerp(itemToLerp.transform.localRotation, Quaternion.Euler(Vector3.zero), 0.1f);
            yield return new WaitForEndOfFrame();
            if (itemToLerp == null)
            {
                yield break;
            }

        }
    }

    void GenerateSortingSystem()
    {
        Vector3 nextPos = Vector3.zero;
        int count = 0;
        for (int y = (int)limitations.y; y > 0; y--)
        {
            for (int x = (int)limitations.x; x > 0; x--)
            {
                for (int z = (int)limitations.z; z > 0; z--)
                {
                    positions[count] = nextPos;
                    nextPos.z += space.z;
                    count++;
                    if (count >= conditions[0].countNeeded)
                    {
                        return;
                    }
                }
                nextPos.x += space.x;
                nextPos.z = 0;
            }
            nextPos.x = 0;
            nextPos.y += space.y;
        }
    }
}
