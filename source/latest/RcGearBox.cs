using UnityEngine;

public abstract class RcGearBox : MonoBehaviour
{
	public virtual bool IsGoingTooFast()
	{
		return false;
	}

	public abstract float ComputeAcceleration(float _speedMS);

	public abstract float GetMaxSpeed();

	public abstract int GetCurrentGear();

	public abstract float GetBackwardMaxSpeed();

	public abstract float ComputeRpm(float speedMs);
}
