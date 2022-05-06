using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlignManager : MonoBehaviour
{
    [SerializeField] GameObject objectToRotate, congrats;
    [SerializeField] Slider slider;
    [SerializeField] Image middleLine, objColor;
    [SerializeField] float speed, sliderPosition, publicValue;

    [SerializeField] float timer;

    private float previousValue;

    private void Awake()
    {
        slider.onValueChanged.AddListener(OnSliderChanged);

        previousValue = slider.value;
    }

    private void Update()
    {
        if (objectToRotate.transform.rotation.eulerAngles.z <= 3f || objectToRotate.transform.rotation.eulerAngles.z <= -3f)
        {
            middleLine.color = Color.green;
            objColor.color = Color.green;
            timer = timer + Time.deltaTime;
            if (timer >= 2f)
            {
                timer = 2f;
				// PLAYER WINS HERE
                congrats.SetActive(true);
            }
        }
        else
        {
            middleLine.color = Color.white; 
            objColor.color = Color.white;
            timer = 0;
        }
    }

    void OnSliderChanged(float value)
    {
        float delta = value - previousValue;

        objectToRotate.transform.Rotate(Vector3.forward * delta * -90);

        previousValue = value;
    }
}
