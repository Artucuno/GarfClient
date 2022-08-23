using System;
using UnityEngine;

public class BtnItem : AbstractMenu
{
	private GameObject m_oMenuParent;

	private UnityEngine.Object m_oData;

	private GameObject m_oDragPanel;

	private UICheckbox m_oCheckBox;

	public UIAtlas m_oAtlas;

	public override void Awake()
	{
	}

	public override void Update()
	{
		base.Update();
		if ((bool)m_oCheckBox && m_oCheckBox.isChecked && (bool)m_oMenuParent && (bool)m_oDragPanel && (bool)m_oDragPanel)
		{
			UIPanel component = m_oDragPanel.GetComponent<UIPanel>();
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(m_oDragPanel.transform, base.transform);
			Vector3 vector = component.CalculateConstrainOffset(bounds.min, bounds.max);
			float num = (bounds.max.x - bounds.min.x) * 0.4f;
			MenuSelectKart component2 = m_oMenuParent.GetComponent<MenuSelectKart>();
			component2.SetPanelTextAlpha(1f - Math.Min(num, Math.Max(0f, Math.Abs(vector.x) - (bounds.max.x - bounds.min.x) * 0.2f)) / num);
		}
	}

	public void Init(GameObject oMenuParent, GameObject oDragPanel, string sSpriteName, ERarity eRarity, int iPrice, UnityEngine.Object oData, E_UnlockableItemSate eState, bool bSelected)
	{
		m_oMenuParent = oMenuParent;
		m_oData = oData;
		m_oDragPanel = oDragPanel;
		Transform transform = base.transform.FindChild("Button");
		if ((bool)transform)
		{
			UIDragPanelContents component = transform.gameObject.GetComponent<UIDragPanelContents>();
			if ((bool)component)
			{
				component.draggablePanel = oDragPanel.GetComponent<UIDraggablePanel>();
			}
			Transform transform2;
			if (eState != 0)
			{
				transform2 = transform.FindChild("Icon");
				if ((bool)transform2)
				{
					UISprite component2 = transform2.GetComponent<UISprite>();
					if ((bool)component2)
					{
						component2.atlas = m_oAtlas;
						component2.spriteName = sSpriteName;
					}
				}
			}
			else
			{
				transform.GetComponent<BoxCollider>().enabled = false;
			}
			transform2 = transform.FindChild("IconNew");
			if ((bool)transform2)
			{
				if (eState == E_UnlockableItemSate.NewLocked || eState == E_UnlockableItemSate.NewUnlocked)
				{
					transform2.gameObject.SetActive(true);
				}
				else
				{
					transform2.gameObject.SetActive(false);
				}
			}
			transform2 = transform.FindChild("Rarity");
			if ((bool)transform2)
			{
				UITexturePattern component3 = transform2.GetComponent<UITexturePattern>();
				if ((bool)component3)
				{
					component3.ChangeTexture(Tricks.LogBase2(Math.Max(1, (int)eRarity)));
				}
			}
			transform2 = transform.FindChild("PriceTag");
			if ((bool)transform2)
			{
				if ((eState == E_UnlockableItemSate.Locked || eState == E_UnlockableItemSate.NewLocked) && iPrice > 0)
				{
					UILabel component4 = transform2.GetComponent<UILabel>();
					if ((bool)component4)
					{
						component4.text = string.Format("{0}", iPrice);
					}
					transform2.gameObject.SetActive(true);
					transform2 = transform.FindChild("BgPrice");
					if ((bool)transform2)
					{
						transform2.gameObject.SetActive(true);
					}
					transform2 = transform.FindChild("IconMoney");
					if ((bool)transform2)
					{
						transform2.gameObject.SetActive(true);
					}
				}
				else
				{
					transform2.gameObject.SetActive(false);
					transform2 = transform.FindChild("BgPrice");
					if ((bool)transform2)
					{
						transform2.gameObject.SetActive(false);
					}
					transform2 = transform.FindChild("IconMoney");
					if ((bool)transform2)
					{
						transform2.gameObject.SetActive(false);
					}
				}
			}
			m_oCheckBox = transform.GetComponent<UICheckbox>();
			if ((bool)m_oCheckBox)
			{
				m_oCheckBox.isChecked = bSelected;
				m_oCheckBox.radioButtonRoot = oDragPanel.transform;
			}
		}
		OnEnter();
	}

	public void OnClick()
	{
		if ((bool)m_oCheckBox && !m_oCheckBox.isChecked)
		{
			m_oCheckBox.isChecked = true;
		}
		if ((bool)m_oMenuParent)
		{
			m_oMenuParent.SendMessage("OnClickItem", m_oData, SendMessageOptions.DontRequireReceiver);
		}
		UIPanel component = m_oDragPanel.GetComponent<UIPanel>();
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(m_oDragPanel.transform, base.transform);
		Vector3 vector = component.CalculateConstrainOffset(bounds.min, bounds.max);
		SpringPanel.Begin(component.gameObject, m_oDragPanel.transform.localPosition + vector, 13f);
	}

	public bool IsDataEqual(UnityEngine.Object oData)
	{
		return (m_oData == oData) ? true : false;
	}

	public void RefreshState(E_UnlockableItemSate eState)
	{
		Transform transform = base.transform.FindChild("Button");
		if (!transform)
		{
			return;
		}
		if (eState != 0)
		{
			transform.GetComponent<BoxCollider>().enabled = true;
		}
		Transform transform2 = transform.FindChild("IconNew");
		if ((bool)transform2)
		{
			if (eState == E_UnlockableItemSate.NewLocked || eState == E_UnlockableItemSate.NewUnlocked)
			{
				transform2.gameObject.SetActive(true);
			}
			else
			{
				transform2.gameObject.SetActive(false);
			}
		}
		transform2 = transform.FindChild("PriceTag");
		if ((bool)transform2 && (eState == E_UnlockableItemSate.Unlocked || eState == E_UnlockableItemSate.NewUnlocked))
		{
			transform2.gameObject.SetActive(false);
			transform2 = transform.FindChild("BgPrice");
			if ((bool)transform2)
			{
				transform2.gameObject.SetActive(false);
			}
			transform2 = transform.FindChild("IconMoney");
			if ((bool)transform2)
			{
				transform2.gameObject.SetActive(false);
			}
		}
	}

	public UnityEngine.Object GetData()
	{
		return m_oData;
	}
}
