using System;
using UnityEngine;

public class PanelKartIcon : MonoBehaviour
{
	public UISprite m_oCharacterIcon;

	public UISprite m_oKartIcon;

	public UISprite m_oCustomIcon;

	public UISprite m_oHatIcon;

	public Color NoneColor = default(Color);

	[HideInInspector]
	public Color[] CharacterColor = new Color[Enum.GetValues(typeof(ECharacter)).Length - 1];

	public void OnUpdatePanel()
	{
		if ((bool)m_oCharacterIcon && (bool)m_oHatIcon && (bool)m_oKartIcon && (bool)m_oCustomIcon)
		{
			PlayerConfig playerConfig = Singleton<GameConfigurator>.Instance.PlayerConfig;
			KartCarac kartCarac = (KartCarac)Resources.Load("Kart/" + playerConfig.KartPrefab[(int)playerConfig.m_eKart], typeof(KartCarac));
			if (playerConfig.m_oHat.name.Contains("_Def"))
			{
				m_oHatIcon.color = NoneColor;
			}
			else
			{
				m_oHatIcon.color = CharacterColor[(int)playerConfig.m_oHat.Character];
			}
			if (playerConfig.m_oKartCustom.name.Contains("_Def"))
			{
				m_oCustomIcon.color = NoneColor;
			}
			else
			{
				m_oCustomIcon.color = CharacterColor[(int)playerConfig.m_oKartCustom.Owner];
			}
			m_oKartIcon.color = CharacterColor[(int)kartCarac.Owner];
			m_oCharacterIcon.color = CharacterColor[(int)playerConfig.m_eCharacter];
		}
	}
}
