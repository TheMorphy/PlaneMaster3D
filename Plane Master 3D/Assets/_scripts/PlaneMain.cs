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
			anim = GetComponent<Animator>();
			anim.Play("PlaneFlyOff");
            print("all is repaired");
			planeDropZone.SetActive(false);
			Destroy(transform.parent.gameObject, 3);
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
		yield return new WaitForSeconds(3);
		planeDropZone.SetActive(true);
		dzS = planeDropZone.GetComponent<DroppingZone>();
		dzS.GetEachCondition();
	}
}
