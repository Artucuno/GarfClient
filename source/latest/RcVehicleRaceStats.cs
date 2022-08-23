using System;
using UnityEngine;

public class RcVehicleRaceStats : MonoBehaviour
{
	private const int MAX_LAP = 16;

	private const int MAX_CHECKPOINTS = 16;

	public bool CanCompleteLap = true;

	protected int m_iRank;

	protected bool m_bReverse;

	protected int m_iNbLapCompleted;

	protected int m_iLogicNbLap;

	protected int m_iRaceTimeMs;

	protected int m_iLapStartTimeMs;

	protected int m_iCurrentLapTime;

	protected int m_iCurrentDiffTime;

	protected int[] m_LapTimeMs;

	protected int m_iIdxLastLapTimeMs;

	protected int[] m_CheckPointTimeMs;

	protected float m_fDistToEndOfLap;

	protected float m_fDistToEndOfRace;

	protected int m_iRaceNbLap;

	protected int m_iRaceNbCheckPoints;

	protected int m_iBestLapTimeMs;

	protected int m_iNbCheckPointValidated;

	protected bool m_bRaceEnded;

	public MultiPathPosition m_GuidePosition;

	protected RcVehicle m_pVehicle;

	protected RcVehicle m_pPrecedingVehicle;

	protected RcVehicle m_pPursuantVehicle;

	protected bool m_bDebugDraw;

	protected RcRace m_pRace;

	public RcVehicleRaceStats()
	{
		m_pVehicle = null;
		m_iRank = 0;
		m_bReverse = false;
		m_iNbLapCompleted = 0;
		m_iLogicNbLap = 0;
		m_iLapStartTimeMs = 0;
		m_iRaceTimeMs = -1;
		m_iIdxLastLapTimeMs = -1;
		m_iBestLapTimeMs = 599990;
		m_iNbCheckPointValidated = 0;
		m_bRaceEnded = false;
		m_fDistToEndOfLap = 0f;
		m_fDistToEndOfRace = 0f;
		m_iRaceNbLap = 0;
		m_iRaceNbCheckPoints = 0;
		m_GuidePosition = MultiPathPosition.UNDEFINED_MP_POS;
		m_pPrecedingVehicle = null;
		m_pPursuantVehicle = null;
		m_bDebugDraw = false;
		m_LapTimeMs = new int[16];
		m_CheckPointTimeMs = new int[16];
		m_pRace = null;
		for (int i = 0; i < 16; i++)
		{
			m_LapTimeMs[i] = 0;
		}
		for (int j = 0; j < 16; j++)
		{
			m_CheckPointTimeMs[j] = 0;
		}
	}

	public int GetRank()
	{
		return m_iRank;
	}

	public int GetBestLapTime()
	{
		return m_iBestLapTimeMs;
	}

	public void SetBestLapTime(int _iValue)
	{
		m_iBestLapTimeMs = _iValue;
	}

	public bool IsReverse()
	{
		return m_bReverse;
	}

	public int GetRaceTime()
	{
		return (m_iRaceTimeMs >= 0) ? m_iRaceTimeMs : 0;
	}

	public int GetCurrentLapTime()
	{
		return (m_iCurrentLapTime >= 0) ? m_iCurrentLapTime : 0;
	}

	public int GetCurrentDiffTime()
	{
		return m_iCurrentDiffTime;
	}

	public void SetCurrentDiffTime(int _iValue)
	{
		m_iCurrentDiffTime = _iValue;
	}

	public int GetLastLapTime()
	{
		return (m_iIdxLastLapTimeMs >= 0) ? m_LapTimeMs[m_iIdxLastLapTimeMs] : 0;
	}

	public int GetLapTime(int _Idx)
	{
		return (_Idx <= m_iIdxLastLapTimeMs && m_iIdxLastLapTimeMs >= 0) ? m_LapTimeMs[_Idx] : 0;
	}

	public int GetCheckPointTime(int _Idx)
	{
		return (_Idx <= m_iNbCheckPointValidated && _Idx >= 0) ? m_CheckPointTimeMs[_Idx] : 0;
	}

	public int GetNbLapCompleted()
	{
		return m_iNbLapCompleted;
	}

	public int GetLogicNbLap()
	{
		return m_iLogicNbLap;
	}

	public float GetDistToEndOfLap()
	{
		return m_fDistToEndOfLap;
	}

	public float GetDistToEndOfRace()
	{
		return m_fDistToEndOfRace;
	}

	public bool IsRaceEnded()
	{
		return m_bRaceEnded;
	}

	public MultiPathPosition GetGuidePosition()
	{
		return m_GuidePosition;
	}

	public void SetRank(int _Pos)
	{
		m_iRank = _Pos;
	}

	public void SetReverse(bool _bIsReverse)
	{
		m_bReverse = _bIsReverse;
	}

	public void SetDistToEndOfLap(float _dist)
	{
		m_fDistToEndOfLap = _dist;
	}

	public void SetDistToEndOfRace(float _dist)
	{
		m_fDistToEndOfRace = _dist;
	}

	public void SetPreceding(RcVehicle pPrecedingPlayer)
	{
		m_pPrecedingVehicle = pPrecedingPlayer;
	}

	public void SetPursuant(RcVehicle pPursuantPlayer)
	{
		m_pPursuantVehicle = pPursuantPlayer;
	}

	public RcVehicle GetVehicle()
	{
		return m_pVehicle;
	}

	public RcVehicle GetPreceding()
	{
		return m_pPrecedingVehicle;
	}

	public RcVehicle GetPursuant()
	{
		return m_pPursuantVehicle;
	}

	public int GetNbCheckPointValidated()
	{
		return m_iNbCheckPointValidated;
	}

	public void SetRaceNbLap(int _NbLaps)
	{
		m_iRaceNbLap = _NbLaps;
	}

	public void SetRaceNbCheckPoints(int _NbCheckpoints)
	{
		m_iRaceNbCheckPoints = _NbCheckpoints;
		m_iNbCheckPointValidated = m_iRaceNbCheckPoints;
	}

	public int GetRaceNbLap()
	{
		return m_iRaceNbLap;
	}

	public void SetDebugDraw(bool bDebugDraw)
	{
		m_bDebugDraw = bDebugDraw;
	}

	public void Awake()
	{
		m_pVehicle = base.transform.parent.GetComponentInChildren<RcVehicle>();
		if (m_pVehicle != null)
		{
			m_pVehicle.RaceStats = this;
		}
	}

	public void Start()
	{
		Reset();
		m_pRace = UnityEngine.Object.FindObjectOfType(typeof(RcRace)) as RcRace;
		if (m_pRace != null)
		{
			m_pRace.AddVehicle(this);
		}
		m_bDebugDraw = m_pVehicle.GetControlType() == RcVehicle.ControlType.Human;
	}

	public void Update()
	{
		MultiPathPosition guidePosition = GetGuidePosition();
		if (guidePosition.pathPosition.index == -1)
		{
			return;
		}
		RcFastPath simplePath = guidePosition.section.GetSimplePath();
		int pInd = (guidePosition.pathPosition.index + 1) % simplePath.GetNbPoints();
		Vector3 lhs = simplePath.GetPositionPoint(pInd) - simplePath.GetPositionPoint(guidePosition.pathPosition.index);
		lhs.Normalize();
		Vector3 rhs = base.transform.rotation * Vector3.forward;
		float value = Vector3.Dot(lhs, rhs);
		value = Mathf.Clamp(value, -1f, 1f);
		if (value < 0f)
		{
			float num = Mathf.Acos(value);
			if (num > (float)Math.PI / 2f)
			{
				SetReverse(true);
			}
			else
			{
				SetReverse(false);
			}
		}
		else
		{
			SetReverse(false);
		}
	}

	public void Reset()
	{
		m_GuidePosition = MultiPathPosition.UNDEFINED_MP_POS;
		m_iRank = 0;
		m_bReverse = false;
		m_iNbLapCompleted = 0;
		m_iLogicNbLap = 0;
		m_iLapStartTimeMs = 0;
		m_iRaceTimeMs = -1;
		m_iIdxLastLapTimeMs = -1;
		m_iBestLapTimeMs = int.MaxValue;
		m_iCurrentDiffTime = 0;
		m_iNbCheckPointValidated = m_iRaceNbCheckPoints;
		m_bRaceEnded = false;
		for (int i = 0; i < 16; i++)
		{
			m_LapTimeMs[i] = 0;
		}
		for (int j = 0; j < 16; j++)
		{
			m_CheckPointTimeMs[j] = 0;
		}
	}

	public void RefreshTime(int _gameTime)
	{
		if (!m_bRaceEnded)
		{
			if (m_iRaceTimeMs >= 0)
			{
				m_iRaceTimeMs = _gameTime;
			}
			m_iCurrentLapTime = m_iRaceTimeMs - m_iLapStartTimeMs;
		}
	}

	public void CrossEndLine(int _gameTime)
	{
		if (m_bRaceEnded)
		{
			return;
		}
		bool flag = false;
		if (m_iNbCheckPointValidated == m_iRaceNbCheckPoints)
		{
			m_iLogicNbLap++;
			m_CheckPointTimeMs[0] = m_iCurrentLapTime;
			m_iNbCheckPointValidated = 0;
		}
		if (m_iNbLapCompleted != 0)
		{
			return;
		}
		if (m_iNbLapCompleted <= m_iRaceNbLap && m_iNbLapCompleted < m_iLogicNbLap)
		{
			if (m_iNbLapCompleted < m_iRaceNbLap)
			{
				m_iNbLapCompleted++;
			}
			flag = true;
		}
		if (!CanCompleteLap)
		{
			m_iNbLapCompleted = 0;
			m_iLogicNbLap = 0;
		}
		int num = 0;
		bool flag2 = false;
		if (!m_bRaceEnded)
		{
			m_bRaceEnded = true;
			flag2 = true;
		}
		if (m_iRaceTimeMs >= 0 && m_iLogicNbLap > 1 && flag)
		{
			m_LapTimeMs[++m_iIdxLastLapTimeMs] = m_iCurrentLapTime;
			num = m_iCurrentLapTime;
			if (!m_bRaceEnded)
			{
				m_iCurrentLapTime = 0;
			}
			if (num != 0 && num < m_iBestLapTimeMs)
			{
				m_iBestLapTimeMs = num;
			}
			m_iLapStartTimeMs = _gameTime;
			if (m_pVehicle.OnLapEnded != null)
			{
				m_pVehicle.OnLapEnded();
			}
		}
		if (flag2)
		{
			if (m_pVehicle.OnRaceEnded != null)
			{
				m_pVehicle.OnRaceEnded(m_pVehicle);
			}
			m_pVehicle.SetRaceEnded(true);
		}
	}

	public void CrossStartLine(int _gameTime, bool _bReverse)
	{
		if (m_bRaceEnded)
		{
			if (m_pVehicle.OnLapEndedAfterRace != null)
			{
				m_pVehicle.OnLapEndedAfterRace();
			}
			return;
		}
		bool flag = false;
		if (_bReverse)
		{
			m_iLogicNbLap--;
			m_iNbCheckPointValidated = m_iRaceNbCheckPoints;
		}
		else if (m_iNbCheckPointValidated == m_iRaceNbCheckPoints)
		{
			m_iLogicNbLap++;
			m_CheckPointTimeMs[0] = m_iCurrentLapTime;
			m_iNbCheckPointValidated = 0;
		}
		if (m_pVehicle.OnCheckpointsReseted != null)
		{
			m_pVehicle.OnCheckpointsReseted();
		}
		if (m_iNbLapCompleted <= m_iRaceNbLap && m_iNbLapCompleted < m_iLogicNbLap)
		{
			if (m_iNbLapCompleted < m_iRaceNbLap)
			{
				m_iNbLapCompleted++;
			}
			flag = true;
		}
		if (!CanCompleteLap)
		{
			m_iNbLapCompleted = 0;
			m_iLogicNbLap = 0;
		}
		int num = 0;
		bool flag2 = false;
		if (m_iLogicNbLap >= m_iRaceNbLap + 1 && !m_bRaceEnded)
		{
			RefreshTime(_gameTime);
			m_bRaceEnded = true;
			flag2 = true;
		}
		if (m_iRaceTimeMs >= 0 && flag)
		{
			if (m_iLogicNbLap > 1)
			{
				m_LapTimeMs[++m_iIdxLastLapTimeMs] = m_iCurrentLapTime;
				num = m_iCurrentLapTime;
				if (!m_bRaceEnded)
				{
					m_iCurrentLapTime = 0;
				}
				if (num != 0 && num < m_iBestLapTimeMs)
				{
					m_iBestLapTimeMs = num;
				}
				m_iLapStartTimeMs = _gameTime;
				if (m_pVehicle.OnLapEnded != null)
				{
					m_pVehicle.OnLapEnded();
				}
				if (m_pVehicle.OnLapEnded != null)
				{
					m_pVehicle.OnLapEnded();
				}
			}
			else if (m_iLogicNbLap == 0 && m_pVehicle.OnFirstLapStarted != null)
			{
				m_pVehicle.OnFirstLapStarted();
			}
		}
		if (flag2)
		{
			if (m_pVehicle.OnRaceEnded != null)
			{
				m_pVehicle.OnRaceEnded(m_pVehicle);
			}
			m_pVehicle.SetRaceEnded(true);
		}
	}

	public void StartChrono()
	{
		if (m_iRaceTimeMs < 0)
		{
			m_iRaceTimeMs = 0;
			m_iLapStartTimeMs = 0;
		}
		m_iCurrentLapTime = 0;
		if (m_pVehicle.OnRaceStated != null)
		{
			m_pVehicle.OnRaceStated();
		}
	}

	public void CrossCheckPoint(int _Cross)
	{
		if (_Cross == m_iNbCheckPointValidated)
		{
			m_CheckPointTimeMs[m_iNbCheckPointValidated] = m_iCurrentLapTime;
			m_iNbCheckPointValidated++;
			RcEventCheckPoint rcEventCheckPoint = default(RcEventCheckPoint);
			rcEventCheckPoint.m_pVehicle = m_pVehicle;
			rcEventCheckPoint.m_iCheckpointIdx = _Cross;
			rcEventCheckPoint.m_iCheckpointTime = m_iCurrentLapTime;
			if (m_pVehicle.OnCheckpointValidated != null)
			{
				m_pVehicle.OnCheckpointValidated();
			}
		}
	}

	public void Respawn()
	{
		m_GuidePosition = MultiPathPosition.UNDEFINED_MP_POS;
	}

	public int ForceEndRace(int _OverallWorstLap, int _WorstRace)
	{
		m_bRaceEnded = true;
		int num = 0;
		int num2 = 0;
		m_iRaceTimeMs = 0;
		int num3 = 10;
		int num4 = num3 + GetRank();
		int num5 = _OverallWorstLap * num4 / num3;
		for (num = 0; num < m_iNbLapCompleted; num++)
		{
			if (m_LapTimeMs[num] > num2)
			{
				num2 = m_LapTimeMs[num];
			}
		}
		float length = m_GuidePosition.section.GetLength();
		float num6 = length - m_GuidePosition.section.GetDistToEndLine();
		int num7 = (int)((float)m_iCurrentLapTime * (length / num6));
		if (num2 < num7)
		{
			num2 = num7;
		}
		if (num2 == 0)
		{
			num2 = num5;
		}
		for (num = 0; num < m_iRaceNbLap; num++)
		{
			if (m_LapTimeMs[num] == 0)
			{
				m_LapTimeMs[num] = num2;
			}
			m_iRaceTimeMs += m_LapTimeMs[num];
			if (num == m_iRaceNbLap - 1 && m_iRaceTimeMs <= _WorstRace)
			{
				int num8 = _WorstRace - m_iRaceTimeMs;
				num8 += 500 + (int)(UnityEngine.Random.value * 1000f);
				m_LapTimeMs[num] += num8;
				m_iRaceTimeMs += num8;
			}
		}
		return m_iRaceTimeMs;
	}

	public int GetWorstLap()
	{
		int num = 0;
		for (int i = 0; i < m_iRaceNbLap; i++)
		{
			if (m_LapTimeMs[i] > num)
			{
				num = m_LapTimeMs[i];
			}
		}
		return num;
	}

	public void Stop()
	{
		if (m_pRace != null)
		{
			m_pRace.RemoveVehicle(this);
		}
	}

	public void ForceRefreshRespawn()
	{
		if (m_pRace != null)
		{
			m_pRace.ForceRefreshRespawn(this);
		}
	}
}
