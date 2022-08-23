using System;
using System.Collections.Generic;

public class RankingManager
{
	private List<RaceScoreData> m_vRaceScores = new List<RaceScoreData>();

	private List<int> m_vChampionshipSorted = new List<int>();

	private List<int> m_vRaceSorted = new List<int>();

	public void Reset()
	{
		m_vRaceScores.Clear();
		m_vChampionshipSorted.Clear();
		m_vRaceSorted.Clear();
	}

	public void InitPlayer(int iKartIndex, bool bIsAi)
	{
		foreach (RaceScoreData vRaceScore in m_vRaceScores)
		{
			if (vRaceScore.KartIndex == iKartIndex)
			{
				return;
			}
		}
		RaceScoreData raceScoreData = null;
		if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.CHAMPIONSHIP)
		{
			raceScoreData = new ChampionShipScoreData(iKartIndex, bIsAi);
			m_vChampionshipSorted.Add(m_vRaceScores.Count);
		}
		else
		{
			raceScoreData = new RaceScoreData(iKartIndex, bIsAi);
		}
		m_vRaceSorted.Add(m_vRaceScores.Count);
		m_vRaceScores.Add(raceScoreData);
	}

	public void PlayerFinish(int iKartIndex, int iScore)
	{
		foreach (RaceScoreData vRaceScore in m_vRaceScores)
		{
			if (vRaceScore.KartIndex == iKartIndex)
			{
				vRaceScore.SetRaceScore(iScore);
				break;
			}
		}
	}

	public void RestartRace()
	{
		foreach (RaceScoreData vRaceScore in m_vRaceScores)
		{
			vRaceScore.RestartRace();
		}
	}

	public void ResetRace()
	{
		foreach (RaceScoreData vRaceScore in m_vRaceScores)
		{
			vRaceScore.ResetRace();
		}
	}

	public void ComputePositions(bool bChampionship, bool bScore)
	{
		int[] array = m_vRaceSorted.ToArray();
		Array.Sort(array, new RaceScoreDataComparer(m_vRaceScores.ToArray(), bScore));
		for (int i = 0; i < array.Length; i++)
		{
			m_vRaceSorted[i] = array[i];
			m_vRaceScores[m_vRaceSorted[i]].SetRacePosition(i);
		}
		if (bChampionship)
		{
			int[] array2 = m_vChampionshipSorted.ToArray();
			Array.Sort(array2, new ChampionShipScoreDataComparer(m_vRaceScores.ToArray()));
			for (int j = 0; j < array2.Length; j++)
			{
				m_vChampionshipSorted[j] = array2[j];
				ChampionShipScoreData championShipScoreData = (ChampionShipScoreData)m_vRaceScores[m_vChampionshipSorted[j]];
				championShipScoreData.SetChampionShipPosition(j);
			}
		}
	}

	public RaceScoreData GetRacePos(int iPos)
	{
		if (iPos >= 0 && iPos < m_vRaceSorted.Count)
		{
			return m_vRaceScores[m_vRaceSorted[iPos]];
		}
		return null;
	}

	public ChampionShipScoreData GetChampionshipPos(int iPos)
	{
		if (iPos >= 0 && iPos < m_vChampionshipSorted.Count)
		{
			return (ChampionShipScoreData)m_vRaceScores[m_vChampionshipSorted[iPos]];
		}
		return null;
	}

	public RaceScoreData GetScoreData(int iKartIndex)
	{
		foreach (RaceScoreData vRaceScore in m_vRaceScores)
		{
			if (vRaceScore.KartIndex == iKartIndex)
			{
				return vRaceScore;
			}
		}
		return null;
	}

	public void SetInitialRank(int iKartIndex, int iRank)
	{
		RaceScoreData scoreData = GetScoreData(iKartIndex);
		if (scoreData != null && scoreData.RacePosition == 0 && scoreData.PreviousRacePosition == 0)
		{
			scoreData.SetRacePosition(iRank);
		}
	}

	public int RaceScoreCount()
	{
		return m_vRaceScores.Count;
	}

	public List<RaceScoreData> GetRaceScoreDataList()
	{
		return m_vRaceScores;
	}
}
