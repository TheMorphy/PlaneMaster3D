using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerUI : MonoBehaviour
{
    [SerializeField]
    RectTransform mask;
    [SerializeField]
    float closedButtom, openedButtom;
    bool isOpen;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchOpen()
    {
        print("switch");
        isOpen = !isOpen;
        mask.sizeDelta = new Vector2(mask.sizeDelta.x, isOpen ? openedButtom : closedButtom);
    }
}
