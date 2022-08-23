using System;
using UnityEngine;

public class ChallengeManager : Singleton<ChallengeManager>
{
	public const int FinishPosNoMonday = 2;

	public const int FinishPosMonday = 1;

	public const int FinishPosForAllMonday = 1;

	public const int FinishPosForAllNoMonday = 2;

	public const int EarnCoinsNoMonday = 5;

	public const int EarnCoinsMonday = 10;

	public const float TimerToShowFailedScreen = 2f;

	private bool m_bIsMonday;

	private EChallengeChampionshipObjective m_eChampionshipObjective;

	private EChallengeFirstObjective m_eFirstObjective;

	private EChallengeSingleRaceObjective m_eSingleRaceObjective;

	private E_GameModeType m_eGameMode;

	private ECharacter m_eCharacterToBeat;

	private ECharacter m_eCharacterImposed;

	private ECharacter m_eKartImposed;

	private BonusCustom m_pHatImposed;

	private KartCustom m_pCustoImposed;

	private int m_iPosToFinish;

	private int m_iPosToFinishForAll;

	private int m_iCoinsToEarn;

	private bool m_bIsChallengeActive;

	private int m_iTrackImposed;

	private ChampionShipData m_pChampionshipImposed;

	private E_TimeTrialMedal m_eMedalImposed;

	private EChallengeState m_eState;

	private float m_iTimer;

	private string m_sReward;

	private E_RewardType m_eRewardType;

	private EDifficulty m_eDifficulty;

	private ERarity m_eRarity;

	private bool m_bForceInit;

	private bool m_bForceMonday;

	private bool m_bSuccesFirstObjective;

	private bool m_bSuccesSecondObjective;

	private bool m_bCurrentFailed;

	public bool IsMonday
	{
		get
		{
			return m_bIsMonday;
		}
	}

	public bool IsActive
	{
		get
		{
			return m_bIsChallengeActive;
		}
	}

	public E_GameModeType GameMode
	{
		get
		{
			return m_eGameMode;
		}
	}

	public bool Success
	{
		get
		{
			return m_eState == EChallengeState.Succeeded;
		}
	}

	public ChampionShipData ChampionshipData
	{
		get
		{
			return m_pChampionshipImposed;
		}
	}

	public EDifficulty Difficulty
	{
		get
		{
			return m_eDifficulty;
		}
	}

	public string Reward
	{
		get
		{
			return m_sReward;
		}
	}

	public E_RewardType RewardType
	{
		get
		{
			return m_eRewardType;
		}
	}

	public ERarity RewardRarity
	{
		get
		{
			return m_eRarity;
		}
	}

	public int TrackIndex
	{
		get
		{
			return m_iTrackImposed;
		}
	}

	public bool SuccessFirstObjective
	{
		get
		{
			return m_bSuccesFirstObjective;
		}
	}

	public bool SuccessSecondObjective
	{
		get
		{
			return m_bSuccesSecondObjective;
		}
	}

	public E_TimeTrialMedal MedalImposed
	{
		get
		{
			return m_eMedalImposed;
		}
	}

	public bool AlreadyPlayed
	{
		get
		{
			return m_eState != EChallengeState.NotPlayed;
		}
	}

	public bool CurrentSuccess
	{
		get
		{
			return !m_bCurrentFailed;
		}
	}

	public void ForceInit(bool _Monday)
	{
		m_bForceMonday = _Monday;
		m_bForceInit = true;
		Init();
	}

	public void Init()
	{
		string text = DateTime.Today.ToString("ddMMyyyy");
		bool flag = false;
		string opDateTime;
		string opChampionShipData;
		Singleton<GameSaveManager>.Instance.GetChallengeInfos(out opDateTime, out m_eGameMode, out m_eFirstObjective, out m_eCharacterToBeat, out m_eSingleRaceObjective, out m_eChampionshipObjective, out m_eMedalImposed, out m_eCharacterImposed, out m_eKartImposed, out opChampionShipData, out m_iTrackImposed, out m_eState, out m_bIsMonday, out m_eDifficulty, out m_sReward, out m_eRewardType, out m_eRarity);
		if (opDateTime != text || m_bForceInit)
		{
			flag = true;
			m_bForceInit = false;
		}
		if (flag)
		{
			m_bIsMonday = DateTime.Now.DayOfWeek == DayOfWeek.Monday || m_bForceMonday;
			m_eGameMode = (E_GameModeType)Singleton<RandomManager>.Instance.Next((int)GameManager.FirstRealGameMode, (int)GameManager.LastRealGameMode);
			m_eDifficulty = (m_bIsMonday ? EDifficulty.HARD : EDifficulty.NORMAL);
			Singleton<RewardManager>.Instance.GetChallengeReward(m_bIsMonday, ref m_sReward, ref m_eRewardType, ref m_eRarity);
		}
		if (m_eGameMode != E_GameModeType.TIME_TRIAL)
		{
			if (flag)
			{
				m_eFirstObjective = (EChallengeFirstObjective)UnityEngine.Random.Range(0, Enum.GetValues(typeof(EChallengeFirstObjective)).Length);
			}
			if (m_eFirstObjective == EChallengeFirstObjective.FinishAtPosX)
			{
				m_iPosToFinish = (m_bIsMonday ? 1 : 2);
			}
			else if (m_eFirstObjective == EChallengeFirstObjective.FinishBeforeX && flag)
			{
				m_eCharacterToBeat = (ECharacter)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ECharacter)).Length - 1);
			}
			if (m_eGameMode == E_GameModeType.SINGLE)
			{
				if (flag)
				{
					m_eSingleRaceObjective = (EChallengeSingleRaceObjective)UnityEngine.Random.Range(0, Enum.GetValues(typeof(EChallengeSingleRaceObjective)).Length);
				}
			}
			else if (m_eGameMode == E_GameModeType.CHAMPIONSHIP && flag)
			{
				m_eChampionshipObjective = (EChallengeChampionshipObjective)UnityEngine.Random.Range(0, Enum.GetValues(typeof(EChallengeChampionshipObjective)).Length);
			}
			if ((m_eGameMode == E_GameModeType.SINGLE && m_eSingleRaceObjective == EChallengeSingleRaceObjective.EarnXCoins) || (m_eGameMode == E_GameModeType.CHAMPIONSHIP && m_eChampionshipObjective == EChallengeChampionshipObjective.EarnXCoins))
			{
				m_iCoinsToEarn = ((!m_bIsMonday) ? 5 : 10);
			}
			else if (m_eChampionshipObjective == EChallengeChampionshipObjective.FinishAllAtPosX)
			{
				m_iPosToFinishForAll = (m_bIsMonday ? 1 : 2);
			}
		}
		else if (m_bIsMonday)
		{
			m_eMedalImposed = E_TimeTrialMedal.Gold;
		}
		else if (flag)
		{
			m_eMedalImposed = (E_TimeTrialMedal)UnityEngine.Random.Range(1, Enum.GetValues(typeof(E_TimeTrialMedal)).Length - 1);
		}
		if (m_bIsMonday || (m_eGameMode != E_GameModeType.TIME_TRIAL && m_eFirstObjective == EChallengeFirstObjective.FinishBeforeX))
		{
			if (flag)
			{
				if (m_eGameMode != E_GameModeType.TIME_TRIAL && m_eFirstObjective == EChallengeFirstObjective.FinishBeforeX)
				{
					ChooseOppositeCharacter();
				}
				else
				{
					m_eCharacterImposed = (ECharacter)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ECharacter)).Length - 1);
				}
				m_eKartImposed = (ECharacter)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ECharacter)).Length - 1);
			}
			UnityEngine.Object[] array = Resources.LoadAll("Kart");
			UnityEngine.Object[] array2 = array;
			foreach (UnityEngine.Object @object in array2)
			{
				if (!(@object is GameObject))
				{
					continue;
				}
				GameObject gameObject = (GameObject)@object;
				if (!gameObject.name.Contains("_Def"))
				{
					continue;
				}
				KartCustom component = gameObject.GetComponent<KartCustom>();
				if (component != null)
				{
					ECharacter character = component.Character;
					if (character == m_eKartImposed)
					{
						m_pCustoImposed = component;
						break;
					}
				}
			}
			UnityEngine.Object[] array3 = Resources.LoadAll("Hat");
			UnityEngine.Object[] array4 = array3;
			foreach (UnityEngine.Object object2 in array4)
			{
				if (!(object2 is GameObject))
				{
					continue;
				}
				GameObject gameObject2 = (GameObject)object2;
				if (!gameObject2.name.Contains("_Def"))
				{
					continue;
				}
				BonusCustom component2 = gameObject2.GetComponent<BonusCustom>();
				if (component2 != null)
				{
					ECharacter character2 = component2.Character;
					if (character2 == m_eCharacterImposed)
					{
						m_pHatImposed = component2;
						break;
					}
				}
			}
		}
		if (flag)
		{
			m_pChampionshipImposed = null;
			int num = 0;
			while (m_pChampionshipImposed == null)
			{
				num++;
				UnityEngine.Object[] array5 = Resources.LoadAll("ChampionShip", typeof(ChampionShipData));
				int num2 = UnityEngine.Random.Range(0, array5.Length);
				if (!(array5[num2] is ChampionShipData))
				{
					continue;
				}
				ChampionShipData championShipData = (ChampionShipData)array5[num2];
				if ((m_eDifficulty == EDifficulty.NORMAL && (championShipData.NormalState == E_UnlockableItemSate.NewUnlocked || championShipData.NormalState == E_UnlockableItemSate.Unlocked)) || (m_eDifficulty == EDifficulty.HARD && (championShipData.HardState == E_UnlockableItemSate.NewUnlocked || championShipData.HardState == E_UnlockableItemSate.Unlocked)))
				{
					m_pChampionshipImposed = (ChampionShipData)array5[num2];
					if (m_eGameMode != E_GameModeType.CHAMPIONSHIP)
					{
						m_iTrackImposed = UnityEngine.Random.Range(0, m_pChampionshipImposed.Tracks.Length);
					}
					else
					{
						m_iTrackImposed = 0;
					}
				}
				if (num >= 20)
				{
					m_pChampionshipImposed = (ChampionShipData)array5[0];
					if (m_eGameMode != E_GameModeType.CHAMPIONSHIP)
					{
						m_iTrackImposed = UnityEngine.Random.Range(0, m_pChampionshipImposed.Tracks.Length);
					}
					else
					{
						m_iTrackImposed = 0;
					}
				}
			}
		}
		else
		{
			m_pChampionshipImposed = (ChampionShipData)Resources.Load("ChampionShip/" + opChampionShipData, typeof(ChampionShipData));
		}
		m_pChampionshipImposed.Localize();
		if (flag)
		{
			m_eState = EChallengeState.NotPlayed;
			Singleton<GameSaveManager>.Instance.SetChallengeInfos(m_eGameMode, m_eFirstObjective, m_eCharacterToBeat, m_eSingleRaceObjective, m_eChampionshipObjective, m_eMedalImposed, m_eCharacterImposed, m_eKartImposed, m_pChampionshipImposed.name, m_iTrackImposed, m_eState, m_bIsMonday, m_eDifficulty, m_sReward, m_eRewardType, m_eRarity, true);
		}
		m_iTimer = 0f;
	}

	public void SetTried()
	{
		if (m_eState == EChallengeState.NotPlayed)
		{
			m_eState = EChallengeState.Failed;
		}
	}

	public void Launch()
	{
		m_bIsChallengeActive = true;
		m_bCurrentFailed = false;
		m_iTimer = 0f;
		m_bSuccesFirstObjective = false;
		m_bSuccesSecondObjective = false;
		if ((m_bIsMonday && m_eGameMode != E_GameModeType.TIME_TRIAL) || (m_eGameMode != E_GameModeType.TIME_TRIAL && m_eFirstObjective == EChallengeFirstObjective.FinishBeforeX))
		{
			Singleton<GameConfigurator>.Instance.PlayerConfig.Character = m_eCharacterImposed;
			Singleton<GameConfigurator>.Instance.PlayerConfig.Kart = m_eKartImposed;
			Singleton<GameConfigurator>.Instance.PlayerConfig.m_oHat = m_pHatImposed;
			Singleton<GameConfigurator>.Instance.PlayerConfig.m_oKartCustom = m_pCustoImposed;
		}
		Singleton<GameConfigurator>.Instance.StartScene = m_pChampionshipImposed.Tracks[m_iTrackImposed];
		Singleton<GameConfigurator>.Instance.CurrentTrackIndex = m_iTrackImposed;
		Singleton<GameConfigurator>.Instance.SetChampionshipData(m_pChampionshipImposed, false);
		Singleton<GameConfigurator>.Instance.GameModeType = m_eGameMode;
		Singleton<GameConfigurator>.Instance.Difficulty = m_eDifficulty;
		Singleton<GameSaveManager>.Instance.SetChallengeInfos(m_eGameMode, m_eFirstObjective, m_eCharacterToBeat, m_eSingleRaceObjective, m_eChampionshipObjective, m_eMedalImposed, m_eCharacterImposed, m_eKartImposed, m_pChampionshipImposed.name, m_iTrackImposed, m_eState, m_bIsMonday, m_eDifficulty, m_sReward, m_eRewardType, m_eRarity, true);
		if ((m_bIsMonday && m_eGameMode != E_GameModeType.TIME_TRIAL) || (m_eGameMode != E_GameModeType.TIME_TRIAL && m_eFirstObjective == EChallengeFirstObjective.FinishBeforeX))
		{
			LoadingManager.LoadLevel(Singleton<GameConfigurator>.Instance.StartScene);
			return;
		}
		MenuEntryPoint component = GameObject.Find("MenuEntryPoint").GetComponent<MenuEntryPoint>();
		component.SetState(EMenus.MENU_SELECT_KART, 2);
	}

	public void DeActivate()
	{
		m_bIsChallengeActive = false;
	}

	public void Update()
	{
		if (!m_bCurrentFailed || !(m_iTimer >= 0f))
		{
			return;
		}
		m_iTimer += Time.deltaTime;
		if (m_iTimer >= 2f)
		{
			if (Singleton<GameManager>.Instance.GameMode != null)
			{
				Singleton<GameManager>.Instance.GameMode.FailedChallenge();
			}
			m_iTimer = -1f;
		}
	}

	public void CheckSuccess()
	{
		if (m_bCurrentFailed)
		{
			return;
		}
		if (m_eGameMode == E_GameModeType.TIME_TRIAL)
		{
			if (Singleton<GameManager>.Instance.GameMode is TimeTrialGameMode)
			{
				E_TimeTrialMedal eMedalImposed = m_eMedalImposed;
				if (!TimeTrialGameMode.BeatTime(eMedalImposed))
				{
					ImmediateFailed();
					m_bSuccesFirstObjective = false;
					return;
				}
				m_bSuccesFirstObjective = true;
			}
		}
		else
		{
			m_bSuccesFirstObjective = true;
			m_bSuccesSecondObjective = true;
			bool flag = false;
			int num = -1;
			Kart humanKart = Singleton<GameManager>.Instance.GameMode.GetHumanKart();
			if (humanKart != null)
			{
				RcVehicleRaceStats raceStats = humanKart.RaceStats;
				num = raceStats.GetRank();
			}
			if (m_eGameMode == E_GameModeType.SINGLE || Singleton<GameConfigurator>.Instance.CurrentTrackIndex == 3)
			{
				if (m_eFirstObjective == EChallengeFirstObjective.FinishBeforeX)
				{
					Kart kart = Singleton<GameManager>.Instance.GameMode.GetKart(m_eCharacterToBeat, true);
					if (kart != null)
					{
						RcVehicleRaceStats raceStats2 = kart.RaceStats;
						if (raceStats2.GetRank() < num)
						{
							flag = true;
							m_bSuccesFirstObjective = false;
						}
					}
				}
				else if (m_eFirstObjective == EChallengeFirstObjective.FinishAtPosX && m_iPosToFinish < num)
				{
					flag = true;
					m_bSuccesFirstObjective = false;
				}
			}
			if (((m_eGameMode == E_GameModeType.SINGLE && m_eSingleRaceObjective == EChallengeSingleRaceObjective.EarnXCoins) || (m_eGameMode == E_GameModeType.CHAMPIONSHIP && m_eChampionshipObjective == EChallengeChampionshipObjective.EarnXCoins)) && Singleton<RewardManager>.Instance.RaceCoins < m_iCoinsToEarn)
			{
				flag = true;
				m_bSuccesSecondObjective = false;
			}
			if (m_eGameMode == E_GameModeType.CHAMPIONSHIP && m_eChampionshipObjective == EChallengeChampionshipObjective.FinishAllAtPosX && m_iPosToFinishForAll < num)
			{
				flag = true;
				m_bSuccesSecondObjective = false;
			}
			if (flag)
			{
				ImmediateFailed();
				return;
			}
		}
		if (m_eGameMode != E_GameModeType.CHAMPIONSHIP || (m_eGameMode == E_GameModeType.CHAMPIONSHIP && Singleton<GameConfigurator>.Instance.CurrentTrackIndex == 3))
		{
			m_eState = EChallengeState.Succeeded;
			Singleton<RewardManager>.Instance.EndChallenge(true, m_sReward, m_eRewardType, m_eRarity);
			Singleton<GameSaveManager>.Instance.SetChallengeInfos(m_eGameMode, m_eFirstObjective, m_eCharacterToBeat, m_eSingleRaceObjective, m_eChampionshipObjective, m_eMedalImposed, m_eCharacterImposed, m_eKartImposed, m_pChampionshipImposed.name, m_iTrackImposed, m_eState, m_bIsMonday, m_eDifficulty, m_sReward, m_eRewardType, m_eRarity, true);
		}
	}

	public void ImmediateFailed()
	{
		m_bCurrentFailed = true;
		if (m_eState != EChallengeState.Succeeded)
		{
			m_eState = EChallengeState.Failed;
		}
		Singleton<GameSaveManager>.Instance.SetChallengeInfos(m_eGameMode, m_eFirstObjective, m_eCharacterToBeat, m_eSingleRaceObjective, m_eChampionshipObjective, m_eMedalImposed, m_eCharacterImposed, m_eKartImposed, m_pChampionshipImposed.name, m_iTrackImposed, m_eState, m_bIsMonday, m_eDifficulty, m_sReward, m_eRewardType, m_eRarity, true);
	}

	public void Notify(EChallengeSingleRaceObjective _Objective)
	{
		if (m_eGameMode == E_GameModeType.SINGLE && m_eSingleRaceObjective == _Objective)
		{
			ImmediateFailed();
		}
	}

	public void GetLocalizedObjectives(out string _First, out string _Second)
	{
		_First = null;
		_Second = null;
		if (m_eGameMode == E_GameModeType.TIME_TRIAL)
		{
			_First = Localization.instance.Get("MENU_CHALLENGE_TIMETRIAL") + " " + Localization.instance.Get("MENU_REWARDS_MEDAL" + (int)m_eMedalImposed);
			return;
		}
		if (m_eFirstObjective == EChallengeFirstObjective.FinishAtPosX)
		{
			_First = string.Format(Localization.instance.Get("MENU_CHALLENGE_FIRST_OBJ2"), m_iPosToFinish + 1);
		}
		else if (m_eFirstObjective == EChallengeFirstObjective.FinishBeforeX)
		{
			if (m_eGameMode == E_GameModeType.SINGLE)
			{
				_First = string.Format(Localization.instance.Get("MENU_CHALLENGE_FIRST_OBJ1"), m_eCharacterToBeat.ToString());
			}
			else
			{
				_First = string.Format(Localization.instance.Get("MENU_CHALLENGE_FIRST_OBJ1B"), m_eCharacterToBeat.ToString());
			}
		}
		if (m_eGameMode == E_GameModeType.SINGLE)
		{
			switch (m_eSingleRaceObjective)
			{
			case EChallengeSingleRaceObjective.EarnXCoins:
				_Second = string.Format(Localization.instance.Get("MENU_CHALLENGE_SECOND_OBJ1"), m_iCoinsToEarn);
				break;
			case EChallengeSingleRaceObjective.NoBonus:
				_Second = Localization.instance.Get("MENU_CHALLENGE_SECOND_OBJ3");
				break;
			case EChallengeSingleRaceObjective.NoDrift:
				_Second = Localization.instance.Get("MENU_CHALLENGE_SECOND_OBJ2");
				break;
			case EChallengeSingleRaceObjective.NoFall:
				_Second = Localization.instance.Get("MENU_CHALLENGE_SECOND_OBJ4");
				break;
			}
		}
		else if (m_eGameMode == E_GameModeType.CHAMPIONSHIP)
		{
			switch (m_eChampionshipObjective)
			{
			case EChallengeChampionshipObjective.EarnXCoins:
				_Second = string.Format(Localization.instance.Get("MENU_CHALLENGE_SECOND_OBJ1_2"), m_iCoinsToEarn);
				break;
			case EChallengeChampionshipObjective.FinishAllAtPosX:
				_Second = string.Format(Localization.instance.Get("MENU_CHALLENGE_SECOND_OBJ5"), m_iPosToFinishForAll + 1);
				break;
			}
		}
	}

	public bool GetSomeoneToBeat()
	{
		return m_eFirstObjective == EChallengeFirstObjective.FinishBeforeX;
	}

	public ECharacter GetCharacterToBeat()
	{
		return m_eCharacterToBeat;
	}

	public void ChooseOppositeCharacter()
	{
		switch (m_eCharacterToBeat)
		{
		case ECharacter.ODIE:
		case ECharacter.ARLENE:
		case ECharacter.NERMAL:
			m_eCharacterImposed = ECharacter.GARFIELD;
			break;
		case ECharacter.JON:
			m_eCharacterImposed = ECharacter.LIZ;
			break;
		case ECharacter.LIZ:
			m_eCharacterImposed = ECharacter.JON;
			break;
		case ECharacter.HARRY:
			m_eCharacterImposed = ECharacter.SQUEAK;
			break;
		case ECharacter.SQUEAK:
			m_eCharacterImposed = ECharacter.HARRY;
			break;
		case ECharacter.GARFIELD:
			switch (UnityEngine.Random.Range(0, 3))
			{
			case 0:
				m_eCharacterImposed = ECharacter.ARLENE;
				break;
			case 1:
				m_eCharacterImposed = ECharacter.ODIE;
				break;
			case 2:
				m_eCharacterImposed = ECharacter.NERMAL;
				break;
			}
			break;
		}
	}
}
