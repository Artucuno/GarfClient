using System;
using UnityEngine;

public class KartArcadeGearBox : RcArcadeGearBox
{
	public float MaxSpeed;

	public float MaxAcceleration;

	public float[] PercentDifficulty = new float[Enum.GetValues(typeof(EDifficulty)).Length];

	private float m_fSpeedDiff;

	private float m_fAccelerationDiff;

	private EDifficulty m_eLastDifficulty;

	private float m_BoostTimer;

	private float m_BoostSpeedUp;

	private float m_BoostAcceleration;

	private float m_SlowedDownTimer;

	private float m_SlowedDownMaxSpeed;

	private float m_SlowedDownAcceleration;

	private float m_ParfumeBoostTimer;

	private float m_ParfumeBoostSpeedUp;

	private PlayerCarac m_pPlayerCarac;

	private PlayerCustom m_pCustom;

	private float m_fDifficultyMaxSpeed;

	public KartArcadeGearBox()
	{
		m_pPlayerCarac = null;
		MaxSpeed = 0f;
		m_fSpeedDiff = -1f;
	}

	public override void Awake()
	{
		base.Awake();
		m_eLastDifficulty = Singleton<GameConfigurator>.Instance.Difficulty;
		m_pCustom = base.transform.parent.FindChild("Base").GetComponent<PlayerCustom>();
		m_pPlayerCarac = base.transform.parent.FindChild("Tunning").GetComponent<PlayerCarac>();
		m_fAccelerationDiff = m_vAcceleration[m_vAcceleration.Count - 1] - MaxAcceleration;
		ComputeDiffSpeed();
	}

	public void ComputeDiffSpeed()
	{
		if (m_vSpeed.Count > 0)
		{
			float num = PercentDifficulty[(int)m_eLastDifficulty] / 100f;
			float num2 = m_vSpeed[m_vSpeed.Count - 1] + num * m_vSpeed[m_vSpeed.Count - 1];
			m_fDifficultyMaxSpeed = MaxSpeed + num * MaxSpeed;
			m_fSpeedDiff = m_fDifficultyMaxSpeed - num2;
		}
	}

	public float GetDifficultyPercent()
	{
		return PercentDifficulty[(int)m_eLastDifficulty];
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		if (m_BoostTimer > 0f)
		{
			m_BoostTimer -= deltaTime;
		}
		else if (m_BoostTimer != 0f)
		{
			m_BoostTimer = 0f;
			m_BoostSpeedUp = 0f;
			m_BoostAcceleration = 0f;
			Kart kart = (Kart)m_pVehicle;
			kart.FxMgr.StopKartFx(eKartFx.BoostLeft);
			kart.FxMgr.StopKartFx(eKartFx.BoostRight);
			if (kart.GetControlType() == RcVehicle.ControlType.Human)
			{
				Camera.mainCamera.GetComponent<CamStateFollow>().bBoost = false;
				ParticleSystem componentInChildren = Camera.mainCamera.GetComponentInChildren<ParticleSystem>();
				if ((bool)componentInChildren)
				{
					componentInChildren.Stop();
				}
			}
		}
		if (m_SlowedDownTimer > 0f)
		{
			m_SlowedDownTimer -= deltaTime;
		}
		else if (m_SlowedDownTimer != 0f)
		{
			m_SlowedDownTimer = 0f;
			m_SlowedDownAcceleration = 0f;
			m_SlowedDownMaxSpeed = 0f;
			if (m_pVehicle.GetControlType() == RcVehicle.ControlType.Human)
			{
				Camera.mainCamera.GetComponent<CamStateFollow>().bSlow = false;
			}
		}
		if (m_ParfumeBoostTimer > 0f)
		{
			m_ParfumeBoostTimer -= deltaTime;
		}
		else if (m_ParfumeBoostTimer != 0f)
		{
			m_ParfumeBoostTimer = 0f;
			m_ParfumeBoostSpeedUp = 0f;
		}
	}

	public void SetDefaultValues()
	{
		m_BoostTimer = -1f;
		m_BoostSpeedUp = 0f;
		m_BoostAcceleration = 0f;
		m_SlowedDownTimer = 0f;
		m_SlowedDownMaxSpeed = 0f;
		m_SlowedDownAcceleration = 0f;
		m_ParfumeBoostTimer = -1f;
		m_ParfumeBoostSpeedUp = 0f;
	}

	public override float GetMaxSpeed()
	{
		float num = base.GetMaxSpeed();
		if (m_SlowedDownTimer > 0f && num > m_SlowedDownMaxSpeed)
		{
			num = m_SlowedDownMaxSpeed;
		}
		if (m_BoostTimer > 0f || m_BoostTimer == -1f)
		{
			num += m_BoostSpeedUp;
		}
		else if (m_ParfumeBoostTimer > 0f)
		{
			num += m_ParfumeBoostSpeedUp;
		}
		return num;
	}

	public float GetBaseMaxSpeed()
	{
		return base.GetMaxSpeed();
	}

	public override float ComputeAcceleration(float _speedMS)
	{
		float num = base.ComputeAcceleration(_speedMS);
		if (m_BoostTimer > 0f || m_BoostTimer == -1f)
		{
			return num * m_BoostAcceleration;
		}
		if (m_SlowedDownTimer > 0f)
		{
			return num * m_SlowedDownAcceleration;
		}
		return num;
	}

	public void Boost(float _speedUpMs, float _boostDelay, float _BoostAcceleration, bool bWithEffect)
	{
		m_BoostSpeedUp = Mathf.Max(m_BoostSpeedUp, _speedUpMs * base.GetMaxSpeed() / 100f);
		m_BoostTimer = Mathf.Max(m_BoostTimer, _boostDelay);
		m_BoostAcceleration = Mathf.Max(m_BoostAcceleration, _BoostAcceleration);
		Kart kart = (Kart)m_pVehicle;
		if (bWithEffect)
		{
			kart.FxMgr.Boost();
		}
		if (kart.GetControlType() == RcVehicle.ControlType.Human && bWithEffect)
		{
			Camera.mainCamera.GetComponent<CamStateFollow>().bBoost = true;
			ParticleSystem componentInChildren = Camera.mainCamera.GetComponentInChildren<ParticleSystem>();
			if ((bool)componentInChildren)
			{
				componentInChildren.Play();
			}
		}
	}

	public void ParfumeBoost(float _speedUpMs, float _boostDelay)
	{
		m_ParfumeBoostSpeedUp = Mathf.Max(m_ParfumeBoostSpeedUp, _speedUpMs * base.GetMaxSpeed() / 100f);
		m_ParfumeBoostTimer = Mathf.Max(m_ParfumeBoostTimer, _boostDelay);
	}

	public void SlowDown(float _SlowedDownMaxSpeed, float _SlowedDownDelay, float _SlowedDownDownMs)
	{
		m_SlowedDownMaxSpeed = base.GetMaxSpeed() - _SlowedDownMaxSpeed * base.GetMaxSpeed() / 100f;
		m_SlowedDownTimer = _SlowedDownDelay;
		m_SlowedDownAcceleration = _SlowedDownDownMs;
		if (m_pVehicle.GetControlType() == RcVehicle.ControlType.Human)
		{
			Camera.mainCamera.GetComponent<CamStateFollow>().bSlow = true;
		}
	}

	public bool IsBoosting()
	{
		return m_BoostTimer > 0f;
	}

	public bool IsParfume()
	{
		return m_ParfumeBoostTimer > 0f;
	}

	public void ApplySpeedAdvantages(int _Index)
	{
		float speed = base.GetSpeed(_Index);
		if (!(speed <= 0f))
		{
			if (m_fSpeedDiff == -1f || m_eLastDifficulty != Singleton<GameConfigurator>.Instance.Difficulty)
			{
				m_eLastDifficulty = Singleton<GameConfigurator>.Instance.Difficulty;
				ComputeDiffSpeed();
			}
			float num = speed + PercentDifficulty[(int)m_eLastDifficulty] * speed / 100f;
			float percentAdvantages = GetPercentAdvantages(DrivingCaracteristics.SPEED);
			num += percentAdvantages * m_fSpeedDiff / 100f;
			m_vSpeed[_Index] = num;
		}
	}

	public void ApplyAccelerationAdvantages(int _Index)
	{
		float num = base.GetAcceleration(_Index);
		if (num != 0f)
		{
			float percentAdvantages = GetPercentAdvantages(DrivingCaracteristics.ACCELERATION);
			num -= percentAdvantages * m_fAccelerationDiff / 100f;
		}
		m_vAcceleration[_Index] = num;
	}

	public float GetSpeedDiff()
	{
		return m_fSpeedDiff;
	}

	public float GetPercentAdvantages(DrivingCaracteristics _Carac)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		if (m_pPlayerCarac != null && m_pPlayerCarac.KartCarac != null)
		{
			num = m_pPlayerCarac.KartCarac.GetCarac(_Carac);
			if (m_pPlayerCarac.CharacterCarac != null && m_pPlayerCarac.KartCarac.BonusCaracteristic == _Carac)
			{
				num += ((m_pPlayerCarac.CharacterCarac.Owner != m_pPlayerCarac.KartCarac.Owner) ? 0f : m_pPlayerCarac.KartCarac.Bonus);
			}
		}
		if (m_pPlayerCarac != null && m_pPlayerCarac.CharacterCarac != null)
		{
			num2 = m_pPlayerCarac.CharacterCarac.GetCarac(_Carac);
		}
		if (m_pCustom != null && m_pCustom.KartCustom != null)
		{
			num3 = m_pCustom.KartCustom.GetCarac(_Carac);
			if (m_pPlayerCarac != null && m_pPlayerCarac.KartCarac != null && m_pCustom.KartCustom.BonusCaracteristic == _Carac)
			{
				num3 += ((m_pPlayerCarac.KartCarac.Owner != m_pCustom.KartCustom.Owner) ? 0f : m_pCustom.KartCustom.Bonus);
			}
		}
		return num + num2 + num3;
	}

	public void StartScene()
	{
		for (int i = 0; i < m_vSpeed.Count; i++)
		{
			ApplySpeedAdvantages(i);
		}
		for (int j = 0; j < m_vAcceleration.Count; j++)
		{
			ApplyAccelerationAdvantages(j);
		}
	}

	public float GetDifficultyMaxSpeed()
	{
		return m_fDifficultyMaxSpeed;
	}
}
