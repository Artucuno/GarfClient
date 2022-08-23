using System;
using UnityEngine;

public class RaceGameState : GameState
{
	private bool selfEnded;

	private RcRace m_pRace;

	private NetworkMgr networkMgr;

	public void Awake()
	{
		networkMgr = (NetworkMgr)UnityEngine.Object.FindObjectOfType(typeof(NetworkMgr));
		GameObject gameObject = GameObject.Find("Race");
		if (gameObject != null)
		{
			m_pRace = gameObject.GetComponent<RcRace>();
		}
	}

	public override void Enter()
	{
		for (int i = 0; i < m_pGameMode.PlayerCount; i++)
		{
			if (m_pGameMode.GetKart(i) != null)
			{
				Kart kart = m_pGameMode.GetKart(i);
				kart.SetLocked(false);
				kart.StartRace();
			}
		}
		if (m_pRace != null)
		{
			m_pRace.StartRace();
		}
		Kart humanKart = m_pGameMode.GetHumanKart();
		humanKart.OnRaceEnded = (Action<RcVehicle>)Delegate.Combine(humanKart.OnRaceEnded, new Action<RcVehicle>(OnRaceEnded));
		selfEnded = false;
		Singleton<GameManager>.Instance.GameMode.MainMusic.Play();
	}

	public override void Exit()
	{
	}

	protected override void Update()
	{
		if (selfEnded)
		{
			if (!networkMgr.WaitingSynchronization)
			{
				OnStateChanged(E_GameState.End);
			}
			if (!Singleton<GameManager>.Instance.SoundManager.SoundsList[3].isPlaying && !Singleton<GameManager>.Instance.SoundManager.SoundsList[4].isPlaying)
			{
				Singleton<GameManager>.Instance.SoundManager.PlayMusic(ERaceMusicLoops.InterRace);
			}
		}
	}

	protected void OnRaceEnded(RcVehicle pVehicle)
	{
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			OnStateChanged(E_GameState.End);
		}
		else
		{
			selfEnded = true;
			networkMgr.StartSynchronization();
		}
		Camera.main.GetComponent<CameraBase>().SwitchCamera(ECamState.End, ECamState.TransCut);
		Singleton<GameManager>.Instance.GameMode.MainMusic.Stop();
	}
}
