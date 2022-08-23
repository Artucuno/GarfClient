using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	private GameMode m_pGameMode;

	private GameObject m_pGameModeObject;

	public static readonly E_GameModeType FirstRealGameMode = E_GameModeType.SINGLE;

	public static readonly E_GameModeType LastRealGameMode = E_GameModeType.TIME_TRIAL;

	private SoundManager m_pSoundManager;

	private NetworkView m_pNetworkView;

	public GameMode GameMode
	{
		get
		{
			return m_pGameMode;
		}
		set
		{
			m_pGameMode = value;
		}
	}

	public SoundManager SoundManager
	{
		get
		{
			return m_pSoundManager;
		}
		set
		{
			m_pSoundManager = value;
		}
	}

	public NetworkView NetworkView
	{
		get
		{
			return m_pNetworkView;
		}
	}

	public GameManager()
	{
		m_pGameMode = null;
		m_pGameModeObject = new GameObject("Game Mode");
		m_pNetworkView = (NetworkView)m_pGameModeObject.AddComponent(typeof(NetworkView));
		if ((bool)m_pNetworkView)
		{
			m_pNetworkView.stateSynchronization = NetworkStateSynchronization.Off;
			m_pNetworkView.observed = null;
		}
		Object.DontDestroyOnLoad(m_pGameModeObject);
	}

	public void Reset()
	{
		if (m_pGameMode != null)
		{
			m_pGameMode.Dispose();
			Object.Destroy(m_pGameMode);
			m_pGameMode = null;
		}
	}

	public void Init()
	{
	}

	public void LaunchGame()
	{
		if (DebugMgr.Instance != null && Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.DEBUG)
		{
			Singleton<GameConfigurator>.Instance.GameModeType = DebugMgr.Instance.dbgData.GameMode;
		}
		switch (Singleton<GameConfigurator>.Instance.GameModeType)
		{
		case E_GameModeType.DEBUG:
			m_pGameMode = (GameMode)m_pGameModeObject.AddComponent(typeof(DebugGameMode));
			break;
		case E_GameModeType.DEBUG_AI:
			m_pGameMode = (GameMode)m_pGameModeObject.AddComponent(typeof(DebugAiGameMode));
			break;
		case E_GameModeType.SINGLE:
			m_pGameMode = (GameMode)m_pGameModeObject.AddComponent(typeof(SingleRaceGameMode));
			break;
		case E_GameModeType.CHAMPIONSHIP:
			m_pGameMode = (GameMode)m_pGameModeObject.AddComponent(typeof(ChampionShipGameMode));
			break;
		case E_GameModeType.TIME_TRIAL:
			m_pGameMode = (GameMode)m_pGameModeObject.AddComponent(typeof(TimeTrialGameMode));
			break;
		case E_GameModeType.TUTORIAL:
			m_pGameMode = (GameMode)m_pGameModeObject.AddComponent(typeof(TutorialGameMode));
			break;
		}
		NetworkMgr networkMgr = (NetworkMgr)Object.FindObjectOfType(typeof(NetworkMgr));
		if ((bool)networkMgr)
		{
			m_pGameModeObject.networkView.viewID = networkMgr.GameModeId;
		}
	}

	public void CreatePlayers()
	{
		m_pGameMode.CreatePlayers();
	}
}
