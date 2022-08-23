using UnityEngine;

public abstract class CamState : MonoBehaviour
{
	protected GameObject m_Dummy;

	[HideInInspector]
	public bool m_bDamping = true;

	public abstract ECamState state { get; }

	public Transform m_Transform { get; protected set; }

	public Transform m_Target { get; set; }

	public virtual void Enter(Transform _Transform, Transform _Target)
	{
		if (m_Dummy != null)
		{
			Object.Destroy(m_Dummy);
		}
		m_Dummy = new GameObject("DummyCam_" + state);
		m_Transform = m_Dummy.transform;
		if (_Transform != null)
		{
			m_Transform.position = _Transform.position;
			m_Transform.rotation = _Transform.rotation;
		}
		if (_Target != null)
		{
			m_Target = _Target;
		}
	}

	public virtual ECamState Manage(float dt)
	{
		return state;
	}

	public virtual void Exit()
	{
	}
}
