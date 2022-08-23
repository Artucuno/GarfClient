using UnityEngine;

public class MenuMultiCreate : AbstractMenu
{
	private NetworkMgr networkMgr;

	public UIInput Input;

	public UILabel CreatingLabel;

	private bool m_bNeedToCreateSolo;

	private bool m_bNeedToCreateChampionship;

	public override void Awake()
	{
		base.Awake();
		networkMgr = (NetworkMgr)Object.FindObjectOfType(typeof(NetworkMgr));
	}

	public override void OnEnter()
	{
		base.OnEnter();
		Input.text = networkMgr.SGameName;
		m_bNeedToCreateSolo = false;
		m_bNeedToCreateChampionship = false;
		if (CreatingLabel != null)
		{
			CreatingLabel.enabled = false;
		}
		Input.defaultText = Localization.instance.Get("MENU_GAME_NAME");
	}

	public override void OnExit()
	{
		OnSubmit();
		base.OnExit();
	}

	private bool CreateSession()
	{
		Network.Disconnect();
		Network.maxConnections = networkMgr.maxPlayers;
		if (Network.InitializeServer(networkMgr.maxPlayers, networkMgr.port, !Network.HavePublicAddress()) != 0)
		{
			PopupDialog popupDialog = (PopupDialog)m_pMenuEntryPoint.ShowPopup(EPopUps.POPUP_DIALOG, false);
			if ((bool)popupDialog)
			{
				popupDialog.Show("MENU_POPUP_ERROR_INIT_SERVER");
			}
			Network.Disconnect();
			return false;
		}
		NetworkViewID networkViewID = Network.AllocateViewID();
		networkMgr.networkView.RPC("SetGameModeID", RPCMode.All, networkViewID);
		return true;
	}

	public void OnButtonSingle()
	{
		OnSubmit();
		if (CreateSession())
		{
			if (CreatingLabel != null)
			{
				CreatingLabel.enabled = true;
			}
			m_bNeedToCreateSolo = true;
		}
	}

	public void OnButtonChampionship()
	{
		OnSubmit();
		if (CreateSession())
		{
			if (CreatingLabel != null)
			{
				CreatingLabel.enabled = true;
			}
			m_bNeedToCreateChampionship = true;
		}
	}

	private void OnSubmit()
	{
		if (Input != null)
		{
			string empty = string.Empty;
			empty = ((!(Input.text != string.Empty)) ? Localization.instance.Get(Input.defaultText) : NGUITools.StripSymbols(Input.text));
			if (!string.IsNullOrEmpty(empty) && networkMgr != null && empty != Localization.instance.Get("MENU_GAME_NAME"))
			{
				networkMgr.SGameName = empty;
			}
			else if (Input.text.Equals(string.Empty))
			{
				networkMgr.SGameName = string.Empty;
			}
			Input.selected = false;
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

	public override void Update()
	{
		if (Application.platform == RuntimePlatform.Android && UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		{
			ActSwapMenu(EMenus.MENU_MULTI);
		}
		if (networkMgr.DoneTesting)
		{
			if (m_bNeedToCreateSolo)
			{
				MasterServer.RegisterHost("GK12", "GK12", "Single race," + ((!networkMgr.BLanOnly) ? "WAN," : "LAN,") + networkMgr.SGameName + "," + networkMgr.ExternalIP + ",waitPlayers," + (int)networkMgr.ConnectionStatus);
				ActSwapMenu(EMenus.MENU_MULTI_PLAYERS_LIST);
				MenuMultiWaitingRoom menuMultiWaitingRoom = m_pMenuEntryPoint.MenuRefList[7] as MenuMultiWaitingRoom;
				menuMultiWaitingRoom.Init(EMenus.MENU_MULTI_CREATE, 0, networkMgr.SGameName, 0);
				Singleton<GameConfigurator>.Instance.GameModeType = E_GameModeType.SINGLE;
				Singleton<GameConfigurator>.Instance.CurrentTrackIndex = Random.Range(0, 3);
				m_bNeedToCreateSolo = false;
			}
			else if (m_bNeedToCreateChampionship)
			{
				MasterServer.RegisterHost("GK12", "GK12", "Championship," + ((!networkMgr.BLanOnly) ? "WAN," : "LAN,") + networkMgr.SGameName + "," + networkMgr.ExternalIP + ",waitPlayers," + (int)networkMgr.ConnectionStatus);
				ActSwapMenu(EMenus.MENU_MULTI_PLAYERS_LIST);
				MenuMultiWaitingRoom menuMultiWaitingRoom2 = m_pMenuEntryPoint.MenuRefList[7] as MenuMultiWaitingRoom;
				menuMultiWaitingRoom2.Init(EMenus.MENU_MULTI_CREATE, 0, networkMgr.SGameName, 1);
				Singleton<GameConfigurator>.Instance.GameModeType = E_GameModeType.CHAMPIONSHIP;
				Singleton<GameConfigurator>.Instance.CurrentTrackIndex = 0;
				m_bNeedToCreateChampionship = false;
			}
		}
	}
}
