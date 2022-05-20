using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine;

public class LongClickButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float requiredHoldTime;
    public UnityEvent onLongClick;
    [SerializeField] private Image fillImage;
    [SerializeField] float speed, fuelQuantity;
    [SerializeField] Slider fuelSlider;
    [SerializeField] GameObject taskCompleted;
	[SerializeField] GameObject taskObject;
	[SerializeField] GameObject checkMark;
	[SerializeField] bool isMultipleRefill;

    private bool pointerDown;
	private bool isRefilled;
    private float pointerDownTimer;
	RefuelMinigameMultipleManager refuelMultipleScript;

	private void Start()
	{
		if (isMultipleRefill)
			refuelMultipleScript = FindObjectOfType<RefuelMinigameMultipleManager>();
	}

	public void OnPointerDown(PointerEventData eventData)
    {
        pointerDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Reset();
    }

    private void Update()
    {
		fuelSlider.value = fuelQuantity;

		if (pointerDown)
        {
			/*if (pointerDownTimer > requiredHoldTime)
            {
                Reset();
            }*/
			fuelQuantity += Time.unscaledDeltaTime * speed;
		}

        if (fuelQuantity >= 1 && !isMultipleRefill)
        {
			fuelQuantity = 0;
			StartCoroutine(OncePlayerWins());
        }

		if (fuelQuantity >= 1 && isMultipleRefill && !isRefilled)
		{
			fuelQuantity = 1;
			refuelMultipleScript.GetEachCompletedFill();
			isRefilled = true;
			checkMark.SetActive(true);
			//StartCoroutine(OncePlayerWinsMultipleRefill());
		}
    }

	public void ResetMultipleRefills()
	{
		fuelQuantity = 0;
		isRefilled = false;
		pointerDown = false;
		checkMark.SetActive(false);
	}

    private void Reset()
    {
        pointerDown = false;
        pointerDownTimer = 0;
        fillImage.fillAmount = pointerDownTimer / requiredHoldTime;
    }

    private IEnumerator OncePlayerWins()
    {
		pointerDown = false;
		taskObject.SetActive(false);
		yield return new WaitForSecondsRealtime(0.2f);
	}
}
