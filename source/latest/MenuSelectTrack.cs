using System;
using UnityEngine;

public class MenuSelectTrack : AbstractMenu
{
	private int m_iCurrentTrack;

	public UILabel TrackName;

	public UICheckbox DefaultButton;

	public UITexturePattern ChampionshipIcon;

	public UITexturePattern GameModeIcon;

	public UITexturePattern DifficultyIcon;

	public UILabel LabelCup;

	public GameObject TimeTrialPanel;

	public GameObject ChampionshipPanel;

	public UILabel Description;

	public UILabel TimeTrialTimeRef;

	public UITexturePattern MedalSprite;

	public UITexturePattern PuzzlePiece1;

	public UITexturePattern PuzzlePiece2;

	public UITexturePattern PuzzlePiece3;

	public UITexture TrackScreenshot;

	public MessageButton[] ButtonSelectTrack = new MessageButton[4];

	private bool m_bLateInitialized;

	public override void Start()
	{
		m_iCurrentTrack = 0;
	}

	public override void OnEnter()
	{
		base.OnEnter();
		Singleton<GameConfigurator>.Instance.StartScene = Singleton<GameConfigurator>.Instance.ChampionShipData.Tracks[0];
		ChampionshipIcon.ChangeTexture(Singleton<GameConfigurator>.Instance.ChampionShipData.Index);
		LabelCup.text = Singleton<GameConfigurator>.Instance.ChampionShipData.ChampionShipName;
		if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.CHAMPIONSHIP)
		{
			GameModeIcon.ChangeTexture(0);
			Description.text = string.Format(Localization.instance.Get("MENU_SELECT_TRACK_CHAMPIONSHIP"), Singleton<GameConfigurator>.Instance.ChampionShipData.ChampionShipName);
			TrackName.text = Singleton<GameConfigurator>.Instance.ChampionShipData.ChampionShipName;
			TimeTrialPanel.SetActive(false);
			ChampionshipPanel.SetActive(true);
		}
		else if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.SINGLE)
		{
			GameModeIcon.ChangeTexture(1);
			Description.text = Localization.instance.Get("MENU_SELECT_TRACK_SINGLE");
			TimeTrialPanel.SetActive(false);
			ChampionshipPanel.SetActive(true);
		}
		else if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TIME_TRIAL)
		{
			GameModeIcon.ChangeTexture(2);
			Description.text = Localization.instance.Get("MENU_SELECT_TRACK_TIMETRIAL");
			ChampionshipPanel.SetActive(false);
			TimeTrialPanel.SetActive(true);
		}
		DifficultyIcon.ChangeTexture((int)Singleton<GameConfigurator>.Instance.Difficulty);
		RefreshPanel();
		m_bLateInitialized = false;
	}

	public override void OnExit()
	{
		base.OnExit();
		DefaultButton.isChecked = true;
		Resources.UnloadAsset(TrackScreenshot.mainTexture);
	}

	private void LateUpdate()
	{
		if (!m_bLateInitialized)
		{
			if ((bool)ButtonSelectTrack[Singleton<GameConfigurator>.Instance.CurrentTrackIndex])
			{
				ButtonSelectTrack[Singleton<GameConfigurator>.Instance.CurrentTrackIndex].SendMessage("OnClick");
			}
			m_bLateInitialized = true;
		}
	}

	public override void Update()
	{
		if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape))
		{
			ActSwapMenu(EMenus.MENU_CHAMPIONSHIP);
		}
	}

	public void OnSelectTrack(int iTrack)
	{
		m_iCurrentTrack = iTrack;
		RefreshPanel();
		if (Singleton<GameConfigurator>.Instance.GameModeType != E_GameModeType.CHAMPIONSHIP)
		{
			Singleton<GameConfigurator>.Instance.StartScene = Singleton<GameConfigurator>.Instance.ChampionShipData.Tracks[m_iCurrentTrack];
			Singleton<GameConfigurator>.Instance.CurrentTrackIndex = iTrack;
		}
	}

	public void RefreshPanel()
	{
		string text = Singleton<GameConfigurator>.Instance.ChampionShipData.Tracks[m_iCurrentTrack];
		TrackName.text = Singleton<GameConfigurator>.Instance.ChampionShipData.TracksName[m_iCurrentTrack];
		if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TIME_TRIAL)
		{
			int rpTime = 0;
			Singleton<GameSaveManager>.Instance.GetTimeTrialRecord(text, ref rpTime);
			string text2 = TimeSpan.FromMilliseconds(rpTime).FormatRaceTime();
			TimeTrialTimeRef.text = text2;
			E_TimeTrialMedal medal = Singleton<GameSaveManager>.Instance.GetMedal(text, false);
			if (medal != 0)
			{
				MedalSprite.gameObject.SetActive(true);
				MedalSprite.ChangeTexture((int)(medal - 1));
			}
			else
			{
				MedalSprite.gameObject.SetActive(false);
			}
		}
		else
		{
			PuzzlePiece1.ChangeTexture(Singleton<GameSaveManager>.Instance.IsPuzzlePieceUnlocked(text + "_0") ? 1 : 0);
			PuzzlePiece2.ChangeTexture(Singleton<GameSaveManager>.Instance.IsPuzzlePieceUnlocked(text + "_1") ? 1 : 0);
			PuzzlePiece3.ChangeTexture(Singleton<GameSaveManager>.Instance.IsPuzzlePieceUnlocked(text + "_2") ? 1 : 0);
		}
		if (TrackScreenshot != null)
		{
			Resources.UnloadAsset(TrackScreenshot.mainTexture);
			TrackScreenshot.mainTexture = Resources.Load(text, typeof(Texture2D)) as Texture2D;
		}
	}
}
