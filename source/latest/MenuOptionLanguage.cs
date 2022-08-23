using UnityEngine;

public class MenuOptionLanguage : AbstractMenu
{
	public UITexturePattern m_oIconLanguage;

	public override void OnEnter()
	{
		base.OnEnter();
		if ((bool)m_oIconLanguage)
		{
			m_oIconLanguage.ChangeTexture((int)Singleton<GameOptionManager>.Instance.GetCurrentLangId());
		}
	}

	public void OnSelectLanguage(int iLang)
	{
		Singleton<GameOptionManager>.Instance.SetLanguage((GameOptionManager.ELangID)iLang, true);
		if ((bool)m_oIconLanguage)
		{
			m_oIconLanguage.ChangeTexture(iLang);
		}
	}

	public override void Update()
	{
		if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape))
		{
			ActSwapMenu(EMenus.MENU_OPTIONS);
		}
	}
}
