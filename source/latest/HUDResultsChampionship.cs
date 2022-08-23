using UnityEngine;

public class HUDResultsChampionship : MonoBehaviour
{
	public UILabel ChampionshipName;

	public UILabel Summary;

	public UITexturePattern Cup;

	public GameObject Halo;

	public UITexturePattern DifficultyIcon;

	public UITexturePattern ChampionshipIcon;

	public UITexturePattern Position;

	public GameObject FacebookButton;

	private EntryPoint m_cEntryPoint;

	public void Start()
	{
		int humanPlayerVehicleId = Singleton<GameManager>.Instance.GameMode.GetHumanPlayerVehicleId();
		int num = 0;
		for (int i = 0; i < Singleton<GameConfigurator>.Instance.RankingManager.RaceScoreCount(); i++)
		{
			ChampionShipScoreData championshipPos = Singleton<GameConfigurator>.Instance.RankingManager.GetChampionshipPos(i);
			if (humanPlayerVehicleId == championshipPos.KartIndex)
			{
				num = championshipPos.ChampionshipPosition;
				break;
			}
		}
		if (Singleton<GameConfigurator>.Instance.ChampionShipData != null)
		{
			ChampionshipName.text = Singleton<GameConfigurator>.Instance.ChampionShipData.ChampionShipName;
		}
		ChampionshipIcon.ChangeTexture(Singleton<GameConfigurator>.Instance.ChampionShipData.Index);
		DifficultyIcon.ChangeTexture((int)Singleton<GameConfigurator>.Instance.Difficulty);
		Position.ChangeTexture(num);
		if (num < 3)
		{
			Cup.ChangeTexture(Singleton<GameConfigurator>.Instance.ChampionShipData.Index * 3 + num);
			Halo.SetActive(true);
		}
		else
		{
			Cup.gameObject.SetActive(false);
			Halo.SetActive(false);
		}
		Summary.text = Localization.instance.Get((num >= 3) ? "HUD_RESULTS_FAILED" : ((num != 0) ? "HUD_RESULTS_CONGRATS" : "HUD_FINISHRACE_FINISHFIRST"));
		m_cEntryPoint = GameObject.Find("EntryPoint").GetComponent<EntryPoint>();
		FacebookButton.SetActive(false);
	}

	public void OnFacebook()
	{
		if ((bool)m_cEntryPoint)
		{
			RaceScoreData scoreData = Singleton<GameConfigurator>.Instance.RankingManager.GetScoreData(Singleton<GameManager>.Instance.GameMode.GetHumanPlayerVehicleId());
			int num = ((ChampionShipScoreData)scoreData).ChampionshipPosition + 1;
			int num2 = num;
			string empty = string.Empty;
			if (num2 > 3)
			{
				num2 = 4;
			}
			empty = string.Format(Localization.instance.Get("FB_CHAMPIONSHIP_TITLE_" + num2), Singleton<GameConfigurator>.Instance.ChampionShipData.ChampionShipName);
			string sDescription = string.Format(Localization.instance.Get("FB_CHAMPIONSHIP_DESCRIPTION"), Localization.instance.Get("FB_PLAYER_PLACE_" + num));
			m_cEntryPoint.OnFacebook(empty, sDescription);
		}
	}
}
