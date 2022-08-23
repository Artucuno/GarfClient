using System;
using System.Collections.Generic;
using UnityEngine;

public class ChampionShipGameMode : InGameGameMode
{
	private static int _nbFirstPlace;

	private NetworkMgr networkMgr;

	public override void Awake()
	{
		base.Awake();
		networkMgr = (NetworkMgr)UnityEngine.Object.FindObjectOfType(typeof(NetworkMgr));
		_gameStates[6] = base.gameObject.AddComponent<PodiumGameState>();
		GameState obj = _gameStates[6];
		obj.OnStateChanged = (Action<E_GameState>)Delegate.Combine(obj.OnStateChanged, new Action<E_GameState>(StateChange));
		_gameStates[6].gameMode = this;
		_gameStates[6].enabled = false;
		ChanceDispatcher chanceDispatcher = _chanceDispatcher;
		chanceDispatcher.OnCreatePlayer = (Action<PlayerData, int>)Delegate.Combine(chanceDispatcher.OnCreatePlayer, new Action<PlayerData, int>(AddPlayerData));
	}

	public override void Dispose()
	{
		base.Dispose();
		ChanceDispatcher chanceDispatcher = _chanceDispatcher;
		chanceDispatcher.OnCreatePlayer = (Action<PlayerData, int>)Delegate.Remove(chanceDispatcher.OnCreatePlayer, new Action<PlayerData, int>(AddPlayerData));
	}

	public override void CreatePlayers()
	{
		base.CreatePlayers();
		SubscribeRaceEnd();
	}

	protected override void RaceEnd(RcVehicle pVehicle)
	{
		base.RaceEnd(pVehicle);
		Singleton<RewardManager>.Instance.CanUnlockPuzzlePieces(true);
		Singleton<GameConfigurator>.Instance.CurrentTrackIndex++;
		int rank = GetHumanKart().RaceStats.GetRank();
		Singleton<RewardManager>.Instance.EndSingleRace(rank, false);
		PlayEndOfRaceSound(rank < 3);
		Kart kart = (Kart)pVehicle;
		if (kart.GetControlType() == RcVehicle.ControlType.Human)
		{
			base.Hud.HUDFinish.FillRank(rank);
		}
		if (rank == 0)
		{
			kart.KartSound.PlayVoice(KartSound.EVoices.Selection);
			kart.Anim.LaunchVictoryAnim(true);
		}
		else if (rank > 2)
		{
			kart.KartSound.PlayVoice(KartSound.EVoices.Bad);
			kart.Anim.LaunchDefeatAnim(true);
		}
		else
		{
			kart.KartSound.PlayVoice(KartSound.EVoices.Good);
			kart.Anim.LaunchSuccessAnim(true);
		}
	}

	public static void NextRace()
	{
		if (Singleton<GameConfigurator>.Instance.CurrentTrackIndex < Singleton<GameConfigurator>.Instance.ChampionShipData.Tracks.Length)
		{
			Singleton<GameManager>.Instance.Reset();
			Singleton<BonusMgr>.Instance.Reset();
			Singleton<GameConfigurator>.Instance.StartScene = Singleton<GameConfigurator>.Instance.ChampionShipData.Tracks[Singleton<GameConfigurator>.Instance.CurrentTrackIndex];
			UnityEngine.Object.Destroy(GameObject.Find("Root"));
			LoadingManager.LoadLevel(Singleton<GameConfigurator>.Instance.StartScene);
		}
		else
		{
			Singleton<GameConfigurator>.Instance.MenuToLaunch = EMenus.MENU_WELCOME;
			if ((bool)Singleton<GameConfigurator>.Instance.PlayerConfig)
			{
				Singleton<GameConfigurator>.Instance.PlayerConfig.ResetAdvantages();
			}
			LoadingManager.LoadLevel("MenuRoot");
		}
	}

	public override void UpdateScores()
	{
		List<Tuple<int, GameObject>> list = new List<Tuple<int, GameObject>>();
		int num = 0;
		for (int i = 0; i < 6; i++)
		{
			Kart kart = GetKart(i);
			GameObject player = GetPlayer(i);
			RcVehicleRaceStats raceStats = kart.RaceStats;
			if (raceStats.IsRaceEnded() || (raceStats.GetLogicNbLap() == raceStats.GetRaceNbLap() - 1 && raceStats.GetDistToEndOfRace() < Singleton<GameConfigurator>.Instance.GameSettings.MinDistToEndLine))
			{
				int rank = raceStats.GetRank();
				if (rank > num)
				{
					num = rank;
				}
			}
			else
			{
				list.Add(new Tuple<int, GameObject>(i, player));
			}
		}
		num++;
		Tuple<int, GameObject>[] array = list.ToArray();
		Array.Sort(array, new RankComparer(_aiManager));
		for (int j = 0; j < array.Length; j++)
		{
			int num2 = j + num;
			Tuple<int, GameObject> tuple = array[j];
			Kart kart2 = GetKart(tuple.Item1);
			if ((bool)kart2)
			{
				PlayerFinish(kart2.GetVehicleId(), Singleton<GameConfigurator>.Instance.GameSettings.ChampionShipScores[num2]);
			}
		}
	}

	public override void ConfigurePlaceVehiclesOnStartLine()
	{
		if (Singleton<GameConfigurator>.Instance.CurrentTrackIndex == 0)
		{
			if (Network.peerType != 0 && !Network.isServer)
			{
				return;
			}
			int num = 1;
			if (Network.peerType != 0)
			{
				NetworkMgr networkMgr = (NetworkMgr)UnityEngine.Object.FindObjectOfType(typeof(NetworkMgr));
				num = networkMgr.InitialNbPlayers;
			}
			int num2 = 6 - num;
			List<int> list = new List<int>();
			for (int i = 0; i < num2; i++)
			{
				list.Add(i);
			}
			int num3 = 5;
			for (int j = 0; j < base.PlayerCount; j++)
			{
				Kart kart = GetKart(j);
				if ((bool)kart)
				{
					if (kart.GetControlType() != RcVehicle.ControlType.AI)
					{
						SetInitialPlayerRank(kart.GetVehicleId(), num3);
						num3--;
					}
					else
					{
						int num4 = list[Singleton<RandomManager>.Instance.Next(list.Count - 1)];
						SetInitialPlayerRank(kart.GetVehicleId(), num4);
						list.Remove(num4);
					}
				}
			}
			ComputePlayersRank(true, false, true);
		}
		else
		{
			m_bInitialPlacementConfigured = true;
		}
	}

	public override void PlaceVehiclesOnStartLine()
	{
		GameObject gameObject = GameObject.Find("Start");
		if (!(gameObject != null))
		{
			return;
		}
		if (Network.peerType != 0)
		{
			for (int i = 0; i < m_pPlayers.Length; i++)
			{
				Kart kart = GetKart(i);
				int vehicleId = kart.GetVehicleId();
				RaceScoreData scoreData = Singleton<GameConfigurator>.Instance.RankingManager.GetScoreData(vehicleId);
				if (scoreData != null)
				{
					StartPositions[scoreData.KartIndex] = gameObject.transform.GetChild(gameObject.transform.GetChildCount() - scoreData.RacePosition - 1);
				}
			}
			return;
		}
		int rpRankOffset = 0;
		int rpHumanIndex = 0;
		bool flag = false;
		GetHumanKart(ref rpRankOffset, ref rpHumanIndex);
		if (Singleton<GameConfigurator>.Instance.PlayerConfig.GetAdvantage() == EAdvantage.FirstPlaceOnStart)
		{
			StartPositions[rpHumanIndex] = gameObject.transform.GetChild(m_pPlayers.Length - 1);
			flag = true;
		}
		else
		{
			StartPositions[rpHumanIndex] = gameObject.transform.GetChild(gameObject.transform.GetChildCount() - rpRankOffset - 1);
		}
		for (int j = 0; j < m_pPlayers.Length; j++)
		{
			Kart kart2 = GetKart(j);
			int vehicleId2 = kart2.GetVehicleId();
			RaceScoreData scoreData2 = Singleton<GameConfigurator>.Instance.RankingManager.GetScoreData(vehicleId2);
			if (scoreData2 != null && scoreData2.KartIndex != rpHumanIndex)
			{
				int num = 0;
				if (flag && scoreData2.RacePosition < rpRankOffset)
				{
					num++;
				}
				StartPositions[scoreData2.KartIndex] = gameObject.transform.GetChild(gameObject.transform.GetChildCount() - scoreData2.RacePosition - num - 1);
			}
		}
	}

	public override void FillResults()
	{
		if (Singleton<GameConfigurator>.Instance.CurrentTrackIndex == 0)
		{
			_nbFirstPlace = 0;
		}
		ComputePlayersRank(true, true, false);
		base.Hud.EndChampionshipRace.FillPositions();
		int rpRankOffset = 0;
		int rpHumanIndex = 0;
		GetHumanKart(ref rpRankOffset, ref rpHumanIndex);
		if (rpRankOffset == 0)
		{
			_nbFirstPlace++;
		}
		PlayEndOfRaceSound(rpRankOffset < 3);
		if (Singleton<GameConfigurator>.Instance.CurrentTrackIndex == Singleton<GameConfigurator>.Instance.ChampionShipData.Tracks.Length)
		{
			int humanChampionshipPos = GetHumanChampionshipPos();
			Singleton<RewardManager>.Instance.PlayerRank = humanChampionshipPos;
			Singleton<RewardManager>.Instance.EndChampionShip(humanChampionshipPos, _nbFirstPlace);
			List<RewardBase> rewards = Singleton<GameConfigurator>.Instance.ChampionShipData.Rewards;
			foreach (RewardBase item in rewards)
			{
				bool flag = false;
				bool flag2 = Singleton<GameConfigurator>.Instance.ChampionshipPass != null && Singleton<GameConfigurator>.Instance.ChampionshipPass.State != 0 && Singleton<GameConfigurator>.Instance.ChampionshipPass.ChampionshipSelectionned == Singleton<GameConfigurator>.Instance.ChampionShipData && Singleton<GameConfigurator>.Instance.ChampionshipPass.Difficulty == Singleton<GameConfigurator>.Instance.Difficulty;
				if (item is RewardChampionShip && (flag2 || Network.peerType != 0))
				{
					flag = true;
				}
				if (!flag)
				{
					item.GiveReward();
				}
			}
			Singleton<RewardManager>.Instance.GiveChampionShipReward(humanChampionshipPos, _nbFirstPlace);
			_nbFirstPlace = 0;
		}
		base.FillResults();
	}

	protected int GetHumanChampionshipPos()
	{
		Kart humanKart = GetHumanKart();
		if ((bool)humanKart)
		{
			ChampionShipScoreData championShipScoreData = (ChampionShipScoreData)Singleton<GameConfigurator>.Instance.RankingManager.GetScoreData(humanKart.GetVehicleId());
			return championShipScoreData.ChampionshipPosition;
		}
		return -1;
	}
}
