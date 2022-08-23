public class Popup3Choices : AbstractPopup
{
	public delegate void Callback(object param);

	public UILocalize m_oLabelButton1;

	public UILocalize m_oLabelButton2;

	public UILocalize m_oLabelButton3;

	private Callback m_oCbButton1;

	private Callback m_oCbButton2;

	private Callback m_oCbButton3;

	private object m_oParam;

	public void Show(string sTextIdDialog, Callback oCb1 = null, Callback oCb2 = null, Callback oCb3 = null, object oParam = null, string sTextIdBtn1 = null, string sTextIdBtn2 = null, string sTextIdBtn3 = null)
	{
		m_oCbButton1 = oCb1;
		m_oCbButton2 = oCb2;
		m_oCbButton3 = oCb3;
		m_oParam = oParam;
		if (sTextIdBtn1 != null)
		{
			m_oLabelButton1.key = sTextIdBtn1;
		}
		if (sTextIdBtn2 != null)
		{
			m_oLabelButton2.key = sTextIdBtn2;
		}
		if (sTextIdBtn3 != null)
		{
			m_oLabelButton3.key = sTextIdBtn3;
		}
		base.Show(sTextIdDialog);
	}

	public void ShowText(string sTextDialog, Callback oCb1 = null, Callback oCb2 = null, Callback oCb3 = null, object oParam = null, string sTextIdBtn1 = null, string sTextIdBtn2 = null, string sTextIdBtn3 = null)
	{
		base.gameObject.SetActive(true);
		m_oCbButton1 = oCb1;
		m_oCbButton2 = oCb2;
		m_oCbButton3 = oCb3;
		m_oParam = oParam;
		if (sTextIdBtn1 != null)
		{
			m_oLabelButton1.key = sTextIdBtn1;
		}
		if (sTextIdBtn2 != null)
		{
			m_oLabelButton2.key = sTextIdBtn2;
		}
		if (sTextIdBtn3 != null)
		{
			m_oLabelButton3.key = sTextIdBtn3;
		}
		UILabel component = Text.gameObject.GetComponent<UILabel>();
		if ((bool)component)
		{
			component.text = sTextDialog;
		}
	}

	public void OnButton1()
	{
		OnQuit();
		if (m_oCbButton1 != null)
		{
			m_oCbButton1(m_oParam);
		}
	}

	public void OnButton2()
	{
		OnQuit();
		if (m_oCbButton2 != null)
		{
			m_oCbButton2(m_oParam);
		}
	}

	public void OnButton3()
	{
		OnQuit();
		if (m_oCbButton3 != null)
		{
			m_oCbButton3(m_oParam);
		}
	}
}
