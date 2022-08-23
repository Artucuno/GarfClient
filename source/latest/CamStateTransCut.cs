using UnityEngine;

public class CamStateTransCut : CamStateTransition
{
	public override ECamState state
	{
		get
		{
			return ECamState.TransCut;
		}
	}

	public override void Enter(Transform _Transform, Transform _Target)
	{
		base.Enter(_Transform, _Target);
		m_ToState.m_bDamping = false;
	}

	protected override bool Merge(float dt)
	{
		m_ToState.m_bDamping = true;
		return true;
	}
}
