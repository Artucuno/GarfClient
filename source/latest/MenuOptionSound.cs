using UnityEngine;

public class MenuOptionSound : AbstractMenu
{
	private float m_fVolumeSfx;

	private bool m_bIsMute;

	public UISlider m_pSfxSlider;

	public UISprite m_pMuteIcon;

	public override void OnEnter()
	{
		base.OnEnter();
		m_fVolumeSfx = Singleton<GameOptionManager>.Instance.GetSfxVolume();
		if ((bool)m_pSfxSlider)
		{
			m_pSfxSlider.sliderValue = m_fVolumeSfx;
		}
		m_bIsMute = m_fVolumeSfx == 0f;
		m_pMuteIcon.enabled = m_bIsMute;
	}

	public override void OnExit()
	{
		base.OnExit();
		Singleton<GameOptionManager>.Instance.SetSfxVolume((!m_bIsMute) ? m_fVolumeSfx : 0f, true);
		Singleton<GameOptionManager>.Instance.SetMusicVolume((!m_bIsMute) ? m_fVolumeSfx : 0f, true);
	}

	public void OnMute()
	{
		m_bIsMute = !m_bIsMute;
		Singleton<GameOptionManager>.Instance.SetSfxVolume((!m_bIsMute) ? m_fVolumeSfx : 0f, false);
		Singleton<GameOptionManager>.Instance.SetMusicVolume((!m_bIsMute) ? m_fVolumeSfx : 0f, false);
		m_pSfxSlider.sliderValue = ((!m_bIsMute) ? m_fVolumeSfx : 0f);
		m_pMuteIcon.enabled = m_bIsMute;
	}

	public void OnChangeVolumeSfx(float fValue)
	{
		if (fValue > 0f)
		{
			m_fVolumeSfx = fValue;
		}
		Singleton<GameOptionManager>.Instance.SetSfxVolume(fValue, false);
		Singleton<GameOptionManager>.Instance.SetMusicVolume(fValue, false);
		m_bIsMute = fValue == 0f;
		m_pMuteIcon.enabled = m_bIsMute;
	}

	public void OnPressSfx(bool isPressed)
	{
	}

	public override void Update()
	{
		if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape))
		{
			ActSwapMenu(EMenus.MENU_OPTIONS);
		}
	}
}
