using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerHouse : MonoBehaviour
{
    [SerializeField]
    Outline visual;
    [SerializeField]
    GameObject ui;
    bool alreadyActivated;
    Coroutine outline;
    [SerializeField]
    List<WorkerAI> workers = new List<WorkerAI>();
    [SerializeField] Transform workerStart;
    // Start is called before the first frame update
    void Start()
    {
        LoadWorkers();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadWorkers()
    {
        for(int i = 0; i < workers.Count; i++)
        {
            if(PlayerPrefs.GetInt("worker" + i.ToString()) > 0)
            {
                workers[i].gameObject.SetActive(true);
                workers[i].setLevel(PlayerPrefs.GetInt("worker" + i.ToString()));
            }
            else
            {
                workers[i].gameObject.SetActive(false);
                workers[i].transform.SetPositionAndRotation(workerStart.position, workerStart.rotation);
            }
        }
    }

    public void UnlockWorker(int index)
    {
        PlayerPrefs.SetInt("worker" + index.ToString(), PlayerPrefs.GetInt("worker" + index.ToString()) + 1);
        workers[index].gameObject.SetActive(true);
        workers[index].setLevel(PlayerPrefs.GetInt("worker" + index.ToString()));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 8)
        {
            if (outline != null)
                StopCoroutine(outline);
            outline = StartCoroutine(ActivateOutline());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            if (outline != null)
                StopCoroutine(outline);
            outline = StartCoroutine(DeactivateOutline());
            alreadyActivated = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            if(!other.GetComponent<Player>().isMoving && !alreadyActivated)
            {
                alreadyActivated = true;
                if (outline != null)
                    StopCoroutine(outline);
                outline = StartCoroutine(ActivateOutline());
                ui.SetActive(true);
            }
            else if(other.GetComponent<Player>().isMoving)
            {
                alreadyActivated = false;
                if (outline != null)
                    StopCoroutine(outline);
                outline = StartCoroutine(ActivateOutline());
            }
        }
    }

    public void CloseMenu()
    {
        ui.SetActive(false);
        if (outline != null)
            StopCoroutine(outline);
        outline = StartCoroutine(DeactivateOutline());
    }

    IEnumerator ActivateOutline()
    {
        visual.enabled = true;
        while (visual.OutlineColor.a != 89)
        {
            visual.OutlineColor = new Color(visual.OutlineColor.r, visual.OutlineColor.g, visual.OutlineColor.b, Mathf.Lerp(visual.OutlineColor.a, 0.43f, 0.1f)); 
            yield return null;
        }
    }

    IEnumerator DeactivateOutline()
    {
        while (visual.OutlineColor.a != 0)
        {
            visual.OutlineColor = new Color(visual.OutlineColor.r, visual.OutlineColor.g, visual.OutlineColor.b, Mathf.Lerp(visual.OutlineColor.a, 0, 0.1f));
            yield return null;
        }
        visual.enabled = false;
    }
}
