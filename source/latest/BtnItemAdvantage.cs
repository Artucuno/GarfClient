using UnityEngine;

public class BtnItemAdvantage : MonoBehaviour
{
	private PanelAdvantages m_pPanelAdvantages;

	public EAdvantage m_eAdvantage;

	public UILabel m_oLabel;

	public GameObject m_pIcon;

	public GameObject m_pIconNumber;

	private int m_iNb;

	private int m_iNbLocked;

	public Transform m_pItemLink;

	private bool m_bDisabledFromRestriction;

	private bool m_bColliderDisabled;

	public AudioSource SlotsFullErrorSound;

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
		m_iNb = -1;
		m_iNbLocked = 0;
		m_bDisabledFromRestriction = false;
		m_bColliderDisabled = false;
		OnInitialize();
		OnUpdatePanel();
	}

	public void OnInitialize()
	{
		if (!m_pPanelAdvantages)
		{
			return;
		}
		if (m_pPanelAdvantages.IsAdvantageAvailable(m_eAdvantage))
		{
			m_bDisabledFromRestriction = false;
			if ((bool)m_pIcon)
			{
				UISprite component = m_pIcon.GetComponent<UISprite>();
				if ((bool)component)
				{
					component.alpha = 1f;
				}
				component = m_pIconNumber.GetComponent<UISprite>();
				if ((bool)component)
				{
					component.alpha = 1f;
				}
				if (m_oLabel != null)
				{
					m_oLabel.alpha = 1f;
				}
			}
		}
		else
		{
			m_bDisabledFromRestriction = true;
			if ((bool)m_pIcon)
			{
				UISprite component2 = m_pIcon.GetComponent<UISprite>();
				if ((bool)component2)
				{
					component2.alpha = 0.3f;
				}
				component2 = m_pIconNumber.GetComponent<UISprite>();
				if ((bool)component2)
				{
					component2.alpha = 0.3f;
				}
				if (m_oLabel != null)
				{
					m_oLabel.alpha = 0.3f;
				}
			}
		}
		UpdateCollider();
		m_pItemLink = null;
	}

	private void OnPress(bool bIsPressed)
	{
		if (bIsPressed)
		{
			BtnSlotAdvantage freeSlot = m_pPanelAdvantages.GetFreeSlot();
			if (freeSlot != null)
			{
				freeSlot.SetItem(this);
			}
			else if ((bool)SlotsFullErrorSound)
			{
				SlotsFullErrorSound.Play();
			}
			if (!m_pItemLink)
			{
				m_pItemLink = m_pPanelAdvantages.GetAdvantageBtnItem(m_eAdvantage);
			}
			if ((bool)m_pItemLink)
			{
				m_pItemLink.gameObject.SendMessage("OnClick");
			}
			OnUpdatePanel();
		}
	}

	public void OnUpdatePanel()
	{
		int num = Singleton<GameSaveManager>.Instance.GetAdvantageQuantity(m_eAdvantage) - m_iNbLocked;
		if (m_iNb == num || !m_oLabel || !m_pIcon || !m_pIconNumber)
		{
			return;
		}
		if (num > 0)
		{
			m_oLabel.text = num.ToString();
			if (m_iNb <= 0)
			{
				m_oLabel.gameObject.SetActive(true);
				m_pIcon.SetActive(true);
				m_pIconNumber.SetActive(true);
				m_bColliderDisabled = false;
			}
		}
		else
		{
			m_oLabel.gameObject.SetActive(false);
			m_pIcon.SetActive(false);
			m_pIconNumber.SetActive(false);
			m_bColliderDisabled = true;
		}
		UpdateCollider();
		m_iNb = num;
	}

	public void LockItem()
	{
		m_iNbLocked++;
	}

	public void UnlockItem()
	{
		m_iNbLocked--;
		if (m_iNbLocked < 0)
		{
		}
		OnUpdatePanel();
	}

	private void UpdateCollider()
	{
		if ((bool)base.collider)
		{
			base.collider.enabled = ((!m_bColliderDisabled && !m_bDisabledFromRestriction) ? true : false);
		}
	}
}
