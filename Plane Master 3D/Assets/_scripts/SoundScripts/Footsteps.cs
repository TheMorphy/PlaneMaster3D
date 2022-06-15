using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    [SerializeField]
    List<AudioClip> stoneClips = new List<AudioClip>(), sandClips = new List<AudioClip>(), grasClips = new List<AudioClip>();

	[SerializeField]
	Transform checkForGround;


	private AudioSource source;
	private void Start()
	{
		source = GetComponent<AudioSource>();

		Sound sound = SoundSystem.instance.sounds.Find(sound => sound.name == "Footsteps");
		sound.sources.Add(source);
		SoundSystem.instance.SendMessage("UpdateVolumes");
	}

	private void Step()
	{
		if(source == null)
			source = GetComponent<AudioSource>();

		List<AudioClip> currentClips = null;
		RaycastHit hit;
		if(Physics.Raycast(checkForGround.position, -checkForGround.up, out hit))
		{
			switch (hit.collider.gameObject.layer)
			{
				case 15:
					currentClips = stoneClips;
					break;
				case 16:
					currentClips = sandClips;
					break;
				case 17:
					currentClips = grasClips;
					break;
				default:
					currentClips = stoneClips;
					break;

			}
			source.PlayOneShot(currentClips[Random.Range(0, currentClips.Count)]);
		}

		
	}
}
