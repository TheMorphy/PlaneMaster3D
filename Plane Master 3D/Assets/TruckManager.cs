using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckManager : MonoBehaviour
{
    [Space]
    [Header("TruckData")]
    [SerializeField] GameObject truckPrefab;
    [SerializeField] GameObject truckSummonPosition;
    [SerializeField] GameObject planeSummonPosition;
    [Space]
    [Header("Destinations")]
    [SerializeField] GameObject firstDestination;
    [SerializeField] GameObject lastDestination;
    [Space]
    [SerializeField] int truckSpeed = 1, waitToMove;
    [SerializeField] GameObject planePrefab;
    [SerializeField]
    List<Transform> groundPositions = new List<Transform>();

    bool isTruckInScene, isTruckStopped, isPlaneInScene, isTruckOpen;
    GameObject truck;
    Transform changeDestination;
    ManagerTest ts;
    private Animator anim;

    private void Start()
    {
        changeDestination = firstDestination.transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (!isTruckInScene)
            {
                SummonTruck();
                //ts.OnTruckSummon();
            }
        }

        if(changeDestination != null)
        {
            GoToCheckpoint(changeDestination, 3f, truckSpeed);
        }

        if (isTruckStopped)
        {
            if (!isPlaneInScene)
            {
                //SummonPlane();
            }
            StartCoroutine(WaitToChangeDestination(waitToMove));
        }
    }

    void SummonTruck()
    {
        Vector3 v = truckSummonPosition.transform.position;
        truck = Instantiate(truckPrefab, v, transform.rotation * Quaternion.Euler(0f, -90f, 0f));
        isTruckInScene = true;
    }
    void SummonPlane()
    {
        Vector3 v = planeSummonPosition.transform.position;
        Instantiate(planePrefab, v, Quaternion.identity);
        isPlaneInScene = true;
    }

    void GoToCheckpoint(Transform target, float distanceToStop, float speed)
    {
        if (truck != null)
        {
            Rigidbody rb = truck.GetComponent<Rigidbody>();
            var direction = Vector3.zero;

            if (isTruckInScene)
            {
                if (Vector3.Distance(truck.transform.position, target.position) > distanceToStop)
                {
                    direction = target.position - truck.transform.position;
                    rb.AddRelativeForce(Vector3.forward * speed, ForceMode.Force);
                }
                else isTruckStopped = true;
            }
        }
    }

    private IEnumerator WaitToChangeDestination(float waitTime)
    {
        anim = truck.GetComponent<Animator>();
        anim.Play("TruckOpeningAnimation");
        yield return new WaitForSeconds(waitTime);
        if (!isTruckOpen)
        {
            ts = FindObjectOfType<ManagerTest>();
            ts.DoScript(groundPositions);
            print("call do script");
            isTruckOpen = true;
        }
        if (lastDestination != null)
        {
            changeDestination = lastDestination.transform;
        }
    }
}
