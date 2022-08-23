using System;
using System.Collections.Generic;
using UnityEngine;

public class KartSound : RcVehicleSnd
{
	public enum EKartSounds
	{
		SND_MINIBOOST_OK = 7,
		SND_FALL,
		SND_FALL_WATER,
		SND_TAKE_BONUS,
		SND_CRASHED,
		SND_SLIDE,
		SND_ASLEEP,
		SND_TELEPORTED,
		SND_LEVITATE,
		SND_SPRING,
		SND_LASAGNA,
		SND_TAKE_COINS,
		SND_SKID2
	}

	public enum EVoices
	{
		Awake,
		Snore,
		Good,
		Good2,
		Bad,
		Bad2,
		Collision,
		Collision2,
		Selection
	}

	public LayerMask WaterLayer;

	public LayerMask KillLayer;

	private ECharacter m_eCharacter;

	private int m_iNbGoodVoices;

	private int m_iNbBadVoices;

	private int m_iNbColVoices;

	private int m_iLastRank;

	public AudioSource VoiceAudioSource;

	[HideInInspector]
	public List<AudioClip> VoiceList = new List<AudioClip>();

	private bool m_bCanSpeak = true;

	public bool CanSpeak
	{
		get
		{
			return m_bCanSpeak;
		}
		set
		{
			m_bCanSpeak = value;
		}
	}

	public ECharacter Character
	{
		get
		{
			return m_eCharacter;
		}
		set
		{
			m_eCharacter = value;
		}
	}

	public override void Start()
	{
		base.Start();
		CanSpeak = false;
		m_iLastRank = -1;
		int length = Enum.GetValues(typeof(EVoices)).Length;
		m_iNbBadVoices = (m_iNbColVoices = (m_iNbGoodVoices = 0));
		for (int i = 0; i < length; i++)
		{
			if (((EVoices)i).ToString().Contains("Good"))
			{
				m_iNbGoodVoices++;
			}
			else if (((EVoices)i).ToString().Contains("Bad"))
			{
				m_iNbBadVoices++;
			}
			else if (((EVoices)i).ToString().Contains("Collision"))
			{
				m_iNbColVoices++;
			}
		}
	}

	public override void Update()
	{
		base.Update();
		Kart kart = (Kart)m_pVehicle;
		for (int i = 0; i < kart.GetVehiclePhysic().GetNbWheels(); i++)
		{
			int surface = kart.GetGroundCharac(i).surface;
			if (((1 << surface) & WaterLayer.value) != 0)
			{
				PlaySound(9);
			}
			else if (((1 << surface) & KillLayer.value) != 0)
			{
				PlaySound(8);
			}
		}
		int rank = kart.RaceStats.GetRank();
		if (m_iLastRank == rank)
		{
			return;
		}
		if (rank < m_iLastRank)
		{
			int num = Singleton<RandomManager>.Instance.Next(0, 4);
			if (num < 3)
			{
				kart.KartSound.PlayVoice(EVoices.Good);
			}
		}
		m_iLastRank = rank;
	}

	public void StartVoices()
	{
		CanSpeak = true;
	}

	public void PlayVoice(EVoices eVoice)
	{
		if (!CanSpeak)
		{
			return;
		}
		int num = 0;
		if (eVoice == EVoices.Good || eVoice == EVoices.Bad || eVoice == EVoices.Collision)
		{
			RandomManager instance = Singleton<RandomManager>.Instance;
			int num2;
			switch (eVoice)
			{
			case EVoices.Good:
				num2 = m_iNbGoodVoices;
				break;
			case EVoices.Bad:
				num2 = m_iNbBadVoices;
				break;
			default:
				num2 = m_iNbColVoices;
				break;
			}
			num = instance.Next(0, num2 - 1);
		}
		int index = (int)((int)(eVoice + num) * (Enum.GetValues(typeof(ECharacter)).Length - 1) + m_eCharacter);
		if (VoiceList[index] != null && VoiceAudioSource != null && !VoiceAudioSource.isPlaying)
		{
			VoiceAudioSource.clip = VoiceList[index];
			VoiceAudioSource.Play();
			if (eVoice == EVoices.Snore)
			{
				VoiceAudioSource.loop = true;
			}
		}
	}

	public void StopVoice()
	{
		if (VoiceAudioSource != null && VoiceAudioSource.isPlaying)
		{
			VoiceAudioSource.Stop();
			VoiceAudioSource.loop = false;
		}
	}

	public override void ApplyVolume()
	{
		base.ApplyVolume();
		float sfxVolume = Singleton<GameOptionManager>.Instance.GetSfxVolume();
		if ((bool)VoiceAudioSource)
		{
			VoiceAudioSource.volume = sfxVolume;
		}
	}

	public override void PlayCollisionSound(bool bOther)
	{
		base.PlayCollisionSound(bOther);
		PlayVoice(EVoices.Collision);
	}

	public override int GetSkidSound()
	{
		Kart kart = (Kart)m_pVehicle;
		if (kart.MiniBoost < kart.ThresholdMiniBoost)
		{
			StopSound(19);
			return base.GetSkidSound();
		}
		StopSound(3);
		return 19;
	}
}
