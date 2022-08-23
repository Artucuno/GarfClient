using UnityEngine;

public class BtnSlotAdvantage : MonoBehaviour
{
	private PanelAdvantages m_pPanelAdvantages;

	public UITexturePattern m_pIcon;

	private bool m_bEmpty = true;

	private BtnItemAdvantage m_pItem;

	public AudioSource DropAdvantageSound;

	public bool m_bIsActive;

	public bool IsEmpty
	{
		get
		{
			return m_bEmpty;
		}
	}

	public bool IsActive
	{
		get
		{
			return m_bIsActive;
		}
	}

	private void Start()
	{
		Transform parent = base.transform.parent;
		while ((bool)parent && !parent.GetComponent<PanelAdvantages>())
		{
			parent = parent.parent;
		}
		if ((bool)parent)
		{
			m_pPanelAdvantages = parent.GetComponent<PanelAdvantages>();
		}
		m_pIcon.gameObject.SetActive(false);
	}

	private void Update()
	{
	}

	public void SetEnable(bool bIsEnabled)
	{
		base.gameObject.SetActive(bIsEnabled);
		if (bIsEnabled)
		{
			OnUpdatePanel();
			return;
		}
		m_pIcon.gameObject.SetActive(false);
		if (!m_bEmpty)
		{
			m_pItem.UnlockItem();
			m_bEmpty = true;
		}
	}

	public void OnUpdatePanel()
	{
		if (m_bEmpty)
		{
			m_pIcon.gameObject.SetActive(false);
		}
		else
		{
			m_pIcon.gameObject.SetActive(true);
		}
	}

	public void SetItem(BtnItemAdvantage pItem)
	{
		m_pItem = pItem;
		m_pIcon.ChangeTexture((int)m_pItem.m_eAdvantage);
		m_pIcon.transform.localScale = m_pItem.m_pIcon.transform.localScale;
		m_pItem.LockItem();
		m_pItem.OnUpdatePanel();
		m_bEmpty = false;
		OnUpdatePanel();
		if ((bool)DropAdvantageSound)
		{
			DropAdvantageSound.Play();
		}
	}

	private void OnPress(bool bIsPressed)
	{
		if (bIsPressed && !m_bEmpty && (bool)m_pItem)
		{
			m_pItem.UnlockItem();
			m_bEmpty = true;
			OnUpdatePanel();
			Transform advantageBtnItem = m_pPanelAdvantages.GetAdvantageBtnItem(m_pItem.m_eAdvantage);
			if ((bool)advantageBtnItem)
			{
				advantageBtnItem.gameObject.SendMessage("OnClick");
			}
		}
	}

	public void ValidSlot()
	{
		if (!m_bEmpty)
		{
			Singleton<GameConfigurator>.Instance.PlayerConfig.AddAdvantage(m_pItem.m_eAdvantage);
		}
	}

	public void Initialize()
	{
		if (!m_bEmpty && (bool)m_pPanelAdvantages && !m_pPanelAdvantages.IsAdvantageAvailable(m_pItem.m_eAdvantage))
		{
			m_pItem.UnlockItem();
			m_bEmpty = true;
			OnUpdatePanel();
		}
	}

	public void OnEnable()
	{
		m_bIsActive = true;
	}

	public void OnDisable()
	{
		m_bIsActive = false;
	}
}
