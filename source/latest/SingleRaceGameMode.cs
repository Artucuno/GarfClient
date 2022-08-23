using System;
using System.Collections.Generic;
using UnityEngine;

public class SingleRaceGameMode : InGameGameMode
{
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

	public override void CreatePlayers()
	{
		base.CreatePlayers();
		SubscribeRaceEnd();
	}

	protected override void RaceEnd(RcVehicle pVehicle)
	{
		base.RaceEnd(pVehicle);
		int rank = pVehicle.RaceStats.GetRank();
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
		Singleton<RewardManager>.Instance.EndSingleRace(rank, true);
		PlayEndOfRaceSound(rank == 0);
		Singleton<RewardManager>.Instance.CanUnlockPuzzlePieces(true);
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
		ComputePlayersRank(false, false, true);
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
				if ((bool)kart)
				{
					int vehicleId = kart.GetVehicleId();
					RaceScoreData scoreData = Singleton<GameConfigurator>.Instance.RankingManager.GetScoreData(vehicleId);
					if (scoreData != null)
					{
						StartPositions[scoreData.KartIndex] = gameObject.transform.GetChild(gameObject.transform.GetChildCount() - scoreData.RacePosition - 1);
					}
				}
			}
			return;
		}
		int rpRankOffset = 0;
		int rpHumanIndex = 0;
		Kart humanKart = GetHumanKart(ref rpRankOffset, ref rpHumanIndex);
		if (humanKart != null)
		{
			if (Singleton<GameConfigurator>.Instance.PlayerConfig.GetAdvantage() == EAdvantage.FirstPlaceOnStart)
			{
				StartPositions[rpHumanIndex] = gameObject.transform.GetChild(m_pPlayers.Length - 1);
			}
			else
			{
				StartPositions[rpHumanIndex] = gameObject.transform.GetChild(gameObject.transform.GetChildCount() - rpRankOffset - 1);
				rpRankOffset = 0;
			}
		}
		for (int j = 0; j < Singleton<GameConfigurator>.Instance.RankingManager.RaceScoreCount(); j++)
		{
			RaceScoreData racePos = Singleton<GameConfigurator>.Instance.RankingManager.GetRacePos(j);
			if (racePos != null && racePos.KartIndex != rpHumanIndex)
			{
				int num = 0;
				if (j < rpRankOffset)
				{
					num++;
				}
				StartPositions[racePos.KartIndex] = gameObject.transform.GetChild(gameObject.transform.GetChildCount() - j - num - 1);
			}
		}
	}

	public override void FillResults()
	{
		ComputePlayersRank(false, true, false);
		base.Hud.HUDEndSingleRace.FillPositions();
		int rpRankOffset = 0;
		int rpHumanIndex = 0;
		GetHumanKart(ref rpRankOffset, ref rpHumanIndex);
		base.FillResults();
	}
}
