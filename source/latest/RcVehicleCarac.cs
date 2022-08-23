using System.Collections.Generic;
using UnityEngine;

public class RcVehicleCarac : MonoBehaviour
{
	public float m_fHardTerrainBrakingTime;

	public float m_fBrakingTime;

	public float m_fDecelerationTime;

	public float m_fTimeToMaxSteering;

	public float m_fDriftTimeToMaxSteering;

	public float m_fResetSteeringNoInput;

	public float m_fDriftResetSteeringNoInput;

	public float m_fResetSteeringOppositeInput;

	public float m_fSteeringTopSpeedMalusPrc;

	protected float m_fBrakingMSS;

	protected float m_fHardTerrainBrakingMSS;

	protected float m_fDecelerationMSS;

	public List<float> m_vSpeedRef;

	public List<float> m_vTurningRadius;

	public List<float> m_vDriftTurningRadius;

	public List<float> m_vDriftNoInputTurningRadius;

	public List<float> m_vCounterSteeringTurningRadius;

	public RcVehicleCarac()
	{
		m_fBrakingTime = 5f;
		m_fHardTerrainBrakingTime = 1f;
		m_fDecelerationTime = 10f;
		m_fTimeToMaxSteering = 0.3f;
		m_fDriftTimeToMaxSteering = 0.5f;
		m_fDriftResetSteeringNoInput = 1f;
		m_fResetSteeringNoInput = 1f;
		m_fResetSteeringOppositeInput = 0f;
		m_fSteeringTopSpeedMalusPrc = 0.1f;
		m_vSpeedRef = new List<float>();
		m_vTurningRadius = new List<float>();
		m_vDriftTurningRadius = new List<float>();
		m_vDriftNoInputTurningRadius = new List<float>();
		m_vCounterSteeringTurningRadius = new List<float>();
	}

	public virtual void Start()
	{
		RcVehicle componentInChildren = base.transform.parent.GetComponentInChildren<RcVehicle>();
		if (componentInChildren != null)
		{
			m_fBrakingMSS = componentInChildren.GetMaxSpeed() / m_fBrakingTime;
			m_fHardTerrainBrakingMSS = componentInChildren.GetMaxSpeed() / m_fHardTerrainBrakingTime;
			m_fDecelerationMSS = componentInChildren.GetMaxSpeed() / m_fDecelerationTime;
		}
		if (m_vDriftTurningRadius.Count == 0)
		{
			foreach (float item4 in m_vTurningRadius)
			{
				float item = item4;
				m_vDriftTurningRadius.Add(item);
			}
		}
		if (m_vDriftNoInputTurningRadius.Count == 0)
		{
			foreach (float item5 in m_vTurningRadius)
			{
				float item2 = item5;
				m_vDriftNoInputTurningRadius.Add(item2);
			}
		}
		if (m_vCounterSteeringTurningRadius.Count != 0)
		{
			return;
		}
		foreach (float item6 in m_vTurningRadius)
		{
			float item3 = item6;
			m_vCounterSteeringTurningRadius.Add(item3);
		}
	}

	public float GetTimeToStop(float fromSpeed)
	{
		if (GetBrakingMSS() != 0f)
		{
			return Mathf.Abs(fromSpeed) / GetBrakingMSS();
		}
		return 0f;
	}

	public void ComputeHandling(float _speedMS, out VehicleHandling _handlingOut)
	{
		_handlingOut.speedMS = _speedMS;
		int i = 0;
		int count;
		for (count = m_vSpeedRef.Count; i < count && _speedMS >= GetSpeedRef(i) / 3.6f; i++)
		{
		}
		if (i != 0)
		{
			i--;
		}
		float num = GetSpeedRef(i) / 3.6f;
		if (i < count - 1)
		{
			_handlingOut.minTurningRadius = RcUtils.LinearInterpolation(num, m_vTurningRadius[i], GetSpeedRef(i + 1) / 3.6f, m_vTurningRadius[i + 1], _speedMS);
			_handlingOut.driftTurningRadius = RcUtils.LinearInterpolation(num, m_vDriftTurningRadius[i], GetSpeedRef(i + 1) / 3.6f, m_vDriftTurningRadius[i + 1], _speedMS);
			_handlingOut.driftNoInputTurningRadius = RcUtils.LinearInterpolation(num, m_vDriftNoInputTurningRadius[i], GetSpeedRef(i + 1) / 3.6f, m_vDriftNoInputTurningRadius[i + 1], _speedMS);
			_handlingOut.counterSteeringTurningRadius = RcUtils.LinearInterpolation(num, m_vCounterSteeringTurningRadius[i], GetSpeedRef(i + 1) / 3.6f, m_vCounterSteeringTurningRadius[i + 1], _speedMS);
		}
		else if (i > 0)
		{
			_handlingOut.minTurningRadius = RcUtils.LinearInterpolation(GetSpeedRef(i - 1) / 3.6f, m_vTurningRadius[i - 1], num, m_vTurningRadius[i], _speedMS);
			_handlingOut.driftTurningRadius = RcUtils.LinearInterpolation(GetSpeedRef(i - 1) / 3.6f, m_vDriftTurningRadius[i - 1], num, m_vDriftTurningRadius[i], _speedMS);
			_handlingOut.driftNoInputTurningRadius = RcUtils.LinearInterpolation(GetSpeedRef(i - 1) / 3.6f, m_vDriftNoInputTurningRadius[i - 1], num, m_vDriftNoInputTurningRadius[i], _speedMS);
			_handlingOut.counterSteeringTurningRadius = RcUtils.LinearInterpolation(GetSpeedRef(i - 1) / 3.6f, m_vCounterSteeringTurningRadius[i - 1], num, m_vCounterSteeringTurningRadius[i], _speedMS);
		}
		else
		{
			_handlingOut.minTurningRadius = m_vTurningRadius[i];
			_handlingOut.driftTurningRadius = m_vDriftTurningRadius[i];
			_handlingOut.driftNoInputTurningRadius = m_vDriftNoInputTurningRadius[i];
			_handlingOut.counterSteeringTurningRadius = m_vCounterSteeringTurningRadius[i];
		}
		_handlingOut.timeToMaxSteering = GetTimeToMaxSteering();
		_handlingOut.driftTimeToMaxSteering = GetTimeToMaxSteering();
		_handlingOut.resetSteeringNoInput = GetResetSteeringNoInput();
		_handlingOut.driftResetSteeringNoInput = GetDriftResetSteeringNoInput();
		_handlingOut.resetSteeringOppositeInput = GetResetSteeringOppositeInput();
		_handlingOut.brakingMSS = GetBrakingMSS();
		_handlingOut.toofastBrakingMSS = GetHardTerrainBrakingMSS();
		_handlingOut.decelerationMSS = GetDecelerationMSS();
		_handlingOut.steeringTopSpeedMalus = GetSteeringTopSpeedMalusPrc();
	}

	public float ComputeMinRadius(float _speedMS)
	{
		int i = 0;
		int count;
		for (count = m_vSpeedRef.Count; i < count && _speedMS >= GetSpeedRef(i) / 3.6f; i++)
		{
		}
		i--;
		if (i < 0)
		{
			i = 0;
		}
		float num = GetSpeedRef(i) / 3.6f;
		if (i < count - 1)
		{
			return RcUtils.LinearInterpolation(num, m_vTurningRadius[i], GetSpeedRef(i + 1) / 3.6f, m_vTurningRadius[i + 1], _speedMS);
		}
		if (i > 0)
		{
			return RcUtils.LinearInterpolation(GetSpeedRef(i - 1) / 3.6f, m_vTurningRadius[i - 1], num, m_vTurningRadius[i], _speedMS);
		}
		return m_vTurningRadius[i];
	}

	public float ComputeMaxSpeedForRadius(float _radius)
	{
		int i = 0;
		int count;
		for (count = m_vTurningRadius.Count; i < count && _radius >= m_vTurningRadius[i]; i++)
		{
		}
		i--;
		if (i < 0)
		{
			return GetSpeedRef(0) / 3.6f;
		}
		if (i < count - 1)
		{
			return RcUtils.LinearInterpolation(m_vTurningRadius[i], GetSpeedRef(i) / 3.6f, m_vTurningRadius[i + 1], GetSpeedRef(i + 1) / 3.6f, _radius);
		}
		if (i > 0)
		{
			return RcUtils.LinearInterpolation(m_vTurningRadius[i - 1], GetSpeedRef(i - 1) / 3.6f, m_vTurningRadius[i], GetSpeedRef(i) / 3.6f, _radius);
		}
		return GetSpeedRef(i) / 3.6f;
	}

	public virtual float GetSpeedRef(int _Index)
	{
		return m_vSpeedRef[_Index];
	}

	public float GetSteeringTopSpeedMalusPrc()
	{
		return m_fSteeringTopSpeedMalusPrc;
	}

	public virtual float GetDecelerationMSS()
	{
		return m_fDecelerationMSS;
	}

	public virtual float GetBrakingMSS()
	{
		return m_fBrakingMSS;
	}

	public virtual float GetHardTerrainBrakingMSS()
	{
		return m_fHardTerrainBrakingMSS;
	}

	public virtual float GetTimeToMaxSteering()
	{
		return m_fTimeToMaxSteering;
	}

	public virtual float GetDriftTimeToMaxSteering()
	{
		return m_fDriftTimeToMaxSteering;
	}

	public virtual float GetResetSteeringNoInput()
	{
		return m_fResetSteeringNoInput;
	}

	public virtual float GetDriftResetSteeringNoInput()
	{
		return m_fDriftResetSteeringNoInput;
	}

	public virtual float GetResetSteeringOppositeInput()
	{
		return m_fResetSteeringOppositeInput;
	}
}
