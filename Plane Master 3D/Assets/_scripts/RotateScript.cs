using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateScript : MonoBehaviour
{
    public bool isRight = true;
    public float speed = 270.0f;

    void Update()
    {
        if (Input.GetMouseButton(0)) {
            bool onRight = (Input.mousePosition.x > Screen.width / 2.0f);

            if (isRight && onRight || !isRight && !onRight)
            {
                Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
                Vector3 dir = Input.mousePosition - pos;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Quaternion targetRot = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, Time.deltaTime * speed);
            }
        }
    }
}
