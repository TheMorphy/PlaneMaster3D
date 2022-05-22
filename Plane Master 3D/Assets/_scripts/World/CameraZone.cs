using UnityEngine;

public class CameraZone : MonoBehaviour
{
	[SerializeField]
	int cameraIndex;


	public void SetCameraIndex(int newindex)
	{
		LevelSystem.instance.ChangeCamera(cameraIndex, 0);
		cameraIndex = newindex;
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			LevelSystem.instance.ChangeCamera(cameraIndex, 2);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			LevelSystem.instance.ChangeCamera(cameraIndex, 2);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			LevelSystem.instance.ChangeCamera(cameraIndex, 0);
		}
	}
}
