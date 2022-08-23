using UnityEngine;

public class RcController : MonoBehaviour
{
	protected RcVehicle m_pVehicle;

	protected bool m_bDrivingEnabled;

	public RcController()
	{
		m_pVehicle = null;
		m_bDrivingEnabled = true;
	}

	public virtual void SetVehicle(RcVehicle _pVehicle)
	{
		m_pVehicle = _pVehicle;
	}

	public virtual float GetSteer()
	{
		return 0f;
	}

	public virtual float GetSpeedBehaviour()
	{
		return 0f;
	}

	public virtual void StartRace()
	{
	}

	public virtual void Reset()
	{
	}

	public virtual void SetDrivingEnabled(bool _enabled)
	{
		m_bDrivingEnabled = _enabled;
	}

	public bool IsDrivingEnabled()
	{
		return m_bDrivingEnabled;
	}

	public RcVehicle GetVehicle()
	{
		return m_pVehicle;
	}

	public virtual void Awake()
	{
		m_pVehicle = base.transform.parent.GetComponentInChildren<RcVehicle>();
	}
}
