using System;
using System.Collections;
using UnityEngine;

public class HUDInGame : MonoBehaviour
{
	public float ChampResultNextButtonDelay = 5f;

	private GameObject m_pHudBonus;

	private float m_fDelayNext = 0.3f;

	private GameObject m_pHudControls;

	private GameObject m_pHudPosition;

	private GameObject m_pHudCountdown;

	private GameObject m_pHudFinish;

	private GameObject m_pHudTrackPresentation;

	private GameObject m_pHudTutorial;

	private GameObject m_pHudChallenge;

	private GameObject _hudEndChampionShipRace;

	private GameObject _hudEndChampionShipRank;

	private GameObject _nextButton;

	private GameObject _hudEndSingleRace;

	private GameObject _hudEndTimeTrialRace1;

	private GameObject _hudEndTimeTrialRace2;

	private GameObject _hudTimeTrialResults;

	private GameObject _hudRadar;

	private GameObject m_pHudFade;

	private GameObject _hudChampionShipResult;

	private HUDControls m_pHudControlsComp;

	private HUDCountdown m_pHudCountdownComp;

	private HUDBonus m_pHudBonusComp;

	private HUDPosition m_pHudPositionComp;

	private HUDTrackPresentation m_pHudTrackPresentationComp;

	private HUDTutorial m_pHudTutorialComp;

	private HUDEndChampionsShipRace m_pHudEndChampionshipRaceComp;

	private HUDEndSingleRace m_pHudEndSingleRaceComp;

	private HUDEndTimeTrial m_pHudEndTimeTrialComp;

	private HUDEndTimeTrialMedal m_pHudEndTimeTrialMedalComp;

	private HUDResultsTimeTrial m_pHudTimeTrialResultsComp;

	private HUDFinish m_pHudFinishComp;

	private HUDPause m_pHudPauseComp;

	private HUDFade m_pHudFadeComp;

	private bool m_bEndChampionshipSecond;

	private bool m_bNeedToEnterFinishRace;

	private bool m_bEndHUDDisplayed;

	private bool m_bFinishLineCrossed;

	private HUDRadar _hudRadarComp;

	private NetworkMgr m_oNetworkMgr;

	private bool m_bDelayTrackPresentation;

	private float _deferNextDisplayTimer;

	private bool _deferNextDisplay;

	public HUDPause HUDPause
	{
		get
		{
			return m_pHudPauseComp;
		}
	}

	public HUDRadar HudRadarComp
	{
		get
		{
			return _hudRadarComp;
		}
	}

	public HUDCountdown Countdown
	{
		get
		{
			return m_pHudCountdownComp;
		}
	}

	public HUDBonus Bonus
	{
		get
		{
			return m_pHudBonusComp;
		}
	}

	public HUDPosition Position
	{
		get
		{
			return m_pHudPositionComp;
		}
	}

	public HUDTrackPresentation TrackPresentation
	{
		get
		{
			return m_pHudTrackPresentationComp;
		}
	}

	public HUDTutorial HUDTutorial
	{
		get
		{
			return m_pHudTutorialComp;
		}
	}

	public HUDEndChampionsShipRace EndChampionshipRace
	{
		get
		{
			return m_pHudEndChampionshipRaceComp;
		}
	}

	public HUDEndSingleRace HUDEndSingleRace
	{
		get
		{
			return m_pHudEndSingleRaceComp;
		}
	}

	public HUDEndTimeTrial HUDEndTimeTrial
	{
		get
		{
			return m_pHudEndTimeTrialComp;
		}
	}

	public HUDResultsTimeTrial HUDTimeTrialResults
	{
		get
		{
			return m_pHudTimeTrialResultsComp;
		}
	}

	public HUDEndTimeTrialMedal HUDEndTimeTrial2
	{
		get
		{
			return m_pHudEndTimeTrialMedalComp;
		}
	}

	public HUDFinish HUDFinish
	{
		get
		{
			return m_pHudFinishComp;
		}
	}

	public HUDFade HUDFade
	{
		get
		{
			return m_pHudFadeComp;
		}
	}

	public GameObject HudEndChampionshipRace
	{
		get
		{
			return _hudEndChampionShipRace;
		}
	}

	public GameObject HudEndChampionshipRank
	{
		get
		{
			return _hudEndChampionShipRank;
		}
	}

	public GameObject NextButton
	{
		get
		{
			return _nextButton;
		}
	}

	public bool FinishLineCrossed
	{
		get
		{
			return m_bFinishLineCrossed;
		}
	}

	public bool EndHUDDisplayed
	{
		get
		{
			return m_bEndHUDDisplayed;
		}
	}

	public HUDControls HUDControls
	{
		get
		{
			return m_pHudControlsComp;
		}
	}

	public GameObject HUDPodium
	{
		get
		{
			return _hudChampionShipResult;
		}
	}

	public void Awake()
	{
		m_pHudBonus = null;
		m_pHudControls = null;
		m_pHudPosition = null;
		m_pHudCountdown = null;
		m_pHudControlsComp = null;
		_hudEndChampionShipRace = null;
		_hudEndChampionShipRank = null;
		_hudEndTimeTrialRace1 = null;
		_hudEndTimeTrialRace2 = null;
		_hudTimeTrialResults = null;
		m_bEndChampionshipSecond = false;
		m_pHudTrackPresentation = null;
		m_pHudCountdownComp = null;
		m_pHudBonusComp = null;
		m_pHudPositionComp = null;
		m_pHudFinish = null;
		m_pHudChallenge = null;
		m_pHudFadeComp = null;
		m_pHudFade = null;
		HUDNextButton.OnNextClick = (Action)Delegate.Combine(HUDNextButton.OnNextClick, new Action(DoNext));
		m_pHudBonus = GameObject.Find("PanelBonus");
		GameObject gameObject = GameObject.Find("PanelControles");
		if ((bool)gameObject)
		{
			gameObject.SetActive(false);
		}
		GameObject gameObject2 = GameObject.Find("PanelControles43");
		if ((bool)gameObject2)
		{
			gameObject2.SetActive(false);
		}
		if (!Tricks.isTablet())
		{
			m_pHudControls = gameObject;
		}
		else
		{
			m_pHudControls = gameObject2;
		}
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			m_pHudControls.SetActive(true);
		}
		m_pHudPosition = GameObject.Find("PanelPosition");
		m_pHudCountdown = GameObject.Find("PanelCountdown");
		m_pHudTrackPresentation = GameObject.Find("PanelTrackPresentation");
		m_pHudTutorial = GameObject.Find("PanelTutorial");
		if (m_pHudTutorial != null)
		{
			m_pHudTutorial.SetActive(false);
		}
		GameObject gameObject3 = GameObject.Find("PanelTutorial43");
		gameObject3.SetActive(false);
		if (Tricks.isTablet())
		{
			m_pHudTutorial = gameObject3;
		}
		m_pHudFinish = GameObject.Find("PanelFinish");
		m_pHudChallenge = GameObject.Find("PanelResultatsChallenge");
		m_pHudPauseComp = base.gameObject.GetComponent<HUDPause>();
		m_pHudControlsComp = m_pHudControls.GetComponent<HUDControls>();
		m_pHudCountdownComp = m_pHudCountdown.GetComponent<HUDCountdown>();
		m_pHudBonusComp = GetComponent<HUDBonus>();
		m_pHudPositionComp = m_pHudPosition.GetComponent<HUDPosition>();
		m_pHudTrackPresentationComp = m_pHudTrackPresentation.GetComponent<HUDTrackPresentation>();
		if (m_pHudTutorial != null)
		{
			m_pHudTutorialComp = m_pHudTutorial.GetComponent<HUDTutorial>();
		}
		m_pHudFinishComp = m_pHudFinish.GetComponent<HUDFinish>();
		_hudEndChampionShipRace = GameObject.Find("PanelEndChampionship1");
		m_pHudEndChampionshipRaceComp = _hudEndChampionShipRace.GetComponent<HUDEndChampionsShipRace>();
		m_pHudEndChampionshipRaceComp.Init();
		_hudEndChampionShipRank = GameObject.Find("PanelEndChampionship2");
		_hudEndChampionShipRank.GetComponent<HUDChampionsShipRanking>().Init();
		_hudEndSingleRace = GameObject.Find("PanelEndRace");
		m_pHudEndSingleRaceComp = _hudEndSingleRace.GetComponent<HUDEndSingleRace>();
		m_pHudEndSingleRaceComp.Init();
		_hudEndTimeTrialRace1 = GameObject.Find("PanelEndTimeTrial1");
		_hudEndTimeTrialRace2 = GameObject.Find("PanelEndTimeTrial2");
		_hudTimeTrialResults = GameObject.Find("PanelResultatsTimeTrial");
		m_pHudEndTimeTrialComp = _hudEndTimeTrialRace1.GetComponent<HUDEndTimeTrial>();
		m_pHudTimeTrialResultsComp = _hudTimeTrialResults.GetComponent<HUDResultsTimeTrial>();
		m_pHudEndTimeTrialMedalComp = _hudEndTimeTrialRace2.GetComponent<HUDEndTimeTrialMedal>();
		_hudChampionShipResult = GameObject.Find("PanelResultatsChampionnat");
		_nextButton = GameObject.Find("PanelButtonNext");
		_hudRadar = GameObject.Find("PanelRadar");
		_hudRadarComp = _hudRadar.GetComponent<HUDRadar>();
		m_pHudFade = GameObject.Find("PanelFade");
		m_pHudFadeComp = m_pHudFade.GetComponentInChildren<HUDFade>();
		m_bNeedToEnterFinishRace = false;
		m_oNetworkMgr = (NetworkMgr)UnityEngine.Object.FindObjectOfType(typeof(NetworkMgr));
	}

	public void Start()
	{
		_hudEndChampionShipRace.SetActive(false);
		_hudEndChampionShipRank.SetActive(false);
		_hudEndSingleRace.SetActive(false);
		_hudEndTimeTrialRace1.SetActive(false);
		_hudEndTimeTrialRace2.SetActive(false);
		_hudTimeTrialResults.SetActive(false);
		_hudChampionShipResult.SetActive(false);
		_nextButton.SetActive(false);
		m_pHudBonus.SetActive(false);
		m_pHudPosition.SetActive(false);
		m_pHudCountdown.SetActive(false);
		m_pHudTrackPresentation.SetActive(false);
		m_pHudChallenge.SetActive(false);
		_hudRadar.SetActive(false);
	}

	private void OnDestroy()
	{
		HUDNextButton.OnNextClick = (Action)Delegate.Remove(HUDNextButton.OnNextClick, new Action(DoNext));
	}

	public void EnterTutorial(Action<bool> _OnNext)
	{
		if (m_pHudTutorial != null && m_pHudTutorialComp != null)
		{
			m_pHudTutorialComp.Next = _OnNext;
			m_pHudTutorial.SetActive(true);
		}
		else
		{
			_OnNext(false);
		}
	}

	public void ExitTutorial()
	{
		if (m_pHudTutorial != null && m_pHudTutorialComp != null)
		{
			m_pHudTutorial.SetActive(false);
			m_pHudTutorialComp.Next = null;
		}
	}

	public void EnterTrackPresentation()
	{
		m_bDelayTrackPresentation = true;
	}

	public void ExitTrackPresentation()
	{
		m_pHudTrackPresentation.SetActive(false);
	}

	public void EnterFinishRace()
	{
		m_bNeedToEnterFinishRace = true;
		m_bFinishLineCrossed = true;
	}

	public void ExitFinishRace()
	{
		m_pHudFinish.SetActive(false);
	}

	public void StartRace()
	{
		bool flag = Singleton<GameConfigurator>.Instance.GameModeType != E_GameModeType.TIME_TRIAL && Singleton<GameConfigurator>.Instance.GameModeType != E_GameModeType.TUTORIAL;
		if (flag)
		{
			_hudRadarComp.StartRace();
		}
		if (Singleton<GameConfigurator>.Instance.GameModeType != E_GameModeType.TUTORIAL)
		{
			m_pHudPosition.SetActive(true);
		}
		m_pHudBonus.SetActive(flag);
		_hudRadar.SetActive(flag);
		if (Singleton<GameConfigurator>.Instance.GameModeType != E_GameModeType.TUTORIAL)
		{
			m_pHudCountdown.SetActive(true);
		}
		if (m_pHudControls.activeSelf)
		{
			m_pHudControlsComp.StartRace();
		}
	}

	public void ActivateHUDBonus(bool Active)
	{
		m_pHudBonus.SetActive(Active);
	}

	public void ShowEndTutorialHUD()
	{
		if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TUTORIAL && ((TutorialGameMode)Singleton<GameManager>.Instance.GameMode).State == ETutorialState.End)
		{
			_hudRadarComp.StartRace();
			_hudRadar.SetActive(true);
			m_pHudPosition.SetActive(true);
		}
	}

	public void Pause(bool _Pause)
	{
		if (Singleton<GameManager>.Instance.GameMode.State == E_GameState.Result)
		{
			if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.CHAMPIONSHIP)
			{
				if (!m_bEndChampionshipSecond)
				{
					_hudEndChampionShipRace.SetActive(!_Pause);
				}
				else
				{
					_hudEndChampionShipRank.SetActive(!_Pause);
				}
			}
			else if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.SINGLE)
			{
				_hudEndSingleRace.SetActive(!_Pause);
			}
			_nextButton.SetActive(!_Pause);
		}
		else if (Singleton<GameManager>.Instance.GameMode.State == E_GameState.TrackPresentation)
		{
			m_pHudTrackPresentation.SetActive(!_Pause);
		}
		else if (!FinishLineCrossed)
		{
			m_pHudBonus.SetActive(Singleton<GameConfigurator>.Instance.GameModeType != E_GameModeType.TIME_TRIAL && (Singleton<GameConfigurator>.Instance.GameModeType != E_GameModeType.TUTORIAL || ((TutorialGameMode)Singleton<GameManager>.Instance.GameMode).ShowHUDBonus) && !_Pause);
			_hudRadar.SetActive(Singleton<GameConfigurator>.Instance.GameModeType != E_GameModeType.TIME_TRIAL && (Singleton<GameConfigurator>.Instance.GameModeType != E_GameModeType.TUTORIAL || ((TutorialGameMode)Singleton<GameManager>.Instance.GameMode).Ended) && !_Pause);
			m_pHudPosition.SetActive((Singleton<GameConfigurator>.Instance.GameModeType != E_GameModeType.TUTORIAL || ((TutorialGameMode)Singleton<GameManager>.Instance.GameMode).Ended) && !_Pause);
			if (m_pHudControls.activeSelf)
			{
				m_pHudControlsComp.Pause(_Pause);
			}
			if (Singleton<GameManager>.Instance.GameMode.State == E_GameState.Start && Singleton<GameConfigurator>.Instance.GameModeType != E_GameModeType.TUTORIAL)
			{
				m_pHudCountdown.SetActive(!_Pause);
			}
			if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TUTORIAL)
			{
				TutorialGameMode tutorialGameMode = (TutorialGameMode)Singleton<GameManager>.Instance.GameMode;
				tutorialGameMode.Pause(_Pause);
			}
		}
		m_pHudFinish.SetActive(!_Pause);
	}

	public void EndRace()
	{
		m_bEndHUDDisplayed = true;
		_hudRadar.SetActive(false);
		switch (Singleton<GameConfigurator>.Instance.GameModeType)
		{
		case E_GameModeType.CHAMPIONSHIP:
			_hudEndChampionShipRace.SetActive(true);
			break;
		case E_GameModeType.SINGLE:
			_hudEndSingleRace.SetActive(true);
			break;
		case E_GameModeType.TIME_TRIAL:
			_hudEndTimeTrialRace1.SetActive(true);
			break;
		}
	}

	public void DoNext()
	{
		StartCoroutine(Next());
	}

	private IEnumerator Next()
	{
		if (m_pHudChallenge.activeSelf)
		{
			if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.CHAMPIONSHIP && Singleton<ChallengeManager>.Instance.Success)
			{
				Singleton<GameManager>.Instance.GameMode.State = E_GameState.Podium;
				m_pHudChallenge.SetActive(false);
				_nextButton.SetActive(false);
			}
			else
			{
				LoadingManager.LoadLevel("MenuRoot");
			}
		}
		else if (m_pHudFinish.activeSelf)
		{
			m_pHudFinishComp.Next();
		}
		else if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.CHAMPIONSHIP)
		{
			if (_hudEndChampionShipRank.activeSelf)
			{
				_hudEndChampionShipRank.SetActive(false);
				_nextButton.SetActive(false);
				yield return new WaitForSeconds(m_fDelayNext);
				if (Singleton<GameManager>.Instance.GameMode.State != E_GameState.Podium)
				{
					ShowEndOfRace();
				}
			}
			else if (_hudEndChampionShipRace.activeSelf)
			{
				m_bEndChampionshipSecond = true;
				_hudEndChampionShipRace.SetActive(false);
				yield return new WaitForSeconds(m_fDelayNext);
				if (Singleton<GameManager>.Instance.GameMode.State != E_GameState.Podium)
				{
					_hudEndChampionShipRank.SetActive(true);
					_hudEndChampionShipRank.GetComponent<HUDChampionsShipRanking>().FillPositions();
				}
			}
			else if (Network.isServer)
			{
				m_oNetworkMgr.networkView.RPC("NextRace", RPCMode.All);
			}
			else if (Network.peerType == NetworkPeerType.Disconnected)
			{
				ChampionShipGameMode.NextRace();
			}
			else if (Network.isClient)
			{
				m_pHudPauseComp.OnQuit();
			}
		}
		else if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TIME_TRIAL)
		{
			if (_hudTimeTrialResults.activeSelf)
			{
				_hudTimeTrialResults.SetActive(false);
				_nextButton.SetActive(false);
				yield return new WaitForSeconds(m_fDelayNext);
				ShowEndOfRace();
			}
			else if (_hudEndTimeTrialRace2.activeSelf)
			{
				_hudEndTimeTrialRace2.SetActive(false);
				yield return new WaitForSeconds(m_fDelayNext);
				_hudTimeTrialResults.SetActive(true);
			}
			else if (_hudEndTimeTrialRace1.activeSelf)
			{
				m_bEndChampionshipSecond = true;
				_hudEndTimeTrialRace1.SetActive(false);
				yield return new WaitForSeconds(m_fDelayNext);
				_hudEndTimeTrialRace2.SetActive(true);
			}
		}
		else if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.SINGLE)
		{
			_hudEndSingleRace.SetActive(false);
			_nextButton.SetActive(false);
			yield return new WaitForSeconds(m_fDelayNext);
			if (Singleton<GameManager>.Instance.GameMode.State != E_GameState.Podium)
			{
				ShowEndOfRace();
			}
		}
		yield return null;
	}

	public void ShowEndOfRace()
	{
		if (Singleton<ChallengeManager>.Instance.IsActive)
		{
			if (Singleton<ChallengeManager>.Instance.GameMode == E_GameModeType.CHAMPIONSHIP && Singleton<GameConfigurator>.Instance.CurrentTrackIndex < Singleton<GameConfigurator>.Instance.ChampionShipData.Tracks.Length && Singleton<ChallengeManager>.Instance.CurrentSuccess)
			{
				m_pHudPauseComp.ShowEndOfRace();
				return;
			}
			m_pHudChallenge.SetActive(true);
			_nextButton.SetActive(true);
		}
		else
		{
			m_pHudPauseComp.ShowEndOfRace();
		}
	}

	public void ShowFailedChallenge()
	{
		m_pHudChallenge.SetActive(true);
		_nextButton.SetActive(true);
		m_pHudPosition.SetActive(false);
		m_pHudBonus.SetActive(false);
		_hudRadar.SetActive(false);
		if (m_pHudControls.activeSelf)
		{
			m_pHudControlsComp.ShowExceptPause(false);
		}
	}

	public void Update()
	{
		if (m_bNeedToEnterFinishRace)
		{
			m_bNeedToEnterFinishRace = false;
			_hudRadar.SetActive(false);
			HideRaceHud();
			_nextButton.SetActive(true);
			m_pHudFinish.GetComponent<HUDFinish>().Show();
		}
		if (m_bDelayTrackPresentation)
		{
			m_pHudTrackPresentation.SetActive(true);
			m_bDelayTrackPresentation = false;
		}
		if (_deferNextDisplay)
		{
			_deferNextDisplayTimer -= Time.deltaTime;
			if (_deferNextDisplayTimer <= 0f)
			{
				_nextButton.SetActive(true);
				_deferNextDisplayTimer = 0f;
				_deferNextDisplay = false;
			}
		}
	}

	public void ShowHudPodium()
	{
		DeferedNextButtonDisplay(ChampResultNextButtonDelay);
		_hudChampionShipResult.SetActive(true);
	}

	private void DeferedNextButtonDisplay(float Duration)
	{
		_deferNextDisplay = true;
		_deferNextDisplayTimer = Duration;
	}

	public void HideRaceHud()
	{
		if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TIME_TRIAL)
		{
			m_pHudPositionComp.Lap.SetActive(false);
		}
		else
		{
			m_pHudPosition.SetActive(false);
		}
		m_pHudBonus.SetActive(false);
		if (m_pHudControls.activeSelf)
		{
			m_pHudControlsComp.ShowExceptPause(false);
		}
	}
}
