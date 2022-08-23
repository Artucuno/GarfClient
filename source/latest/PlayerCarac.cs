using System;
using System.Collections.Generic;

public class PlayerCarac : RcVehicleCarac
{
	public float MaxDecelerationTime;

	public float MaxBrakingTime;

	public float MaxSteeringTime;

	public float MaxResetSteeringNoInput;

	public float MaxDriftResetSteeringNoInput;

	public float MaxResetSteeringOppositeInput;

	public List<float> CharacterWeight;

	private float m_fDecelTimeDiff;

	private float m_fBrakingTimeDiff;

	private float m_fSteeringTimeDiff;

	private float m_fResetSteeringNoInputDiff;

	private float m_fDriftResetSteeringNoInputDiff;

	private float m_fResetSteeringOppositeInputDiff;

	private KartArcadeGearBox m_pKartArcadeGearBox;

	private KartCarac m_pKartCarac;

	private CharacterCarac m_pCharacterCarac;

	private Kart m_pKart;

	public KartCarac KartCarac
	{
		get
		{
			return m_pKartCarac;
		}
		set
		{
			m_pKartCarac = value;
		}
	}

	public CharacterCarac CharacterCarac
	{
		get
		{
			return m_pCharacterCarac;
		}
		set
		{
			m_pCharacterCarac = value;
		}
	}

	public PlayerCarac()
	{
		m_pKartArcadeGearBox = null;
		m_pKart = null;
		CharacterWeight = new List<float>();
	}

	public override void Start()
	{
		base.Start();
		m_pKartArcadeGearBox = base.transform.parent.FindChild("Tunning").GetComponent<KartArcadeGearBox>();
		m_pKart = base.transform.parent.GetComponentInChildren<Kart>();
		m_fDecelTimeDiff = m_fDecelerationTime - MaxDecelerationTime;
		m_fBrakingTimeDiff = m_fBrakingTime - MaxBrakingTime;
		m_fSteeringTimeDiff = m_fTimeToMaxSteering - MaxSteeringTime;
		m_fResetSteeringNoInputDiff = m_fResetSteeringNoInput - MaxResetSteeringNoInput;
		m_fDriftResetSteeringNoInputDiff = m_fDriftResetSteeringNoInput - MaxDriftResetSteeringNoInput;
		m_fResetSteeringOppositeInputDiff = m_fResetSteeringOppositeInput - MaxResetSteeringOppositeInput;
	}

	public void ApplySpeedRefAdvantages(int _Index)
	{
		float speedRef = base.GetSpeedRef(_Index);
		float num = 0f;
		if (m_pKartArcadeGearBox != null)
		{
			num = m_pKartArcadeGearBox.GetDifficultyPercent();
		}
		float num2 = speedRef + num * speedRef / 100f;
		float num3 = 0f;
		if (m_pKartArcadeGearBox != null)
		{
			num3 = m_pKartArcadeGearBox.GetPercentAdvantages(DrivingCaracteristics.SPEED);
			num2 += num3 * m_pKartArcadeGearBox.GetSpeedDiff() / 100f;
		}
		m_vSpeedRef[_Index] = num2;
	}

	public float ApplyManiabilityPercentageOnSpeed(float _Value, float _ValueDiff)
	{
		float num = 0f;
		float num2 = _Value;
		if (num2 != 0f)
		{
			if (m_pKartArcadeGearBox != null)
			{
				num = m_pKartArcadeGearBox.GetPercentAdvantages(DrivingCaracteristics.MANIABILITY);
				num2 -= num * _ValueDiff / 100f;
			}
			return m_pKart.GetMaxSpeed() / num2;
		}
		return 0f;
	}

	public float ApplyManiabilityPercentage(float _Value, float _ValueDiff)
	{
		float num = 0f;
		float num2 = _Value;
		if (num2 != 0f && m_pKartArcadeGearBox != null)
		{
			num = m_pKartArcadeGearBox.GetPercentAdvantages(DrivingCaracteristics.MANIABILITY);
			num2 -= num * _ValueDiff / 100f;
		}
		return num2;
	}

	public void ApplyDecelerationMSSAdvantages()
	{
		m_fDecelerationMSS = ApplyManiabilityPercentageOnSpeed(m_fDecelerationTime, m_fDecelTimeDiff);
	}

	public void ApplyBrakingMSSAdvantages()
	{
		m_fBrakingMSS = ApplyManiabilityPercentageOnSpeed(m_fBrakingTime, m_fBrakingTimeDiff);
	}

	public void ApplyTimeToMaxSteeringAdvantages()
	{
		m_fTimeToMaxSteering = ApplyManiabilityPercentage(base.GetTimeToMaxSteering(), m_fSteeringTimeDiff);
	}

	public void ApplyResetSteeringNoInputAdvantages()
	{
		m_fResetSteeringNoInput = ApplyManiabilityPercentage(base.GetResetSteeringNoInput(), m_fResetSteeringNoInputDiff);
	}

	public void ApplyDriftResetSteeringNoInputAdvantages()
	{
		m_fDriftResetSteeringNoInput = ApplyManiabilityPercentage(base.GetDriftResetSteeringNoInput(), m_fDriftResetSteeringNoInputDiff);
	}

	public void ApplyResetSteeringOppositeInputAdvantages()
	{
		m_fResetSteeringOppositeInput = ApplyManiabilityPercentage(base.GetResetSteeringOppositeInput(), m_fResetSteeringOppositeInputDiff);
	}

	public void ApplyWeight()
	{
		RcKinematicPhysic rcKinematicPhysic = (RcKinematicPhysic)m_pKart.GetVehiclePhysic();
		if (rcKinematicPhysic != null && CharacterWeight.Count >= Enum.GetValues(typeof(EWeight)).Length)
		{
			rcKinematicPhysic.m_fMass = CharacterWeight[(int)CharacterCarac.Weight];
		}
	}

	public void StartScene()
	{
		for (int i = 0; i < m_vSpeedRef.Count; i++)
		{
			ApplySpeedRefAdvantages(i);
		}
		ApplyDecelerationMSSAdvantages();
		ApplyBrakingMSSAdvantages();
		ApplyTimeToMaxSteeringAdvantages();
		ApplyResetSteeringNoInputAdvantages();
		ApplyDriftResetSteeringNoInputAdvantages();
		ApplyResetSteeringOppositeInputAdvantages();
		ApplyWeight();
	}
}
