using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        //print("niwajuidwnia");
        if (other.gameObject.layer == 8)
        {
            
            GetComponent<Rigidbody>().AddForce((other.transform.position - transform.position).normalized * 1, ForceMode.Impulse);
        }
    }
}
