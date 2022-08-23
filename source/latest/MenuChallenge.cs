using System;
using UnityEngine;

public class MenuChallenge : AbstractMenu
{
	public UITexturePattern GameModeIcon;

	public UITexturePattern ChampionshipIcon;

	public UITexturePattern DifficultyIcon;

	public UISprite RewardIcon;

	public UILabel FirstObjective;

	public UILabel SecondObjective;

	public UITexturePattern RewardRarityIcon;

	public UILabel TrackName;

	public UILabel ChampionshipName;

	public UICheckbox MondayChecked;

	public UILabel ChallengeName;

	public UITexturePattern ChallengeState;

	public UITexturePattern ButtonNext;

	public GameObject ButtonNextGO;

	public int PriceRetry;

	public UITexture TrackScreenshot;

	private UISlicedSprite m_oGoMonday;

	private UISlicedSprite m_oGoNormalBackground;

	private UILabel m_oLabelRetry;

	public UITexturePattern SpriteChallenge;

	public override void Awake()
	{
		base.Awake();
		m_oGoMonday = ButtonNextGO.transform.Find("GoArrow").GetComponent<UISlicedSprite>();
		m_oGoNormalBackground = ButtonNextGO.transform.Find("Background").GetComponent<UISlicedSprite>();
		m_oLabelRetry = ButtonNextGO.transform.Find("LabelRecommencer").GetComponent<UILabel>();
	}

	public override void OnEnter()
	{
		base.OnEnter();
		Singleton<ChallengeManager>.Instance.ChampionshipData.Localize();
		if (Singleton<ChallengeManager>.Instance.GameMode == E_GameModeType.CHAMPIONSHIP)
		{
			GameModeIcon.ChangeTexture(0);
		}
		else if (Singleton<ChallengeManager>.Instance.GameMode == E_GameModeType.SINGLE)
		{
			GameModeIcon.ChangeTexture(1);
		}
		else if (Singleton<ChallengeManager>.Instance.GameMode == E_GameModeType.TIME_TRIAL)
		{
			GameModeIcon.ChangeTexture(2);
		}
		ChampionshipIcon.ChangeTexture(Singleton<ChallengeManager>.Instance.ChampionshipData.Index);
		DifficultyIcon.ChangeTexture((int)Singleton<ChallengeManager>.Instance.Difficulty);
		IconCarac iconCarac = null;
		if (Singleton<ChallengeManager>.Instance.RewardType == E_RewardType.Kart)
		{
			ECharacter index = (ECharacter)(int)Enum.Parse(typeof(ECharacter), Singleton<ChallengeManager>.Instance.Reward);
			iconCarac = (IconCarac)Resources.Load("Kart/" + Singleton<GameConfigurator>.Instance.PlayerConfig.KartPrefab[(int)index], typeof(IconCarac));
		}
		else
		{
			string text = string.Empty;
			if (Singleton<ChallengeManager>.Instance.RewardType == E_RewardType.Custom)
			{
				text = "Kart/";
			}
			else if (Singleton<ChallengeManager>.Instance.RewardType == E_RewardType.Hat)
			{
				text = "Hat/";
			}
			GameObject gameObject = (GameObject)Resources.Load(text + Singleton<ChallengeManager>.Instance.Reward);
			iconCarac = gameObject.GetComponent<IconCarac>();
		}
		RewardIcon.spriteName = iconCarac.spriteName;
		string _Second = string.Empty;
		string _First;
		Singleton<ChallengeManager>.Instance.GetLocalizedObjectives(out _First, out _Second);
		FirstObjective.text = _First;
		SecondObjective.text = _Second;
		RewardRarityIcon.ChangeTexture(Tricks.LogBase2((int)Singleton<ChallengeManager>.Instance.RewardRarity));
		if (Singleton<ChallengeManager>.Instance.GameMode == E_GameModeType.CHAMPIONSHIP)
		{
			TrackName.text = Singleton<ChallengeManager>.Instance.ChampionshipData.ChampionShipName;
		}
		else
		{
			TrackName.text = Singleton<ChallengeManager>.Instance.ChampionshipData.TracksName[Singleton<ChallengeManager>.Instance.TrackIndex];
		}
		ChampionshipName.text = Singleton<ChallengeManager>.Instance.ChampionshipData.ChampionShipName;
		string arg = ((!Singleton<ChallengeManager>.Instance.IsMonday) ? Localization.instance.Get("MENU_CHALLENGE_DAY") : Localization.instance.Get("MENU_CHALLENGE_MONDAY"));
		ChallengeName.text = string.Format(Localization.instance.Get("MENU_CHALLENGE_NAME"), arg);
		if ((bool)SpriteChallenge)
		{
			SpriteChallenge.ChangeTexture(Singleton<ChallengeManager>.Instance.IsMonday ? 1 : 0);
		}
		if (Singleton<ChallengeManager>.Instance.AlreadyPlayed)
		{
			ChallengeState.ChangeTexture((!Singleton<ChallengeManager>.Instance.Success) ? 1 : 0);
			m_oLabelRetry.enabled = true;
		}
		else
		{
			ChallengeState.gameObject.SetActive(false);
			m_oLabelRetry.enabled = false;
		}
		if (Singleton<ChallengeManager>.Instance.Success)
		{
			ButtonNextGO.SetActive(false);
		}
		else
		{
			ButtonNextGO.SetActive(true);
			if (Singleton<ChallengeManager>.Instance.IsMonday)
			{
				if (Singleton<ChallengeManager>.Instance.AlreadyPlayed)
				{
					ButtonNext.enabled = true;
					m_oGoNormalBackground.enabled = true;
					m_oGoMonday.enabled = false;
					ButtonNext.ChangeTexture(1);
				}
				else
				{
					ButtonNext.enabled = false;
					m_oGoNormalBackground.enabled = false;
					m_oGoMonday.enabled = true;
				}
			}
			else
			{
				ButtonNext.enabled = true;
				m_oGoNormalBackground.enabled = true;
				m_oGoMonday.enabled = false;
				ButtonNext.ChangeTexture(0);
			}
		}
		if (TrackScreenshot != null)
		{
			Resources.UnloadAsset(TrackScreenshot.mainTexture);
			TrackScreenshot.mainTexture = Resources.Load(Singleton<ChallengeManager>.Instance.ChampionshipData.Tracks[Singleton<ChallengeManager>.Instance.TrackIndex], typeof(Texture2D)) as Texture2D;
		}
	}

	public override void OnExit()
	{
		base.OnExit();
		Resources.UnloadAsset(TrackScreenshot.mainTexture);
	}

	public void OnNext()
	{
		if (Singleton<ChallengeManager>.Instance.IsMonday && Singleton<ChallengeManager>.Instance.AlreadyPlayed)
		{
			m_pMenuEntryPoint.ShowPurchasePopup(string.Format(Localization.instance.Get("MENU_POPUP_BUY_ITEM_CONFIRMATION_CHALLENGE"), PriceRetry), PriceRetry, PurchaseItem);
		}
		else
		{
			Singleton<ChallengeManager>.Instance.Launch();
		}
	}

	public void PurchaseItem(object oParam)
	{
		Singleton<ChallengeManager>.Instance.Launch();
	}

	public void OnRegenerate()
	{
		Singleton<ChallengeManager>.Instance.ForceInit(MondayChecked.isChecked);
	}
}
