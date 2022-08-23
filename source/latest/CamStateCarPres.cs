using UnityEngine;

public class CamStateCarPres : CamState
{
	private GameObject m_Start;

	public float StartDistance = 1f;

	public float Height = 1f;

	public float Speed = 1f;

	public override ECamState state
	{
		get
		{
			return ECamState.CarPres;
		}
	}

	public override void Enter(Transform _Transform, Transform _Target)
	{
		base.Enter(_Transform, _Target);
		m_Start = GameObject.Find("Start");
		base.m_Transform.position = m_Start.transform.position;
		base.m_Transform.rotation = m_Start.transform.rotation;
		base.m_Transform.position += base.m_Transform.up * Height + base.m_Transform.forward * StartDistance;
		base.m_Transform.LookAt(base.m_Transform.position - base.m_Transform.forward);
	}

	public override ECamState Manage(float dt)
	{
		base.m_Transform.position += base.m_Transform.forward * dt * Speed;
		return state;
	}

	public override void Exit()
	{
	}
}
