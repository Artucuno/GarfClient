using UnityEngine;

public class CarPresentationGameState : GameState
{
	private float _presentationTimer = 3f;

	public override void Enter()
	{
		Singleton<GameManager>.Instance.GameMode.Hud.HUDPause.CreateNamePlates();
		Singleton<GameManager>.Instance.SoundManager.StopMusic();
		Camera.main.GetComponent<CameraBase>().SwitchCamera(ECamState.CarPres, ECamState.TransCut);
		if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TIME_TRIAL)
		{
			Kart kart = m_pGameMode.GetKart(0);
			KartSound kartSound = kart.KartSound;
			if (kartSound != null)
			{
				kartSound.StartSound();
			}
			_presentationTimer = 0f;
		}
		else
		{
			for (int i = 0; i < m_pGameMode.PlayerCount; i++)
			{
				Kart kart2 = m_pGameMode.GetKart(i);
				if ((bool)kart2)
				{
					KartSound kartSound2 = kart2.KartSound;
					if (kartSound2 != null)
					{
						kartSound2.StartSound();
					}
				}
			}
		}
		Singleton<GameManager>.Instance.SoundManager.PlaySound(ERaceSounds.JingleCountdown);
	}

	public override void Exit()
	{
	}

	protected override void Update()
	{
		_presentationTimer -= Time.deltaTime;
		if (_presentationTimer < 0f || Input.anyKeyDown)
		{
			if (Network.peerType == NetworkPeerType.Disconnected && Singleton<GameSaveManager>.Instance.GetShowTutorial())
			{
				OnStateChanged(E_GameState.Tutorial);
			}
			else if (Network.isServer)
			{
				base.networkView.RPC("CountDown", RPCMode.All);
			}
			else if (!Network.isClient)
			{
				CountDown();
			}
		}
	}

	[RPC]
	public void CountDown()
	{
		OnStateChanged(E_GameState.Start);
	}
}
