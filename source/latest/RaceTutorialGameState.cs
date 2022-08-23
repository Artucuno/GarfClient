using System;
using UnityEngine;

public class RaceTutorialGameState : GameState
{
	public float Timer = 1f;

	private bool m_bKeyPressed;

	private float m_fStartTime;

	private RcRace m_pRace;

	public void Awake()
	{
		m_fStartTime = Time.time;
		GameEntryPoint.OnVehicleCreated = (Action)Delegate.Combine(GameEntryPoint.OnVehicleCreated, new Action(VehicleCreated));
		GameObject gameObject = GameObject.Find("Race");
		if (gameObject != null)
		{
			m_pRace = gameObject.GetComponent<RcRace>();
		}
	}

	public void OnDestroy()
	{
		GameEntryPoint.OnVehicleCreated = (Action)Delegate.Remove(GameEntryPoint.OnVehicleCreated, new Action(VehicleCreated));
	}

	private void VehicleCreated()
	{
		((InGameGameMode)m_pGameMode).VehicleCreated();
	}

	public override void Enter()
	{
		Camera.main.GetComponent<CameraBase>().SwitchCamera(ECamState.Respawn, ECamState.TransCut);
		HUDInGame hud = Singleton<GameManager>.Instance.GameMode.Hud;
		if (hud != null)
		{
			hud.StartRace();
		}
		for (int i = 0; i < m_pGameMode.PlayerCount; i++)
		{
			Kart kart = m_pGameMode.GetKart(i);
			if ((bool)kart)
			{
				KartSound kartSound = kart.KartSound;
				if (kartSound != null)
				{
					kartSound.StartVoices();
				}
			}
		}
		for (int j = 0; j < m_pGameMode.PlayerCount; j++)
		{
			if (m_pGameMode.GetKart(j) != null)
			{
				Kart kart2 = m_pGameMode.GetKart(j);
				kart2.StartRace();
			}
		}
		if (m_pRace != null)
		{
			m_pRace.StartRace();
		}
		Singleton<GameManager>.Instance.GameMode.MainMusic.Play();
	}

	public override void Exit()
	{
	}

	protected override void Update()
	{
		switch (((InGameGameMode)m_pGameMode).PlaceVehicles)
		{
		case EVehiclePlacementState.Init:
			((InGameGameMode)m_pGameMode).ValidateAdvantage();
			break;
		case EVehiclePlacementState.ReadyToTeleport:
			if (!m_bKeyPressed)
			{
				m_bKeyPressed = true;
			}
			if (m_bKeyPressed)
			{
				((InGameGameMode)m_pGameMode).TeleportVehiclesOnStartLine();
				m_bKeyPressed = false;
			}
			break;
		case EVehiclePlacementState.ReadyToStart:
			if (!m_bKeyPressed)
			{
				CameraBase component = Camera.main.GetComponent<CameraBase>();
				component.SwitchCamera(ECamState.Follow, ECamState.TransCut);
				m_bKeyPressed = true;
			}
			break;
		}
		if (!((TutorialGameMode)m_pGameMode).Launched && Timer < Time.time - m_fStartTime)
		{
			((TutorialGameMode)m_pGameMode).FirstPanel();
		}
	}
}
