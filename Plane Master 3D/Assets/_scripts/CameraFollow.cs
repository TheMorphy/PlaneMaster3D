using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    Transform target;
    [SerializeField]
    float smooth;
    Vector3 offset;
	Transform stableForward;
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - target.position;
		stableForward = transform.GetChild(0);
		stableForward.parent = null;
	}

    // Update is called once per frame
    void LateUpdate()
    {

		stableForward.rotation = Quaternion.Euler(0, transform.eulerAngles.y , 0);

		//transform.GetChild(0).rotation = Quaternion.Euler(0, transform.GetChild(0).rotation.eulerAngles.y, 0);
		//print(stableForward.rotation.eulerAngles.x);
        //transform.position = Vector3.Slerp(transform.position, target.position + offset, smooth);
    }

	
}
