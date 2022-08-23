using UnityEngine;

public class GkVirtualController : RcVirtualController
{
	[HideInInspector]
	public float Influence;

	public override void Turn()
	{
		GetVehicle().Turn(m_fWantedSteer - Influence, false);
	}
}
