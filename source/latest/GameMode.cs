using System;
using UnityEngine;

public abstract class GameMode : MonoBehaviour
{
	protected Tuple<GameObject, Kart>[] m_pPlayers;

	protected HUDInGame _hud;

	protected GameState[] _gameStates;

	protected GameState _currentGameState;

	protected E_GameState _state;

	protected bool m_bReadyToStart;

	private AudioSource m_pMainMusic;

	public E_GameState State
	{
		get
		{
			return _state;
		}
		set
		{
			_state = value;
			if (_currentGameState != null)
			{
				_currentGameState.Exit();
				_currentGameState.enabled = false;
			}
			_currentGameState = _gameStates[(int)_state];
			_currentGameState.enabled = true;
			_currentGameState.Enter();
		}
	}

	public AudioSource MainMusic
	{
		get
		{
			return m_pMainMusic;
		}
	}

	public bool ReadyToStart
	{
		get
		{
			return m_bReadyToStart;
		}
	}

	public int PlayerCount
	{
		get
		{
			return m_pPlayers.Length;
		}
	}

	public HUDInGame Hud
	{
		get
		{
			return _hud;
		}
	}

	public GameMode()
	{
	}

	protected virtual void StateChange(E_GameState pNewState)
	{
		State = pNewState;
	}

	public virtual void Dispose()
	{
		for (int i = 0; i < m_pPlayers.Length; i++)
		{
			if (m_pPlayers[i].Item1 != null)
			{
				UnityEngine.Object.Destroy(m_pPlayers[i].Item1);
			}
		}
		if (_hud != null)
		{
			UnityEngine.Object.Destroy(_hud.gameObject);
		}
		for (int j = 0; j < _gameStates.Length; j++)
		{
			if (_gameStates[j] != null)
			{
				UnityEngine.Object.Destroy(_gameStates[j]);
			}
		}
	}

	public virtual void Awake()
	{
		m_bReadyToStart = false;
		_gameStates = new GameState[Enum.GetValues(typeof(E_GameState)).Length];
		m_pPlayers = new Tuple<GameObject, Kart>[6];
		for (int i = 0; i < 6; i++)
		{
			m_pPlayers[i] = new Tuple<GameObject, Kart>(null, null);
		}
		GameObject gameObject = GameObject.Find(Singleton<GameConfigurator>.Instance.StartScene + "LD");
		if ((bool)gameObject)
		{
			m_pMainMusic = gameObject.GetComponent<AudioSource>();
			if ((bool)m_pMainMusic)
			{
				m_pMainMusic.ignoreListenerPause = true;
			}
		}
		InitState();
		Singleton<RewardManager>.Instance.RaceCoins = 0;
	}

	public abstract void StartScene();

	public abstract void CreatePlayers();

	public virtual void InitState()
	{
		_state = E_GameState.TrackPresentation;
	}

	public void CreatePlayer(ECharacter _Character, ECharacter _Kart, string customName, string hatName, int iNbStars, int pIndex, bool pLock, bool pIsAI)
	{
		GameObject gameObject = null;
		UnityEngine.Object @object = Resources.Load("Player");
		gameObject = ((Network.peerType != 0) ? ((GameObject)Network.Instantiate(@object, Vector3.zero, Quaternion.identity, 0)) : ((GameObject)UnityEngine.Object.Instantiate(@object, Vector3.zero, Quaternion.identity)));
		PlayerBuilder componentInChildren = gameObject.GetComponentInChildren<PlayerBuilder>();
		Vector3 vector = new Vector3(Singleton<GameConfigurator>.Instance.PlayerConfig.PlayerColor.r, Singleton<GameConfigurator>.Instance.PlayerConfig.PlayerColor.g, Singleton<GameConfigurator>.Instance.PlayerConfig.PlayerColor.b);
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			componentInChildren.Build((int)_Character, (int)_Kart, customName, hatName, iNbStars, pIndex, pLock, pIsAI, string.Empty, vector);
		}
		else
		{
			componentInChildren.networkView.RPC("Build", RPCMode.All, (int)_Character, (int)_Kart, customName, hatName, iNbStars, pIndex, pLock, pIsAI, Singleton<GameSaveManager>.Instance.GetPseudo(), vector);
		}
		gameObject.GetComponentInChildren<Kart>().Disable();
		KartSound componentInChildren2 = gameObject.GetComponentInChildren<KartSound>();
		if ((bool)componentInChildren2)
		{
			componentInChildren2.Character = _Character;
		}
	}

	public GameObject GetPlayer(int _Index)
	{
		if (_Index < 0 || _Index >= m_pPlayers.Length)
		{
			return null;
		}
		return m_pPlayers[_Index].Item1;
	}

	public Kart GetKart(int _Index)
	{
		if (_Index < 0 || _Index >= m_pPlayers.Length)
		{
			return null;
		}
		return m_pPlayers[_Index].Item2;
	}

	public GameObject GetPlayerWithVehicleId(int _Index)
	{
		if (_Index < 0 || _Index >= m_pPlayers.Length)
		{
			return null;
		}
		for (int i = 0; i < m_pPlayers.Length; i++)
		{
			if (m_pPlayers[i].Item2 != null && m_pPlayers[i].Item2.GetVehicleId() == _Index)
			{
				return m_pPlayers[i].Item1;
			}
		}
		return null;
	}

	public Kart GetKartWithVehicleId(int _Index)
	{
		if (_Index < 0 || _Index >= m_pPlayers.Length)
		{
			return null;
		}
		for (int i = 0; i < m_pPlayers.Length; i++)
		{
			if (m_pPlayers[i].Item2 != null && m_pPlayers[i].Item2.GetVehicleId() == _Index)
			{
				return m_pPlayers[i].Item2;
			}
		}
		return null;
	}

	public GameObject GetHumanPlayer(ref int _iHumanIndex)
	{
		for (int i = 0; i < m_pPlayers.Length; i++)
		{
			if (m_pPlayers[i] != null && m_pPlayers[i].Item2 != null && m_pPlayers[i].Item2.GetControlType() == RcVehicle.ControlType.Human)
			{
				_iHumanIndex = i;
				return m_pPlayers[i].Item1;
			}
		}
		return null;
	}

	public Kart GetHumanKart(ref int _iHumanIndex)
	{
		for (int i = 0; i < m_pPlayers.Length; i++)
		{
			if (m_pPlayers[i] != null && m_pPlayers[i].Item2 != null && m_pPlayers[i].Item2.GetControlType() == RcVehicle.ControlType.Human)
			{
				_iHumanIndex = i;
				return m_pPlayers[i].Item2;
			}
		}
		return null;
	}

	public GameObject GetPlayer(ECharacter _Character, bool bOnlyAI)
	{
		for (int i = 0; i < m_pPlayers.Length; i++)
		{
			if (m_pPlayers[i] == null)
			{
				continue;
			}
			bool flag = false;
			if (m_pPlayers[i].Item2 != null && m_pPlayers[i].Item1 != null)
			{
				flag = m_pPlayers[i].Item2.GetControlType() != RcVehicle.ControlType.Human;
				CharacterCarac componentInChildren = m_pPlayers[i].Item1.GetComponentInChildren<CharacterCarac>();
				if (((bOnlyAI && flag) || !bOnlyAI) && (bool)componentInChildren && componentInChildren.Owner == _Character)
				{
					return m_pPlayers[i].Item1;
				}
			}
		}
		return null;
	}

	public GameObject GetPlayer(NetworkPlayer nplayer)
	{
		for (int i = 0; i < m_pPlayers.Length; i++)
		{
			if (m_pPlayers[i] != null && m_pPlayers[i].Item1 != null && m_pPlayers[i].Item1.networkView.owner == nplayer)
			{
				return m_pPlayers[i].Item1;
			}
		}
		return null;
	}

	public GameObject GetPlayer(NetworkViewID viewID)
	{
		for (int i = 0; i < m_pPlayers.Length; i++)
		{
			if (m_pPlayers[i] != null && m_pPlayers[i].Item1 != null && m_pPlayers[i].Item1.networkView.viewID == viewID)
			{
				return m_pPlayers[i].Item1;
			}
		}
		return null;
	}

	public Kart GetKart(ECharacter _Character, bool bOnlyAI)
	{
		for (int i = 0; i < m_pPlayers.Length; i++)
		{
			if (m_pPlayers[i] == null)
			{
				continue;
			}
			bool flag = false;
			if (m_pPlayers[i].Item2 != null && m_pPlayers[i].Item1 != null)
			{
				flag = m_pPlayers[i].Item2.GetControlType() != RcVehicle.ControlType.Human;
				CharacterCarac componentInChildren = m_pPlayers[i].Item1.GetComponentInChildren<CharacterCarac>();
				if (((bOnlyAI && flag) || !bOnlyAI) && (bool)componentInChildren && componentInChildren.Owner == _Character)
				{
					return m_pPlayers[i].Item2;
				}
			}
		}
		return null;
	}

	public GameObject GetHumanPlayer()
	{
		int _iHumanIndex = 0;
		return GetHumanPlayer(ref _iHumanIndex);
	}

	public Kart GetHumanKart()
	{
		int _iHumanIndex = 0;
		return GetHumanKart(ref _iHumanIndex);
	}

	public int GetHumanPlayerVehicleId()
	{
		int _iHumanIndex = 0;
		Kart humanKart = GetHumanKart(ref _iHumanIndex);
		if ((bool)humanKart)
		{
			return humanKart.GetVehicleId();
		}
		return -1;
	}

	public void AddPlayer(GameObject _Player, Kart _Kart)
	{
		for (int i = 0; i < 6; i++)
		{
			if (m_pPlayers[i] != null && m_pPlayers[i].Item1 == null)
			{
				m_pPlayers[i].Item1 = _Player;
				m_pPlayers[i].Item2 = _Kart;
				break;
			}
		}
	}

	public void ComputePlayerAdvantages()
	{
		for (int i = 0; i < m_pPlayers.Length; i++)
		{
			if (m_pPlayers[i].Item1 != null && m_pPlayers[i].Item2 != null)
			{
				KartArcadeGearBox kartArcadeGearBox = (KartArcadeGearBox)m_pPlayers[i].Item2.GetGearBox();
				if ((bool)kartArcadeGearBox)
				{
					kartArcadeGearBox.StartScene();
				}
				PlayerCarac componentInChildren = m_pPlayers[i].Item1.GetComponentInChildren<PlayerCarac>();
				if ((bool)componentInChildren)
				{
					componentInChildren.StartScene();
				}
			}
		}
	}

	public void FailedChallenge()
	{
		if (_state == E_GameState.Race)
		{
			_hud.ShowFailedChallenge();
			RcHumanController componentInChildren = GetHumanPlayer().GetComponentInChildren<RcHumanController>();
			if ((bool)componentInChildren)
			{
				componentInChildren.SetAutopilotEnabled(true);
			}
		}
	}

	public void SetAdvantage(int iIndex, EAdvantage eAdvantage)
	{
		if (iIndex >= 0 && iIndex < m_pPlayers.Length)
		{
			if (Network.peerType == NetworkPeerType.Disconnected)
			{
				DoSelectAdvantage(iIndex, (int)eAdvantage);
				return;
			}
			base.networkView.RPC("DoSelectAdvantage", RPCMode.All, iIndex, (int)eAdvantage);
		}
	}

	[RPC]
	public void DoSelectAdvantage(int iIndex, int eAdvantage)
	{
		GameObject playerWithVehicleId = GetPlayerWithVehicleId(iIndex);
		if ((bool)playerWithVehicleId)
		{
			Kart componentInChildren = playerWithVehicleId.GetComponentInChildren<Kart>();
			if ((bool)componentInChildren)
			{
				componentInChildren.SelectedAdvantage = (EAdvantage)eAdvantage;
			}
		}
	}
}
