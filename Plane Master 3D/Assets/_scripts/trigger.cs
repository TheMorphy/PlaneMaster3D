using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trigger : MonoBehaviour
{
    MonitorTreeManager mts;

    private void Start()
    {
        mts = FindObjectOfType<MonitorTreeManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "1")
        {
            mts.P1 = true;
            mts.ChangeColour1();
        }
        if (collision.gameObject.name == "2")
        {
            mts.P2 = true;
            mts.ChangeColour2();
        }
        if (collision.gameObject.name == "3")
        {
            mts.P3 = true;
            mts.ChangeColour3();
        }
        if (collision.gameObject.name == "4")
        {
            mts.P4 = true;
            mts.ChangeColour4();
        }
		if (mts.P1 && mts.P2 && mts.P3 && mts.P4)
		{
			StartCoroutine(mts.OncePlayerWins());
		}
	}

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "1")
        {
            mts.P1 = false;
            mts.ChangeColour1();
        }
        if (collision.gameObject.name == "2")
        {
            mts.P2 = false;
            mts.ChangeColour2();
        }
        if (collision.gameObject.name == "3")
        {
            mts.P3 = false;
            mts.ChangeColour3();
        }
        if (collision.gameObject.name == "4")
        {
            mts.P4 = false;
            mts.ChangeColour4();
        }
    }
}
