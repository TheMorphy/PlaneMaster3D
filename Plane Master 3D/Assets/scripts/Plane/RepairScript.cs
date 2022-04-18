using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RepairScript : MonoBehaviour
{
    [SerializeField]
    float debug;
    [SerializeField] Plane plane;
    [SerializeField] GameObject[] scaleObject;
    [SerializeField] GameObject niceUi;
    [Space]
    [Header("Masks")]
    [SerializeField] GameObject mFront;
    [SerializeField] GameObject mLeft;
    [SerializeField] GameObject mRight;

    [SerializeField]
    Transform f, l, r;

    [Range(3.0f, 0.0f)]
    public float silhouetteSlider;

    DroppingZone dzScript;
    int currentCogs, currentIron, i, maskPosition, startingScaling, t;
    float scaleX, scaleY, scaleZ, percentage;
    private Vector3 scaleChange;
    List<float> defaulzScales = new List<float>();

    private void Start()
    {
        dzScript = GetComponent<DroppingZone>();
        plane.randomRepair = 0;
        plane.RandomRepair();
        
        for(int i = 0; i < scaleObject.Length; i++)
        {
            defaulzScales.Add(scaleObject[i].transform.localScale.z);
        }
    }

    private void Update()
    {
        //FillSilhouette();

        if (silhouetteSlider <= 0)
        {
            //niceUi.SetActive(true);
        }
        
    }

    private void OnAddItem()
    {
        
        int maxCount = 0, currentCount = 0;
        foreach (UpgradeCondition u in dzScript.conditions)
        {
            currentCount += u.count;

            maxCount += u.countNeeded;


        }
        print(currentCount);
        percentage = (float)currentCount / maxCount;
        print(percentage);
        silhouetteSlider = (1 - percentage) * 3;
        FillSilhouette();
    }

    private void FillSilhouette()
    {
        l.localScale = new Vector3(l.localScale.x, l.localScale.y, 5.346772f * Mathf.Clamp01((silhouetteSlider - 2)));
        r.localScale = new Vector3(r.localScale.x, r.localScale.y, 5.346772f * Mathf.Clamp01((silhouetteSlider - 1)));
        f.localScale = new Vector3(f.localScale.x, f.localScale.y, 25.89941f * Mathf.Clamp01((silhouetteSlider)));










        /*
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
        */

    }

}
