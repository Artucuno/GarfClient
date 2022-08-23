public class Popup2Choices : AbstractPopup
{
	public delegate void Callback(object param);

	public UILocalize m_oLabelButtonRight;

	public UILocalize m_oLabelButtonLeft;

	private Callback m_oCbButtonRight;

	private Callback m_oCbButtonLeft;

	private object m_oParam;

	public void Show(string sTextIdDialog, Callback oCbLeft = null, Callback oCbRight = null, object oParam = null, string sTextIdBtnLeft = "MENU_POPUP_NO", string sTextIdBtnRight = "MENU_POPUP_YES")
	{
		m_oCbButtonLeft = oCbLeft;
		m_oCbButtonRight = oCbRight;
		m_oParam = oParam;
		if (sTextIdBtnLeft != null)
		{
			m_oLabelButtonLeft.key = sTextIdBtnLeft;
		}
		if (sTextIdBtnRight != null)
		{
			m_oLabelButtonRight.key = sTextIdBtnRight;
		}
		base.Show(sTextIdDialog);
	}

	public void ShowText(string sTextDialog, Callback oCbLeft = null, Callback oCbRight = null, object oParam = null, string sTextIdBtnLeft = "MENU_POPUP_NO", string sTextIdBtnRight = "MENU_POPUP_YES")
	{
		base.gameObject.SetActive(true);
		m_oCbButtonLeft = oCbLeft;
		m_oCbButtonRight = oCbRight;
		m_oParam = oParam;
		if (sTextIdBtnLeft != null)
		{
			m_oLabelButtonLeft.key = sTextIdBtnLeft;
		}
		if (sTextIdBtnRight != null)
		{
			m_oLabelButtonRight.key = sTextIdBtnRight;
		}
		UILabel component = Text.gameObject.GetComponent<UILabel>();
		if ((bool)component)
		{
			component.text = sTextDialog;
		}
	}

	public void OnButtonLeft()
	{
		OnQuit();
		if (m_oCbButtonLeft != null)
		{
			m_oCbButtonLeft(m_oParam);
		}
	}

	public void OnButtonRight()
	{
		OnQuit();
		if (m_oCbButtonRight != null)
		{
			m_oCbButtonRight(m_oParam);
		}
	}
}
