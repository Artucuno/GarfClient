using System.Collections.Generic;
using UnityEngine;

public class HUDPause : MonoBehaviour
{
	private bool m_bPause;

	private GameObject m_pPanelPauseChampionship;

	private GameObject m_pOptionsPauseChampionship;

	private GameObject m_pButtonRetryChampionship;

	public UILocalize ResumeChampionshipLabel;

	public GameObject WaitingForServerChampionshipLabel;

	public GameObject ResumeChampionshipButton;

	public GameObject PrevButton;

	private GameObject m_pPanelPauseRace;

	public GameObject ResumeRaceButton;

	private GameObject m_pOptionsPauseRace;

	private GameObject m_pChangeTrackRace;

	private GameObject m_pChangeCharacterRace;

	private GameObject m_pRetryRace;

	private GameObject m_pPanelPauseRaceMultiClient;

	public GameObject ResumeRaceMultiClientButton;

	public GameObject WaitingForServerLabel;

	public GameObject OptionsButtonMultiClient;

	private GameObject m_pOptionsPauseRaceMulti;

	private GameObject m_pControlsPanel;

	private GameObject m_oLastPauseBeforeOptions;

	private HUDInGame m_pHudInGame;

	private bool m_bEndOfRace;

	private NetworkMgr m_oNetworkMgr;

	public List<UITexturePattern> PuzzlePiecePauseChampionship = new List<UITexturePattern>();

	public List<UITexturePattern> PuzzlePiecePauseRace = new List<UITexturePattern>();

	public List<GameObject> NamePlates = new List<GameObject>();

	public GameObject PanelPauseChampionship
	{
		get
		{
			return m_pPanelPauseChampionship;
		}
	}

	private void Awake()
	{
		m_oNetworkMgr = (NetworkMgr)Object.FindObjectOfType(typeof(NetworkMgr));
		m_pPanelPauseChampionship = null;
		m_pPanelPauseRaceMultiClient = null;
		m_pPanelPauseRace = null;
		m_pButtonRetryChampionship = null;
		m_pHudInGame = null;
		m_bEndOfRace = false;
		m_oLastPauseBeforeOptions = null;
		if ((bool)PrevButton)
		{
			PrevButton.SetActive(true);
		}
	}

	private void Start()
	{
		m_bPause = false;
		m_pHudInGame = base.gameObject.GetComponent<HUDInGame>();
		m_pPanelPauseChampionship = base.transform.Find("Camera/PanelPauseChampionship").gameObject;
		m_pPanelPauseRace = base.transform.Find("Camera/PanelPauseRace").gameObject;
		m_pControlsPanel = base.transform.Find("Camera/PanelOptionControles").gameObject;
		if ((bool)m_pControlsPanel)
		{
			m_pControlsPanel.SetActive(false);
		}
		if (m_pPanelPauseChampionship != null)
		{
			m_pButtonRetryChampionship = m_pPanelPauseChampionship.transform.Find("PanelButtonsPause/Anchor_Center/ButtonRecommencer").gameObject;
			m_pOptionsPauseChampionship = m_pPanelPauseChampionship.transform.Find("PanelButtonsPause/Anchor_Center/ButtonOptions").gameObject;
			if ((bool)m_pOptionsPauseChampionship)
			{
				if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
				{
					m_pOptionsPauseChampionship.SetActive(false);
				}
				if (Singleton<ChallengeManager>.Instance.IsActive)
				{
					m_pButtonRetryChampionship.SetActive(false);
				}
			}
			m_pPanelPauseChampionship.SetActive(false);
		}
		if (m_pPanelPauseRace != null)
		{
			m_pOptionsPauseRace = m_pPanelPauseRace.transform.Find("PanelButtonsPause/Anchor_Center/ButtonOptions").gameObject;
			m_pChangeTrackRace = m_pPanelPauseRace.transform.Find("PanelButtonsPause/Anchor_Center/ButtonChangerCircuit").gameObject;
			m_pChangeCharacterRace = m_pPanelPauseRace.transform.Find("PanelButtonsPause/Anchor_Center/ButtonChangePersonage").gameObject;
			m_pRetryRace = m_pPanelPauseRace.transform.Find("PanelButtonsPause/Anchor_Center/ButtonRecommencer").gameObject;
			if ((bool)m_pOptionsPauseRace)
			{
				if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
				{
					m_pOptionsPauseRace.SetActive(false);
				}
				if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TUTORIAL)
				{
					m_pChangeTrackRace.SetActive(false);
					m_pChangeCharacterRace.SetActive(false);
				}
				if (Singleton<ChallengeManager>.Instance.IsActive)
				{
					m_pChangeTrackRace.SetActive(false);
					m_pChangeCharacterRace.SetActive(false);
					m_pRetryRace.SetActive(false);
				}
			}
			m_pPanelPauseRace.SetActive(false);
		}
		m_pPanelPauseRaceMultiClient = base.transform.Find("Camera/PanelPauseRaceMultiClient").gameObject;
		if ((bool)m_pPanelPauseRaceMultiClient)
		{
			m_pOptionsPauseRaceMulti = m_pPanelPauseRaceMultiClient.transform.Find("PanelButtonsPause/Anchor_Center/ButtonOptions").gameObject;
			if ((bool)m_pOptionsPauseRaceMulti && Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
			{
				m_pOptionsPauseRaceMulti.SetActive(false);
			}
			m_pPanelPauseRaceMultiClient.SetActive(false);
		}
	}

	private void Update()
	{
		if (m_bEndOfRace || m_pHudInGame.EndHUDDisplayed || Singleton<GameManager>.Instance.GameMode.State == E_GameState.Tutorial || Singleton<InputManager>.Instance.GetAction(EAction.Pause) != 1f)
		{
			return;
		}
		if (!m_bPause)
		{
			if ((bool)Singleton<GameManager>.Instance.SoundManager)
			{
				Singleton<GameManager>.Instance.SoundManager.PlaySound(ERaceSounds.Pause);
				Kart humanKart = Singleton<GameManager>.Instance.GameMode.GetHumanKart();
				if ((bool)humanKart)
				{
					KartSound kartSound = humanKart.KartSound;
					if ((bool)kartSound)
					{
						kartSound.PauseSounds(true);
					}
				}
			}
			if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.CHAMPIONSHIP)
			{
				if (m_pPanelPauseChampionship != null)
				{
					m_pPanelPauseChampionship.SetActive(true);
					if ((bool)m_pOptionsPauseChampionship && Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
					{
						m_pOptionsPauseChampionship.SetActive(false);
					}
					else
					{
						m_pOptionsPauseChampionship.SetActive(true);
					}
					ResumeChampionshipLabel.key = "HUD_PAUSE_RESUME";
					WaitingForServerChampionshipLabel.SetActive(false);
					ShowPuzzlePiece(true);
					Pause();
					NeedToShowRetry();
				}
			}
			else
			{
				if (Network.peerType != 0 && Network.isClient)
				{
					if (m_pPanelPauseRaceMultiClient != null)
					{
						m_pPanelPauseRaceMultiClient.SetActive(true);
						if ((bool)m_pOptionsPauseRaceMulti && Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
						{
							m_pOptionsPauseRaceMulti.SetActive(false);
						}
						WaitingForServerLabel.SetActive(false);
						ResumeRaceMultiClientButton.SetActive(true);
						OptionsButtonMultiClient.SetActive(true);
					}
				}
				else if (m_pPanelPauseRace != null)
				{
					m_pPanelPauseRace.SetActive(true);
					if ((bool)m_pOptionsPauseRace && Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
					{
						m_pOptionsPauseRace.SetActive(false);
					}
					ShowPuzzlePiece(false);
				}
				Pause();
			}
			if (m_bPause && Network.peerType == NetworkPeerType.Disconnected)
			{
				Time.timeScale = 0f;
			}
			return;
		}
		Unpause();
		if (Time.timeScale == 1f && Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TUTORIAL)
		{
			TutorialGameMode tutorialGameMode = (TutorialGameMode)Singleton<GameManager>.Instance.GameMode;
			if (!tutorialGameMode.InstructionShown)
			{
				Time.timeScale = 0f;
			}
		}
	}

	public void NeedToShowRetry()
	{
		if (Singleton<GameConfigurator>.Instance.PlayerConfig.GetAdvantage() == EAdvantage.RestartRace && (bool)m_pButtonRetryChampionship)
		{
			m_pButtonRetryChampionship.SetActive(true);
		}
		else
		{
			m_pButtonRetryChampionship.SetActive(false);
		}
	}

	public void ShowEndOfRace()
	{
		m_bEndOfRace = true;
		if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.CHAMPIONSHIP)
		{
			NeedToShowRetry();
			if (Singleton<GameConfigurator>.Instance.CurrentTrackIndex < Singleton<GameConfigurator>.Instance.ChampionShipData.Tracks.Length)
			{
				ResumeChampionshipLabel.key = "HUD_QUITCHAMPIONSHIP_CIRCUITSUIVANT";
				if (Network.isClient)
				{
					ResumeChampionshipButton.SetActive(false);
					WaitingForServerChampionshipLabel.SetActive(true);
					m_pOptionsPauseChampionship.SetActive(false);
					PrevButton.SetActive(false);
				}
				else
				{
					ResumeChampionshipButton.SetActive(true);
					WaitingForServerChampionshipLabel.SetActive(false);
					if ((bool)m_pOptionsPauseChampionship && Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
					{
						m_pOptionsPauseChampionship.SetActive(false);
					}
					else
					{
						m_pOptionsPauseChampionship.SetActive(true);
					}
					PrevButton.SetActive(true);
				}
			}
			else
			{
				ResumeChampionshipLabel.key = "HUD_QUITCHAMPIONSHIP_RESULTS";
				if (Network.isClient)
				{
					ResumeChampionshipButton.SetActive(false);
					WaitingForServerChampionshipLabel.SetActive(true);
					m_pOptionsPauseChampionship.SetActive(false);
				}
				else
				{
					ResumeChampionshipButton.SetActive(true);
					WaitingForServerChampionshipLabel.SetActive(false);
					if ((bool)m_pOptionsPauseChampionship && Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
					{
						m_pOptionsPauseChampionship.SetActive(false);
					}
					else
					{
						m_pOptionsPauseChampionship.SetActive(true);
					}
				}
			}
			m_pPanelPauseChampionship.SetActive(true);
			ShowPuzzlePiece(true);
		}
		else if (Network.peerType != 0 && Network.isClient)
		{
			if (m_pPanelPauseRaceMultiClient != null)
			{
				m_pPanelPauseRaceMultiClient.SetActive(true);
				WaitingForServerLabel.SetActive(true);
				ResumeRaceMultiClientButton.SetActive(false);
				OptionsButtonMultiClient.SetActive(false);
			}
		}
		else if (m_pPanelPauseRace != null)
		{
			m_pPanelPauseRace.SetActive(true);
			if ((bool)m_pOptionsPauseRace && Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
			{
				m_pOptionsPauseRace.SetActive(false);
			}
			ShowPuzzlePiece(false);
			ResumeRaceButton.SetActive(false);
		}
	}

	public void Pause()
	{
		m_bPause = true;
		if (m_pHudInGame != null)
		{
			m_pHudInGame.Pause(true);
		}
	}

	public void Unpause()
	{
		if (!m_bPause)
		{
			return;
		}
		if ((bool)m_pControlsPanel && m_pControlsPanel.activeSelf)
		{
			OnBackOptions();
		}
		if (m_pPanelPauseChampionship != null)
		{
			m_pPanelPauseChampionship.SetActive(false);
		}
		if (m_pPanelPauseRace != null)
		{
			m_pPanelPauseRace.SetActive(false);
		}
		if (m_pPanelPauseRaceMultiClient != null)
		{
			m_pPanelPauseRaceMultiClient.SetActive(false);
		}
		m_bPause = false;
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			Time.timeScale = 1f;
		}
		Kart humanKart = Singleton<GameManager>.Instance.GameMode.GetHumanKart();
		if ((bool)humanKart)
		{
			KartSound kartSound = humanKart.KartSound;
			if ((bool)kartSound)
			{
				kartSound.PauseSounds(false);
			}
		}
		if (m_pHudInGame != null)
		{
			m_pHudInGame.Pause(false);
		}
		if ((bool)Singleton<GameManager>.Instance.SoundManager)
		{
			Singleton<GameManager>.Instance.SoundManager.PlaySound(ERaceSounds.Pause);
		}
	}

	public void OnRetry()
	{
		if (!m_bEndOfRace)
		{
			Unpause();
		}
		else
		{
			if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.CHAMPIONSHIP)
			{
				Singleton<GameConfigurator>.Instance.CurrentTrackIndex--;
			}
			m_bEndOfRace = false;
		}
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.CHAMPIONSHIP)
			{
				Singleton<GameConfigurator>.Instance.RestartChampionShipRace();
			}
			else
			{
				Singleton<GameConfigurator>.Instance.RankingManager.RestartRace();
			}
			if ((bool)Singleton<GameConfigurator>.Instance.PlayerConfig)
			{
				Singleton<GameConfigurator>.Instance.PlayerConfig.ResetAdvantages();
			}
			LoadingManager.LoadLevel(Application.loadedLevelName);
		}
		else if (Network.isServer)
		{
			m_oNetworkMgr.networkView.RPC("RetryRace", RPCMode.All);
		}
	}

	public void OnQuit()
	{
		Unpause();
		Singleton<GameConfigurator>.Instance.MenuToLaunch = EMenus.MENU_WELCOME;
		if ((bool)Singleton<GameConfigurator>.Instance.PlayerConfig)
		{
			Singleton<GameConfigurator>.Instance.PlayerConfig.ResetAdvantages();
		}
		LoadingManager.LoadLevel("MenuRoot");
		Network.Disconnect();
	}

	public void OnChangeCharacter()
	{
		Unpause();
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			Singleton<GameConfigurator>.Instance.MenuToLaunch = EMenus.MENU_SELECT_KART;
			if ((bool)Singleton<GameConfigurator>.Instance.PlayerConfig)
			{
				Singleton<GameConfigurator>.Instance.PlayerConfig.ResetAdvantages();
			}
			LoadingManager.LoadLevel("MenuRoot");
		}
		else if (Network.isServer)
		{
			m_oNetworkMgr.networkView.RPC("QuitToMenu", RPCMode.All, 4);
		}
	}

	public void OnChangeTrack()
	{
		Unpause();
		if (Network.peerType != NetworkPeerType.Client)
		{
			Singleton<GameConfigurator>.Instance.MenuToLaunch = EMenus.MENU_SELECT_TRACK;
			if ((bool)Singleton<GameConfigurator>.Instance.PlayerConfig)
			{
				Singleton<GameConfigurator>.Instance.PlayerConfig.ResetAdvantages();
			}
			if (Network.isServer)
			{
				m_oNetworkMgr.networkView.RPC("QuitToMenu", RPCMode.Others, 7);
			}
			LoadingManager.LoadLevel("MenuRoot");
		}
	}

	public void OnResume()
	{
		if (m_bEndOfRace)
		{
			if ((bool)Singleton<GameConfigurator>.Instance.PlayerConfig)
			{
				Singleton<GameConfigurator>.Instance.PlayerConfig.ResetAdvantages();
			}
			if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.CHAMPIONSHIP)
			{
				if (Singleton<GameConfigurator>.Instance.CurrentTrackIndex < Singleton<GameConfigurator>.Instance.ChampionShipData.Tracks.Length)
				{
					((InGameGameMode)Singleton<GameManager>.Instance.GameMode).ResetRace();
					if (Network.isServer)
					{
						m_oNetworkMgr.networkView.RPC("NextRace", RPCMode.All);
					}
					else if (Network.peerType == NetworkPeerType.Disconnected)
					{
						ChampionShipGameMode.NextRace();
					}
				}
				else if (Network.isServer)
				{
					m_oNetworkMgr.networkView.RPC("ShowPodium", RPCMode.All);
				}
				else if (Network.peerType == NetworkPeerType.Disconnected)
				{
					Singleton<GameManager>.Instance.GameMode.State = E_GameState.Podium;
					m_pPanelPauseChampionship.SetActive(false);
				}
				return;
			}
			Unpause();
			if (Time.timeScale == 1f && Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TUTORIAL)
			{
				TutorialGameMode tutorialGameMode = (TutorialGameMode)Singleton<GameManager>.Instance.GameMode;
				if (!tutorialGameMode.InstructionShown)
				{
					Time.timeScale = 0f;
				}
			}
			return;
		}
		Unpause();
		if (Time.timeScale == 1f && Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TUTORIAL)
		{
			TutorialGameMode tutorialGameMode2 = (TutorialGameMode)Singleton<GameManager>.Instance.GameMode;
			if (!tutorialGameMode2.InstructionShown)
			{
				Time.timeScale = 0f;
			}
		}
	}

	public void ShowPuzzlePiece(bool bChampionship)
	{
		if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TIME_TRIAL)
		{
			for (int i = 0; i < PuzzlePiecePauseRace.Count; i++)
			{
				PuzzlePiecePauseRace[i].gameObject.SetActive(false);
			}
			return;
		}
		List<UITexturePattern> list = ((!bChampionship) ? PuzzlePiecePauseRace : PuzzlePiecePauseChampionship);
		string startScene = Singleton<GameConfigurator>.Instance.StartScene;
		for (int j = 0; j < list.Count; j++)
		{
			string pPiece = startScene + "_" + j;
			list[j].ChangeTexture(Singleton<GameSaveManager>.Instance.IsPuzzlePieceUnlocked(pPiece) ? 1 : 0);
		}
	}

	public void OnBackOptions()
	{
		if ((bool)m_pControlsPanel)
		{
			HUDOptions component = m_pControlsPanel.GetComponent<HUDOptions>();
			if ((bool)component)
			{
				component.SaveSensibility();
			}
			m_pControlsPanel.SetActive(false);
		}
		if ((bool)m_oLastPauseBeforeOptions)
		{
			m_oLastPauseBeforeOptions.SetActive(true);
		}
	}

	public void OnOptions()
	{
		if ((bool)m_pControlsPanel)
		{
			m_pControlsPanel.SetActive(true);
			if (m_pPanelPauseChampionship.activeSelf)
			{
				m_oLastPauseBeforeOptions = m_pPanelPauseChampionship;
			}
			else if (m_pPanelPauseRace.activeSelf)
			{
				m_oLastPauseBeforeOptions = m_pPanelPauseRace;
			}
			else if ((bool)m_pPanelPauseRaceMultiClient)
			{
				m_oLastPauseBeforeOptions = m_pPanelPauseRaceMultiClient;
			}
			if ((bool)m_oLastPauseBeforeOptions)
			{
				m_oLastPauseBeforeOptions.SetActive(false);
			}
		}
	}

	public void CreateNamePlates()
	{
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			return;
		}
		PlayerData[] playerDataList = Singleton<GameConfigurator>.Instance.PlayerDataList;
		for (int i = 0; i < Singleton<GameManager>.Instance.GameMode.PlayerCount; i++)
		{
			Kart kart = Singleton<GameManager>.Instance.GameMode.GetKart(i);
			if (!(kart != null))
			{
				continue;
			}
			HUDInGame hud = Singleton<GameManager>.Instance.GameMode.Hud;
			if (!(hud != null))
			{
				continue;
			}
			GameObject original = (GameObject)Resources.Load("NamePlate");
			RaceScoreData scoreData = Singleton<GameConfigurator>.Instance.RankingManager.GetScoreData(kart.GetVehicleId());
			if (!scoreData.IsAI && kart.GetControlType() != 0)
			{
				GameObject gameObject = (GameObject)Object.Instantiate(original);
				NamePlates.Add(gameObject);
				UILabel component = gameObject.GetComponent<UILabel>();
				if (component != null)
				{
					component.text = playerDataList[kart.GetVehicleId()].Pseudo;
					component.color = playerDataList[kart.GetVehicleId()].CharacColor;
					gameObject.transform.parent = kart.transform;
					gameObject.transform.localPosition = new Vector3(0f, 1.5f, 0f);
					gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
				}
			}
		}
	}

	private void OnApplicationPause(bool goingPause)
	{
		Debug.Log("OnApplicationPause + " + goingPause);
		if (goingPause || !LoadingManager.loadingFinished)
		{
			return;
		}
		if (m_bPause)
		{
			Kart humanKart = Singleton<GameManager>.Instance.GameMode.GetHumanKart();
			if ((bool)humanKart)
			{
				KartSound kartSound = humanKart.KartSound;
				if ((bool)kartSound)
				{
					kartSound.PauseSounds(true);
				}
			}
		}
		else
		{
			Singleton<InputManager>.Instance.SetAction(EAction.Pause, 1f);
		}
	}
}
