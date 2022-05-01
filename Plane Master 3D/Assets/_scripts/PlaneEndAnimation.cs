using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneEndAnimation : MonoBehaviour
{

    [SerializeField] GameObject animationPlane, normalPlane;
    void EndPlaneAnimation()
    {
        normalPlane.SetActive(true);
        animationPlane.SetActive(false);
    }
}
