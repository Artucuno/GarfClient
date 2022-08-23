using System.Collections.Generic;

internal class ChampionShipScoreDataComparer : IComparer<int>
{
	private RaceScoreData[] m_pData;

	public ChampionShipScoreDataComparer(RaceScoreData[] pData)
	{
		m_pData = pData;
	}

	public int Compare(int a, int b)
	{
		ChampionShipScoreData championShipScoreData = (ChampionShipScoreData)m_pData[a];
		ChampionShipScoreData championShipScoreData2 = (ChampionShipScoreData)m_pData[b];
		if (championShipScoreData.KartIndex == championShipScoreData2.KartIndex)
		{
			return 0;
		}
		if (championShipScoreData.ChampionshipScore != championShipScoreData2.ChampionshipScore)
		{
			return championShipScoreData2.ChampionshipScore.CompareTo(championShipScoreData.ChampionshipScore);
		}
		if (championShipScoreData2.IsAI == championShipScoreData.IsAI)
		{
			return championShipScoreData.PreviousChampionshipScore.CompareTo(championShipScoreData2.PreviousChampionshipScore);
		}
		if (championShipScoreData2.IsAI)
		{
			return championShipScoreData2.ChampionshipScore.CompareTo(championShipScoreData.ChampionshipScore + 1);
		}
		return championShipScoreData2.ChampionshipScore.CompareTo(championShipScoreData.ChampionshipScore - 1);
	}
}
