using UnityEngine;

public struct GroundCharac
{
	public Vector3 point;

	public Vector3 normal;

	public int surface;

	public void reset()
	{
		point = Vector3.zero;
		normal = Vector3.zero;
		surface = 0;
	}
}
