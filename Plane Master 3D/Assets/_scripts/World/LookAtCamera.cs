using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
	[SerializeField]
	Vector3 upwardsVector;

	Transform cam;
    // Start is called before the first frame update
    void Start()
    {
		cam = Camera.main.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
		transform.LookAt(cam.position);
		transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x + upwardsVector.x, transform.rotation.eulerAngles.y + upwardsVector.y, transform.rotation.eulerAngles.z + upwardsVector.z);
    }
}
