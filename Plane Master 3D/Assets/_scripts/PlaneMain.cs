using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMain : MonoBehaviour
{

	[SerializeField] GameObject planeDropZone;
	DroppingZone dz;
    [SerializeField]
    public List<Breakable> breakables = new List<Breakable>();

    public int Lenght;
    [SerializeField] GameObject[] randomParts;
    [SerializeField] GameObject[] containerPositions;

    [SerializeField] public List<int> randomNumbers = new List<int>();
    int randomNumber;
    public int Rand;

    public bool allIsRepaired = false;
    
    Animator anim;
    TruckManager tm;
	DroppingZone dzS;
	[SerializeField]
	List<GameObject> disableOnFinish = new List<GameObject>(), enableOnFinish = new List<GameObject>();
	bool hoppedOn;
	[SerializeField]
	Transform hopOnPos;

	GameObject player;

    public int RandomNumber { get => randomNumber; set => randomNumber = value; }

    private void OnEnable()
    {
        dz = GetComponent<DroppingZone>();
        AssignPlainMains();
    }

    private void Start()
    {
        AssignRandomParts();
		StartCoroutine(WaitToActivate());
    }

    void AssignPlainMains()
    {
        foreach (Breakable b in breakables)
        {
            b.planeMain = this;
        }
    }

    void OnBreakableRepaired()
    {
        allIsRepaired = true;
        //bool allRepaired = true;
        foreach(Breakable b in breakables)
        {
            if(!b.isRepaired)
            {
                allIsRepaired = false;
            }
        }

        if(allIsRepaired)
        {
			//TakeOff
			//Get The money 
			//...
			LevelSystem.instance.PlayMinigame();
			QuestSystem.instance.AddProgress("Repair a plane", 1);
			StartCoroutine(WaitForTakeOff());
			for(int i = 0; i < disableOnFinish.Count; i++)
			{
				disableOnFinish[i].SetActive(false);
			}
			
		}
    }

	IEnumerator WaitForTakeOff()
	{
		yield return new WaitUntil(() => hoppedOn && player == null);
		anim = GetComponent<Animator>();
		anim.Play("PlaneFlyOff");
		print("all is repaired");
		planeDropZone.SetActive(false);
		QuestSystem.instance.AddProgress("Leave the island", 1);
		Destroy(transform.parent.gameObject, 3);
	}

	private void OnTriggerEnter(Collider other)
	{
		if(allIsRepaired && other.tag == "Player")
		{
			hoppedOn = true;
			player = other.gameObject;
			LevelSystem.instance.ControlPlayer(hopOnPos.position, true);
		}
	}

	public void AssignRandomParts()
    {
        randomNumbers = new List<int>(new int[Lenght]);

        for (int j = 0; j < Lenght; j++)
        {
            Rand = Random.Range(0, 5);

            while (randomNumbers.Contains(Rand))
            {
                Rand = Random.Range(0, 5);
            }

            randomNumbers[j] = Rand;

            breakables[Rand].transform.position = containerPositions[j].transform.position;
            breakables[Rand].isRepaired = false;
            breakables[Rand].GetComponent<DroppingZone>().enabled = true;

            //print(randomNumbers[j]);
        }
    }

	IEnumerator WaitToActivate()
	{
		yield return new WaitForSeconds(0.1f);
		planeDropZone.SetActive(true);
		dzS = planeDropZone.GetComponent<DroppingZone>();
		dzS.GetEachCondition();
	}
}
