using UnityEngine;

public class GameConfigurator : Singleton<GameConfigurator>
{
	public const int MAX_NB_PLAYERS = 6;

	public const int MAX_NB_TRACKS_PER_CHAMPIONSHIP = 4;

	private EDifficulty m_eDifficulty;

	private PlayerConfig m_pPlayerConfig;

	private string _startScene = "MenuRoot";

	private AISettings _aiSettings;

	private GameSettings _gameSettings;

	private E_GameModeType _gameModeType;

	private ChampionShipData _championShipData;

	private int _currentTrackIndex;

	private int m_iNbSlots;

	private PriceConfig m_pPriceConfig;

	private ChampionshipPass m_pChampionshipPass;

	private EMenus m_eMenuToLaunch;

	public RankingManager RankingManager;

	public bool m_bMobilePlatform;

	public PlayerData[] PlayerDataList;

	public string ChartBoostAppID = "YOUR_APP_ID";

	public string ChartBoostAppSignature = "YOUR_APP_SIGNATURE";

	public string FacebookAppID = "YOUR_FACEBOOK_ID";

	public GameSettings GameSettings
	{
		get
		{
			return _gameSettings;
		}
		set
		{
			_gameSettings = value;
		}
	}

	public string StartScene
	{
		get
		{
			return _startScene;
		}
		set
		{
			_startScene = value;
		}
	}

	public PlayerConfig PlayerConfig
	{
		get
		{
			return m_pPlayerConfig;
		}
	}

	public EDifficulty Difficulty
	{
		get
		{
			return m_eDifficulty;
		}
		set
		{
			m_eDifficulty = value;
		}
	}

	public AISettings AISettings
	{
		get
		{
			return _aiSettings;
		}
		set
		{
			_aiSettings = value;
		}
	}

	public ChampionShipData ChampionShipData
	{
		get
		{
			return _championShipData;
		}
	}

	public E_GameModeType GameModeType
	{
		get
		{
			return _gameModeType;
		}
		set
		{
			_gameModeType = value;
		}
	}

	public int CurrentTrackIndex
	{
		get
		{
			return _currentTrackIndex;
		}
		set
		{
			_currentTrackIndex = value;
		}
	}

	public int NbSlots
	{
		get
		{
			return m_iNbSlots;
		}
		set
		{
			m_iNbSlots = value;
		}
	}

	public PriceConfig PriceConfig
	{
		get
		{
			return m_pPriceConfig;
		}
	}

	public ChampionshipPass ChampionshipPass
	{
		get
		{
			return m_pChampionshipPass;
		}
		set
		{
			m_pChampionshipPass = value;
		}
	}

	public EMenus MenuToLaunch
	{
		get
		{
			return m_eMenuToLaunch;
		}
		set
		{
			m_eMenuToLaunch = value;
		}
	}

	public void Init()
	{
		m_eDifficulty = EDifficulty.NORMAL;
		m_pChampionshipPass = null;
		m_pPlayerConfig = GameObject.Find("EntryPoint").GetComponent<PlayerConfig>();
		m_pPriceConfig = GameObject.Find("EntryPoint").GetComponent<PriceConfig>();
		m_eMenuToLaunch = EMenus.MENU_WELCOME;
		RankingManager = new RankingManager();
	}

	public void ResetChampionShip()
	{
		if (_gameModeType == E_GameModeType.CHAMPIONSHIP)
		{
			_currentTrackIndex = 0;
		}
		RankingManager.Reset();
		if (PlayerDataList != null)
		{
			PlayerDataList = null;
		}
	}

	public void SetChampionshipData(bool _WithPass, int index)
	{
		Object[] array = Resources.LoadAll("ChampionShip", typeof(ChampionShipData));
		Object[] array2 = array;
		foreach (Object @object in array2)
		{
			if (@object is ChampionShipData)
			{
				ChampionShipData championShipData = (ChampionShipData)@object;
				if (championShipData.Index == index)
				{
					SetChampionshipData(championShipData, _WithPass);
					break;
				}
			}
		}
	}

	public void SetChampionshipData(ChampionShipData _Data, bool _WithPass)
	{
		_championShipData = _Data;
		_championShipData.Localize();
		if (_WithPass)
		{
			m_pChampionshipPass = new ChampionshipPass();
			m_pChampionshipPass.State = EChampionshipPassState.Selected;
			m_pChampionshipPass.ChampionshipSelectionned = _Data;
			m_pChampionshipPass.Difficulty = m_eDifficulty;
		}
	}

	public void RestartChampionShipRace()
	{
		RankingManager.RestartRace();
	}

	public RaceScoreData GetScoreData(int iKartIndex)
	{
		return RankingManager.GetScoreData(iKartIndex);
	}
}
