public class RcVirtualController : RcController
{
	protected float m_fSpeedBehaviour;

	protected float m_fWantedSteer;

	public int AIIndex;

	public RcVirtualController()
	{
		m_fSpeedBehaviour = 0f;
		m_fWantedSteer = 0f;
	}

	public override float GetSteer()
	{
		return m_fWantedSteer;
	}

	public override float GetSpeedBehaviour()
	{
		return m_fSpeedBehaviour;
	}

	public void setDriveParameters(float _wantedSteer, float _speedBehaviour)
	{
		m_fSpeedBehaviour = _speedBehaviour;
		m_fWantedSteer = _wantedSteer;
	}

	public void Update()
	{
		if (GetVehicle().IsAutoPilot() && GetVehicle().GetControlType() != RcVehicle.ControlType.Net && m_bDrivingEnabled)
		{
			Turn();
			if (m_fSpeedBehaviour >= 1f)
			{
				GetVehicle().Accelerate();
			}
			else if (m_fSpeedBehaviour < 0f)
			{
				GetVehicle().Brake();
			}
			else
			{
				GetVehicle().Decelerate();
			}
			GetVehicle().SetDrift(0f);
		}
	}

	public virtual void Turn()
	{
		GetVehicle().Turn(m_fWantedSteer, false);
	}

	public override void Reset()
	{
		m_fSpeedBehaviour = 0f;
		m_fWantedSteer = 0f;
	}

	public override void SetDrivingEnabled(bool _enabled)
	{
		base.SetDrivingEnabled(_enabled);
		if ((bool)GetVehicle())
		{
			GetVehicle().SetAutoPilot(_enabled);
		}
	}
}
