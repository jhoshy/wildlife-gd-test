using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
	public static AudioController Singleton;

	public SongProfile songProfile;
	public AudioSource source;

	public float CurrentBeatDelay => 60 / songProfile.BPM;

	private void Awake()
	{
		Singleton = this;
	}

	void Start()
    {
		source = GetComponent<AudioSource>();

		//debug
		Play();
	}

	public void Play()
	{
		source.clip = songProfile.clip;
		source.Play();
	}
}