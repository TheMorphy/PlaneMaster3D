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

	[SerializeField] GameObject taskObject;

    private float previousValue;

    private void OnEnable()
    {
        slider.onValueChanged.AddListener(OnSliderChanged);

        previousValue = slider.value;
    }

    private void Update()
    {
		float angle = objectToRotate.transform.rotation.eulerAngles.z;
		angle = (angle > 180) ? angle - 360 : angle;

		if (angle <= 4f && angle >= -4f)
        { 
            middleLine.color = Color.green;
            objColor.color = Color.green;
            timer = timer + Time.deltaTime;
            if (timer >= 1f)
            {
                timer = 1f;
				// PLAYER WINS HERE
				taskObject.SetActive(false);
                //congrats.SetActive(true);
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
