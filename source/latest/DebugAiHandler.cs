using UnityEngine;

public class DebugAiHandler : AIPathHandler
{
	public E_AILevel AILevel;

	public bool IsPlayerIA;

	public GameObject StartPath;

	protected override void Start()
	{
		if (Application.isPlaying)
		{
			StartIdealPath();
			base._AIManager.Init(this);
			Kart kart = Singleton<GameManager>.Instance.GameMode.GetKart(0);
			GameObject player = Singleton<GameManager>.Instance.GameMode.GetPlayer(0);
			if ((bool)kart && IsPlayerIA)
			{
				kart.SetControlType(RcVehicle.ControlType.AI);
				RegisterController(player, AILevel);
				player.GetComponentInChildren<RcVirtualController>().SetDrivingEnabled(true);
			}
			else
			{
				kart.SetControlType(RcVehicle.ControlType.Human);
			}
		}
	}

	public override void InitIdealPaths(RacingAI pAI, int pIndex)
	{
		pAI.IdealPath = new RcFastValuePath(StartPath);
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
	}
}
