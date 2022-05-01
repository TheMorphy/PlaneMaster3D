using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine;

public class RotateOnCurve : MonoBehaviour
{
    [SerializeField] private Transform[] routes;
    [SerializeField] float speed;

    private int routeToGo;

    private float tParam;

    private Vector3 buttonPosition;

    private float speedModifier;

    private bool coroutineAllowed;


    public float requiredHoldTime;
    public UnityEvent onLongClick;

    private bool pointerDown;
    private float pointerDownTimer;

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDown = true;
        print("DOWN");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Reset();
    }

    private void Start()
    {
        routeToGo = 0;
        tParam = 0f;
        speedModifier = 0.5f;
        coroutineAllowed = true;
    }

    private void Update()
    {
        if (pointerDown)
        {
            /*if (pointerDownTimer > requiredHoldTime)
            {
                Reset();
            }*/

            print("WORK");

            if (coroutineAllowed)
                StartCoroutine(GoByTheRoute(routeToGo));
        }
    }

    private void Reset()
    {
        pointerDown = false;
        pointerDownTimer = 0;
    }

    private IEnumerator CompletedUI()
    {
        yield return new WaitForSeconds(1);
    }

    private IEnumerator GoByTheRoute(int routeNumber)
    {
        coroutineAllowed = false;

        Vector2 p0 = routes[routeNumber].GetChild(0).position;
        Vector2 p1 = routes[routeNumber].GetChild(1).position;
        Vector2 p2 = routes[routeNumber].GetChild(2).position;
        Vector2 p3 = routes[routeNumber].GetChild(3).position;

        while (tParam < 1)
        {
            tParam += Time.deltaTime * speedModifier;

            buttonPosition = Mathf.Pow(1 - tParam, 3) * p0 +
                3 * Mathf.Pow(1 - tParam, 2) * tParam * p1 +
                3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 +
                Mathf.Pow(tParam, 3) * p3;

            transform.position = buttonPosition;
            yield return new WaitForEndOfFrame();
        }

        tParam = 0f;

        routeToGo += 1;

        if (routeToGo > routes.Length - 1)
        {
            routeToGo = 0;
        }

        coroutineAllowed = true;
    }
}
