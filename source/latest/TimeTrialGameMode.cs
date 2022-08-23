using System;
using UnityEngine;

public class TimeTrialGameMode : InGameGameMode
{
	private GameObject UFO;

	public override void Dispose()
	{
		base.Dispose();
		ChanceDispatcher chanceDispatcher = _chanceDispatcher;
		chanceDispatcher.OnCreatePlayer = (Action<PlayerData, int>)Delegate.Remove(chanceDispatcher.OnCreatePlayer, new Action<PlayerData, int>(AddPlayerData));
	}

	public override void Awake()
	{
		base.Awake();
		ChanceDispatcher chanceDispatcher = _chanceDispatcher;
		chanceDispatcher.OnCreatePlayer = (Action<PlayerData, int>)Delegate.Combine(chanceDispatcher.OnCreatePlayer, new Action<PlayerData, int>(AddPlayerData));
	}

	protected override void Start()
	{
		base.Start();
		Singleton<BonusMgr>.Instance.Reset();
	}

	public override void CreatePlayers()
	{
		if (DebugMgr.Instance != null && !DebugMgr.Instance.dbgData.RandomPlayer)
		{
			DebugMgr.Instance.LoadDefaultPlayer(0, this);
		}
		else
		{
			ECharacter character = Singleton<GameConfigurator>.Instance.PlayerConfig.Character;
			ECharacter kart = Singleton<GameConfigurator>.Instance.PlayerConfig.Kart;
			GameObject gameObject = (GameObject)Resources.Load("Hat/" + Singleton<GameConfigurator>.Instance.PlayerConfig.m_oHat.name);
			GameObject gameObject2 = (GameObject)Resources.Load("Kart/" + Singleton<GameConfigurator>.Instance.PlayerConfig.m_oKartCustom.name);
			int nbStars = Singleton<GameConfigurator>.Instance.PlayerConfig.NbStars;
			Singleton<GameSaveManager>.Instance.EarnAdvantage(EAdvantage.BoostStart, 1, false);
			Singleton<GameConfigurator>.Instance.PlayerConfig.SelectAdvantage(EAdvantage.BoostStart);
			CreatePlayer(character, kart, gameObject2.name, gameObject.name, nbStars, 0, true, false);
			_chanceDispatcher.AddPlayerData(character, kart, gameObject2, gameObject, nbStars, 0);
		}
		CreateAIPaths();
		SubscribeRaceEnd();
		UnityEngine.Object @object = Resources.Load("TimeTrial_Ufo");
		if (Network.isServer)
		{
			UFO = (GameObject)Network.Instantiate(@object, Vector3.zero, Quaternion.identity, 0);
		}
		else if (Network.peerType == NetworkPeerType.Disconnected)
		{
			UFO = (GameObject)UnityEngine.Object.Instantiate(@object);
		}
	}

	public static bool GetMedalBeaten(ref E_TimeTrialMedal _FromMedal, int record, out float _DiffTime)
	{
		_DiffTime = -1f;
		Kart humanKart = Singleton<GameManager>.Instance.GameMode.GetHumanKart();
		RcVehicleRaceStats raceStats = humanKart.RaceStats;
		GameObject gameObject = GameObject.Find("Race");
		TimeTrialConfig component = gameObject.GetComponent<TimeTrialConfig>();
		int[] array = new int[5] { 0, component.Bronze, component.Silver, component.Gold, component.Platinium };
		E_TimeTrialMedal e_TimeTrialMedal = E_TimeTrialMedal.None;
		if (raceStats.GetRaceTime() < array[4])
		{
			e_TimeTrialMedal = E_TimeTrialMedal.Platinium;
		}
		else
		{
			for (int num = 4; num > (int)_FromMedal; num--)
			{
				if (raceStats.GetRaceTime() <= array[num])
				{
					e_TimeTrialMedal = (E_TimeTrialMedal)num;
					break;
				}
			}
		}
		if (e_TimeTrialMedal > _FromMedal)
		{
			_FromMedal = e_TimeTrialMedal;
			return true;
		}
		if (_FromMedal != E_TimeTrialMedal.Platinium)
		{
			_DiffTime = (float)(raceStats.GetRaceTime() - array[(int)(_FromMedal + 1)]) / 1000f;
		}
		else if (_FromMedal == E_TimeTrialMedal.Platinium)
		{
			_DiffTime = (float)(record - raceStats.GetRaceTime()) / 1000f;
		}
		return false;
	}

	public static bool BeatTime(E_TimeTrialMedal _Medal)
	{
		GameObject gameObject = GameObject.Find("Race");
		TimeTrialConfig component = gameObject.GetComponent<TimeTrialConfig>();
		int[] array = new int[5] { 0, component.Bronze, component.Silver, component.Gold, component.Platinium };
		Kart humanKart = Singleton<GameManager>.Instance.GameMode.GetHumanKart();
		RcVehicleRaceStats raceStats = humanKart.RaceStats;
		if (_Medal == E_TimeTrialMedal.Platinium)
		{
			string startScene = Singleton<GameConfigurator>.Instance.StartScene;
			int rpTime = 0;
			Singleton<GameSaveManager>.Instance.GetTimeTrialRecord(startScene, ref rpTime);
			return raceStats.GetRaceTime() < rpTime;
		}
		return raceStats.GetRaceTime() < array[(int)_Medal];
	}

	protected override void StateChange(E_GameState pNewState)
	{
		base.StateChange(pNewState);
		if (pNewState == E_GameState.Race)
		{
			UFO.GetComponent<TimeTrialUFO>().Launch();
		}
	}

	protected override void RaceEnd(RcVehicle pVehicle)
	{
		base.RaceEnd(pVehicle);
		base.Hud.HUDEndTimeTrial.FillStats(Singleton<GameConfigurator>.Instance.PlayerDataList);
		base.Hud.HUDEndTimeTrial2.FillStats(Singleton<GameConfigurator>.Instance.PlayerDataList);
		string startScene = Singleton<GameConfigurator>.Instance.StartScene;
		Kart humanKart = Singleton<GameManager>.Instance.GameMode.GetHumanKart();
		RcVehicleRaceStats raceStats = humanKart.RaceStats;
		PlayerData playerData = Singleton<GameConfigurator>.Instance.PlayerDataList[0];
		E_TimeTrialMedal _FromMedal = Singleton<GameSaveManager>.Instance.GetMedal(startScene, false);
		base.Hud.HUDFinish.FillRank(1);
		base.Hud.HUDTimeTrialResults.SetPreviousMedal(_FromMedal);
		int rpTime = 0;
		Singleton<GameSaveManager>.Instance.GetTimeTrialRecord(startScene, ref rpTime);
		float _DiffTime;
		bool medalBeaten = GetMedalBeaten(ref _FromMedal, rpTime, out _DiffTime);
		Singleton<RewardManager>.Instance.EndTimeTrial(startScene, _FromMedal, _DiffTime);
		if (medalBeaten)
		{
			Singleton<GameSaveManager>.Instance.SetMedal(startScene, _FromMedal, false);
			humanKart.KartSound.PlayVoice(KartSound.EVoices.Selection);
			base.Hud.HUDFinish.FillRank(0);
		}
		bool flag = raceStats.GetRaceTime() < rpTime || rpTime < 0;
		if (flag)
		{
			Singleton<GameSaveManager>.Instance.SetTimeTrial(startScene, raceStats.GetRaceTime(), playerData.Character, playerData.Kart, playerData.Custom, playerData.Hat, false);
			humanKart.KartSound.PlayVoice(KartSound.EVoices.Selection);
			base.Hud.HUDFinish.FillRank(0);
		}
		if ((bool)Singleton<GameManager>.Instance.SoundManager)
		{
			if (medalBeaten || (_FromMedal == E_TimeTrialMedal.Platinium && flag))
			{
				Singleton<GameManager>.Instance.SoundManager.PlaySound(ERaceSounds.FinishGood);
			}
			else
			{
				Singleton<GameManager>.Instance.SoundManager.PlaySound(ERaceSounds.FinishBad);
			}
		}
		int rpTime2 = 0;
		Singleton<GameSaveManager>.Instance.GetTimeTrialBestTime(startScene, ref rpTime2);
		if (raceStats.GetBestLapTime() < rpTime2 || rpTime2 < 0)
		{
			Singleton<GameSaveManager>.Instance.SetTimeTrialBestTime(startScene, raceStats.GetBestLapTime());
		}
		Singleton<RewardManager>.Instance.CanUnlockPuzzlePieces(true);
	}

	public override void PlaceVehiclesOnStartLine()
	{
		GameObject gameObject = GameObject.Find("Start");
		if (gameObject != null)
		{
			Kart humanKart = GetHumanKart();
			if (humanKart != null)
			{
				StartPositions[0] = gameObject.transform.GetChild(gameObject.transform.GetChildCount() - 1);
			}
		}
		UFO.GetComponent<TimeTrialUFO>().Place();
	}
}
