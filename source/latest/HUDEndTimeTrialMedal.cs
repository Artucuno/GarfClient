using System;
using UnityEngine;

public class HUDEndTimeTrialMedal : HUDEndTimeTrial
{
	public GameObject[] AlienIcons;

	public GameObject[] UFOIcons;

	private UILabel m_LapTime;

	private GameObject m_LapRecord;

	public override void Awake()
	{
		base.Awake();
		for (int i = 0; i < GetNbPlayers(); i++)
		{
			string text = base.gameObject.name + "/Panel/Anchor_Center/Panel" + (i + 1) + "Place";
		}
		string text2 = base.gameObject.name + "/Panel/Anchor_Center/";
		GameObject gameObject = GameObject.Find(text2 + "BestTime");
		m_LapTime = gameObject.GetComponent<UILabel>();
		m_LapRecord = GameObject.Find(text2 + "BestTimeRecordText");
	}

	public override void FillStats(PlayerData[] pPlayerData)
	{
		string startScene = Singleton<GameConfigurator>.Instance.StartScene;
		Kart humanKart = Singleton<GameManager>.Instance.GameMode.GetHumanKart();
		RcVehicleRaceStats raceStats = humanKart.RaceStats;
		GameObject gameObject = GameObject.Find("Race");
		TimeTrialConfig component = gameObject.GetComponent<TimeTrialConfig>();
		E_TimeTrialMedal medal = Singleton<GameSaveManager>.Instance.GetMedal(startScene, true);
		E_TimeTrialMedal e_TimeTrialMedal = medal;
		if (e_TimeTrialMedal != E_TimeTrialMedal.Platinium)
		{
			e_TimeTrialMedal++;
		}
		int num = 1;
		if (!TimeTrialGameMode.BeatTime(e_TimeTrialMedal))
		{
			m_iIndexToFillPlayerData = 1;
			num = 0;
		}
		base.FillStats(pPlayerData);
		if (GuiPerso[m_iIndexToFillPlayerData] != null)
		{
			GuiPerso[m_iIndexToFillPlayerData].spriteName = "GUI_HUD_Select";
		}
		int rpTime = 0;
		Singleton<GameSaveManager>.Instance.GetTimeTrialRecord(startScene, ref rpTime);
		if (raceStats.GetRaceTime() < rpTime || rpTime < 0)
		{
			RecordGO[m_iIndexToFillPlayerData].gameObject.SetActive(true);
		}
		else
		{
			RecordGO[m_iIndexToFillPlayerData].gameObject.SetActive(false);
		}
		RecordGO[num].gameObject.SetActive(false);
		foreach (GameObject item in m_Stars[num])
		{
			item.SetActive(false);
		}
		switch (medal)
		{
		case E_TimeTrialMedal.Platinium:
		{
			int pTime = 0;
			string pseudo = Singleton<GameSaveManager>.Instance.GetPseudo();
			ECharacter rpCharacter = ECharacter.NONE;
			ECharacter rpKart = ECharacter.NONE;
			string rpCustom = string.Empty;
			string rpHat = string.Empty;
			Singleton<GameSaveManager>.Instance.GetTimeTrial(startScene, ref pTime, ref rpCharacter, ref rpKart, ref rpCustom, ref rpHat);
			CharacterName[num].text = pseudo;
			RaceTime[num].text = TimeSpan.FromMilliseconds(pTime).FormatRaceTime();
			foreach (CharacterCarac oCharacter in m_oCharacterList)
			{
				if (oCharacter.Owner == rpCharacter)
				{
					m_Character[num].spriteName = oCharacter.spriteName;
					break;
				}
			}
			foreach (KartCarac oKart in m_oKartList)
			{
				if (oKart.Owner == rpKart)
				{
					m_Kart[num].spriteName = oKart.spriteName;
					break;
				}
			}
			if (rpCustom.Contains("_Def"))
			{
				m_Custom[num].SetActive(false);
				break;
			}
			m_Custom[num].SetActive(true);
			foreach (KartCustom oKartCustom in m_oKartCustomList)
			{
				if (oKartCustom.name == rpCustom)
				{
					m_CustomSprite[num].spriteName = oKartCustom.spriteName;
					m_CustomRarity[num].ChangeTexture((int)oKartCustom.Rarity);
					break;
				}
			}
			break;
		}
		case E_TimeTrialMedal.Gold:
			RaceTime[num].text = TimeSpan.FromMilliseconds(component.Platinium).FormatRaceTime();
			break;
		case E_TimeTrialMedal.Silver:
			RaceTime[num].text = TimeSpan.FromMilliseconds(component.Gold).FormatRaceTime();
			break;
		case E_TimeTrialMedal.Bronze:
			RaceTime[num].text = TimeSpan.FromMilliseconds(component.Silver).FormatRaceTime();
			break;
		case E_TimeTrialMedal.None:
			RaceTime[num].text = TimeSpan.FromMilliseconds(component.Bronze).FormatRaceTime();
			break;
		}
		if (medal != E_TimeTrialMedal.Platinium)
		{
			m_Custom[num].SetActive(false);
			m_CharacterGO[num].SetActive(false);
			m_KartGO[num].SetActive(false);
			UFOIcons[num].SetActive(true);
			AlienIcons[num].SetActive(true);
			CharacterName[num].text = Localization.instance.Get("MENU_TUTO_UFO_TITLE");
		}
		else
		{
			UFOIcons[num].SetActive(false);
			AlienIcons[num].SetActive(false);
		}
	}

	public override int GetNbPlayers()
	{
		return 2;
	}

	public override void ShowLapTime(RcVehicleRaceStats pStats)
	{
		int rpTime = 0;
		string startScene = Singleton<GameConfigurator>.Instance.StartScene;
		Singleton<GameSaveManager>.Instance.GetTimeTrialBestTime(startScene, ref rpTime);
		if (pStats.GetBestLapTime() < rpTime || rpTime < 0)
		{
			m_LapTime.text = TimeSpan.FromMilliseconds(pStats.GetBestLapTime()).FormatRaceTime();
			m_LapRecord.SetActive(true);
		}
		else
		{
			m_LapTime.text = TimeSpan.FromMilliseconds(rpTime).FormatRaceTime();
			m_LapRecord.SetActive(false);
		}
	}
}
