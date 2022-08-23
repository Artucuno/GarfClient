using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public List<AudioSource> SoundsList = new List<AudioSource>();

	public AudioSource Music;

	public List<AudioClip> MusicsList = new List<AudioClip>();

	public void Awake()
	{
		Music.ignoreListenerPause = true;
	}

	public void Update()
	{
		if (Music.clip != null && !Music.isPlaying && LoadingManager.loadingFinished)
		{
			LoadingManager.loadingFinished = false;
			Music.Play();
		}
	}

	public void SetMusic(ERaceMusicLoops _Music)
	{
		Music.clip = MusicsList[(int)_Music];
	}

	public void PlayMusic()
	{
		if (Music.clip != null && !Music.isPlaying)
		{
			Music.Play();
		}
	}

	public void PlayMusic(ERaceMusicLoops _Music)
	{
		SetMusic(_Music);
		if (_Music != ERaceMusicLoops.InterRace && _Music != 0)
		{
			PlayMusic();
		}
	}

	public void StopMusic()
	{
		if (Music.isPlaying)
		{
			Music.Stop();
		}
	}

	public void PlaySound(ERaceSounds _Sound)
	{
		if (SoundsList[(int)_Sound] != null && !SoundsList[(int)_Sound].isPlaying)
		{
			SoundsList[(int)_Sound].Play();
		}
	}

	public void StopSound(ERaceSounds _Sound)
	{
		if (SoundsList[(int)_Sound] != null && SoundsList[(int)_Sound].isPlaying)
		{
			SoundsList[(int)_Sound].Stop();
		}
	}

	public void ApplyVolume()
	{
		float sfxVolume = Singleton<GameOptionManager>.Instance.GetSfxVolume();
		for (int i = 0; i < SoundsList.Count; i++)
		{
			if ((bool)SoundsList[i])
			{
				SoundsList[i].volume = sfxVolume;
			}
		}
	}
}
