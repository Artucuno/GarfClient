using System.Collections.Generic;
using UnityEngine;

public class MenuMultiJoin : AbstractMenu
{
	public GameObject m_oScrollPanel;

	public GameObject m_oButtonServerTemplate;

	public float m_fAutoRefreshDelay = 3f;

	public GameObject NoGame;

	private NetworkMgr networkMgr;

	private float m_fRefreshTimer;

	private string sServerName;

	private int iGameType;

	private int serverId = -1;

	private int m_iNextId;

	public UILabel ConnectingLabel;

	private Dictionary<int, GameObject> m_oButtonServerList = new Dictionary<int, GameObject>();

	private Dictionary<int, HostData> m_oHostDataDic = new Dictionary<int, HostData>();

	private PopupDialog oPopup;

	private float m_fSpeedScroll;

	private float m_fTimeScroll = 0.3f;

	public override void Awake()
	{
		base.Awake();
		networkMgr = (NetworkMgr)Object.FindObjectOfType(typeof(NetworkMgr));
	}

	public override void OnEnter()
	{
		base.OnEnter();
		serverId = -1;
		m_fRefreshTimer = m_fAutoRefreshDelay;
		if (ConnectingLabel != null)
		{
			ConnectingLabel.enabled = false;
		}
	}

	public void RefreshList()
	{
		m_fRefreshTimer = 0f;
		MasterServer.ClearHostList();
		MasterServer.RequestHostList("GK12");
	}

	public override void OnExit()
	{
		RemoveAllServers();
		base.OnExit();
	}

	public override void Update()
	{
		if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape))
		{
			ActSwapMenu(EMenus.MENU_MULTI);
		}
		float deltaTime = Time.deltaTime;
		m_fTimeScroll -= deltaTime;
		if (m_oScrollPanel != null && m_fSpeedScroll != 0f && m_fTimeScroll < 0f)
		{
			m_fTimeScroll = 0.1f;
			UIDraggablePanel component = m_oScrollPanel.GetComponent<UIDraggablePanel>();
			if ((bool)component)
			{
				component.Scroll(m_fSpeedScroll);
			}
		}
		if (serverId != -1 && networkMgr.DoneTesting)
		{
			string[] array = m_oHostDataDic[serverId].comment.Split(',');
			string text = array[3];
			ConnectionTesterStatus serverNATType = (ConnectionTesterStatus)int.Parse(array[5]);
			if (!((!(networkMgr.ExternalIP != string.Empty)) ? text.Equals(networkMgr.networkView.owner.externalIP) : text.Equals(networkMgr.ExternalIP)) && !networkMgr.CanConnectTo(serverNATType))
			{
				oPopup = (PopupDialog)m_pMenuEntryPoint.ShowPopup(EPopUps.POPUP_DIALOG, false);
				if ((bool)oPopup)
				{
					oPopup.Show("MENU_POPUP_NAT_ERROR");
				}
			}
			Network.Connect(m_oHostDataDic[serverId]);
			if (ConnectingLabel != null)
			{
				ConnectingLabel.enabled = false;
			}
			serverId = -1;
		}
		m_fRefreshTimer += Time.deltaTime;
		if (m_fRefreshTimer > m_fAutoRefreshDelay || m_fRefreshTimer > 1f)
		{
			HostData[] array2 = MasterServer.PollHostList();
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, HostData> item in m_oHostDataDic)
			{
				bool flag = false;
				HostData[] array3 = array2;
				foreach (HostData hostData in array3)
				{
					string[] array4 = hostData.comment.Split(',');
					if (hostData.guid == item.Value.guid && array4.Length > 4 && array4[4] == "waitPlayers")
					{
						BtnServer component2 = m_oButtonServerList[item.Key].GetComponent<BtnServer>();
						item.Value.connectedPlayers = hostData.connectedPlayers;
						component2.SetPlayerCount(hostData.connectedPlayers);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					list.Add(item.Key);
				}
			}
			foreach (int item2 in list)
			{
				RemoveServer(item2);
			}
			List<HostData> list2 = new List<HostData>();
			HostData[] array5 = array2;
			foreach (HostData hostData2 in array5)
			{
				string[] array6 = hostData2.comment.Split(',');
				bool flag2 = false;
				if (array6.Length > 4 && array6[4] == "startGame")
				{
					continue;
				}
				foreach (KeyValuePair<int, HostData> item3 in m_oHostDataDic)
				{
					if (hostData2.guid == item3.Value.guid)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					list2.Add(hostData2);
				}
			}
			foreach (HostData item4 in list2)
			{
				string[] array7 = item4.comment.Split(',');
				int type = ((!array7[0].Equals("Single race")) ? 1 : 0);
				bool flag3 = array7[1].Equals("LAN") || networkMgr.BLanOnly;
				string sGameName = array7[2];
				string text2 = string.Empty;
				if (array7.Length > 3)
				{
					text2 = array7[3];
				}
				if (!flag3 || text2.Equals(networkMgr.ExternalIP))
				{
					AddServer(m_iNextId++, item4, type, sGameName);
				}
			}
		}
		if (m_fRefreshTimer > m_fAutoRefreshDelay)
		{
			RefreshList();
		}
	}

	public void AddServer(int iId, HostData host, int type, string sGameName)
	{
		m_oHostDataDic.Add(iId, host);
		AddServer(iId, sGameName, host.connectedPlayers, type);
	}

	public void AddServer(int iId, string sServerName, int iNbPlayers, int iGameType)
	{
		if (!m_oScrollPanel)
		{
			return;
		}
		GameObject gameObject = m_oScrollPanel.transform.GetChild(0).gameObject;
		GameObject gameObject2 = (GameObject)Object.Instantiate(m_oButtonServerTemplate);
		if (!gameObject2)
		{
			return;
		}
		gameObject2.transform.parent = gameObject.transform;
		m_oButtonServerList.Add(iId, gameObject2);
		BtnServer component = gameObject2.GetComponent<BtnServer>();
		if ((bool)component)
		{
			component.Init(iId, sServerName, iNbPlayers, iGameType, base.gameObject, m_oScrollPanel);
			gameObject.SendMessage("Reposition");
			if ((bool)NoGame && NoGame.activeSelf)
			{
				NoGame.SetActive(false);
			}
		}
	}

	public void RemoveServer(int iId)
	{
		GameObject value;
		if (m_oButtonServerList.TryGetValue(iId, out value))
		{
			Object.Destroy(value);
			m_oButtonServerList.Remove(iId);
		}
		m_oHostDataDic.Remove(iId);
		GameObject gameObject = m_oScrollPanel.transform.GetChild(0).gameObject;
		gameObject.SendMessage("Reposition");
		if (m_oButtonServerList.Count == 0 && (bool)NoGame && !NoGame.activeSelf)
		{
			NoGame.SetActive(true);
		}
	}

	public void RemoveAllServers()
	{
		foreach (KeyValuePair<int, GameObject> oButtonServer in m_oButtonServerList)
		{
			Object.Destroy(oButtonServer.Value);
		}
		m_oButtonServerList.Clear();
		m_oHostDataDic.Clear();
		if ((bool)NoGame && !NoGame.activeSelf)
		{
			NoGame.SetActive(true);
		}
	}

	public void OnButtonUpDown(int iStep)
	{
		m_fSpeedScroll = (float)iStep * 0.1f;
	}

	public void OnButtonRefresh()
	{
		m_fRefreshTimer = m_fAutoRefreshDelay + 1f;
	}

	public void OnServer(int iId)
	{
		GameObject value;
		if (!m_oButtonServerList.TryGetValue(iId, out value))
		{
			return;
		}
		BtnServer component = value.GetComponent<BtnServer>();
		if (!component || serverId != -1)
		{
			return;
		}
		if (component.GetPlayerCount() >= 6)
		{
			PopupDialog popupDialog = (PopupDialog)m_pMenuEntryPoint.ShowPopup(EPopUps.POPUP_DIALOG, false);
			if ((bool)popupDialog)
			{
				popupDialog.Show("MENU_POPUP_FULL");
			}
			return;
		}
		sServerName = component.GetServerName();
		iGameType = component.GetGameType();
		Network.Disconnect();
		if (ConnectingLabel != null)
		{
			ConnectingLabel.enabled = true;
		}
		serverId = iId;
	}

	public void OnConnectedToServer()
	{
		if (oPopup != null)
		{
			oPopup.Hide();
			oPopup.OnQuit();
			oPopup = null;
		}
		if (ConnectingLabel != null)
		{
			ConnectingLabel.enabled = false;
		}
		MenuMultiWaitingRoom menuMultiWaitingRoom = m_pMenuEntryPoint.MenuRefList[7] as MenuMultiWaitingRoom;
		menuMultiWaitingRoom.Init(EMenus.MENU_MULTI_JOIN, serverId, sServerName, iGameType);
		ActSwapMenu(EMenus.MENU_MULTI_PLAYERS_LIST);
		Singleton<GameConfigurator>.Instance.GameModeType = ((iGameType != 0) ? E_GameModeType.CHAMPIONSHIP : E_GameModeType.SINGLE);
	}

	public void OnFailedToConnect(NetworkConnectionError error)
	{
		serverId = -1;
		PopupDialog popupDialog = (PopupDialog)m_pMenuEntryPoint.ShowPopup(EPopUps.POPUP_DIALOG, false);
		if ((bool)popupDialog)
		{
			popupDialog.Show("MENU_POPUP_SERVER_CONNECTION_ERROR");
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
