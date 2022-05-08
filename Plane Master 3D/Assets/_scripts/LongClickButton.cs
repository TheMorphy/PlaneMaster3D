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

    private bool pointerDown;
    private float pointerDownTimer;

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
        if (pointerDown)
        {
            /*if (pointerDownTimer > requiredHoldTime)
            {
                Reset();
            }*/
            fuelQuantity += Time.unscaledDeltaTime * speed;
            fuelSlider.value = fuelQuantity;
        }

        if (fuelQuantity >= 1)
        {
            fuelQuantity = 1;
            StartCoroutine(OncePlayerWins());
        }
    }

    private void Reset()
    {
        pointerDown = false;
        pointerDownTimer = 0;
        fillImage.fillAmount = pointerDownTimer / requiredHoldTime;
    }

    private IEnumerator OncePlayerWins()
    {
		fuelQuantity = 0;
		taskObject.SetActive(false);
        //taskCompleted.SetActive(true);
        yield return new WaitForSecondsRealtime(1);
        //taskCompleted.SetActive(false);
        
    }
}
