using UnityEngine;

public class BtnServer : AbstractMenu
{
	private int m_iId;

	private GameObject m_oMenuParent;

	private string m_sServerName;

	private int m_iGameType;

	private int m_iNbPlayers;

	private Transform m_oButton;

	private UILabel m_oLabelNbPlayers;

	public override void Awake()
	{
	}

	public void Init(int iId, string sServerName, int iNbPlayers, int iGameType, GameObject oMenuParent, GameObject oDragPanel)
	{
		m_iId = iId;
		m_oMenuParent = oMenuParent;
		m_iGameType = iGameType;
		m_sServerName = sServerName;
		m_iNbPlayers = iNbPlayers;
		m_oButton = base.transform.FindChild("Button");
		if ((bool)m_oButton)
		{
			UIDragPanelContents component = m_oButton.gameObject.GetComponent<UIDragPanelContents>();
			if ((bool)component)
			{
				component.draggablePanel = oDragPanel.GetComponent<UIDraggablePanel>();
			}
			Transform transform = m_oButton.Find("LabelServerName");
			if ((bool)transform)
			{
				UILabel component2 = transform.gameObject.GetComponent<UILabel>();
				if ((bool)component2)
				{
					component2.text = sServerName;
				}
			}
			SetPlayerCount(iNbPlayers);
			transform = m_oButton.Find("SpriteTypeGame");
			if ((bool)transform)
			{
				UITexturePattern component3 = transform.gameObject.GetComponent<UITexturePattern>();
				if ((bool)component3)
				{
					component3.ChangeTexture(iGameType);
				}
			}
		}
		OnEnter();
	}

	public string GetServerName()
	{
		return m_sServerName;
	}

	public int GetGameType()
	{
		return m_iGameType;
	}

	public int GetPlayerCount()
	{
		return m_iNbPlayers;
	}

	public void SetPlayerCount(int nb)
	{
		m_iNbPlayers = nb;
		if (m_oLabelNbPlayers == null)
		{
			Transform transform = m_oButton.Find("LabelNbPlayers");
			if ((bool)transform)
			{
				m_oLabelNbPlayers = transform.gameObject.GetComponent<UILabel>();
			}
		}
		if ((bool)m_oLabelNbPlayers)
		{
			m_oLabelNbPlayers.text = string.Format("{0}/6 " + Localization.instance.Get("MENU_PLAYERS"), m_iNbPlayers);
		}
	}

	public void OnClick()
	{
		if ((bool)m_oMenuParent)
		{
			m_oMenuParent.SendMessage("OnServer", m_iId, SendMessageOptions.DontRequireReceiver);
		}
	}
}
