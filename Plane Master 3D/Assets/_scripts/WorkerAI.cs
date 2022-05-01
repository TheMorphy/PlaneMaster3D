using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorkerAI : MonoBehaviour
{
    [SerializeField]
    Backpack backpack;
    [SerializeField]
    List<WorkerLevel> levels = new List<WorkerLevel>();
    [SerializeField]
    WorkerLevel generationParameters;
    [SerializeField]
    WorkerLevel currentLevel;
    public enum TaskType { Courier , Scientist }
    [SerializeField]
    TaskType task;
    [SerializeField]
    Transform getItemPos, itemDestinationPos;
    [SerializeField]
    public string itemToCarry;

    [SerializeField]
    float movementSpeed = 2, animSmooth, stayTime;
    float stayTimer;
    [SerializeField]
    Animator visual3D;

    NavMeshAgent agent;
    public bool isMoving;
    [SerializeField]
    ResearchSystem researchSystem;

    // Start is called before the first frame update
    

    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        switch (task)
        {
            case TaskType.Courier:
                StartCoroutine(CourierWorker());
                break;
            case TaskType.Scientist:
                StartCoroutine(ScientistWorker());
                break;
        }
    }

    public void setLevel(int level)
    {
        if(levels.Count <= level)
        {
            WorkerLevel generatedLevel = new WorkerLevel();
            generatedLevel.backpackSize = levels[level - 1].backpackSize + generationParameters.backpackSize;
            generatedLevel.speed = levels[level - 1].speed + generationParameters.speed;
            levels.Add(generatedLevel);
        }
        backpack.backpackSize = levels[level].backpackSize;
        agent.speed = levels[level].speed;
        currentLevel.backpackSize = levels[level].backpackSize;
        currentLevel.speed = levels[level].speed;
    }

    void Update()
    {
        
        

        visual3D.SetFloat("Speed", isMoving ? agent.speed : 0, animSmooth, Time.deltaTime);

    }

    private void FixedUpdate()
    {
        isMoving = agent.remainingDistance > agent.stoppingDistance;

    }

    IEnumerator ScientistWorker()
    {
        while(true)
        {
            if(!isMoving)
            {
                researchSystem.scientistAvailable = true;
            }
            else
            {
                researchSystem.scientistAvailable = false;

            }
            agent.SetDestination(itemDestinationPos.position);
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator CourierWorker()
    {
        while(true)
        {
            if (isMoving)
            {
                stayTimer = stayTime;
            }
            else
            {
                stayTimer -= 0.5f;
            }
            if (backpack.items.Count >= backpack.backpackSize && stayTimer <= 0)
            {
                agent.SetDestination(itemDestinationPos.position);
            }
            else if (backpack.items.Count == 0 && stayTimer <= 0)
            {
                agent.SetDestination(getItemPos.position);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
