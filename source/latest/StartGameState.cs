using UnityEngine;

public class StartGameState : GameState
{
	private float _startTimer = 1f;

	private int _startCounter = 4;

	private HUDCountdown _hudCD;

	public override void Enter()
	{
		Reset();
		CameraBase component = Camera.main.GetComponent<CameraBase>();
		if (component != null && component.CurrentState != 0)
		{
			component.SwitchCamera(ECamState.Follow, ECamState.TransCut);
		}
		HUDInGame hud = Singleton<GameManager>.Instance.GameMode.Hud;
		if (hud != null)
		{
			hud.StartRace();
			_hudCD = hud.Countdown;
		}
		for (int i = 0; i < ((Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TIME_TRIAL) ? 1 : m_pGameMode.PlayerCount); i++)
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
	}

	public override void Exit()
	{
		Reset();
	}

	private void Reset()
	{
		_startTimer = 1f;
		_startCounter = 4;
	}

	protected override void Update()
	{
		float deltaTime = Time.deltaTime;
		if (_startCounter <= 0)
		{
			return;
		}
		_startTimer -= deltaTime;
		if (!(_startTimer <= 0f))
		{
			return;
		}
		_startTimer += 1f;
		_startCounter--;
		if ((bool)Singleton<GameManager>.Instance.SoundManager)
		{
			Singleton<GameManager>.Instance.SoundManager.StopSound(ERaceSounds.JingleCountdown);
			if (_startCounter == 0)
			{
				Singleton<GameManager>.Instance.SoundManager.PlaySound(ERaceSounds.CountdownGo);
			}
			else
			{
				Singleton<GameManager>.Instance.SoundManager.PlaySound(ERaceSounds.Countdown);
			}
		}
		if (_hudCD != null)
		{
			_hudCD.SetCountdown(_startCounter);
		}
		if (_startCounter == 0)
		{
			OnStateChanged(E_GameState.Race);
		}
	}
}
