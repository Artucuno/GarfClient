using UnityEngine;

public class RcKinematicCollisionManager : MonoBehaviour
{
	protected RcKinematicPhysic[] vehicles;

	private RcKinematicCollisionManager()
	{
	}

	public void Start()
	{
		vehicles = (RcKinematicPhysic[])Object.FindObjectsOfType(typeof(RcKinematicPhysic));
	}

	public void FixedUpdate()
	{
		if (Singleton<GameConfigurator>.Instance.m_bMobilePlatform && (vehicles[0].m_iCountFixedUpdate & 3) == 0)
		{
			return;
		}
		RcKinematicPhysic[] array = vehicles;
		foreach (RcKinematicPhysic rcKinematicPhysic in array)
		{
			RcKinematicPhysic[] array2 = vehicles;
			foreach (RcKinematicPhysic rcKinematicPhysic2 in array2)
			{
				if (!(rcKinematicPhysic == rcKinematicPhysic2) && !(rcKinematicPhysic == null) && !(rcKinematicPhysic2 == null) && rcKinematicPhysic.Enable && rcKinematicPhysic2.Enable && !rcKinematicPhysic.IsLocked() && !rcKinematicPhysic2.IsLocked() && (rcKinematicPhysic.GetTransform().position - rcKinematicPhysic2.GetTransform().position).sqrMagnitude < 1f)
				{
					ManageVehicleContact(rcKinematicPhysic, rcKinematicPhysic2);
				}
			}
		}
	}

	protected void ManageVehicleContact(RcKinematicPhysic vehicle1, RcKinematicPhysic vehicle2)
	{
		Vector3 position = vehicle1.transform.position;
		Vector3 position2 = vehicle2.transform.position;
		Vector3 linearVelocity = vehicle1.GetLinearVelocity();
		Vector3 linearVelocity2 = vehicle2.GetLinearVelocity();
		float magnitude = linearVelocity.magnitude;
		float magnitude2 = linearVelocity2.magnitude;
		float num = 1f;
		float num2 = 1f;
		if (magnitude > 0f || magnitude2 > 0f)
		{
			num = 0.5f + magnitude * vehicle1.m_fMass / (magnitude * vehicle1.m_fMass + magnitude2 * vehicle2.m_fMass);
			num2 = 0.5f + magnitude2 * vehicle2.m_fMass / (magnitude * vehicle1.m_fMass + magnitude2 * vehicle2.m_fMass);
		}
		Vector3 vector = position - position2;
		float num3 = RcUtils.FastSqrtApprox(vector.sqrMagnitude);
		vector.Normalize();
		Vector3 vector2 = (vehicle1.m_fMass * linearVelocity + vehicle2.m_fMass * linearVelocity2) / (vehicle1.m_fMass + vehicle2.m_fMass);
		Vector3 vector3 = vector2;
		if (vector3.sqrMagnitude <= 0.0001f)
		{
			vector3 = Vector3.Cross(vector, Vector3.up);
		}
		bool flag = RcUtils.IsOnRight(position, position + vector3, position2);
		bool flag2 = !flag;
		Vector3 vector4 = Vector3.Cross(Vector3.up, vector3);
		vector4.Normalize();
		Vector3 vector5 = 1.3f * vector2;
		Vector3 vector6 = 1.3f * vector2;
		if (num < num2)
		{
			vector5 -= vehicle1.m_fVehicleCollBackPrc * vector2;
		}
		else
		{
			vector6 -= vehicle2.m_fVehicleCollBackPrc * vector2;
		}
		float magnitude3 = vector5.magnitude;
		vector5 += Vector3.up * vehicle1.m_fVehicleCollUpImpulseIntensity;
		vector5 += num2 * vehicle1.m_fVehicleCollSideImpulseIntensity * vector4 * ((!flag) ? 1f : (-1f));
		vector5.Normalize();
		vector5 *= magnitude3;
		if (magnitude3 <= 1f)
		{
			vector5 += vector * (1f - magnitude3);
		}
		float magnitude4 = vector6.magnitude;
		vector6 += Vector3.up * vehicle2.m_fVehicleCollUpImpulseIntensity;
		vector6 += num * vehicle2.m_fVehicleCollSideImpulseIntensity * vector4 * ((!flag2) ? 1f : (-1f));
		vector6.Normalize();
		vector6 *= magnitude4;
		if (magnitude4 <= 1f)
		{
			vector6 -= vector * (1f - magnitude4);
		}
		Vector3 impulse = vector5 - linearVelocity;
		Vector3 impulse2 = vector6 - linearVelocity2;
		Vector3 fVehicleCollInertiaDamping = vehicle1.m_fVehicleCollInertiaDamping;
		Vector3 fVehicleCollInertiaDamping2 = vehicle2.m_fVehicleCollInertiaDamping;
		if (!vehicle1.BInertiaMode && (Network.peerType == NetworkPeerType.Disconnected || vehicle1.transform.parent.gameObject.networkView.isMine))
		{
			vehicle1.SwitchToInertiaMode(vehicle1.m_fVehicleCollInertiaDelay, impulse, true, false, fVehicleCollInertiaDamping);
		}
		if (!vehicle2.BInertiaMode && (Network.peerType == NetworkPeerType.Disconnected || vehicle2.transform.parent.gameObject.networkView.isMine))
		{
			vehicle2.SwitchToInertiaMode(vehicle2.m_fVehicleCollInertiaDelay, impulse2, true, false, fVehicleCollInertiaDamping2);
		}
		CollisionData collisionInfos = default(CollisionData);
		collisionInfos.normal = vector;
		collisionInfos.solid = vehicle1.GetVehicleBody();
		collisionInfos.other = vehicle2.GetVehicleBody();
		collisionInfos.surface = vehicle2.gameObject.layer;
		collisionInfos.position = (position + position2) / 2f;
		collisionInfos.depth = 1f - num3;
		vehicle1.FireOnCollision(collisionInfos);
		collisionInfos.solid = vehicle2.GetVehicleBody();
		collisionInfos.other = vehicle1.GetVehicleBody();
		collisionInfos.surface = vehicle1.gameObject.layer;
		vehicle2.FireOnCollision(collisionInfos);
	}
}
