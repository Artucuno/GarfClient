public class VehicleChangedStateData
{
	public RcVehicle.eVehicleState m_State;

	public bool m_Active;

	public RcVehicle m_Vehicle;

	public VehicleChangedStateData(RcVehicle.eVehicleState _State, bool _Active, RcVehicle _Vehicle)
	{
		m_State = _State;
		m_Active = _Active;
		m_Vehicle = _Vehicle;
	}
}
