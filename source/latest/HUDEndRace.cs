using System.Collections.Generic;
using UnityEngine;

public abstract class HUDEndRace : MonoBehaviour
{
	private float m_fInertia = 0.3f;

	private float m_fDelayCounter;

	private List<UILabel> LabelCharacter = new List<UILabel>();

	protected List<UILabel> LabelPoint = new List<UILabel>();

	private List<float> CurrentPointsAdded = new List<float>();

	protected List<int> PointsToAdd = new List<int>();

	protected List<int> BasePoints = new List<int>();

	private List<UITexturePattern> LabelPosition = new List<UITexturePattern>();

	private List<UITexturePattern> BackgroundPlace = new List<UITexturePattern>();

	private List<UITexturePattern> BackgroundPersonage = new List<UITexturePattern>();

	private List<UITexturePattern> m_CustomRarity = new List<UITexturePattern>();

	private List<UISprite> m_CustomSprite = new List<UISprite>();

	private List<GameObject> m_Custom = new List<GameObject>();

	private List<UITexturePattern> m_HatRarity = new List<UITexturePattern>();

	private List<UISprite> m_HatSprite = new List<UISprite>();

	private List<GameObject> m_Hat = new List<GameObject>();

	private List<UISprite> m_Character = new List<UISprite>();

	private List<UISprite> m_Kart = new List<UISprite>();

	private List<List<GameObject>> m_Stars = new List<List<GameObject>>();

	protected List<UISprite> m_Advantage = new List<UISprite>();

	private List<UITexturePattern> m_PuzzlePieces = new List<UITexturePattern>();

	public UITexturePattern DifficultySprite;

	public UITexturePattern ChampionshipIcon;

	protected UILabel LabelTitle;

	private List<CharacterCarac> m_oCharacterList = new List<CharacterCarac>();

	private List<BonusCustom> m_oHatList = new List<BonusCustom>();

	private List<KartCarac> m_oKartList = new List<KartCarac>();

	private List<KartCustom> m_oKartCustomList = new List<KartCustom>();

	private List<AdvantageData> m_oAdvantagesList = new List<AdvantageData>();

	private float m_fElapsedTime;

	public virtual void Init()
	{
		Object[] array = Resources.LoadAll("Character", typeof(CharacterCarac));
		Object[] array2 = array;
		foreach (Object @object in array2)
		{
			m_oCharacterList.Add((CharacterCarac)@object);
		}
		Object[] array3 = Resources.LoadAll("Hat", typeof(BonusCustom));
		Object[] array4 = array3;
		foreach (Object object2 in array4)
		{
			if (object2.name.Contains("_Def"))
			{
				m_oHatList.Insert(0, (BonusCustom)object2);
			}
			else
			{
				m_oHatList.Add((BonusCustom)object2);
			}
		}
		Object[] array5 = Resources.LoadAll("Kart", typeof(KartCarac));
		Object[] array6 = array5;
		foreach (Object object3 in array6)
		{
			m_oKartList.Add((KartCarac)object3);
		}
		Object[] array7 = Resources.LoadAll("Kart", typeof(KartCustom));
		Object[] array8 = array7;
		foreach (Object object4 in array8)
		{
			if (object4.name.Contains("_Def"))
			{
				m_oKartCustomList.Insert(0, (KartCustom)object4);
			}
			else
			{
				m_oKartCustomList.Add((KartCustom)object4);
			}
		}
		Object[] array9 = Resources.LoadAll("Advantages", typeof(AdvantageData));
		Object[] array10 = array9;
		foreach (Object object5 in array10)
		{
			m_oAdvantagesList.Add((AdvantageData)object5);
		}
		for (int n = 0; n < 6; n++)
		{
			string text = base.gameObject.name + "/Anchor_Center/Panel" + (n + 1) + "place";
			GameObject gameObject = GameObject.Find(text + "/02LabelName");
			LabelCharacter.Add(gameObject.GetComponent<UILabel>());
			GameObject gameObject2 = GameObject.Find(text + "/02LabelPts");
			LabelPoint.Add(gameObject2.GetComponent<UILabel>());
			PointsToAdd.Add(-1);
			CurrentPointsAdded.Add(0f);
			BasePoints.Add(0);
			LabelPosition.Add(GameObject.Find(text + "/02place").GetComponent<UITexturePattern>());
			if (LabelPosition[n] != null)
			{
				LabelPosition[n].ChangeTexture(0);
			}
			GameObject gameObject3 = GameObject.Find(text + "/02PersonagePlace");
			BackgroundPlace.Add(gameObject3.GetComponent<UITexturePattern>());
			gameObject3 = GameObject.Find(text + "/02Personage");
			BackgroundPersonage.Add(gameObject3.GetComponent<UITexturePattern>());
			GameObject item = GameObject.Find(text + "/02custom");
			m_Custom.Add(item);
			item = GameObject.Find(text + "/02custom/02custom");
			m_CustomRarity.Add(item.GetComponent<UITexturePattern>());
			item = GameObject.Find(text + "/02custom/02customIcon");
			m_CustomSprite.Add(item.GetComponent<UISprite>());
			GameObject item2 = GameObject.Find(text + "/02hat");
			m_Hat.Add(item2);
			item2 = GameObject.Find(text + "/02hat/02hat");
			m_HatRarity.Add(item2.GetComponent<UITexturePattern>());
			item2 = GameObject.Find(text + "/02hat/02hatIcon");
			m_HatSprite.Add(item2.GetComponent<UISprite>());
			GameObject gameObject4 = GameObject.Find(text + "/02iconpersonage");
			m_Character.Add(gameObject4.GetComponent<UISprite>());
			gameObject4 = GameObject.Find(text + "/02kartIcon");
			m_Kart.Add(gameObject4.GetComponent<UISprite>());
			m_Stars.Add(new List<GameObject>());
			for (int num = 0; num < 5; num++)
			{
				string text2 = text + "/stars/02star" + (num + 1);
				GameObject item3 = GameObject.Find(text2);
				m_Stars[n].Add(item3);
			}
			GameObject gameObject5 = GameObject.Find(text + "/02adv");
			m_Advantage.Add(gameObject5.GetComponent<UISprite>());
		}
		for (int num2 = 0; num2 < 3; num2++)
		{
			string text3 = base.gameObject.name + "/Anchor_TopRight/Puzzle" + (num2 + 1);
			GameObject gameObject6 = GameObject.Find(text3);
			m_PuzzlePieces.Add(gameObject6.GetComponent<UITexturePattern>());
		}
		GameObject gameObject7 = GameObject.Find(base.gameObject.name + "/Anchor_Center/TrackPres/Title");
		LabelTitle = gameObject7.GetComponent<UILabel>();
		DifficultySprite.ChangeTexture((int)Singleton<GameConfigurator>.Instance.Difficulty);
		if (Singleton<GameConfigurator>.Instance.ChampionShipData != null)
		{
			ChampionshipIcon.ChangeTexture(Singleton<GameConfigurator>.Instance.ChampionShipData.Index);
		}
		m_fElapsedTime = Time.time;
	}

	public virtual void FillPositions()
	{
		int trackIndex = GetTrackIndex();
		string text = Singleton<GameConfigurator>.Instance.ChampionShipData.Tracks[trackIndex];
		foreach (UITexturePattern item in BackgroundPersonage)
		{
			item.ChangeTexture(0);
		}
		foreach (UITexturePattern item2 in BackgroundPlace)
		{
			item2.ChangeTexture(0);
		}
		for (int i = 0; i < 3; i++)
		{
			string pPiece = text + "_" + i;
			m_PuzzlePieces[i].ChangeTexture(Singleton<GameSaveManager>.Instance.IsPuzzlePieceUnlocked(pPiece) ? 1 : 0);
		}
		PlayerData[] playerDataList = Singleton<GameConfigurator>.Instance.PlayerDataList;
		bool bEquality = false;
		int humanPlayerVehicleId = Singleton<GameManager>.Instance.GameMode.GetHumanPlayerVehicleId();
		for (int j = 0; j < Singleton<GameConfigurator>.Instance.RankingManager.RaceScoreCount(); j++)
		{
			int iKartIndex;
			int iScore;
			int iTotalScore;
			GetScoreInfos(j, out iKartIndex, out iScore, out iTotalScore, out bEquality);
			PlayerData playerData = null;
			for (int k = 0; k < playerDataList.Length; k++)
			{
				if (playerDataList[k].KartIndex == iKartIndex)
				{
					playerData = playerDataList[k];
					break;
				}
			}
			FillRank(playerData, iScore, iTotalScore, j, bEquality, humanPlayerVehicleId == playerData.KartIndex);
		}
		if (LogManager.Instance == null)
		{
			return;
		}
		m_fElapsedTime = Time.time - m_fElapsedTime;
		if (!(this is HUDChampionsShipRanking) && Singleton<GameConfigurator>.Instance.CurrentTrackIndex >= 0 && Singleton<GameConfigurator>.Instance.CurrentTrackIndex < Singleton<GameConfigurator>.Instance.ChampionShipData.TracksName.Length)
		{
			RaceScoreData scoreData = Singleton<GameConfigurator>.Instance.RankingManager.GetScoreData(Singleton<GameManager>.Instance.GameMode.GetHumanPlayerVehicleId());
			KartHumanController componentInChildren = Singleton<GameManager>.Instance.GameMode.GetHumanPlayer().GetComponentInChildren<KartHumanController>();
			if (!(componentInChildren != null))
			{
			}
		}
	}

	private void FillRank(PlayerData pData, int iScore, int iTotalScore, int pPosition, bool pEquality, bool bHighlight)
	{
		if (pData != null)
		{
			LabelCharacter[pPosition].text = pData.Character.ToString();
			foreach (CharacterCarac oCharacter in m_oCharacterList)
			{
				if (oCharacter.Owner == pData.Character)
				{
					m_Character[pPosition].spriteName = oCharacter.spriteName;
					break;
				}
			}
			foreach (KartCarac oKart in m_oKartList)
			{
				if (oKart.Owner == pData.Character)
				{
					m_Kart[pPosition].spriteName = oKart.spriteName;
					break;
				}
			}
			if (pData.Hat.Contains("_Def"))
			{
				m_Hat[pPosition].SetActive(false);
			}
			else
			{
				m_Hat[pPosition].SetActive(true);
				foreach (BonusCustom oHat in m_oHatList)
				{
					if (oHat.name == pData.Hat)
					{
						m_HatSprite[pPosition].spriteName = oHat.spriteName;
						m_HatRarity[pPosition].ChangeTexture((int)oHat.Rarity);
						break;
					}
				}
			}
			if (pData.Custom.Contains("_Def"))
			{
				m_Custom[pPosition].SetActive(false);
			}
			else
			{
				m_Custom[pPosition].SetActive(true);
				foreach (KartCustom oKartCustom in m_oKartCustomList)
				{
					if (oKartCustom.name == pData.Custom)
					{
						m_CustomSprite[pPosition].spriteName = oKartCustom.spriteName;
						m_CustomRarity[pPosition].ChangeTexture((int)oKartCustom.Rarity);
						break;
					}
				}
			}
			Kart kartWithVehicleId = Singleton<GameManager>.Instance.GameMode.GetKartWithVehicleId(pData.KartIndex);
			if ((bool)kartWithVehicleId)
			{
				RaceScoreData scoreData = Singleton<GameConfigurator>.Instance.RankingManager.GetScoreData(kartWithVehicleId.GetVehicleId());
				if (!scoreData.IsAI)
				{
					if (Network.peerType != 0)
					{
						LabelCharacter[pPosition].color = pData.CharacColor;
					}
					if (kartWithVehicleId.GetControlType() == RcVehicle.ControlType.Human)
					{
						LabelCharacter[pPosition].text = Singleton<GameSaveManager>.Instance.GetPseudo();
					}
					else
					{
						LabelCharacter[pPosition].text = pData.Pseudo;
					}
				}
				if (kartWithVehicleId.SelectedAdvantage != EAdvantage.None)
				{
					m_Advantage[pPosition].gameObject.SetActive(true);
					foreach (AdvantageData oAdvantages in m_oAdvantagesList)
					{
						if (oAdvantages.AdvantageType == kartWithVehicleId.SelectedAdvantage)
						{
							m_Advantage[pPosition].spriteName = oAdvantages.spriteName;
							break;
						}
					}
				}
				else
				{
					m_Advantage[pPosition].gameObject.SetActive(false);
				}
			}
			foreach (GameObject item in m_Stars[pPosition])
			{
				item.SetActive(false);
			}
			for (int i = 0; i < pData.NbStars; i++)
			{
				m_Stars[pPosition][i].SetActive(true);
			}
		}
		LabelPoint[pPosition].text = iTotalScore + " pts";
		PointsToAdd[pPosition] = iScore;
		CurrentPointsAdded[pPosition] = iTotalScore;
		BasePoints[pPosition] = iTotalScore;
		if (pEquality && LabelPosition[pPosition] != null)
		{
			LabelPosition[pPosition].ChangeTexture(1);
		}
		if ((bool)BackgroundPersonage[pPosition] && (bool)BackgroundPlace[pPosition])
		{
			if (bHighlight)
			{
				BackgroundPersonage[pPosition].ChangeTexture(2);
				BackgroundPlace[pPosition].ChangeTexture(2);
			}
			else
			{
				BackgroundPersonage[pPosition].ChangeTexture(pPosition % 2);
				BackgroundPlace[pPosition].ChangeTexture(pPosition % 2);
			}
		}
	}

	public abstract void GetScoreInfos(int iIndex, out int iKartIndex, out int iScore, out int iTotalScore, out bool bEquality);

	public virtual int GetTrackIndex()
	{
		return Singleton<GameConfigurator>.Instance.CurrentTrackIndex;
	}

	public virtual void Update()
	{
		float deltaTime = Time.deltaTime;
		if (PointsToAdd.Count > 0 && m_fDelayCounter < 1f)
		{
			m_fDelayCounter += deltaTime;
		}
		if (!(m_fDelayCounter >= 1f))
		{
			return;
		}
		for (int i = 0; i < PointsToAdd.Count; i++)
		{
			if (PointsToAdd[i] + BasePoints[i] != (int)(CurrentPointsAdded[i] + 0.5f))
			{
				CurrentPointsAdded[i] = Tricks.ComputeInertia(CurrentPointsAdded[i], PointsToAdd[i] + BasePoints[i], m_fInertia, deltaTime);
				LabelPoint[i].text = (int)(CurrentPointsAdded[i] + 0.5f) + " pts";
			}
		}
	}
}
