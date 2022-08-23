using UnityEngine;

public class HUDResultsTimeTrial : MonoBehaviour
{
	public GameObject[] Medals;

	public UILabel TrackName;

	public UILabel Summary;

	public UILabel MedalEarned;

	public UITexturePattern DifficultyIcon;

	public UITexturePattern ChampionshipIcon;

	public GameObject GUIMedalEarned;

	public GameObject FacebookButton;

	private EntryPoint m_cEntryPoint;

	private E_TimeTrialMedal m_ePreviousMedal;

	private bool m_bDeferedDisplay;

	private E_TimeTrialMedal CurrMedal;

	public void SetPreviousMedal(E_TimeTrialMedal Medal)
	{
		m_ePreviousMedal = Medal;
	}

	public void Start()
	{
		for (int i = 0; i < Medals.Length; i++)
		{
			Medals[i].SetActive(i <= (int)(m_ePreviousMedal - 1));
		}
		string startScene = Singleton<GameConfigurator>.Instance.StartScene;
		CurrMedal = Singleton<GameSaveManager>.Instance.GetMedal(startScene, false);
		if (m_ePreviousMedal < CurrMedal)
		{
			m_bDeferedDisplay = true;
			m_ePreviousMedal++;
			MedalEarned.text = string.Format(Localization.instance.Get("MENU_REWARDS_TIME_TRIAL_MEDAL"), Localization.instance.Get("MENU_REWARDS_MEDAL" + (int)CurrMedal));
			Summary.text = Localization.instance.Get("HUD_FINISHRACE_FINISHFIRST");
		}
		else
		{
			Singleton<GameManager>.Instance.GameMode.Hud.DoNext();
		}
		ChampionshipIcon.ChangeTexture(Singleton<GameConfigurator>.Instance.ChampionShipData.Index);
		DifficultyIcon.ChangeTexture((int)Singleton<GameConfigurator>.Instance.Difficulty);
		if (Singleton<GameConfigurator>.Instance.ChampionShipData != null)
		{
			TrackName.text = Singleton<GameConfigurator>.Instance.ChampionShipData.TracksName[Singleton<GameConfigurator>.Instance.CurrentTrackIndex];
		}
		m_cEntryPoint = GameObject.Find("EntryPoint").GetComponent<EntryPoint>();
		FacebookButton.SetActive(false);
	}

	public void Update()
	{
		if (m_bDeferedDisplay)
		{
			if (!Medals[(int)(m_ePreviousMedal - 1)].activeSelf)
			{
				Medals[(int)(m_ePreviousMedal - 1)].SetActive(true);
				Medals[(int)(m_ePreviousMedal - 1)].animation.Play();
			}
			if (m_ePreviousMedal >= CurrMedal)
			{
				m_bDeferedDisplay = false;
			}
			if (!Medals[(int)(m_ePreviousMedal - 1)].animation.isPlaying)
			{
				m_ePreviousMedal++;
			}
		}
	}

	public void OnFacebook()
	{
		if ((bool)m_cEntryPoint)
		{
			string empty = string.Empty;
			int currMedal = (int)CurrMedal;
			string startScene = Singleton<GameConfigurator>.Instance.StartScene;
			string rpTime = string.Empty;
			Singleton<GameSaveManager>.Instance.GetTimeTrialRecord(startScene, ref rpTime);
			empty = string.Format(Localization.instance.Get("FB_TIMETRIAL_TITLE_" + currMedal), Localization.instance.Get("MENU_REWARDS_MEDAL" + (int)CurrMedal), Singleton<GameConfigurator>.Instance.ChampionShipData.TracksName[Singleton<GameConfigurator>.Instance.CurrentTrackIndex]);
			string sDescription = string.Format(Localization.instance.Get("FB_TIMETRIAL_DESCRIPTION"), rpTime);
			m_cEntryPoint.OnFacebook(empty, sDescription);
		}
	}
}
