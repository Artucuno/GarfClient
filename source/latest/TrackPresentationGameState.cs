using System;
using UnityEngine;

public class TrackPresentationGameState : GameState
{
	private bool m_bKeyPressed;

	private NetworkMgr networkMgr;

	public void Awake()
	{
		GameEntryPoint.OnVehicleCreated = (Action)Delegate.Combine(GameEntryPoint.OnVehicleCreated, new Action(VehicleCreated));
		networkMgr = (NetworkMgr)UnityEngine.Object.FindObjectOfType(typeof(NetworkMgr));
	}

	public void OnDestroy()
	{
		GameEntryPoint.OnVehicleCreated = (Action)Delegate.Remove(GameEntryPoint.OnVehicleCreated, new Action(VehicleCreated));
	}

	public override void Enter()
	{
		Singleton<GameManager>.Instance.GameMode.MainMusic.Stop();
		Singleton<GameManager>.Instance.SoundManager.PlayMusic(ERaceMusicLoops.TrackPresentation);
		m_bKeyPressed = false;
		GameObject gameObject = GameObject.Find("SplineRespawn");
		RcMultiPath pathToFollow = null;
		if (gameObject != null)
		{
			pathToFollow = gameObject.GetComponent<RcMultiPath>();
		}
		Camera.main.GetComponent<CamStatePath>().Setup(pathToFollow);
		Camera.main.GetComponent<CameraBase>().SwitchCamera(ECamState.Path, ECamState.TransCut);
		HUDInGame hud = m_pGameMode.Hud;
		if (hud != null)
		{
			hud.EnterTrackPresentation();
		}
	}

	public override void Exit()
	{
		if (Singleton<GameConfigurator>.Instance.PlayerConfig.GetAdvantage() == EAdvantage.PuzzleRadar)
		{
			Kart humanKart = m_pGameMode.GetHumanKart();
			UnityEngine.Object[] array = UnityEngine.Object.FindSceneObjectsOfType(typeof(PuzzlePiece));
			for (int i = 0; i < array.Length; i++)
			{
				PuzzlePiece puzzlePiece = (PuzzlePiece)array[i];
				puzzlePiece.Player = humanKart;
			}
		}
		HUDInGame hud = Singleton<GameManager>.Instance.GameMode.Hud;
		if (hud != null)
		{
			hud.ExitTrackPresentation();
		}
	}

	private void VehicleCreated()
	{
		((InGameGameMode)m_pGameMode).VehicleCreated();
	}

	protected override void Update()
	{
		EVehiclePlacementState placeVehicles = ((InGameGameMode)m_pGameMode).PlaceVehicles;
		switch (placeVehicles)
		{
		case EVehiclePlacementState.Init:
			if (m_pGameMode.Hud.TrackPresentation != null && m_pGameMode.Hud.TrackPresentation.ValidateAdvantage())
			{
				((InGameGameMode)m_pGameMode).ValidateAdvantage();
			}
			break;
		case EVehiclePlacementState.ReadyToTeleport:
			if (!m_bKeyPressed)
			{
				m_bKeyPressed = true;
				if (Network.peerType != 0 && !networkMgr.WaitingSynchronization)
				{
					networkMgr.StartSynchronization();
				}
			}
			if (m_bKeyPressed && (Network.peerType == NetworkPeerType.Disconnected || !networkMgr.WaitingSynchronization))
			{
				((InGameGameMode)m_pGameMode).TeleportVehiclesOnStartLine();
				m_bKeyPressed = false;
			}
			break;
		}
		if (placeVehicles != EVehiclePlacementState.ReadyToStart)
		{
			return;
		}
		if (!m_bKeyPressed)
		{
			m_bKeyPressed = true;
			if (Network.peerType != 0 && !networkMgr.WaitingSynchronization)
			{
				networkMgr.StartSynchronization();
			}
		}
		if (m_bKeyPressed && (Network.peerType == NetworkPeerType.Disconnected || !networkMgr.WaitingSynchronization))
		{
			OnStateChanged(E_GameState.CarPresentation);
		}
	}
}
