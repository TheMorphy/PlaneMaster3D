using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SoundSystem : MonoBehaviour
{
	//Singleton
	public static SoundSystem instance;


	[SerializeField] [Range(0, 100)]
	float maxVolume;
	public List<Sound> sounds = new List<Sound>();
	[Space(20)]
	[Header("System Variables")]
	[SerializeField]
	Slider volumeSlider;

	//Music Handler
	[Header("Music")]
	[SerializeField] [Range(0, 100)]
	float musicVolume;
	AudioSource musicSource;
	[SerializeField]
	List<AudioClip> startingMusic = new List<AudioClip>(), longSessionMusic = new List<AudioClip>();
	Slider musicVolumeSlider;

	

	[Space(100)]
	[SerializeField]
	bool debug;

	
    void UpdateVolumes()
	{
        foreach(Sound s in sounds)
		{
            foreach(AudioSource a in s.sources)
			{
                a.volume = (maxVolume / 100) * (s.relVolume / 100);
            }
        }
	}
	private void Update()
	{
		if(debug)
		{
            UpdateVolumes();
            UpdateMusicVolume();    
		}
	}

	IEnumerator MusicHandler()
	{
        List<AudioClip> playedClips = new List<AudioClip>();
        yield return new WaitForSeconds(5);
        //Play random start song
        AudioClip clipToPlay = startingMusic[Random.Range(0, startingMusic.Count)];
        playedClips.Add(clipToPlay);
        musicSource.PlayOneShot(clipToPlay);
        yield return new WaitWhile(() => musicSource.isPlaying);
        //Play second random start song
        if(startingMusic.Count > 1)
		{
            while (playedClips.Contains(clipToPlay))
            {
                clipToPlay = startingMusic[Random.Range(0, startingMusic.Count)];
                yield return null;
            }
            playedClips.Add(clipToPlay);
            musicSource.PlayOneShot(startingMusic[Random.Range(0, startingMusic.Count)]);
            yield return new WaitWhile(() => musicSource.isPlaying);
        }

        while(true)
		{

            do
            {
                if (playedClips.Count >= longSessionMusic.Count)
                {
                    playedClips.Clear();
                    playedClips.Add(clipToPlay);
                }

                clipToPlay = longSessionMusic[Random.Range(0, longSessionMusic.Count)];
                yield return null;
            } while (playedClips.Contains(clipToPlay));



            musicSource.PlayOneShot(clipToPlay);

            yield return new WaitWhile(() => musicSource.isPlaying);
            yield return null;
		}

    }

    void UpdateMusicVolume()
	{
        musicSource.volume = musicVolume / 100;
	}

    public void OnMusicVolumeChange()
	{
        musicVolume = musicVolumeSlider.value;
        UpdateMusicVolume();
	}
	private void Awake()
	{
		instance = this;
	}

	void Start()
    {
        instance = this;
        musicSource = GetComponent<AudioSource>();
        UpdateVolumes();
        UpdateMusicVolume();
        StartCoroutine(MusicHandler());
    }

    public void OnVolumeChange()
	{
        maxVolume = volumeSlider.value;
        UpdateVolumes();
	}
}

[System.Serializable]
public class Sound
{
    public string name;
    public List<AudioSource> sources = new List<AudioSource>();
    [Range (0, 100)]
    public float relVolume;
}
