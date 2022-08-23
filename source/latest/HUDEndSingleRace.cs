using UnityEngine;

public class HUDEndSingleRace : HUDEndRace
{
	public GameObject FacebookButton;

	private EntryPoint m_cEntryPoint;

	public override void Init()
	{
		base.Init();
		m_cEntryPoint = GameObject.Find("EntryPoint").GetComponent<EntryPoint>();
		FacebookButton.SetActive(false);
	}

	public override void FillPositions()
	{
		base.FillPositions();
		if (Singleton<GameConfigurator>.Instance.ChampionShipData != null)
		{
			string arg = Singleton<GameConfigurator>.Instance.ChampionShipData.TracksName[Singleton<GameConfigurator>.Instance.CurrentTrackIndex];
			LabelTitle.text = string.Format(Localization.instance.Get("HUD_DYN_SINGLE_RACE_RESULTS"), arg);
		}
	}

	public override void GetScoreInfos(int iIndex, out int iKartIndex, out int iScore, out int iTotalScore, out bool bEquality)
	{
		RaceScoreData racePos = Singleton<GameConfigurator>.Instance.RankingManager.GetRacePos(iIndex);
		if (iIndex > 0)
		{
			bEquality = racePos.RaceScore == Singleton<GameConfigurator>.Instance.RankingManager.GetRacePos(iIndex - 1).RaceScore;
		}
		else
		{
			bEquality = false;
		}
		iKartIndex = racePos.KartIndex;
		iScore = racePos.RaceScore;
		iTotalScore = 0;
	}

	public void OnFacebook()
	{
		if ((bool)m_cEntryPoint)
		{
			RaceScoreData scoreData = Singleton<GameConfigurator>.Instance.RankingManager.GetScoreData(Singleton<GameManager>.Instance.GameMode.GetHumanPlayerVehicleId());
			int num = scoreData.RacePosition + 1;
			string empty = string.Empty;
			if (num > 3)
			{
				num = 4;
			}
			empty = string.Format(Localization.instance.Get("FB_SINGLERACE_TITLE_" + num), Singleton<GameConfigurator>.Instance.ChampionShipData.TracksName[Singleton<GameConfigurator>.Instance.CurrentTrackIndex]);
			string sDescription = string.Format(Localization.instance.Get("FB_SINGLERACE_DESCRIPTION"), Localization.instance.Get("FB_PLAYER_PLACE_" + (scoreData.RacePosition + 1)));
			m_cEntryPoint.OnFacebook(empty, sDescription);
		}
	}
}
