using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Breakable : MonoBehaviour
{
    public bool isRepaired;
    [SerializeField]
    Transform originalPosition;

    public PlaneMain planeMain;
    void OnAllConditionsComplete()
    {
        isRepaired = true;
        StartCoroutine(LerpToOriginalPosition());
    }

    IEnumerator LerpToOriginalPosition()
    {
		planeMain.SendMessage("OnBreakableRepaired");
		//print("HERE IS THE BREALABLEEEEEE");
		while (transform.position != originalPosition.position)
        {
			//print("BREAKABLE IS LERPING RN");
			transform.position = Vector3.Lerp(transform.position, originalPosition.position, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, originalPosition.rotation, 0.1f);

            yield return null;
        }
		//print("BREAKABLE STOPPED LERPING");
        // Call method
        
        yield break;
    }
}
