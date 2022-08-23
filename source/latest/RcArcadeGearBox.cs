using System.Collections.Generic;
using UnityEngine;

public class RcArcadeGearBox : RcGearBox
{
	public List<float> m_vAcceleration;

	public List<float> m_vSpeed;

	private int m_iGear;

	public float m_fReverseMaxSpeedKph;

	protected RcVehicle m_pVehicle;

	public RcArcadeGearBox()
	{
		m_vAcceleration = new List<float>();
		m_vSpeed = new List<float>();
		m_iGear = 0;
		m_fReverseMaxSpeedKph = 0f;
	}

	public override float GetBackwardMaxSpeed()
	{
		return (0f - m_fReverseMaxSpeedKph) / 3.6f;
	}

	public override int GetCurrentGear()
	{
		return m_iGear;
	}

	public virtual void Awake()
	{
		m_pVehicle = base.transform.parent.GetComponentInChildren<RcVehicle>();
	}

	public override float ComputeAcceleration(float _speedMS)
	{
		if (_speedMS < 0f)
		{
			return 0f;
		}
		int i;
		for (i = 0; i < m_vSpeed.Count && _speedMS >= GetSpeed(i) / 3.6f; i++)
		{
		}
		m_iGear = i;
		if (i == m_vSpeed.Count)
		{
			m_iGear--;
		}
		if (i != 0)
		{
			i--;
		}
		if (i < 0 || i >= m_vSpeed.Count)
		{
			return 0f;
		}
		for (i = 0; i < m_vSpeed.Count && _speedMS >= GetSpeed(i); i++)
		{
		}
		i--;
		if (i < 0 || i >= m_vSpeed.Count)
		{
			return 0f;
		}
		if (m_vAcceleration.Count == m_vSpeed.Count && i < m_vSpeed.Count - 1)
		{
			float speed = GetSpeed(i + 1);
			if (speed < GetSpeed(i))
			{
				m_vSpeed[i + 1] = GetSpeed(i) + 0.01f;
				return 0f;
			}
			float num = (speed - GetSpeed(i)) / 3.6f;
			float acceleration = GetAcceleration(i + 1);
			if (acceleration < GetAcceleration(i))
			{
				m_vAcceleration[i + 1] = GetAcceleration(i) + 0.01f;
				return 0f;
			}
			return num / (acceleration - GetAcceleration(i));
		}
		return 0f;
	}

	public override float GetMaxSpeed()
	{
		if (m_vSpeed.Count > 0)
		{
			return GetSpeed(m_vSpeed.Count - 1) / 3.6f;
		}
		return 0f;
	}

	public override float ComputeRpm(float speedMs)
	{
		int i;
		for (i = 0; i < m_vSpeed.Count && Mathf.Abs(speedMs) >= GetSpeed(i) / 3.6f; i++)
		{
		}
		if (i != 0)
		{
			i--;
		}
		if (i < m_vSpeed.Count - 1)
		{
			if (i == 0)
			{
				return RcUtils.LinearInterpolation(GetSpeed(i), 2000f, GetSpeed(i + 1), 6000f, 3.6f * Mathf.Abs(speedMs));
			}
			return RcUtils.LinearInterpolation(GetSpeed(i), 3500f, GetSpeed(i + 1), 6000f, 3.6f * Mathf.Abs(speedMs));
		}
		if (i > 0)
		{
			return RcUtils.LinearInterpolation(GetSpeed(i - 1), 3500f, GetSpeed(i), 6000f, 3.6f * Mathf.Abs(speedMs));
		}
		return 2000f;
	}

	public override bool IsGoingTooFast()
	{
		return false;
	}

	public virtual float GetSpeed(int _Index)
	{
		return m_vSpeed[_Index];
	}

	public virtual float GetAcceleration(int _Index)
	{
		return m_vAcceleration[_Index];
	}
}
