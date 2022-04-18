using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RepairScript : MonoBehaviour
{
    [SerializeField] Plane plane;
    [SerializeField] GameObject[] scaleObject;
    [SerializeField] GameObject niceUi;
    [Space]
    [Header("Masks")]
    [SerializeField] GameObject mFront;
    [SerializeField] GameObject mLeft;
    [SerializeField] GameObject mRight;

    [Range(1.0f, 0.0f)]
    public float silhouetteSlider;

    DroppingZone dzScript;
    int currentCogs, currentIron, i, maskPosition, startingScaling, t;
    float scaleX, scaleY, scaleZ, percentage, maxCount, currentCount;
    private Vector3 scaleChange;

    private void Start()
    {
        dzScript = gameObject.GetComponent<DroppingZone>();
        plane.randomRepair = 0;
        plane.RandomRepair();
    }

    private void Update()
    {
        FillSilhouette();

        if (silhouetteSlider <= 0)
        {
            niceUi.SetActive(true);
        }

        foreach (UpgradeCondition u in dzScript.conditions)
        {
            if (dzScript.conditions.Count > t)
            {
                maxCount += u.countNeeded;
                t += 1;
            }

            if (currentCount < u.countNeeded)
            {
                currentCount = u.count;
            }

            percentage = currentCount / maxCount;

            silhouetteSlider = 1 - percentage;

            //print(maxCount);
            //print(percentage);
        }
    }

    void OnAddItem()
    {
        

        
    }

    private void FillSilhouette()
    {
        for (i = 0; i < scaleObject.Length; i++)
        {
            string maskToString = scaleObject[i].name;

            if (maskToString == plane.repairParts)
            {
                maskPosition = i;

                if (plane.repairParts == "Front")
                {
                    scaleY = scaleObject[i].transform.localScale.y;
                    scaleZ = scaleObject[i].transform.localScale.z;

                    scaleChange = new Vector3(silhouetteSlider, scaleY, scaleZ);

                    scaleObject[i].transform.localScale = scaleChange;
                }

                if (plane.repairParts == "Left")
                {
                    scaleY = scaleObject[i].transform.localScale.y;
                    scaleX = scaleObject[i].transform.localScale.x;

                    scaleChange = new Vector3(scaleX, scaleY, silhouetteSlider);

                    scaleObject[i].transform.localScale = scaleChange;
                }

                if (plane.repairParts == "Right")
                {
                    scaleY = scaleObject[i].transform.localScale.y;
                    scaleX = scaleObject[i].transform.localScale.x;

                    scaleChange = new Vector3(scaleX, scaleY, silhouetteSlider);

                    scaleObject[i].transform.localScale = scaleChange;
                }
            }
        }

        switch (plane.randomRepair)
        {
            case 1:
                mFront.SetActive(true);
                mLeft.SetActive(false);
                mRight.SetActive(false);
                break;
            case 2:
                mLeft.SetActive(true);
                mFront.SetActive(false);
                mRight.SetActive(false);
                break;
            case 3:
                mRight.SetActive(true);
                mLeft.SetActive(false);
                mFront.SetActive(false);
                break;
        }
    }

}
