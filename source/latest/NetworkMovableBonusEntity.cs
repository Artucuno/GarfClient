using UnityEngine;

public class NetworkMovableBonusEntity : NetworkBonusEntity
{
	public struct Packet
	{
		public double time;

		public Vector3 pos;

		public Vector3 vel;

		public Quaternion rot;
	}

	public double m_dInterpBkTime;

	public double m_dMaxExtrap;

	protected int m_iTimestampCount;

	protected Packet[] m_BufferedState = new Packet[20];

	public NetworkMovableBonusEntity()
	{
		m_dInterpBkTime = 0.12;
		m_dMaxExtrap = 0.55;
		m_iTimestampCount = -1;
	}

	public virtual void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		m_pBonusEntity.OnSerializeNetworkView(stream, info);
		if (!m_pBonusEntity.BSynchronizePosition || !m_pBonusEntity.Activate)
		{
			return;
		}
		if (stream.isWriting)
		{
			Vector3 value = m_pBonusEntity.rigidbody.position;
			Vector3 value2 = m_pBonusEntity.rigidbody.velocity;
			stream.Serialize(ref value);
			stream.Serialize(ref value2);
			if (m_pBonusEntity.BSynchronizeRotation)
			{
				Quaternion value3 = m_pBonusEntity.rigidbody.rotation;
				stream.Serialize(ref value3);
			}
		}
		else if (!base.networkView.isMine)
		{
			Vector3 value4 = Vector3.zero;
			Vector3 value5 = Vector3.zero;
			Quaternion value6 = Quaternion.identity;
			stream.Serialize(ref value4);
			stream.Serialize(ref value5);
			if (m_pBonusEntity.BSynchronizeRotation)
			{
				stream.Serialize(ref value6);
			}
			for (int num = m_BufferedState.Length - 1; num >= 1; num--)
			{
				m_BufferedState[num] = m_BufferedState[num - 1];
			}
			Packet packet = default(Packet);
			packet.time = info.timestamp;
			packet.pos = value4;
			packet.vel = value5;
			packet.rot = value6;
			m_BufferedState[0] = packet;
			m_iTimestampCount = Mathf.Min(m_iTimestampCount + 1, m_BufferedState.Length);
		}
	}

	public void FixedUpdate()
	{
		if (base.networkView.isMine || !m_pBonusEntity.BSynchronizePosition || !m_pBonusEntity.Activate)
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
					float t = 0f;
					if (num2 > 0.0001)
					{
						t = (float)((num - packet2.time) / num2);
					}
					m_pBonusEntity.rigidbody.position = Vector3.Lerp(packet2.pos, packet.pos, t);
					if (m_pBonusEntity.BSynchronizeRotation)
					{
						m_pBonusEntity.rigidbody.rotation = Quaternion.Lerp(packet2.rot, packet.rot, t);
					}
					break;
				}
			}
			return;
		}
		Packet packet3 = m_BufferedState[0];
		float num3 = (float)(num - packet3.time);
		if ((double)num3 < m_dMaxExtrap)
		{
			m_pBonusEntity.rigidbody.position = packet3.pos + packet3.vel * num3;
			if (m_pBonusEntity.BSynchronizeRotation)
			{
				m_pBonusEntity.rigidbody.rotation = packet3.rot;
			}
		}
	}

	[RPC]
	public virtual void Launch(NetworkViewID launcherViewID)
	{
		NetworkInitialize(launcherViewID);
		m_pBonusEntity.Launch();
	}
}
