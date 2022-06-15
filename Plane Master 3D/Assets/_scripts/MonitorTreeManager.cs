using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonitorTreeManager : MonoBehaviour
{
    [SerializeField] GameObject congrats;
    [SerializeField] Image pos1, pos2, pos3, pos4;
    [SerializeField] GameObject[] positionsA;
    [SerializeField] GameObject[] positionsB;
    [SerializeField] GameObject[] positionsC;
    [SerializeField] GameObject[] positionsD;
	[Space(10)]
	[SerializeField] GameObject button1, button2, button3, button4;

	Vector2 button1InitialPosition, button2InitialPosition, button3InitialPosition, button4InitialPosition;


	private bool p1, p2, p3, p4;

    public bool P1 { get => p1; set => p1 = value; }
    public bool P2 { get => p2; set => p2 = value; }
    public bool P3 { get => p3; set => p3 = value; }
    public bool P4 { get => p4; set => p4 = value; }

	private void Start()
	{
		RandomPosition();
		button1.GetComponent<Slider>().value = 0;
		button2.GetComponent<Slider>().value = 0;
		button3.GetComponent<Slider>().value = 0;
		button4.GetComponent<Slider>().value = 0;
	}

	private void ResetTheGame()
	{
		RandomPosition();
		button1.GetComponent<Slider>().value = 0;
		button2.GetComponent<Slider>().value = 0;
		button3.GetComponent<Slider>().value = 0;
		button4.GetComponent<Slider>().value = 0;
	}

	private void OnEnable()
    {
        //RandomPosition();
    }

	public void OnValueChanged()
	{
		if (p1 && p2 && p3 && p4)
		{
			StartCoroutine(OncePlayerWins());
		}
	}
    public IEnumerator OncePlayerWins()
    {
        yield return new WaitForSecondsRealtime(0.1f);
		//congrats.SetActive(true);
		ResetTheGame();
		gameObject.SetActive(false);
    }

    void RandomPosition()
    {
        int r1 = Random.Range(0, 4);
        int r2 = Random.Range(0, 4);
        int r3 = Random.Range(0, 4);
        int r4 = Random.Range(0, 4);

        pos1.transform.position = positionsA[r1].transform.position;
        pos2.transform.position = positionsB[r2].transform.position;
        pos3.transform.position = positionsC[r3].transform.position;
        pos4.transform.position = positionsD[r4].transform.position;
    }

    public void ChangeColour1()
    {
        if (P1)
        {
            pos1.color = Color.green;
        }
        else pos1.color = Color.white;
    }
    public void ChangeColour2()
    {
        if (p2)
        {
            pos2.color = Color.green;
        }
        else pos2.color = Color.white;
    }
    public void ChangeColour3()
    {
        if (p3)
        {
            pos3.color = Color.green;
        }
        else pos3.color = Color.white;
    }
    public void ChangeColour4()
    {
        if (p4)
        {
            pos4.color = Color.green;
        }
        else pos4.color = Color.white;
    }
}
