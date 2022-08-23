using UnityEngine;

public class UFOAnimation : MonoBehaviour
{
	private UFO m_pParent;

	private void Start()
	{
		m_pParent = base.transform.parent.GetComponent<UFO>();
	}

	public void LeaveFinished()
	{
		if ((bool)m_pParent)
		{
			m_pParent.Leave();
		}
	}

	public void LaunchFinished()
	{
		if ((bool)m_pParent)
		{
			m_pParent.Idle();
		}
	}
}
