using System.Collections.Generic;
using UnityEngine;

public class RcVehicleSnd : MonoBehaviour, RcCollisionListener
{
	public enum eVehicleSounds
	{
		SND_MOTOR,
		SND_SHIFT,
		SND_BRAKE,
		SND_SKID,
		SND_COLL1,
		SND_COLL2,
		SND_BOOST
	}

	public const eVehicleSounds LastVehicleSound = eVehicleSounds.SND_BOOST;

	[HideInInspector]
	public List<AudioSource> SoundsList = new List<AudioSource>();

	public List<SurfaceSounds> SurfaceSoundsList = new List<SurfaceSounds>();

	protected RcVehicle m_pVehicle;

	private RcGearBox m_pGearBox;

	private float m_fBrakingTimer;

	[HideInInspector]
	public float m_fEngineOriginalRpm;

	[HideInInspector]
	public float m_fSkidVolume;

	[HideInInspector]
	public float m_fBrakeVolume;

	[HideInInspector]
	public float m_fMinDist;

	[HideInInspector]
	public float m_fMaxDist;

	public RcVehicleSnd()
	{
		m_pVehicle = null;
		m_pGearBox = null;
		m_fEngineOriginalRpm = 0f;
		m_fSkidVolume = 0f;
		m_fBrakeVolume = 0f;
		m_fMinDist = 0f;
		m_fMaxDist = 0f;
		m_fBrakingTimer = 0f;
	}

	public void Awake()
	{
		m_pVehicle = base.transform.parent.GetComponentInChildren<RcVehicle>();
		m_pGearBox = base.transform.parent.GetComponentInChildren<RcGearBox>();
	}

	public void OnDestroy()
	{
		Stop();
	}

	public virtual void Start()
	{
		if ((bool)m_pVehicle)
		{
			m_pVehicle.AddCollisionListener(this);
		}
		ApplyVolume();
	}

	public void StartSound()
	{
		InitSound(0);
	}

	public void Stop()
	{
		if ((bool)m_pVehicle)
		{
			m_pVehicle.RemoveCollisionListener(this);
			m_pVehicle = null;
		}
		for (int i = 0; i < SoundsList.Count; i++)
		{
			TermSound(i);
		}
	}

	public void InitSound(int _soundIndex)
	{
		if (_soundIndex != 4 && _soundIndex != 5 && _soundIndex != 1)
		{
			PlaySound(_soundIndex);
		}
	}

	public void TermSound(int _soundIndex)
	{
		if (SoundsList[_soundIndex] != null && SoundsList[_soundIndex].isPlaying)
		{
			SoundsList[_soundIndex].Stop();
		}
	}

	public virtual void Update()
	{
		bool flag = m_pVehicle.IsOnGround();
		if (SoundsList[0] != null)
		{
			float num = m_pGearBox.ComputeRpm(m_pVehicle.GetWheelSpeedMS());
			SoundsList[0].pitch = num / m_fEngineOriginalRpm;
		}
		int skidSound = GetSkidSound();
		float driftRatio = m_pVehicle.GetDriftRatio();
		if (Mathf.Abs(driftRatio) > 0f && m_pVehicle.GetGroundSurface() != 0)
		{
			PlaySound(skidSound);
			if (SoundsList[skidSound] != null)
			{
				SoundsList[skidSound].volume = Mathf.Clamp(m_fSkidVolume * Mathf.Abs(driftRatio), 0f, 1f);
			}
		}
		else
		{
			StopSound(skidSound);
		}
		float num2 = Mathf.Abs(m_pVehicle.GetMotorSpeedMS());
		float moveFactor = m_pVehicle.GetMoveFactor();
		if (moveFactor <= -0.5f && m_pVehicle.GetMotorSpeedMS() > 0f && flag)
		{
			m_fBrakingTimer += Time.deltaTime;
		}
		else
		{
			m_fBrakingTimer = 0f;
		}
		bool flag2 = m_pVehicle.GetDrift() > 0f && m_pVehicle.IsOnGround() && m_pVehicle.GetMotorSpeedMS() > 0f;
		if (m_fBrakingTimer > 0.3f || flag2)
		{
			PlaySound(2);
			if (SoundsList[2] != null)
			{
				float value = m_fBrakingTimer / 2f;
				SoundsList[2].volume = Mathf.Clamp(Mathf.Clamp(value, 0f, 1f) * 2f * m_fBrakeVolume, 0f, 1f);
			}
		}
		else
		{
			StopSound(2);
		}
		for (int i = 0; i < SurfaceSoundsList.Count; i++)
		{
			bool flag3 = false;
			if (flag && num2 > 1f)
			{
				for (int j = 0; j < m_pVehicle.GetVehiclePhysic().GetNbWheels(); j++)
				{
					int surface = m_pVehicle.GetGroundCharac(j).surface;
					if (((1 << surface) & SurfaceSoundsList[i].Mask.value) != 0 && SurfaceSoundsList[i].Sound != null)
					{
						flag3 = true;
						if (!SurfaceSoundsList[i].Sound.isPlaying)
						{
							SurfaceSoundsList[i].Sound.minDistance = m_fMinDist;
							SurfaceSoundsList[i].Sound.maxDistance = m_fMaxDist;
							SurfaceSoundsList[i].Sound.Play();
						}
						SurfaceSoundsList[i].Sound.pitch = RcUtils.LinearInterpolation(2f, 0.5f, m_pVehicle.GetMaxSpeed(), 2f, num2, true);
					}
				}
			}
			if (SurfaceSoundsList[i].Sound != null && SurfaceSoundsList[i].Sound.isPlaying && !flag3)
			{
				SurfaceSoundsList[i].Sound.Stop();
			}
		}
	}

	public void PlaySound(int _Sound)
	{
		if (!(SoundsList[_Sound] != null))
		{
			return;
		}
		if (SoundsList[_Sound].isPlaying)
		{
			if (_Sound == 4 || _Sound == 5 || _Sound == 1)
			{
				SoundsList[_Sound].Stop();
				SoundsList[_Sound].Play();
			}
		}
		else
		{
			SoundsList[_Sound].pitch = 1f;
			SoundsList[_Sound].minDistance = m_fMinDist;
			SoundsList[_Sound].maxDistance = m_fMaxDist;
			SoundsList[_Sound].Play();
		}
	}

	public void PlaySoundImmediately(int _Sound)
	{
		if (SoundsList[_Sound] != null)
		{
			SoundsList[_Sound].Play();
		}
	}

	public void StopSound(int _Sound)
	{
		if (SoundsList[_Sound] != null && SoundsList[_Sound].isPlaying)
		{
			SoundsList[_Sound].Stop();
		}
	}

	public void OnCollision(CollisionData collisionInfo)
	{
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		if ((bool)m_pVehicle)
		{
			vector = m_pVehicle.GetVehiclePhysic().GetLinearVelocity();
		}
		if (collisionInfo.other != null)
		{
			vector2 = collisionInfo.other.velocity;
		}
		float f = Vector3.Dot(vector - vector2, collisionInfo.normal);
		if (Mathf.Abs(f) > 2f && !m_pVehicle.IsLocked())
		{
			PlayCollisionSound(collisionInfo.other != null);
		}
	}

	public virtual void PlayCollisionSound(bool bOther)
	{
		float num = Random.Range(-0.5f, 0.5f);
		int num2 = -1;
		num2 = ((!bOther) ? 5 : 4);
		PlaySound(num2);
		SoundsList[num2].pitch = 1f + num;
	}

	public void PauseSounds(bool _Pause)
	{
		if ((bool)m_pVehicle.AudioListener && m_pVehicle.AudioListener.enabled)
		{
			AudioListener.pause = _Pause;
		}
	}

	public virtual void ApplyVolume()
	{
		float sfxVolume = Singleton<GameOptionManager>.Instance.GetSfxVolume();
		for (int i = 0; i < SoundsList.Count; i++)
		{
			if ((bool)SoundsList[i])
			{
				SoundsList[i].volume = sfxVolume;
			}
		}
		for (int j = 0; j < SurfaceSoundsList.Count; j++)
		{
			if ((bool)SurfaceSoundsList[j].Sound)
			{
				SurfaceSoundsList[j].Sound.volume = sfxVolume;
			}
		}
	}

	public virtual int GetSkidSound()
	{
		return 3;
	}
}
