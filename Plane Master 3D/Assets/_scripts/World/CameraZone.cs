using UnityEngine;

public class CameraZone : MonoBehaviour
{
	[SerializeField]
	int cameraIndex;


	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			LevelSystem.instance.ChangeCamera(cameraIndex);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			LevelSystem.instance.ChangeCamera(0);
		}
	}
}
