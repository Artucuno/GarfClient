public class HUDEndChampionsShipRace : HUDEndRace
{
	public override void FillPositions()
	{
		base.FillPositions();
		if (Singleton<GameConfigurator>.Instance.ChampionShipData != null)
		{
			string championShipName = Singleton<GameConfigurator>.Instance.ChampionShipData.ChampionShipName;
			LabelTitle.text = string.Format(Localization.instance.Get("HUD_DYN_CHAMPIONSHIP_ROUND"), championShipName, Singleton<GameConfigurator>.Instance.CurrentTrackIndex);
		}
	}

	public override void GetScoreInfos(int iIndex, out int iKartIndex, out int iScore, out int iTotalScore, out bool bEquality)
	{
		ChampionShipScoreData championShipScoreData = (ChampionShipScoreData)Singleton<GameConfigurator>.Instance.RankingManager.GetRacePos(iIndex);
		if (iIndex > 0)
		{
			bEquality = championShipScoreData.RaceScore == Singleton<GameConfigurator>.Instance.RankingManager.GetRacePos(iIndex - 1).RaceScore;
		}
		else
		{
			bEquality = false;
		}
		iKartIndex = championShipScoreData.KartIndex;
		iScore = championShipScoreData.RaceScore;
		iTotalScore = championShipScoreData.PreviousChampionshipScore;
	}

	public override int GetTrackIndex()
	{
		return Singleton<GameConfigurator>.Instance.CurrentTrackIndex - 1;
	}
}
