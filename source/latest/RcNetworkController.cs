using UnityEngine;

public class RcNetworkController : RcController
{
	public struct Packet
	{
		public double time;

		public Vector3 pos;

		public Vector3 vel;

		public Quaternion quat;

		public Vector3 omega;

		public float leftRightFactor;

		public float FwdBkdFactor;

		public float engineVel;

		public float drift;
	}

	public double m_dInterpBkTime;

	public double m_dMaxExtrap;

	protected int m_iTimestampCount;

	protected Packet[] m_BufferedState = new Packet[20];

	protected RcVehiclePhysic m_pPhysic;

	public RcNetworkController()
	{
		m_dInterpBkTime = 0.12;
		m_dMaxExtrap = 0.55;
		m_iTimestampCount = -1;
		m_pPhysic = null;
	}

	public override void Awake()
	{
		m_pVehicle = GetComponentInChildren<RcVehicle>();
		m_pPhysic = GetComponentInChildren<RcVehiclePhysic>();
		base.networkView.observed = this;
	}

	private void OnNetworkInstantiate(NetworkMessageInfo info)
	{
		if (!base.networkView.isMine)
		{
			m_pVehicle.SetControlType(RcVehicle.ControlType.Net);
		}
	}

	private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if (base.networkView.stateSynchronization == NetworkStateSynchronization.ReliableDeltaCompressed)
		{
			OnSerializeNetworkViewReliable(stream, info);
		}
		else
		{
			OnSerializeNetworkViewUnreliable(stream, info);
		}
	}

	private void OnSerializeNetworkViewReliable(BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting)
		{
			Vector3 value = base.rigidbody.position;
			Quaternion value2 = base.rigidbody.rotation;
			Vector3 value3 = m_pPhysic.GetLinearVelocity();
			Vector3 value4 = m_pPhysic.GetAngularVelocity();
			stream.Serialize(ref value);
			stream.Serialize(ref value3);
			stream.Serialize(ref value2);
			stream.Serialize(ref value4);
			float value5 = m_pVehicle.GetSteeringFactor();
			float value6 = m_pVehicle.GetMoveFactor();
			stream.Serialize(ref value5);
			stream.Serialize(ref value6);
			float value7 = m_pVehicle.GetWheelSpeedMS();
			float value8 = m_pVehicle.GetArcadeDriftFactor();
			stream.Serialize(ref value7);
			stream.Serialize(ref value8);
		}
		else if (m_pVehicle.GetControlType() == RcVehicle.ControlType.Net)
		{
			Vector3 value9 = Vector3.zero;
			Vector3 value10 = Vector3.zero;
			Quaternion value11 = Quaternion.identity;
			Vector3 value12 = Vector3.zero;
			float value13 = 0f;
			float value14 = 0f;
			stream.Serialize(ref value9);
			stream.Serialize(ref value10);
			stream.Serialize(ref value11);
			stream.Serialize(ref value12);
			stream.Serialize(ref value13);
			stream.Serialize(ref value14);
			float value15 = 0f;
			float value16 = 0f;
			stream.Serialize(ref value15);
			stream.Serialize(ref value16);
			for (int num = m_BufferedState.Length - 1; num >= 1; num--)
			{
				m_BufferedState[num] = m_BufferedState[num - 1];
			}
			Packet packet = default(Packet);
			packet.time = info.timestamp;
			packet.pos = value9;
			packet.vel = value10;
			packet.quat = value11;
			packet.omega = value12;
			packet.leftRightFactor = value13;
			packet.FwdBkdFactor = value14;
			packet.engineVel = value15;
			packet.drift = value16;
			m_BufferedState[0] = packet;
			m_iTimestampCount = Mathf.Min(m_iTimestampCount + 1, m_BufferedState.Length);
		}
	}

	private void OnSerializeNetworkViewUnreliable(BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting && m_pVehicle.GetControlType() != RcVehicle.ControlType.Net)
		{
			Vector3 position = base.rigidbody.position;
			Quaternion rotation = base.rigidbody.rotation;
			Vector3 linearVelocity = m_pPhysic.GetLinearVelocity();
			Vector3 angularVelocity = m_pPhysic.GetAngularVelocity();
			RcUtils.SerializeAndCompressVector(stream, position, new Vector3(1000f, 200f, 100f));
			RcUtils.SerializeAndCompressVector(stream, linearVelocity, Vector3.one * 300f);
			RcUtils.SerializeAndCompressQuaternion(stream, rotation);
			RcUtils.SerializeAndCompressVector(stream, angularVelocity, Vector3.one * 100f);
			float steeringFactor = m_pVehicle.GetSteeringFactor();
			float moveFactor = m_pVehicle.GetMoveFactor();
			short value = RcUtils.CompressFloat(steeringFactor, -1f, 1f);
			short value2 = RcUtils.CompressFloat(moveFactor, -1f, 2f);
			stream.Serialize(ref value);
			stream.Serialize(ref value2);
			float wheelSpeedMS = m_pVehicle.GetWheelSpeedMS();
			float arcadeDriftFactor = m_pVehicle.GetArcadeDriftFactor();
			short value3 = RcUtils.CompressFloat(wheelSpeedMS, -300f, 300f);
			short value4 = RcUtils.CompressFloat(arcadeDriftFactor, -1f, 1f);
			stream.Serialize(ref value3);
			stream.Serialize(ref value4);
		}
		else if (m_pVehicle.GetControlType() == RcVehicle.ControlType.Net)
		{
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			Quaternion identity = Quaternion.identity;
			Vector3 zero3 = Vector3.zero;
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			zero = RcUtils.UnserializeCompressedVector(stream, new Vector3(1000f, 200f, 100f));
			zero2 = RcUtils.UnserializeCompressedVector(stream, Vector3.one * 300f);
			identity = RcUtils.UnserializeCompressedQuaternion(stream);
			zero3 = RcUtils.UnserializeCompressedVector(stream, Vector3.one * 100f);
			short value5 = 0;
			short value6 = 0;
			stream.Serialize(ref value5);
			stream.Serialize(ref value6);
			num = RcUtils.DecompressFloat(value5, -1f, 1f);
			num2 = RcUtils.DecompressFloat(value6, -1f, 2f);
			short value7 = 0;
			short value8 = 0;
			stream.Serialize(ref value7);
			stream.Serialize(ref value8);
			num3 = RcUtils.DecompressFloat(value7, -300f, 300f);
			num4 = RcUtils.DecompressFloat(value8, -1f, 1f);
			for (int num5 = m_BufferedState.Length - 1; num5 >= 1; num5--)
			{
				m_BufferedState[num5] = m_BufferedState[num5 - 1];
			}
			Packet packet = default(Packet);
			packet.time = info.timestamp;
			packet.pos = zero;
			packet.vel = zero2;
			packet.quat = identity;
			packet.omega = zero3;
			packet.leftRightFactor = num;
			packet.FwdBkdFactor = num2;
			packet.engineVel = num3;
			packet.drift = num4;
			m_BufferedState[0] = packet;
			m_iTimestampCount = Mathf.Min(m_iTimestampCount + 1, m_BufferedState.Length);
		}
	}

	public void FixedUpdate()
	{
		if (m_pVehicle.GetControlType() != RcVehicle.ControlType.Net)
		{
			return;
		}
		double num = Network.time - m_dInterpBkTime;
		if (m_BufferedState[0].time > num)
		{
			for (int i = 0; i < m_iTimestampCount; i++)
			{
				if (m_BufferedState[i].time <= num || i == m_iTimestampCount - 1)
				{
					Packet packet = m_BufferedState[Mathf.Max(i - 1, 0)];
					Packet packet2 = m_BufferedState[i];
					double num2 = packet.time - packet2.time;
					float num3 = 0f;
					if (num2 > 0.0001)
					{
						num3 = (float)((num - packet2.time) / num2);
					}
					m_pPhysic.NetMove(Vector3.Lerp(packet2.pos, packet.pos, num3), Quaternion.Slerp(packet2.quat, packet.quat, num3), Vector3.Lerp(packet2.vel, packet.vel, num3), Vector3.zero);
					float steering = RcUtils.LinearInterpolation(0f, packet2.leftRightFactor, 1f, packet.leftRightFactor, num3, true);
					float accelerationPrc = RcUtils.LinearInterpolation(0f, packet2.FwdBkdFactor, 1f, packet.FwdBkdFactor, num3, true);
					float wheelSpeedMS = RcUtils.LinearInterpolation(0f, packet2.engineVel, 1f, packet.engineVel, num3, true);
					float arcadeDriftFactor = RcUtils.LinearInterpolation(0f, packet2.drift, 1f, packet.drift, num3, true);
					m_pVehicle.SetWheelSpeedMS(wheelSpeedMS);
					m_pVehicle.SetArcadeDriftFactor(arcadeDriftFactor);
					m_pVehicle.Accelerate(accelerationPrc);
					m_pVehicle.Turn(steering, false);
					break;
				}
			}
		}
		else
		{
			Packet packet3 = m_BufferedState[0];
			float num4 = (float)(num - packet3.time);
			if ((double)num4 < m_dMaxExtrap)
			{
				float angle = num4 * packet3.omega.magnitude * 57.29578f;
				Quaternion quaternion = Quaternion.AngleAxis(angle, packet3.omega);
				m_pPhysic.NetMove(packet3.pos + packet3.vel * num4, quaternion * packet3.quat, packet3.vel, packet3.omega);
				m_pPhysic.SetLinearVelocity(packet3.vel);
				m_pPhysic.SetAngularVelocity(packet3.omega);
				float leftRightFactor = packet3.leftRightFactor;
				float fwdBkdFactor = packet3.FwdBkdFactor;
				m_pVehicle.SetWheelSpeedMS(packet3.engineVel);
				m_pVehicle.SetArcadeDriftFactor(packet3.drift);
				m_pVehicle.Accelerate(fwdBkdFactor);
				m_pVehicle.Turn(leftRightFactor, false);
			}
		}
	}
}
