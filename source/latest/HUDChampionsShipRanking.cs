public class HUDChampionsShipRanking : HUDEndRace
{
	public override void FillPositions()
	{
		base.FillPositions();
		if (Singleton<GameConfigurator>.Instance.ChampionShipData != null)
		{
			string championShipName = Singleton<GameConfigurator>.Instance.ChampionShipData.ChampionShipName;
			LabelTitle.text = string.Format(Localization.instance.Get("HUD_DYN_CHAMPIONSHIP_GENERAL"), championShipName);
		}
		for (int i = 0; i < PointsToAdd.Count; i++)
		{
			LabelPoint[i].text = PointsToAdd[i] + " pts";
		}
		foreach (UISprite item in m_Advantage)
		{
			item.gameObject.SetActive(false);
		}
	}

	public override void GetScoreInfos(int iIndex, out int iKartIndex, out int iScore, out int iTotalScore, out bool bEquality)
	{
		ChampionShipScoreData championshipPos = Singleton<GameConfigurator>.Instance.RankingManager.GetChampionshipPos(iIndex);
		if (LogManager.Instance == null || championshipPos.KartIndex == Singleton<GameManager>.Instance.GameMode.GetHumanPlayerVehicleId())
		{
		}
		if (iIndex > 0)
		{
			ChampionShipScoreData championshipPos2 = Singleton<GameConfigurator>.Instance.RankingManager.GetChampionshipPos(iIndex - 1);
			bEquality = championshipPos.ChampionshipScore == championshipPos2.ChampionshipScore;
		}
		else
		{
			bEquality = false;
		}
		iKartIndex = championshipPos.KartIndex;
		iScore = championshipPos.ChampionshipScore;
		iTotalScore = 0;
	}

	public override int GetTrackIndex()
	{
		return Singleton<GameConfigurator>.Instance.CurrentTrackIndex - 1;
	}

	public override void Update()
	{
	}
}
