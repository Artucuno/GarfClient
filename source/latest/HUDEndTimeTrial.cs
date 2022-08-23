using System;
using System.Collections.Generic;
using UnityEngine;

public class HUDEndTimeTrial : MonoBehaviour
{
	protected List<UILabel> CharacterName = new List<UILabel>();

	protected List<UILabel> RaceTime = new List<UILabel>();

	protected List<GameObject> RecordGO = new List<GameObject>();

	protected UILabel LapTimes;

	public UILabel TrackName;

	public UITexturePattern DifficultySprite;

	public UITexturePattern ChampionshipIcon;

	protected List<GameObject> m_Custom = new List<GameObject>();

	protected List<UISprite> m_CustomSprite = new List<UISprite>();

	protected List<UITexturePattern> m_CustomRarity = new List<UITexturePattern>();

	protected List<UISprite> m_Character = new List<UISprite>();

	protected List<GameObject> m_CharacterGO = new List<GameObject>();

	protected List<UISprite> m_Kart = new List<UISprite>();

	protected List<GameObject> m_KartGO = new List<GameObject>();

	protected List<List<GameObject>> m_Stars = new List<List<GameObject>>();

	protected List<CharacterCarac> m_oCharacterList = new List<CharacterCarac>();

	protected List<KartCarac> m_oKartList = new List<KartCarac>();

	protected List<KartCustom> m_oKartCustomList = new List<KartCustom>();

	protected List<GameObject> Records = new List<GameObject>();

	protected List<UISprite> GuiPerso = new List<UISprite>();

	protected int m_iIndexToFillPlayerData;

	private float m_fElapsedTime;

	public virtual void Awake()
	{
		UnityEngine.Object[] array = Resources.LoadAll("Character", typeof(CharacterCarac));
		UnityEngine.Object[] array2 = array;
		foreach (UnityEngine.Object @object in array2)
		{
			m_oCharacterList.Add((CharacterCarac)@object);
		}
		UnityEngine.Object[] array3 = Resources.LoadAll("Kart", typeof(KartCarac));
		UnityEngine.Object[] array4 = array3;
		foreach (UnityEngine.Object object2 in array4)
		{
			m_oKartList.Add((KartCarac)object2);
		}
		UnityEngine.Object[] array5 = Resources.LoadAll("Kart", typeof(KartCustom));
		UnityEngine.Object[] array6 = array5;
		foreach (UnityEngine.Object object3 in array6)
		{
			if (object3.name.Contains("_Def"))
			{
				m_oKartCustomList.Insert(0, (KartCustom)object3);
			}
			else
			{
				m_oKartCustomList.Add((KartCustom)object3);
			}
		}
		int nbPlayers = GetNbPlayers();
		for (int l = 0; l < nbPlayers; l++)
		{
			string text = base.gameObject.name + "/Panel/Anchor_Center/Panel" + (l + 1) + "Place";
			GameObject gameObject = GameObject.Find(text + "/Name");
			CharacterName.Add(gameObject.GetComponent<UILabel>());
			GameObject gameObject2 = GameObject.Find(text + "/Time");
			RaceTime.Add(gameObject2.GetComponent<UILabel>());
			GameObject item = GameObject.Find(text + "/RecordText");
			RecordGO.Add(item);
			GameObject gameObject3 = GameObject.Find(text + "/GuiPersonage");
			GuiPerso.Add(gameObject3.GetComponent<UISprite>());
			GameObject item2 = GameObject.Find(text + "/02custom");
			m_Custom.Add(item2);
			item2 = GameObject.Find(text + "/02custom/custom");
			m_CustomRarity.Add(item2.GetComponent<UITexturePattern>());
			item2 = GameObject.Find(text + "/02custom/customIcon");
			m_CustomSprite.Add(item2.GetComponent<UISprite>());
			GameObject gameObject4 = GameObject.Find(text + "/iconpersonage");
			m_CharacterGO.Add(gameObject4);
			m_Character.Add(gameObject4.GetComponent<UISprite>());
			gameObject4 = GameObject.Find(text + "/kartIcon");
			m_KartGO.Add(gameObject4);
			m_Kart.Add(gameObject4.GetComponent<UISprite>());
			m_Stars.Add(new List<GameObject>());
			for (int m = 0; m < 3; m++)
			{
				string text2 = text + "/02star" + (m + 1);
				GameObject item3 = GameObject.Find(text2);
				m_Stars[l].Add(item3);
			}
		}
		GameObject gameObject5 = GameObject.Find(base.gameObject.name + "/Panel/Anchor_Center/LabelTime");
		if ((bool)gameObject5)
		{
			LapTimes = gameObject5.GetComponent<UILabel>();
		}
		for (int n = 0; n < 3; n++)
		{
			GameObject gameObject6 = GameObject.Find(base.gameObject.name + "/Panel/Anchor_Center/RecordLap" + (n + 1));
			if ((bool)gameObject6)
			{
				Records.Add(gameObject6);
			}
		}
		m_iIndexToFillPlayerData = 0;
		m_fElapsedTime = Time.time;
	}

	public virtual void FillStats(PlayerData[] pPlayerData)
	{
		DifficultySprite.ChangeTexture((int)Singleton<GameConfigurator>.Instance.Difficulty);
		if (Singleton<GameConfigurator>.Instance.ChampionShipData != null)
		{
			ChampionshipIcon.ChangeTexture(Singleton<GameConfigurator>.Instance.ChampionShipData.Index);
		}
		if (Singleton<GameConfigurator>.Instance.ChampionShipData != null)
		{
			string arg = Singleton<GameConfigurator>.Instance.ChampionShipData.TracksName[Singleton<GameConfigurator>.Instance.CurrentTrackIndex];
			TrackName.text = string.Format(Localization.instance.Get("HUD_DYN_ENDTIMETRIAL_RESULTS"), arg);
		}
		PlayerData playerData = pPlayerData[0];
		if (CharacterName != null)
		{
			CharacterName[m_iIndexToFillPlayerData].text = Singleton<GameSaveManager>.Instance.GetPseudo();
		}
		foreach (CharacterCarac oCharacter in m_oCharacterList)
		{
			if (oCharacter.Owner == playerData.Character)
			{
				m_Character[m_iIndexToFillPlayerData].spriteName = oCharacter.spriteName;
				break;
			}
		}
		foreach (KartCarac oKart in m_oKartList)
		{
			if (oKart.Owner == playerData.Kart)
			{
				m_Kart[m_iIndexToFillPlayerData].spriteName = oKart.spriteName;
				break;
			}
		}
		if (playerData.Custom.Contains("_Def"))
		{
			m_Custom[m_iIndexToFillPlayerData].SetActive(false);
		}
		else
		{
			m_Custom[m_iIndexToFillPlayerData].SetActive(true);
			foreach (KartCustom oKartCustom in m_oKartCustomList)
			{
				if (oKartCustom.name == playerData.Custom)
				{
					m_CustomSprite[m_iIndexToFillPlayerData].spriteName = oKartCustom.spriteName;
					m_CustomRarity[m_iIndexToFillPlayerData].ChangeTexture((int)oKartCustom.Rarity);
					break;
				}
			}
		}
		foreach (GameObject item in m_Stars[m_iIndexToFillPlayerData])
		{
			item.SetActive(false);
		}
		for (int i = 0; i < playerData.NbStars; i++)
		{
			m_Stars[m_iIndexToFillPlayerData][i].SetActive(true);
		}
		Kart humanKart = Singleton<GameManager>.Instance.GameMode.GetHumanKart();
		RcVehicleRaceStats raceStats = humanKart.RaceStats;
		if (RaceTime != null)
		{
			RaceTime[m_iIndexToFillPlayerData].text = TimeSpan.FromMilliseconds(raceStats.GetRaceTime()).FormatRaceTime();
		}
		ShowLapTime(raceStats);
		if (LogManager.Instance != null)
		{
			m_fElapsedTime = Time.time - m_fElapsedTime;
			if (Singleton<GameConfigurator>.Instance.CurrentTrackIndex >= 0 && Singleton<GameConfigurator>.Instance.CurrentTrackIndex < Singleton<GameConfigurator>.Instance.ChampionShipData.TracksName.Length)
			{
				RaceScoreData scoreData = Singleton<GameConfigurator>.Instance.RankingManager.GetScoreData(Singleton<GameManager>.Instance.GameMode.GetHumanPlayerVehicleId());
			}
		}
	}

	public virtual int GetNbPlayers()
	{
		return 1;
	}

	public virtual void ShowLapTime(RcVehicleRaceStats pStats)
	{
		if (LapTimes != null)
		{
			LapTimes.text = string.Empty;
			for (int i = 0; i < 3; i++)
			{
				UILabel lapTimes = LapTimes;
				string text = lapTimes.text;
				lapTimes.text = text + (i + 1) + "- " + TimeSpan.FromMilliseconds(pStats.GetLapTime(i)).FormatRaceTime();
				if (i < 2)
				{
					LapTimes.text += "\n";
				}
			}
		}
		string startScene = Singleton<GameConfigurator>.Instance.StartScene;
		int rpTime = 0;
		Singleton<GameSaveManager>.Instance.GetTimeTrialRecord(startScene, ref rpTime);
		if (pStats.GetRaceTime() < rpTime || rpTime < 0)
		{
			RecordGO[0].gameObject.SetActive(true);
		}
		else
		{
			RecordGO[0].gameObject.SetActive(false);
		}
		int rpTime2 = 0;
		Singleton<GameSaveManager>.Instance.GetTimeTrialBestTime(startScene, ref rpTime2);
		for (int j = 0; j < 3; j++)
		{
			if (Records[j] != null)
			{
				if (pStats.GetLapTime(j) == pStats.GetBestLapTime() && (pStats.GetBestLapTime() < rpTime2 || rpTime2 < 0))
				{
					Records[j].gameObject.SetActive(true);
				}
				else
				{
					Records[j].gameObject.SetActive(false);
				}
			}
		}
	}
}
