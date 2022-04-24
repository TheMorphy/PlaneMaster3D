using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckDropParts : MonoBehaviour
{
    /*[Space]
    [Header("Plane Data")]
    [SerializeField] string planeName;
    [SerializeField] List<GameObject> planes = new List<GameObject>();
    [Space]
    [Header("Parts")]*/
    [SerializeField] List<GameObject> planeParts = new List<GameObject>();
    [Space]
    [SerializeField] GameObject dropZone;
    [SerializeField] GameObject[] parts;

    GameObject nullGameobject;
    PlaneMain ps;
    int listSize;
    TruckManager tms;

    private void Start()
    {
        print("spawned");
        dropZone = GameObject.FindGameObjectWithTag("dropzone");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            DropParts();
        }
    }
    public void DropParts()
    {
        ps = FindObjectOfType<PlaneMain>();

        if (ps != null)
        {
            //listSize = ps.breakables.Count;
        }
        
        for (int i = 0; listSize > planeParts.Count; i++)
        {
            GameObject ChildGameObject = dropZone.transform.GetChild(i).gameObject;
            planeParts.Add(ChildGameObject);

            parts = GameObject.FindGameObjectsWithTag("brokenpos");

            parts[i].transform.position = planeParts[i].transform.position;

            //ps.OnAddItem();
        }
    }
}
