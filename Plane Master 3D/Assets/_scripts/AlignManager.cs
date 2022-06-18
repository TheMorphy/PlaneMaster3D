using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlignManager : MonoBehaviour
{
	[SerializeField] GameObject objectToRotate;
    [SerializeField] Slider slider;
    [SerializeField] Image middleLine, objColor;

    float timer;

    private float previousValue;

    private void Start()
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
                timer = 0f;
				// PLAYER WINS HERE
				slider.value = 0;
				gameObject.SetActive(false);
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
