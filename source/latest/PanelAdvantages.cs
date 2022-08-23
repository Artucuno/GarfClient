using UnityEngine;

public class PanelAdvantages : AbstractMenu
{
	public BtnSlotAdvantage[] m_oSlots = new BtnSlotAdvantage[4];

	private MenuSelectKart m_pMenuSelectKart;

	public GameObject[] m_oToHide;

	public GameObject m_oTarget;

	private GameObject m_oPanel;

	private TweenPosition m_oTweenPosition;

	private TweenScale m_oTweenScale;

	private bool m_bHaveToBeDisabled;

	public override void Awake()
	{
		Transform parent = base.transform.parent;
		m_pMenuSelectKart = parent.GetComponent<MenuSelectKart>();
		while (m_pMenuSelectKart == null)
		{
			parent = parent.parent;
			if (!parent)
			{
				return;
			}
			m_pMenuSelectKart = parent.GetComponent<MenuSelectKart>();
		}
		m_oPanel = GameObject.Find("Anchor_Center/PanelData/PanelAdvantages");
		if (!(m_oPanel != null))
		{
			return;
		}
		m_oTweenPosition = m_oPanel.GetComponent<TweenPosition>();
		m_oTweenScale = m_oPanel.GetComponent<TweenScale>();
		if (m_oTweenPosition != null)
		{
			m_oTweenPosition.to = m_oPanel.transform.localPosition;
			m_oTweenPosition.from = base.transform.InverseTransformPoint(m_oTarget.transform.position);
			m_oTweenPosition.eventReceiver = base.gameObject;
			if (m_oTweenScale != null)
			{
				m_oTweenScale.delay = m_oTweenPosition.delay;
			}
		}
	}

	public void Initialize()
	{
		int nbSlots = Singleton<GameConfigurator>.Instance.NbSlots;
		for (int i = 0; i < 4; i++)
		{
			m_oSlots[i].SetEnable((i < nbSlots) ? true : false);
			m_oSlots[i].Initialize();
		}
		BroadcastMessage("OnInitialize", SendMessageOptions.DontRequireReceiver);
	}

	public override void OnEnter()
	{
		base.OnEnter();
		if (m_oTweenPosition != null && m_oTweenScale != null)
		{
			GameObject[] oToHide = m_oToHide;
			foreach (GameObject gameObject in oToHide)
			{
				gameObject.SetActive(true);
			}
			m_oTweenPosition.callWhenFinished = string.Empty;
			m_oTweenPosition.Play(true);
			m_oTweenScale.Play(true);
		}
		m_oTarget.SetActive(true);
		Initialize();
	}

	public override void OnExit()
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		if (m_oTweenPosition != null && m_oTweenScale != null)
		{
			m_oTweenPosition.Play(false);
			m_oTweenPosition.callWhenFinished = "Disable";
			m_oTweenScale.Play(false);
			m_bHaveToBeDisabled = true;
			GameObject[] oToHide = m_oToHide;
			foreach (GameObject gameObject in oToHide)
			{
				gameObject.SetActive(false);
			}
			m_oTarget.SetActive(false);
		}
		else
		{
			base.OnExit();
		}
	}

	public void HiddenExit()
	{
		if (base.gameObject.activeSelf)
		{
			if (m_oTweenPosition != null && m_oTweenScale != null)
			{
				m_oTweenPosition.Reset();
				m_oTweenScale.Reset();
			}
			base.OnExit();
		}
	}

	public void Disable()
	{
		if (m_bHaveToBeDisabled)
		{
			m_bHaveToBeDisabled = false;
			base.gameObject.SetActive(false);
			m_oTarget.SetActive(true);
		}
	}

	public void ValidSlots()
	{
		int nbSlots = Singleton<GameConfigurator>.Instance.NbSlots;
		for (int i = 0; i < nbSlots; i++)
		{
			m_oSlots[i].ValidSlot();
		}
	}

	public bool IsAdvantageAvailable(EAdvantage eAdvantage)
	{
		if ((bool)m_pMenuSelectKart)
		{
			return m_pMenuSelectKart.IsAdvantageAvailable(eAdvantage);
		}
		return false;
	}

	public Transform GetAdvantageBtnItem(EAdvantage eAdv)
	{
		if (!m_pMenuSelectKart || !m_pMenuSelectKart.m_oScrollPanel)
		{
			return null;
		}
		BtnItem[] componentsInChildren = m_pMenuSelectKart.m_oScrollPanel.GetComponentsInChildren<BtnItem>();
		foreach (BtnItem btnItem in componentsInChildren)
		{
			AdvantageData advantageData = (AdvantageData)btnItem.GetData();
			if ((bool)advantageData && advantageData.AdvantageType == eAdv)
			{
				return btnItem.transform;
			}
		}
		return null;
	}

	public BtnSlotAdvantage GetFreeSlot()
	{
		BtnSlotAdvantage[] oSlots = m_oSlots;
		foreach (BtnSlotAdvantage btnSlotAdvantage in oSlots)
		{
			if (btnSlotAdvantage.IsActive && btnSlotAdvantage.IsEmpty)
			{
				return btnSlotAdvantage;
			}
		}
		return null;
	}
}
