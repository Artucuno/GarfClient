using UnityEngine;

public class MenuOption : AbstractMenu
{
	public UITexturePattern m_oIconLanguage;

	public GameObject Controls;

	public override void OnEnter()
	{
		base.OnEnter();
		if ((bool)m_oIconLanguage)
		{
			m_oIconLanguage.ChangeTexture((int)Singleton<GameOptionManager>.Instance.GetCurrentLangId());
		}
		if ((bool)Controls && Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
		{
			Controls.SetActive(true);
			Controls.GetComponentInChildren<UILocalize>().key = "Multiplayer";
		}
	}
}
