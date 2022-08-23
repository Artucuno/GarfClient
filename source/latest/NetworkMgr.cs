using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkMgr : MonoBehaviour
{
	public int port = 31069;

	public int maxPlayers = 6;

	public List<Color> SelectedColors;

	private int m_SynchronizeIndex;

	private int m_SynchronizeCounter;

	private bool m_WaitingSynchronization;

	private int networkID = -1;

	private int initialNbPlayers = 1;

	private NetworkViewID gameModeId;

	private Dictionary<NetworkPlayer, string> peerNames;

	private Dictionary<NetworkPlayer, bool> readyToGo;

	private Dictionary<Color, bool> m_SelectorColors;

	private Dictionary<NetworkPlayer, Color> playersColor;

	private bool m_bLanOnly;

	private string m_sPlayerName = "Player";

	private string m_sGameName = string.Empty;

	public string ServerName;

	private string m_sExternalIP;

	private bool waitForNamesSynchro = true;

	private bool waitForColorsSynchro = true;

	public float m_fTestingTimeout = 10f;

	private float m_fTimer;

	private float m_fTimerNAT;

	private bool m_bTestAsked = true;

	private bool m_bDoneTesting;

	private bool m_bProbingPublicIP;

	private bool m_bUseNat;

	private bool m_bCanConnect;

	private ConnectionTesterStatus m_eConnectionTestResult = ConnectionTesterStatus.Undetermined;

	private bool m_bTrackChoiceReceived;

	public ConnectionTesterStatus ConnectionStatus
	{
		get
		{
			return m_eConnectionTestResult;
		}
	}

	public bool TrackChoiceReceived
	{
		get
		{
			return m_bTrackChoiceReceived;
		}
	}

	public bool UsingNAT
	{
		get
		{
			return m_bUseNat;
		}
	}

	public bool DoneTesting
	{
		get
		{
			return m_bDoneTesting;
		}
	}

	public bool CanConnect
	{
		get
		{
			return m_bCanConnect;
		}
	}

	public string ExternalIP
	{
		get
		{
			return m_sExternalIP;
		}
	}

	public int SynchronizationCounter
	{
		get
		{
			return m_SynchronizeIndex;
		}
	}

	public Dictionary<NetworkPlayer, string> PeerNames
	{
		get
		{
			return peerNames;
		}
	}

	public Dictionary<NetworkPlayer, bool> ReadyToGo
	{
		get
		{
			return readyToGo;
		}
	}

	public Dictionary<Color, bool> SelectorColors
	{
		get
		{
			return m_SelectorColors;
		}
		set
		{
			m_SelectorColors = value;
		}
	}

	public Dictionary<NetworkPlayer, Color> PlayersColor
	{
		get
		{
			return playersColor;
		}
		set
		{
			playersColor = value;
		}
	}

	public bool BLanOnly
	{
		get
		{
			return m_bLanOnly;
		}
		set
		{
			m_bLanOnly = value;
		}
	}

	public string SPlayerName
	{
		get
		{
			return m_sPlayerName;
		}
		set
		{
			m_sPlayerName = value;
		}
	}

	public string SGameName
	{
		get
		{
			if (m_sGameName == string.Empty)
			{
				return Localization.instance.Get("MENU_GAME_NAME");
			}
			return m_sGameName;
		}
		set
		{
			m_sGameName = value;
		}
	}

	public NetworkViewID GameModeId
	{
		get
		{
			return gameModeId;
		}
	}

	public bool WaitingSynchronization
	{
		get
		{
			return m_WaitingSynchronization;
		}
	}

	public int InitialNbPlayers
	{
		get
		{
			return initialNbPlayers;
		}
	}

	public int NbPeers
	{
		get
		{
			if (Network.peerType == NetworkPeerType.Disconnected)
			{
				return 1;
			}
			return peerNames.Count;
		}
	}

	public NetworkMgr()
	{
		m_SynchronizeIndex = 0;
		m_SynchronizeCounter = 0;
		m_WaitingSynchronization = false;
		peerNames = new Dictionary<NetworkPlayer, string>();
		readyToGo = new Dictionary<NetworkPlayer, bool>();
		m_SelectorColors = new Dictionary<Color, bool>();
		playersColor = new Dictionary<NetworkPlayer, Color>();
	}

	public bool CanConnectTo(ConnectionTesterStatus serverNATType)
	{
		if (m_eConnectionTestResult == ConnectionTesterStatus.LimitedNATPunchthroughPortRestricted && serverNATType == ConnectionTesterStatus.LimitedNATPunchthroughSymmetric)
		{
			return false;
		}
		if (m_eConnectionTestResult == ConnectionTesterStatus.LimitedNATPunchthroughSymmetric && serverNATType == ConnectionTesterStatus.LimitedNATPunchthroughPortRestricted)
		{
			return false;
		}
		if (m_eConnectionTestResult == ConnectionTesterStatus.LimitedNATPunchthroughSymmetric && serverNATType == ConnectionTesterStatus.LimitedNATPunchthroughSymmetric)
		{
			return false;
		}
		return true;
	}

	public IEnumerator CheckIP()
	{
		WWW myExtIPWWW = new WWW("http://api.externalip.net/ip/");
		if (myExtIPWWW != null)
		{
			yield return myExtIPWWW;
			if (string.IsNullOrEmpty(myExtIPWWW.error))
			{
				m_sExternalIP = myExtIPWWW.text;
			}
		}
	}

	public void StartTestConnection()
	{
		m_bDoneTesting = false;
		m_bTestAsked = true;
		m_fTimer = 0f;
	}

	public void StopTestConnection()
	{
		m_bDoneTesting = true;
		m_bTestAsked = false;
	}

	private void TestConnection()
	{
		if (!m_bTestAsked || m_bDoneTesting)
		{
			return;
		}
		m_eConnectionTestResult = Network.TestConnection();
		switch (m_eConnectionTestResult)
		{
		case ConnectionTesterStatus.Error:
			m_bDoneTesting = true;
			break;
		case ConnectionTesterStatus.Undetermined:
			m_bDoneTesting = false;
			break;
		case ConnectionTesterStatus.PublicIPIsConnectable:
			m_bUseNat = false;
			m_bDoneTesting = true;
			m_bCanConnect = true;
			break;
		case ConnectionTesterStatus.PublicIPPortBlocked:
			m_bUseNat = false;
			if (!m_bProbingPublicIP)
			{
				m_eConnectionTestResult = Network.TestConnectionNAT();
				m_bProbingPublicIP = true;
				Debug.LogWarning("Testing if blocked public IP can be circumvented");
				m_fTimerNAT = Time.time + 10f;
			}
			else if (Time.time > m_fTimerNAT)
			{
				m_bProbingPublicIP = false;
				m_bUseNat = true;
				m_bDoneTesting = true;
			}
			break;
		case ConnectionTesterStatus.LimitedNATPunchthroughPortRestricted:
			m_bUseNat = true;
			m_bDoneTesting = true;
			break;
		case ConnectionTesterStatus.LimitedNATPunchthroughSymmetric:
			m_bUseNat = true;
			m_bDoneTesting = true;
			break;
		case ConnectionTesterStatus.NATpunchthroughFullCone:
		case ConnectionTesterStatus.NATpunchthroughAddressRestrictedCone:
			m_bUseNat = true;
			m_bDoneTesting = true;
			m_bCanConnect = true;
			break;
		default:
			m_bDoneTesting = true;
			break;
		case ConnectionTesterStatus.PublicIPNoServerStarted:
			break;
		}
		m_fTimer += Time.deltaTime;
		if (m_fTimer > m_fTestingTimeout)
		{
			m_bDoneTesting = true;
		}
	}

	public void OnDestroy()
	{
		Network.Disconnect();
	}

	public void Awake()
	{
		Application.runInBackground = true;
		m_SynchronizeIndex = 0;
		m_SynchronizeCounter = 0;
		m_WaitingSynchronization = false;
		MasterServer.ipAddress = "94.23.51.63";
		MasterServer.port = 23466;
		Network.natFacilitatorIP = MasterServer.ipAddress;
		Network.natFacilitatorPort = 50005;
		Object.DontDestroyOnLoad(this);
		selectorColorsInit();
	}

	public void selectorColorsInit()
	{
		m_SelectorColors.Clear();
		foreach (Color selectedColor in SelectedColors)
		{
			m_SelectorColors.Add(selectedColor, false);
		}
	}

	public void Update()
	{
		TestConnection();
		if (Network.isServer && m_SynchronizeCounter > Network.connections.Length && m_SynchronizeCounter > 0)
		{
			m_SynchronizeCounter = 0;
			if (Network.connections.Length > 0)
			{
				base.networkView.RPC("EndSynchronization", RPCMode.All, m_SynchronizeIndex + 1);
			}
			else
			{
				EndSynchronization(m_SynchronizeIndex + 1);
				Network.Disconnect();
			}
		}
		if (!waitForNamesSynchro)
		{
			waitForNamesSynchro = true;
			foreach (KeyValuePair<NetworkPlayer, string> peerName in peerNames)
			{
				base.networkView.RPC("SetPeerName", RPCMode.Others, peerName.Key, peerName.Value, false);
			}
		}
		if (waitForColorsSynchro)
		{
			return;
		}
		waitForColorsSynchro = true;
		foreach (KeyValuePair<NetworkPlayer, Color> item in playersColor)
		{
			Vector3 vector = new Vector3(item.Value.r, item.Value.g, item.Value.b);
			base.networkView.RPC("SetPlayersColor", RPCMode.Others, item.Key, vector, false);
		}
	}

	public int GetNetworkID()
	{
		return networkID;
	}

	public void OnPlayerDisconnected(NetworkPlayer player)
	{
		Network.RemoveRPCs(player, 0);
		if (Singleton<GameManager>.Instance.GameMode != null)
		{
			if (Singleton<GameManager>.Instance.GameMode.State != E_GameState.Podium)
			{
				GameObject player2 = Singleton<GameManager>.Instance.GameMode.GetPlayer(player);
				NetworkViewID viewID = player2.networkView.viewID;
				player2.networkView.viewID = Network.AllocateViewID();
				base.networkView.RPC("NotifyPlayerDisconnection", RPCMode.Others, viewID, player2.networkView.viewID);
				Kart componentInChildren = player2.GetComponentInChildren<Kart>();
				componentInChildren.SetControlType(RcVehicle.ControlType.AI);
				componentInChildren.SetAutoPilot(true);
				RaceScoreData scoreData = Singleton<GameConfigurator>.Instance.RankingManager.GetScoreData(componentInChildren.Index);
				if (scoreData != null)
				{
					scoreData.IsAI = true;
				}
				((InGameGameMode)Singleton<GameManager>.Instance.GameMode).PlayerDisconnected(componentInChildren);
				RcVirtualController componentInChildren2 = componentInChildren.gameObject.transform.parent.GetComponentInChildren<RcVirtualController>();
				componentInChildren2.SetDrivingEnabled(true);
				((InGameGameMode)Singleton<GameManager>.Instance.GameMode).AIManager.RegisterVirtualController(componentInChildren2, E_AILevel.AVERAGE);
				initialNbPlayers--;
			}
		}
		else
		{
			Network.DestroyPlayerObjects(player);
		}
		base.networkView.RPC("RemoveNetworkPlayer", RPCMode.All, player);
	}

	public void OnServerInitialized(NetworkPlayer player)
	{
		m_SynchronizeIndex = 0;
		m_SynchronizeCounter = 0;
		m_WaitingSynchronization = false;
		m_sPlayerName = Singleton<GameSaveManager>.Instance.GetPseudo();
		selectorColorsInit();
		playersColor.Clear();
		peerNames.Clear();
		readyToGo.Clear();
		Color color = selectFreeColor();
		peerNames.Add(Network.player, m_sPlayerName);
		readyToGo.Add(Network.player, false);
		m_SelectorColors[color] = true;
		playersColor.Add(Network.player, color);
	}

	public void OnPlayerConnected(NetworkPlayer player)
	{
		foreach (KeyValuePair<NetworkPlayer, bool> item in readyToGo)
		{
			base.networkView.RPC("SetReadyToGo", RPCMode.Others, item.Key, item.Value);
		}
		if (Network.isServer)
		{
			base.networkView.RPC("SetGameModeID", RPCMode.Others, gameModeId);
		}
	}

	public void DispatchIds()
	{
		if (Network.isServer)
		{
			networkID = 0;
			initialNbPlayers = 1;
			NetworkPlayer[] connections = Network.connections;
			initialNbPlayers += connections.Length;
			int num = networkID;
			NetworkPlayer[] array = connections;
			foreach (NetworkPlayer target in array)
			{
				base.networkView.RPC("SetNetworkID", target, ++num, initialNbPlayers);
			}
		}
	}

	public void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		if (!Network.isServer)
		{
			peerNames.Clear();
			readyToGo.Clear();
			if (!LoadingManager.SLevelToLoad.Equals("MenuRoot"))
			{
				Singleton<GameConfigurator>.Instance.MenuToLaunch = EMenus.MENU_WELCOME;
				LoadingManager.LoadLevel("MenuRoot");
			}
		}
	}

	public Color selectFreeColor()
	{
		Color white = Color.white;
		foreach (KeyValuePair<Color, bool> selectorColor in m_SelectorColors)
		{
			if (!selectorColor.Value)
			{
				return selectorColor.Key;
			}
		}
		return white;
	}

	public void OnConnectedToServer()
	{
		m_SynchronizeIndex = 0;
		m_SynchronizeCounter = 0;
		m_WaitingSynchronization = false;
		selectorColorsInit();
		playersColor.Clear();
		peerNames.Clear();
		readyToGo.Clear();
		m_sPlayerName = Singleton<GameSaveManager>.Instance.GetPseudo();
		base.networkView.RPC("SetPeerName", RPCMode.Server, Network.player, m_sPlayerName, true);
		base.networkView.RPC("SetReadyToGo", RPCMode.All, Network.player, false);
		base.networkView.RPC("SetPlayersColor", RPCMode.Server, Network.player, new Vector3(-1f, -1f, -1f), true);
	}

	public void OnFailedToConnect(NetworkConnectionError error)
	{
		peerNames.Clear();
		readyToGo.Clear();
	}

	[RPC]
	public void SetPeerName(NetworkPlayer player, string name, bool synchro)
	{
		if (peerNames.ContainsKey(player))
		{
			peerNames[player] = name;
		}
		else
		{
			peerNames.Add(player, name);
		}
		if (synchro)
		{
			waitForNamesSynchro = false;
		}
	}

	[RPC]
	public void SetReadyToGo(NetworkPlayer player, bool ready)
	{
		if (readyToGo.ContainsKey(player))
		{
			readyToGo[player] = ready;
		}
		else
		{
			readyToGo.Add(player, ready);
		}
	}

	public void ResetReadyStates()
	{
		List<NetworkPlayer> list = new List<NetworkPlayer>();
		foreach (NetworkPlayer key in readyToGo.Keys)
		{
			list.Add(key);
		}
		foreach (NetworkPlayer item in list)
		{
			readyToGo[item] = false;
		}
		list.Clear();
	}

	[RPC]
	public void NotifySelectingTrack()
	{
		GameObject.Find("MENU_REJOINDRE2").GetComponent<MenuMultiWaitingRoom>().NotifySelectingTrack();
	}

	[RPC]
	public void SetPlayersColor(NetworkPlayer player, Vector3 vColor, bool synchro)
	{
		Color color = ((!synchro) ? new Color(vColor.x, vColor.y, vColor.z, 1f) : selectFreeColor());
		m_SelectorColors[color] = true;
		if (playersColor.ContainsKey(player))
		{
			playersColor[player] = color;
		}
		else
		{
			playersColor.Add(player, color);
		}
		if (synchro)
		{
			waitForColorsSynchro = false;
		}
	}

	[RPC]
	public void AssignPlayerColor()
	{
		if (playersColor.ContainsKey(Network.player))
		{
			Singleton<GameConfigurator>.Instance.PlayerConfig.PlayerColor = playersColor[Network.player];
		}
		else
		{
			Singleton<GameConfigurator>.Instance.PlayerConfig.PlayerColor = Color.yellow;
		}
	}

	[RPC]
	public void SetNetID(NetworkViewID id)
	{
		base.networkView.viewID = id;
	}

	[RPC]
	public void NotifyPlayerDisconnection(NetworkViewID oldID, NetworkViewID newID)
	{
		GameObject player = Singleton<GameManager>.Instance.GameMode.GetPlayer(oldID);
		player.networkView.viewID = newID;
		Kart componentInChildren = player.GetComponentInChildren<Kart>();
		RaceScoreData scoreData = Singleton<GameConfigurator>.Instance.RankingManager.GetScoreData(componentInChildren.Index);
		if (scoreData != null)
		{
			scoreData.IsAI = true;
		}
	}

	[RPC]
	public void RemoveNetworkPlayer(NetworkPlayer player)
	{
		peerNames.Remove(player);
		readyToGo.Remove(player);
		if (playersColor.ContainsKey(player))
		{
			Color key = playersColor[player];
			playersColor.Remove(player);
			if (m_SelectorColors.ContainsKey(key))
			{
				m_SelectorColors[key] = false;
			}
		}
	}

	[RPC]
	public void ShareTrackChoice(int gameModeType, int championshipIndex, int trackIndex, int difficulty)
	{
		Singleton<GameConfigurator>.Instance.CurrentTrackIndex = trackIndex;
		Singleton<GameConfigurator>.Instance.GameModeType = (E_GameModeType)gameModeType;
		Singleton<GameConfigurator>.Instance.Difficulty = (EDifficulty)difficulty;
		Singleton<GameConfigurator>.Instance.SetChampionshipData(false, championshipIndex);
		m_bTrackChoiceReceived = true;
	}

	[RPC]
	public void Go(string sceneName)
	{
		m_bTrackChoiceReceived = false;
		Singleton<GameConfigurator>.Instance.StartScene = sceneName;
		LoadingManager.LoadLevel(sceneName);
	}

	[RPC]
	public void SetGameModeID(NetworkViewID id)
	{
		gameModeId = id;
		if ((bool)Singleton<GameManager>.Instance.NetworkView)
		{
			Singleton<GameManager>.Instance.NetworkView.viewID = gameModeId;
		}
	}

	[RPC]
	public void NextRace()
	{
		ChampionShipGameMode.NextRace();
	}

	[RPC]
	public void ShowPodium()
	{
		Singleton<GameManager>.Instance.GameMode.State = E_GameState.Podium;
		HUDInGame hUDInGame = Object.FindObjectOfType(typeof(HUDInGame)) as HUDInGame;
		HUDPause hUDPause = Object.FindObjectOfType(typeof(HUDPause)) as HUDPause;
		if (Network.isServer)
		{
			hUDPause.PanelPauseChampionship.SetActive(false);
		}
		else if (Network.isClient)
		{
			hUDPause.ShowEndOfRace();
			hUDInGame.HudEndChampionshipRace.SetActive(false);
			hUDInGame.HudEndChampionshipRank.SetActive(false);
			hUDPause.PanelPauseChampionship.SetActive(false);
		}
	}

	[RPC]
	public IEnumerator SetMenu(int menuId)
	{
		MenuEntryPoint pMenuEntryPoint = GameObject.Find("MenuEntryPoint").GetComponent<MenuEntryPoint>();
		yield return new WaitForFixedUpdate();
		pMenuEntryPoint.SetState((EMenus)menuId);
	}

	[RPC]
	public void SetNetworkID(int id, int nbPlayers)
	{
		networkID = id;
		initialNbPlayers = nbPlayers;
	}

	[RPC]
	public void Synchronize(int networkID, int synchronizeIndex)
	{
		if (Network.isServer && synchronizeIndex == m_SynchronizeIndex)
		{
			m_SynchronizeCounter++;
		}
	}

	[RPC]
	public void EndSynchronization(int synchronizeIndex)
	{
		if (synchronizeIndex != m_SynchronizeIndex + 1)
		{
			Debug.LogError("[NETWORK] received wrong synchronization ended index");
			return;
		}
		m_SynchronizeIndex = synchronizeIndex;
		m_WaitingSynchronization = false;
	}

	public void StartSynchronization()
	{
		if (!m_WaitingSynchronization)
		{
			if (Network.isServer)
			{
				m_SynchronizeCounter++;
				m_WaitingSynchronization = true;
			}
			else if (Network.isClient)
			{
				m_WaitingSynchronization = true;
				base.networkView.RPC("Synchronize", RPCMode.Server, networkID, m_SynchronizeIndex);
			}
		}
	}

	[RPC]
	public void QuitToMenu(int menu)
	{
		Singleton<GameConfigurator>.Instance.MenuToLaunch = (EMenus)menu;
		if ((bool)Singleton<GameConfigurator>.Instance.PlayerConfig)
		{
			Singleton<GameConfigurator>.Instance.PlayerConfig.ResetAdvantages();
		}
		LoadingManager.LoadLevel("MenuRoot");
	}

	[RPC]
	public void RetryRace()
	{
		Singleton<GameConfigurator>.Instance.RankingManager.RestartRace();
		if ((bool)Singleton<GameConfigurator>.Instance.PlayerConfig)
		{
			Singleton<GameConfigurator>.Instance.PlayerConfig.ResetAdvantages();
		}
		LoadingManager.LoadLevel(Application.loadedLevelName);
	}

	[RPC]
	public void SetLevelIndex(int iLevelIndex)
	{
		LoadingManager.LevelIndex = iLevelIndex;
	}
}
