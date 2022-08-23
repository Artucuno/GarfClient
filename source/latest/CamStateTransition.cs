using UnityEngine;

public abstract class CamStateTransition : CamState
{
	protected CamState m_FromState;

	protected CamState m_ToState;

	private bool m_bFreezeFrom;

	public void Setup(CamState _From, CamState _To, bool _bFreezeFrom)
	{
		m_FromState = _From;
		m_ToState = _To;
		m_bFreezeFrom = _bFreezeFrom;
	}

	public override void Enter(Transform _Transform, Transform _Target)
	{
		base.Enter(_Transform, _Target);
		m_ToState.Enter(_Transform, _Target);
	}

	public override ECamState Manage(float dt)
	{
		if (m_FromState != null && !m_bFreezeFrom)
		{
			m_FromState.Manage(dt);
		}
		if (m_ToState != null)
		{
			m_ToState.Manage(dt);
		}
		if (Merge(dt))
		{
			return m_ToState.state;
		}
		return state;
	}

	protected abstract bool Merge(float dt);
}
