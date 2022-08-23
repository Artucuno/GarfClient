using UnityEngine;

public struct CollisionData
{
	public Rigidbody solid;

	public Rigidbody other;

	public Vector3 position;

	public Vector3 normal;

	public float depth;

	public int surface;

	public void Reset()
	{
		solid = null;
		other = null;
		position = Vector3.zero;
		normal = Vector3.zero;
		depth = 0f;
		surface = 0;
	}
}
