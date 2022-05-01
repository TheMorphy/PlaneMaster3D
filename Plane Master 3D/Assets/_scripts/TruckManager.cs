using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckManager : MonoBehaviour
{
    [Space]
    [Header("TruckData")]
    [SerializeField] GameObject truckPrefab;
    [SerializeField] GameObject truckSummonPosition;
    [Space]
    [Header("Destinations")]
    [SerializeField] GameObject firstDestination;
    [SerializeField] GameObject lastDestination;
    [Space]
    [SerializeField] int truckSpeed = 1;
    [SerializeField] List<Transform> groundPositions = new List<Transform>();

    [SerializeField] GameObject planeDad;

    bool isTruckStopped, isTruckOpen, reset, asigned;
    GameObject truck, planeSon;
    Transform changeDestination;
    ManagerTest ts;
    PlaneMain pm;
    private Animator anim, planeAnim;

    public GameObject Truck { get => truck; set => truck = value; }

    private void Update()
    {

        // Summon the truck
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (Truck == null)
            {
                SummonTruck();
            }
        }

        // Move truck to the first checkpoint
        if(changeDestination != null)
        {
            GoToCheckpoint(changeDestination, 3f, truckSpeed);
        } else changeDestination = firstDestination.transform;

        // Once the truck stops, move him to the next checkpoint
        if (isTruckStopped)
        {
            if (Truck != null)
            {
                // Play the truck opening animation
                anim.Play("TruckOpeningAnimation");
                anim.SetBool("isClosed", false);

                StartCoroutine(WaitToChangeDestination());
            }

        // If the truck is going to move again do the closing animation
        } else if (anim != null)
		{
			anim.SetBool("isClosed", true);
		}
    }

    // Method that summons the truck
    void SummonTruck()
    {
        Vector3 v = truckSummonPosition.transform.position;
        Truck = Instantiate(truckPrefab, v, transform.rotation * Quaternion.Euler(0f, -90f, 0f));
        anim = Truck.GetComponent<Animator>();
    }

    // Method that makes the truck checkpoints
    void GoToCheckpoint(Transform target, float distanceToStop, float speed)
    {
        if (Truck != null)
        {
            Rigidbody rb = Truck.GetComponent<Rigidbody>();
            var direction = Vector3.zero;

            if (Vector3.Distance(Truck.transform.position, target.position) > distanceToStop)
            {
                direction = target.position - Truck.transform.position;
                rb.AddRelativeForce(Vector3.forward * speed, ForceMode.Force);
                isTruckStopped = false;
            }
            else isTruckStopped = true;
        }
    }

    // Method that creates the loop
    private IEnumerator WaitToChangeDestination()
    {
        yield return new WaitForSeconds(1);

        // Open Truck
        if (!isTruckOpen)
        {
            if (Truck != null)
            {
				// Assign Plane Gameobject a new Parent, other than the truck
				planeSon = Truck.transform.GetChild(2).gameObject;
				planeSon.transform.parent = planeDad.transform;

				// Move plane parts inside the truck to the groud positions
				ts = FindObjectOfType<ManagerTest>();
                ts.DoScript(groundPositions);
            }
        }
        else isTruckOpen = false;

        if (lastDestination != null)
        {
            // Change the truck destination to the last one
            changeDestination = lastDestination.transform;
        }

        //print(isTruckOpen);
    }

    // Method that resets the script
    public void ResetTruckManager()
    {
        isTruckStopped = false;
        isTruckOpen = false;
		planeSon = null;
        Truck = null;
        changeDestination = null;
        ts = null;
        pm = null;
        anim = null;
        planeAnim = null;
		firstDestination.SetActive(true);
		lastDestination.SetActive(false);
    }
}
