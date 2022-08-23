using UnityEngine;

public class RaceCoin : RaceItem
{
	private bool _sendEvent;

	protected override void Awake()
	{
		_sendEvent = false;
		if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TIME_TRIAL)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			base.Awake();
		}
	}

	public override void DoOnTriggerEnter(GameObject other, int otherlayer)
	{
		if (!(other == null))
		{
			RcVehicle componentInChildren = other.GetComponentInChildren<RcVehicle>();
			if (componentInChildren != null)
			{
				_sendEvent = componentInChildren.GetControlType() == RcVehicle.ControlType.Human && Singleton<GameManager>.Instance.GameMode.State == E_GameState.Race;
				DoTrigger(componentInChildren);
			}
		}
	}

	protected override void DoTrigger(RcVehicle pVehicle)
	{
		base.DoTrigger(pVehicle);
		if (_sendEvent)
		{
			Kart kart = (Kart)pVehicle;
			kart.KartSound.PlaySoundImmediately(18);
			Singleton<RewardManager>.Instance.EarnCoins();
		}
	}
}
