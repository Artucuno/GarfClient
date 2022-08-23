using System.Collections.Generic;
using UnityEngine;

public class RcRace : MonoBehaviour
{
	public class VehicleComparer : IComparer<RcVehicleRaceStats>
	{
		public int Compare(RcVehicleRaceStats pa, RcVehicleRaceStats pb)
		{
			if (pa.IsRaceEnded() != pb.IsRaceEnded())
			{
				return (!pa.IsRaceEnded()) ? 1 : (-1);
			}
			if (pa.IsRaceEnded() && pb.IsRaceEnded())
			{
				return pa.GetRaceTime() - pb.GetRaceTime();
			}
			return (!(pa.GetDistToEndOfRace() < pb.GetDistToEndOfRace())) ? 1 : (-1);
		}
	}

	protected int m_iRaceTime;

	protected List<RcVehicleRaceStats> m_pVehicleRaceStats;

	protected bool m_bEndOfRace;

	public int m_iRaceNbLap = 3;

	private int m_iRaceNbCheckPoints;

	protected int m_iNbPlayers;

	protected RcMultiPath m_pMultiPath;

	protected RcCatchUp m_pCatchUp;

	private bool m_bRaceStarted;

	public RcRace()
	{
		m_iRaceTime = 0;
		m_bEndOfRace = false;
		m_iRaceNbCheckPoints = 0;
		m_iNbPlayers = 0;
		m_pMultiPath = null;
		m_pCatchUp = null;
		m_pVehicleRaceStats = new List<RcVehicleRaceStats>();
	}

	public int GetRaceNbLap()
	{
		return m_iRaceNbLap;
	}

	public int GetRaceNbCheckPoints()
	{
		return m_iRaceNbCheckPoints;
	}

	public int GetRaceNbPlayers()
	{
		return m_iNbPlayers;
	}

	public void ResetRaceTime()
	{
		m_iRaceTime = 0;
	}

	public int GetRaceTime()
	{
		return m_iRaceTime;
	}

	public void Awake()
	{
		AwakeNet();
		m_iRaceNbCheckPoints = 0;
		RcPortalTrigger[] array = (RcPortalTrigger[])Object.FindObjectsOfType(typeof(RcPortalTrigger));
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].m_eActionType == RcPortalTrigger.PortalAction.CheckPoint)
			{
				m_iRaceNbCheckPoints++;
			}
		}
		m_pMultiPath = (RcMultiPath)Object.FindObjectOfType(typeof(RcMultiPath));
		m_bRaceStarted = false;
	}

	public void AddVehicle(RcVehicleRaceStats pStats)
	{
		pStats.SetRaceNbLap(m_iRaceNbLap);
		pStats.SetRaceNbCheckPoints(m_iRaceNbCheckPoints);
		m_pVehicleRaceStats.Add(pStats);
		m_iNbPlayers++;
		if (m_iRaceTime > 0)
		{
			pStats.GetVehicle().SetLocked(false);
			pStats.StartChrono();
			pStats.RefreshTime(m_iRaceTime);
		}
	}

	public void RemoveVehicle(RcVehicleRaceStats pStats)
	{
		m_pVehicleRaceStats.Remove(pStats);
		m_iNbPlayers--;
	}

	public void Start()
	{
		Reset();
		m_pCatchUp = GetComponent<RcCatchUp>();
	}

	public RcVehicleRaceStats GetRankedVehicle(int _Rank)
	{
		for (int i = 0; i < m_iNbPlayers; i++)
		{
			if ((bool)m_pVehicleRaceStats[i] && m_pVehicleRaceStats[i].GetRank() == _Rank)
			{
				return m_pVehicleRaceStats[i];
			}
		}
		return null;
	}

	public void FixedUpdate()
	{
		if (m_bRaceStarted)
		{
			m_iRaceTime += (int)(Time.fixedDeltaTime * 1000f);
		}
	}

	public void Update()
	{
		if (m_bRaceStarted)
		{
			for (int i = 0; i < m_iNbPlayers; i++)
			{
				if ((bool)m_pVehicleRaceStats[i])
				{
					m_pVehicleRaceStats[i].RefreshTime(m_iRaceTime);
				}
			}
		}
		if ((bool)m_pMultiPath)
		{
			float refDistToEndOfRace = 0f;
			if ((bool)Singleton<GameManager>.Instance.GameMode)
			{
				if (Network.peerType == NetworkPeerType.Disconnected)
				{
					GameObject humanPlayer = Singleton<GameManager>.Instance.GameMode.GetHumanPlayer();
					if ((bool)humanPlayer)
					{
						RcVehicleRaceStats componentInChildren = humanPlayer.GetComponentInChildren<RcVehicleRaceStats>();
						if ((bool)componentInChildren)
						{
							refDistToEndOfRace = componentInChildren.GetDistToEndOfRace();
						}
					}
				}
				else if (m_pVehicleRaceStats.Count != 0)
				{
					List<RaceScoreData> raceScoreDataList = Singleton<GameConfigurator>.Instance.RankingManager.GetRaceScoreDataList();
					float num = 1E+12f;
					for (int j = 0; j < raceScoreDataList.Count; j++)
					{
						RaceScoreData raceScoreData = raceScoreDataList[j];
						if (raceScoreData != null && !raceScoreData.IsAI && raceScoreData.KartIndex < m_pVehicleRaceStats.Count)
						{
							num = Mathf.Min(num, m_pVehicleRaceStats[raceScoreData.KartIndex].GetDistToEndOfRace());
						}
					}
					refDistToEndOfRace = num;
				}
			}
			for (int k = 0; k < m_iNbPlayers; k++)
			{
				if ((bool)m_pVehicleRaceStats[k])
				{
					m_pMultiPath.RefreshRespawn(m_pVehicleRaceStats[k]);
					if ((bool)m_pCatchUp)
					{
						float distToEndOfRace = m_pVehicleRaceStats[k].GetDistToEndOfRace();
						m_pCatchUp.ComputeCatchUp(m_pVehicleRaceStats[k].GetVehicle(), distToEndOfRace, refDistToEndOfRace);
					}
				}
			}
		}
		ComputeRacePositions();
	}

	public void ComputeRacePositions()
	{
		List<RcVehicleRaceStats> list = new List<RcVehicleRaceStats>();
		for (int i = 0; i < m_iNbPlayers; i++)
		{
			if ((bool)m_pVehicleRaceStats[i])
			{
				list.Add(m_pVehicleRaceStats[i]);
			}
		}
		VehicleComparer comparer = new VehicleComparer();
		list.Sort(comparer);
		for (int j = 0; j < list.Count; j++)
		{
			list[j].SetRank(j);
			if (j == 0)
			{
				list[j].SetPreceding(list[list.Count - 1].GetVehicle());
			}
			else
			{
				list[j].SetPreceding(list[j - 1].GetVehicle());
			}
			if (j == list.Count - 1)
			{
				list[j].SetPursuant(list[0].GetVehicle());
			}
			else
			{
				list[j].SetPursuant(list[j + 1].GetVehicle());
			}
		}
	}

	public bool Reset()
	{
		m_bEndOfRace = false;
		return true;
	}

	public void StartRace()
	{
		for (int i = 0; i < m_iNbPlayers; i++)
		{
			if ((bool)m_pVehicleRaceStats[i])
			{
				m_pVehicleRaceStats[i].StartChrono();
			}
		}
		m_bRaceStarted = true;
	}

	public void End()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		RcVehicleRaceStats[] array = new RcVehicleRaceStats[m_iNbPlayers];
		for (int i = 0; i < m_iNbPlayers; i++)
		{
			array[i] = null;
		}
		for (int j = 0; j < m_iNbPlayers; j++)
		{
			if (!m_pVehicleRaceStats[j])
			{
				continue;
			}
			num2 = m_pVehicleRaceStats[j].GetWorstLap();
			if (num2 > num)
			{
				num = num2;
			}
			if (m_pVehicleRaceStats[j].IsRaceEnded())
			{
				num4 = m_pVehicleRaceStats[j].GetRaceTime();
				if (num4 > num3)
				{
					num3 = num4;
				}
			}
			array[m_pVehicleRaceStats[j].GetRank()] = m_pVehicleRaceStats[j];
		}
		for (int k = 0; k < m_iNbPlayers; k++)
		{
			if ((bool)array[k] && !array[k].IsRaceEnded())
			{
				num3 = array[k].ForceEndRace(num, num3);
			}
		}
		m_bEndOfRace = true;
	}

	public void CrossStartLine(RcVehicleRaceStats pStats, bool _bReverse)
	{
		pStats.CrossStartLine(m_iRaceTime, _bReverse);
		if (_bReverse)
		{
			return;
		}
		if (pStats.GetLogicNbLap() >= m_iRaceNbLap + 1)
		{
			ComputeRacePositions();
			if (pStats.GetRank() == m_iNbPlayers)
			{
				m_bEndOfRace = true;
			}
		}
		ComputeRacePositions();
	}

	public void CrossEndLine(RcVehicleRaceStats pStats)
	{
		pStats.CrossEndLine(m_iRaceTime);
		if (pStats.GetRank() == m_iNbPlayers)
		{
			m_bEndOfRace = true;
		}
		ComputeRacePositions();
	}

	public void CrossCheckPoint(RcVehicleRaceStats pStats, int _Id)
	{
		pStats.CrossCheckPoint(_Id);
	}

	public void ForceRefreshRespawn(RcVehicleRaceStats pStats)
	{
		m_pMultiPath.ResetPosition(ref pStats.m_GuidePosition, pStats.GetVehicle().GetPosition());
	}

	private void AwakeNet()
	{
		if (base.networkView == null)
		{
		}
		base.networkView.stateSynchronization = NetworkStateSynchronization.Unreliable;
		base.networkView.observed = base.transform;
	}

	private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if (!Network.isClient && stream.isWriting)
		{
			stream.Serialize(ref m_iRaceTime);
		}
		else if (Network.isClient && !stream.isWriting)
		{
			int value = 0;
			stream.Serialize(ref value);
			m_iRaceTime = value + (int)((Network.time - info.timestamp) * 1000.0);
		}
	}

	public RcVehicleRaceStats GetVehicleStats(NetworkViewID id)
	{
		for (int i = 0; i < m_iNbPlayers; i++)
		{
			if (m_pVehicleRaceStats[i].transform.parent.gameObject.networkView.viewID == id)
			{
				return m_pVehicleRaceStats[i];
			}
		}
		return null;
	}

	public void OnNetworkLoadedLevel()
	{
		StartRace();
	}
}
