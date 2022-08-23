using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InGameGameMode : GameMode
{
	protected GkAIManager _aiManager;

	public EVehiclePlacementState PlaceVehicles;

	protected Transform[] StartPositions;

	private GameObject _start;

	private GameObject _pathResource;

	protected bool _hasStart;

	protected ChanceDispatcher _chanceDispatcher;

	protected bool m_bVehicleCreated;

	protected bool m_bInitialPlacementConfigured;

	public GkAIManager AIManager
	{
		get
		{
			return _aiManager;
		}
	}

	protected virtual void Start()
	{
		base.State = _state;
	}

	public override void Awake()
	{
		base.Awake();
		_aiManager = new GkAIManager();
		PlaceVehicles = EVehiclePlacementState.Init;
		StartPositions = new Transform[6];
		_hasStart = false;
		_chanceDispatcher = new ChanceDispatcher();
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("HUD"));
		gameObject.transform.localPosition = Vector3.zero;
		_hud = gameObject.GetComponent<HUDInGame>();
		_gameStates[0] = base.gameObject.AddComponent<TrackPresentationGameState>();
		GameState obj = _gameStates[0];
		obj.OnStateChanged = (Action<E_GameState>)Delegate.Combine(obj.OnStateChanged, new Action<E_GameState>(StateChange));
		_gameStates[0].gameMode = this;
		_gameStates[0].enabled = false;
		_gameStates[1] = base.gameObject.AddComponent<CarPresentationGameState>();
		GameState obj2 = _gameStates[1];
		obj2.OnStateChanged = (Action<E_GameState>)Delegate.Combine(obj2.OnStateChanged, new Action<E_GameState>(StateChange));
		_gameStates[1].gameMode = this;
		_gameStates[1].enabled = false;
		_gameStates[2] = base.gameObject.AddComponent<StartGameState>();
		GameState obj3 = _gameStates[2];
		obj3.OnStateChanged = (Action<E_GameState>)Delegate.Combine(obj3.OnStateChanged, new Action<E_GameState>(StateChange));
		_gameStates[2].gameMode = this;
		_gameStates[2].enabled = false;
		_gameStates[3] = base.gameObject.AddComponent<RaceGameState>();
		GameState obj4 = _gameStates[3];
		obj4.OnStateChanged = (Action<E_GameState>)Delegate.Combine(obj4.OnStateChanged, new Action<E_GameState>(StateChange));
		_gameStates[3].gameMode = this;
		_gameStates[3].enabled = false;
		_gameStates[4] = base.gameObject.AddComponent<EndRaceGameState>();
		GameState obj5 = _gameStates[4];
		obj5.OnStateChanged = (Action<E_GameState>)Delegate.Combine(obj5.OnStateChanged, new Action<E_GameState>(StateChange));
		_gameStates[4].gameMode = this;
		_gameStates[4].enabled = false;
		_gameStates[5] = base.gameObject.AddComponent<ResultGameState>();
		GameState obj6 = _gameStates[5];
		obj6.OnStateChanged = (Action<E_GameState>)Delegate.Combine(obj6.OnStateChanged, new Action<E_GameState>(StateChange));
		_gameStates[5].gameMode = this;
		_gameStates[5].enabled = false;
		_gameStates[7] = base.gameObject.AddComponent<TutorialGameState>();
		GameState obj7 = _gameStates[7];
		obj7.OnStateChanged = (Action<E_GameState>)Delegate.Combine(obj7.OnStateChanged, new Action<E_GameState>(StateChange));
		_gameStates[7].gameMode = this;
		_gameStates[7].enabled = false;
		_gameStates[8] = base.gameObject.AddComponent<RaceTutorialGameState>();
		GameState obj8 = _gameStates[8];
		obj8.OnStateChanged = (Action<E_GameState>)Delegate.Combine(obj8.OnStateChanged, new Action<E_GameState>(StateChange));
		_gameStates[8].gameMode = this;
		_gameStates[8].enabled = false;
		m_bVehicleCreated = false;
		m_bInitialPlacementConfigured = false;
	}

	protected override void StateChange(E_GameState pNewState)
	{
		base.StateChange(pNewState);
		if (pNewState == E_GameState.Race)
		{
			_hasStart = true;
		}
	}

	public override void Dispose()
	{
		UnityEngine.Object.Destroy(_pathResource);
		base.Dispose();
	}

	public void Update()
	{
		if (_hasStart)
		{
			_aiManager.Update();
		}
		if (PlaceVehicles == EVehiclePlacementState.ValidateAdvantage && m_bVehicleCreated)
		{
			Kart humanKart = GetHumanKart();
			if ((bool)humanKart)
			{
				SetAdvantage(humanKart.GetVehicleId(), Singleton<GameConfigurator>.Instance.PlayerConfig.GetAdvantage());
			}
			_aiManager.SetBoostStart(this);
			PlaceVehicles = EVehiclePlacementState.ComputeInitialPlacement;
			ConfigurePlaceVehiclesOnStartLine();
		}
		else if (PlaceVehicles == EVehiclePlacementState.ComputeInitialPlacement && m_bInitialPlacementConfigured)
		{
			PlaceVehicles = EVehiclePlacementState.ReadyToTeleport;
			PlaceVehiclesOnStartLine();
		}
		else if (PlaceVehicles == EVehiclePlacementState.NeedToTeleport)
		{
			PlaceVehicles = EVehiclePlacementState.Teleported;
			for (int i = 0; i < StartPositions.Length; i++)
			{
				for (int j = 0; j < m_pPlayers.Length; j++)
				{
					if (m_pPlayers[j] != null && m_pPlayers[j].Item2 != null)
					{
						Kart item = m_pPlayers[j].Item2;
						if ((bool)item && item.GetVehicleId() == i)
						{
							RcKinematicPhysic rcKinematicPhysic = (RcKinematicPhysic)item.GetVehiclePhysic();
							rcKinematicPhysic.Teleport(StartPositions[i].position, StartPositions[i].rotation);
							item.Enable();
						}
					}
				}
			}
		}
		else if (PlaceVehicles == EVehiclePlacementState.Teleported)
		{
			PlaceVehicles = EVehiclePlacementState.ReadyToStart;
			m_bReadyToStart = true;
			UnityEngine.Object[] array = UnityEngine.Object.FindSceneObjectsOfType(typeof(RcPortalTrigger));
			for (int k = 0; k < array.Length; k++)
			{
				RcPortalTrigger rcPortalTrigger = (RcPortalTrigger)array[k];
				rcPortalTrigger.enabled = true;
			}
		}
	}

	public void TeleportVehiclesOnStartLine()
	{
		PlaceVehicles = EVehiclePlacementState.NeedToTeleport;
	}

	public override void StartScene()
	{
		ComputePlayerAdvantages();
		GameObject gameObject = new GameObject();
		gameObject.name = "Vehicles Collision Manager";
		gameObject.AddComponent(typeof(RcKinematicCollisionManager));
		GameObject gameObject2 = new GameObject();
		gameObject2.name = "Respawn Checker";
		gameObject2.AddComponent(typeof(RcRespawnChecker));
		GameObject gameObject3 = GameObject.Find("Race");
		RcCatchUp component = gameObject3.GetComponent<RcCatchUp>();
		if (!component)
		{
			component = (RcCatchUp)gameObject3.AddComponent(typeof(RcCatchUp));
			if (Network.peerType == NetworkPeerType.Disconnected)
			{
				if (Singleton<GameConfigurator>.Instance.Difficulty == EDifficulty.EASY)
				{
					component.AddFactor(-200f, 0f, 0.05f);
					component.AddFactor(0f, 0f, -0.15f);
					component.AddFactor(50f, 0f, -0.25f);
					component.AddFactor(200f, 0f, -0.5f);
				}
				else if (Singleton<GameConfigurator>.Instance.Difficulty == EDifficulty.NORMAL)
				{
					component.AddFactor(-100f, 0f, 0.1f);
					component.AddFactor(0f, 0f, 0.05f);
					component.AddFactor(100f, 0f, -0.1f);
					component.AddFactor(500f, 0f, -0.5f);
				}
				else
				{
					component.AddFactor(-50f, 0f, 0.1f);
					component.AddFactor(0f, 0f, 0.1f);
					component.AddFactor(200f, 0f, -0.05f);
				}
			}
			else if (Singleton<GameConfigurator>.Instance.Difficulty == EDifficulty.EASY)
			{
				component.AddFactor(-200f, 0.1f, 0.05f);
				component.AddFactor(0f, 0f, -0.05f);
				component.AddFactor(50f, 0f, -0.25f);
				component.AddFactor(200f, 0f, -0.5f);
			}
			else if (Singleton<GameConfigurator>.Instance.Difficulty == EDifficulty.NORMAL)
			{
				component.AddFactor(-100f, 0.05f, 0.1f);
				component.AddFactor(0f, 0f, 0.05f);
				component.AddFactor(100f, 0f, -0.1f);
				component.AddFactor(500f, 0f, -0.5f);
			}
			else
			{
				component.AddFactor(-50f, 0.05f, 0.1f);
				component.AddFactor(0f, 0f, 0.1f);
				component.AddFactor(200f, 0f, -0.05f);
			}
			gameObject3.GetComponent<RcRace>().Start();
		}
		if (Singleton<ChallengeManager>.Instance.IsActive)
		{
			Singleton<ChallengeManager>.Instance.SetTried();
		}
	}

	public override void CreatePlayers()
	{
		NetworkMgr networkMgr = (NetworkMgr)UnityEngine.Object.FindObjectOfType(typeof(NetworkMgr));
		int num = 6;
		if (Network.peerType != 0)
		{
			num -= networkMgr.InitialNbPlayers - 1;
		}
		bool flag = Singleton<GameConfigurator>.Instance.PlayerDataList == null;
		for (int i = 0; i < num; i++)
		{
			if (Network.isClient && i != 0)
			{
				continue;
			}
			int num2 = i;
			if (Network.peerType != 0)
			{
				num2 = ((i != 0) ? (i + (networkMgr.InitialNbPlayers - 1)) : networkMgr.GetNetworkID());
			}
			if (!flag)
			{
				PlayerData playerData = Singleton<GameConfigurator>.Instance.PlayerDataList[num2];
				CreatePlayer(playerData.Character, playerData.Kart, playerData.Custom, playerData.Hat, playerData.NbStars, num2, true, i > 0);
				continue;
			}
			GameObject vHat = null;
			GameObject vKartCusto = null;
			int iNbStars = 0;
			if (i == 0 && DebugMgr.Instance != null && !DebugMgr.Instance.dbgData.RandomPlayer)
			{
				DebugMgr.Instance.LoadDefaultPlayer(num2, this, true, i > 0);
				continue;
			}
			ECharacter vPerso;
			ECharacter vKart;
			if (i > 0)
			{
				_chanceDispatcher.DispatchAI(num2, true, out vPerso, out vKart, out vHat, out vKartCusto);
			}
			else
			{
				vPerso = Singleton<GameConfigurator>.Instance.PlayerConfig.Character;
				vKart = Singleton<GameConfigurator>.Instance.PlayerConfig.Kart;
				iNbStars = Singleton<GameConfigurator>.Instance.PlayerConfig.NbStars;
				vHat = (GameObject)Resources.Load("Hat/" + Singleton<GameConfigurator>.Instance.PlayerConfig.m_oHat.name);
				vKartCusto = (GameObject)Resources.Load("Kart/" + Singleton<GameConfigurator>.Instance.PlayerConfig.m_oKartCustom.name);
				_chanceDispatcher.AddPlayerData(vPerso, vKart, vKartCusto, vHat, iNbStars, num2);
			}
			CreatePlayer(vPerso, vKart, vKartCusto.name, vHat.name, iNbStars, num2, true, i > 0);
		}
		CreateAIPaths();
	}

	protected void CreateAIPaths()
	{
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(Singleton<GameConfigurator>.Instance.StartScene);
		GameObject gameObject = (GameObject)Resources.Load("AI/Paths" + fileNameWithoutExtension);
		if (gameObject != null)
		{
			_pathResource = (GameObject)UnityEngine.Object.Instantiate(gameObject);
		}
	}

	private int GetRandomStartPos(List<int> pAvailableStartPos)
	{
		int num = pAvailableStartPos[UnityEngine.Random.Range(0, pAvailableStartPos.Count)];
		pAvailableStartPos.Remove(num);
		return num;
	}

	public virtual void ConfigurePlaceVehiclesOnStartLine()
	{
		m_bInitialPlacementConfigured = true;
	}

	public virtual void PlaceVehiclesOnStartLine()
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
				Transform transform = null;
				transform = gameObject.transform.GetChild(vehicleId);
				StartPositions[i] = transform;
			}
			return;
		}
		List<int> list = new List<int>();
		list.Add(0);
		list.Add(1);
		list.Add(2);
		list.Add(3);
		list.Add(4);
		list.Add(5);
		List<int> list2 = list;
		int _iHumanIndex = 0;
		GameObject humanPlayer = GetHumanPlayer(ref _iHumanIndex);
		if (humanPlayer != null)
		{
			if (Singleton<GameConfigurator>.Instance.PlayerConfig.GetAdvantage() == EAdvantage.FirstPlaceOnStart)
			{
				int num = gameObject.transform.GetChildCount() - 1;
				StartPositions[_iHumanIndex] = gameObject.transform.GetChild(num);
				list2.Remove(num);
			}
			else
			{
				StartPositions[_iHumanIndex] = gameObject.transform.GetChild(0);
				list2.Remove(0);
			}
		}
		for (int j = 0; j < m_pPlayers.Length; j++)
		{
			if (GetPlayer(j) != humanPlayer)
			{
				Transform transform2 = null;
				transform2 = gameObject.transform.GetChild(GetRandomStartPos(list2));
				StartPositions[j] = transform2;
			}
		}
	}

	public void SubscribeRaceEnd()
	{
		for (int i = 0; i < base.PlayerCount; i++)
		{
			Kart kart = GetKart(i);
			if ((bool)kart)
			{
				if (kart.GetControlType() == RcVehicle.ControlType.Human)
				{
					kart.OnRaceEnded = (Action<RcVehicle>)Delegate.Combine(kart.OnRaceEnded, new Action<RcVehicle>(RaceEnd));
				}
				else
				{
					kart.OnRaceEnded = (Action<RcVehicle>)Delegate.Combine(kart.OnRaceEnded, new Action<RcVehicle>(IANetRaceEnded));
				}
			}
		}
	}

	protected virtual void IANetRaceEnded(RcVehicle pVehicle)
	{
		int rank = pVehicle.RaceStats.GetRank();
		Kart kart = (Kart)pVehicle;
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
		int num = Singleton<GameConfigurator>.Instance.GameSettings.ChampionShipScores[rank];
		if (num < 4 && kart.SelectedAdvantage == EAdvantage.FourPoints)
		{
			num = 4;
		}
		else if (num < 2 && kart.SelectedAdvantage == EAdvantage.TwoPoints)
		{
			num = 2;
		}
		PlayerFinish(pVehicle.GetVehicleId(), num);
	}

	protected virtual void RaceEnd(RcVehicle pVehicle)
	{
		int rank = pVehicle.RaceStats.GetRank();
		Kart kart = (Kart)pVehicle;
		int num = Singleton<GameConfigurator>.Instance.GameSettings.ChampionShipScores[rank];
		if (num < 4 && kart.SelectedAdvantage == EAdvantage.FourPoints)
		{
			num = 4;
		}
		else if (num < 2 && kart.SelectedAdvantage == EAdvantage.TwoPoints)
		{
			num = 2;
		}
		PlayerFinish(pVehicle.GetVehicleId(), num);
		if (Singleton<ChallengeManager>.Instance.IsActive)
		{
			Singleton<ChallengeManager>.Instance.CheckSuccess();
		}
	}

	public virtual void AddPlayerData(PlayerData pPlayerData, int pKartIndex)
	{
		if (Singleton<GameConfigurator>.Instance.PlayerDataList == null)
		{
			Singleton<GameConfigurator>.Instance.PlayerDataList = new PlayerData[6];
		}
		pPlayerData.KartIndex = pKartIndex;
		Singleton<GameConfigurator>.Instance.PlayerDataList[pKartIndex] = pPlayerData;
	}

	public void PlayEndOfRaceSound(bool bSuccess)
	{
		if ((bool)Singleton<GameManager>.Instance.SoundManager)
		{
			Singleton<GameManager>.Instance.SoundManager.PlaySound((!bSuccess) ? ERaceSounds.FinishBad : ERaceSounds.FinishGood);
		}
	}

	public void ValidateAdvantage()
	{
		PlaceVehicles = EVehiclePlacementState.ValidateAdvantage;
	}

	public void VehicleCreated()
	{
		m_bVehicleCreated = true;
	}

	public void SetInitialPlayerRank(int iKartIndex, int iRank)
	{
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			DoSetInitialPlayerRank(iKartIndex, iRank);
			return;
		}
		base.networkView.RPC("DoSetInitialPlayerRank", RPCMode.All, iKartIndex, iRank);
	}

	[RPC]
	public void DoSetInitialPlayerRank(int iKartIndex, int iRank)
	{
		Singleton<GameConfigurator>.Instance.RankingManager.SetInitialRank(iKartIndex, iRank);
	}

	public void ComputePlayersRank(bool bChampionship, bool bScore, bool bInitialPlacement)
	{
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			DoComputePlayersRank(bChampionship, bScore, bInitialPlacement);
			return;
		}
		base.networkView.RPC("DoComputePlayersRank", RPCMode.All, bChampionship, bScore, bInitialPlacement);
	}

	[RPC]
	public void DoComputePlayersRank(bool bChampionship, bool bScore, bool bInitialPlacement)
	{
		Singleton<GameConfigurator>.Instance.RankingManager.ComputePositions(bChampionship, bScore);
		if (bInitialPlacement)
		{
			m_bInitialPlacementConfigured = true;
		}
	}

	public void RestartRanking()
	{
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			DoRestartRanking();
		}
		else
		{
			base.networkView.RPC("DoRestartRanking", RPCMode.All);
		}
	}

	[RPC]
	public void DoRestartRanking()
	{
		Singleton<GameConfigurator>.Instance.RankingManager.RestartRace();
	}

	public void PlayerFinish(int iKartIndex, int iScore)
	{
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			DoPlayerFinish(iKartIndex, iScore);
			return;
		}
		base.networkView.RPC("DoPlayerFinish", RPCMode.All, iKartIndex, iScore);
	}

	[RPC]
	public void DoPlayerFinish(int iKartIndex, int iScore)
	{
		Singleton<GameConfigurator>.Instance.RankingManager.PlayerFinish(iKartIndex, iScore);
	}

	public void ResetRace()
	{
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			DoResetRace();
		}
		else
		{
			base.networkView.RPC("DoResetRace", RPCMode.All);
		}
	}

	[RPC]
	public void DoResetRace()
	{
		Singleton<GameConfigurator>.Instance.RankingManager.ResetRace();
	}

	public virtual void FillResults()
	{
		base.Hud.EndRace();
	}

	public virtual void UpdateScores()
	{
	}

	protected Kart GetHumanKart(ref int rpRankOffset, ref int rpHumanIndex)
	{
		Kart humanKart = GetHumanKart();
		if ((bool)humanKart)
		{
			rpRankOffset = Singleton<GameConfigurator>.Instance.RankingManager.GetScoreData(humanKart.GetVehicleId()).RacePosition;
			rpHumanIndex = humanKart.GetVehicleId();
			return humanKart;
		}
		return null;
	}

	public void PlayerDisconnected(Kart _Kart)
	{
		if ((bool)_Kart)
		{
			_Kart.OnRaceEnded = (Action<RcVehicle>)Delegate.Combine(_Kart.OnRaceEnded, new Action<RcVehicle>(IANetRaceEnded));
		}
	}
}
