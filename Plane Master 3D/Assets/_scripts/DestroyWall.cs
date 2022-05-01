using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWall : MonoBehaviour
{
    [SerializeField] int timeToDestroy;
    [SerializeField] GameObject wall;
    TruckManager tm;

    private void Start()
    {
        tm = FindObjectOfType<TruckManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject g = collision.gameObject;

        if (g.layer == 9)
        {
            if (g != null)
            {
                Destroy(g, timeToDestroy + 1);
                wall.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }
}
