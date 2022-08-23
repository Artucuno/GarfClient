using System;
using System.Collections.Generic;
using UnityEngine;

public class GameSaveManager : Singleton<GameSaveManager>
{
	private const string _SAVE = "progession";

	private const string _COINS = "coins";

	private const string _COINS_COLLECTED = "col_coins";

	private const string _PUZZLE_PIECE = "pp_";

	private const string _COMIC_STRIP = "ct_";

	private const string _TIME_TRIAL = "tt_";

	private const string _TIME_TRIAL_INFO = "tf_";

	private const string _TIME_TRIAL_MEDAL = "tm_";

	private const string _TIME_TRIAL_BEST_LAP = "tb_";

	private const string _HAT = "ht_";

	private const string _CUSTOM = "cm_";

	private const string _CHAMPION_SHIP_RECORDS = "cr_";

	private const string _CHARACTER = "ch_";

	private const string _KART = "kt_";

	private const string _CHAMPION_SHIP = "cs_";

	private const string _ADVANTAGE = "av_";

	private const string _ADVANTAGE_QUANTITY = "aq_";

	private const string _CHALLENGE = "chal";

	private const string _PSEUDO = "PSEUDO";

	private const string _CONFIG = "CONFIG";

	private const string _SHOWTUTO = "SHOWTUTO";

	private const string _FIRSTTIME = "FIRSTTIME";

	private const string _ASKRATING = "ASKRATING";

	private const string _ASKSHARING = "ASKSHARING";

	private GameSave _gameSave;

	private int _coins;

	private int _collectedCoins;

	private Dictionary<string, E_UnlockableItemSate> _comicStrips;

	private Dictionary<string, bool> _puzzlePieces;

	private Dictionary<string, int> _timeTrialRecords;

	private Dictionary<string, int> _timeTrialBestTimes;

	private Dictionary<string, E_TimeTrialMedal> _timeTrialMedals;

	private Dictionary<string, string> _timeTrialInfos;

	private Dictionary<string, E_UnlockableItemSate> _hats;

	private Dictionary<string, E_UnlockableItemSate> _customs;

	private Dictionary<string, int> _championShipsRecords;

	private Dictionary<string, E_UnlockableItemSate> _characters;

	private Dictionary<string, E_UnlockableItemSate> _karts;

	private Dictionary<string, E_UnlockableItemSate> _championsShips;

	private Dictionary<string, int> _advantagesQuantity;

	private Dictionary<string, E_UnlockableItemSate> _advantages;

	private string _challenge;

	private string _pseudo;

	private string _playerConfig;

	private bool _showTuto;

	private bool _firstTime;

	private int _askRating;

	private bool _askSharing;

	public void Init()
	{
		Load(out _coins, out _collectedCoins, out _comicStrips, out _puzzlePieces, out _timeTrialRecords, out _hats, out _customs, out _championShipsRecords, out _characters, out _karts, out _championsShips, out _timeTrialInfos, out _advantagesQuantity, out _advantages, out _timeTrialMedals, out _timeTrialBestTimes, out _challenge, out _pseudo, out _gameSave, out _playerConfig, out _showTuto, out _firstTime, out _askRating, out _askSharing);
		Save();
		CheckEasyChampionShipStar(false);
		CheckNormalChampionShipStar(false);
		CheckHardChampionShipStar(false);
		CheckTimeTrialStar(false);
		CheckEndStar(false);
	}

	public void Load(out int opCoins, out int opCollectedCoins, out Dictionary<string, E_UnlockableItemSate> opComicStrips, out Dictionary<string, bool> opPuzzlePieces, out Dictionary<string, int> opTimeTrialRecords, out Dictionary<string, E_UnlockableItemSate> opHats, out Dictionary<string, E_UnlockableItemSate> opCustoms, out Dictionary<string, int> opChampionShipsRecords, out Dictionary<string, E_UnlockableItemSate> opCharacters, out Dictionary<string, E_UnlockableItemSate> opKarts, out Dictionary<string, E_UnlockableItemSate> opChampionShips, out Dictionary<string, string> opTimeTrialInfos, out Dictionary<string, int> opAdvantagesQuantity, out Dictionary<string, E_UnlockableItemSate> opAdvantages, out Dictionary<string, E_TimeTrialMedal> opMedals, out Dictionary<string, int> opBestTimes, out string opChallenge, out string opPseudo, out GameSave opGameSave, out string opPlayerConfig, out bool opShowTuto, out bool opFirstTime, out int opAskRating, out bool opAskSharing)
	{
		opCoins = 0;
		opCollectedCoins = 0;
		opComicStrips = new Dictionary<string, E_UnlockableItemSate>();
		opPuzzlePieces = new Dictionary<string, bool>();
		opTimeTrialRecords = new Dictionary<string, int>();
		opHats = new Dictionary<string, E_UnlockableItemSate>();
		opCustoms = new Dictionary<string, E_UnlockableItemSate>();
		opChampionShipsRecords = new Dictionary<string, int>();
		opCharacters = new Dictionary<string, E_UnlockableItemSate>();
		opKarts = new Dictionary<string, E_UnlockableItemSate>();
		opChampionShips = new Dictionary<string, E_UnlockableItemSate>();
		opTimeTrialInfos = new Dictionary<string, string>();
		opAdvantagesQuantity = new Dictionary<string, int>();
		opAdvantages = new Dictionary<string, E_UnlockableItemSate>();
		opMedals = new Dictionary<string, E_TimeTrialMedal>();
		opBestTimes = new Dictionary<string, int>();
		opChallenge = string.Empty;
		opGameSave = GameSave.Load("progession");
		opPseudo = string.Empty;
		opPseudo = opGameSave.GetString("PSEUDO", string.Empty);
		string pDefaultValue = (opPlayerConfig = string.Format("{0};{1};{2};{3}", ECharacter.GARFIELD, ECharacter.HARRY, "None", "None"));
		opPlayerConfig = opGameSave.GetString("CONFIG", pDefaultValue);
		opShowTuto = opGameSave.GetBool("SHOWTUTO", true);
		opFirstTime = opGameSave.GetBool("FIRSTTIME", true);
		opAskRating = opGameSave.GetInt("ASKRATING", 0);
		opAskSharing = opGameSave.GetBool("ASKSHARING", true);
		opCoins = opGameSave.GetInt("coins", 400);
		opCollectedCoins = opGameSave.GetInt("col_coins", 0);
		UnityEngine.Object @object = Resources.Load("Tracks", typeof(TrackList));
		string[] tracks = ((TrackList)@object).Tracks;
		string[] array = tracks;
		foreach (string text in array)
		{
			string text2 = "ct_" + text;
			int @int = opGameSave.GetInt(text2, 0);
			if (!opComicStrips.ContainsKey(text2))
			{
				opComicStrips.Add(text2, (E_UnlockableItemSate)@int);
			}
			for (int j = 0; j < 3; j++)
			{
				string text3 = "pp_" + text + "_" + j;
				bool @bool = opGameSave.GetBool(text3, false);
				if (!opPuzzlePieces.ContainsKey(text3))
				{
					opPuzzlePieces.Add(text3, @bool);
				}
			}
			string text4 = "tt_" + text;
			int int2 = opGameSave.GetInt(text4, -1);
			if (!opTimeTrialRecords.ContainsKey(text4))
			{
				opTimeTrialRecords.Add(text4, int2);
			}
			string text5 = text4.Replace("tt_", "tf_");
			string pDefaultValue2 = string.Format("{0};{1};{2};{3}", ECharacter.NONE, ECharacter.NONE, "None", "None");
			string @string = opGameSave.GetString(text5, pDefaultValue2);
			if (!opTimeTrialInfos.ContainsKey(text5))
			{
				opTimeTrialInfos.Add(text5, @string);
			}
			string text6 = text4.Replace("tt_", "tb_");
			int int3 = opGameSave.GetInt(text6, -1);
			if (!opBestTimes.ContainsKey(text6))
			{
				opBestTimes.Add(text6, int3);
			}
			string text7 = text4.Replace("tt_", "tm_");
			int int4 = opGameSave.GetInt(text7, 0);
			opMedals.Add(text7, (E_TimeTrialMedal)int4);
		}
		UnityEngine.Object[] array2 = Resources.LoadAll("Hat", typeof(BonusCustom));
		UnityEngine.Object[] array3 = array2;
		foreach (UnityEngine.Object object2 in array3)
		{
			string text8 = "ht_" + object2.name;
			int int5 = opGameSave.GetInt(text8, (int)((BonusCustom)object2).State);
			if (!opHats.ContainsKey(text8))
			{
				opHats.Add(text8, (E_UnlockableItemSate)int5);
			}
		}
		UnityEngine.Object[] array4 = Resources.LoadAll("Kart", typeof(KartCustom));
		UnityEngine.Object[] array5 = array4;
		foreach (UnityEngine.Object object3 in array5)
		{
			string text9 = "cm_" + object3.name;
			int int6 = opGameSave.GetInt(text9, (int)((KartCustom)object3).State);
			if (!opCustoms.ContainsKey(text9))
			{
				opCustoms.Add(text9, (E_UnlockableItemSate)int6);
			}
		}
		UnityEngine.Object[] array6 = Resources.LoadAll("ChampionShip", typeof(ChampionShipData));
		string name = array6[0].name;
		UnityEngine.Object[] array7 = array6;
		foreach (UnityEngine.Object object4 in array7)
		{
			string text10 = "cr_" + object4.name;
			string text11 = text10 + "_Easy";
			int int7 = opGameSave.GetInt(text11, -1);
			if (!opChampionShipsRecords.ContainsKey(text11))
			{
				opChampionShipsRecords.Add(text11, int7);
			}
			string text12 = text10 + "_Normal";
			int int8 = opGameSave.GetInt(text12, -1);
			if (!opChampionShipsRecords.ContainsKey(text12))
			{
				opChampionShipsRecords.Add(text12, int8);
			}
			string text13 = text10 + "_Hard";
			int int9 = opGameSave.GetInt(text13, -1);
			if (!opChampionShipsRecords.ContainsKey(text13))
			{
				opChampionShipsRecords.Add(text13, int9);
			}
			string text14 = "cs_" + object4.name;
			string text15 = text14 + "_Easy";
			int int10 = opGameSave.GetInt(text15, (int)((ChampionShipData)object4).EasyState);
			if (!opChampionShips.ContainsKey(text15))
			{
				opChampionShips.Add(text15, (E_UnlockableItemSate)int10);
			}
			string text16 = text14 + "_Normal";
			int int11 = opGameSave.GetInt(text16, (int)((ChampionShipData)object4).NormalState);
			if (!opChampionShips.ContainsKey(text16))
			{
				opChampionShips.Add(text16, (E_UnlockableItemSate)int11);
			}
			string text17 = text14 + "_Hard";
			int int12 = opGameSave.GetInt(text17, (int)((ChampionShipData)object4).HardState);
			if (!opChampionShips.ContainsKey(text17))
			{
				opChampionShips.Add(text17, (E_UnlockableItemSate)int12);
			}
		}
		UnityEngine.Object[] array8 = Resources.LoadAll("Character", typeof(CharacterCarac));
		for (int n = 0; n < array8.Length; n++)
		{
			if (array8[n] is CharacterCarac)
			{
				CharacterCarac characterCarac = (CharacterCarac)array8[n];
				string text18 = characterCarac.Owner.ToString();
				string text19 = "ch_" + text18;
				int int13 = opGameSave.GetInt(text19, (int)characterCarac.State);
				if (!opCharacters.ContainsKey(text19))
				{
					opCharacters.Add(text19, (E_UnlockableItemSate)int13);
				}
			}
		}
		UnityEngine.Object[] array9 = Resources.LoadAll("Kart", typeof(KartCarac));
		for (int num = 0; num < array9.Length; num++)
		{
			if (array9[num] is KartCarac)
			{
				KartCarac kartCarac = (KartCarac)array9[num];
				string text20 = kartCarac.Owner.ToString();
				string text21 = "kt_" + text20;
				int int14 = opGameSave.GetInt(text21, (int)kartCarac.State);
				if (!opKarts.ContainsKey(text21))
				{
					opKarts.Add(text21, (E_UnlockableItemSate)int14);
				}
			}
		}
		UnityEngine.Object[] array10 = Resources.LoadAll("Advantages", typeof(AdvantageData));
		for (int num2 = 0; num2 < array10.Length; num2++)
		{
			if (array10[num2] is AdvantageData)
			{
				AdvantageData advantageData = (AdvantageData)array10[num2];
				string text22 = advantageData.AdvantageType.ToString();
				string text23 = "av_" + text22;
				int int15 = opGameSave.GetInt(text23, (int)advantageData.State);
				if (!opAdvantages.ContainsKey(text23))
				{
					opAdvantages.Add(text23, (E_UnlockableItemSate)int15);
				}
				string text24 = "aq_" + text22;
				int pDefaultValue3 = 0;
				if (advantageData.State != 0)
				{
					pDefaultValue3 = 2;
				}
				int int16 = opGameSave.GetInt(text24, pDefaultValue3);
				if (!opAdvantagesQuantity.ContainsKey(text24))
				{
					opAdvantagesQuantity.Add(text24, int16);
				}
			}
		}
		string pDefaultValue4 = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16}", new DateTime(2000, 1, 1).ToString("ddMMyyyy"), E_GameModeType.SINGLE.ToString(), EChallengeFirstObjective.FinishAtPosX.ToString(), ECharacter.NONE.ToString(), EChallengeSingleRaceObjective.EarnXCoins.ToString(), EChallengeChampionshipObjective.EarnXCoins.ToString(), E_TimeTrialMedal.None.ToString(), ECharacter.NONE.ToString(), ECharacter.NONE.ToString(), name, 0.ToString(), EChallengeState.NotPlayed, true.ToString(), EDifficulty.NORMAL, string.Empty, E_RewardType.Custom, ERarity.Base);
		opChallenge = opGameSave.GetString("chal", pDefaultValue4);
	}

	public void Save()
	{
		_gameSave.Save();
	}

	public void SetPseudo(string pPseudo, bool pSave)
	{
		_pseudo = pPseudo;
		_gameSave.SetString("PSEUDO", _pseudo);
		if (pSave)
		{
			Save();
		}
	}

	public string GetPseudo()
	{
		if (_pseudo.Equals(string.Empty))
		{
			return Localization.instance.Get("MENU_PLAYER");
		}
		return _pseudo;
	}

	public void SetPlayerConfig(ECharacter pCharacter, ECharacter pKart, string pCustom, string pHat, bool pSave)
	{
		string playerConfig = string.Format("{0};{1};{2};{3}", pCharacter, pKart, pCustom, pHat);
		_playerConfig = playerConfig;
		_gameSave.SetString("CONFIG", _playerConfig);
		if (pSave)
		{
			Save();
		}
	}

	public void GetPlayerConfig(ref ECharacter rpCharacter, ref ECharacter rpKart, ref string rpCustom, ref string rpHat)
	{
		string[] array = _playerConfig.Split(';');
		rpCharacter = (ECharacter)(int)Enum.Parse(typeof(ECharacter), array[0]);
		rpKart = (ECharacter)(int)Enum.Parse(typeof(ECharacter), array[1]);
		rpCustom = array[2];
		rpHat = array[3];
	}

	public void GetChallengeInfos(out string opDateTime, out E_GameModeType opGameModeType, out EChallengeFirstObjective opFirstObjective, out ECharacter opCharacterToBeat, out EChallengeSingleRaceObjective opSingleRaceObj, out EChallengeChampionshipObjective opChampionShipObj, out E_TimeTrialMedal opMedal, out ECharacter opImposedCharacter, out ECharacter opImposedKart, out string opChampionShipData, out int opTrack, out EChallengeState opState, out bool opIsMonday, out EDifficulty opDifficulty, out string opReward, out E_RewardType opRewardType, out ERarity opRarity)
	{
		string[] array = _challenge.Split(';');
		opDateTime = array[0];
		opGameModeType = (E_GameModeType)(int)Enum.Parse(typeof(E_GameModeType), array[1]);
		opFirstObjective = (EChallengeFirstObjective)(int)Enum.Parse(typeof(EChallengeFirstObjective), array[2]);
		opCharacterToBeat = (ECharacter)(int)Enum.Parse(typeof(ECharacter), array[3]);
		opSingleRaceObj = (EChallengeSingleRaceObjective)(int)Enum.Parse(typeof(EChallengeSingleRaceObjective), array[4]);
		opChampionShipObj = (EChallengeChampionshipObjective)(int)Enum.Parse(typeof(EChallengeChampionshipObjective), array[5]);
		opMedal = (E_TimeTrialMedal)(int)Enum.Parse(typeof(E_TimeTrialMedal), array[6]);
		opImposedCharacter = (ECharacter)(int)Enum.Parse(typeof(ECharacter), array[7]);
		opImposedKart = (ECharacter)(int)Enum.Parse(typeof(ECharacter), array[8]);
		opChampionShipData = array[9];
		opTrack = int.Parse(array[10]);
		opState = (EChallengeState)(int)Enum.Parse(typeof(EChallengeState), array[11]);
		opIsMonday = bool.Parse(array[12]);
		opDifficulty = (EDifficulty)(int)Enum.Parse(typeof(EDifficulty), array[13]);
		opReward = array[14];
		opRewardType = (E_RewardType)(int)Enum.Parse(typeof(E_RewardType), array[15]);
		opRarity = (ERarity)(int)Enum.Parse(typeof(ERarity), array[16]);
	}

	public void SetChallengeInfos(E_GameModeType pGameModeType, EChallengeFirstObjective pFirstObjective, ECharacter pCharacterToBeat, EChallengeSingleRaceObjective pSingleRaceObj, EChallengeChampionshipObjective pChampionShipObj, E_TimeTrialMedal pMedal, ECharacter pImposedCharacter, ECharacter pImposedKart, string pChampionShipData, int pTrack, EChallengeState pState, bool pIsMonday, EDifficulty pDifficulty, string pReward, E_RewardType pRewardType, ERarity pRarity, bool pSave)
	{
		_challenge = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16}", DateTime.Today.ToString("ddMMyyyy"), pGameModeType.ToString(), pFirstObjective.ToString(), pCharacterToBeat.ToString(), pSingleRaceObj.ToString(), pChampionShipObj.ToString(), pMedal.ToString(), pImposedCharacter.ToString(), pImposedKart.ToString(), pChampionShipData.ToString(), pTrack.ToString(), pState.ToString(), pIsMonday.ToString(), pDifficulty.ToString(), pReward, pRewardType.ToString(), pRarity);
		if (pSave)
		{
			_gameSave.SetString("chal", _challenge);
			Save();
		}
	}

	public bool GetShowTutorial()
	{
		return _showTuto;
	}

	public void SetShowTutorial(bool showTuto, bool pSave)
	{
		_showTuto = showTuto;
		_gameSave.SetBool("SHOWTUTO", _showTuto);
		if (pSave)
		{
			Save();
		}
	}

	public bool GetFirstTime()
	{
		if (_firstTime)
		{
			SetFirstTime(false, true);
			return true;
		}
		return false;
	}

	public void SetFirstTime(bool firstTime, bool pSave)
	{
		_firstTime = firstTime;
		_gameSave.SetBool("FIRSTTIME", _firstTime);
		if (pSave)
		{
			Save();
		}
	}

	public int GetAskRating()
	{
		return _askRating;
	}

	public void SetAskRating(int askRating, bool pSave)
	{
		_askRating = askRating;
		_gameSave.SetInt("ASKRATING", _askRating);
		if (pSave)
		{
			Save();
		}
	}

	public void AddAskRating(bool pSave)
	{
		if (_askRating != -1 && _askRating < 11)
		{
			_askRating++;
			_gameSave.SetInt("ASKRATING", _askRating);
			if (pSave)
			{
				Save();
			}
		}
	}

	public bool GetAskSharing()
	{
		return _askSharing;
	}

	public void SetAskSharing(bool askSharing, bool pSave)
	{
		_askSharing = askSharing;
		_gameSave.SetBool("ASKSHARING", _askSharing);
		if (pSave)
		{
			Save();
		}
	}

	public int GetCoins()
	{
		return _coins;
	}

	public int GetCollectedCoins()
	{
		return _collectedCoins;
	}

	private void SetCoins(int pValue, bool pSave)
	{
		_coins += pValue;
		_gameSave.SetInt("coins", _coins);
		if (pSave)
		{
			Save();
		}
	}

	public void EarnCoins(int pValue, bool pCollectCoins, bool pSave)
	{
		if (pCollectCoins)
		{
			_collectedCoins += pValue;
			_gameSave.SetInt("col_coins", _collectedCoins);
		}
		SetCoins(pValue, pSave);
	}

	public void SpendCoins(int pValue, bool pSave)
	{
		SetCoins(-pValue, pSave);
	}

	public void GetTimeTrialRecord(string pTrack, ref int rpTime)
	{
		rpTime = _timeTrialRecords["tt_" + pTrack];
	}

	public void GetTimeTrialRecord(string pTrack, ref string rpTime)
	{
		int rpTime2 = 0;
		GetTimeTrialRecord(pTrack, ref rpTime2);
		rpTime = TimeSpan.FromMilliseconds(rpTime2).FormatRaceTime();
	}

	private void GetTimeTrialInfos(string pTrack, ref ECharacter rpCharacter, ref ECharacter rpKart, ref string rpCustom, ref string rpHat)
	{
		string[] array = _timeTrialInfos["tf_" + pTrack].Split(';');
		rpCharacter = (ECharacter)(int)Enum.Parse(typeof(ECharacter), array[0]);
		rpKart = (ECharacter)(int)Enum.Parse(typeof(ECharacter), array[1]);
		rpCustom = array[2];
		rpHat = array[3];
	}

	public void GetTimeTrial(string pTrack, ref string pTime, ref ECharacter rpCharacter, ref ECharacter rpKart, ref string rpCustom, ref string rpHat)
	{
		GetTimeTrialRecord(pTrack, ref pTime);
		GetTimeTrialInfos(pTrack, ref rpCharacter, ref rpKart, ref rpCustom, ref rpHat);
	}

	public void GetTimeTrial(string pTrack, ref int pTime, ref ECharacter rpCharacter, ref ECharacter rpKart, ref string rpCustom, ref string rpHat)
	{
		GetTimeTrialRecord(pTrack, ref pTime);
		GetTimeTrialInfos(pTrack, ref rpCharacter, ref rpKart, ref rpCustom, ref rpHat);
	}

	private void SetTimeTrialRecord(string pTrack, int pTime)
	{
		string text = "tt_" + pTrack;
		_timeTrialRecords[text] = pTime;
		_gameSave.SetInt(text, pTime);
	}

	private void SetTimeTrialInfos(string pTrack, ECharacter pCharacter, ECharacter pKart, string pCustom, string pHat)
	{
		string text = "tf_" + pTrack;
		string text2 = string.Format("{0};{1};{2};{3}", pCharacter, pKart, pCustom, pHat);
		_timeTrialInfos[text] = text2;
		_gameSave.SetString(text, text2);
	}

	public void SetTimeTrial(string pTrack, int pTime, ECharacter pCharacter, ECharacter pKart, string pCustom, string pHat, bool pSave)
	{
		SetTimeTrialRecord(pTrack, pTime);
		SetTimeTrialInfos(pTrack, pCharacter, pKart, pCustom, pHat);
		if (pSave)
		{
			Save();
		}
	}

	public E_TimeTrialMedal GetMedal(string pTrack, bool bIsChallenge)
	{
		if (bIsChallenge && Singleton<ChallengeManager>.Instance.IsActive && Singleton<ChallengeManager>.Instance.GameMode == E_GameModeType.TIME_TRIAL)
		{
			return Singleton<ChallengeManager>.Instance.MedalImposed - 1;
		}
		string key = "tm_" + pTrack;
		return _timeTrialMedals[key];
	}

	public void SetMedal(string pTrack, E_TimeTrialMedal pMedal, bool pSave)
	{
		string text = "tm_" + pTrack;
		_timeTrialMedals[text] = pMedal;
		_gameSave.SetInt(text, (int)pMedal);
		if (pSave)
		{
			_gameSave.Save();
		}
		if (pMedal == E_TimeTrialMedal.Platinium && !Singleton<GameConfigurator>.Instance.PlayerConfig.HasTimeTrialStar)
		{
			CheckTimeTrialStar(true);
		}
	}

	public void GetTimeTrialBestTime(string pTrack, ref int rpTime)
	{
		rpTime = _timeTrialBestTimes["tb_" + pTrack];
	}

	public void GetTimeTrialBestTime(string pTrack, ref string rpTime)
	{
		int rpTime2 = 0;
		GetTimeTrialBestTime(pTrack, ref rpTime2);
		rpTime = TimeSpan.FromMilliseconds(rpTime2).FormatRaceTime();
	}

	public void SetTimeTrialBestTime(string pTrack, int pTime)
	{
		string text = "tb_" + pTrack;
		_timeTrialBestTimes[text] = pTime;
		_gameSave.SetInt(text, pTime);
	}

	public bool IsPuzzlePieceUnlocked(string pPiece)
	{
		return _puzzlePieces["pp_" + pPiece];
	}

	public void UnlockPuzzlePiece(string pPiece, bool pSave)
	{
		string text = "pp_" + pPiece;
		_puzzlePieces[text] = true;
		_gameSave.SetBool(text, true);
		if (pSave)
		{
			Save();
		}
	}

	public E_UnlockableItemSate GetComicStripState(string pComicStrip)
	{
		return _comicStrips["ct_" + pComicStrip];
	}

	public void SetComicStripState(string pComicStrip, E_UnlockableItemSate pState, bool pSave)
	{
		string text = "ct_" + pComicStrip;
		_comicStrips[text] = pState;
		_gameSave.SetInt(text, (int)pState);
		if (pSave)
		{
			Save();
		}
	}

	public E_UnlockableItemSate GetHatState(string pHat)
	{
		return _hats["ht_" + pHat];
	}

	public void SetHatState(string pHat, E_UnlockableItemSate pState, bool pSave)
	{
		string text = "ht_" + pHat;
		_hats[text] = pState;
		_gameSave.SetInt(text, (int)pState);
		if (pSave)
		{
			Save();
		}
	}

	public List<string> GetAvailableHats(ERarity pRarity, bool pIncludeDefault)
	{
		List<string> hats = GkUtils.GetHats(pRarity, pIncludeDefault);
		List<string> list = new List<string>();
		foreach (string item in hats)
		{
			if (GetHatState(item) != 0)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public E_UnlockableItemSate GetCustomState(string pCustom)
	{
		return _customs["cm_" + pCustom];
	}

	public void SetCustomState(string pCustom, E_UnlockableItemSate pState, bool pSave)
	{
		string text = "cm_" + pCustom;
		_customs[text] = pState;
		_gameSave.SetInt(text, (int)pState);
		if (pSave)
		{
			Save();
		}
	}

	public List<string> GetAvailableCustoms(ERarity pRarity, bool pIncludeDefault)
	{
		List<string> customs = GkUtils.GetCustoms(pRarity, pIncludeDefault);
		List<string> list = new List<string>();
		foreach (string item in customs)
		{
			if (GetCustomState(item) != 0)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public E_UnlockableItemSate GetCharacterState(ECharacter pCharacter)
	{
		return _characters["ch_" + pCharacter];
	}

	public void SetCharacterState(ECharacter pCharacter, E_UnlockableItemSate pState, bool pSave)
	{
		string text = "ch_" + pCharacter;
		_characters[text] = pState;
		_gameSave.SetInt(text, (int)pState);
		if (pSave)
		{
			Save();
		}
	}

	public E_UnlockableItemSate GetKartState(ECharacter pKart)
	{
		return _karts["kt_" + pKart];
	}

	public void SetKartState(ECharacter pKart, E_UnlockableItemSate pState, bool pSave)
	{
		string text = "kt_" + pKart;
		_karts[text] = pState;
		_gameSave.SetInt(text, (int)pState);
		if (pSave)
		{
			Save();
		}
	}

	public List<ECharacter> GetAvailableKarts()
	{
		List<ECharacter> karts = GkUtils.GetKarts();
		List<ECharacter> list = new List<ECharacter>();
		foreach (ECharacter item in karts)
		{
			if (GetKartState(item) == E_UnlockableItemSate.Locked)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public string DifficultyToString(EDifficulty pDifficulty)
	{
		switch (pDifficulty)
		{
		case EDifficulty.EASY:
			return "_Easy";
		case EDifficulty.NORMAL:
			return "_Normal";
		case EDifficulty.HARD:
			return "_Hard";
		default:
			return "_Normal";
		}
	}

	public int GetRank(string pChampionShip, EDifficulty pDifficulty)
	{
		return _championShipsRecords["cr_" + pChampionShip + DifficultyToString(pDifficulty)];
	}

	public void SetRank(string pChampionShip, int pRank, EDifficulty pDifficulty, bool pSave)
	{
		string text = DifficultyToString(pDifficulty);
		string text2 = "cr_" + pChampionShip + text;
		_championShipsRecords[text2] = pRank;
		_gameSave.SetInt(text2, pRank);
		if (pSave)
		{
			Save();
		}
		if (pRank == 0)
		{
			if (pDifficulty == EDifficulty.EASY && !Singleton<GameConfigurator>.Instance.PlayerConfig.HasEasyChampionShipStar)
			{
				CheckEasyChampionShipStar(true);
			}
			else if (pDifficulty == EDifficulty.NORMAL && !Singleton<GameConfigurator>.Instance.PlayerConfig.HasNormalChampionShipStar)
			{
				CheckNormalChampionShipStar(true);
			}
			else if (pDifficulty == EDifficulty.HARD && !Singleton<GameConfigurator>.Instance.PlayerConfig.HasHardChampionShipStar)
			{
				CheckHardChampionShipStar(true);
			}
		}
	}

	public E_UnlockableItemSate GetChampionShipState(string pChampionShip, EDifficulty pDifficulty)
	{
		string text = DifficultyToString(pDifficulty);
		string key = "cs_" + pChampionShip + text;
		return _championsShips[key];
	}

	public void SetChampionShipState(string pChampionShip, EDifficulty pDifficulty, E_UnlockableItemSate pState, bool pSave)
	{
		string text = DifficultyToString(pDifficulty);
		string text2 = "cs_" + pChampionShip + text;
		_championsShips[text2] = pState;
		_gameSave.SetInt(text2, (int)pState);
		if (pSave)
		{
			Save();
		}
	}

	public int GetAdvantageQuantity(EAdvantage pAdvantage)
	{
		string key = "aq_" + pAdvantage;
		return _advantagesQuantity[key];
	}

	private void IncrementAdvantageQuantity(EAdvantage pAdvantage, int pQuantity, bool pSave)
	{
		string text = "aq_" + pAdvantage;
		Dictionary<string, int> advantagesQuantity;
		Dictionary<string, int> dictionary = (advantagesQuantity = _advantagesQuantity);
		string key;
		string key2 = (key = text);
		int num = advantagesQuantity[key];
		dictionary[key2] = num + pQuantity;
		_gameSave.SetInt(text, _advantagesQuantity[text]);
		if (pSave)
		{
			Save();
		}
	}

	public void SetAdvantageQuantity(EAdvantage pAdvantage, int pQuantity, bool pSave)
	{
		string text = "aq_" + pAdvantage;
		_advantagesQuantity[text] = pQuantity;
		_gameSave.SetInt(text, _advantagesQuantity[text]);
		if (pSave)
		{
			Save();
		}
	}

	public void EarnAdvantage(EAdvantage pAdvantage, int pQuantity, bool pSave)
	{
		IncrementAdvantageQuantity(pAdvantage, pQuantity, pSave);
	}

	public void UseAdvantage(EAdvantage pAdvantage, int pQuantity, bool pSave)
	{
		IncrementAdvantageQuantity(pAdvantage, -pQuantity, pSave);
	}

	public E_UnlockableItemSate GetAdvantageState(EAdvantage pAdvantage)
	{
		return _advantages["av_" + pAdvantage];
	}

	public void SetAdvantageState(EAdvantage pAdvantage, E_UnlockableItemSate pState, bool pSave)
	{
		string text = "av_" + pAdvantage;
		_advantages[text] = pState;
		_gameSave.SetInt(text, (int)pState);
		if (pSave)
		{
			Save();
		}
	}

	private bool CheckChampionShipStar(string pDifficulty)
	{
		int num = 0;
		foreach (KeyValuePair<string, int> championShipsRecord in _championShipsRecords)
		{
			if (championShipsRecord.Key.EndsWith(pDifficulty) && championShipsRecord.Value == 0)
			{
				num++;
			}
		}
		return num == 4;
	}

	public void CheckEasyChampionShipStar(bool pCheckNew)
	{
		bool flag = CheckChampionShipStar("_Easy");
		if (pCheckNew && !Singleton<GameConfigurator>.Instance.PlayerConfig.HasEasyChampionShipStar && flag)
		{
			Singleton<RewardManager>.Instance.WinEasyChampionShipStar();
		}
		Singleton<GameConfigurator>.Instance.PlayerConfig.HasEasyChampionShipStar = flag;
	}

	public void CheckNormalChampionShipStar(bool pCheckNew)
	{
		bool flag = CheckChampionShipStar("_Normal");
		if (pCheckNew && !Singleton<GameConfigurator>.Instance.PlayerConfig.HasNormalChampionShipStar && flag)
		{
			Singleton<RewardManager>.Instance.WinNormalChampionShipStar();
		}
		Singleton<GameConfigurator>.Instance.PlayerConfig.HasNormalChampionShipStar = flag;
	}

	public void CheckHardChampionShipStar(bool pCheckNew)
	{
		bool flag = CheckChampionShipStar("_Hard");
		if (pCheckNew && !Singleton<GameConfigurator>.Instance.PlayerConfig.HasHardChampionShipStar && flag)
		{
			Singleton<RewardManager>.Instance.WinHardChampionShipStar();
		}
		Singleton<GameConfigurator>.Instance.PlayerConfig.HasHardChampionShipStar = flag;
	}

	public void CheckTimeTrialStar(bool pCheckNew)
	{
		int num = 0;
		foreach (KeyValuePair<string, E_TimeTrialMedal> timeTrialMedal in _timeTrialMedals)
		{
			if (timeTrialMedal.Value == E_TimeTrialMedal.Platinium)
			{
				num++;
			}
		}
		bool flag = num == 16;
		if (pCheckNew && !Singleton<GameConfigurator>.Instance.PlayerConfig.HasTimeTrialStar && flag)
		{
			Singleton<RewardManager>.Instance.WinTimeTrialStar();
		}
		Singleton<GameConfigurator>.Instance.PlayerConfig.HasTimeTrialStar = flag;
	}

	public void CheckEndStar(bool pCheckNew)
	{
		bool flag = Singleton<GameConfigurator>.Instance.PlayerConfig.HasEasyChampionShipStar && Singleton<GameConfigurator>.Instance.PlayerConfig.HasNormalChampionShipStar && Singleton<GameConfigurator>.Instance.PlayerConfig.HasHardChampionShipStar && Singleton<GameConfigurator>.Instance.PlayerConfig.HasTimeTrialStar;
		if (flag)
		{
			foreach (KeyValuePair<string, bool> puzzlePiece in _puzzlePieces)
			{
				flag = flag && puzzlePiece.Value;
			}
			foreach (KeyValuePair<string, E_UnlockableItemSate> hat in _hats)
			{
				flag = flag && (hat.Value == E_UnlockableItemSate.NewUnlocked || hat.Value == E_UnlockableItemSate.Unlocked);
			}
			foreach (KeyValuePair<string, E_UnlockableItemSate> custom in _customs)
			{
				flag = flag && (custom.Value == E_UnlockableItemSate.NewUnlocked || custom.Value == E_UnlockableItemSate.Unlocked);
			}
			foreach (KeyValuePair<string, E_UnlockableItemSate> kart in _karts)
			{
				flag = flag && (kart.Value == E_UnlockableItemSate.NewUnlocked || kart.Value == E_UnlockableItemSate.Unlocked);
			}
		}
		if (pCheckNew && !Singleton<GameConfigurator>.Instance.PlayerConfig.HasEndStar && flag)
		{
			Singleton<RewardManager>.Instance.WinEndStar();
		}
		Singleton<GameConfigurator>.Instance.PlayerConfig.HasEndStar = flag;
	}
}
