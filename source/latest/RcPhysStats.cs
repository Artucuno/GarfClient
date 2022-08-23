using UnityEngine;

public struct RcPhysStats
{
	public float fTime;

	public Vector3 vPos;

	public Vector3 vCombineMv;

	public Quaternion vOrient;

	public bool bFlying;

	public bool bInertiaMode;

	public RcWheelStats[] Wheel;

	public float fWheelSpeed;

	public float fWheelSpeedMs;

	public float fTurningRadius;
}
