using System;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : Singleton<RewardManager>
{
	private static readonly List<int> _sMedalPrice = new List<int> { 100, 100, 200, 300 };

	private int _coins;

	private int _raceCoins;

	private List<string> _puzzlePieces;

	public List<string> _comicStrips;

	private List<string> _lockedCustoms;

	private List<Tuple<string, ERarity>> _unlockedCustoms;

	private List<string> _lockedHats;

	private List<Tuple<string, ERarity>> _unlockedHats;

	private List<string> _characters;

	private List<string> _karts;

	private List<Tuple<string, EDifficulty>> _lockedChampionShips;

	private List<Tuple<string, EDifficulty>> _unlockedChampionShips;

	private List<string> _advantages;

	private Tuple<int, string> _timeTrial;

	private int _playerRank = -1;

	private E_TimeTrialMedal _medal;

	private Tuple<string, ERarity, E_RewardType> _tradedReward;

	private bool _wonTimeTrialStar;

	private bool _wonEasyChampionShipStar;

	private bool _wonNormalChampionShipStar;

	private bool _wonHardChampionShipStar;

	private bool _wonEndStar;

	private E_RewardType m_eFirstRewardToGive;

	public Action<int> OnEarnCoins;

	public int PlayerRank
	{
		get
		{
			return _playerRank;
		}
		set
		{
			_playerRank = value;
		}
	}

	public E_TimeTrialMedal Medal
	{
		get
		{
			return _medal;
		}
		set
		{
			_medal = value;
		}
	}

	public int Coins
	{
		get
		{
			return _coins;
		}
	}

	public int RaceCoins
	{
		get
		{
			return _raceCoins;
		}
		set
		{
			_raceCoins = value;
		}
	}

	public void Reset()
	{
		_coins = 0;
		_raceCoins = 0;
		_puzzlePieces = new List<string>();
		_comicStrips = new List<string>();
		_unlockedHats = new List<Tuple<string, ERarity>>();
		_unlockedCustoms = new List<Tuple<string, ERarity>>();
		_lockedCustoms = new List<string>();
		_lockedHats = new List<string>();
		_characters = new List<string>();
		_karts = new List<string>();
		_lockedChampionShips = new List<Tuple<string, EDifficulty>>();
		_unlockedChampionShips = new List<Tuple<string, EDifficulty>>();
		_advantages = new List<string>();
		_timeTrial = new Tuple<int, string>(-1, string.Empty);
		_tradedReward = null;
		_wonTimeTrialStar = false;
		_wonEasyChampionShipStar = false;
		_wonNormalChampionShipStar = false;
		_wonHardChampionShipStar = false;
		_wonEndStar = false;
		m_eFirstRewardToGive = E_RewardType.Custom;
	}

	public void EarnCoins()
	{
		_coins++;
		_raceCoins++;
		if (OnEarnCoins != null)
		{
			OnEarnCoins(_coins);
		}
	}

	public void EarnCoins(int pQuantity)
	{
		_coins += pQuantity;
		_raceCoins += pQuantity;
	}

	public void UnlockPuzzlePiece(int pIndex)
	{
		string item = Singleton<GameConfigurator>.Instance.StartScene + "_" + pIndex;
		_puzzlePieces.Add(item);
	}

	public void CanUnlockPuzzlePieces(bool pCanUnlock)
	{
		if (!pCanUnlock)
		{
			_puzzlePieces.Clear();
		}
		if (_puzzlePieces.Count <= 0)
		{
			return;
		}
		string startScene = Singleton<GameConfigurator>.Instance.StartScene;
		for (int i = 0; i < 3; i++)
		{
			string text = startScene + "_" + i;
			bool flag = _puzzlePieces.Contains(text);
			if (!flag)
			{
				flag = Singleton<GameSaveManager>.Instance.IsPuzzlePieceUnlocked(text);
			}
			if (!flag)
			{
				return;
			}
		}
		_comicStrips.Add(startScene);
	}

	public void ShowHat(string pHat)
	{
		_lockedHats.Add(pHat);
	}

	public void UnlockHat(string pHat, ERarity pRarity)
	{
		_unlockedHats.Add(new Tuple<string, ERarity>(pHat, pRarity));
	}

	public void ShowCustom(string pCustom)
	{
		_lockedCustoms.Add(pCustom);
	}

	public void UnlockCustom(string pCustom, ERarity pRarity)
	{
		_unlockedCustoms.Add(new Tuple<string, ERarity>(pCustom, pRarity));
	}

	public void UnlockCharacter(ECharacter pCharacter)
	{
		_characters.Add(pCharacter.ToString());
	}

	public void UnlockKart(ECharacter pKart)
	{
		_karts.Add(pKart.ToString());
	}

	public void ShowChampionShip(string pChampionShip, EDifficulty pDifficulty)
	{
		_lockedChampionShips.Add(new Tuple<string, EDifficulty>(pChampionShip, pDifficulty));
	}

	public void UnlockChampionShip(string pChampionShip, EDifficulty pDifficulty)
	{
		_unlockedChampionShips.Add(new Tuple<string, EDifficulty>(pChampionShip, pDifficulty));
	}

	public bool ContainsShownChampionShip(string pChampionShip, EDifficulty pDifficulty)
	{
		return _lockedChampionShips.Contains(new Tuple<string, EDifficulty>(pChampionShip, pDifficulty));
	}

	public void ShowAdvantage(EAdvantage pAdvantage)
	{
		_advantages.Add(pAdvantage.ToString());
	}

	public void SetTimeTrialRank(int pRank)
	{
		_timeTrial = new Tuple<int, string>(pRank, Singleton<GameConfigurator>.Instance.StartScene);
	}

	public EMenus GetState()
	{
		if (_tradedReward != null)
		{
			if (_tradedReward.Item3 == E_RewardType.Custom)
			{
				E_UnlockableItemSate customState = Singleton<GameSaveManager>.Instance.GetCustomState(_tradedReward.Item1);
				if (customState == E_UnlockableItemSate.NewUnlocked || customState == E_UnlockableItemSate.Unlocked)
				{
					return EMenus.MENU_REWARDS_CUSTOMS_DUPLICATE;
				}
				return EMenus.MENU_REWARDS_CUSTOMS_UNLOCKED;
			}
			if (_tradedReward.Item3 == E_RewardType.Hat)
			{
				E_UnlockableItemSate hatState = Singleton<GameSaveManager>.Instance.GetHatState(_tradedReward.Item1);
				if (hatState == E_UnlockableItemSate.NewUnlocked || hatState == E_UnlockableItemSate.Unlocked)
				{
					return EMenus.MENU_REWARDS_HATS_DUPLICATE;
				}
				return EMenus.MENU_REWARDS_HATS_UNLOCKED;
			}
			return Singleton<GameConfigurator>.Instance.MenuToLaunch;
		}
		if (_timeTrial.Item1 >= 0)
		{
			return EMenus.MENU_REWARDS_TIME_TRIAL;
		}
		if (m_eFirstRewardToGive == E_RewardType.Custom && _unlockedCustoms.Count > 0)
		{
			E_UnlockableItemSate customState2 = Singleton<GameSaveManager>.Instance.GetCustomState(_unlockedCustoms[0].Item1);
			if (customState2 == E_UnlockableItemSate.NewUnlocked || customState2 == E_UnlockableItemSate.Unlocked)
			{
				return EMenus.MENU_REWARDS_CUSTOMS_DUPLICATE;
			}
			return EMenus.MENU_REWARDS_CUSTOMS_UNLOCKED;
		}
		if (m_eFirstRewardToGive == E_RewardType.Hat && _unlockedHats.Count > 0)
		{
			E_UnlockableItemSate hatState2 = Singleton<GameSaveManager>.Instance.GetHatState(_unlockedHats[0].Item1);
			if (hatState2 == E_UnlockableItemSate.NewUnlocked || hatState2 == E_UnlockableItemSate.Unlocked)
			{
				return EMenus.MENU_REWARDS_HATS_DUPLICATE;
			}
			return EMenus.MENU_REWARDS_HATS_UNLOCKED;
		}
		if (m_eFirstRewardToGive == E_RewardType.Kart && _karts.Count > 0)
		{
			return EMenus.MENU_REWARDS_KART;
		}
		if (_characters.Count > 0)
		{
			return EMenus.MENU_REWARDS_CHARACTER;
		}
		if (m_eFirstRewardToGive != E_RewardType.Custom && _unlockedCustoms.Count > 0)
		{
			E_UnlockableItemSate customState3 = Singleton<GameSaveManager>.Instance.GetCustomState(_unlockedCustoms[0].Item1);
			if (customState3 == E_UnlockableItemSate.NewUnlocked || customState3 == E_UnlockableItemSate.Unlocked)
			{
				return EMenus.MENU_REWARDS_CUSTOMS_DUPLICATE;
			}
			return EMenus.MENU_REWARDS_CUSTOMS_UNLOCKED;
		}
		if (m_eFirstRewardToGive != E_RewardType.Hat && _unlockedHats.Count > 0)
		{
			E_UnlockableItemSate hatState3 = Singleton<GameSaveManager>.Instance.GetHatState(_unlockedHats[0].Item1);
			if (hatState3 == E_UnlockableItemSate.NewUnlocked || hatState3 == E_UnlockableItemSate.Unlocked)
			{
				return EMenus.MENU_REWARDS_HATS_DUPLICATE;
			}
			return EMenus.MENU_REWARDS_HATS_UNLOCKED;
		}
		if (_lockedCustoms.Count > 0)
		{
			return EMenus.MENU_REWARDS_CUSTOMS_SHOWN;
		}
		if (_lockedHats.Count > 0)
		{
			return EMenus.MENU_REWARDS_HATS_SHOWN;
		}
		if (_advantages.Count > 0)
		{
			return EMenus.MENU_REWARDS_ADVANTAGES;
		}
		if (_comicStrips.Count > 0)
		{
			return EMenus.MENU_REWARDS_COMIC_STRIPS;
		}
		if (_lockedChampionShips.Count > 0)
		{
			return EMenus.MENU_REWARDS_CHAMPION_SHIP_SHOWN;
		}
		if (_unlockedChampionShips.Count > 0)
		{
			return EMenus.MENU_REWARDS_CHAMPION_SHIP_UNLOCKED;
		}
		if (_wonEasyChampionShipStar || _wonNormalChampionShipStar || _wonHardChampionShipStar || _wonTimeTrialStar || _wonEndStar)
		{
			return EMenus.MENU_REWARDS_STAR;
		}
		Singleton<GameSaveManager>.Instance.Save();
		Reset();
		return Singleton<GameConfigurator>.Instance.MenuToLaunch;
	}

	public E_Star PopStar()
	{
		if (_wonEasyChampionShipStar)
		{
			_wonEasyChampionShipStar = false;
			return E_Star.ChEasy;
		}
		if (_wonHardChampionShipStar)
		{
			_wonHardChampionShipStar = false;
			return E_Star.ChHard;
		}
		if (_wonTimeTrialStar)
		{
			_wonTimeTrialStar = false;
			return E_Star.TimeTrial;
		}
		if (_wonNormalChampionShipStar)
		{
			_wonNormalChampionShipStar = false;
			return E_Star.ChNormal;
		}
		if (_wonEndStar)
		{
			_wonEndStar = false;
			return E_Star.End;
		}
		return E_Star.None;
	}

	public string PopComicStrip()
	{
		return PopItem(ref _comicStrips);
	}

	public Tuple<string, ERarity> PopUnlockedCustom()
	{
		if (_tradedReward == null)
		{
			return PopItem(ref _unlockedCustoms);
		}
		Tuple<string, ERarity> result = new Tuple<string, ERarity>(_tradedReward.Item1, _tradedReward.Item2);
		_tradedReward = null;
		return result;
	}

	public Tuple<string, ERarity> PopUnlockedHat()
	{
		if (_tradedReward == null)
		{
			return PopItem(ref _unlockedHats);
		}
		Tuple<string, ERarity> result = new Tuple<string, ERarity>(_tradedReward.Item1, _tradedReward.Item2);
		_tradedReward = null;
		return result;
	}

	public ECharacter PopCharacter()
	{
		string value = PopItem(ref _characters);
		return (ECharacter)(int)Enum.Parse(typeof(ECharacter), value);
	}

	public ECharacter PopKart()
	{
		string value = PopItem(ref _karts);
		return (ECharacter)(int)Enum.Parse(typeof(ECharacter), value);
	}

	public Tuple<string, EDifficulty> PopLockedChampionShip()
	{
		return PopItem(ref _lockedChampionShips);
	}

	public Tuple<string, EDifficulty> PopUnlockedChampionShip()
	{
		return PopItem(ref _unlockedChampionShips);
	}

	public string[] PopLockedCustoms()
	{
		string[] array = new string[_lockedCustoms.Count];
		_lockedCustoms.CopyTo(array);
		_lockedCustoms.Clear();
		return array;
	}

	public string[] PopLockedHats()
	{
		string[] array = new string[_lockedHats.Count];
		_lockedHats.CopyTo(array);
		_lockedHats.Clear();
		return array;
	}

	public string[] PopAdvantage()
	{
		string[] array = new string[_advantages.Count];
		_advantages.CopyTo(array);
		_advantages.Clear();
		return array;
	}

	private string PopItem(ref List<string> rpItemList)
	{
		string result = rpItemList[0];
		rpItemList.RemoveAt(0);
		return result;
	}

	private Tuple<string, ERarity> PopItem(ref List<Tuple<string, ERarity>> rpItemList)
	{
		Tuple<string, ERarity> result = rpItemList[0];
		rpItemList.RemoveAt(0);
		return result;
	}

	private Tuple<string, EDifficulty> PopItem(ref List<Tuple<string, EDifficulty>> rpItemList)
	{
		Tuple<string, EDifficulty> result = rpItemList[0];
		rpItemList.RemoveAt(0);
		return result;
	}

	public Tuple<int, string> PopTimeTrial()
	{
		Tuple<int, string> timeTrial = _timeTrial;
		_timeTrial = new Tuple<int, string>(-1, string.Empty);
		return timeTrial;
	}

	public void CheckCoins()
	{
		UnityEngine.Object[] array = Resources.LoadAll("Reward", typeof(RewardBase));
		UnityEngine.Object[] array2 = array;
		foreach (UnityEngine.Object @object in array2)
		{
			if (@object is RewardBase)
			{
				((RewardBase)@object).GiveReward();
			}
		}
		Singleton<GameSaveManager>.Instance.EarnCoins(_coins, true, true);
		_raceCoins = 0;
		_coins = 0;
	}

	private void UnlockRandomItem(string pReward, E_RewardType pRewardType, ERarity pRarity)
	{
		switch (pRewardType)
		{
		case E_RewardType.Custom:
			UnlockCustom(pReward, pRarity);
			break;
		case E_RewardType.Hat:
			UnlockHat(pReward, pRarity);
			break;
		}
	}

	private void GetRandomItem(ERarity pRarity, ref string rpReward, ref E_RewardType rpRewardType)
	{
		List<string> availableHats = Singleton<GameSaveManager>.Instance.GetAvailableHats(pRarity, false);
		List<string> availableCustoms = Singleton<GameSaveManager>.Instance.GetAvailableCustoms(pRarity, false);
		int num = availableHats.Count + availableCustoms.Count;
		int num2 = Singleton<RandomManager>.Instance.Next(0, num - 1);
		if (num2 >= 0 && num2 < availableHats.Count)
		{
			rpReward = availableHats[num2];
			rpRewardType = E_RewardType.Hat;
		}
		else if (num2 >= availableHats.Count)
		{
			int index = num2 - availableHats.Count;
			rpReward = availableCustoms[index];
			rpRewardType = E_RewardType.Custom;
		}
	}

	private void GiveTimeTrialMedalCoins(int pLastMedal, int pNewMedal)
	{
		if (pLastMedal >= pNewMedal)
		{
			return;
		}
		for (int i = pLastMedal; i <= pNewMedal; i++)
		{
			if (i > 0)
			{
				EarnCoins(_sMedalPrice[i - 1]);
			}
		}
	}

	public void EndChallenge(bool pSuccess, string pReward, E_RewardType pRewardType, ERarity pRarity)
	{
		if (pSuccess)
		{
			switch (pRewardType)
			{
			case E_RewardType.Custom:
				UnlockCustom(pReward, pRarity);
				m_eFirstRewardToGive = E_RewardType.Custom;
				break;
			case E_RewardType.Hat:
				UnlockHat(pReward, pRarity);
				m_eFirstRewardToGive = E_RewardType.Hat;
				break;
			case E_RewardType.Kart:
				UnlockKart((ECharacter)(int)Enum.Parse(typeof(ECharacter), pReward));
				m_eFirstRewardToGive = E_RewardType.Kart;
				break;
			}
			if (Singleton<ChallengeManager>.Instance.IsMonday)
			{
				EarnCoins(500);
			}
			else
			{
				EarnCoins(200);
			}
		}
		CheckCoins();
	}

	public void EndSingleRace(int pRank, bool pIsSingleRace)
	{
		if (pIsSingleRace && pRank == 0)
		{
			EarnCoins(5);
		}
		EarnCoins(Singleton<GameConfigurator>.Instance.GameSettings.ChampionShipScores[pRank]);
		CheckCoins();
	}

	public void GiveChampionShipReward(int pFinalRank, int pNbFirstPlace)
	{
		if (pFinalRank == 0)
		{
			if (_characters.Count == 0)
			{
				ERarity championShipItemRarity = GetChampionShipItemRarity(pNbFirstPlace);
				string rpReward = string.Empty;
				E_RewardType rpRewardType = E_RewardType.Custom;
				GetRandomItem(championShipItemRarity, ref rpReward, ref rpRewardType);
				UnlockRandomItem(rpReward, rpRewardType, championShipItemRarity);
			}
			EarnCoins(20);
		}
		CheckCoins();
	}

	public void EndChampionShip(int pFinalRank, int pNbFirstPlace)
	{
		string name = Singleton<GameConfigurator>.Instance.ChampionShipData.name;
		EDifficulty difficulty = Singleton<GameConfigurator>.Instance.Difficulty;
		int rank = Singleton<GameSaveManager>.Instance.GetRank(name, difficulty);
		if (rank == -1 || pFinalRank < rank)
		{
			Singleton<GameSaveManager>.Instance.SetRank(name, pFinalRank, difficulty, true);
		}
	}

	public void EndTimeTrial(string pTrack, E_TimeTrialMedal pMedal, float pDiffTime)
	{
		if (pMedal != 0)
		{
			E_TimeTrialMedal medal = Singleton<GameSaveManager>.Instance.GetMedal(pTrack, false);
			if (pMedal != medal || (pMedal == E_TimeTrialMedal.Platinium && pDiffTime > 0f))
			{
				ERarity timeTrialItemRarity = GetTimeTrialItemRarity(pMedal);
				string rpReward = string.Empty;
				E_RewardType rpRewardType = E_RewardType.Custom;
				GetRandomItem(timeTrialItemRarity, ref rpReward, ref rpRewardType);
				UnlockRandomItem(rpReward, rpRewardType, timeTrialItemRarity);
				if (pMedal != medal)
				{
					GiveTimeTrialMedalCoins((int)medal, (int)pMedal);
					SetTimeTrialRank((int)pMedal);
				}
			}
		}
		if (pDiffTime >= 0f && pDiffTime <= 1f)
		{
			int num = (int)Mathf.Lerp(5f, 50f, 1f - pDiffTime);
			if (num > 0)
			{
				EarnCoins(num);
			}
		}
		CheckCoins();
	}

	public void GetChallengeReward(bool pIsMonday, ref string rpReward, ref E_RewardType rpRewardType, ref ERarity rpRarity)
	{
		rpRarity = ERarity.Unique;
		bool flag = !pIsMonday;
		if (!pIsMonday)
		{
			rpRarity = GetChallengeItemRarity();
		}
		else
		{
			List<ECharacter> availableKarts = Singleton<GameSaveManager>.Instance.GetAvailableKarts();
			if (availableKarts.Count > 0)
			{
				int index = Singleton<RandomManager>.Instance.Next(0, availableKarts.Count - 1);
				rpReward = availableKarts[index].ToString();
				rpRewardType = E_RewardType.Kart;
			}
			else
			{
				flag = true;
			}
		}
		if (flag)
		{
			GetRandomItem(rpRarity, ref rpReward, ref rpRewardType);
		}
	}

	public void TradeReward(ERarity pRarity)
	{
		string rpReward = string.Empty;
		E_RewardType rpRewardType = E_RewardType.Custom;
		GetRandomItem(pRarity, ref rpReward, ref rpRewardType);
		_tradedReward = new Tuple<string, ERarity, E_RewardType>(rpReward, pRarity, rpRewardType);
	}

	public ERarity GetChallengeItemRarity()
	{
		int num = Singleton<RandomManager>.Instance.Next(0, 99);
		if (num < 65)
		{
			return ERarity.Base;
		}
		return ERarity.Rare;
	}

	public ERarity GetTimeTrialItemRarity(E_TimeTrialMedal pMedal)
	{
		switch (pMedal)
		{
		case E_TimeTrialMedal.Bronze:
		case E_TimeTrialMedal.Silver:
			return ERarity.Base;
		case E_TimeTrialMedal.Gold:
			return ERarity.Rare;
		case E_TimeTrialMedal.Platinium:
			return ERarity.Unique;
		default:
			return ERarity.Base;
		}
	}

	public ERarity GetChampionShipItemRarity(int pNbFirstPlace)
	{
		switch (Singleton<GameConfigurator>.Instance.Difficulty)
		{
		case EDifficulty.EASY:
			if (pNbFirstPlace < 3)
			{
				return ERarity.Base;
			}
			return ERarity.Rare;
		case EDifficulty.NORMAL:
			if (pNbFirstPlace < 3)
			{
				return ERarity.Base;
			}
			if (pNbFirstPlace < 4)
			{
				return ERarity.Rare;
			}
			return ERarity.Unique;
		case EDifficulty.HARD:
			if (pNbFirstPlace < 4)
			{
				return ERarity.Rare;
			}
			return ERarity.Unique;
		default:
			return ERarity.Base;
		}
	}

	public void WinEasyChampionShipStar()
	{
		_wonEasyChampionShipStar = true;
	}

	public void WinNormalChampionShipStar()
	{
		_wonNormalChampionShipStar = true;
	}

	public void WinHardChampionShipStar()
	{
		_wonHardChampionShipStar = true;
	}

	public void WinTimeTrialStar()
	{
		_wonTimeTrialStar = true;
	}

	public void WinEndStar()
	{
		_wonEndStar = true;
	}

	public void GiveSharingReward()
	{
		ERarity pRarity = ERarity.Base;
		string rpReward = string.Empty;
		E_RewardType rpRewardType = E_RewardType.Custom;
		GetRandomItem(pRarity, ref rpReward, ref rpRewardType);
		UnlockRandomItem(rpReward, rpRewardType, pRarity);
	}
}
