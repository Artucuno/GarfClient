using System.Collections.Generic;

internal class RaceScoreDataComparer : IComparer<int>
{
	private RaceScoreData[] m_pData;

	private bool m_bScore;

	public RaceScoreDataComparer(RaceScoreData[] pData, bool bScore)
	{
		m_pData = pData;
		m_bScore = bScore;
	}

	public int Compare(int a, int b)
	{
		RaceScoreData raceScoreData = m_pData[a];
		RaceScoreData raceScoreData2 = m_pData[b];
		if (raceScoreData.KartIndex == raceScoreData2.KartIndex)
		{
			return 0;
		}
		if (m_bScore)
		{
			if (raceScoreData.RaceScore != raceScoreData2.RaceScore)
			{
				return raceScoreData2.RaceScore.CompareTo(raceScoreData.RaceScore);
			}
			if (raceScoreData2.IsAI == raceScoreData.IsAI)
			{
				return raceScoreData.PreviousRaceScore.CompareTo(raceScoreData2.PreviousRaceScore);
			}
			if (raceScoreData2.IsAI)
			{
				return raceScoreData2.RaceScore.CompareTo(raceScoreData.RaceScore + 1);
			}
			return raceScoreData2.RaceScore.CompareTo(raceScoreData.RaceScore - 1);
		}
		if (raceScoreData.RacePosition != raceScoreData2.RacePosition)
		{
			return raceScoreData.RacePosition.CompareTo(raceScoreData2.RacePosition);
		}
		if (raceScoreData2.IsAI)
		{
			return raceScoreData.RacePosition.CompareTo(raceScoreData2.RacePosition + 1);
		}
		return raceScoreData.RacePosition.CompareTo(raceScoreData2.RacePosition - 1);
	}
}
