using UnityEngine;

public class CamStateKill : CamState
{
	public override ECamState state
	{
		get
		{
			return ECamState.Kill;
		}
	}

	public override void Enter(Transform _Transform, Transform _Target)
	{
		base.Enter(_Transform, _Target);
	}

	public override ECamState Manage(float dt)
	{
		base.m_Transform.LookAt(base.m_Target);
		return state;
	}

	public override void Exit()
	{
	}
}
