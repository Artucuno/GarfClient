using System.Collections.Generic;
using UnityEngine;

public class MenuMultiWaitingRoom : AbstractMenu
{
	private EMenus m_eFromMenu;

	private Dictionary<int, string> m_oPlayerList = new Dictionary<int, string>();

	private Dictionary<int, Color> m_oPlayerColor = new Dictionary<int, Color>();

	public UILabel[] m_oPlayerLabel = new UILabel[6];

	public UITexturePattern m_oGameTypeIcon;

	public UILabel m_oNbPlayersLabel;

	public GameObject m_oNextButton;

	private NetworkMgr networkMgr;

	public UILabel ServerName;

	public AudioSource PlayerConnectedSound;

	public AudioSource PlayerDisconnectedSound;

	private int m_iLastNbPeers;

	public UITexturePattern IconType;

	public List<UISlicedSprite> PlayerBackground;

	public GameObject SelectingTrackLabel;

	private bool m_bRefreshPanel;

	private bool m_bInitCalled;

	public override void Awake()
	{
		base.Awake();
		networkMgr = (NetworkMgr)Object.FindObjectOfType(typeof(NetworkMgr));
		m_bRefreshPanel = false;
	}

	public void Init(EMenus eFromMenu, int iSessionId, string sServerName, int iGameType)
	{
		m_bInitCalled = true;
		networkMgr.ServerName = sServerName;
		m_oPlayerList.Clear();
		m_oPlayerColor.Clear();
		m_eFromMenu = eFromMenu;
		if ((bool)m_oGameTypeIcon)
		{
			m_oGameTypeIcon.ChangeTexture(iGameType);
		}
		if ((bool)m_oNextButton)
		{
			m_oNextButton.SetActive(false);
		}
		if ((bool)ServerName)
		{
			ServerName.text = sServerName;
		}
		int num = 0;
		foreach (KeyValuePair<NetworkPlayer, Color> item in networkMgr.PlayersColor)
		{
			m_oPlayerColor.Add(num, item.Value);
			num++;
		}
		num = 0;
		foreach (KeyValuePair<NetworkPlayer, string> peerName in networkMgr.PeerNames)
		{
			AddPlayer(num, peerName.Value);
			num++;
		}
		m_iLastNbPeers = num;
		if (Network.isServer)
		{
			IconType.ChangeTexture(1);
		}
		else
		{
			IconType.ChangeTexture(0);
		}
	}

	public override void Update()
	{
		if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape))
		{
			OnBackButton();
		}
		if (networkMgr.PeerNames.Count != m_oPlayerList.Count)
		{
			UpdatePlayers();
		}
	}

	public void AddPlayer(int iPlayerId, string sPlayerName)
	{
		m_oPlayerList.Add(iPlayerId, sPlayerName);
		UpdatePlayers();
	}

	public void RemovePlayer(int iPlayerId)
	{
		m_oPlayerList.Remove(iPlayerId);
		UpdatePlayers();
	}

	public void OnPlayerConnected(NetworkPlayer player)
	{
		PlayerConnectedSound.Play();
	}

	public void OnPlayerDisconnected(NetworkPlayer player)
	{
		PlayerDisconnectedSound.Play();
	}

	private void UpdatePlayers()
	{
		if (networkMgr.PlayersColor.Count != networkMgr.PeerNames.Count)
		{
			return;
		}
		int i = 0;
		m_oPlayerList.Clear();
		m_oPlayerColor.Clear();
		int num = 0;
		foreach (KeyValuePair<NetworkPlayer, Color> item in networkMgr.PlayersColor)
		{
			m_oPlayerColor.Add(num, item.Value);
			num++;
		}
		num = 0;
		foreach (KeyValuePair<NetworkPlayer, string> peerName in networkMgr.PeerNames)
		{
			m_oPlayerList.Add(num, peerName.Value);
			num++;
		}
		if (m_iLastNbPeers != num)
		{
			m_iLastNbPeers = num;
			if (m_iLastNbPeers < 2)
			{
				m_oNextButton.SetActive(false);
			}
			else
			{
				m_oNextButton.SetActive(Network.isServer);
			}
		}
		foreach (KeyValuePair<int, string> oPlayer in m_oPlayerList)
		{
			if ((bool)m_oPlayerLabel[i])
			{
				m_oPlayerLabel[i].text = oPlayer.Value;
			}
			if ((bool)PlayerBackground[i])
			{
				PlayerBackground[i].color = m_oPlayerColor[i];
				PlayerBackground[i].gameObject.SetActive(true);
			}
			i++;
		}
		for (; i < 6; i++)
		{
			if ((bool)m_oPlayerLabel[i])
			{
				m_oPlayerLabel[i].text = string.Empty;
			}
			if ((bool)PlayerBackground[i])
			{
				PlayerBackground[i].gameObject.SetActive(false);
			}
		}
		if ((bool)m_oNbPlayersLabel)
		{
			m_oNbPlayersLabel.text = string.Format("{0} / 6 ", networkMgr.NbPeers) + Localization.instance.Get("MENU_PLAYERS");
		}
	}

	public override void OnEnter()
	{
		base.OnEnter();
		if (SelectingTrackLabel != null)
		{
			SelectingTrackLabel.SetActive(false);
		}
		for (int i = 1; i < PlayerBackground.Count; i++)
		{
			PlayerBackground[i].gameObject.SetActive(false);
		}
		PlayerBackground[0].color = Singleton<GameConfigurator>.Instance.PlayerConfig.PlayerColor;
		PlayerBackground[0].gameObject.SetActive(true);
		if (!m_bInitCalled && !Network.isServer)
		{
			ServerName.text = networkMgr.ServerName;
			NotifySelectingTrack();
		}
	}

	public override void OnExit()
	{
		base.OnExit();
		m_oPlayerList.Clear();
		m_bInitCalled = false;
	}

	public void OnBackButton()
	{
		ActSwapMenu(m_eFromMenu);
		Network.Disconnect();
	}

	public void OnNextButton()
	{
		networkMgr.networkView.RPC("AssignPlayerColor", RPCMode.All);
		ActSwapMenu(EMenus.MENU_CHAMPIONSHIP);
		if (Network.isServer)
		{
			Network.maxConnections = 0;
			for (int i = networkMgr.maxPlayers; i < Network.connections.Length; i++)
			{
				Network.CloseConnection(Network.connections[i], true);
			}
		}
		networkMgr.networkView.RPC("NotifySelectingTrack", RPCMode.Others);
		MasterServer.RegisterHost("GK12", "GK12", "Championship," + ((!networkMgr.BLanOnly) ? "WAN," : "LAN,") + networkMgr.SGameName + "," + Network.player.externalIP + ",startGame");
	}

	public void NotifySelectingTrack()
	{
		if (SelectingTrackLabel != null)
		{
			SelectingTrackLabel.SetActive(true);
		}
	}

	public void OnDisconnectedFromServer()
	{
		if (Network.isClient)
		{
			OnBackButton();
		}
	}

	public void OnFailedToConnectToMasterServer(NetworkConnectionError Error)
	{
		ActSwapMenu(EMenus.MENU_SOLO);
		PopupDialog popupDialog = (PopupDialog)m_pMenuEntryPoint.ShowPopup(EPopUps.POPUP_DIALOG, false);
		if ((bool)popupDialog)
		{
			popupDialog.Show("MENU_POPUP_MASTER_CONNECTION_ERROR");
		}
		Network.Disconnect();
	}
}
