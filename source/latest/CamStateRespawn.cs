using UnityEngine;

public class CamStateRespawn : CamState
{
	private Vector3 m_RespawnPos;

	private Quaternion m_RespawnOrient;

	public float Distance = 10f;

	public float Height = 5f;

	public float m_fCamLength = 0.5f;

	protected float m_fCurrTime;

	public override ECamState state
	{
		get
		{
			return ECamState.Respawn;
		}
	}

	public void Setup(Vector3 _RespawnPos, Quaternion _RespawnOrient)
	{
		m_RespawnPos = _RespawnPos;
		m_RespawnOrient = _RespawnOrient;
	}

	public override void Enter(Transform _Transform, Transform _Target)
	{
		base.Enter(_Transform, _Target);
		m_fCurrTime = m_fCamLength;
		base.m_Transform.position = m_RespawnPos;
		base.m_Transform.position -= m_RespawnOrient * Vector3.forward * Distance;
		base.m_Transform.position += m_RespawnOrient * Vector3.up * Height;
	}

	public override ECamState Manage(float dt)
	{
		base.m_Transform.LookAt(base.m_Target);
		m_fCurrTime -= dt;
		if (m_fCurrTime < 0f)
		{
			return ECamState.Follow;
		}
		return state;
	}

	public override void Exit()
	{
	}
}
