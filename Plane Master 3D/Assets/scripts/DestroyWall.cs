using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWall : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        GameObject g = collision.gameObject;

        if (g.layer == 9)
        {
            Destroy(gameObject, 2);
        }
    }
}
