using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Breakable : MonoBehaviour
{
    public bool isRepaired;
    [SerializeField]
    Transform originalPosition;

    [SerializeField]
    Vector3 paletteRotation;

	[SerializeField]
	RepairStation station;
	[SerializeField]
	public List<UpgradeCondition> conditions = new List<UpgradeCondition>();

	public RepairStation Station { get => station; set => station = value; }
	public Vector3 PaletteRotation { get => paletteRotation; set => paletteRotation = value; }

	void OnAllConditionsComplete()
    {
        isRepaired = true;
        StartCoroutine(LerpToOriginalPosition());
    }

    IEnumerator LerpToOriginalPosition()
    {
		//print("HERE IS THE BREALABLEEEEEE");
		Vector3 toLerpPos = originalPosition != null ? originalPosition.position : Vector3.zero;
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
