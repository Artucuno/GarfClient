using System;
using System.Collections.Generic;
using UnityEngine;

public class HUDTrackPresentation : MonoBehaviour
{
	public GameObject PanelChallenge;

	public GameObject PanelTimeTrial;

	public GameObject PanelAdvantages;

	public UILabel TrackName;

	public UILabel AdvantageLabel;

	public UITexturePattern GameModeSprite;

	public UITexturePattern DifficultySprite;

	public UITexturePattern ChampionshipIcon;

	public List<AdvantageSlots> AdvantageSlotsList;

	public UILabel NetworkSynchro;

	public GameObject NoThanks;

	public GameObject Valid;

	private UITexturePattern ValidTexture;

	public UISprite MedalIcon;

	public UILabel MedalTime;

	public UILabel BestTime;

	public UILabel ChallengeFirstObjective;

	public UILabel ChallengeSecondObjective;

	private int m_iIndex;

	private bool m_bValidateAdvantage;

	public GameObject MedalPanel;

	public List<GameObject> PuzzleGO = new List<GameObject>();

	private List<UITexturePattern> m_PuzzlePiece = new List<UITexturePattern>();

	public int m_iTimeLimit = 30;

	public int m_iStartDisplaying = 10;

	public UILabel m_oTimerLabel;

	private bool m_bLimitTime = true;

	private float m_fElapsedTime;

	private void Start()
	{
		if (NoThanks == null)
		{
			NoThanks = GameObject.Find("NoThanks");
		}
		if (Valid == null)
		{
			Valid = GameObject.Find("Valid");
		}
		if (Valid != null)
		{
			ValidTexture = Valid.GetComponent<UITexturePattern>();
		}
		if (Network.peerType == NetworkPeerType.Disconnected || m_oTimerLabel == null)
		{
			m_bLimitTime = false;
		}
		else
		{
			m_fElapsedTime = 0f;
			m_bLimitTime = true;
		}
		if (m_oTimerLabel != null)
		{
			m_oTimerLabel.enabled = false;
		}
		foreach (GameObject item in PuzzleGO)
		{
			UITexturePattern component = item.GetComponent<UITexturePattern>();
			m_PuzzlePiece.Add(component);
		}
		m_bValidateAdvantage = false;
		m_iIndex = -1;
		NetworkSynchro.gameObject.SetActive(false);
		if (Singleton<GameConfigurator>.Instance.ChampionShipData != null)
		{
			ChampionshipIcon.ChangeTexture(Singleton<GameConfigurator>.Instance.ChampionShipData.Index);
			if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.CHAMPIONSHIP)
			{
				TrackName.text = string.Format(Localization.instance.Get("HUD_DYN_CHAMPIONSHIP_ROUND"), Singleton<GameConfigurator>.Instance.ChampionShipData.ChampionShipName, Singleton<GameConfigurator>.Instance.CurrentTrackIndex + 1);
			}
			else
			{
				TrackName.text = Singleton<GameConfigurator>.Instance.ChampionShipData.TracksName[Singleton<GameConfigurator>.Instance.CurrentTrackIndex];
			}
		}
		if (Singleton<GameConfigurator>.Instance.PlayerConfig.GetAdvantages().Count == 0)
		{
			Valid.SetActive(true);
			NoThanks.SetActive(false);
			AdvantageLabel.text = Localization.instance.Get("HUD_TRACKPRESENTATION_ADV_EMPTY");
		}
		else
		{
			Valid.SetActive(false);
			NoThanks.SetActive(true);
			AdvantageLabel.text = Localization.instance.Get("HUD_TRACKPRESENTATION_ADV");
		}
		DifficultySprite.ChangeTexture((int)Singleton<GameConfigurator>.Instance.Difficulty);
		PanelChallenge.NullSafe(delegate(GameObject p)
		{
			p.SetActive(false);
		});
		PanelTimeTrial.NullSafe(delegate(GameObject p)
		{
			p.SetActive(false);
		});
		PanelAdvantages.NullSafe(delegate(GameObject p)
		{
			p.SetActive(true);
		});
		if (Singleton<ChallengeManager>.Instance.IsActive)
		{
			string _First = string.Empty;
			string _Second = string.Empty;
			Singleton<ChallengeManager>.Instance.GetLocalizedObjectives(out _First, out _Second);
			ChallengeFirstObjective.text = _First;
			ChallengeSecondObjective.text = _Second;
			PanelChallenge.NullSafe(delegate(GameObject p)
			{
				p.SetActive(true);
			});
			if (Singleton<ChallengeManager>.Instance.IsMonday)
			{
				PanelAdvantages.NullSafe(delegate(GameObject p)
				{
					p.SetActive(false);
				});
			}
		}
		switch (Singleton<GameConfigurator>.Instance.GameModeType)
		{
		case E_GameModeType.TIME_TRIAL:
		{
			if (GameModeSprite != null)
			{
				GameModeSprite.ChangeTexture(2);
			}
			PanelTimeTrial.NullSafe(delegate(GameObject p)
			{
				p.SetActive(true);
			});
			PanelAdvantages.NullSafe(delegate(GameObject p)
			{
				p.SetActive(false);
			});
			Singleton<GameSaveManager>.Instance.EarnAdvantage(EAdvantage.BoostStart, 1, false);
			Singleton<GameConfigurator>.Instance.PlayerConfig.SelectAdvantage(EAdvantage.BoostStart);
			GameObject gameObject = GameObject.Find("Race");
			string startScene2 = Singleton<GameConfigurator>.Instance.StartScene;
			string rpTime = string.Empty;
			Singleton<GameSaveManager>.Instance.GetTimeTrialRecord(startScene2, ref rpTime);
			BestTime.text = rpTime;
			TimeTrialConfig component2 = gameObject.GetComponent<TimeTrialConfig>();
			switch (Singleton<GameSaveManager>.Instance.GetMedal(startScene2, true))
			{
			case E_TimeTrialMedal.Platinium:
				MedalPanel.SetActive(false);
				break;
			case E_TimeTrialMedal.Gold:
				MedalTime.text = TimeSpan.FromMilliseconds(component2.Platinium).FormatRaceTime();
				MedalIcon.spriteName = "icon_medal_platinum";
				break;
			case E_TimeTrialMedal.Silver:
				MedalTime.text = TimeSpan.FromMilliseconds(component2.Gold).FormatRaceTime();
				MedalIcon.spriteName = "icon_medal_gold";
				break;
			case E_TimeTrialMedal.Bronze:
				MedalTime.text = TimeSpan.FromMilliseconds(component2.Silver).FormatRaceTime();
				MedalIcon.spriteName = "icon_medal_silver";
				break;
			case E_TimeTrialMedal.None:
				MedalTime.text = TimeSpan.FromMilliseconds(component2.Bronze).FormatRaceTime();
				MedalIcon.spriteName = "icon_medal_bronze";
				break;
			}
			foreach (GameObject item2 in PuzzleGO)
			{
				item2.SetActive(false);
			}
			NoThanks.SetActive(false);
			Valid.SetActive(true);
			break;
		}
		case E_GameModeType.CHAMPIONSHIP:
		{
			if (GameModeSprite != null)
			{
				GameModeSprite.ChangeTexture(0);
			}
			string startScene4 = Singleton<GameConfigurator>.Instance.StartScene;
			for (int k = 0; k < m_PuzzlePiece.Count; k++)
			{
				string pPiece3 = startScene4 + "_" + k;
				m_PuzzlePiece[k].ChangeTexture(Singleton<GameSaveManager>.Instance.IsPuzzlePieceUnlocked(pPiece3) ? 1 : 0);
			}
			break;
		}
		case E_GameModeType.SINGLE:
		{
			if (GameModeSprite != null)
			{
				GameModeSprite.ChangeTexture(1);
			}
			string startScene3 = Singleton<GameConfigurator>.Instance.StartScene;
			for (int j = 0; j < m_PuzzlePiece.Count; j++)
			{
				string pPiece2 = startScene3 + "_" + j;
				m_PuzzlePiece[j].ChangeTexture(Singleton<GameSaveManager>.Instance.IsPuzzlePieceUnlocked(pPiece2) ? 1 : 0);
			}
			break;
		}
		default:
		{
			string startScene = Singleton<GameConfigurator>.Instance.StartScene;
			for (int i = 0; i < m_PuzzlePiece.Count; i++)
			{
				string pPiece = startScene + "_" + i;
				m_PuzzlePiece[i].ChangeTexture(Singleton<GameSaveManager>.Instance.IsPuzzlePieceUnlocked(pPiece) ? 1 : 0);
			}
			break;
		}
		}
		if (DebugMgr.Instance != null && Singleton<GameConfigurator>.Instance.NbSlots == 0)
		{
			Singleton<GameConfigurator>.Instance.NbSlots = 3;
			DebugMgr.Instance.ApplyAdvantages();
		}
		ConfigureSlots();
	}

	public void Update()
	{
		if (!m_bLimitTime)
		{
			return;
		}
		m_fElapsedTime += Time.deltaTime;
		if (m_fElapsedTime > (float)(m_iTimeLimit - m_iStartDisplaying))
		{
			m_oTimerLabel.enabled = true;
			m_oTimerLabel.text = ((int)((float)m_iTimeLimit - m_fElapsedTime)).ToString();
			if (m_fElapsedTime >= (float)m_iTimeLimit)
			{
				m_oTimerLabel.enabled = false;
				m_bLimitTime = false;
				OnValid();
			}
		}
	}

	public void ConfigureSlots()
	{
		for (int i = 0; i < AdvantageSlotsList.Count; i++)
		{
			if (i < Singleton<GameConfigurator>.Instance.NbSlots)
			{
				if (i < Singleton<GameConfigurator>.Instance.PlayerConfig.GetAdvantages().Count)
				{
					EAdvantage eAdvantage = Singleton<GameConfigurator>.Instance.PlayerConfig.GetAdvantages()[i];
					if (eAdvantage != EAdvantage.None)
					{
						AdvantageSlotsList[i].Sprite.gameObject.SetActive(true);
						AdvantageSlotsList[i].Parent.GetComponent<UICheckbox>().enabled = true;
						AdvantageSlotsList[i].Sprite.ChangeTexture((int)eAdvantage);
					}
				}
				else
				{
					AdvantageSlotsList[i].Sprite.gameObject.SetActive(false);
					UICheckbox component = AdvantageSlotsList[i].Parent.GetComponent<UICheckbox>();
					component.enabled = false;
					AdvantageSlotsList[i].Parent.SetActive(false);
				}
			}
			else
			{
				AdvantageSlotsList[i].Parent.SetActive(false);
			}
		}
	}

	public void OnSlot1(bool Checked)
	{
		SelectAdvantage(0, Checked);
	}

	public void OnSlot2(bool Checked)
	{
		SelectAdvantage(1, Checked);
	}

	public void OnSlot3(bool Checked)
	{
		SelectAdvantage(2, Checked);
	}

	public void OnSlot4(bool Checked)
	{
		SelectAdvantage(3, Checked);
	}

	public void OnNoThanks(bool Checked)
	{
		SelectAdvantage(4, Checked);
	}

	public void OnValid()
	{
		if (m_bValidateAdvantage)
		{
			return;
		}
		if (m_iIndex > -1 && m_iIndex < Singleton<GameConfigurator>.Instance.PlayerConfig.GetAdvantages().Count)
		{
			EAdvantage advantage = Singleton<GameConfigurator>.Instance.PlayerConfig.GetAdvantages()[m_iIndex];
			Singleton<GameConfigurator>.Instance.PlayerConfig.SelectAdvantage(advantage);
		}
		m_bValidateAdvantage = true;
		for (int i = 0; i < AdvantageSlotsList.Count; i++)
		{
			if (i < Singleton<GameConfigurator>.Instance.NbSlots)
			{
				UICheckbox component = AdvantageSlotsList[i].Parent.GetComponent<UICheckbox>();
				component.enabled = false;
			}
		}
		PanelAdvantages.SetActive(false);
		if (Network.peerType != 0)
		{
			ValidTexture.ChangeTexture(1);
			NetworkSynchro.gameObject.SetActive(true);
			if (!Network.isServer)
			{
				NetworkSynchro.text = Localization.instance.Get("NETWORK_WAITING_SERVER");
			}
			else
			{
				NetworkSynchro.text = Localization.instance.Get("NETWORK_WAITING_CLIENTS");
			}
		}
	}

	public void SelectAdvantage(int Index, bool Checked)
	{
		if (Checked)
		{
			m_iIndex = ((Index >= 4) ? (-1) : Index);
			Valid.SetActive(true);
		}
		else
		{
			m_iIndex = -1;
			Valid.SetActive(false);
		}
	}

	public bool ValidateAdvantage()
	{
		return m_bValidateAdvantage;
	}
}
